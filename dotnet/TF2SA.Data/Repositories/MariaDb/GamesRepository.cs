using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb;

public class GamesRepository : IGamesRepository<Game, uint>
{
	private readonly TF2SADbContext tF2SADbContext;

	public GamesRepository(TF2SADbContext tF2SADbContext)
	{
		this.tF2SADbContext = tF2SADbContext;
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
		return tF2SADbContext.Games.AsQueryable();
	}

	public Task<EitherStrict<Error, Game?>> GetById(
		uint id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, Game>> Insert(
		Game entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<Error, Game>> Update(
		Game entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
