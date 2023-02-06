using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public interface ILogIngestionRepositoryUpdater
{
	Task<OptionStrict<Error>> InsertInvalidLog(
		GameLog log,
		uint logId,
		List<Error> ingestionErrors,
		CancellationToken cancellationToken
	);
	Task<OptionStrict<Error>> InsertValidLog(
		GameLog log,
		uint logId,
		CancellationToken cancellationToken
	);
	Task<OptionStrict<Error>> UpdatePlayers(
		CancellationToken cancellationToken
	);
}
