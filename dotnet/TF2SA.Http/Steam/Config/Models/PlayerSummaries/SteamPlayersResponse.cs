using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Config.Models.PlayerSummaries;

public class SteamPlayersResponse
{
	[JsonPropertyName("response")]
	public Response? Response;
}
