using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQuery : IRequest<EitherStrict<Error, GetPlayersResult>>
{
	public int Count { get; set; } = 20;
	public int Offset { get; set; } = 0;
	public string Sort { get; set; } = string.Empty;
	public string SortOrder { get; set; } = string.Empty;
	public string FilterString { get; set; } = string.Empty;

	public GetPlayersQuery() { }

	public GetPlayersQuery(int count, int offset)
	{
		Count = count;
		Offset = offset;
	}

	public GetPlayersQuery(
		int count,
		int offset,
		string sortBy,
		string sortOrder,
		string filterString
	) : this(count, offset)
	{
		Sort = sortBy;
		SortOrder = sortOrder;
		FilterString = filterString;
	}
}
