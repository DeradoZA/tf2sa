namespace TF2SA.Http.LogsTF.Models
{
	public class LogListQueryParams
	{
		public string? Title { get; set; }
		public string? Map { get; set; }
		public uint? Uploader { get; set; }
		public uint[]? Players { get; set; }
		public int? Limit { get; set; }
		public int? Offset { get; set; }
	}
}
