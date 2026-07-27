namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// A row of <c>players_progression_overall(steamids, bucketpart)</c> or
/// <c>players_progression_class(steamids, bucketpart, classid)</c> — one player, one time
/// bucket. Every stat is present so the frontend can switch series without refetching.
/// </summary>
public class PlayerProgressionRow
{
	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
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
