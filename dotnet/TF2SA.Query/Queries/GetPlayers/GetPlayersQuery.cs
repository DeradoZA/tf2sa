using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQuery : IRequest<EitherStrict<Error, GetPlayersResult>>
{
	public int Count { get; set; } = 20;
	public int Offset { get; set; } = 0;
	public string? SortBy { get; set; } = string.Empty;
	public string? FilterString { get; set; } = string.Empty;

	public GetPlayersQuery() { }

	public GetPlayersQuery(int count, int offset)
	{
		Count = count;
		Offset = offset;
	}

	public GetPlayersQuery(
		int count,
		int offset,
		string? sortBy,
		string? filterString
	) : this(count, offset)
	{
		SortBy = sortBy;
		FilterString = filterString;
	}
}
