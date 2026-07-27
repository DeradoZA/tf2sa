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
		IEnumerable<PlayerClassSplitDomain> classSplits,
		IEnumerable<PlayerMapSplitDomain> mapSplits,
		IEnumerable<PlayerWeaponSplitDomain> weaponSplits,
		IEnumerable<PlayerSynergyDomain> topTeammates,
		IEnumerable<PlayerSynergyDomain> toughestOpponents,
		MedicDepthDomain? medicDepth
	)
	{
		Summary = summary;
		Bucket = bucket;
		ClassId = classId;
		Progression = progression;
		ClassSplits = classSplits;
		MapSplits = mapSplits;
		WeaponSplits = weaponSplits;
		TopTeammates = topTeammates;
		ToughestOpponents = toughestOpponents;
		MedicDepth = medicDepth;
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
	public IEnumerable<PlayerMapSplitDomain> MapSplits { get; }
	public IEnumerable<PlayerWeaponSplitDomain> WeaponSplits { get; }

	/// <summary>
	/// Top 10 by the player's own win rate when sharing a team with them (min 3
	/// games together). <see cref="ToughestOpponents"/> is the same shape sorted the
	/// opposite way — worst results first.
	/// </summary>
	public IEnumerable<PlayerSynergyDomain> TopTeammates { get; }
	public IEnumerable<PlayerSynergyDomain> ToughestOpponents { get; }

	/// <summary>Null when the player has no medic uber-depth data on record.</summary>
	public MedicDepthDomain? MedicDepth { get; }
}
