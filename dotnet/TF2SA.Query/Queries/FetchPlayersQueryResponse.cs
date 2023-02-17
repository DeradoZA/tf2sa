namespace TF2SA.Query.Queries;

public class FetchPlayersQueryResponse<TPlayer> where TPlayer : class
{
	public FetchPlayersQueryResponse(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<TPlayer>? players
	)
	{
		TotalResults = totalResults;
		Count = count;
		Offset = offset;
		Sort = sort;
		SortOrder = sortOrder;
		FilterField = filterField;
		FilterValue = filterValue;
		Players = players;
	}

	public int TotalResults { get; set; }
	public int Count { get; set; }
	public int Offset { get; set; }
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterField { get; set; } = string.Empty;
	public string FilterValue { get; set; } = string.Empty;

	public IEnumerable<TPlayer>? Players { get; set; }
}
