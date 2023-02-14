using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TF2SA.Web.Controllers.V1.Statistics.Models.GetScoutRecent;

namespace TF2SA.Web.Controllers.V1.Statistics;

[ApiController]
[Route("api/[controller]")]
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

	[HttpGet()]
	public async Task<ActionResult<GetScoutRecentHttpResult>> GetScoutRecent(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "playerName",
		[FromQuery] string? sortOrder = "asc",
		[FromQuery] string? filterString = ""
	)
	{
		await Task.Delay(1000);
		return Ok(new GetScoutRecentHttpResult());
	}
}
