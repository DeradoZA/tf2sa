using AutoMapper;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogIngestionRepositoryUpdater : ILogIngestionRepositoryUpdater
{
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly ILogger<LogIngestionRepositoryUpdater> logger;
	private readonly IMapper mapper;

	public LogIngestionRepositoryUpdater(
		IGamesRepository<Game, uint> gamesRepository,
		ILogger<LogIngestionRepositoryUpdater> logger,
		IMapper mapper
	)
	{
		this.gamesRepository = gamesRepository;
		this.logger = logger;
		this.mapper = mapper;
	}

	public async Task<OptionStrict<Error>> InsertInvalidLog(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors,
		CancellationToken cancellationToken
	)
	{
		Game game = mapper.Map<Game>(log);
		game.GameId = logId;
		game.IsValidStats = false;
		game.InvalidStatsReason = string.Join(", ", ingestionErrors.Select(e => e.Message));

		await Task.Delay(1 * 1000, cancellationToken);
		//EitherStrict<Error, Game> insertResult = await gamesRepository.Insert(
		//	gameEntity
		//);

		//if (insertResult.IsLeft)
		//{
		//	logger.LogWarning(
		//		"Failed to write game to db: {error}",
		//		insertResult.Left.Message
		//	);
		//	return insertResult.Left;
		//}

		return OptionStrict<Error>.Nothing;
	}

	public Task<OptionStrict<Error>> InsertValidLog(
		GameLog log,
		uint logId,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
