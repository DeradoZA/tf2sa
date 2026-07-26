using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.PlayerDetail;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.GetPlayersComparison;

public class GetPlayersComparisonQueryHandler
{
	private static readonly string[] AllowedBuckets = { "week", "month" };
	private const string DefaultBucket = "month";

	// 0 = overall (cross-class); otherwise the 1/2/4/7 convention used throughout —
	// only the classes with their own board (Scout/Soldier/Demoman/Medic).
	private static readonly int[] AllowedClassIds = { 0, 1, 2, 4, 7 };

	private const string DefaultPeriod = "alltime";

	private readonly IAnalyticsStore analyticsStore;

	public GetPlayersComparisonQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetPlayersComparisonResult>> Handle(
		GetPlayersComparisonQuery request,
		CancellationToken cancellationToken
	)
	{
		// This view exists only on the analytics store — there is no pre-aggregated
		// MariaDB table to fall back to, unlike the leaderboard handlers.
		if (!analyticsStore.Enabled)
		{
			return new DatabaseError(
				"Analytics store is disabled; player comparison is unavailable."
			);
		}

		// Whitelisted: both values reach SQL as literals, so neither may be
		// caller-controlled text. steamIds is a ulong list — a ulong can only render
		// as digits — guarded further at the controller (parsed, count capped at 6).
		string bucket = AllowedBuckets.Contains(
			request.Bucket,
			StringComparer.OrdinalIgnoreCase
		)
			? request.Bucket.ToLowerInvariant()
			: DefaultBucket;
		int classId = AllowedClassIds.Contains(request.ClassId)
			? request.ClassId
			: 0;
		int recentOnly = string.Equals(
			request.Period,
			"recent",
			StringComparison.OrdinalIgnoreCase
		)
			? 1
			: 0;
		string periodUsed = recentOnly == 1 ? "recent" : DefaultPeriod;

		if (request.SteamIds.Count == 0)
		{
			return new GetPlayersComparisonResult(
				bucket,
				classId,
				periodUsed,
				Enumerable.Empty<PlayerComparisonDomain>()
			);
		}

		string idList = $"[{string.Join(",", request.SteamIds)}]";
		string viewCall =
			classId == 0
				? $"players_progression_overall({idList}, '{bucket}', {recentOnly})"
				: $"players_progression_class({idList}, '{bucket}', {classId}, {recentOnly})";

		try
		{
			IReadOnlyList<PlayerProgressionRow> rows =
				await analyticsStore.GetAllAsync<PlayerProgressionRow>(
					viewCall,
					cancellationToken
				);

			List<PlayerComparisonDomain> players = rows.GroupBy(r => r.SteamId)
				.Select(g => new PlayerComparisonDomain
				{
					SteamId = g.Key,
					PlayerName = g.First().PlayerName,
					Avatar = g.First().Avatar,
					NumberOfGames = g.Sum(r => r.NumberOfGames),
					Progression = g.OrderBy(r => r.Bucket)
						.Select(r => new PlayerProgressionPointDomain
						{
							Bucket = r.Bucket,
							NumberOfGames = r.NumberOfGames,
							Wins = r.Wins,
							Draws = r.Draws,
							Losses = r.Losses,
							WinPercentage = r.WinPercentage,
							AverageDpm = r.AverageDpm,
							AverageKills = r.AverageKills,
							AverageDeaths = r.AverageDeaths,
							AverageAssists = r.AverageAssists,
							AverageAirshots = r.AverageAirshots,
						})
						.ToList(),
				})
				.ToList();

			// A selected player with zero games in this class/window has no row to
			// group, so they'd silently drop out of the legend. Keep them, with an
			// empty series — name/avatar are unavailable here without a second query,
			// so the frontend falls back to the SteamID for the legend/chip label.
			IEnumerable<ulong> missing = request.SteamIds.Except(
				players.Select(p => p.SteamId)
			);
			players.AddRange(
				missing.Select(id => new PlayerComparisonDomain
				{
					SteamId = id,
				})
			);

			return new GetPlayersComparisonResult(
				bucket,
				classId,
				periodUsed,
				players
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
