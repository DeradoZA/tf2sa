using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogIngestionRepositoryUpdater : ILogIngestionRepositoryUpdater
{
	private readonly IGamesRepository<Game, uint> gamesRepository;

	public LogIngestionRepositoryUpdater(
		IGamesRepository<Game, uint> gamesRepository
	)
	{
		this.gamesRepository = gamesRepository;
	}

	public Task<OptionStrict<Error>> InsertInvalidLog(GameLog log)
	{
		throw new NotImplementedException();
	}

	public Task<OptionStrict<Error>> InsertValidLog(GameLog log)
	{
		throw new NotImplementedException();
	}
}
