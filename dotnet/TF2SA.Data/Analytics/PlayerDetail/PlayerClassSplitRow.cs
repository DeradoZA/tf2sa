namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// A row of <c>player_class_splits(steamid)</c> — one class the player has played.
/// </summary>
public class PlayerClassSplitRow
{
	public int ClassId { get; set; }
	public int NumberOfGames { get; set; }
	public int Wins { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageAssists { get; set; }
	public float? AverageDeaths { get; set; }
}
