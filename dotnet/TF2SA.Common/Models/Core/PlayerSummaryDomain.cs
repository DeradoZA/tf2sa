namespace TF2SA.Common.Models.Core;

public class PlayerSummaryDomain
{
	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
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
