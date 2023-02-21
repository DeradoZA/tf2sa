using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.Statistics.Demoman;

public class GetDemomanStatsResult
	: FetchPlayersQueryResponse<DemomanStatDomain>
{
	public GetDemomanStatsResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<DemomanStatDomain>? players
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
