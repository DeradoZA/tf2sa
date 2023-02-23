using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.Statistics.Overall;

public class GetOverallStatsResult
	: FetchPlayersQueryResponse<OverallStatDomain>
{
	public GetOverallStatsResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<OverallStatDomain>? players
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
