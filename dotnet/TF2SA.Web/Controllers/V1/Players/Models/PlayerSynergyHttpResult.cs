namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerSynergyHttpResult
{
	// String, not number: a 64-bit SteamID exceeds Number.MAX_SAFE_INTEGER (matches
	// PlayerSummaryHttpResult's SteamId convention).
	public string SteamId { get; set; } = string.Empty;
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? WinPercentage { get; set; }
}
