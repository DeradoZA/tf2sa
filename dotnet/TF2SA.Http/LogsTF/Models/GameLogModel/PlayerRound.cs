using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class PlayerRound
{
	public string? Team { get; set; }
	public int Kills { get; set; } = -1;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;
}
