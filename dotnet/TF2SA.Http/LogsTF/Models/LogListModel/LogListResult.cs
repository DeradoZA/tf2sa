using System.Text.Json.Serialization;

namespace TF2SA.Http.LogsTF.Models.LogListModel;

public class LogListResult
{
	[JsonPropertyName("success")]
	public bool Success { get; set; }

	[JsonPropertyName("results")]
	public int Results { get; set; }

	[JsonPropertyName("total")]
	public int Total { get; set; }

	[JsonPropertyName("parameters")]
	public Parameters Parameters { get; set; } = new Parameters();

	[JsonPropertyName("logs")]
	public LogListItem[] Logs { get; set; } = Array.Empty<LogListItem>();
}
