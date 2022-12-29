using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class ClassValues
{
	public int? Scout { get; set; }
	public int? Soldier { get; set; }
	public int? Pyro { get; set; }
	public int? Demoman { get; set; }

	[JsonPropertyName("heavyweapons")]
	public int? Heavy { get; set; }
	public int? Engineer { get; set; }
	public int? Medic { get; set; }
	public int? Sniper { get; set; }
	public int? Spy { get; set; }
}
