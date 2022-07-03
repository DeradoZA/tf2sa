namespace TF2SA.StatsETLService;

public class StatsETLServiceRunner : BackgroundService
{
    private readonly ILogger<StatsETLServiceRunner> logger;
    public IServiceProvider serviceProvider { get; }

    public StatsETLServiceRunner(
        ILogger<StatsETLServiceRunner> logger,
        IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Hosted service running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        logger.LogInformation("Pulling scoped service.");

        using (var scope = serviceProvider.CreateScope())
        {
            var scopedStatsETLService =
                scope.ServiceProvider.GetRequiredService<IStatsETLService>();

            await scopedStatsETLService.ProcessLogs(cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted service stopping.");
        await base.StopAsync(cancellationToken);
    }
}
