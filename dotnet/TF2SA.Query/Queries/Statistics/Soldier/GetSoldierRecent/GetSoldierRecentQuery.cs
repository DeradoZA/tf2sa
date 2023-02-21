using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;

public class GetSoldierRecentQuery
	: FetchPlayersQueryRequest,
		IRequest<EitherStrict<Error, GetSoldierStatsResult>>
{
	public GetSoldierRecentQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	) : base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
