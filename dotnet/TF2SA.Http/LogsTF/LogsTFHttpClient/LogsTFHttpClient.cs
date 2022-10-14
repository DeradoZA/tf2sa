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
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public Task<OptionStrict<GameLog>> GetGameLog(uint logId)
        {
			HttpClient httpClient = httpClientFactory.CreateClient();

			try {
				//var result = await httpClient.
			} catch (Exception e){

			}
        }

        public Task<OptionStrict<LogList>> GetLogList(LogListQueryParams filter)
        {
            throw new NotImplementedException();
        }
    }
}