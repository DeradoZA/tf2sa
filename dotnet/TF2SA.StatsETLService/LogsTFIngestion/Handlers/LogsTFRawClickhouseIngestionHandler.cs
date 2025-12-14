using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Configuration;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Configuration;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;
using TF2SA.StatsETLService.LogsTFIngestion.Services;

namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal class LogsTFRawClickhouseIngestionHandler : ILogsTFIngestionHandler
{
	private int _count;
	private readonly LogsTFIngestionConfig _logsTfIngestionConfig;
	private readonly ILogger<LogsTFIngestionHandler> _logger;
	private readonly IGamesRepository<Game, uint> _gamesRepository;
	private readonly ILogsTFService _logsTfService;
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogIngestionRepositoryUpdater _logIngestionRepositoryUpdater;
	private readonly IStatisticsUpdater _statisticsUpdater;

	public LogsTFRawClickhouseIngestionHandler (
		IOptions<LogsTFIngestionConfig> logsTfIngestionConfig,
		ILogger<LogsTFIngestionHandler> logger,
		ILogsTFService logsTfService,
		IGamesRepository<Game, uint> gamesRepository,
		IServiceProvider serviceProvider,
		ILogIngestionRepositoryUpdater logIngestionRepositoryUpdater,
		IStatisticsUpdater statisticsUpdater
	)
	{
		_logsTfIngestionConfig = logsTfIngestionConfig.Value;
		_logger = logger;
		_logsTfService = logsTfService;
		_gamesRepository = gamesRepository;
		_serviceProvider = serviceProvider;
		_logIngestionRepositoryUpdater = logIngestionRepositoryUpdater;
		_statisticsUpdater = statisticsUpdater;
	}
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_count++;

			if (!_logsTfIngestionConfig.EnableIngestion)
			{
				_logger.LogInformation(
					message: "Processing disabled, iteration {iteration}",
					args: _count
				);
				await Task.Delay(
					millisecondsDelay: _logsTfIngestionConfig.IngestionIntervalSeconds * 1000,
					cancellationToken: cancellationToken
				);
				continue;
			}

			var processResult = await ProcessLogs(
				cancellationToken: cancellationToken
			);
			if (processResult.HasValue)
			{
				_logger.LogWarning(
					message: "Failed to fetch logs: {Error}",
					args: processResult.Value
				);
			}
			_logger.LogInformation(
				message: "Successfully processed logs, iteration {Iteration}. Next run in {IngestionIntervalSeconds}s",
				 _count, _logsTfIngestionConfig.IngestionIntervalSeconds
			);

			await Task.Delay(
				millisecondsDelay: _logsTfIngestionConfig.IngestionIntervalSeconds * 1000,
				cancellationToken: cancellationToken
			);
		}
	}

	private async Task<OptionStrict<Error>> ProcessLogs(
		CancellationToken cancellationToken
	)
	{
		var logsToProcessResult =
			await GetLogsToProcess(cancellationToken: cancellationToken);
		if (logsToProcessResult.IsLeft)
		{
			return logsToProcessResult.Left;
		}
		var logsToProcess = logsToProcessResult.Right;
		if (_logsTfIngestionConfig.DebugIngestion)
		{
			logsToProcess = logsToProcess.Take(count: 100).ToList();
		}

		await Parallel.ForEachAsync(
			source: logsToProcess,
			parallelOptions: new ParallelOptions
			{
				MaxDegreeOfParallelism = Constants.MAX_CONCURRENT_HTTP_THREADS,
				CancellationToken = cancellationToken
			},
			body: async (log, token) =>
			{
				_logger.LogInformation(
					"Processing {iteration} of {length}: log {logId}",
					logsToProcess.IndexOf(item: log),
						logsToProcess.Count,
						log.Id
				);

				await IngestLog(
					log: log, cancellationToken: token
				);
			}
		);

		// TODO: Update steam profiles

		return OptionStrict<Error>.Nothing;
	}

	private async Task IngestLog(LogListItem log, CancellationToken cancellationToken)
	{
		List<Error> ingestionErrors = new();

		EitherStrict<HttpError, string> logResult =
			await _logsTfService.GetGameLogRaw(logId: log.Id, cancellationToken: cancellationToken);
		if (logResult.IsLeft)
		{
			ReportLogFetchFailed(logListItem: log, ingestionErrors: ingestionErrors, logResult: logResult);
			return;
		}
		var logResultRaw = logResult.Right;

		// TODO: Insert into Clickhouse
		_logger.LogInformation(
			message: "Successfully inserted valid log {logId}",
			args: log.Id
		);
	}

	private async Task<EitherStrict<Error, List<LogListItem>>> GetLogsToProcess(
		CancellationToken cancellationToken
	)
	{
		var allLogsResult =
			await _logsTfService.GetAllLogs(cancellationToken: cancellationToken);
		if (allLogsResult.IsLeft)
		{
			return allLogsResult.Left;
		}
		var allLogs = allLogsResult.Right;
		_logger.LogInformation(message: "Fetched all logs: {count}", args: allLogs.Count);

		return allLogs;
		// TODO: consider existing logs
	}
	
	private void ReportLogFetchFailed(
		LogListItem logListItem,
		List<Error> ingestionErrors,
		EitherStrict<HttpError, string> logResult
	)
	{
		ingestionErrors.Add(
			item: new IngestionError(message: $"Failed to fetch Log: {logResult.Left.Message}")
		);
		_logger.LogWarning(
			message: "Failed to fetch {logId} from LogsTF API. "
			         + "No changes to database. Error: {error}",
			logListItem.Id,
				logResult.Left.Message
		);
	}
}
