using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class WeaponStats
{
	[JsonIgnore]
	public string? WeaponName { get; set; }
	public int? Kills { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("avg_dmg")]
	public double? AverageDamage { get; set; }

	// TODO how to handle 0 shots with nonzero hits
	// milestone: 7
	public int? Shots { get; set; }
	public int? Hits { get; set; }
}
