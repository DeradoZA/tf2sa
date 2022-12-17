using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;

namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal class LogsTFIngestionHandler : ILogsTFIngestionHandler
{
	private int count = 0;
	private const int PROCESS_INTERVAL_SECONDS = 20;
	private const int MAX_CONCURRENT_THREADS = 5;
	private readonly ILogger<LogsTFIngestionHandler> logger;
	private readonly IPlayersRepository<Player, ulong> playerRepository;
	private readonly ILogsTFService logsTFService;

	public LogsTFIngestionHandler(
		ILogger<LogsTFIngestionHandler> logger,
		IPlayersRepository<Player, ulong> playerRepository,
		ILogsTFService logsTFService
	)
	{
		this.logger = logger;
		this.playerRepository = playerRepository;
		this.logsTFService = logsTFService;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;
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
				PROCESS_INTERVAL_SECONDS * 1000,
				cancellationToken
			);
		}
	}

	private async Task<OptionStrict<Error>> ProcessLogs(
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

		await Parallel.ForEachAsync(
			allLogs,
			new ParallelOptions()
			{
				MaxDegreeOfParallelism = MAX_CONCURRENT_THREADS,
				CancellationToken = cancellationToken
			},
			async (log, token) => await ProcessLog(log, allLogs, token)
		);

		return OptionStrict<Error>.Nothing;
	}

	private async Task ProcessLog(
		LogListItem log,
		List<LogListItem> allLogs,
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation(
			"processing {iteration} of {length}: log {logId}",
			allLogs.IndexOf(log),
			allLogs.Count,
			log.Id
		);

		await Task.Delay(1 * 1000, cancellationToken);
	}
}
