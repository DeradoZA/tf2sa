using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class LogInfo
{
	public string Map { get; set; } = string.Empty;
	public bool Supplemental { get; set; }

	[JsonPropertyName("total_length")]
	public int TotalLength { get; set; }

	public bool HasRealDamage { get; set; }
	public bool HasWeaponDamage { get; set; }
	public bool HasAccuracy { get; set; }
	public bool HasHP { get; set; }

	[JsonPropertyName("hasHP_real")]
	public bool HasHPReal { get; set; }

	[JsonPropertyName("hasHS")]
	public bool HasHeadShots { get; set; }

	[JsonPropertyName("hasHS_hit")]
	public bool HasHeadShotsHit { get; set; }

	[JsonPropertyName("hasBS")]
	public bool HasBS { get; set; }

	[JsonPropertyName("hasCP")]
	public bool HasCP { get; set; }
	public bool HasSB { get; set; }
	public bool HasDT { get; set; }

	[JsonPropertyName("hasAS")]
	public bool HasAirshots { get; set; }
	public bool HasHR { get; set; }
	public bool HasIntel { get; set; }

	[JsonPropertyName("AD_scoring")]
	public bool ADScoring { get; set; }
	public object[] Notifications { get; set; } = Array.Empty<object>();
	public string Title { get; set; } = string.Empty;
	public int Date { get; set; }
	public Uploader Uploader { get; set; } = new Uploader();
}

public class Uploader
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Info { get; set; } = string.Empty;
}
