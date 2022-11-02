using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Config;
using TF2SA.Http.LogsTF.Serialization;

namespace TF2SA.Http.LogsTF.Client;

public class LogsTFHttpClient : ILogsTFHttpClient
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly ILogger<LogsTFHttpClient> logger;
	private readonly LogsTFConfig logsTFConfig;

	public LogsTFHttpClient(
		ILogger<LogsTFHttpClient> logger,
		IHttpClientFactory httpClientFactory,
		IOptions<LogsTFConfig> logsTFConfig)
	{
		this.logger = logger;
		this.httpClientFactory = httpClientFactory;
		this.logsTFConfig = logsTFConfig.Value;
	}

	public Task<EitherStrict<HttpError, LogListItem[]>> GetAllLogs(uint[] uploaders)
	{
		throw new NotImplementedException();
	}

	public async Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId)
	{
		logger.LogInformation($"fetching game log {logId}");
		HttpClient httpClient = httpClientFactory.CreateClient();

		try
		{
			var response = await httpClient.GetAsync($"{logsTFConfig.BaseUrl}/log/{logId}");
			var json = await response.Content.ReadAsStringAsync();

			EitherStrict<SerializationError, GameLog> deserialized =
				LogsTFSerializer<GameLog>.Deserialize(json);
			if (deserialized.IsLeft)
			{
				return new HttpError(deserialized.Left.Message);
			}

			return deserialized.Right;
		}
		catch (Exception e)
		{
			return new HttpError(e.Message);
		}
	}

	public async Task<EitherStrict<HttpError, LogListItem[]>> GetLogList(LogListQueryParams filter)
	{
		logger.LogInformation($"Fetching log list");

		HttpClient httpClient = httpClientFactory.CreateClient();

		string url = $"{logsTFConfig.BaseUrl}/log";
		string? queryString = LogListQueryParams.GetQueryString(filter);
		if (queryString is not null)
		{
			url += $"?{queryString}";
		}

		try
		{
			var response = await httpClient.GetAsync(url);
			var json = await response.Content.ReadAsStringAsync();

			EitherStrict<SerializationError, LogListResult> deserialized =
				LogsTFSerializer<LogListResult>.Deserialize(json);
			if (deserialized.IsLeft)
			{
				return new HttpError(deserialized.Left.Message);
			}

			return deserialized.Right.Logs;
		}
		catch (Exception e)
		{
			return new HttpError(e.Message);
		}
	}
}
