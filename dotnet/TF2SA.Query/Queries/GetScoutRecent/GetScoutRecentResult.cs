using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetScoutRecent;

public class GetScoutRecentResult
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public IEnumerable<ScoutRecentDomain>? Players { get; set; }
}
