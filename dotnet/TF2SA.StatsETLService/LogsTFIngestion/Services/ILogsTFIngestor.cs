using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface ILogsTFIngestor
{
	public Task<EitherStrict<Error, List<LogListItem>>> GetLogsToProcess(
		CancellationToken cancellationToken
	);
}
