using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class Chat
{
	public string SteamId { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("msg")]
	public string Message { get; set; } = string.Empty;
}
