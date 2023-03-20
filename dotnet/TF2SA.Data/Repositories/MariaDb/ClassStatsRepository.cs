using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class ClassStatsRepository : IClassStatsRepository<ClassStatEntity, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public ClassStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, ClassStatEntity>> Delete(
		ClassStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<ClassStatEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<ClassStatEntity> GetAllQueryable()
	{
		return tF2SADbContext.ClassStatsEntities.AsQueryable();
	}

	public Task<EitherStrict<Error, ClassStatEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStatEntity>> Insert(
		ClassStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStatEntity>> Update(
		ClassStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
