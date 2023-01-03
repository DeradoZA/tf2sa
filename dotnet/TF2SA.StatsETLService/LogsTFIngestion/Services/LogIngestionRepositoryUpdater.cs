using AutoMapper;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using Player = TF2SA.Data.Entities.MariaDb.Player;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogIngestionRepositoryUpdater : ILogIngestionRepositoryUpdater
{
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly IPlayersRepository<Player, ulong> playersRepository;
	private readonly ILogger<LogIngestionRepositoryUpdater> logger;
	private readonly IMapper mapper;

	public LogIngestionRepositoryUpdater(
		IGamesRepository<Game, uint> gamesRepository,
		ILogger<LogIngestionRepositoryUpdater> logger,
		IMapper mapper,
		IPlayersRepository<Player, ulong> playersRepository
	)
	{
		this.gamesRepository = gamesRepository;
		this.logger = logger;
		this.mapper = mapper;
		this.playersRepository = playersRepository;
	}

	public async Task<OptionStrict<Error>> InsertInvalidLog(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors,
		CancellationToken cancellationToken
	)
	{
		Game game = MakeInvalidGame(log, logId, ingestionErrors);

		EitherStrict<Error, Game> insertResult = await gamesRepository.Insert(
			game,
			cancellationToken
		);

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

	private Game MakeInvalidGame(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors
	)
	{
		log?.PlayerStats?.Clear();
		Game game = mapper.Map<Game>(log);
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
		Game game = MakeValidGame(log, logId);

		HashSet<Player> players = new();
		foreach (PlayerStat playerStat in game.PlayerStats)
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

		EitherStrict<Error, Game> insertResult = await gamesRepository.Insert(
			game,
			cancellationToken
		);
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

	private Game MakeValidGame(GameLog log, uint logId)
	{
		Game game = mapper.Map<Game>(log);
		game.GameId = logId;
		game.IsValidStats = true;

		return game;
	}
}
