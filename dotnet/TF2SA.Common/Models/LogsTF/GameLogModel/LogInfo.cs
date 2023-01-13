using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class LogInfo : IJsonOnDeserialized
{
	[JsonPropertyName("map")]
	public string? Map { get; set; }

	[JsonPropertyName("supplemental")]
	public bool? IsSupplemental { get; set; } = false;

	[JsonPropertyName("total_length")]
	public int? TotalLength { get; set; }

	[JsonPropertyName("hasRealDamage")]
	public bool? HasRealDamage { get; set; } = false;

	[JsonPropertyName("hasWeaponDamage")]
	public bool? HasWeaponDamage { get; set; } = false;

	[JsonPropertyName("hasAccuracy")]
	public bool? HasAccuracy { get; set; } = false;

	[JsonPropertyName("hasHP")]
	public bool? HasHp { get; set; } = false;

	[JsonPropertyName("hasHP_real")]
	public bool? HasHPReal { get; set; } = false;

	[JsonPropertyName("hasHS")]
	public bool? HasHeadshots { get; set; } = false;

	[JsonPropertyName("hasHS_hit")]
	public bool? HasHeadshotsHit { get; set; } = false;

	[JsonPropertyName("hasBS")]
	public bool? HasBackstabs { get; set; } = false;

	[JsonPropertyName("hasCP")]
	public bool? HasCapturePointsCaptured { get; set; } = false;

	[JsonPropertyName("hasSB")]
	public bool? HasSentriesBuilt { get; set; } = false;

	[JsonPropertyName("hasDT")]
	public bool? HasDamageTaken { get; set; } = false;

	[JsonPropertyName("hasAS")]
	public bool? HasAirshots { get; set; } = false;

	[JsonPropertyName("hasHR")]
	public bool? HasHealsReceived { get; set; } = false;

	[JsonPropertyName("hasIntel")]
	public bool? HasIntelCaptures { get; set; } = false;

	[JsonPropertyName("AD_scoring")]
	public bool? HasAdscoring { get; set; } = false;

	[JsonPropertyName("notifications")]
	public List<string>? NotificationsArray { get; set; }

	[JsonIgnore]
	public string? Notifications { get; set; }

	[JsonPropertyName("title")]
	public string? Title { get; set; }

	[JsonPropertyName("date")]
	public int? Date { get; set; }

	[JsonPropertyName("uploader")]
	public Uploader? Uploader { get; set; }

	public void OnDeserialized()
	{
		if (NotificationsArray is not null)
		{
			Notifications = string.Join(",", NotificationsArray);
		}
	}
}
