using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamResult
{
	[JsonPropertyName("players")]
	public List<SteamPlayer>? Players;
}
