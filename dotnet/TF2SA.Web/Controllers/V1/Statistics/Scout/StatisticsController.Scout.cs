using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Statistics.Scout;

namespace TF2SA.Web.Controllers.V1.Statistics;

public partial class StatisticsController : ControllerBase
{
	[HttpGet]
	[Route("ScoutRecent")]
	public async Task<ActionResult<GetScoutStatsHttpResult>> GetScoutRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetScoutStatsResult> result = await mediator.Send(
			new GetScoutRecentQuery(
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

		GetScoutStatsHttpResult httpResult =
			mapper.Map<GetScoutStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}

	[HttpGet]
	[Route("ScoutAllTime")]
	public async Task<ActionResult<GetScoutStatsHttpResult>> GetScoutAllTime(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetScoutStatsResult> result = await mediator.Send(
			new GetScoutAllTimeQuery(
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

		GetScoutStatsHttpResult httpResult =
			mapper.Map<GetScoutStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
