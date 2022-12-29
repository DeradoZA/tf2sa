using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Data.Repositories.Base;

public interface ICrudRepository<TEntity, TId>
{
	// CREATE
	public Task<EitherStrict<Error, TEntity>> Insert(TEntity entity);

	// READ
	public Task<EitherStrict<Error, TEntity?>> GetById(TId id);
	public List<TEntity> GetAll();
	public IQueryable<TEntity> GetAllQueryable();

	// UPDATE
	public Task<EitherStrict<Error, TEntity>> Update(TEntity entity);

	// DELETE
	public Task<EitherStrict<Error, TEntity>> Delete(TEntity entity);
}
