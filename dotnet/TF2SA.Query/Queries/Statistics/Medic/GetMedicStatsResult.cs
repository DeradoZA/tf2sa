using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.Statistics.Medic;

public class GetMedicStatsResult : FetchPlayersQueryResponse<MedicStatDomain>
{
	public GetMedicStatsResult(
		int totalResults,
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		IEnumerable<MedicStatDomain>? players
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
