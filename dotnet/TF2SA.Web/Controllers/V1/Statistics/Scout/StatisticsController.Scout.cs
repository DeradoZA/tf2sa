using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Statistics.Scout;
using TF2SA.Web.Mapping;

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
		EitherStrict<Error, GetScoutStatsResult> result =
			await getScoutRecentHandler.Handle(
				new GetScoutRecentQuery(
					count,
					offset,
					sort!,
					sortOrder!,
					filterField!,
					filterValue!
				),
				default
			);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(result.Right.ToHttpResult());
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
		EitherStrict<Error, GetScoutStatsResult> result =
			await getScoutAllTimeHandler.Handle(
				new GetScoutAllTimeQuery(
					count,
					offset,
					sort!,
					sortOrder!,
					filterField!,
					filterValue!
				),
				default
			);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(result.Right.ToHttpResult());
	}
}
