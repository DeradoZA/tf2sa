using AutoMapper;
using MediatR;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;

public class GetSoldierRecentQueryHandler
	: IRequestHandler<
		GetSoldierRecentQuery,
		EitherStrict<Error, GetSoldierStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetSoldierRecentQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetSoldierStatsResult>> Handle(
		GetSoldierRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<SoldierRecent> page =
				await analyticsStore.GetPageAsync<SoldierRecent>(
					"class_board(2, 5, 1)",
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
