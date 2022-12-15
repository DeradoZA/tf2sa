using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class GameLog
{
	public int Version { get; set; } = -1;
	public Dictionary<string, TeamStats> Teams { get; set; } =
		new Dictionary<string, TeamStats>(0);
	public uint Length { get; set; }
	public Dictionary<string, PlayerStats> Players { get; set; } =
		new Dictionary<string, PlayerStats>(0);
	public Dictionary<string, string> Names { get; set; } =
		new Dictionary<string, string>(0);
	public Round[] Rounds { get; set; } = Array.Empty<Round>();

	[JsonPropertyName("healspread")]
	public Dictionary<
		string,
		Dictionary<string, int>
	> HealSpread { get; set; } =
		new Dictionary<string, Dictionary<string, int>>(0);
	public Dictionary<string, ClassValues> ClassKills { get; set; } =
		new Dictionary<string, ClassValues>(0);
	public Dictionary<string, ClassValues> ClassDeaths { get; set; } =
		new Dictionary<string, ClassValues>(0);
	public Dictionary<string, ClassValues> ClassKillAssists { get; set; } =
		new Dictionary<string, ClassValues>(0);

	[JsonPropertyName("chat")]
	public Chat[] Chats { get; set; } = Array.Empty<Chat>();
	public LogInfo Info { get; set; } = new LogInfo();
	public KillStreak[] KillStreaks { get; set; } = Array.Empty<KillStreak>();
	public bool Success { get; set; } = false;
}
