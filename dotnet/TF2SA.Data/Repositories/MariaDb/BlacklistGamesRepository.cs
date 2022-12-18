using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class BlacklistGamesRepository
	: IBlacklistGamesRepository<BlacklistGame, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public BlacklistGamesRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public BlacklistGame Delete(BlacklistGame entity)
	{
		throw new NotImplementedException();
	}

	public List<BlacklistGame> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<BlacklistGame> GetAllQueryable()
	{
		return tF2SADbContext.BlacklistGames.AsQueryable();
	}

	public BlacklistGame? GetById(uint id)
	{
		throw new NotImplementedException();
	}

	public BlacklistGame Insert(BlacklistGame entity)
	{
		throw new NotImplementedException();
	}

	public BlacklistGame Update(BlacklistGame entity)
	{
		throw new NotImplementedException();
	}
}
