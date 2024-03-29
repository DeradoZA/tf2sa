using TF2SA.Data;
using TF2SA.Http;
using TF2SA.StatsETLService.LogsTFIngestion.Configuration;
using TF2SA.StatsETLService.LogsTFIngestion.Handlers;
using TF2SA.StatsETLService.LogsTFIngestion.Services;

namespace TF2SA.StatsETLService;

public static class DependencyInjection
{
	public static void AddStatsETLService(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services.Configure<LogsTFIngestionConfig>(
			configuration.GetSection(
				LogsTFIngestionConfig.LogsTFIngestionConfigSection
			)
		);
		services.AddDataLayer(configuration);
		services.AddScoped<ILogsTFIngestionHandler, LogsTFIngestionHandler>();
		services.AddScoped<ILogIngestor, LogIngestor>();
		services.AddScoped<
			ILogIngestionRepositoryUpdater,
			LogIngestionRepositoryUpdater
		>();
		services.AddScoped<IStatisticsUpdater, StatisticsUpdater>();
		services.AddHttpServices(configuration);
		services.AddHostedService<StatsETLServiceRunner>();
	}
}
