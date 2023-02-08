using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersResult
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterString { get; set; } = string.Empty;
	public IEnumerable<Player>? Players { get; set; }
}
