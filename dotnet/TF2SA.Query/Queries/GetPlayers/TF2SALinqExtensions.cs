using PlayerEntity = TF2SA.Data.Entities.MariaDb.Player;
using System.Linq.Expressions;

namespace TF2SA.Query.Queries.GetPlayers;

public static class TF2SALinqExtensions
{
	private static readonly Dictionary<
		string,
		Expression<Func<PlayerEntity, object>>
	> PlayerPropertyKeySelectors =
		new()
		{
			{ "steamId", x => x.SteamId },
			{ "playerName", x => x.PlayerName! },
		};

	public enum SortOrder
	{
		asc,
		desc
	}

	private static SortOrder GetSortOrder(string sortOrder)
	{
		if (Enum.TryParse(sortOrder, out SortOrder sort))
		{
			return sort;
		}
		return SortOrder.asc;
	}

	/// <summary>
	/// Apply filtering to PlayerEntity IQueryable<PlayerEntity>. Filters all properties by the same string
	/// </summary>
	/// <param name="filterString"> string to filter by </param>
	public static IQueryable<PlayerEntity> ApplyFilter(
		this IQueryable<PlayerEntity> playerQueryable,
		string filterString,
		out string filterStringUsed
	)
	{
		if (string.IsNullOrWhiteSpace(filterString))
		{
			filterStringUsed = "";
			return playerQueryable;
		}
		filterStringUsed = filterString;

		return playerQueryable.Where(
			p =>
				p.PlayerName!.Contains(filterString)
				|| p.SteamId.ToString().Contains(filterString)
		);
	}

	/// <summary>
	/// Apply sorting to PlayerEntity IQueryable<PlayerEntity>
	/// </summary>
	/// <param name="sort">The first parameter</param>
	/// <param name="sortOrder"> asc / desc </param>
	/// <returns> Sorted queryable. Defaults to PlayerName Desc given invalid params. </returns>
	public static IOrderedQueryable<PlayerEntity> ApplySort(
		this IQueryable<PlayerEntity> playerQueryable,
		string sort,
		string sortOrder,
		out string sortUsed,
		out string sortOrderUsed
	)
	{
		sortOrderUsed = "asc";
		sortUsed = "playerName";
		switch (GetSortOrder(sortOrder))
		{
			case SortOrder.asc:
				if (
					PlayerPropertyKeySelectors.TryGetValue(
						sort,
						out Expression<Func<PlayerEntity, object>>? ascSelector
					)
				)
				{
					sortUsed = sort;
					return playerQueryable.OrderBy(ascSelector);
				}

				return playerQueryable.OrderBy(p => p.PlayerName);
			case SortOrder.desc:
				sortOrderUsed = "desc";
				if (
					PlayerPropertyKeySelectors.TryGetValue(
						sort,
						out Expression<Func<PlayerEntity, object>>? descSelector
					)
				)
				{
					sortUsed = sort;
					return playerQueryable.OrderByDescending(descSelector);
				}
				return playerQueryable.OrderByDescending(p => p.PlayerName);
		}

		return playerQueryable.OrderBy(p => p.PlayerName);
	}
}
