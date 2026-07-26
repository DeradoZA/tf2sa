namespace TF2SA.Common.Models.Core;

public class PlayerMapSplitDomain
{
	public string? Map { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? WinPercentage { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageDeaths { get; set; }
}
