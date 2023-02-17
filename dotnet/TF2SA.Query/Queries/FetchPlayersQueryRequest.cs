namespace TF2SA.Query.Queries;

public class FetchPlayersQueryRequest
{
	public FetchPlayersQueryRequest(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	)
	{
		Count = count;
		Offset = offset;
		Sort = sort;
		SortOrder = sortOrder;
		FilterField = filterField;
		FilterValue = filterValue;
	}

	public int Count { get; set; } = 20;
	public int Offset { get; set; } = 0;
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterField { get; set; } = string.Empty;
	public string FilterValue { get; set; } = string.Empty;
}
