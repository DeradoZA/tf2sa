using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface IStatisticsUpdater
{
	Task<OptionStrict<Error>> UpdateAggregatedStatistics(
		CancellationToken cancellationToken
	);
}
