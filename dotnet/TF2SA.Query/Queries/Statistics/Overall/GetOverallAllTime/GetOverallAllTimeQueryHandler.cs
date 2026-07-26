using AutoMapper;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;

public class GetOverallAllTimeQueryHandler
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetOverallAllTimeQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
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
			IEnumerable<OverallStatDomain> mappedPlayers = mapper.Map<
				List<OverallStatDomain>
			>(page.Rows);
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
