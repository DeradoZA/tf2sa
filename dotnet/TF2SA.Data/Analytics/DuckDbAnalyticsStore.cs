using System.Data.Common;
using System.Globalization;
using System.Reflection;
using DuckDB.NET.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Errors;

namespace TF2SA.Data.Analytics;

/// <summary>
/// DuckDB-backed analytics store (ADR 0008). <see cref="RefreshAsync"/> copies the
/// raw tables from MariaDB (via DuckDB's mysql scanner) into a columnar DuckDB file
/// and installs the leaderboard macros; <see cref="GetPageAsync"/> serves paged,
/// sorted, filtered reads of a board. The file is a derived, rebuildable cache —
/// MariaDB stays the system of record. One connection is shared and access is
/// serialized (single writer at refresh, fast reads), which suits this scale.
/// </summary>
public class DuckDbAnalyticsStore : IAnalyticsStore, IDisposable
{
	private static readonly string[] RawTables =
	{
		"Players",
		"Games",
		"PlayerStats",
		"ClassStats"
	};

	private readonly ILogger<DuckDbAnalyticsStore> logger;
	private readonly string duckDbPath;
	private readonly string mysqlAttach;
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

		string host = Escape(configuration["TF2SA_MYSQL_HOST"] ?? "localhost");
		string port = Escape(configuration["TF2SA_MYSQL_PORT"] ?? "3306");
		string user = Escape(configuration["TF2SA_MYSQL_USR"] ?? "root");
		string password = Escape(configuration["TF2SA_MYSQL_PWD"] ?? "");
		string database = Escape(configuration["TF2SA_MYSQL_DB"] ?? "tf2sa_db");
		mysqlAttach =
			$"host={host} port={port} user={user} password={password} database={database}";
	}

	// ---- refresh ----------------------------------------------------------

	public async Task<OptionStrict<Error>> RefreshAsync(
		CancellationToken cancellationToken
	)
	{
		if (!Enabled)
		{
			logger.LogInformation(
				"Analytics store disabled; skipping DuckDB refresh."
			);
			return OptionStrict<Error>.Nothing;
		}

		await gate.WaitAsync(cancellationToken);
		try
		{
			Rebuild(cancellationToken);
			return OptionStrict<Error>.Nothing;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"DuckDB analytics refresh failed: {e.Message}"
			);
		}
		finally
		{
			gate.Release();
		}
	}

	private void Rebuild(CancellationToken cancellationToken)
	{
		logger.LogInformation(
			"Refreshing DuckDB analytics store at {path} from MariaDB.",
			duckDbPath
		);

		DuckDBConnection conn = Connection();
		Execute(conn, "INSTALL mysql; LOAD mysql;");
		Execute(conn, $"ATTACH '{mysqlAttach}' AS maria (TYPE mysql, READ_ONLY);");
		try
		{
			foreach (string table in RawTables)
			{
				cancellationToken.ThrowIfCancellationRequested();
				Execute(
					conn,
					$"CREATE OR REPLACE TABLE {table} AS SELECT * FROM maria.{table};"
				);
			}
		}
		finally
		{
			Execute(conn, "DETACH maria;");
		}

		LoadViews(conn);

		logger.LogInformation(
			"DuckDB analytics refresh complete: {counts}.",
			string.Join(
				", ",
				RawTables.Select(
					t =>
						$"{t}={ScalarLong(conn, $"SELECT COUNT(*) FROM {t}")}"
				)
			)
		);
	}

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
		PropertyInfo? sortProp = props.FirstOrDefault(
			p => p.Name.Equals(sort, StringComparison.OrdinalIgnoreCase)
		);
		string sortUsed = sortProp is not null ? sort : defaultSortKey;
		string sortColumn = (
			sortProp
			?? props.First(
				p =>
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
		PropertyInfo? filterProp = props.FirstOrDefault(
			p =>
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
			int total = (int)
				ScalarLong(conn, $"SELECT COUNT(*) FROM {boardCall} {where}");
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

			// Reload the view definitions from the assembly on first use, so a
			// deploy that changes them takes effect without waiting for a refresh
			// (which only runs when an ingestion cycle finds new logs). Skipped when
			// the raw tables are absent — DuckDB resolves table references when a
			// macro is created, so a fresh file must be populated by a refresh first,
			// which loads them itself.
			if (RawTablesExist(connection))
			{
				LoadViews(connection);
			}
		}
		return connection;
	}

	private void LoadViews(DuckDBConnection conn)
	{
		Execute(conn, LoadEmbeddedSql("Leaderboards.sql"));
		Execute(conn, LoadEmbeddedSql("PlayerDetail.sql"));
	}

	private bool RawTablesExist(DuckDBConnection conn)
	{
		string names = string.Join(", ", RawTables.Select(t => $"'{t}'"));
		return ScalarLong(
				conn,
				$"SELECT COUNT(*) FROM duckdb_tables() WHERE table_name IN ({names})"
			) == RawTables.Length;
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
			.First(
				n => n.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase)
			);
		using Stream stream = assembly.GetManifestResourceStream(name)!;
		using StreamReader reader = new(stream);
		return reader.ReadToEnd();
	}

	private static string Escape(string value) => value.Replace("'", "''");

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
