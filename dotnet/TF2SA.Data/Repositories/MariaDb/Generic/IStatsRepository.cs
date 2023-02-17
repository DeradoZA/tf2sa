using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Data.Repositories.MariaDb.Generic;

public interface IStatsRepository<TEntity> where TEntity : class
{
	Task<EitherStrict<Error, List<TEntity>>> GetAll(
		CancellationToken cancellationToken
	);
	IQueryable<TEntity> GetAllQueryable();
	IOrderedQueryable<TEntity> ApplySort(
		IQueryable<TEntity> queryable,
		string sortField,
		string sortOrder,
		out string sortFieldUsed,
		out string sortOrderUsed
	);
	Task<OptionStrict<Error>> CallUpdateStoredProc(
		CancellationToken cancellationToken
	);
}
