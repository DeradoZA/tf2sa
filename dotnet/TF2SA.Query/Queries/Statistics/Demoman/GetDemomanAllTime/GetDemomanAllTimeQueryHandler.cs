using AutoMapper;
using MediatR;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;

public class GetDemomanAllTimeQueryHandler
	: IRequestHandler<
		GetDemomanAllTimeQuery,
		EitherStrict<Error, GetDemomanStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetDemomanAllTimeQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetDemomanStatsResult>> Handle(
		GetDemomanAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<DemomanAllTime> page =
				await analyticsStore.GetPageAsync<DemomanAllTime>(
					"class_board(4, 20, 0)",
					"averageDpm",
					request.Sort,
					request.SortOrder,
					request.FilterField,
					request.FilterValue,
					request.Offset,
					request.Count,
					cancellationToken
				);
			IEnumerable<DemomanStatDomain> mappedPlayers = mapper.Map<
				List<DemomanStatDomain>
			>(page.Rows);
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
