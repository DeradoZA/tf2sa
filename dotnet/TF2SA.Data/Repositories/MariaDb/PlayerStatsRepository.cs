using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayerStatsRepository
	: IPlayerStatsRepository<PlayerStatsEntity, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public PlayerStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, PlayerStatsEntity>> Delete(
		PlayerStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<PlayerStatsEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<PlayerStatsEntity> GetAllQueryable()
	{
		return tF2SADbContext.PlayerStatsEntities.AsQueryable();
	}

	public Task<EitherStrict<Error, PlayerStatsEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStatsEntity>> Insert(
		PlayerStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStatsEntity>> Update(
		PlayerStatsEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
