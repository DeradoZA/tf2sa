namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerHttpResult
{
	public string SteamId { get; set; } = string.Empty;
	public string? PlayerName { get; set; }
	public string? LocalCountryCode { get; set; }
	public string? ProfileUrl { get; set; }
	public string? Avatar { get; set; }
	public string? AvatarMedium { get; set; }
	public string? AvatarFull { get; set; }
	public string? AvatarHash { get; set; }
	public string? RealName { get; set; }
}
