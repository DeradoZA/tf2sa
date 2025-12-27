using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Configuration;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal class LogsTFIngestionHandler : ILogsTFIngestionHandler
{
	private int _count;
	private readonly LogsTFIngestionConfig _logsTfIngestionConfig;
	private readonly ILogger<LogsTFIngestionHandler> _logger;
	private readonly ILogsTFService _logsTfService;

    public LogsTFIngestionHandler (
		IOptions<LogsTFIngestionConfig> logsTfIngestionConfig,
		ILogger<LogsTFIngestionHandler> logger,
		ILogsTFService logsTfService
	)
	{
		_logsTfIngestionConfig = logsTfIngestionConfig.Value;
		_logger = logger;
		_logsTfService = logsTfService;
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

    private async Task<OptionStrict<Error>> ProcessLogs(CancellationToken cancellationToken)
    {
        var logsToProcessResult = await GetLogsToProcess(cancellationToken: cancellationToken);
        if (logsToProcessResult.IsLeft) return logsToProcessResult.Left;
        var logsToProcess = logsToProcessResult.Right;

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _logsTfIngestionConfig.ConcurrentThreads,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(logsToProcess, parallelOptions, async (log, token) =>
        {
            _logger.LogInformation("Processing {i}/{n}: log {id}", logsToProcess.IndexOf(log), logsToProcess.Count, log.Id);
            await IngestLog(log, token);
			_logger.LogInformation("Processed {i}/{n}: log {id}", logsToProcess.IndexOf(log), logsToProcess.Count, log.Id);
        });

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
        var gameLogRaw = logResult.Right;

        await using var connection = new ClickHouseConnection(_logsTfIngestionConfig.ClickhouseConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        // parameters matching the logs_tf table schema
        command.AddParameter("id", "UInt32", log.Id);
        command.AddParameter("title", "String", log.Title);
        command.AddParameter("map", "String", log.Map);
        command.AddParameter("date_raw", "Int32", log.Date);
        command.AddParameter("views", "Int32", log.Views);
        command.AddParameter("players", "Nullable(Int32)", log.Players.HasValue ? log.Players.Value : null);
        command.AddParameter("raw_game_log", "String", gameLogRaw);

        command.CommandText = @"
INSERT INTO default.logs_tf
  (id, title, map, date_raw, views, players, raw_game_log)
VALUES
  ({id:UInt32}, {title:String}, {map:String}, {date_raw:Int32}, {views:Int32}, {players:Nullable(Int32)}, {raw_game_log:String});
";
        await command.ExecuteNonQueryAsync(cancellationToken);
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
