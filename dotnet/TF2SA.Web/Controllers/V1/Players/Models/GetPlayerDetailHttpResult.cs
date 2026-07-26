namespace TF2SA.Web.Controllers.V1.Players.Models;

public class GetPlayerDetailHttpResult
{
	/// <summary>Null when the player has no recorded games.</summary>
	public PlayerSummaryHttpResult? Summary { get; set; }
	public string Bucket { get; set; } = string.Empty;

	/// <summary>0 = overall (cross-class); otherwise 1/2/4/7.</summary>
	public int ClassId { get; set; }
	public IEnumerable<PlayerProgressionPointHttpResult>? Progression { get; set; }
	public IEnumerable<PlayerClassSplitHttpResult>? ClassSplits { get; set; }
}
