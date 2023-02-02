using SteamKit2;

namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class Player
{
	public Player(string steamId, string? playerName)
	{
		SteamId = MakeSteamIdFromString(steamId);
		PlayerName = playerName;
	}

	public static SteamID MakeSteamIdFromString(string steamId)
	{
		SteamID id = new();
		if (!id.SetFromSteam3String(steamId))
		{
			id.SetFromString(steamId, EUniverse.Public);
		}
		return id;
	}

	public SteamID? SteamId { get; set; }
	public string? PlayerName { get; set; }
}
