using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;

public class GetMedicRecentQuery
	: FetchPlayersQueryRequest,
		IRequest<EitherStrict<Error, GetMedicStatsResult>>
{
	public GetMedicRecentQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	) : base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
