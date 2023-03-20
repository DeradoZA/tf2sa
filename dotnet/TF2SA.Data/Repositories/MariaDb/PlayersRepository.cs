using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using System.Data;
using TF2SA.Data.Errors;
using Microsoft.EntityFrameworkCore;

namespace TF2SA.Data.Repositories.MariaDb;

public class PlayersRepository : IPlayersRepository<PlayerEntity, ulong>
{
	private readonly TF2SADbContext dbContext;

	public PlayersRepository(TF2SADbContext dbContext)
	{
		this.dbContext = dbContext;
	}

	public Task<EitherStrict<Error, PlayerEntity>> Delete(
		PlayerEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<PlayerEntity> GetAll()
	{
		return GetAllQueryable().ToList();
	}

	public IQueryable<PlayerEntity> GetAllQueryable()
	{
		return dbContext.PlayersEntities.AsQueryable();
	}

	public Task<EitherStrict<Error, PlayerEntity?>> GetById(
		ulong id,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public List<PlayerEntity> GetPlayerByName(string name)
	{
		var Result = GetAllQueryable()
			.Where(p => p.PlayerName == name)
			.ToList();
		return Result;
	}

	public Task<EitherStrict<Error, PlayerEntity>> Insert(
		PlayerEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	// TODO generify InsertPlayersIfNotExists as DbSet extension method
	// this can be added as an extension method on the DbSet
	// milestone: StatsETL

	// TODO investigate occasional Duplicate entry exception
	// MySqlConnector.MySqlException (0x80004005): Duplicate entry '76561198107170907' for key 'PRIMARY'
	// milestone: 7
	public async Task<OptionStrict<Error>> InsertPlayersIfNotExists(
		IEnumerable<PlayerEntity> players,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IEnumerable<ulong> keys = players.Select(p => p.SteamId);
			List<ulong> existingEntites = await dbContext.PlayersEntities
				.Select(p => p.SteamId)
				.Where(id => keys.Contains(id))
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ulong> idsToAdd = keys.Except(existingEntites);
			IEnumerable<PlayerEntity> playersToAdd = players.Where(
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

	public Task<EitherStrict<Error, PlayerEntity>> Update(
		PlayerEntity entity,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public async Task<OptionStrict<Error>> UpdatePlayers(
		IEnumerable<PlayerEntity> updatedPlayers,
		CancellationToken cancellationToken
	)
	{
		try
		{
			DbSet<PlayerEntity> trackedPlayers = dbContext.PlayersEntities;
			foreach (var player in trackedPlayers)
			{
				PlayerEntity? updatedPlayer = updatedPlayers
					.Where(p => p.SteamId == player.SteamId)
					.FirstOrDefault();
				player.Avatar = updatedPlayer?.Avatar;
				player.AvatarFull = updatedPlayer?.AvatarFull;
				player.AvatarHash = updatedPlayer?.Avatar;
				player.AvatarMedium = updatedPlayer?.AvatarMedium;
				player.LocalCountryCode = updatedPlayer?.LocalCountryCode;
				player.PlayerName = updatedPlayer?.PlayerName;
				player.RealName = updatedPlayer?.RealName;
				player.ProfileUrl = updatedPlayer?.ProfileUrl;
			}
			dbContext.PlayersEntities.UpdateRange(trackedPlayers);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			return new DatabaseError($"Error updating players: {e.Message}");
		}
		return OptionStrict<Error>.Nothing;
	}
}
