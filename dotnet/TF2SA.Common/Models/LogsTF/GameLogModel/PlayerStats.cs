using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class PlayerStats
{
	public string Team { get; set; } = string.Empty;

	[JsonPropertyName("class_stats")]
	public ClassStats[] ClassStats { get; set; } = Array.Empty<ClassStats>();
	public int Kills { get; set; } = -1;
	public int Deaths { get; set; } = -1;
	public int Assists { get; set; } = -1;
	public int Suicides { get; set; } = -1;
	public string Kapd { get; set; } = string.Empty;
	public string Kpd { get; set; } = string.Empty;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;

	[JsonPropertyName("dmg_real")]
	public int DamageReal { get; set; } = -1;

	[JsonPropertyName("dt")]
	public int DamageTaken { get; set; } = -1;

	[JsonPropertyName("dt_real")]
	public int DamageTakenReal { get; set; } = -1;

	[JsonPropertyName("hr")]
	public int HealsReceived { get; set; } = -1;

	[JsonPropertyName("lks")]
	public int Lks { get; set; } = -1;

	[JsonPropertyName("as")]
	public int Airshots { get; set; } = -1;
	public int Dapd { get; set; } = -1;
	public int Dapm { get; set; } = -1;
	public int Ubers { get; set; } = -1;
	public Dictionary<string, int> UberTypes { get; set; } =
		new Dictionary<string, int>(0);
	public int Drops { get; set; } = -1;
	public int MedKits { get; set; } = -1;

	[JsonPropertyName("medkits_hp")]
	public int MedKitsHealth { get; set; } = -1;
	public int BackStabs { get; set; } = -1;
	public int Headshots { get; set; } = -1;

	[JsonPropertyName("headshots_hit")]
	public int HeadshotsHit { get; set; } = -1;
	public int Sentries { get; set; } = -1;

	[JsonPropertyName("heal")]
	public int Heals { get; set; } = -1;
	public int Cpc { get; set; } = -1;
	public int Ic { get; set; } = -1;
	public MedicStats MedicStats { get; set; } = new MedicStats();
}
