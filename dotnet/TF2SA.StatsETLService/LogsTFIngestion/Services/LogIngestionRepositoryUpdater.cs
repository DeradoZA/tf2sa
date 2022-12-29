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

	public LogIngestionRepositoryUpdater(
		IGamesRepository<Game, uint> gamesRepository,
		ILogger<LogIngestionRepositoryUpdater> logger
	)
	{
		this.gamesRepository = gamesRepository;
		this.logger = logger;
	}

	public async Task<OptionStrict<Error>> InsertInvalidLog(GameLog log)
	{
		Game gameEntity = new() { };

		EitherStrict<Error, Game> insertResult = await gamesRepository.Insert(
			gameEntity
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

	public Task<OptionStrict<Error>> InsertValidLog(GameLog log)
	{
		throw new NotImplementedException();
	}
}
