using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.GetPlayers;

namespace TF2SA.Web.Controllers.v1.Home;

[ApiController]
[Route("v1/[controller]")]
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
		[FromQuery] int offset = 0,
		[FromQuery] string? sort = "playerName",
		[FromQuery] string? sortOrder = "asc",
		[FromQuery] string? filterString = ""
	)
	{
		EitherStrict<Error, GetPlayersResult> result = await mediator.Send(
			new GetPlayersQuery(count, offset, sort!, sortOrder!, filterString!)
		);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(result.Right);
	}
}
