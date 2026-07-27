using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;

public class GetDemomanRecentQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetDemomanRecentQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetDemomanStatsResult>> Handle(
		GetDemomanRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
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
			IEnumerable<DemomanStatDomain> mappedPlayers = page.Rows.Select(r =>
				r.ToDomain()
			);
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
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
