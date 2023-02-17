using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetScoutRecent;

public class GetScoutRecentResult : FetchPlayersQueryResponse<ScoutRecentDomain>
{
	public GetScoutRecentResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<ScoutRecentDomain>? players
	)
		: base(
			totalResults,
			count,
			offset,
			sort,
			sortOrder,
			filterField,
			filterValue,
			players
		) { }
}
