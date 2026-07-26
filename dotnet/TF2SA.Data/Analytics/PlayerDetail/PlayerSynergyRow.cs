namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// A row of <c>player_teammate_synergy(steamid, mingames)</c> or
/// <c>player_opponent_synergy(steamid, mingames)</c> — one other player the querying
/// player has shared games with, and the querying player's own win rate in those games.
/// </summary>
public class PlayerSynergyRow
{
	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? WinPercentage { get; set; }
}
