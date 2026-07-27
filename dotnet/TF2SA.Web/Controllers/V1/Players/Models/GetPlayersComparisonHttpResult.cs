namespace TF2SA.Web.Controllers.V1.Players.Models;

public class GetPlayersComparisonHttpResult
{
	public string Bucket { get; set; } = string.Empty;

	/// <summary>0 = overall (cross-class); otherwise 1/2/4/7.</summary>
	public int ClassId { get; set; }

	/// <summary>"recent" or "alltime" — echoes back which window was actually applied.</summary>
	public string Period { get; set; } = string.Empty;
	public IEnumerable<PlayerComparisonHttpResult>? Players { get; set; }
}
