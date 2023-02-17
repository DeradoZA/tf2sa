namespace TF2SA.Web.Controllers.V1.Statistics.Models.GetScoutRecent;

public class PlayersFetchResultHttp<TPlayer> where TPlayer : class
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterField { get; set; } = string.Empty;
	public string FilterValue { get; set; } = string.Empty;
	public IEnumerable<TPlayer>? Players { get; set; }
}
