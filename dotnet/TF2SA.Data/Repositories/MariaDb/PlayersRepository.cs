using Monad;
using TF2SA.Common.Errors;
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

	public Task<EitherStrict<Error, Player>> Delete(Player entity)
	{
		throw new NotImplementedException();
	}

	public List<Player> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<Player> GetAllQueryable()
	{
		return tF2SADbContext.Players.AsQueryable();
	}

	public Task<EitherStrict<Error, Player?>> GetById(ulong id)
	{
		throw new NotImplementedException();
	}

	public List<Player> GetPlayerByName(string name)
	{
		var Result = GetAllQueryable()
			.Where(p => p.PlayerName == name)
			.ToList();
		return Result;
	}

	public Task<EitherStrict<Error, Player>> Insert(Player entity)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, Player>> Update(Player entity)
	{
		throw new NotImplementedException();
	}
}
