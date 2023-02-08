using System.Text.Json.Serialization;

namespace TF2SA.Http.Steam.Models.PlayerSummaries;

public class SteamPlayer
{
	[JsonPropertyName("steamid")]
	public string SteamId { get; set; } = string.Empty;

	[JsonPropertyName("communityvisibilitystate")]
	public long Communityvisibilitystate { get; set; }

	[JsonPropertyName("profilestate")]
	public long Profilestate { get; set; }

	[JsonPropertyName("personaname")]
	public string PlayerName { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("commentpermission")]
	public long? Commentpermission { get; set; }

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

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("lastlogoff")]
	public long? Lastlogoff { get; set; }

	[JsonPropertyName("personastate")]
	public long Personastate { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("realname")]
	public string RealName { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("primaryclanid")]
	public string Primaryclanid { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("timecreated")]
	public long? Timecreated { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("personastateflags")]
	public long? Personastateflags { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("loccountrycode")]
	public string LocalCountryCode { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("locstatecode")]
	public string Locstatecode { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("loccityid")]
	public long? Loccityid { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("gameextrainfo")]
	public string Gameextrainfo { get; set; } = string.Empty;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("gameid")]
	public string Gameid { get; set; } = string.Empty;
}
