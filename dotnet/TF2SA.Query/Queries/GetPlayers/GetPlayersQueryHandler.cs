using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Errors;
using TF2SA.Data.Extensions;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQueryHandler
	: IRequestHandler<GetPlayersQuery, EitherStrict<Error, GetPlayersResult>>
{
	private readonly IPlayersRepository<PlayersEntity, ulong> playersRepository;
	private readonly IMapper mapper;

	public GetPlayersQueryHandler(
		IPlayersRepository<PlayersEntity, ulong> playersRepository,
		IMapper mapper
	)
	{
		this.playersRepository = playersRepository;
		this.mapper = mapper;
	}

	public async Task<EitherStrict<Error, GetPlayersResult>> Handle(
		GetPlayersQuery request,
		CancellationToken cancellationToken
	)
	{
		IEnumerable<Player> players;
		int playerCount;

		try
		{
			playerCount = await playersRepository
				.GetAllQueryable()
				.ApplyFilter(request.FilterString, out string _)
				.CountAsync(cancellationToken: cancellationToken);

			List<PlayersEntity> playersEntities = await playersRepository
				.GetAllQueryable()
				.ApplyFilter(request.FilterString, out string filterStringUsed)
				.ApplySort(
					request.Sort,
					request.SortOrder,
					out string sortUsed,
					out string sortOrderUsed
				)
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			players = mapper.Map<List<Player>>(playersEntities);

			return new GetPlayersResult
			{
				TotalResults = playerCount,
				Players = players,
				Offset = request.Offset,
				Count = players.Count(),
				Sort = sortUsed,
				SortOrder = sortOrderUsed,
				FilterString = filterStringUsed
			};
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
