using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;

public class GetMedicAllTimeQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetMedicAllTimeQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetMedicStatsResult>> Handle(
		GetMedicAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
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
			IEnumerable<MedicStatDomain> mappedPlayers = page.Rows.Select(r =>
				r.ToDomain()
			);
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
