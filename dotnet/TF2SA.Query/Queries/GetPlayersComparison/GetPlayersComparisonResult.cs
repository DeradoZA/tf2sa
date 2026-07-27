using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetPlayersComparison;

public class GetPlayersComparisonResult
{
	public GetPlayersComparisonResult(
		string bucket,
		int classId,
		string period,
		IEnumerable<PlayerComparisonDomain> players
	)
	{
		Bucket = bucket;
		ClassId = classId;
		Period = period;
		Players = players;
	}

	public string Bucket { get; }
	public int ClassId { get; }

	/// <summary>"recent" or "alltime" — echoes back which window was actually applied.</summary>
	public string Period { get; }
	public IEnumerable<PlayerComparisonDomain> Players { get; }
}
