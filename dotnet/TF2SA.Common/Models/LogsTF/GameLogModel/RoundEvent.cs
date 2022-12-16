using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class RoundEvent
{
	public string Type { get; set; } = string.Empty;
	public int Time { get; set; } = -1;
	public string Team { get; set; } = string.Empty;
	public string SteamId { get; set; } = string.Empty;

	[JsonPropertyName("killer")]
	public string KillerSteamId { get; set; } = string.Empty;
}
