using System.Text.Json.Serialization;
using SteamKit2;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class PlayerStats
{
	public Player? Player { get; set; }
	public string? Team { get; set; } = string.Empty;

	[JsonPropertyName("class_stats")]
	public ClassStats[]? ClassStats { get; set; }
	public int? Kills { get; set; }
	public int? Deaths { get; set; }
	public int? Assists { get; set; }
	public int? Suicides { get; set; }
	public string? Kapd { get; set; }
	public string? Kpd { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("dmg_real")]
	public int? DamageReal { get; set; }

	[JsonPropertyName("dt")]
	public int? DamageTaken { get; set; }

	[JsonPropertyName("dt_real")]
	public int? DamageTakenReal { get; set; }

	[JsonPropertyName("hr")]
	public int? HealsReceived { get; set; }

	[JsonPropertyName("lks")]
	public int? Lks { get; set; }

	[JsonPropertyName("as")]
	public int? Airshots { get; set; }
	public int? Dapd { get; set; }
	public int? Dapm { get; set; }
	public int? Ubers { get; set; }
	public Dictionary<string, int>? UberTypes { get; set; }
	public int? Drops { get; set; }
	public int? MedKits { get; set; }

	[JsonPropertyName("medkits_hp")]
	public int? MedKitsHealth { get; set; }
	public int? BackStabs { get; set; }
	public int? Headshots { get; set; }

	[JsonPropertyName("headshots_hit")]
	public int? HeadshotsHit { get; set; }
	public int? Sentries { get; set; }

	[JsonPropertyName("heal")]
	public int? Heals { get; set; }
	public int? Cpc { get; set; }
	public int? Ic { get; set; }
	public MedicStats? MedicStats { get; set; }
}
