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
}
