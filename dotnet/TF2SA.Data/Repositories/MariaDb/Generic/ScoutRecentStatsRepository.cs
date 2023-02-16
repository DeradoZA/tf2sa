using Microsoft.Extensions.Logging;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data.Repositories.MariaDb.Generic;

public class ScoutRecentStatsRepository : StatsRepository<ScoutRecent>
{
	public override string UpdateProcQuery => "CALL UpdateScoutRecentStats();";

	public ScoutRecentStatsRepository(
		TF2SADbContext dbContext,
		ILogger<ScoutRecentStatsRepository> logger
	) : base(dbContext, logger) { }
}
