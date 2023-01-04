using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using System.Data;
using TF2SA.Data.Errors;
using Microsoft.EntityFrameworkCore;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayersRepository : IPlayersRepository<Player, ulong>
{
	private readonly TF2SADbContext dbContext;

	public PlayersRepository(TF2SADbContext dbContext)
	{
		this.dbContext = dbContext;
	}

	public Task<EitherStrict<Error, Player>> Delete(
		Player entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<Player> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<Player> GetAllQueryable()
	{
		return dbContext.Players.AsQueryable();
	}

	public Task<EitherStrict<Error, Player?>> GetById(
		ulong id,
		CancellationToken cancellationToken
	)
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

	public Task<EitherStrict<Error, Player>> Insert(
		Player entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	// TODO generify InsertPlayersIfNotExists as DbSet extension method
	// this can be added as an extension method on the DbSet
	// milestone: StatsETL
	public async Task<OptionStrict<Error>> InsertPlayersIfNotExists(
		IEnumerable<Player> players,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IEnumerable<ulong> keys = players.Select(p => p.SteamId);
			List<ulong> existingEntites = await dbContext.Players
				.Select(p => p.SteamId)
				.Where(id => keys.Contains(id))
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ulong> idsToAdd = keys.Except(existingEntites);
			IEnumerable<Player> playersToAdd = players.Where(
				p => idsToAdd.Contains(p.SteamId)
			);

			await dbContext.AddRangeAsync(playersToAdd, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}

		return OptionStrict<Error>.Nothing;
	}

	public Task<EitherStrict<Error, Player>> Update(
		Player entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
