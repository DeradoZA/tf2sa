using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayer
{
	[JsonPropertyName("steamid")]
	public string Steamid { get; set; } = string.Empty;

	[JsonPropertyName("communityvisibilitystate")]
	public int Communityvisibilitystate { get; set; }

	[JsonPropertyName("profilestate")]
	public int Profilestate { get; set; }

	[JsonPropertyName("personaname")]
	public string Personaname { get; set; } = string.Empty;

	[JsonPropertyName("profileurl")]
	public string Profileurl { get; set; } = string.Empty;

	[JsonPropertyName("avatar")]
	public string Avatar { get; set; } = string.Empty;

	[JsonPropertyName("avatarmedium")]
	public string Avatarmedium { get; set; } = string.Empty;

	[JsonPropertyName("avatarfull")]
	public string Avatarfull { get; set; } = string.Empty;

	[JsonPropertyName("avatarhash")]
	public string Avatarhash { get; set; } = string.Empty;

	[JsonPropertyName("lastlogoff")]
	public int Lastlogoff { get; set; }

	[JsonPropertyName("personastate")]
	public int Personastate { get; set; }

	[JsonPropertyName("realname")]
	public string Realname { get; set; } = string.Empty;

	[JsonPropertyName("primaryclanid")]
	public string Primaryclanid { get; set; } = string.Empty;

	[JsonPropertyName("timecreated")]
	public int Timecreated { get; set; }

	[JsonPropertyName("personastateflags")]
	public int Personastateflags { get; set; }

	[JsonPropertyName("loccountrycode")]
	public string Loccountrycode { get; set; } = string.Empty;

	[JsonPropertyName("locstatecode")]
	public string Locstatecode { get; set; } = string.Empty;

	[JsonPropertyName("commentpermission")]
	public int? Commentpermission { get; set; }

	[JsonPropertyName("loccityid")]
	public int? Loccityid { get; set; }
}
