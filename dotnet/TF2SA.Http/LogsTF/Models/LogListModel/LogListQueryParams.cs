namespace TF2SA.Http.LogsTF.Models;

public class LogListQueryParams
{
	public string Title { get; set; } = string.Empty;
	public string Map { get; set; } = string.Empty;
	public uint Uploader { get; set; }
	public uint[] Players { get; set; } = Array.Empty<uint>();
	public int Limit { get; set; }
	public int Offset { get; set; }
}
