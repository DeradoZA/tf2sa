using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class ClassStats : IJsonOnDeserialized
{
	public string? Type { get; set; }
	public int? Kills { get; set; }
	public int? Assists { get; set; }
	public int? Deaths { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("weapon")]
	public Dictionary<string, WeaponStats>? WeaponsDict { get; set; }

	[JsonIgnore]
	public List<WeaponStats>? WeaponStats { get; set; }

	[JsonPropertyName("total_time")]
	public int? TotalTime { get; set; }

	public void OnDeserialized()
	{
		WeaponStats = WeaponsDict
			?.Select(wd =>
			{
				WeaponStats weaponStats = wd.Value;
				weaponStats.WeaponName = wd.Key;
				return weaponStats;
			})
			.ToList();
	}
}
