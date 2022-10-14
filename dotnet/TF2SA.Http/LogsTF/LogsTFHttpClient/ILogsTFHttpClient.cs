using Monad;
using TF2SA.Http.LogsTF.Models;

namespace TF2SA.Http.LogsTF.LogsTFHttpClient
{
    public interface ILogsTFHttpClient
    {
		Task<OptionStrict<GameLog>> GetGameLog(uint logId);
		Task<OptionStrict<LogList>> GetLogList(LogListQueryParams filter);
    }
}