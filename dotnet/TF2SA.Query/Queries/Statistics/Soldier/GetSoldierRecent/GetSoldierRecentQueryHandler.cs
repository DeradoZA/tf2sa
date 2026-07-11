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

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;

public class GetSoldierRecentQueryHandler
	: IRequestHandler<
		GetSoldierRecentQuery,
		EitherStrict<Error, GetSoldierStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<SoldierRecent> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetSoldierRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<SoldierRecent> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetSoldierStatsResult>> Handle(
		GetSoldierRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<SoldierRecent> page =
					await analyticsStore.GetPageAsync<SoldierRecent>(
						"class_board(2, 5, 1)",
						"averageDpm",
						request.Sort,
						request.SortOrder,
						request.FilterField,
						request.FilterValue,
						request.Offset,
						request.Count,
						cancellationToken
					);
				IEnumerable<SoldierStatDomain> mappedPlayers =
					mapper.Map<List<SoldierStatDomain>>(page.Rows);
				return new GetSoldierStatsResult(
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

			IQueryable<SoldierRecent> allQueryable =
				repository.GetAllQueryable();

			IQueryable<SoldierRecent> filteredQueryable =
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

			IOrderedQueryable<SoldierRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<SoldierRecent> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<SoldierStatDomain> domainMappedPlayers = mapper.Map<
				List<SoldierStatDomain>
			>(entities);

			return new GetSoldierStatsResult(
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
