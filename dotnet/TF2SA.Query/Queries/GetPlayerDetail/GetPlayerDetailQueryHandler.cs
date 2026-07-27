using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.PlayerDetail;
using TF2SA.Data.Errors;
using TF2SA.Data.Mapping;

namespace TF2SA.Query.Queries.GetPlayerDetail;

public class GetPlayerDetailQueryHandler
{
	private static readonly string[] AllowedBuckets = { "week", "month" };
	private const string DefaultBucket = "month";

	// 0 = overall (cross-class); otherwise the 1/2/4/7 convention used throughout —
	// only the classes with their own board (Scout/Soldier/Demoman/Medic).
	private static readonly int[] AllowedClassIds = { 0, 1, 2, 4, 7 };

	// Minimum shared games before a teammate/opponent pairing is shown — filters
	// noise from one-off pairings, matching the small-round-number thresholds used
	// elsewhere (class_board/medic_board use 5/20).
	private const int MinSynergyGames = 3;

	// Display cap, not a SQL LIMIT, so it stays consistent if MinSynergyGames changes.
	private const int MaxSynergyRows = 10;

	private readonly IAnalyticsStore analyticsStore;

	public GetPlayerDetailQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetPlayerDetailResult>> Handle(
		GetPlayerDetailQuery request,
		CancellationToken cancellationToken
	)
	{
		// Whitelisted: the bucket reaches SQL as a literal, so it must never be
		// caller-controlled text. SteamId is a ulong, so it can only render as digits.
		string bucket = AllowedBuckets.Contains(
			request.Bucket,
			StringComparer.OrdinalIgnoreCase
		)
			? request.Bucket.ToLowerInvariant()
			: DefaultBucket;
		int classId = AllowedClassIds.Contains(request.ClassId)
			? request.ClassId
			: 0;

		try
		{
			PlayerSummaryRow? summaryRow =
				await analyticsStore.GetSingleAsync<PlayerSummaryRow>(
					$"player_summary({request.SteamId})",
					cancellationToken
				);

			if (summaryRow is null)
			{
				return new GetPlayerDetailResult(
					null,
					bucket,
					classId,
					Enumerable.Empty<PlayerProgressionPointDomain>(),
					Enumerable.Empty<PlayerClassSplitDomain>(),
					Enumerable.Empty<PlayerMapSplitDomain>(),
					Enumerable.Empty<PlayerWeaponSplitDomain>(),
					Enumerable.Empty<PlayerSynergyDomain>(),
					Enumerable.Empty<PlayerSynergyDomain>(),
					null
				);
			}

			// One id in the list — the same macros the comparison endpoint uses, so a
			// player's own page always agrees with how they show up in a comparison.
			// Always all-time (0): this page has no Recent/All-Time control.
			string viewCall =
				classId == 0
					? $"players_progression_overall([{request.SteamId}], '{bucket}', 0)"
					: $"players_progression_class([{request.SteamId}], '{bucket}', {classId}, 0)";
			IReadOnlyList<PlayerProgressionRow> progressionRows =
				await analyticsStore.GetAllAsync<PlayerProgressionRow>(
					viewCall,
					cancellationToken
				);

			IReadOnlyList<PlayerClassSplitRow> classSplitRows =
				await analyticsStore.GetAllAsync<PlayerClassSplitRow>(
					$"player_class_splits({request.SteamId})",
					cancellationToken
				);

			IReadOnlyList<PlayerMapSplitRow> mapSplitRows =
				await analyticsStore.GetAllAsync<PlayerMapSplitRow>(
					$"player_map_splits({request.SteamId})",
					cancellationToken
				);

			IReadOnlyList<PlayerWeaponSplitRow> weaponSplitRows =
				await analyticsStore.GetAllAsync<PlayerWeaponSplitRow>(
					$"player_weapon_splits({request.SteamId})",
					cancellationToken
				);

			IReadOnlyList<PlayerSynergyRow> teammateRows =
				await analyticsStore.GetAllAsync<PlayerSynergyRow>(
					$"player_teammate_synergy({request.SteamId}, {MinSynergyGames})",
					cancellationToken
				);

			IReadOnlyList<PlayerSynergyRow> opponentRows =
				await analyticsStore.GetAllAsync<PlayerSynergyRow>(
					$"player_opponent_synergy({request.SteamId}, {MinSynergyGames})",
					cancellationToken
				);

			MedicDepthRow? medicDepthRow =
				await analyticsStore.GetSingleAsync<MedicDepthRow>(
					$"medic_depth({request.SteamId})",
					cancellationToken
				);

			return new GetPlayerDetailResult(
				summaryRow.ToDomain(),
				bucket,
				classId,
				progressionRows.Select(r => r.ToDomain()),
				classSplitRows.Select(r => r.ToDomain()),
				mapSplitRows.Select(r => r.ToDomain()),
				weaponSplitRows.Select(r => r.ToDomain()),
				teammateRows
					.Select(r => r.ToDomain())
					.OrderByDescending(s => s.WinPercentage)
					.Take(MaxSynergyRows),
				opponentRows
					.Select(r => r.ToDomain())
					.OrderBy(s => s.WinPercentage)
					.Take(MaxSynergyRows),
				medicDepthRow?.ToDomain()
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
