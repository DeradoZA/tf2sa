using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class GamesRepository : IGamesRepository<GameEntity, uint>
{
	private readonly TF2SADbContext dbContext;

	public GamesRepository(TF2SADbContext dbContext)
	{
		this.dbContext = dbContext;
	}

	public Task<EitherStrict<Error, GameEntity>> Delete(
		GameEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<GameEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<GameEntity> GetAllQueryable()
	{
		return dbContext.GamesEntities.AsQueryable();
	}

	public async Task<EitherStrict<Error, GameEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		try
		{
			GameEntity? entity = await dbContext.GamesEntities.FindAsync(
				new object?[] { id },
				cancellationToken: cancellationToken
			);
			return entity;
		}
		catch (Exception e)
		{
			return new DatabaseError($"Failed to fetch game {id}: {e.Message}");
		}
	}

	public async Task<EitherStrict<Error, GameEntity>> Insert(
		GameEntity entity,
		CancellationToken cancellationToken
	)
	{
		try
		{
			await dbContext.GamesEntities.AddAsync(entity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
			return entity;
		}
		catch (Exception e)
		{
			return new DatabaseError(
				$"Failed to insert game with error: {e.Message}"
			);
		}
	}

	public Task<EitherStrict<Error, GameEntity>> Update(
		GameEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
