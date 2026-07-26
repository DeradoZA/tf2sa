namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerSummaryHttpResult
{
	// String, not ulong: a 64-bit SteamID exceeds JS's safe integer range.
	public string SteamId { get; set; } = string.Empty;
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public string? Description { get; set; }
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
