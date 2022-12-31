using Monad;
using TF2SA.Common.Configuration;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Services;

namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal class LogsTFIngestionHandler : ILogsTFIngestionHandler
{
	private int count = 0;
	private const int PROCESS_INTERVAL_MINUTES = 10;
	private bool ENABLE_PROCESSING = true;
	private readonly ILogger<LogsTFIngestionHandler> logger;
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly IPlayersRepository<Player, ulong> playerRepository;
	private readonly ILogsTFService logsTFService;
	private readonly ILogIngestor logIngestor ;

    public LogsTFIngestionHandler(
        ILogger<LogsTFIngestionHandler> logger,
        IPlayersRepository<Player, ulong> playerRepository,
        ILogsTFService logsTFService,
        IGamesRepository<Game, uint> gamesRepository
,
        ILogIngestor logIngestor)
    {
        this.logger = logger;
        this.playerRepository = playerRepository;
        this.logsTFService = logsTFService;
        this.gamesRepository = gamesRepository;
        this.logIngestor = logIngestor;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;

			if (!ENABLE_PROCESSING)
			{
				logger.LogInformation(
					"Processing disabled, iteration {iteration}",
					count
				);
				await Task.Delay(20 * 1000, cancellationToken);
				continue;
			}

			OptionStrict<Error> processResult = await ProcessLogs(
				cancellationToken
			);
			if (processResult.HasValue)
			{
				logger.LogWarning(
					"Failed to fetch logs: {Error}",
					processResult.Value
				);
			}
			logger.LogInformation(
				"Successfully processed logs, iteration {iteration}",
				count
			);

			await Task.Delay(
				PROCESS_INTERVAL_MINUTES * 1000,
				cancellationToken
			);
		}
	}

	private async Task<OptionStrict<Error>> ProcessLogs(
		CancellationToken cancellationToken
	)
	{
		EitherStrict<Error, List<LogListItem>> logsToProcessResult =
			await GetLogsToProcess(cancellationToken);
		if (logsToProcessResult.IsLeft)
		{
			return logsToProcessResult.Left;
		}
		List<LogListItem> logsToProcess = logsToProcessResult.Right;

		await Parallel.ForEachAsync(
			logsToProcess,
			new ParallelOptions()
			{
				MaxDegreeOfParallelism = Constants.MAX_CONCURRENT_HTTP_THREADS,
				CancellationToken = cancellationToken
			},
			async (log, token) => await logIngestor.IngestLog(log, logsToProcess.IndexOf(log), logsToProcess.Count, token)
		);

		return OptionStrict<Error>.Nothing;
	}

	private async Task<EitherStrict<Error, List<LogListItem>>> GetLogsToProcess(
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
		try
		{
			// repository methods should use monads - so we can handle db exceptions cleaner.
			// therefore here we need try catch to handle failure
			processedLogs = gamesRepository.GetAll();
			logger.LogInformation(
				"Processed logs: {count}",
				processedLogs.Count
			);

			int logsRemoved = allLogs.RemoveAll(
				a => processedLogs.Where(p => p.GameId == a.Id).Any()
			);
			logger.LogInformation(
				"{logsRemoved} logs already processed. {logToProcess} logs to process",
				logsRemoved,
				allLogs.Count
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to access games/blacklist from database: {e.Message}"
			);
		}

		return allLogs;
	}
}
