using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class StatisticsUpdater : IStatisticsUpdater
{
	private readonly IStatsRepository<ScoutRecent> scoutRecentStatsRepository;
	private readonly ILogger<StatisticsUpdater> logger;

	public StatisticsUpdater(
		IStatsRepository<ScoutRecent> scoutRecentStatsRepository,
		ILogger<StatisticsUpdater> logger
	)
	{
		this.scoutRecentStatsRepository = scoutRecentStatsRepository;
		this.logger = logger;
	}

	public async Task<OptionStrict<Error>> UpdateAggregatedStatistics(
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation("Updating Aggregated statistics.");

		OptionStrict<Error> updateScoutRecent =
			await scoutRecentStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateScoutRecent.HasValue)
		{
			return updateScoutRecent.Value;
		}

		return OptionStrict<Error>.Nothing;
	}
}
