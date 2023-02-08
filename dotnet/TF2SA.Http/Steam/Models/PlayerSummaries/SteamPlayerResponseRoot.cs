using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayerResponseRoot
{
	[JsonPropertyName("response")]
	public SteamPlayerResponse? Response { get; set; }
}
