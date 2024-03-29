using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Configuration;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Configuration;
using TF2SA.StatsETLService.LogsTFIngestion.Services;

namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal class LogsTFIngestionHandler : ILogsTFIngestionHandler
{
	private int count = 0;
	private readonly LogsTFIngestionConfig logsTFIngestionConfig;
	private readonly ILogger<LogsTFIngestionHandler> logger;
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly ILogsTFService logsTFService;
	private readonly IServiceProvider serviceProvider;
	private readonly ILogIngestionRepositoryUpdater logIngestionRepositoryUpdater;
	private readonly IStatisticsUpdater statisticsUpdater;

	public LogsTFIngestionHandler(
		IOptions<LogsTFIngestionConfig> logsTFIngestionConfig,
		ILogger<LogsTFIngestionHandler> logger,
		ILogsTFService logsTFService,
		IGamesRepository<Game, uint> gamesRepository,
		IServiceProvider serviceProvider,
		ILogIngestionRepositoryUpdater logIngestionRepositoryUpdater,
		IStatisticsUpdater statisticsUpdater
	)
	{
		this.logsTFIngestionConfig = logsTFIngestionConfig.Value;
		this.logger = logger;
		this.logsTFService = logsTFService;
		this.gamesRepository = gamesRepository;
		this.serviceProvider = serviceProvider;
		this.logIngestionRepositoryUpdater = logIngestionRepositoryUpdater;
		this.statisticsUpdater = statisticsUpdater;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;

			if (!logsTFIngestionConfig.EnableIngestion)
			{
				logger.LogInformation(
					"Processing disabled, iteration {iteration}",
					count
				);
				await Task.Delay(
					logsTFIngestionConfig.IngestionIntervalSeconds * 1000,
					cancellationToken
				);
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
				logsTFIngestionConfig.IngestionIntervalSeconds * 1000,
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
		if (logsTFIngestionConfig.DebugIngestion == true)
		{
			logsToProcess = logsToProcess.Take(100).ToList();
		}

		await Parallel.ForEachAsync(
			logsToProcess,
			new ParallelOptions()
			{
				MaxDegreeOfParallelism = Constants.MAX_CONCURRENT_HTTP_THREADS,
				CancellationToken = cancellationToken
			},
			async (log, token) =>
			{
				using IServiceScope scope = serviceProvider.CreateScope();
				ILogIngestor logIngestor =
					scope.ServiceProvider.GetRequiredService<ILogIngestor>();

				await logIngestor.IngestLog(
					log,
					logsToProcess.IndexOf(log),
					logsToProcess.Count,
					token
				);
			}
		);

		logger.LogInformation("Updating player steam details.");
		OptionStrict<Error> updatePlayersResult =
			await logIngestionRepositoryUpdater.UpdatePlayers(
				cancellationToken
			);
		if (updatePlayersResult.HasValue)
		{
			logger.LogWarning("Failed to update players steam details.");
		}

		logger.LogInformation("Updating aggregated statistics.");
		if (logsToProcess.Any() || logsTFIngestionConfig.DebugIngestion)
		{
			OptionStrict<Error> updateStatisticsResult =
				await statisticsUpdater.UpdateAggregatedStatistics(
					cancellationToken
				);
			if (updatePlayersResult.HasValue)
			{
				logger.LogWarning(
					"Failed to update aggregated statistics with error(s): {errors}",
					updatePlayersResult.Value.Message
				);
			}
		}
		else
		{
			logger.LogInformation(
				"No new logs, not updating aggregated statistics"
			);
		}

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
			// TODO repository methods should use monads and propagate cancellation tokens
			// this will enhance error handling and we won't have to try catch everywhere
			// propagate cancellation tokens so we can handle cancellation properly
			// milestone: StatsETL
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
