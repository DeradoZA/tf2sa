using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class Chat
{
	public string? SteamId { get; set; }
	public string? Name { get; set; }

	[JsonPropertyName("msg")]
	public string? Message { get; set; }
}
