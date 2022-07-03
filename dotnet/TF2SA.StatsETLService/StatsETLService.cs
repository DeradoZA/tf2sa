namespace TF2SA.StatsETLService
{
    internal class StatsETLService : IStatsETLService
    {
        private int count = 0;
        private readonly ILogger<StatsETLService> logger;

        public StatsETLService(ILogger<StatsETLService> logger)
        {
            this.logger = logger;
        }

        public async Task ProcessLogs(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                count++;
                logger.LogInformation($"Scoped Service executing: {count}");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}