using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data.Repositories.Base;

public interface IStatsAggregationRepository
{
	public Task<OptionStrict<Error>> UpdateScoutRecentStats(
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<Error, List<ScoutRecent>>> GetAllScoutRecent();
	public IQueryable<ScoutRecent> GetAllScoutRecentQueryable();
}
