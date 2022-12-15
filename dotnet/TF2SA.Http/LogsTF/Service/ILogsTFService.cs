using Monad;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Errors;

namespace TF2SA.Http.LogsTF.Service;

public interface ILogsTFService
{
	public Task<EitherStrict<HttpError, GameLog>> GetGameLog(
		ulong logId,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, LogListResult>> GetLogList(
		LogListQueryParams filter,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, List<LogListItem>>> GetAllLogs(
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, List<LogListItem>>> GetAllLogs(
		ulong uploader,
		CancellationToken cancellationToken
	);
}
