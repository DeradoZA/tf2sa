using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class GamesRepository : IGamesRepository<Game, uint>
{
	private readonly TF2SADbContext dbContext;
	private readonly ILogger<GamesRepository> logger;

	public GamesRepository(
		TF2SADbContext dbContext,
		ILogger<GamesRepository> logger
	)
	{
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public Task<EitherStrict<Error, Game>> Delete(
		Game entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<Game> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<Game> GetAllQueryable()
	{
		return dbContext.Games.AsQueryable();
	}

	public async Task<EitherStrict<Error, Game?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		try
		{
			Game? entity = await dbContext.Games.FindAsync(
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

	public async Task<EitherStrict<Error, Game>> Insert(
		Game entity,
		CancellationToken cancellationToken
	)
	{
		try
		{
			await dbContext.Games.AddAsync(entity, cancellationToken);
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

	public Task<EitherStrict<Error, Game>> Update(
		Game entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
