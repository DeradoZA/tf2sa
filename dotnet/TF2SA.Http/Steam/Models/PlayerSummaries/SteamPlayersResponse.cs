using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayersResponse
{
	[JsonPropertyName("response")]
	public SteamResult? Response;
}
