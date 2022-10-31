using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class Round
{
	[JsonPropertyName("start_time")]
	public int StartTime { get; set; } = -1;

	[JsonPropertyName("winner")]
	public string WinnerTeam { get; set; } = string.Empty;

	[JsonPropertyName("team")]
	public Dictionary<string, TeamRound> TeamRound { get; set; } = new Dictionary<string, TeamRound>(0);

	[JsonPropertyName("events")]
	public RoundEvent[] RoundEvents { get; set; } = Array.Empty<RoundEvent>();

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerRound> PlayerRounds { get; set; } = new Dictionary<string, PlayerRound>(0);

	[JsonPropertyName("firstcap")]
	public string FirstCapTeam { get; set; } = string.Empty;
	public int Length { get; set; } = -1;
}
