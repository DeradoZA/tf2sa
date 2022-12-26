using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class GameLog : IJsonOnDeserialized
{
	public int? Version { get; set; }

	[JsonPropertyName("teams")]
	public Dictionary<string, TeamStats>? TeamsDict { get; set; }

	[JsonIgnore]
	public List<TeamStats>? Teams { get; set; }
	public uint? Length { get; set; }

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerStats>? PlayersDict { get; set; }

	[JsonIgnore]
	public List<PlayerStats>? Players { get; set; }
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
	public bool? Success { get; set; }

	public void OnDeserialized()
	{
		Teams = TeamsDict
			?.Select(td =>
			{
				TeamStats teamStats = td.Value;
				teamStats.TeamId = td.Key;
				return teamStats;
			})
			.ToList();

		Players = PlayersDict
			?.Select(pd =>
			{
				PlayerStats playerStats = pd.Value;
				playerStats.PlayerID = new(pd.Key);
				return playerStats;
			})
			.ToList();
	}
}
