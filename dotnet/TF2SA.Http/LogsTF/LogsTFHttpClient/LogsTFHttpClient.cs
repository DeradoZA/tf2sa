using Monad;
using TF2SA.Http.LogsTF.Models;
using Microsoft.Extensions.Logging;

namespace TF2SA.Http.LogsTF.LogsTFHttpClient
{
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
				var result = await httpClient.GetAsync($"{BASE_URL}/{logId}");
				var json = result.Content;
				logger.LogInformation($"returned log: ${result}");
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
}
