using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;

public class GetSoldierAllTimeQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetSoldierAllTimeQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetSoldierStatsResult>> Handle(
		GetSoldierAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<SoldierAllTime> page =
				await analyticsStore.GetPageAsync<SoldierAllTime>(
					"class_board(2, 20, 0)",
					"averageDpm",
					request.Sort,
					request.SortOrder,
					request.FilterField,
					request.FilterValue,
					request.Offset,
					request.Count,
					cancellationToken
				);
			IEnumerable<SoldierStatDomain> mappedPlayers = page.Rows.Select(r =>
				r.ToDomain()
			);
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
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
