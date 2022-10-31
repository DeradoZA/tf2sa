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

	public ClassStat Delete(ClassStat entity)
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

	public ClassStat? GetById(uint id)
	{
		throw new NotImplementedException();
	}

	public ClassStat Insert(ClassStat entity)
	{
		throw new NotImplementedException();
	}

	public ClassStat Update(ClassStat entity)
	{
		throw new NotImplementedException();
	}
}
