namespace TF2SA.Http.LogsTF.Config;

public class LogsTFConfig
{
	public const string LogsTFConfigSection = "LogsTF";
	public string BaseUrl { get; set; } = string.Empty;
	public ulong[] Uploaders { get; set; } = Array.Empty<ulong>();
}
