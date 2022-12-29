using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayerStatsRepository : IPlayerStatsRepository<PlayerStat, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public PlayerStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, PlayerStat>> Delete(PlayerStat entity)
	{
		throw new NotImplementedException();
	}

	public List<PlayerStat> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<PlayerStat> GetAllQueryable()
	{
		return tF2SADbContext.PlayerStats.AsQueryable();
	}

	public Task<EitherStrict<Error, PlayerStat?>> GetById(uint id)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStat>> Insert(PlayerStat entity)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStat>> Update(PlayerStat entity)
	{
		throw new NotImplementedException();
	}
}
