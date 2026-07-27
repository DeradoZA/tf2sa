namespace TF2SA.Data.Analytics.Rows;

/// <summary>
/// A player ready for <see cref="IAnalyticsStore.UpsertPlayersAsync"/> or
/// <see cref="IAnalyticsStore.UpdatePlayersAsync"/> — the write-side counterpart to
/// the old EF <c>Player</c> entity. SteamId is always caller-supplied.
/// </summary>
public class PlayerWrite
{
	public ulong SteamId { get; set; }
	public string? LocalCountryCode { get; set; }
	public string? ProfileUrl { get; set; }
	public string? Avatar { get; set; }
	public string? AvatarMedium { get; set; }
	public string? AvatarFull { get; set; }
	public string? AvatarHash { get; set; }
	public string? PlayerName { get; set; }
	public string? RealName { get; set; }
}
