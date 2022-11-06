using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class ClassStats
{
	public string Type { get; set; } = string.Empty;
	public int Kills { get; set; } = -1;
	public int Assists { get; set; } = -1;
	public int Deaths { get; set; } = -1;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;

	[JsonPropertyName("weapon")]
	public Dictionary<string, WeaponStats> Weapons { get; set; } = new Dictionary<string, WeaponStats>(0);

	[JsonPropertyName("total_time")]
	public int TotalTime { get; set; } = -1;
}
