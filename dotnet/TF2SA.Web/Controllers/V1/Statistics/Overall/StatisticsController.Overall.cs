using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Overall;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;
using TF2SA.Web.Controllers.V1.Statistics.Overall;

namespace TF2SA.Web.Controllers.V1.Statistics;

public partial class StatisticsController : ControllerBase
{
	[HttpGet]
	[Route("OverallRecent")]
	public async Task<ActionResult<GetOverallStatsHttpResult>> GetOverallRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageHealsPm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetOverallStatsResult> result = await mediator.Send(
			new GetOverallRecentQuery(
				count,
				offset,
				sort!,
				sortOrder!,
				filterField!,
				filterValue!
			)
		);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		GetOverallStatsHttpResult httpResult =
			mapper.Map<GetOverallStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}

	[HttpGet]
	[Route("OverallAllTime")]
	public async Task<
		ActionResult<GetOverallStatsHttpResult>
	> GetOverallAllTime(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageHealsPm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetOverallStatsResult> result = await mediator.Send(
			new GetOverallAllTimeQuery(
				count,
				offset,
				sort!,
				sortOrder!,
				filterField!,
				filterValue!
			)
		);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		GetOverallStatsHttpResult httpResult =
			mapper.Map<GetOverallStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
