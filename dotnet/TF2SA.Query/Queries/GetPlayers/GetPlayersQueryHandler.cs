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
	: IRequestHandler<GetPlayersQuery, EitherStrict<Error, GetPlayersResult>>
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

	public async Task<EitherStrict<Error, GetPlayersResult>> Handle(
		GetPlayersQuery request,
		CancellationToken cancellationToken
	)
	{
		IEnumerable<Player> players;

		try
		{
			players = await playersRepository
				.GetAllQueryable()
				.OrderBy(p => p.PlayerName)
				.Skip(request.Offset)
				.Take(request.Count)
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

		return new GetPlayersResult
		{
			// TotalResults =
			Players = players,
			Offset = request.Offset,
			Count = players.Count(),
			SortBy = request.SortBy,
			FilterString = request.FilterString
		};
	}
}
