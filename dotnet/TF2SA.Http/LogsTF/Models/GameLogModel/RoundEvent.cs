using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class RoundEvent
{
	public string? Type { get; set; }
	public int Time { get; set; } = -1;
	public string? Team { get; set; }
	public string? SteamId { get; set; }

	[JsonPropertyName("killer")]
	public string? KillerSteamId { get; set; }
}