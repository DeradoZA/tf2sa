namespace TF2SA.Web.Controllers.V1.Players.Models;

public class MedicDepthHttpResult
{
	public int NumberOfGames { get; set; }
	public float? AverageAdvantagesLost { get; set; }
	public float? AverageBiggestAdvantageLost { get; set; }
	public float? AverageUberLength { get; set; }
	public float? AverageTimeToBuild { get; set; }
	public float? AverageTimeBeforeUsing { get; set; }
	public float? AverageDeathsWithin20sAfterUber { get; set; }
}
