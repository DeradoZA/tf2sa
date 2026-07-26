namespace TF2SA.Common.Models.Core;

public class MedicDepthDomain
{
	public int NumberOfGames { get; set; }
	public float? AverageAdvantagesLost { get; set; }
	public float? AverageBiggestAdvantageLost { get; set; }
	public float? AverageUberLength { get; set; }
	public float? AverageTimeToBuild { get; set; }
	public float? AverageTimeBeforeUsing { get; set; }
	public float? AverageDeathsWithin20sAfterUber { get; set; }
}
