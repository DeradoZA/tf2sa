namespace TF2SA.Data.Models;

public class AverageHeadshots
{
	public AverageHeadshots(ulong steamId, double? headshots)
	{
		this.SteamId = steamId;
		this.Headshots = headshots;
	}

	public ulong SteamId { get; set; }
	public double? Headshots { get; set; }
}
