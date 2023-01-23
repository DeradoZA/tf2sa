using MediatR;
using Microsoft.AspNetCore.Mvc;
using TF2SA.Query.Queries.GetPlayers;

namespace TF2SA.Web.Controllers.v1.Home;

[ApiController]
[Route("api/v1/[controller]")]
public class PlayersController : ControllerBase
{
	private readonly IMediator mediator;
	private readonly ILogger<PlayersController> logger;

	public PlayersController(
		ILogger<PlayersController> logger,
		IMediator mediator
	)
	{
		this.logger = logger;
		this.mediator = mediator;
	}

	[HttpGet]
	public async Task<ActionResult<GetPlayersResult>> GetPlayers(
		[FromQuery] int count = 13,
		[FromQuery] int offset = 0
	)
	{
		var result = await mediator.Send(new GetPlayersQuery(count, offset));

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(result.Right);
	}
}
