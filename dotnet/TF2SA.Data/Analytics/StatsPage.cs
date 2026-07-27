namespace TF2SA.Data.Analytics;

/// <summary>A page of leaderboard rows plus the sort/filter that were actually
/// applied (echoed back into the API response, mirroring the EF read path).</summary>
public class StatsPage<T>
{
	public IReadOnlyList<T> Rows { get; init; } = Array.Empty<T>();
	public int Total { get; init; }
	public string SortUsed { get; init; } = string.Empty;
	public string SortOrderUsed { get; init; } = string.Empty;
	public string FilterFieldUsed { get; init; } = string.Empty;
	public string FilterValueUsed { get; init; } = string.Empty;
}
