using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models;
using TF2SA.Http.LogsTF.Models.GameLogModel;

namespace TF2SA.Http.LogsTF.Client;

public interface ILogsTFHttpClient
{
	public Task<EitherStrict<HttpError, GameLog>> GetGameLog(uint logId);
	public Task GetLogList(LogListQueryParams filter);
}
