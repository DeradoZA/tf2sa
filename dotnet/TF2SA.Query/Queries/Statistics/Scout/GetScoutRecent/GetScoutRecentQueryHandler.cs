using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;

public class GetScoutRecentQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetScoutRecentQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<ScoutRecent> page =
				await analyticsStore.GetPageAsync<ScoutRecent>(
					"class_board(1, 5, 1)",
					"averageDpm",
					request.Sort,
					request.SortOrder,
					request.FilterField,
					request.FilterValue,
					request.Offset,
					request.Count,
					cancellationToken
				);
			IEnumerable<ScoutStatDomain> mappedScouts = page.Rows.Select(r =>
				r.ToDomain()
			);
			return new GetScoutStatsResult(
				page.Total,
				mappedScouts.Count(),
				request.Offset,
				page.SortUsed,
				page.SortOrderUsed,
				page.FilterFieldUsed,
				page.FilterValueUsed,
				mappedScouts
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
