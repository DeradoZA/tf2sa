using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class TeamRound
{
	public int? Score { get; set; }
	public int? Kills { get; set; }

	[JsonPropertyName("dmg")]
	public int? Damage { get; set; }
	public int? Ubers { get; set; }
}
