using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.StatsETLService
{
    internal class StatsETLService : IStatsETLService
    {
        private int count = 0;
        private readonly ILogger<StatsETLService> logger;
        private readonly IPlayersRepository<Player, ulong> playerRepository;

        public StatsETLService(
            ILogger<StatsETLService> logger,
            IPlayersRepository<Player, ulong> playerRepository)
        {
            this.logger = logger;
            this.playerRepository = playerRepository;
        }

        public async Task ProcessLogs(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                count++;
                var playerCount = playerRepository.GetAll().Count;
                logger.LogInformation($"Scoped Service executing: {count}, found {playerCount} players!");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}