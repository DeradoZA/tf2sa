using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using PlayerEntity = TF2SA.Data.Entities.MariaDb.Player;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQueryHandler
	: IRequestHandler<GetPlayersQuery, EitherStrict<Error, List<Player>>>
{
	private readonly ILogger<GetPlayersQueryHandler> logger;
	private readonly IPlayersRepository<PlayerEntity, ulong> playersRepository;

	public GetPlayersQueryHandler(
		ILogger<GetPlayersQueryHandler> logger,
		IPlayersRepository<PlayerEntity, ulong> playersRepository
	)
	{
		this.logger = logger;
		this.playersRepository = playersRepository;
	}

	public async Task<EitherStrict<Error, List<Player>>> Handle(
		GetPlayersQuery request,
		CancellationToken cancellationToken
	)
	{
		List<Player> players = new();

		try
		{
			players = await playersRepository
				.GetAllQueryable()
				.Select(
					p =>
						new Player
						{
							PlayerName = p.PlayerName,
							SteamId = p.SteamId
						}
				)
				.ToListAsync();
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}

		return players;
	}
}
