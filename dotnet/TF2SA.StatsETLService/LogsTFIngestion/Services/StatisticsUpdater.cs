using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class StatisticsUpdater : IStatisticsUpdater
{
	private readonly IStatsRepository<ScoutRecent> scoutRecentStatsRepository;
	private readonly IStatsRepository<ScoutAllTime> scoutAllTimeStatsRepository;
	private readonly ILogger<StatisticsUpdater> logger;

	public StatisticsUpdater(
		IStatsRepository<ScoutRecent> scoutRecentStatsRepository,
		ILogger<StatisticsUpdater> logger,
		IStatsRepository<ScoutAllTime> scoutAllTimeStatsRepository
	)
	{
		this.scoutRecentStatsRepository = scoutRecentStatsRepository;
		this.logger = logger;
		this.scoutAllTimeStatsRepository = scoutAllTimeStatsRepository;
	}

	public async Task<OptionStrict<Error>> UpdateAggregatedStatistics(
		CancellationToken cancellationToken
	)
	{
		logger.LogInformation("Updating Aggregated statistics.");
		List<Error> errors = new();

		OptionStrict<Error> updateScoutRecent =
			await scoutRecentStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateScoutRecent.HasValue)
		{
			errors.Add(updateScoutRecent.Value);
		}

		OptionStrict<Error> updateScoutAllTime =
			await scoutAllTimeStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateScoutRecent.HasValue)
		{
			errors.Add(updateScoutAllTime.Value);
		}

		if (errors.Any())
		{
			return new IngestionError(
				string.Join(Environment.NewLine, errors.Select(e => e.Message))
			);
		}
		return OptionStrict<Error>.Nothing;
	}
}
