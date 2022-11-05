using System.Text.Json.Serialization;

namespace TF2SA.Common.Models.LogsTF.LogListModel;

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
	public List<LogListItem> Logs { get; set; } = new List<LogListItem>();
}
