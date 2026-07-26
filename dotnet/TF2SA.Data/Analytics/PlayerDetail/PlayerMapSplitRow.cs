namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// A row of <c>player_map_splits(steamid)</c> — one map the player has played.
/// </summary>
public class PlayerMapSplitRow
{
	public string? Map { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? WinPercentage { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageDeaths { get; set; }
}
