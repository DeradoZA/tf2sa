using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Serialization;

namespace TF2SA.Http.LogsTF.Client;

public class LogsTFHttpClient : ILogsTFHttpClient
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly ILogger<LogsTFHttpClient> logger;
	private const string BASE_URL = "http://logs.tf/api/v1";

	public LogsTFHttpClient(
		ILogger<LogsTFHttpClient> logger,
		IHttpClientFactory httpClientFactory)
	{
		this.logger = logger;
		this.httpClientFactory = httpClientFactory;
	}

	public async Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId)
	{
		logger.LogInformation($"fetching game log {logId}");
		HttpClient httpClient = httpClientFactory.CreateClient();

		try
		{
			var httpResponse = await httpClient.GetAsync($"{BASE_URL}/log/{logId}");
			var json = await httpResponse.Content.ReadAsStringAsync();

			EitherStrict<SerializationError, GameLog> serialized =
				LogsTFSerializer<GameLog>.Deserialize(json);
			if (serialized.IsLeft)
			{
				return new HttpError(serialized.Left.Message);
			}

			return serialized.Right;
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
