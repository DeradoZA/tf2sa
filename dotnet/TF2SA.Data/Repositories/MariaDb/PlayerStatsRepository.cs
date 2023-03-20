using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayerStatsRepository
	: IPlayerStatsRepository<PlayerStatEntity, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public PlayerStatsRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
	}

	public Task<EitherStrict<Error, PlayerStatEntity>> Delete(
		PlayerStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<PlayerStatEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<PlayerStatEntity> GetAllQueryable()
	{
		return tF2SADbContext.PlayerStatsEntities.AsQueryable();
	}

	public Task<EitherStrict<Error, PlayerStatEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStatEntity>> Insert(
		PlayerStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, PlayerStatEntity>> Update(
		PlayerStatEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
