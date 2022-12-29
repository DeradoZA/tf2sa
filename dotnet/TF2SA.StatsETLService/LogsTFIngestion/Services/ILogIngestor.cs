using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface ILogIngestor
{
	Task<OptionStrict<List<Error>>> IngestLog(
		LogListItem logListItem,
		CancellationToken cancellationToken
	);
}
