using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
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
	private readonly IStatsRepository<ScoutAllTime> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetScoutAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<ScoutAllTime> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<ScoutAllTime> page =
					await analyticsStore.GetPageAsync<ScoutAllTime>(
						"class_board(1, 20, 0)",
						"averageDpm",
						request.Sort,
						request.SortOrder,
						request.FilterField,
						request.FilterValue,
						request.Offset,
						request.Count,
						cancellationToken
					);
				IEnumerable<ScoutStatDomain> mappedPlayers =
					mapper.Map<List<ScoutStatDomain>>(page.Rows);
				return new GetScoutStatsResult(
					page.Total,
					mappedPlayers.Count(),
					request.Offset,
					page.SortUsed,
					page.SortOrderUsed,
					page.FilterFieldUsed,
					page.FilterValueUsed,
					mappedPlayers
				);
			}

			IQueryable<ScoutAllTime> allQueryable =
				repository.GetAllQueryable();

			IQueryable<ScoutAllTime> filteredQueryable = repository.ApplyFilter(
				allQueryable,
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

			IEnumerable<ScoutStatDomain> domainMappedPlayers = mapper.Map<
				List<ScoutStatDomain>
			>(scoutStatEntites);

			return new GetScoutStatsResult(
				totalCount,
				domainMappedPlayers.Count(),
				request.Offset,
				sortFieldUsed,
				sortOrderUsed,
				filterFieldUsed,
				filterValueUsed,
				domainMappedPlayers
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
