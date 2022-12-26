using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class ClassStats
{
	public string? Type { get; set; }
	public int? Kills { get; set; }
	public int? Assists { get; set; }
	public int? Deaths { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("weapon")]
	public Dictionary<string, WeaponStats>? Weapons { get; set; }

	[JsonPropertyName("total_time")]
	public int? TotalTime { get; set; } = -1;
}
