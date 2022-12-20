using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogsTFIngestor : ILogsTFIngestor
{
	private readonly ILogger<LogsTFIngestor> logger;
	private readonly ILogsTFService logsTFService;
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly IBlacklistGamesRepository<
		BlacklistGame,
		uint
	> blacklistGamesRepository;

	public LogsTFIngestor(
		ILogger<LogsTFIngestor> logger,
		ILogsTFService logsTFService,
		IGamesRepository<Game, uint> gamesRepository,
		IBlacklistGamesRepository<BlacklistGame, uint> blacklistGamesRepository
	)
	{
		this.logger = logger;
		this.logsTFService = logsTFService;
		this.gamesRepository = gamesRepository;
		this.blacklistGamesRepository = blacklistGamesRepository;
	}

	public async Task<EitherStrict<Error, List<LogListItem>>> GetLogsToProcess(
		CancellationToken cancellationToken
	)
	{
		EitherStrict<HttpError, List<LogListItem>> allLogsResult =
			await logsTFService.GetAllLogs(cancellationToken);
		if (allLogsResult.IsLeft)
		{
			return allLogsResult.Left;
		}
		List<LogListItem> allLogs = allLogsResult.Right;
		logger.LogInformation("Fetched all logs: {count}", allLogs.Count);

		List<Game> processedLogs = new();
		List<BlacklistGame> blacklistGames = new();
		try
		{
			// repository methods should use monads - so we can handle db exceptions cleaner.
			// therefore here we need try catch to handle failure
			processedLogs = gamesRepository.GetAll();
			logger.LogInformation(
				"Processed logs: {count}",
				processedLogs.Count
			);
			blacklistGames = blacklistGamesRepository.GetAll();
			logger.LogInformation(
				"Blacklisted logs: {count}",
				blacklistGames.Count
			);

			int logsRemoved = allLogs.RemoveAll(
				a =>
					processedLogs.Where(p => p.GameId == a.Id).Any()
					|| blacklistGames.Where(b => b.GameId == a.Id).Any()
			);
			logger.LogInformation(
				"{logsRemoved} logs already processed. {logToProcess} logs to process",
				logsRemoved,
				allLogs.Count
			);
		}
		catch (Exception e)
		{
			return new DataAccessError(
				$"Failed to access games/blacklist from database: {e.Message}"
			);
		}

		return allLogs;
	}
}
