using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Errors;
using static TF2SA.Data.Extensions.TF2SALinqExtensions;

namespace TF2SA.Data.Repositories.MariaDb.Generic;

public abstract class StatsRepository<TEntity> : IStatsRepository<TEntity>
	where TEntity : class
{
	private readonly TF2SADbContext dbContext;
	private readonly DbSet<TEntity> dbSet;
	private readonly ILogger logger;
	public abstract string UpdateProcQuery { get; }

	public abstract Dictionary<
		string,
		Expression<Func<TEntity, object>>
	> PropertyKeySelectors { get; }

	public abstract Tuple<
		string,
		Expression<Func<TEntity, object>>
	> DefaultSortField { get; }
	public abstract SortOrder DefaultSortOrder { get; }

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

	public IOrderedQueryable<TEntity> ApplySort(
		IQueryable<TEntity> queryable,
		string sortField,
		string sortOrder,
		out string sortFieldUsed,
		out string sortOrderUsed
	)
	{
		if (
			PropertyKeySelectors.TryGetValue(
				sortField,
				out Expression<Func<TEntity, object>>? sortSelector
			)
		)
		{
			sortFieldUsed = sortField;
		}
		else
		{
			logger.LogWarning(
				"Failed to apply provided sortField {providedSortField}. "
					+ "Defaulting to {defaultSortField}",
				sortField,
				DefaultSortField.Item1
			);
			sortFieldUsed = DefaultSortField.Item1;
			sortSelector = DefaultSortField.Item2;
		}

		SortOrder appliedSortOrder = GetSortOrder(sortOrder);
		sortOrderUsed = appliedSortOrder.ToString();

		if (appliedSortOrder == SortOrder.asc)
		{
			return queryable.OrderBy(sortSelector);
		}
		return queryable.OrderByDescending(sortSelector);
	}

	private SortOrder GetSortOrder(string sortOrder)
	{
		if (Enum.TryParse(sortOrder, out SortOrder sort))
		{
			return sort;
		}
		logger.LogWarning(
			"Failed to parse sort order, defaulting to {sortOrder}",
			DefaultSortOrder.ToString()
		);
		return DefaultSortOrder;
	}
}
