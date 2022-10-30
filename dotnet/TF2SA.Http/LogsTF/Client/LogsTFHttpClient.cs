using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Config;
using TF2SA.Http.LogsTF.Models;
using TF2SA.Http.LogsTF.Models.GameLogModel;
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

	public async Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId)
	{
		logger.LogInformation($"fetching game log {logId}");
		HttpClient httpClient = httpClientFactory.CreateClient();

		try
		{
			var httpResponse = await httpClient.GetAsync($"{logsTFConfig.BaseUrl}/log/{logId}");
			var json = await httpResponse.Content.ReadAsStringAsync();

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

	public Task GetLogList(LogListQueryParams filter)
	{
		throw new NotImplementedException();
	}
}
