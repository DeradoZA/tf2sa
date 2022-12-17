namespace TF2SA.Data.Entities.MariaDb;

public partial class Player
{
	public Player()
	{
		PlayerStats = new HashSet<PlayerStat>();
	}

	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }

	public virtual ICollection<PlayerStat> PlayerStats { get; set; }
}
