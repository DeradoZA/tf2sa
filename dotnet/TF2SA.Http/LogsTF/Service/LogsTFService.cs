using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
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
		IHttpClient httpClient)
	{
		this.logsTFConfig = logsTFConfig.Value;
		this.logger = logger;
		this.httpClient = httpClient;
		logger.LogInformation($"Init: {logsTFConfig.Value.BaseUrl}");
		logger.LogInformation($"Init: {logsTFConfig.Value.Uploaders.Count()}");
		logger.LogInformation($"Init: {logsTFConfig.Value.Uploaders[0]}");
	}

	public async Task<EitherStrict<HttpError, LogListResult>> GetLogList(LogListQueryParams filter)
	{
		logger.LogInformation($"Fetching log list");

		string url = $"{logsTFConfig.BaseUrl}/log";
		string? queryString = LogListQueryParams.GetQueryString(filter);
		if (queryString is not null)
		{
			url += $"?{queryString}";
		}

		EitherStrict<HttpError, LogListResult> logList =
			await httpClient.Get<LogListResult>(url);
		if (logList.IsLeft)
		{
			return logList.Left;
		}

		return logList.Right;
	}

	public async Task<EitherStrict<HttpError, GameLog>> GetGameLog(ulong logId)
	{
		logger.LogInformation($"fetching game log {logId}");
		var url = $"{logsTFConfig.BaseUrl}/log/{logId}";

		EitherStrict<HttpError, GameLog> logList =
			await httpClient.Get<GameLog>(url);
		if (logList.IsLeft)
		{
			return logList.Left;
		}

		return logList.Right;
	}

	public async Task<EitherStrict<HttpError, List<LogListItem>>> GetAllLogs()
	{
		logger.LogInformation("fetching game logs");

		ulong[] uploaders = logsTFConfig.Uploaders;
		List<LogListItem> logs = new();

		foreach (ulong uploader in uploaders)
		{
			LogListQueryParams filter = new()
			{
				Uploader = uploader
			};

			logger.LogInformation($"fetching game logs for uploader {uploader}");
			EitherStrict<HttpError, LogListResult> logListResult = await GetLogList(filter);
			if (logListResult.IsLeft)
			{
				return logListResult.Left;
			}

			List<LogListItem>? logList = logListResult.Right.Logs;
			if (logList is null)
			{
				logger.LogInformation($"No gamelogs found for uploader {uploader}");
			}
			else
			{
				logger.LogInformation($"uploader {uploader}: returned {logList.Count} logs");
				logs.AddRange(logList);
			}
		}

		return logs;
	}
}
