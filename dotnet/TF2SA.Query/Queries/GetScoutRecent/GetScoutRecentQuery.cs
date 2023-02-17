using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.GetScoutRecent;

public class GetScoutRecentQuery
	: IRequest<EitherStrict<Error, GetScoutRecentResult>>
{
	public GetScoutRecentQuery(
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
