using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class Round
{
	[JsonPropertyName("start_time")]
	public int StartTime { get; set; } = -1;

	[JsonPropertyName("winner")]
	public string? WinnerTeam { get; set; }

	[JsonPropertyName("team")]
	public Dictionary<string, TeamRound>? TeamRound { get; set; }

	[JsonPropertyName("events")]
	public RoundEvent[]? RoundEvents { get; set; }

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerRound>? PlayerRounds { get; set; }

	[JsonPropertyName("firstcap")]
	public string? FirstCapTeam { get; set; }
	public int Length { get; set; } = -1;
}
