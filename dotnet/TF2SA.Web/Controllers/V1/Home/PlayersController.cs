using MediatR;
using Microsoft.AspNetCore.Mvc;
using TF2SA.Query.Queries.GetPlayers;

namespace TF2SA.Web.Controllers.v1.Home;

[ApiController]
[Route("[controller]")]
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
	public async Task<IActionResult> GetPlayers()
	{
		var result = await mediator.Send(new GetPlayersQuery());

		if (result.IsLeft)
		{
			return NotFound();
		}

		return Ok(result.Right);
	}
}
