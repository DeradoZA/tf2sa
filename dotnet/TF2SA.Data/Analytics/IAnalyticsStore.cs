using Monad;
using TF2SA.Common.Errors;
using TF2SA.Data.Analytics.Rows;

namespace TF2SA.Data.Analytics;

/// <summary>
/// The embedded DuckDB store (ADR 0008, ADR 0009). DuckDB is the system of record —
/// ingestion writes land directly in these tables, and the leaderboard/player-detail
/// macros aggregate them on the fly. No refresh step exists: there is nothing to copy
/// from, so a write is immediately visible to every read.
/// </summary>
public interface IAnalyticsStore
{
	/// <summary>Whether the analytics store is enabled (config-driven).</summary>
	bool Enabled { get; }

	/// <summary>
	/// Insert a fully-populated game (with its PlayerStats/ClassStats/WeaponStats
	/// graph) in one transaction, rolled back whole on any failure. Surrogate keys
	/// (PlayerStatsID/ClassStatsID/WeaponStatsID) are assigned internally.
	/// </summary>
	Task<OptionStrict<Error>> InsertGameAsync(
		GameWrite game,
		CancellationToken cancellationToken
	);

	/// <summary>
	/// Insert any of <paramref name="players"/> not already present, by SteamID.
	/// Atomic per player (<c>INSERT ... ON CONFLICT DO NOTHING</c>), so concurrent
	/// ingestion of two logs that both introduce the same new player can no longer
	/// race — unlike the EF select-then-insert this replaces.
	/// </summary>
	Task<OptionStrict<Error>> UpsertPlayersAsync(
		IEnumerable<PlayerWrite> players,
		CancellationToken cancellationToken
	);

	/// <summary>
	/// Apply a batch of Steam-API-refreshed player profiles to every player in the
	/// table. Faithfully preserves the behaviour of the EF method this replaces,
	/// including two pre-existing quirks documented in known-issues.md: a player not
	/// present in <paramref name="updatedPlayers"/> has every profile column nulled
	/// out (this batch is expected to cover the whole table), and AvatarHash is set
	/// from the incoming record's Avatar, not its AvatarHash.
	/// </summary>
	Task<OptionStrict<Error>> UpdatePlayersAsync(
		IEnumerable<PlayerWrite> updatedPlayers,
		CancellationToken cancellationToken
	);

	/// <summary>
	/// Read a page of a leaderboard board (e.g. <c>class_board(1,5,1)</c>) with a
	/// whitelisted sort, an optional player-name filter, and paging. Rows are mapped
	/// onto <typeparamref name="T"/> (a row POCO in <c>Analytics/Rows</c>) by
	/// case-insensitive column name. <paramref name="defaultSortKey"/> is used when
	/// <paramref name="sort"/> is not a sortable column on <typeparamref name="T"/>.
	/// </summary>
	Task<StatsPage<T>> GetPageAsync<T>(
		string boardCall,
		string defaultSortKey,
		string sort,
		string sortOrder,
		string filterField,
		string filterValue,
		int offset,
		int count,
		CancellationToken cancellationToken
	)
		where T : class, new();

	/// <summary>
	/// Read every row of a view (e.g. <c>player_progression(76561…, 'month')</c>),
	/// mapped onto <typeparamref name="T"/> by case-insensitive column name. For views
	/// that are already complete and ordered — no paging, sorting, or filtering is
	/// applied, unlike <see cref="GetPageAsync"/>.
	/// </summary>
	Task<IReadOnlyList<T>> GetAllAsync<T>(
		string viewCall,
		CancellationToken cancellationToken
	)
		where T : class, new();

	/// <summary>
	/// Read the first row of a view, or <c>null</c> when it returns none. Views used
	/// here must return zero rows for "no such thing" rather than a row of NULLs.
	/// </summary>
	Task<T?> GetSingleAsync<T>(
		string viewCall,
		CancellationToken cancellationToken
	)
		where T : class, new();
}
