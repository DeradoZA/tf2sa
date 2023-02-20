using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.Statistics.Scout;

public class GetScoutStatsResult : FetchPlayersQueryResponse<ScoutStatDomain>
{
	public GetScoutStatsResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<ScoutStatDomain>? players
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
