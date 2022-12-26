using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class WeaponStats
{
	public int? Kills { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("avg_dmg")]
	public double? AverageDamage { get; set; }
	public int? Shots { get; set; }
	public int? Hits { get; set; }
}
