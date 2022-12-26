using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class RoundEvent
{
	public string? Type { get; set; }
	public int? Time { get; set; }
	public string? Team { get; set; }
	public string? SteamId { get; set; }

	[JsonPropertyName("killer")]
	public string? KillerSteamId { get; set; }
}
