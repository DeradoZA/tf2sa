using System.Text.Json.Serialization;
using TF2SA.Common.Models.LogsTF.Constants;
using TF2SA.Common.Models.LogsTF.Converters;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class ClassStats : IJsonOnDeserialized
{
	public string? Type { get; set; }

	[JsonIgnore]
	public byte? ClassId { get; set; }
	public int? Kills { get; set; }
	public int? Assists { get; set; }
	public int? Deaths { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	// [JsonPropertyName("weapon")]
	// public Dictionary<string, dynamic>? WeaponsDict { get; set; }

	[JsonPropertyName("weapon")]
	[JsonConverter(typeof(WeaponStatsConverter))]
	public List<WeaponStats>? WeaponStats { get; set; }

	[JsonPropertyName("total_time")]
	public int? Playtime { get; set; }

	public void OnDeserialized()
	{
		if (Enum.TryParse(Type, true, out ClassId classId))
		{
			ClassId = (byte)classId;
		}

		//WeaponStats = WeaponsDict
		//	?.Select(wd =>
		//	{
		//		WeaponStats weaponStats = wd.Value;
		//		weaponStats.WeaponName = wd.Key;
		//		return weaponStats;
		//	})
		//	.ToList();
	}
}
