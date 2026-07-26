using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;

public class GetScoutAllTimeQueryHandler
	: IRequestHandler<
		GetScoutAllTimeQuery,
		EitherStrict<Error, GetScoutStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetScoutAllTimeQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<ScoutAllTime> page =
				await analyticsStore.GetPageAsync<ScoutAllTime>(
					"class_board(1, 20, 0)",
					"averageDpm",
					request.Sort,
					request.SortOrder,
					request.FilterField,
					request.FilterValue,
					request.Offset,
					request.Count,
					cancellationToken
				);
			IEnumerable<ScoutStatDomain> mappedPlayers = mapper.Map<
				List<ScoutStatDomain>
			>(page.Rows);
			return new GetScoutStatsResult(
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
