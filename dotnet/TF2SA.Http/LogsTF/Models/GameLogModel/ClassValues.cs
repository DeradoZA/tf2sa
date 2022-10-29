using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class ClassValues
{
	public int? Scout { get; set; }
	public int? Soldier { get; set; } = -1;
	public int? Pyro { get; set; } = -1;
	public int? Demoman { get; set; } = -1;

	[JsonPropertyName("heavyweapons")]
	public int? Heavy { get; set; } = -1;
	public int? Engineer { get; set; } = -1;
	public int? Medic { get; set; } = -1;
	public int? Sniper { get; set; } = -1;
	public int? Spy { get; set; } = -1;
}
