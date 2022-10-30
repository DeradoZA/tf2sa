using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayersRepository : IPlayersRepository<Player, ulong>
{
	private readonly TF2SADbContext tF2SADbContext;

	public PlayersRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Player Delete(Player entity)
	{
		tF2SADbContext.Players.Remove(entity);
		tF2SADbContext.SaveChanges();
		return entity;
	}

	public List<Player> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<Player> GetAllQueryable()
	{
		return tF2SADbContext.Players.AsQueryable();
	}

	public Player? GetById(ulong id)
	{
		var result = tF2SADbContext.Players.Find(id);
		return result;
	}

	public List<Player> GetPlayerByName(string name)
	{
		var Result = GetAllQueryable()
			.Where(p => p.PlayerName == name)
			.ToList();
		return Result;
	}

	public Player Insert(Player entity)
	{
		tF2SADbContext.Players.Add(entity);
		tF2SADbContext.SaveChanges();
		return entity;
	}

	public Player Update(Player entity)
	{
		tF2SADbContext.Players.Update(entity);
		tF2SADbContext.SaveChanges();
		return entity;
	}
}
