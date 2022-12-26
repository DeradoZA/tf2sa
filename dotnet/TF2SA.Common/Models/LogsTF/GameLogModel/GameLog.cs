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
	public uint? Length { get; set; }

	[JsonPropertyName("players")]
	public Dictionary<string, PlayerStats>? PlayersDict { get; set; }

	[JsonIgnore]
	public List<PlayerStats>? Players { get; set; }

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

		Names = NamesDict
			?.Select(nd =>
			{
				SteamID steamid = new();
				steamid.SetFromSteam3String(nd.Key);
				Player player =
					new() { PlayerID = steamid, PlayerName = nd.Value };
				return player;
			})
			.ToList();

		Players = PlayersDict
			?.Select(pd =>
			{
				PlayerStats playerStats = pd.Value;
				playerStats.Player = Names?.SingleOrDefault(
					n =>
						string.Equals(
							pd.Key,
							n?.PlayerID?.ToString(),
							StringComparison.InvariantCultureIgnoreCase
						)
				);
				return playerStats;
			})
			.ToList();
	}
}
