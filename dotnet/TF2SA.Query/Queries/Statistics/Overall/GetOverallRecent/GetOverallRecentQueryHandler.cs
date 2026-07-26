using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;

public class GetOverallRecentQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetOverallRecentQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetOverallStatsResult>> Handle(
		GetOverallRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
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
