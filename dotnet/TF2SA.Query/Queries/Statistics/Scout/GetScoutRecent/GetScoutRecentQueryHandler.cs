using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;

public class GetScoutRecentQueryHandler
	: IRequestHandler<
		GetScoutRecentQuery,
		EitherStrict<Error, GetScoutStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IAnalyticsStore analyticsStore;

	public GetScoutRecentQueryHandler(
		IMapper mapper,
		IAnalyticsStore analyticsStore
	)
	{
		this.mapper = mapper;
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
			IEnumerable<ScoutStatDomain> mappedScouts = mapper.Map<
				List<ScoutStatDomain>
			>(page.Rows);
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
