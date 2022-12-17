namespace TF2SA.Data.Repositories.Base;

public interface ICrudRepository<TEntity, TId>
{
	// CREATE
	public TEntity Insert(TEntity entity);

	// READ
	public TEntity? GetById(TId id);
	public List<TEntity> GetAll();
	public IQueryable<TEntity> GetAllQueryable();

	// UPDATE
	public TEntity Update(TEntity entity);

	// DELETE
	public TEntity Delete(TEntity entity);
}
