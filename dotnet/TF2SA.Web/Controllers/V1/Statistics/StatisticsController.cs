using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Statistics.Models.ScoutStats;

namespace TF2SA.Web.Controllers.V1.Statistics;

[ApiController]
[Route("v1/[controller]")]
public class StatisticsController : ControllerBase
{
	private readonly IMediator mediator;
	private readonly IMapper mapper;
	private readonly ILogger<StatisticsController> logger;

	public StatisticsController(
		IMediator mediator,
		IMapper mapper,
		ILogger<StatisticsController> logger
	)
	{
		this.mediator = mediator;
		this.mapper = mapper;
		this.logger = logger;
	}

	// TODO split into partial classes
	// milestone: 8
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
}
