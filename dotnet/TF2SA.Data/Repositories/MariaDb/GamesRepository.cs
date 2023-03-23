using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class GamesRepository : IGamesRepository<GamesEntity, uint>
{
	private readonly TF2SADbContext dbContext;

	public GamesRepository(TF2SADbContext dbContext)
	{
		this.dbContext = dbContext;
	}

	public Task<EitherStrict<Error, GamesEntity>> Delete(
		GamesEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<GamesEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<GamesEntity> GetAllQueryable()
	{
		return dbContext.GamesEntities.AsQueryable();
	}

	public async Task<EitherStrict<Error, GamesEntity?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		try
		{
			GamesEntity? entity = await dbContext.GamesEntities.FindAsync(
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

	public async Task<EitherStrict<Error, GamesEntity>> Insert(
		GamesEntity entity,
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

	public Task<EitherStrict<Error, GamesEntity>> Update(
		GamesEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
