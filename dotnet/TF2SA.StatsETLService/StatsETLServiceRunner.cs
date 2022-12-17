using TF2SA.StatsETLService.LogsTFIngestion.Handlers;

namespace TF2SA.StatsETLService;

public class StatsETLServiceRunner : BackgroundService
{
	private readonly ILogger<StatsETLServiceRunner> logger;
	private readonly IServiceProvider serviceProvider;

	public StatsETLServiceRunner(
		ILogger<StatsETLServiceRunner> logger,
		IServiceProvider serviceProvider
	)
	{
		this.logger = logger;
		this.serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("StatsETLService running.");

		await DoWork(stoppingToken);
	}

	private async Task DoWork(CancellationToken cancellationToken)
	{
		using IServiceScope scope = serviceProvider.CreateScope();
		ILogsTFIngestionHandler scopedLogsTFIngestionHandler =
			scope.ServiceProvider.GetRequiredService<ILogsTFIngestionHandler>();

		await scopedLogsTFIngestionHandler.ExecuteAsync(cancellationToken);
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("StatsETLService stopping.");
		await base.StopAsync(cancellationToken);
	}
}
