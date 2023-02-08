using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Web.Controllers.V1.Players.Models;

namespace TF2SA.Web.Controllers.V1.Players;

[ApiController]
[Route("v1/[controller]")]
public class PlayersController : ControllerBase
{
	private readonly IMediator mediator;
	private readonly IMapper mapper;
	private readonly ILogger<PlayersController> logger;

	public PlayersController(
		ILogger<PlayersController> logger,
		IMediator mediator,
		IMapper mapper
	)
	{
		this.logger = logger;
		this.mediator = mediator;
		this.mapper = mapper;
	}

	[HttpGet]
	public async Task<ActionResult<GetPlayersHttpResult>> GetPlayers(
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
		GetPlayersHttpResult playersHttpResult =
			mapper.Map<GetPlayersHttpResult>(result.Right);

		return Ok(playersHttpResult);
	}
}
