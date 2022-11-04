using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.LogsTF.Client;

namespace TF2SA.StatsETLService;

internal class StatsETLService : IStatsETLService
{
	private int count = 0;
	private const int PROCESS_INTERVAL_SECONDS = 20;
	private readonly ILogger<StatsETLService> logger;
	private readonly IPlayersRepository<Player, ulong> playerRepository;
	private readonly ILogsTFService logsTFHttpClient;

	public StatsETLService(
		ILogger<StatsETLService> logger,
		IPlayersRepository<Player, ulong> playerRepository,
		ILogsTFService logsTFHttpClient
	)
	{
		this.logger = logger;
		this.playerRepository = playerRepository;
		this.logsTFHttpClient = logsTFHttpClient;
	}

	public async Task ProcessLogs(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;
			var playerCount = playerRepository.GetAll().Count;
			logger.LogInformation(
				$"Scoped Service executing: {count}, found {playerCount} players!"
			);
			var allLogsResult = await logsTFHttpClient.GetLogList(
				new LogListQueryParams()
				{
					Uploader = 76561199085369255
				}
			);
			if (allLogsResult.IsLeft)
			{
				logger.LogWarning("Failed to fetch list");
			}
			var allLogs = allLogsResult.Right.Length;
			logger.LogInformation($"Fetched list of {allLogs} results");

			await Task.Delay(PROCESS_INTERVAL_SECONDS * 1000, cancellationToken);
		}
	}
}
