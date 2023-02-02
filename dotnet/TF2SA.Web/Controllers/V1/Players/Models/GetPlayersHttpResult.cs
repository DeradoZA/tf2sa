namespace TF2SA.Web.Controllers.V1.Players.Models;

public class GetPlayersHttpResult
{
	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterString { get; set; } = string.Empty;
	public IEnumerable<PlayerHttpResult>? Players { get; set; }
}
