using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;

public class GetOverallAllTimeQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetOverallAllTimeQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetOverallStatsResult>> Handle(
		GetOverallAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
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
			IEnumerable<OverallStatDomain> mappedPlayers = page.Rows.Select(r =>
				r.ToDomain()
			);
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
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
