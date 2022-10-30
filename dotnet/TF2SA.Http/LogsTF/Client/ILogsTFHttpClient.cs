using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Models.LogListModel;

namespace TF2SA.Http.LogsTF.Client;

public interface ILogsTFHttpClient
{
	public Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId);
	public Task<EitherStrict<HttpError, LogListItem[]>> GetLogList(LogListQueryParams filter);
	public Task<EitherStrict<HttpError, LogListItem[]>> GetAllLogs(uint[] uploaders);
}
