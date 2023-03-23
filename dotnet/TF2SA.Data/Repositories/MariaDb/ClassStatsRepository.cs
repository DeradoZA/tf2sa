using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class ClassStatsRepository
	: IClassStatsRepository<ClassStatsEntity, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public ClassStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, ClassStatsEntity>> Delete(
		ClassStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<ClassStatsEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<ClassStatsEntity> GetAllQueryable()
	{
		return tF2SADbContext.ClassStatsEntities.AsQueryable();
	}

	public Task<EitherStrict<Error, ClassStatsEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStatsEntity>> Insert(
		ClassStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStatsEntity>> Update(
		ClassStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
