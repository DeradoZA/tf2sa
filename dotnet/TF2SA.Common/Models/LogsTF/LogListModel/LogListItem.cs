using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.LogListModel;

public class LogListItem
{
	[JsonPropertyName("id")]
	public uint Id { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("map")]
	public string Map { get; set; } = string.Empty;

	[JsonPropertyName("date")]
	public int Date { get; set; }

	[JsonPropertyName("views")]
	public int Views { get; set; }

	[JsonPropertyName("players")]
	public int? Players { get; set; }
}
