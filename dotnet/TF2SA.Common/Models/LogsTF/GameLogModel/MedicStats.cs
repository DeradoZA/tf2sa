using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class MedicStats
{
	[JsonPropertyName("advantages_lost")]
	public int? AdvantagesLost { get; set; }

	[JsonPropertyName("biggest_advantage_lost")]
	public int? BiggestAdvantageLost { get; set; }

	[JsonPropertyName("deaths_with_95_99_uber")]
	public int? DeathsWith95To99Uber { get; set; }

	[JsonPropertyName("deaths_within_20s_after_uber")]
	public int? DeathsWitin20sAfterUber { get; set; }

	[JsonPropertyName("avg_time_before_healing")]
	public double? AverageTimeBeforeHealing { get; set; }

	[JsonPropertyName("avg_time_to_build")]
	public double? AverageTimeToBuild { get; set; }

	[JsonPropertyName("avg_time_before_using")]
	public double? AverageTimeBeforeUsing { get; set; }

	[JsonPropertyName("avg_uber_length")]
	public double? AverageUberLength { get; set; }
}
