using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.Constants;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Http.Steam.Models.PlayerSummaries;
using TF2SA.Http.Steam.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;
using ClassStatsDto = TF2SA.Common.Models.LogsTF.GameLogModel.ClassStats;
using PlayerStatsDto = TF2SA.Common.Models.LogsTF.GameLogModel.PlayerStats;
using WeaponStatsDto = TF2SA.Common.Models.LogsTF.GameLogModel.WeaponStats;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

/// <summary>
/// Builds GameWrite/PlayerWrite graphs from raw logs.tf DTOs and writes them via
/// IAnalyticsStore. GameLog -&gt; GameWrite is hand-written (ADR 0009) rather than
/// AutoMapper, since it targets fresh POCOs rather than retrofitting existing
/// entities — the mapping is the same shape AutoMapper's GameMappingProfile used to
/// produce (same flattening of LogInfo/Uploader onto Game, same Red/Blue score
/// lookup, same numeric narrowing), just explicit.
/// </summary>
public class LogIngestionRepositoryUpdater : ILogIngestionRepositoryUpdater
{
	private readonly IAnalyticsStore analyticsStore;
	private readonly ILogger<LogIngestionRepositoryUpdater> logger;
	private readonly ISteamService steamService;

	public LogIngestionRepositoryUpdater(
		IAnalyticsStore analyticsStore,
		ILogger<LogIngestionRepositoryUpdater> logger,
		ISteamService steamService
	)
	{
		this.analyticsStore = analyticsStore;
		this.logger = logger;
		this.steamService = steamService;
	}

	public async Task<OptionStrict<Error>> InsertInvalidLog(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors,
		CancellationToken cancellationToken
	)
	{
		log?.PlayerStats?.Clear();
		GameWrite game = MapGame(log!, logId);
		game.IsValidStats = false;
		game.InvalidStatsReason = string.Join(
			", ",
			ingestionErrors.Select(e => e.Message)
		);

		OptionStrict<Error> insertResult = await analyticsStore.InsertGameAsync(
			game,
			cancellationToken
		);
		if (insertResult.HasValue)
		{
			logger.LogWarning(
				"Failed to write game to db: {error}",
				insertResult.Value.Message
			);
		}
		return insertResult;
	}

	public async Task<OptionStrict<Error>> InsertValidLog(
		GameLog log,
		uint logId,
		CancellationToken cancellationToken
	)
	{
		GameWrite game = MapGame(log, logId);
		game.IsValidStats = true;
		ApplyCapabilityFlagNulling(game);

		List<PlayerWrite> players = game
			.PlayerStats.Select(ps => new PlayerWrite
			{
				SteamId = ps.SteamId,
				PlayerName = log
					.Names?.SingleOrDefault(n =>
						n.SteamId?.ConvertToUInt64() == ps.SteamId
					)
					?.PlayerName,
			})
			.ToList();

		OptionStrict<Error> upsertPlayersResult =
			await analyticsStore.UpsertPlayersAsync(players, cancellationToken);
		if (upsertPlayersResult.HasValue)
		{
			logger.LogWarning(
				"Failed to update players: {error}",
				upsertPlayersResult.Value.Message
			);
			return upsertPlayersResult.Value;
		}

		OptionStrict<Error> insertResult = await analyticsStore.InsertGameAsync(
			game,
			cancellationToken
		);
		if (insertResult.HasValue)
		{
			logger.LogWarning(
				"Failed to write game to db: {error}",
				insertResult.Value.Message
			);
		}
		return insertResult;
	}

	/// <summary>
	/// GameLog -&gt; GameWrite, including its full PlayerStats/ClassStats/WeaponStats
	/// graph. Mirrors GameMappingProfile's old CreateMap&lt;GameLog, Game&gt; chain:
	/// LogInfo's fields flatten onto the game, Uploader flattens through LogInfo,
	/// Red/Blue score come from a lookup in Teams by parsed TeamId, and every
	/// non-nullable destination field defaults when the source is null exactly as
	/// AutoMapper's own nullable-to-non-nullable convention did.
	/// </summary>
	private static GameWrite MapGame(GameLog log, uint logId)
	{
		LogInfo? info = log.Info;
		TeamStats? redTeam = log.Teams?.SingleOrDefault(t =>
			Enum.TryParse(t.TeamId, true, out TeamId id) && id == TeamId.Red
		);
		TeamStats? blueTeam = log.Teams?.SingleOrDefault(t =>
			Enum.TryParse(t.TeamId, true, out TeamId id) && id == TeamId.Blue
		);

		GameWrite game = new()
		{
			GameId = logId,
			Version = (byte?)log.Version,
			RedScore = (byte?)redTeam?.Score,
			BlueScore = (byte?)blueTeam?.Score,
			Duration = (ushort?)log.Duration,
			Success = log.Success,
			Map = info?.Map,
			IsSupplemental = info?.IsSupplemental,
			HasRealDamage = info?.HasRealDamage,
			HasWeaponDamage = info?.HasWeaponDamage ?? false,
			HasAccuracy = info?.HasAccuracy ?? false,
			HasHp = info?.HasHp,
			HasHpReal = info?.HasHPReal,
			HasHeadshots = info?.HasHeadshots ?? false,
			HasHeadshotsHit = info?.HasHeadshotsHit ?? false,
			HasBackstabs = info?.HasBackstabs ?? false,
			HasCapturePointsCaptured = info?.HasCapturePointsCaptured ?? false,
			HasSentriesBuilt = info?.HasSentriesBuilt ?? false,
			HasDamageTaken = info?.HasDamageTaken ?? false,
			HasAirshots = info?.HasAirshots ?? false,
			HasHealsReceived = info?.HasHealsReceived ?? false,
			HasIntelCaptures = info?.HasIntelCaptures ?? false,
			HasAdScoring = info?.HasAdscoring,
			Notifications = info?.Notifications,
			Title = info?.Title,
			Date = (uint)(info?.Date ?? 0),
			UploaderId = ParseUploaderId(info?.Uploader?.Id),
			UploaderName = info?.Uploader?.Name,
			UploaderInfo = info?.Uploader?.Info,
		};

		game.PlayerStats =
			log.PlayerStats?.Select(MapPlayerStat).ToList() ?? new();

		return game;
	}

	private static PlayerStatWrite MapPlayerStat(PlayerStatsDto ps) =>
		new()
		{
			SteamId = ps.Player!.SteamId!.ConvertToUInt64(),
			TeamId = ps.TeamId,
			DamageTaken = (uint?)ps.DamageTaken,
			HealsReceived = (uint?)ps.HealsReceived,
			LongestKillStreak = (uint)(ps.LongestKillStreak ?? 0),
			Airshots = (byte?)ps.Airshots,
			Ubers = (byte?)ps.Ubers,
			Drops = (byte?)ps.Drops,
			Medkits = (uint)(ps.MedKits ?? 0),
			MedkitsHp = (uint?)ps.MedkitsHp,
			Backstabs = (byte?)ps.Backstabs,
			Headshots = (byte?)ps.Headshots,
			HeadshotsHit = (byte?)ps.HeadshotsHit,
			SentriesBuilt = (byte?)ps.SentriesBuilt,
			Heals = (uint?)ps.Heals,
			CapturePointsCaptured = (byte?)ps.CapturePointsCaptured,
			IntelCaptures = (byte?)ps.IntelCaptures,
			ClassStats = ps.ClassStats?.Select(MapClassStat).ToList() ?? new(),
		};

	private static ClassStatWrite MapClassStat(ClassStatsDto cs) =>
		new()
		{
			ClassId = cs.ClassId!.Value,
			Kills = (byte)(cs.Kills ?? 0),
			Assists = (byte)(cs.Assists ?? 0),
			Deaths = (byte)(cs.Deaths ?? 0),
			Damage = (uint)(cs.Damage ?? 0),
			Playtime = (ushort)(cs.Playtime ?? 0),
			WeaponStats =
				cs.WeaponStats?.Select(MapWeaponStat).ToList() ?? new(),
		};

	private static WeaponStatWrite MapWeaponStat(WeaponStatsDto ws) =>
		new()
		{
			WeaponName = ws.WeaponName ?? string.Empty,
			Kills = (byte)(ws.Kills ?? 0),
			Damage = (uint?)ws.Damage,
			Shots = (uint?)ws.Shots,
			Hits = (uint?)ws.Hits,
		};

	private static ulong? ParseUploaderId(string? id) =>
		ulong.TryParse(id, out ulong parsed) ? parsed : null;

	/// <summary>
	/// Nulls out fields the log itself declared unsupported (the Game.Has* flags),
	/// plus airshots for a detected TFTrue game — verbatim port of
	/// LogIngestionRepositoryUpdater.MakeValidGame's post-processing.
	/// </summary>
	private void ApplyCapabilityFlagNulling(GameWrite game)
	{
		bool isTfTrueGame =
			game.UploaderInfo is not null
			&& game.UploaderInfo.Contains(
				"TFTrue",
				StringComparison.InvariantCultureIgnoreCase
			);

		if (isTfTrueGame)
		{
			logger.LogWarning("TFTrue game detected. Nulling airshots.");
		}

		foreach (PlayerStatWrite playerStat in game.PlayerStats)
		{
			playerStat.Headshots = !game.HasHeadshots
				? null
				: playerStat.Headshots;
			playerStat.HeadshotsHit = !game.HasHeadshotsHit
				? null
				: playerStat.HeadshotsHit;
			playerStat.Backstabs = !game.HasBackstabs
				? null
				: playerStat.Backstabs;
			playerStat.CapturePointsCaptured = !game.HasCapturePointsCaptured
				? null
				: playerStat.CapturePointsCaptured;
			playerStat.SentriesBuilt = !game.HasSentriesBuilt
				? null
				: playerStat.SentriesBuilt;
			playerStat.DamageTaken = !game.HasDamageTaken
				? null
				: playerStat.DamageTaken;
			playerStat.Airshots =
				!game.HasAirshots || isTfTrueGame ? null : playerStat.Airshots;
			playerStat.HealsReceived = !game.HasHealsReceived
				? null
				: playerStat.HealsReceived;
			playerStat.IntelCaptures = !game.HasIntelCaptures
				? null
				: playerStat.IntelCaptures;

			foreach (ClassStatWrite classStat in playerStat.ClassStats)
			{
				foreach (WeaponStatWrite weaponStat in classStat.WeaponStats)
				{
					if (!game.HasWeaponDamage)
					{
						weaponStat.Damage = null;
					}
					if (!game.HasAccuracy)
					{
						weaponStat.Shots = null;
						weaponStat.Hits = null;
					}
				}
			}
		}
	}

	public async Task<OptionStrict<Error>> UpdatePlayers(
		CancellationToken cancellationToken
	)
	{
		ulong[] playerIdsToUpdate;
		try
		{
			IReadOnlyList<PlayerIdRow> allPlayers =
				await analyticsStore.GetAllAsync<PlayerIdRow>(
					"Players",
					cancellationToken
				);
			playerIdsToUpdate = allPlayers.Select(p => p.SteamId).ToArray();
		}
		catch (Exception e)
		{
			return new DatabaseError($"Failed to fetch players: {e.Message}");
		}

		if (playerIdsToUpdate.Length < 1)
		{
			return new IngestionError("No players to update.");
		}

		EitherStrict<Error, List<SteamPlayer>> steamPlayersResult =
			await steamService.GetPlayers(playerIdsToUpdate, cancellationToken);
		if (steamPlayersResult.IsLeft)
		{
			return steamPlayersResult.Left;
		}

		List<PlayerWrite> updatedPlayers = steamPlayersResult
			.Right.Select(MapSteamPlayer)
			.ToList();

		return await analyticsStore.UpdatePlayersAsync(
			updatedPlayers,
			cancellationToken
		);
	}

	private static PlayerWrite MapSteamPlayer(SteamPlayer p) =>
		new()
		{
			SteamId = ulong.Parse(p.SteamId),
			PlayerName = p.PlayerName,
			LocalCountryCode = p.LocalCountryCode,
			ProfileUrl = p.ProfileUrl,
			Avatar = p.Avatar,
			AvatarMedium = p.AvatarMedium,
			AvatarFull = p.AvatarFull,
			AvatarHash = p.AvatarHash,
			RealName = p.RealName,
		};
}
