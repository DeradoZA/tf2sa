using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.GetPlayerDetail;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.GetPlayersComparison;
using TF2SA.Web.Controllers.V1.Players.Models;

namespace TF2SA.Web.Controllers.V1.Players;

[ApiController]
[Route("v1/[controller]")]
public class PlayersController : ControllerBase
{
	private readonly IMediator mediator;
	private readonly IMapper mapper;

	public PlayersController(IMediator mediator, IMapper mapper)
	{
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

	/// <summary>
	/// A player's summary, stat progression over time, and per-class splits.
	/// <paramref name="steamId"/> is bound as a ulong, so a non-numeric or overflowing
	/// route segment is rejected with 400 before reaching the query. <paramref
	/// name="classId"/> scopes only <c>Progression</c> — 0 (default) is overall
	/// cross-class, otherwise 1/2/4/7; <c>ClassSplits</c> always covers every class.
	/// </summary>
	[HttpGet("{steamId}/Detail")]
	public async Task<ActionResult<GetPlayerDetailHttpResult>> GetPlayerDetail(
		[FromRoute] ulong steamId,
		[FromQuery] string? bucket = "month",
		[FromQuery] int classId = 0
	)
	{
		EitherStrict<Error, GetPlayerDetailResult> result = await mediator.Send(
			new GetPlayerDetailQuery(steamId, bucket ?? "month", classId)
		);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(mapper.Map<GetPlayerDetailHttpResult>(result.Right));
	}

	// Matches the largest page-size option on the leaderboards (frontend
	// PAGE_SIZE_OPTIONS) — the graph shows every player currently on the table page, so
	// this is a real UI-driven bound, not an arbitrary chart-readability number.
	private const int MaxComparisonPlayers = 100;

	/// <summary>
	/// Stat progression for several players at once — backs each leaderboard's inline
	/// Graph view (charts whichever players are on the current table page).
	/// <paramref name="steamIds"/> is a comma-separated list, parsed to <c>ulong</c>
	/// here (so it can only carry digits) and capped at
	/// <see cref="MaxComparisonPlayers"/> — both rejected with 400 rather than silently
	/// truncated, since a caller expecting N players back should never silently get
	/// fewer.
	/// </summary>
	[HttpGet("Compare")]
	public async Task<ActionResult<GetPlayersComparisonHttpResult>> GetPlayersComparison(
		[FromQuery] string steamIds,
		[FromQuery] string? bucket = "month",
		[FromQuery] int classId = 0,
		[FromQuery] string? period = "alltime"
	)
	{
		string[] rawIds = steamIds.Split(
			',',
			StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
		);

		if (rawIds.Length == 0)
		{
			return BadRequest("steamIds must contain at least one SteamID.");
		}
		if (rawIds.Length > MaxComparisonPlayers)
		{
			return BadRequest(
				$"steamIds must contain at most {MaxComparisonPlayers} SteamIDs."
			);
		}

		List<ulong> parsedIds = new(rawIds.Length);
		foreach (string rawId in rawIds)
		{
			if (!ulong.TryParse(rawId, out ulong parsedId))
			{
				return BadRequest($"'{rawId}' is not a valid SteamID.");
			}
			parsedIds.Add(parsedId);
		}

		EitherStrict<Error, GetPlayersComparisonResult> result = await mediator.Send(
			new GetPlayersComparisonQuery(
				parsedIds,
				bucket ?? "month",
				classId,
				period ?? "alltime"
			)
		);

		if (result.IsLeft)
		{
			return BadRequest(result.Left.Message);
		}

		return Ok(mapper.Map<GetPlayersComparisonHttpResult>(result.Right));
	}
}
