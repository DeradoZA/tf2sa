using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Analytics;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class StatisticsUpdater : IStatisticsUpdater
{
	private readonly ILogger<StatisticsUpdater> logger;
	private readonly IAnalyticsStore analyticsStore;

	public StatisticsUpdater(
		ILogger<StatisticsUpdater> logger,
		IAnalyticsStore analyticsStore
	)
	{
		this.logger = logger;
		this.analyticsStore = analyticsStore;
	}

	public async Task<OptionStrict<Error>> UpdateAggregatedStatistics(
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation("Updating Aggregated statistics.");

		// ADR 0008 Phase 3: the stored-proc/pre-aggregated-table path (formerly 10
		// calls here, one per class/window) has been retired in favour of the DuckDB
		// analytics store, which now serves every leaderboard on its own. A refresh
		// failure is logged but stays non-fatal to the ingestion cycle, as it was
		// during the parallel run — the next cycle retries the full rebuild, and
		// leaderboards keep serving whatever DuckDB last loaded successfully.
		OptionStrict<Error> refreshAnalytics =
			await analyticsStore.RefreshAsync(cancellationToken);
		if (refreshAnalytics.HasValue)
		{
			logger.LogWarning(
				"DuckDB analytics refresh failed: {error}",
				refreshAnalytics.Value.Message
			);
		}
		return OptionStrict<Error>.Nothing;
	}
}
