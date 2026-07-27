namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// The (at most one) row of <c>medic_depth(steamid)</c>. Zero rows means the player
/// has no uber-depth data (never played medic in a log that had it), which the query
/// handler maps to a null <c>MedicDepthDomain</c> — same convention as PlayerSummaryRow.
/// </summary>
public class MedicDepthRow
{
	public int NumberOfGames { get; set; }
	public float? AverageAdvantagesLost { get; set; }
	public float? AverageBiggestAdvantageLost { get; set; }
	public float? AverageUberLength { get; set; }
	public float? AverageTimeToBuild { get; set; }
	public float? AverageTimeBeforeUsing { get; set; }
	public float? AverageDeathsWithin20sAfterUber { get; set; }
}
