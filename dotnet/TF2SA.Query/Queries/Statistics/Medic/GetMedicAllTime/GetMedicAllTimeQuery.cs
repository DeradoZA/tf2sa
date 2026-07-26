using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;

public class GetMedicAllTimeQuery : FetchPlayersQueryRequest
{
	public GetMedicAllTimeQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	)
		: base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
