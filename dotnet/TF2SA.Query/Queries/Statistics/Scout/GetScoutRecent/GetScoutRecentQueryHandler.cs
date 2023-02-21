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

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;

public class GetScoutRecentQueryHandler
	: IRequestHandler<
		GetScoutRecentQuery,
		EitherStrict<Error, GetScoutStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<ScoutRecent> repository;

	public GetScoutRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<ScoutRecent> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<ScoutRecent> scoutRecentAllQueryable =
				repository.GetAllQueryable();

			IQueryable<ScoutRecent> filteredQueryable = repository.ApplyFilter(
				scoutRecentAllQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<ScoutRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<ScoutRecent> scoutRecentEntities =
				await filteredSortedQueryable
					.Skip(request.Offset)
					.Take(request.Count)
					.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ScoutStatDomain> scouts = mapper.Map<
				List<ScoutStatDomain>
			>(scoutRecentEntities);

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
