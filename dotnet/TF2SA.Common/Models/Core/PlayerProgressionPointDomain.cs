namespace TF2SA.Common.Models.Core;

public class PlayerProgressionPointDomain
{
	public DateTime Bucket { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public int Draws { get; set; }
	public int Losses { get; set; }
	public float? WinPercentage { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageDeaths { get; set; }
	public float? AverageAssists { get; set; }
	public float? AverageAirshots { get; set; }
}
