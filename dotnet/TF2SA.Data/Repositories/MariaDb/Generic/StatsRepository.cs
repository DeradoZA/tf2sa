using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Errors;

namespace TF2SA.Data.Repositories.MariaDb.Generic;

public abstract class StatsRepository<TEntity> : IStatsRepository<TEntity>
	where TEntity : class
{
	private readonly TF2SADbContext dbContext;
	private readonly DbSet<TEntity> dbSet;
	private readonly ILogger logger;
	public abstract string UpdateProcQuery { get; }

	public StatsRepository(TF2SADbContext dbContext, ILogger logger)
	{
		this.dbContext = dbContext;
		this.logger = logger;
		dbSet = this.dbContext.Set<TEntity>();
	}

	public async Task<EitherStrict<Error, List<TEntity>>> GetAll(
		CancellationToken cancellationToken
	)
	{
		try
		{
			return await GetAllQueryable()
				.ToListAsync(cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to fetch all records of {typeof(TEntity)}: {e.Message}"
			);
		}
	}

	public IQueryable<TEntity> GetAllQueryable()
	{
		return dbSet.AsQueryable();
	}

	public async Task<OptionStrict<Error>> CallUpdateStoredProc(
		CancellationToken cancellationToken
	)
	{
		try
		{
			int updateResult = await dbContext.Database.ExecuteSqlRawAsync(
				UpdateProcQuery,
				cancellationToken: cancellationToken
			);
			logger.LogInformation(
				"Updated {rows} rows for {type}",
				updateResult,
				typeof(TEntity)
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to update {typeof(TEntity)} stats with {e.Message}"
			);
		}
		return OptionStrict<Error>.Nothing;
	}
}
