namespace TF2SA.Data.Repositories.Base
{
    public interface ICrudRepository<TEntity, TId>
    {
        //CREATE
        public TEntity Insert(TEntity entity);
        //READ
        public TEntity GetById(TId id);
        public List<TEntity> GetAll();
        public IQueryable<TEntity> GetAllQueryable();
        //UPDATE
        public TEntity UpdateEntity(TEntity entity);
        //DELETE
        public bool DeleteEntity(TEntity entity);
    }
}