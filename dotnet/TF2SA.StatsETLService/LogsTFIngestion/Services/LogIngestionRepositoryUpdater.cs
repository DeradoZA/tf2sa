using AutoMapper;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;
using TF2SA.Http.Steam.Service;
using TF2SA.Http.Steam.Models.PlayerSummaries;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogIngestionRepositoryUpdater : ILogIngestionRepositoryUpdater
{
	private readonly IGamesRepository<GameEntity, uint> gamesRepository;
	private readonly IPlayersRepository<PlayerEntity, ulong> playersRepository;
	private readonly ILogger<LogIngestionRepositoryUpdater> logger;
	private readonly IMapper mapper;
	private readonly ISteamService steamService;

	public LogIngestionRepositoryUpdater(
		IGamesRepository<GameEntity, uint> gamesRepository,
		ILogger<LogIngestionRepositoryUpdater> logger,
		IMapper mapper,
		IPlayersRepository<PlayerEntity, ulong> playersRepository,
		ISteamService steamService
	)
	{
		this.gamesRepository = gamesRepository;
		this.logger = logger;
		this.mapper = mapper;
		this.playersRepository = playersRepository;
		this.steamService = steamService;
	}

	public async Task<OptionStrict<Error>> InsertInvalidLog(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors,
		CancellationToken cancellationToken
	)
	{
		GameEntity game = MakeInvalidGame(log, logId, ingestionErrors);

		EitherStrict<Error, GameEntity> insertResult =
			await gamesRepository.Insert(game, cancellationToken);

		if (insertResult.IsLeft)
		{
			logger.LogWarning(
				"Failed to write game to db: {error}",
				insertResult.Left.Message
			);
			return insertResult.Left;
		}

		return OptionStrict<Error>.Nothing;
	}

	private GameEntity MakeInvalidGame(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors
	)
	{
		log?.PlayerStats?.Clear();
		GameEntity game = mapper.Map<GameEntity>(log);
		game.GameId = logId;
		game.IsValidStats = false;
		game.InvalidStatsReason = string.Join(
			", ",
			ingestionErrors.Select(e => e.Message)
		);
		return game;
	}

	public async Task<OptionStrict<Error>> InsertValidLog(
		GameLog log,
		uint logId,
		CancellationToken cancellationToken
	)
	{
		GameEntity game = MakeValidGame(log, logId);

		HashSet<PlayerEntity> players = new();
		foreach (PlayerStatEntity playerStat in game.PlayerStatsEntities)
		{
			players.Add(
				new()
				{
					SteamId = playerStat.SteamId,
					PlayerName = log?.Names?.Single(
						n => n.SteamId == playerStat.SteamId
					).PlayerName
				}
			);
		}

		OptionStrict<Error> updatePlayersResult =
			await playersRepository.InsertPlayersIfNotExists(
				players,
				cancellationToken
			);
		if (updatePlayersResult.HasValue)
		{
			logger.LogWarning(
				"Failed to update players: {error}",
				updatePlayersResult.Value.Message
			);
			return updatePlayersResult.Value;
		}

		EitherStrict<Error, GameEntity> insertResult =
			await gamesRepository.Insert(game, cancellationToken);
		if (insertResult.IsLeft)
		{
			logger.LogWarning(
				"Failed to write game to db: {error}",
				insertResult.Left.Message
			);
			return insertResult.Left;
		}

		return OptionStrict<Error>.Nothing;
	}

	private GameEntity MakeValidGame(GameLog log, uint logId)
	{
		GameEntity game = mapper.Map<GameEntity>(log);
		game.GameId = logId;
		game.IsValidStats = true;
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

		foreach (PlayerStatEntity playerStat in game.PlayerStatsEntities)
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

			foreach (ClassStatEntity classStat in playerStat.ClassStatsEntities)
			{
				foreach (
					WeaponStatEntity weaponStat in classStat.WeaponStatsEntities
				)
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

		return game;
	}

	public async Task<OptionStrict<Error>> UpdatePlayers(
		CancellationToken cancellationToken
	)
	{
		ulong[] playerIdsToUpdate;
		try
		{
			playerIdsToUpdate = playersRepository
				.GetAll()
				.Select(p => p.SteamId)
				.ToArray();
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

		List<PlayerEntity> updatedPlayers = mapper.Map<List<PlayerEntity>>(
			steamPlayersResult.Right
		);

		OptionStrict<Error> updateResult =
			await playersRepository.UpdatePlayers(
				updatedPlayers,
				cancellationToken
			);
		if (updateResult.HasValue)
		{
			return updateResult.Value;
		}

		return OptionStrict<Error>.Nothing;
	}
}
