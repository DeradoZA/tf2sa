using AutoMapper;
using MediatR;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;

public class GetMedicRecentQueryHandler
	: IRequestHandler<
		GetMedicRecentQuery,
		EitherStrict<Error, GetMedicStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetMedicRecentQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetMedicStatsResult>> Handle(
		GetMedicRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
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
			IEnumerable<MedicStatDomain> mappedPlayers = mapper.Map<
				List<MedicStatDomain>
			>(page.Rows);
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
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
