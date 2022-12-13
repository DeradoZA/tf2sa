using System.Web;

namespace TF2SA.Common.Models.LogsTF.LogListModel;

public class LogListQueryParams
{
	public string? Title { get; set; }
	public string? Map { get; set; }
	public ulong? Uploader { get; set; }

	/// <summary>
	/// Providing multiple players finds the intersection of logs where
	/// both players are in the same game.
	/// </summary>
	public ulong[]? Players { get; set; }
	public int? Limit { get; set; }
	public const int LIMIT_MAX = 10000;
	public int? Offset { get; set; }

	public static string? GetQueryString(LogListQueryParams filter)
	{
		var query = HttpUtility.ParseQueryString(string.Empty);

		if (filter.Title is not null)
		{
			query["title"] = filter.Title;
		}

		if (filter.Uploader is not null)
		{
			query["uploader"] = $"{filter.Uploader}";
		}

		if (filter.Limit is not null)
		{
			query["limit"] = $"{filter.Limit}";
		}

		if (filter.Offset is not null)
		{
			query["offset"] = $"{filter.Offset}";
		}

		return query?.ToString();
	}

}
