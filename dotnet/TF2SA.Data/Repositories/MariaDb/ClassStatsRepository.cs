using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class ClassStatsRepository : IClassStatsRepository<ClassStat, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public ClassStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, ClassStat>> Delete(ClassStat entity)
	{
		throw new NotImplementedException();
	}

	public List<ClassStat> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<ClassStat> GetAllQueryable()
	{
		return tF2SADbContext.ClassStats.AsQueryable();
	}

	public Task<EitherStrict<Error, ClassStat?>> GetById(uint id)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStat>> Insert(ClassStat entity)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, ClassStat>> Update(ClassStat entity)
	{
		throw new NotImplementedException();
	}
}
