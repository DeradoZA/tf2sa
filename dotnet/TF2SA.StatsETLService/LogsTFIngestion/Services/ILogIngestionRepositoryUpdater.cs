using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface ILogIngestionRepositoryUpdater
{
	Task<OptionStrict<Error>> InsertInvalidLog(GameLog log);
	Task<OptionStrict<Error>> InsertValidLog(GameLog log);
}
