using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class TeamRound
{
	public int Score { get; set; } = -1;
	public int Kills { get; set; } = -1;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;
	public int Ubers { get; set; } = -1;
}
