using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Configuration;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Config;

namespace TF2SA.Http.LogsTF.Service;

public class LogsTFService : ILogsTFService
{
	private readonly LogsTFConfig logsTFConfig;
	private readonly ILogger<LogsTFService> logger;
	private readonly IHttpClient httpClient;

	public LogsTFService(
		IOptions<LogsTFConfig> logsTFConfig,
		ILogger<LogsTFService> logger,
		IHttpClient httpClient
	)
	{
		this.logsTFConfig = logsTFConfig.Value;
		this.logger = logger;
		this.httpClient = httpClient;
	}

	public async Task<EitherStrict<HttpError, LogListResult>> GetLogList(
		LogListQueryParams filter,
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation(message: $"Fetching log list");

		string url = $"{logsTFConfig.BaseUrl}/log";
		string? queryString = LogListQueryParams.GetQueryString(filter: filter);
		if (queryString is not null)
		{
			url += $"?{queryString}";
		}

		EitherStrict<HttpError, LogListResult> logList =
			await httpClient.Get<LogListResult>(url: url, cancellationToken: cancellationToken);
		if (logList.IsLeft)
		{
			return logList.Left;
		}

		return logList.Right;
	}

	public async Task<EitherStrict<HttpError, string>> GetGameLogRaw(ulong logId, CancellationToken cancellationToken)
	{
		var url = $"{logsTFConfig.BaseUrl}/log/{logId}";

		EitherStrict<HttpError, string> log =
			await httpClient.Get<string>(url: url, cancellationToken: cancellationToken);
		if (log.IsLeft)
		{
			return log.Left;
		}

		return log.Right;
	}

	public async Task<EitherStrict<HttpError, GameLog>> GetGameLog(
		ulong logId,
		CancellationToken cancellationToken
	)
	{
		var url = $"{logsTFConfig.BaseUrl}/log/{logId}";

		EitherStrict<HttpError, GameLog> logList =
			await httpClient.Get<GameLog>(url: url, cancellationToken: cancellationToken);
		if (logList.IsLeft)
		{
			return logList.Left;
		}

		return logList.Right;
	}

	public async Task<EitherStrict<HttpError, List<LogListItem>>> GetAllLogs(
		ulong uploader,
		CancellationToken cancellationToken
	)
	{
		List<LogListItem> logs = new();

		int totalLogsFetched = 0;
		int totalLogCount;
		do
		{
			LogListQueryParams filter =
				new()
				{
					Uploader = uploader,
					Limit = LogListQueryParams.LIMIT_MAX,
					Offset = totalLogsFetched
				};

			EitherStrict<HttpError, LogListResult> logListResult =
				await GetLogList(filter: filter, cancellationToken: cancellationToken);
			if (logListResult.IsLeft)
			{
				return logListResult.Left;
			}

			logs.AddRange(collection: logListResult.Right.Logs);

			totalLogCount = logListResult.Right.Total;
			totalLogsFetched =
				logListResult.Right.Results
				+ logListResult.Right.Parameters.Offset;
			logger.LogTrace(
				message: "Fetching from uploader {uploader}: Fetched {count} logs of {totalLogsFetched}. Offset: {offset}",
				args: new object?[]{uploader,
					totalLogsFetched,
					totalLogCount,
					filter.Offset}
			);
		} while (totalLogsFetched != totalLogCount);

		return logs;
	}

	public async Task<EitherStrict<HttpError, List<LogListItem>>> GetAllLogs(
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation(message: "fetching game logs");

		ulong[] uploaders = logsTFConfig.Uploaders;
		List<LogListItem> logs = new();
		List<HttpError> errors = new();

		await Parallel.ForEachAsync(
			source: uploaders,
			parallelOptions: new ParallelOptions
			{
				MaxDegreeOfParallelism = Constants.MAX_CONCURRENT_HTTP_THREADS,
				CancellationToken = cancellationToken
			},
			body: async (uploader, token) =>
			{
				EitherStrict<HttpError, List<LogListItem>> uploaderLogs =
					await GetAllLogs(uploader: uploader, cancellationToken: token);
				if (uploaderLogs.IsLeft)
				{
					errors.Add(item: uploaderLogs.Left);
					return;
				}

				logs.AddRange(collection: uploaderLogs.Right);
			}
		);

		if (errors.Any())
		{
			return errors[index: 0];
		}

		return logs;
	}
}
