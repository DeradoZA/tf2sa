using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class TeamStats
{
	public int Score { get; set; } = -1;
	public int Kills { get; set; } = -1;
	public int Deaths { get; set; } = -1;

	[JsonPropertyName("dmg")]
	public int Damage { get; set; } = -1;

	[JsonPropertyName("charges")]
	public int UberCharges { get; set; } = -1;
	public int Drops { get; set; } = -1;

	[JsonPropertyName("firstcaps")]
	public int FirstCaptures { get; set; } = -1;

	[JsonPropertyName("caps")]
	public int Captures { get; set; } = -1;
}
