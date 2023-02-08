using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayerResponse
{
	[JsonPropertyName("players")]
	public List<SteamPlayer>? Players { get; set; }
}
