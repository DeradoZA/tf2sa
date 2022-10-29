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
	public Dictionary<string, ClassValues>? ClassKills { get; set; }
	public Dictionary<string, ClassValues>? ClassDeaths { get; set; }
	public Dictionary<string, ClassValues>? ClassKillAssists { get; set; }

	[JsonPropertyName("chat")]
	public Chat[]? Chats { get; set; }
	public LogInfo? Info { get; set; }
	public KillStreak[]? KillStreaks { get; set; }
	public bool? Success { get; set; } = false;
}
