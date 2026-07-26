using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Query.Queries.GetPlayerDetail;

public class GetPlayerDetailQuery
{
	public GetPlayerDetailQuery(ulong steamId, string bucket, int classId)
	{
		SteamId = steamId;
		Bucket = bucket;
		ClassId = classId;
	}

	public ulong SteamId { get; }
	public string Bucket { get; }

	/// <summary>0 = overall (cross-class); otherwise 1/2/4/7 as elsewhere in this app.</summary>
	public int ClassId { get; }
}
