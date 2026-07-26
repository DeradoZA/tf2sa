using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Data.Analytics;

/// <summary>
/// The embedded DuckDB analytics read layer (ADR 0008). Holds a derived, columnar
/// copy of the raw tables and serves leaderboard aggregations on the fly.
/// </summary>
public interface IAnalyticsStore
{
	/// <summary>Whether the analytics store is enabled (config-driven).</summary>
	bool Enabled { get; }

	/// <summary>
	/// Rebuild the DuckDB store from MariaDB (raw tables + leaderboard macros).
	/// No-op when disabled. Errors are returned as values, never thrown.
	/// </summary>
	Task<OptionStrict<Error>> RefreshAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Read a page of a leaderboard board (e.g. <c>class_board(1,5,1)</c>) with a
	/// whitelisted sort, an optional player-name filter, and paging. Rows are mapped
	/// onto <typeparamref name="T"/> (an EF stat entity) by case-insensitive column
	/// name. <paramref name="defaultSortKey"/> is used when <paramref name="sort"/>
	/// is not a sortable column on <typeparamref name="T"/>.
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
