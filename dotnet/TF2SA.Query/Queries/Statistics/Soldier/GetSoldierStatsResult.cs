using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.Statistics.Soldier;

public class GetSoldierStatsResult
	: FetchPlayersQueryResponse<SoldierStatDomain>
{
	public GetSoldierStatsResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<SoldierStatDomain>? players
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
