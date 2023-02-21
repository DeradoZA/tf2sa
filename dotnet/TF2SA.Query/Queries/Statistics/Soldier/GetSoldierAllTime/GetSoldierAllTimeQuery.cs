using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;

public class GetSoldierAllTimeQuery
	: FetchPlayersQueryRequest,
		IRequest<EitherStrict<Error, GetSoldierStatsResult>>
{
	public GetSoldierAllTimeQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	) : base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
