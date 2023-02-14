using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class StatsAggregationRepository : IStatsAggregationRepository
{
	private readonly TF2SADbContext dbContext;
	private readonly ILogger<StatsAggregationRepository> logger;

	public StatsAggregationRepository(
		TF2SADbContext dbContext,
		ILogger<StatsAggregationRepository> logger
	)
	{
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public async Task<
		EitherStrict<Error, List<ScoutRecent>>
	> GetAllScoutRecent()
	{
		try
		{
			return await GetAllScoutRecentQueryable().ToListAsync();
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to fetch scout recent stats with {e.Message}"
			);
		}
	}

	public IQueryable<ScoutRecent> GetAllScoutRecentQueryable()
	{
		return dbContext.ScoutRecents.AsQueryable();
	}

	public async Task<OptionStrict<Error>> UpdateScoutRecentStats(
		CancellationToken cancellationToken
	)
	{
		try
		{
			int updateResult = await dbContext.Database.ExecuteSqlRawAsync(
				"CALL UpdateScoutRecentStats();",
				cancellationToken: cancellationToken
			);
			logger.LogInformation("Updated {rows} rows", updateResult);
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to update scout recent stats with {e.Message}"
			);
		}
		return OptionStrict<Error>.Nothing;
	}
}
