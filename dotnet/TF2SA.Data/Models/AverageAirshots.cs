namespace TF2SA.Data.Models;

public class AverageAirshots
{
	public AverageAirshots(ulong steamId, double? airshots)
	{
		this.SteamId = steamId;
		this.Airshots = airshots;
	}

	public ulong SteamId { get; set; }
	public double? Airshots { get; set; }
}
