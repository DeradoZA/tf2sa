using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Soldier;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;
using TF2SA.Web.Controllers.V1.Statistics.Soldier;

namespace TF2SA.Web.Controllers.V1.Statistics;

public partial class StatisticsController : ControllerBase
{
	[HttpGet]
	[Route("SoldierRecent")]
	public async Task<ActionResult<GetSoldierStatsHttpResult>> GetSoldierRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetSoldierStatsResult> result = await mediator.Send(
			new GetSoldierRecentQuery(
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

		GetSoldierStatsHttpResult httpResult =
			mapper.Map<GetSoldierStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}

	[HttpGet]
	[Route("SoldierAllTime")]
	public async Task<
		ActionResult<GetSoldierStatsHttpResult>
	> GetSoldierAllTime(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetSoldierStatsResult> result = await mediator.Send(
			new GetSoldierAllTimeQuery(
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

		GetSoldierStatsHttpResult httpResult =
			mapper.Map<GetSoldierStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
