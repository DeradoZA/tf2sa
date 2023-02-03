using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Config.Models.PlayerSummaries;

public class Response
{
	[JsonPropertyName("players")]
	public List<SteamPlayer>? Players;
}
