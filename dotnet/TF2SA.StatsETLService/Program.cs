using NLog;
using NLog.Web;
using TF2SA.StatsETLService;

var logger = NLog.LogManager
	.Setup()
	.LoadConfigurationFromAppSettings()
	.GetCurrentClassLogger();
logger.Debug("init main");

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(
		(builderContext, services) =>
		{
			services.AddStatsETLService(builderContext.Configuration);
		}
	)
	.ConfigureLogging(logging =>
	{
		//logging.ClearProviders();
		logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
	})
	.UseNLog()
	.Build();

try
{
	await host.RunAsync();
}
catch (Exception exception)
{
	logger.Error(exception, "Stopped program because of exception");
	throw;
}
finally
{
	LogManager.Shutdown();
}
