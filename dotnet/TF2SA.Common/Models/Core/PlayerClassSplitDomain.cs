namespace TF2SA.Common.Models.Core;

public class PlayerClassSplitDomain
{
	public int ClassId { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageAssists { get; set; }
	public float? AverageDeaths { get; set; }
}
