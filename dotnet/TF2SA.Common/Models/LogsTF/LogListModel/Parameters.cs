using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.LogListModel;

public class Parameters
{
	[JsonPropertyName("player")]
	public string[] Player { get; set; } = Array.Empty<string>();

	[JsonPropertyName("uploader")]
	public string Uploader { get; set; } = string.Empty;

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("map")]
	public string Map { get; set; } = string.Empty;

	[JsonPropertyName("limit")]
	public int Limit { get; set; }

	[JsonPropertyName("offset")]
	public int Offset { get; set; }
}
