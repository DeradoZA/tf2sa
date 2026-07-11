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

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;

public class GetDemomanRecentQueryHandler
	: IRequestHandler<
		GetDemomanRecentQuery,
		EitherStrict<Error, GetDemomanStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<DemomanRecent> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetDemomanRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<DemomanRecent> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetDemomanStatsResult>> Handle(
		GetDemomanRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<DemomanRecent> page =
					await analyticsStore.GetPageAsync<DemomanRecent>(
						"class_board(4, 5, 1)",
						"averageDpm",
						request.Sort,
						request.SortOrder,
						request.FilterField,
						request.FilterValue,
						request.Offset,
						request.Count,
						cancellationToken
					);
				IEnumerable<DemomanStatDomain> mappedPlayers =
					mapper.Map<List<DemomanStatDomain>>(page.Rows);
				return new GetDemomanStatsResult(
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

			IQueryable<DemomanRecent> allQueryable =
				repository.GetAllQueryable();

			IQueryable<DemomanRecent> filteredQueryable =
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

			IOrderedQueryable<DemomanRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<DemomanRecent> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<DemomanStatDomain> domainMappedPlayers = mapper.Map<
				List<DemomanStatDomain>
			>(entities);

			return new GetDemomanStatsResult(
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
