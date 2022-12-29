using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class TeamStats
{
	public string? TeamId { get; set; }
	public int? Score { get; set; }
	public int? Kills { get; set; }
	public int? Deaths { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }

	[JsonPropertyName("charges")]
	public int? UberCharges { get; set; }
	public int? Drops { get; set; }

	[JsonPropertyName("firstcaps")]
	public int? FirstCaptures { get; set; }

	[JsonPropertyName("caps")]
	public int? Captures { get; set; }
}
