namespace TF2SA.Data.Analytics.Rows;

/// <summary>One row of the Players table — the read-side counterpart to PlayerWrite.</summary>
public class PlayerRow
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
