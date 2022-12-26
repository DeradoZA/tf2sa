using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class LogInfo
{
	[JsonPropertyName("map")]
	public string? Map { get; set; }

	[JsonPropertyName("supplemental")]
	public bool? Supplemental { get; set; }

	[JsonPropertyName("total_length")]
	public int? TotalLength { get; set; }

	[JsonPropertyName("hasRealDamage")]
	public bool? HasRealDamage { get; set; }

	[JsonPropertyName("hasWeaponDamage")]
	public bool? HasWeaponDamage { get; set; }

	[JsonPropertyName("hasAccuracy")]
	public bool? HasAccuracy { get; set; }

	[JsonPropertyName("hasHP")]
	public bool? HasHP { get; set; }

	[JsonPropertyName("hasHP_real")]
	public bool? HasHPReal { get; set; }

	[JsonPropertyName("hasHS")]
	public bool? HasHeadShots { get; set; }

	[JsonPropertyName("hasHS_hit")]
	public bool? HasHeadShotsHit { get; set; }

	[JsonPropertyName("hasBS")]
	public bool? HasBackStabs { get; set; }

	[JsonPropertyName("hasCP")]
	public bool? HasCapturePointsCaptured { get; set; }

	[JsonPropertyName("hasSB")]
	public bool? HasSentriesBuilt { get; set; }

	[JsonPropertyName("hasDT")]
	public bool? HasDamageTaken { get; set; }

	[JsonPropertyName("hasAS")]
	public bool? HasAirshots { get; set; }

	[JsonPropertyName("hasHR")]
	public bool? HasHealsReceived { get; set; }

	[JsonPropertyName("hasIntel")]
	public bool? HasIntel { get; set; }

	[JsonPropertyName("AD_scoring")]
	public bool? ADScoring { get; set; }

	[JsonPropertyName("notifications")]
	public List<string>? Notifications { get; set; }

	[JsonPropertyName("title")]
	public string? Title { get; set; }

	[JsonPropertyName("date")]
	public int? Date { get; set; }

	[JsonPropertyName("uploader")]
	public Uploader? Uploader { get; set; }
}
