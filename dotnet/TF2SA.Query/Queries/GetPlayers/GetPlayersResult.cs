namespace TF2SA.Query.Queries.GetPlayers;
using TF2SA.Common.Models.Core;

public class GetPlayersResult
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public IEnumerable<Player>? Players { get; set; }
	public string? SortBy { get; set; }
	public string? FilterString { get; set; }
}
