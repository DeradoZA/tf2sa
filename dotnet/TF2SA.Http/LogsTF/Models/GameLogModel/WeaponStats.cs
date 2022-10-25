using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class WeaponStats
{
	public int Kills { get; set; }

	[JsonPropertyName("dmg")]
	public int Damage { get; set; }

	[JsonPropertyName("avg_dmg")]
	public double AverageDamage { get; set; }
	public int Shots { get; set; }
	public int Hits { get; set; }
}
