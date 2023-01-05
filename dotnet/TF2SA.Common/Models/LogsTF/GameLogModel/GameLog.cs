using System.Text.Json.Serialization;
using SteamKit2;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class GameLog : IJsonOnDeserialized
{
	public int? Version { get; set; }

	[JsonPropertyName("teams")]
	public Dictionary<string, TeamStats>? TeamsDict { get; set; }

	[JsonIgnore]
	public List<TeamStats>? Teams { get; set; }

	[JsonPropertyName("length")]
	public uint? Duration { get; set; }

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerStats>? PlayersDict { get; set; }

	[JsonIgnore]
	public List<PlayerStats>? PlayerStats { get; set; }

	[JsonPropertyName("names")]
	public Dictionary<string, string>? NamesDict { get; set; }

	[JsonIgnore]
	public List<Player>? Names { get; set; }
	public List<Round>? Rounds { get; set; }

	[JsonPropertyName("healspread")]
	public Dictionary<string, Dictionary<string, int>>? HealSpread { get; set; }
	public Dictionary<string, ClassValues>? ClassKills { get; set; }
	public Dictionary<string, ClassValues>? ClassDeaths { get; set; }
	public Dictionary<string, ClassValues>? ClassKillAssists { get; set; }

	[JsonPropertyName("chat")]
	public List<Chat>? Chats { get; set; }
	public LogInfo? Info { get; set; }
	public List<KillStreak>? KillStreaks { get; set; }
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

		Names = NamesDict?.Select(nd => new Player(nd.Key, nd.Value)).ToList();

		PlayerStats = PlayersDict
			?.Select(pd =>
			{
				PlayerStats playerStats = pd.Value;
				SteamID dictPlayerStatsId = Player.MakeSteamIdFromString(
					pd.Key
				);
				playerStats.Player = Names?.SingleOrDefault(
					n => dictPlayerStatsId == n?.SteamId
				);
				return playerStats;
			})
			.ToList();
	}
}
