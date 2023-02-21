using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Demoman;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;
using TF2SA.Web.Controllers.V1.Statistics.Demoman;

namespace TF2SA.Web.Controllers.V1.Statistics;

public partial class StatisticsController : ControllerBase
{
	[HttpGet]
	[Route("DemomanRecent")]
	public async Task<ActionResult<GetDemomanStatsHttpResult>> GetDemomanRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetDemomanStatsResult> result = await mediator.Send(
			new GetDemomanRecentQuery(
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

		GetDemomanStatsHttpResult httpResult =
			mapper.Map<GetDemomanStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}

	[HttpGet]
	[Route("DemomanAllTime")]
	public async Task<
		ActionResult<GetDemomanStatsHttpResult>
	> GetDemomanAllTime(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetDemomanStatsResult> result = await mediator.Send(
			new GetDemomanAllTimeQuery(
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

		GetDemomanStatsHttpResult httpResult =
			mapper.Map<GetDemomanStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
