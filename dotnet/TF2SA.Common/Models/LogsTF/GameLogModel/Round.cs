using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class Round
{
	[JsonPropertyName("start_time")]
	public int? StartTime { get; set; }

	[JsonPropertyName("winner")]
	public string? WinnerTeam { get; set; }

	[JsonPropertyName("team")]
	public Dictionary<string, TeamRound>? TeamRound { get; set; }

	[JsonPropertyName("events")]
	public List<RoundEvent>? RoundEvents { get; set; }

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerRound>? PlayerRounds { get; set; }

	[JsonPropertyName("firstcap")]
	public string? FirstCapTeam { get; set; }
	public int? Length { get; set; }
}
