using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;

public class GetScoutAllTimeQueryHandler
	: IRequestHandler<
		GetScoutAllTimeQuery,
		EitherStrict<Error, GetScoutStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly ILogger<GetScoutAllTimeQueryHandler> logger;
	private readonly IStatsRepository<ScoutAllTime> repository;

	public GetScoutAllTimeQueryHandler(
		IMapper mapper,
		ILogger<GetScoutAllTimeQueryHandler> logger,
		IStatsRepository<ScoutAllTime> repository
	)
	{
		this.mapper = mapper;
		this.logger = logger;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<ScoutAllTime> scoutRecentAllQueryable =
				repository.GetAllQueryable();

			IQueryable<ScoutAllTime> filteredQueryable = repository.ApplyFilter(
				scoutRecentAllQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<ScoutAllTime> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<ScoutAllTime> scoutStatEntites = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ScoutStatDomain> scouts = mapper.Map<
				List<ScoutStatDomain>
			>(scoutStatEntites);

			return new GetScoutStatsResult(
				totalCount,
				scouts.Count(),
				request.Offset,
				sortFieldUsed,
				sortOrderUsed,
				filterFieldUsed,
				filterValueUsed,
				scouts
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
