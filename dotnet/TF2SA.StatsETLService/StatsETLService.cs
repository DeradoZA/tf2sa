using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.LogsTF.Service;

namespace TF2SA.StatsETLService;

internal class StatsETLService : IStatsETLService
{
	private int count = 0;
	private const int PROCESS_INTERVAL_SECONDS = 20;
	private readonly ILogger<StatsETLService> logger;
	private readonly IPlayersRepository<Player, ulong> playerRepository;
	private readonly ILogsTFService logsTFService;

	public StatsETLService(
		ILogger<StatsETLService> logger,
		IPlayersRepository<Player, ulong> playerRepository,
		ILogsTFService logsTFService
	)
	{
		this.logger = logger;
		this.playerRepository = playerRepository;
		this.logsTFService = logsTFService;
	}

	public async Task ProcessLogs(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;
			var allLogsResult = await logsTFService.GetAllLogs();
			if (allLogsResult.IsLeft)
			{
				// investigate 1 failing call
				logger.LogWarning("Failed to fetch list");
			}
			else
			{
				var logCount = allLogsResult.Right.Count;
				logger.LogInformation(
					"Fetched list of {count} results",
					logCount
				);
			}

			await Task.Delay(
				PROCESS_INTERVAL_SECONDS * 1000,
				cancellationToken
			);
		}
	}
}
