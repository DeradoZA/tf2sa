using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;

public class GetOverallAllTimeQueryHandler
	: IRequestHandler<
		GetOverallAllTimeQuery,
		EitherStrict<Error, GetOverallStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<OverallStatsAllTime> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetOverallAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<OverallStatsAllTime> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetOverallStatsResult>> Handle(
		GetOverallAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<OverallStatsAllTime> page =
					await analyticsStore.GetPageAsync<OverallStatsAllTime>(
						"overall_board(20, 0)",
						"averageDpm",
						request.Sort,
						request.SortOrder,
						request.FilterField,
						request.FilterValue,
						request.Offset,
						request.Count,
						cancellationToken
					);
				IEnumerable<OverallStatDomain> mappedPlayers =
					mapper.Map<List<OverallStatDomain>>(page.Rows);
				return new GetOverallStatsResult(
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

			IQueryable<OverallStatsAllTime> allQueryable =
				repository.GetAllQueryable();

			IQueryable<OverallStatsAllTime> filteredQueryable =
				repository.ApplyFilter(
					allQueryable,
					request.FilterField,
					request.FilterValue,
					out string filterFieldUsed,
					out string filterValueUsed
				);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<OverallStatsAllTime> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<OverallStatsAllTime> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<OverallStatDomain> domainMappedPlayers = mapper.Map<
				List<OverallStatDomain>
			>(entities);

			return new GetOverallStatsResult(
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
