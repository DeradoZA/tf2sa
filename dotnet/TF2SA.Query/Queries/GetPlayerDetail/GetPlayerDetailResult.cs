using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetPlayerDetail;

/// <summary>
/// A player's detail view. Deliberately not a <c>FetchPlayersQueryResponse</c> — that
/// envelope describes a paged list of players, whereas this is one player's composite
/// view. <see cref="Summary"/> is null when no such player has any recorded games.
/// </summary>
public class GetPlayerDetailResult
{
	public GetPlayerDetailResult(
		PlayerSummaryDomain? summary,
		string bucket,
		int classId,
		IEnumerable<PlayerProgressionPointDomain> progression,
		IEnumerable<PlayerClassSplitDomain> classSplits
	)
	{
		Summary = summary;
		Bucket = bucket;
		ClassId = classId;
		Progression = progression;
		ClassSplits = classSplits;
	}

	public PlayerSummaryDomain? Summary { get; }
	public string Bucket { get; }

	/// <summary>
	/// Which class <see cref="Progression"/> is scoped to: 0 = overall (cross-class);
	/// otherwise 1/2/4/7. <see cref="ClassSplits"/> is always every class, regardless.
	/// </summary>
	public int ClassId { get; }
	public IEnumerable<PlayerProgressionPointDomain> Progression { get; }
	public IEnumerable<PlayerClassSplitDomain> ClassSplits { get; }
}
