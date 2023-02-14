namespace TF2SA.Web.Controllers.V1.Statistics.Models.GetScoutRecent;

public class GetScoutRecentHttpResult
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterString { get; set; } = string.Empty;
	public IEnumerable<ScoutRecentHttpResult>? Players { get; set; }
}
