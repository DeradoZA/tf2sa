using MediatR;
using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.GetPlayersComparison;

public class GetPlayersComparisonQuery
	: IRequest<EitherStrict<Error, GetPlayersComparisonResult>>
{
	public GetPlayersComparisonQuery(
		IReadOnlyList<ulong> steamIds,
		string bucket,
		int classId,
		string period
	)
	{
		SteamIds = steamIds;
		Bucket = bucket;
		ClassId = classId;
		Period = period;
	}

	public IReadOnlyList<ulong> SteamIds { get; }
	public string Bucket { get; }

	/// <summary>0 = overall (cross-class); otherwise 1/2/4/7 as elsewhere in this app.</summary>
	public int ClassId { get; }

	/// <summary>"recent" (last 365 days) or "alltime" — mirrors each board's own toggle.</summary>
	public string Period { get; }
}
