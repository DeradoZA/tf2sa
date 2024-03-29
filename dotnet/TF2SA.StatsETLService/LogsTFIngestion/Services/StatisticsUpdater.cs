using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class StatisticsUpdater : IStatisticsUpdater
{
	private readonly ILogger<StatisticsUpdater> logger;
	private readonly IStatsRepository<ScoutRecent> scoutRecentStatsRepository;
	private readonly IStatsRepository<ScoutAllTime> scoutAllTimeStatsRepository;
	private readonly IStatsRepository<SoldierRecent> soldierRecentStatsRepository;
	private readonly IStatsRepository<SoldierAllTime> soldierAllTimeStatsRepository;
	private readonly IStatsRepository<DemomanRecent> demomanRecentStatsRepository;
	private readonly IStatsRepository<DemomanAllTime> demomanAllTimeStatsRepository;
	private readonly IStatsRepository<MedicRecent> medicRecentStatsRepository;
	private readonly IStatsRepository<MedicAllTime> medicAllTimeStatsRepository;
	private readonly IStatsRepository<OverallStatsRecent> overallStatsRecentRepository;
	private readonly IStatsRepository<OverallStatsAllTime> overallStatsAllTimeRepository;

	public StatisticsUpdater(
		ILogger<StatisticsUpdater> logger,
		IStatsRepository<ScoutRecent> scoutRecentStatsRepository,
		IStatsRepository<ScoutAllTime> scoutAllTimeStatsRepository,
		IStatsRepository<SoldierRecent> soldierRecentStatsRepository,
		IStatsRepository<SoldierAllTime> soldierAllTimeStatsRepository,
		IStatsRepository<DemomanRecent> demomanRecentStatsRepository,
		IStatsRepository<DemomanAllTime> demomanAllTimeStatsRepository,
		IStatsRepository<MedicRecent> medicRecentStatsRepository,
		IStatsRepository<MedicAllTime> medicAllTimeStatsRepository,
		IStatsRepository<OverallStatsRecent> overallStatsRecentRepository,
		IStatsRepository<OverallStatsAllTime> overallStatsAllTimeRepository
	)
	{
		this.logger = logger;
		this.scoutRecentStatsRepository = scoutRecentStatsRepository;
		this.scoutAllTimeStatsRepository = scoutAllTimeStatsRepository;
		this.soldierRecentStatsRepository = soldierRecentStatsRepository;
		this.soldierAllTimeStatsRepository = soldierAllTimeStatsRepository;
		this.demomanRecentStatsRepository = demomanRecentStatsRepository;
		this.demomanAllTimeStatsRepository = demomanAllTimeStatsRepository;
		this.medicRecentStatsRepository = medicRecentStatsRepository;
		this.medicAllTimeStatsRepository = medicAllTimeStatsRepository;
		this.overallStatsRecentRepository = overallStatsRecentRepository;
		this.overallStatsAllTimeRepository = overallStatsAllTimeRepository;
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

		OptionStrict<Error> updateSoldierRecent =
			await soldierRecentStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateSoldierRecent.HasValue)
		{
			errors.Add(updateSoldierRecent.Value);
		}

		OptionStrict<Error> updateSoldierAllTime =
			await soldierAllTimeStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateSoldierAllTime.HasValue)
		{
			errors.Add(updateSoldierAllTime.Value);
		}

		OptionStrict<Error> updateDemomanRecent =
			await demomanRecentStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateDemomanRecent.HasValue)
		{
			errors.Add(updateDemomanRecent.Value);
		}

		OptionStrict<Error> updateDemomanAllTime =
			await demomanAllTimeStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateDemomanAllTime.HasValue)
		{
			errors.Add(updateDemomanAllTime.Value);
		}

		OptionStrict<Error> updatemedicRecent =
			await medicRecentStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updatemedicRecent.HasValue)
		{
			errors.Add(updatemedicRecent.Value);
		}

		OptionStrict<Error> updatemedicAllTime =
			await medicAllTimeStatsRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updatemedicAllTime.HasValue)
		{
			errors.Add(updatemedicAllTime.Value);
		}

		OptionStrict<Error> updateOverallRecent =
			await overallStatsRecentRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateOverallRecent.HasValue)
		{
			errors.Add(updateOverallRecent.Value);
		}

		OptionStrict<Error> updateOverallAllTime =
			await overallStatsAllTimeRepository.CallUpdateStoredProc(
				cancellationToken
			);
		if (updateOverallAllTime.HasValue)
		{
			errors.Add(updateOverallAllTime.Value);
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
