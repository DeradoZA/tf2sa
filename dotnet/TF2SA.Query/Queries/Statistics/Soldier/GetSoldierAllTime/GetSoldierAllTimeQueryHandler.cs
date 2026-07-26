using AutoMapper;
using MediatR;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;

public class GetSoldierAllTimeQueryHandler
	: IRequestHandler<
		GetSoldierAllTimeQuery,
		EitherStrict<Error, GetSoldierStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetSoldierAllTimeQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
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
			IEnumerable<SoldierStatDomain> mappedPlayers = mapper.Map<
				List<SoldierStatDomain>
			>(page.Rows);
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
