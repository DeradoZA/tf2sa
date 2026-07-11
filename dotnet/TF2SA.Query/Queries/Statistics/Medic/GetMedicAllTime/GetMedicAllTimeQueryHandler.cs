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

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;

public class GetMedicAllTimeQueryHandler
	: IRequestHandler<
		GetMedicAllTimeQuery,
		EitherStrict<Error, GetMedicStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<MedicAllTime> repository;
	private readonly IAnalyticsStore analyticsStore;

	public GetMedicAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<MedicAllTime> repository,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.repository = repository;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetMedicStatsResult>> Handle(
		GetMedicAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			if (analyticsStore.Enabled)
			{
				StatsPage<MedicAllTime> page =
					await analyticsStore.GetPageAsync<MedicAllTime>(
						"medic_board(20, 0)",
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

			IQueryable<MedicAllTime> allQueryable =
				repository.GetAllQueryable();

			IQueryable<MedicAllTime> filteredQueryable = repository.ApplyFilter(
				allQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<MedicAllTime> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<MedicAllTime> entities = await filteredSortedQueryable
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
