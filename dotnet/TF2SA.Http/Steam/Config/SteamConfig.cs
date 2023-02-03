namespace TF2SA.Http.Steam.Config;

public class SteamConfig
{
	public const string SteamConfigSection = "Steam";
	public string BaseUrl { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
}
