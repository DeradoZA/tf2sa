using Monad;
using TF2SA.Http.LogsTF.Models;

namespace TF2SA.Http.LogsTF.LogsTFHttpClient
{
    public interface ILogsTFHttpClient
    {
		Task GetGameLog(uint logId);
		Task GetLogList(LogListQueryParams filter);
    }
}