using TF2SA.Common.Models.LogsTF.LogListModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface ILogIngestor
{
	Task<bool> IngestLog(
		LogListItem logListItem,
		CancellationToken cancellationToken
	);
}
