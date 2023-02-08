namespace TF2SA.Data.Entities.MariaDb;

public partial class Player
{
	public Player()
	{
		PlayerStats = new HashSet<PlayerStat>();
	}

	public ulong SteamId { get; set; }
	public string? LocalCountryCode { get; set; }
	public string? ProfileUrl { get; set; }
	public string? Avatar { get; set; }
	public string? AvatarMedium { get; set; }
	public string? AvatarFull { get; set; }
	public string? AvatarHash { get; set; }
	public string? PlayerName { get; set; }
	public string? RealName { get; set; }

	public virtual ICollection<PlayerStat> PlayerStats { get; set; }
}
