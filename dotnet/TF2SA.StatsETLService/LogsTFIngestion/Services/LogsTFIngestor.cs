using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogsTFIngestor : ILogsTFIngestor
{
	private readonly ILogger<LogsTFIngestor> logger;
	private readonly ILogsTFService logsTFService;
	private readonly IGamesRepository<Game, uint> gamesRepository;
	private readonly IBlacklistGamesRepository<
		BlacklistGame,
		uint
	> blacklistGamesRepository;

	public LogsTFIngestor(
		ILogger<LogsTFIngestor> logger,
		ILogsTFService logsTFService,
		IGamesRepository<Game, uint> gamesRepository,
		IBlacklistGamesRepository<BlacklistGame, uint> blacklistGamesRepository
	)
	{
		this.logger = logger;
		this.logsTFService = logsTFService;
		this.gamesRepository = gamesRepository;
		this.blacklistGamesRepository = blacklistGamesRepository;
	}

	public async Task<EitherStrict<Error, List<LogListItem>>> GetLogsToProcess(
		CancellationToken cancellationToken
	)
	{
		EitherStrict<HttpError, List<LogListItem>> allLogsResult =
			await logsTFService.GetAllLogs(cancellationToken);
		if (allLogsResult.IsLeft)
		{
			return allLogsResult.Left;
		}

		List<Game> processedLogs = gamesRepository.GetAll();
		List<BlacklistGame> BlacklistGames = blacklistGamesRepository.GetAll();

		return allLogsResult.Right;
	}
}
