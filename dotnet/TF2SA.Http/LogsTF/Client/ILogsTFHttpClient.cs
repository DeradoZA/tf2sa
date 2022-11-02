using Monad;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Errors;

namespace TF2SA.Http.LogsTF.Client;

public interface ILogsTFHttpClient
{
	public Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId);
	public Task<EitherStrict<HttpError, LogListItem[]>> GetLogList(LogListQueryParams filter);
	public Task<EitherStrict<HttpError, LogListItem[]>> GetAllLogs(uint[] uploaders);
}
