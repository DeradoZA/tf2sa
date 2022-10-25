using Microsoft.Extensions.Logging;
using TF2SA.Http.LogsTF.Models;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Serialization;

namespace TF2SA.Http.LogsTF.Client;

public class LogsTFHttpClient : ILogsTFHttpClient
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly ILogger<LogsTFHttpClient> logger;
	private const string BASE_URL = "http://logs.tf/api/v1/log";

	public LogsTFHttpClient(
		ILogger<LogsTFHttpClient> logger,
		IHttpClientFactory httpClientFactory
	)
	{
		this.logger = logger;
		this.httpClientFactory = httpClientFactory;
	}

	public async Task GetGameLog(uint logId)
	{
		logger.LogInformation($"fetching game log {logId}");
		HttpClient httpClient = httpClientFactory.CreateClient();

		try
		{
			var httpResponse = await httpClient.GetAsync($"{BASE_URL}/{logId}");
			var json = await httpResponse.Content.ReadAsStringAsync();
			GameLog? serialized = LogsTFSerializer.DeserializeGameLog(json);
			logger.LogInformation(
				$"returned log: {serialized?.Success}\n"
					+ $"version: {serialized?.Version}\n"
					+ $"length: {serialized?.Length}\n"
					+ $"Red Team score: {serialized?.Teams?["Red"].Score}"
			);
		}
		catch (Exception e)
		{
			logger.LogError($"error fetching log: ${e}");
		}
		return;
	}

	public Task GetLogList(LogListQueryParams filter)
	{
		throw new NotImplementedException();
	}
}
