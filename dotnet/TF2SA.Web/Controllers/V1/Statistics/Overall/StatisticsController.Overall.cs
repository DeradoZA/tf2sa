using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Overall;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;
using TF2SA.Web.Controllers.V1.Statistics.Overall;
using TF2SA.Web.Mapping;

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
		EitherStrict<Error, GetOverallStatsResult> result =
			await getOverallRecentHandler.Handle(
				new GetOverallRecentQuery(
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
		EitherStrict<Error, GetOverallStatsResult> result =
			await getOverallAllTimeHandler.Handle(
				new GetOverallAllTimeQuery(
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
