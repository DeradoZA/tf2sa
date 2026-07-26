using System.Data.Common;
using System.Globalization;
using System.Reflection;
using DuckDB.NET.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Data.Analytics;

/// <summary>
/// DuckDB-backed store (ADR 0008, ADR 0009). Schema.sql creates the raw tables and
/// sequences (idempotent — a no-op once they exist); Leaderboards.sql/PlayerDetail.sql
/// install the read macros. Ingestion writes land directly in the raw tables via
/// InsertGameAsync/UpsertPlayersAsync/UpdatePlayersAsync, so a write is immediately
/// visible to every read — there is no refresh step and no second database. One
/// connection is shared and access is serialized (a semaphore, not connection
/// pooling), which suits this scale and incidentally makes the player-upsert race a
/// concurrent EF ingestion used to hit impossible: every write, from every log, now
/// goes through the same single queue.
/// </summary>
public class DuckDbAnalyticsStore : IAnalyticsStore, IDisposable
{
	private readonly ILogger<DuckDbAnalyticsStore> logger;
	private readonly string duckDbPath;
	private readonly bool logDatabaseQueries;
	private readonly SemaphoreSlim gate = new(1, 1);
	private DuckDBConnection? connection;

	public bool Enabled { get; }

	public DuckDbAnalyticsStore(
		IConfiguration configuration,
		ILogger<DuckDbAnalyticsStore> logger
	)
	{
		this.logger = logger;

		Enabled =
			bool.TryParse(
				configuration["TF2SA_ANALYTICS_ENABLED"],
				out bool enabled
			) && enabled;

		// Debug:LogDatabaseQueries defaults to false (appsettings.json) and true
		// locally (appsettings.Development.json): logs every DuckDB SQL statement
		// and its result. Off in production.
		logDatabaseQueries = configuration.GetValue<bool>(
			"Debug:LogDatabaseQueries"
		);

		duckDbPath =
			configuration["TF2SA_DUCKDB_PATH"]
			?? Path.Combine(Path.GetTempPath(), "tf2sa-analytics.duckdb");
	}

	// ---- writes -------------------------------------------------------------

	public async Task<OptionStrict<Error>> InsertGameAsync(
		GameWrite game,
		CancellationToken cancellationToken
	)
	{
		await gate.WaitAsync(cancellationToken);
		try
		{
			DuckDBConnection conn = Connection();
			DbTransaction transaction = conn.BeginTransaction();
			try
			{
				InsertGameRow(conn, game);
				foreach (PlayerStatWrite playerStat in game.PlayerStats)
				{
					long playerStatsId = NextVal(conn, "seq_playerstats");
					InsertPlayerStatRow(
						conn,
						game.GameId,
						playerStatsId,
						playerStat
					);
					foreach (ClassStatWrite classStat in playerStat.ClassStats)
					{
						long classStatsId = NextVal(conn, "seq_classstats");
						InsertClassStatRow(
							conn,
							playerStatsId,
							classStatsId,
							classStat
						);
						foreach (
							WeaponStatWrite weaponStat in classStat.WeaponStats
						)
						{
							long weaponStatsId = NextVal(
								conn,
								"seq_weaponstats"
							);
							InsertWeaponStatRow(
								conn,
								classStatsId,
								weaponStatsId,
								weaponStat
							);
						}
					}
				}
				transaction.Commit();
				return OptionStrict<Error>.Nothing;
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to insert game {game.GameId}: {e.Message}"
			);
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<OptionStrict<Error>> UpsertPlayersAsync(
		IEnumerable<PlayerWrite> players,
		CancellationToken cancellationToken
	)
	{
		await gate.WaitAsync(cancellationToken);
		try
		{
			DuckDBConnection conn = Connection();
			foreach (PlayerWrite player in players)
			{
				ExecuteParameterized(
					conn,
					@"INSERT INTO Players
						(SteamID, PlayerName, LocalCountryCode, ProfileUrl, Avatar,
						 AvatarMedium, AvatarFull, AvatarHash, RealName)
					  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
					  ON CONFLICT (SteamID) DO NOTHING",
					new object?[]
					{
						player.SteamId,
						player.PlayerName,
						player.LocalCountryCode,
						player.ProfileUrl,
						player.Avatar,
						player.AvatarMedium,
						player.AvatarFull,
						player.AvatarHash,
						player.RealName,
					}
				);
			}
			return OptionStrict<Error>.Nothing;
		}
		catch (Exception e)
		{
			return new DatabaseError($"Failed to upsert players: {e.Message}");
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<OptionStrict<Error>> UpdatePlayersAsync(
		IEnumerable<PlayerWrite> updatedPlayers,
		CancellationToken cancellationToken
	)
	{
		await gate.WaitAsync(cancellationToken);
		try
		{
			DuckDBConnection conn = Connection();

			// First-match-wins on a duplicate SteamId, mirroring the .FirstOrDefault()
			// the EF method this replaces used — never throws on a duplicate key.
			Dictionary<ulong, PlayerWrite> updatedBySteamId = updatedPlayers
				.GroupBy(p => p.SteamId)
				.ToDictionary(g => g.Key, g => g.First());

			PropertyInfo[] idProps = typeof(PlayerIdRow).GetProperties(
				BindingFlags.Public | BindingFlags.Instance
			);
			List<PlayerIdRow> allPlayers = Query<PlayerIdRow>(
				conn,
				"SELECT SteamID FROM Players",
				idProps
			);

			foreach (PlayerIdRow player in allPlayers)
			{
				updatedBySteamId.TryGetValue(
					player.SteamId,
					out PlayerWrite? updated
				);

				// Faithfully preserves PlayersRepository.UpdatePlayers' exact
				// behaviour (see known-issues.md): a player absent from this batch
				// has every profile column nulled out, and AvatarHash is set from
				// the incoming record's Avatar — not its AvatarHash — a pre-existing
				// copy-paste bug, not something this port introduced.
				ExecuteParameterized(
					conn,
					@"UPDATE Players SET
						Avatar = ?, AvatarFull = ?, AvatarHash = ?, AvatarMedium = ?,
						LocalCountryCode = ?, PlayerName = ?, RealName = ?, ProfileUrl = ?
					  WHERE SteamID = ?",
					new object?[]
					{
						updated?.Avatar,
						updated?.AvatarFull,
						updated?.Avatar,
						updated?.AvatarMedium,
						updated?.LocalCountryCode,
						updated?.PlayerName,
						updated?.RealName,
						updated?.ProfileUrl,
						player.SteamId,
					}
				);
			}
			return OptionStrict<Error>.Nothing;
		}
		catch (Exception e)
		{
			return new DatabaseError($"Error updating players: {e.Message}");
		}
		finally
		{
			gate.Release();
		}
	}

	private static void InsertGameRow(DuckDBConnection conn, GameWrite g) =>
		ExecuteParameterized(
			conn,
			@"INSERT INTO Games
				(GameID, IsValidStats, InvalidStatsReason, Version, RedScore, BlueScore,
				 Duration, Map, IsSupplemental, HasRealDamage, HasWeaponDamage, HasAccuracy,
				 HasHP, HasHPReal, HasHeadshots, HasHeadshotsHit, HasBackstabs,
				 HasCapturePointsCaptured, HasSentriesBuilt, HasDamageTaken, HasAirshots,
				 HasHealsReceived, HasIntelCaptures, HasADScoring, Notifications, Title,
				 Date, UploaderID, UploaderName, UploaderInfo, Success)
			  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
			new object?[]
			{
				g.GameId,
				g.IsValidStats,
				g.InvalidStatsReason,
				g.Version,
				g.RedScore,
				g.BlueScore,
				g.Duration,
				g.Map,
				g.IsSupplemental,
				g.HasRealDamage,
				g.HasWeaponDamage,
				g.HasAccuracy,
				g.HasHp,
				g.HasHpReal,
				g.HasHeadshots,
				g.HasHeadshotsHit,
				g.HasBackstabs,
				g.HasCapturePointsCaptured,
				g.HasSentriesBuilt,
				g.HasDamageTaken,
				g.HasAirshots,
				g.HasHealsReceived,
				g.HasIntelCaptures,
				g.HasAdScoring,
				g.Notifications,
				g.Title,
				g.Date,
				g.UploaderId,
				g.UploaderName,
				g.UploaderInfo,
				g.Success,
			}
		);

	private static void InsertPlayerStatRow(
		DuckDBConnection conn,
		uint gameId,
		long playerStatsId,
		PlayerStatWrite ps
	) =>
		ExecuteParameterized(
			conn,
			@"INSERT INTO PlayerStats
				(PlayerStatsID, GameID, SteamID, TeamID, DamageTaken, HealsReceived,
				 LongestKillStreak, Airshots, Ubers, Drops, Medkits, MedkitsHP, Backstabs,
				 Headshots, HeadshotsHit, SentriesBuilt, Heals, CapturePointsCaptured, IntelCaptures)
			  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
			new object?[]
			{
				playerStatsId,
				gameId,
				ps.SteamId,
				ps.TeamId,
				ps.DamageTaken,
				ps.HealsReceived,
				ps.LongestKillStreak,
				ps.Airshots,
				ps.Ubers,
				ps.Drops,
				ps.Medkits,
				ps.MedkitsHp,
				ps.Backstabs,
				ps.Headshots,
				ps.HeadshotsHit,
				ps.SentriesBuilt,
				ps.Heals,
				ps.CapturePointsCaptured,
				ps.IntelCaptures,
			}
		);

	private static void InsertClassStatRow(
		DuckDBConnection conn,
		long playerStatsId,
		long classStatsId,
		ClassStatWrite cs
	) =>
		ExecuteParameterized(
			conn,
			@"INSERT INTO ClassStats
				(ClassStatsID, PlayerStatsID, ClassID, Kills, Assists, Deaths, Damage, Playtime)
			  VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
			new object?[]
			{
				classStatsId,
				playerStatsId,
				cs.ClassId,
				cs.Kills,
				cs.Assists,
				cs.Deaths,
				cs.Damage,
				cs.Playtime,
			}
		);

	private static void InsertWeaponStatRow(
		DuckDBConnection conn,
		long classStatsId,
		long weaponStatsId,
		WeaponStatWrite ws
	) =>
		ExecuteParameterized(
			conn,
			@"INSERT INTO WeaponStats
				(WeaponStatsID, ClassStatsID, WeaponName, Kills, Damage, Shots, Hits)
			  VALUES (?, ?, ?, ?, ?, ?, ?)",
			new object?[]
			{
				weaponStatsId,
				classStatsId,
				ws.WeaponName,
				ws.Kills,
				ws.Damage,
				ws.Shots,
				ws.Hits,
			}
		);

	// ---- paged reads ------------------------------------------------------

	public async Task<StatsPage<T>> GetPageAsync<T>(
		string boardCall,
		string defaultSortKey,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		int offset,
		int count,
		CancellationToken cancellationToken
	)
		where T : class, new()
	{
		PropertyInfo[] props = typeof(T).GetProperties(
			BindingFlags.Public | BindingFlags.Instance
		);

		// Whitelisted sort: only a real property is orderable (DuckDB resolves the
		// identifier case-insensitively to the board column). Fall back to default.
		PropertyInfo? sortProp = props.FirstOrDefault(p =>
			p.Name.Equals(sort, StringComparison.OrdinalIgnoreCase)
		);
		string sortUsed = sortProp is not null ? sort : defaultSortKey;
		string sortColumn = (
			sortProp
			?? props.First(p =>
				p.Name.Equals(
					defaultSortKey,
					StringComparison.OrdinalIgnoreCase
				)
			)
		).Name;
		string direction = sortOrder.Equals(
			"asc",
			StringComparison.OrdinalIgnoreCase
		)
			? "ASC"
			: "DESC";

		// Whitelisted filter: only a string property (e.g. PlayerName) is filterable.
		PropertyInfo? filterProp = props.FirstOrDefault(p =>
			p.Name.Equals(filterField, StringComparison.OrdinalIgnoreCase)
			&& p.PropertyType == typeof(string)
		);
		bool applyFilter =
			filterProp is not null && !string.IsNullOrWhiteSpace(filterValue);
		string where = applyFilter
			? $"WHERE {filterProp!.Name} ILIKE '%{EscapeLike(filterValue)}%' ESCAPE '\\'"
			: string.Empty;

		int take = Math.Max(count, 0);
		int skip = Math.Max(offset, 0);

		await gate.WaitAsync(cancellationToken);
		try
		{
			DuckDBConnection conn = Connection();
			int total = (int)ScalarLong(
				conn,
				$"SELECT COUNT(*) FROM {boardCall} {where}"
			);
			List<T> rows = Query<T>(
				conn,
				$"SELECT * FROM {boardCall} {where} "
					+ $"ORDER BY {sortColumn} {direction} LIMIT {take} OFFSET {skip}",
				props
			);
			return new StatsPage<T>
			{
				Rows = rows,
				Total = total,
				SortUsed = sortUsed,
				SortOrderUsed = direction.ToLowerInvariant(),
				FilterFieldUsed = applyFilter ? filterField : string.Empty,
				FilterValueUsed = applyFilter ? filterValue : string.Empty,
			};
		}
		finally
		{
			gate.Release();
		}
	}

	// ---- whole-view reads -------------------------------------------------

	public async Task<IReadOnlyList<T>> GetAllAsync<T>(
		string viewCall,
		CancellationToken cancellationToken
	)
		where T : class, new()
	{
		PropertyInfo[] props = typeof(T).GetProperties(
			BindingFlags.Public | BindingFlags.Instance
		);

		await gate.WaitAsync(cancellationToken);
		try
		{
			return Query<T>(Connection(), $"SELECT * FROM {viewCall}", props);
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<T?> GetSingleAsync<T>(
		string viewCall,
		CancellationToken cancellationToken
	)
		where T : class, new()
	{
		IReadOnlyList<T> rows = await GetAllAsync<T>(
			viewCall,
			cancellationToken
		);
		return rows.Count > 0 ? rows[0] : null;
	}

	// ---- helpers ----------------------------------------------------------

	private DuckDBConnection Connection()
	{
		if (connection is null)
		{
			connection = new DuckDBConnection($"Data Source={duckDbPath}");
			connection.Open();

			// Idempotent: creates the raw tables/sequences on a fresh file, no-ops
			// against an existing one. Then (re)install the macros — this also
			// covers a deploy that changed their SQL taking effect on first use,
			// without any refresh cycle to wait for (there is no refresh anymore).
			Execute(connection, LoadEmbeddedSql("Schema.sql"));
			LoadViews(connection);
		}
		return connection;
	}

	private void LoadViews(DuckDBConnection conn)
	{
		Execute(conn, LoadEmbeddedSql("Leaderboards.sql"));
		Execute(conn, LoadEmbeddedSql("PlayerDetail.sql"));
	}

	private List<T> Query<T>(
		DuckDBConnection conn,
		string sql,
		PropertyInfo[] props
	)
		where T : new()
	{
		if (logDatabaseQueries)
		{
			logger.LogInformation("[DuckDB] {Sql}", sql);
		}

		Dictionary<string, PropertyInfo> byName = props
			.Where(p => p.CanWrite)
			.ToDictionary(p => p.Name.ToLowerInvariant());

		using DuckDBCommand command = conn.CreateCommand();
		command.CommandText = sql;
		using DbDataReader reader = command.ExecuteReader();

		List<T> list = new();
		while (reader.Read())
		{
			T row = new();
			List<string>? rowValues = logDatabaseQueries ? new() : null;
			for (int i = 0; i < reader.FieldCount; i++)
			{
				object? value = reader.IsDBNull(i) ? null : reader.GetValue(i);
				rowValues?.Add($"{reader.GetName(i)}={value ?? "NULL"}");
				if (
					byName.TryGetValue(
						reader.GetName(i).ToLowerInvariant(),
						out PropertyInfo? prop
					)
				)
				{
					prop.SetValue(row, Convert(value, prop.PropertyType));
				}
			}
			if (rowValues is not null)
			{
				logger.LogInformation(
					"[DuckDB] -> {Row}",
					string.Join(", ", rowValues)
				);
			}
			list.Add(row);
		}
		if (logDatabaseQueries)
		{
			logger.LogInformation(
				"[DuckDB] -> {Count} row(s) total",
				list.Count
			);
		}
		return list;
	}

	private static object? Convert(object? value, Type targetType)
	{
		if (value is null or DBNull)
		{
			return null;
		}
		// DuckDB returns HUGEINT (e.g. SUM/max of integers) as BigInteger, which is
		// not IConvertible; normalise it to a long that ChangeType can handle.
		if (value is System.Numerics.BigInteger big)
		{
			value = (long)big;
		}
		Type target = Nullable.GetUnderlyingType(targetType) ?? targetType;
		return target.IsInstanceOfType(value)
			? value
			: System.Convert.ChangeType(
				value,
				target,
				CultureInfo.InvariantCulture
			);
	}

	private void Execute(DuckDBConnection conn, string sql)
	{
		if (logDatabaseQueries)
		{
			logger.LogInformation("[DuckDB] {Sql}", sql);
		}
		using DuckDBCommand command = conn.CreateCommand();
		command.CommandText = sql;
		int rowsAffected = command.ExecuteNonQuery();
		if (logDatabaseQueries)
		{
			logger.LogInformation(
				"[DuckDB] -> {Rows} row(s) affected",
				rowsAffected
			);
		}
	}

	/// <summary>
	/// Executes a parameterized statement — always used for writes, never string
	/// interpolation, since (unlike the read-side macros, which only interpolate
	/// internally-whitelisted identifiers) this persists real user-influenced text:
	/// player names, map names, log titles. Positional <c>?</c> placeholders, bound
	/// in argument order.
	/// </summary>
	private static void ExecuteParameterized(
		DuckDBConnection conn,
		string sql,
		object?[] parameters
	)
	{
		using DuckDBCommand command = conn.CreateCommand();
		command.CommandText = sql;
		foreach (object? value in parameters)
		{
			command.Parameters.Add(new DuckDBParameter(value ?? DBNull.Value));
		}
		command.ExecuteNonQuery();
	}

	/// <summary>
	/// Fetches the next value of a sequence — sequenceName is always one of the
	/// three literals declared in Schema.sql, never caller/user input.
	/// </summary>
	private static long NextVal(DuckDBConnection conn, string sequenceName)
	{
		using DuckDBCommand command = conn.CreateCommand();
		command.CommandText = $"SELECT nextval('{sequenceName}')";
		return (long)command.ExecuteScalar()!;
	}

	private long ScalarLong(DuckDBConnection conn, string sql)
	{
		if (logDatabaseQueries)
		{
			logger.LogInformation("[DuckDB] {Sql}", sql);
		}
		using DuckDBCommand command = conn.CreateCommand();
		command.CommandText = sql;
		long result = System.Convert.ToInt64(command.ExecuteScalar());
		if (logDatabaseQueries)
		{
			logger.LogInformation("[DuckDB] -> {Result}", result);
		}
		return result;
	}

	private static string LoadEmbeddedSql(string endsWith)
	{
		Assembly assembly = typeof(DuckDbAnalyticsStore).Assembly;
		string name = assembly
			.GetManifestResourceNames()
			.First(n =>
				n.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase)
			);
		using Stream stream = assembly.GetManifestResourceStream(name)!;
		using StreamReader reader = new(stream);
		return reader.ReadToEnd();
	}

	private static string EscapeLike(string value) =>
		value
			.Replace("\\", "\\\\")
			.Replace("%", "\\%")
			.Replace("_", "\\_")
			.Replace("'", "''");

	public void Dispose()
	{
		connection?.Dispose();
		gate.Dispose();
		GC.SuppressFinalize(this);
	}
}
