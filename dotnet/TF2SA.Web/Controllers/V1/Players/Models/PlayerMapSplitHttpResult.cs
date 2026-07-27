namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerMapSplitHttpResult
{
	public string? Map { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? WinPercentage { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageDeaths { get; set; }
}
