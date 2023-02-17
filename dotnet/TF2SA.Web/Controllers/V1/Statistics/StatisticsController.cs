using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Statistics.Models.GetScoutRecent;

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

	[HttpGet]
	[Route("ScoutRecent")]
	public async Task<ActionResult<GetScoutRecentHttpResult>> GetScoutRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "averageDpm",
		[FromQuery] string? sortOrder = "desc",
		[FromQuery] string? filterField = "",
		[FromQuery] string? filterValue = ""
	)
	{
		EitherStrict<Error, GetScoutRecentResult> result = await mediator.Send(
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

		GetScoutRecentHttpResult httpResult =
			mapper.Map<GetScoutRecentHttpResult>(result.Right);

		return Ok(httpResult);
	}
}
