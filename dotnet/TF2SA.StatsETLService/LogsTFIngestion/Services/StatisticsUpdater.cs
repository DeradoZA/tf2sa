using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class StatisticsUpdater : IStatisticsUpdater
{
	private readonly ILogger<StatisticsUpdater> logger;
	private readonly IStatsRepository<ScoutRecentEntity> scoutRecentStatsRepository;
	private readonly IStatsRepository<ScoutAllTimeEntity> scoutAllTimeStatsRepository;
	private readonly IStatsRepository<SoldierRecentEntity> soldierRecentStatsRepository;
	private readonly IStatsRepository<SoldierAllTimeEntity> soldierAllTimeStatsRepository;
	private readonly IStatsRepository<DemomanRecentEntity> demomanRecentStatsRepository;
	private readonly IStatsRepository<DemomanAllTimeEntity> demomanAllTimeStatsRepository;
	private readonly IStatsRepository<MedicRecentEntity> medicRecentStatsRepository;
	private readonly IStatsRepository<MedicAllTimeEntity> medicAllTimeStatsRepository;
	private readonly IStatsRepository<OverallStatsRecentEntity> overallStatsRecentRepository;
	private readonly IStatsRepository<OverallStatsAllTimeEntity> overallStatsAllTimeRepository;

	public StatisticsUpdater(
		ILogger<StatisticsUpdater> logger,
		IStatsRepository<ScoutRecentEntity> scoutRecentStatsRepository,
		IStatsRepository<ScoutAllTimeEntity> scoutAllTimeStatsRepository,
		IStatsRepository<SoldierRecentEntity> soldierRecentStatsRepository,
		IStatsRepository<SoldierAllTimeEntity> soldierAllTimeStatsRepository,
		IStatsRepository<DemomanRecentEntity> demomanRecentStatsRepository,
		IStatsRepository<DemomanAllTimeEntity> demomanAllTimeStatsRepository,
		IStatsRepository<MedicRecentEntity> medicRecentStatsRepository,
		IStatsRepository<MedicAllTimeEntity> medicAllTimeStatsRepository,
		IStatsRepository<OverallStatsRecentEntity> overallStatsRecentRepository,
		IStatsRepository<OverallStatsAllTimeEntity> overallStatsAllTimeRepository
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
