using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class WeaponStats
{
	public int Kills { get; set; } = -1;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;

	[JsonPropertyName("avg_dmg")]
	public double AverageDamage { get; set; } = -1;
	public int Shots { get; set; } = -1;
	public int Hits { get; set; } = -1;
}
