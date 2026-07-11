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

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;

public class GetMedicRecentQueryHandler
	: IRequestHandler<
		GetMedicRecentQuery,
		EitherStrict<Error, GetMedicStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<MedicRecent> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetMedicRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<MedicRecent> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetMedicStatsResult>> Handle(
		GetMedicRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<MedicRecent> page =
					await analyticsStore.GetPageAsync<MedicRecent>(
						"medic_board(5, 1)",
						"averageHealsPm",
						request.Sort,
						request.SortOrder,
						request.FilterField,
						request.FilterValue,
						request.Offset,
						request.Count,
						cancellationToken
					);
				IEnumerable<MedicStatDomain> mappedPlayers =
					mapper.Map<List<MedicStatDomain>>(page.Rows);
				return new GetMedicStatsResult(
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

			IQueryable<MedicRecent> allQueryable = repository.GetAllQueryable();

			IQueryable<MedicRecent> filteredQueryable = repository.ApplyFilter(
				allQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<MedicRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<MedicRecent> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<MedicStatDomain> domainMappedPlayers = mapper.Map<
				List<MedicStatDomain>
			>(entities);

			return new GetMedicStatsResult(
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
