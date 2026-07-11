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

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;

public class GetOverallRecentQueryHandler
	: IRequestHandler<
		GetOverallRecentQuery,
		EitherStrict<Error, GetOverallStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<OverallStatsRecent> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetOverallRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<OverallStatsRecent> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetOverallStatsResult>> Handle(
		GetOverallRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<OverallStatsRecent> page =
					await analyticsStore.GetPageAsync<OverallStatsRecent>(
						"overall_board(10, 1)",
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

			IQueryable<OverallStatsRecent> allQueryable =
				repository.GetAllQueryable();

			IQueryable<OverallStatsRecent> filteredQueryable =
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

			IOrderedQueryable<OverallStatsRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<OverallStatsRecent> entities = await filteredSortedQueryable
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
