using TF2SA.Http;
using TF2SA.StatsETLService.LogsTFIngestion.Configuration;
using TF2SA.StatsETLService.LogsTFIngestion.Handlers;

namespace TF2SA.StatsETLService;

public static class DependencyInjection
{
	public static void AddStatsETLService(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services.AddHttpServices(configuration);
		services.AddHostedService<StatsETLServiceRunner>();
		services.Configure<LogsTFIngestionConfig>(
			configuration.GetSection(
                key: "LogsTFIngestion"
			)
		);
		services.AddScoped<ILogsTFIngestionHandler, LogsTFIngestionHandler>();
	}
}
