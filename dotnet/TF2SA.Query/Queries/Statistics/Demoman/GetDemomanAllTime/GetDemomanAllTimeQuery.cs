using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;

public class GetDemomanAllTimeQuery
	: FetchPlayersQueryRequest,
		IRequest<EitherStrict<Error, GetDemomanStatsResult>>
{
	public GetDemomanAllTimeQuery(
		int count,
		int offset,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue
	) : base(count, offset, sort, sortOrder, filterField, filterValue) { }
}
