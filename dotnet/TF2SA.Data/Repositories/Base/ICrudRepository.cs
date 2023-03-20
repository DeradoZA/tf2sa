using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Data.Repositories.Base;

public interface ICrudRepository<TEntity, TId>
{
	// CREATE
	public Task<EitherStrict<Error, TEntity>> Insert(
		TEntity entity,
		CancellationToken cancellationToken
	);

	// READ
	public Task<EitherStrict<Error, TEntity?>> GetById(
		TId id,
		CancellationToken cancellationToken
	);
	public List<TEntity> GetAll();
	public IQueryable<TEntity> GetAllQueryable();

	// UPDATE
	public Task<EitherStrict<Error, TEntity>> Update(
		TEntity entity,
		CancellationToken cancellationToken
	);

	// DELETE
	public Task<EitherStrict<Error, TEntity>> Delete(
		TEntity entity,
		CancellationToken cancellationToken
	);
}
