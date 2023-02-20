using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;

public class GetScoutRecentQuery
	: FetchPlayersQueryRequest,
		IRequest<EitherStrict<Error, GetScoutStatsResult>>
{
	public GetScoutRecentQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	) : base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
