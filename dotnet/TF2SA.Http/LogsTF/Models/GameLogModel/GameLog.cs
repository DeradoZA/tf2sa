using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class GameLog
{
	public int Version { get; set; } = -1;
	public Dictionary<string, TeamStats>? Teams { get; set; }
	public uint Length { get; set; }
	public Dictionary<string, PlayerStats>? Players { get; set; }
	public Dictionary<string, string>? Names { get; set; }
	public Round[]? Rounds { get; set; }

	[JsonPropertyName("healspread")]
	public Dictionary<string, Dictionary<string, int>>? HealSpread { get; set; }
	public object? ClassKills { get; set; }
	public object? ClassDeaths { get; set; }
	public object? ClassKillAssists { get; set; }
	public object? Chat { get; set; }
	public object? Info { get; set; }
	public object? KillStreaks { get; set; }
	public bool? Success { get; set; } = false;
}
