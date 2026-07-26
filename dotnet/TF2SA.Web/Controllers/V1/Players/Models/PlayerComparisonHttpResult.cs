namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerComparisonHttpResult
{
	// String, not ulong: a 64-bit SteamID exceeds JS's safe integer range.
	public string SteamId { get; set; } = string.Empty;
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public int NumberOfGames { get; set; }
	public IEnumerable<PlayerProgressionPointHttpResult>? Progression { get; set; }
}
