using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayer
{
	[JsonPropertyName("steamid")]
	public string SteamId { get; set; } = string.Empty;

	[JsonPropertyName("communityvisibilitystate")]
	public int Communityvisibilitystate { get; set; }

	[JsonPropertyName("profilestate")]
	public int Profilestate { get; set; }

	[JsonPropertyName("personaname")]
	public string PlayerName { get; set; } = string.Empty;

	[JsonPropertyName("profileurl")]
	public string ProfileUrl { get; set; } = string.Empty;

	[JsonPropertyName("avatar")]
	public string Avatar { get; set; } = string.Empty;

	[JsonPropertyName("avatarmedium")]
	public string AvatarMedium { get; set; } = string.Empty;

	[JsonPropertyName("avatarfull")]
	public string AvatarFull { get; set; } = string.Empty;

	[JsonPropertyName("avatarhash")]
	public string AvatarHash { get; set; } = string.Empty;

	[JsonPropertyName("lastlogoff")]
	public int Lastlogoff { get; set; }

	[JsonPropertyName("personastate")]
	public int Personastate { get; set; }

	[JsonPropertyName("realname")]
	public string RealName { get; set; } = string.Empty;

	[JsonPropertyName("primaryclanid")]
	public string Primaryclanid { get; set; } = string.Empty;

	[JsonPropertyName("timecreated")]
	public int Timecreated { get; set; }

	[JsonPropertyName("personastateflags")]
	public int Personastateflags { get; set; }

	[JsonPropertyName("loccountrycode")]
	public string LocalCountryCode { get; set; } = string.Empty;

	[JsonPropertyName("locstatecode")]
	public string Locstatecode { get; set; } = string.Empty;

	[JsonPropertyName("commentpermission")]
	public int? Commentpermission { get; set; }

	[JsonPropertyName("loccityid")]
	public int? Loccityid { get; set; }
}
