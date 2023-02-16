using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.MariaDb.Generic;

namespace TF2SA.Query.Queries.GetScoutRecent;

public class GetScoutRecentQueryHandler
	: IRequestHandler<
		GetScoutRecentQuery,
		EitherStrict<Error, GetScoutRecentResult>
	>
{
	private readonly IMapper mapper;
	private readonly ILogger<GetScoutRecentQueryHandler> logger;
	private readonly IStatsRepository<ScoutRecent> repository;

	public GetScoutRecentQueryHandler(
		IMapper mapper,
		ILogger<GetScoutRecentQueryHandler> logger,
		IStatsRepository<ScoutRecent> repository
	)
	{
		this.mapper = mapper;
		this.logger = logger;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetScoutRecentResult>> Handle(
		GetScoutRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			int count = await repository
				.GetAllQueryable()
				.CountAsync(cancellationToken: cancellationToken);

			List<ScoutRecent> scoutRecentEntities = await repository
				.GetAllQueryable()
				.OrderByDescending(s => s.AverageDpm)
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ScoutRecentDomain> scouts = mapper.Map<
				List<ScoutRecentDomain>
			>(scoutRecentEntities);

			return new GetScoutRecentResult
			{
				TotalResults = count,
				Count = scouts.Count(),
				Offset = request.Offset,
				Sort = request.Sort,
				SortOrder = request.SortOrder,
				Players = scouts,
			};
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
