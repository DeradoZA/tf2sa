using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Medic;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;
using TF2SA.Web.Controllers.V1.Statistics.Medic;

namespace TF2SA.Web.Controllers.V1.Statistics;

public partial class StatisticsController : ControllerBase
{
	[HttpGet]
	[Route("MedicRecent")]
	public async Task<ActionResult<GetMedicStatsHttpResult>> GetMedicRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageHealsPm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetMedicStatsResult> result = await mediator.Send(
			new GetMedicRecentQuery(
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

		GetMedicStatsHttpResult httpResult =
			mapper.Map<GetMedicStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}

	[HttpGet]
	[Route("MedicAllTime")]
	public async Task<ActionResult<GetMedicStatsHttpResult>> GetMedicAllTime(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageHealsPm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetMedicStatsResult> result = await mediator.Send(
			new GetMedicAllTimeQuery(
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

		GetMedicStatsHttpResult httpResult =
			mapper.Map<GetMedicStatsHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
