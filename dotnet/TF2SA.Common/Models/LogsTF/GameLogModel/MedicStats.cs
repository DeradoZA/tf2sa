using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class MedicStats
{
	[JsonPropertyName("advantages_lost")]
	public int AdvantagesLost { get; set; } = -1;

	[JsonPropertyName("biggest_advantage_lost")]
	public int BiggestAdvantageLost { get; set; } = -1;

	[JsonPropertyName("deaths_with_95_99_uber")]
	public int DeathsWith95To99Uber { get; set; } = -1;

	[JsonPropertyName("deaths_within_20s_after_uber")]
	public int DeathsWitin20sAfterUber { get; set; } = -1;

	[JsonPropertyName("avg_time_before_healing")]
	public double AverageTimeBeforeHealing { get; set; } = -1;

	[JsonPropertyName("avg_time_to_build")]
	public double AverageTimeToBuild { get; set; } = -1;

	[JsonPropertyName("avg_time_before_using")]
	public double AverageTimeBeforeUsing { get; set; } = -1;

	[JsonPropertyName("avg_uber_length")]
	public double AverageUberLength { get; set; } = -1;
}
