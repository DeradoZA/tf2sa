using System.Web;

namespace TF2SA.Http.LogsTF.Models;

public class LogListQueryParams
{
	public string? Title { get; set; }
	public string? Map { get; set; }
	public uint? Uploader { get; set; }

	/// <summary>
	/// Providing multiple players finds the intersection of logs where
	/// both players are in the same game.
	/// </summary>
	public uint[]? Players { get; set; }
	public int? Limit { get; set; }
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

		return query?.ToString();
	}

}
