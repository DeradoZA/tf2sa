using Monad;
using TF2SA.Http.LogsTF.Models;
using Microsoft.Extensions.Logging;

namespace TF2SA.Http.LogsTF.LogsTFHttpClient
{
    public class LogsTFHttpClient : ILogsTFHttpClient
    {
		private readonly HttpClient httpClient;
		private readonly ILogger<LogsTFHttpClient> logger;

        public LogsTFHttpClient(
            HttpClient httpClient,
            ILogger<LogsTFHttpClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public Task<OptionStrict<GameLog>> GetGameLog(uint logId)
        {
            throw new NotImplementedException();
        }

        public Task<OptionStrict<LogList>> GetLogList(LogListQueryParams filter)
        {
            throw new NotImplementedException();
        }
    }
}