namespace TF2SA.Data.Models;

public class PlayerNumGames
{
	public PlayerNumGames(ulong steamId, string? steamName, int numberOfGames)
	{
		this.SteamId = steamId;
		this.NumberOfGames = numberOfGames;
		this.SteamName = steamName;
	}

	public ulong SteamId { get; set; }
	public string? SteamName { get; set; }
	public int NumberOfGames { get; set; }
}
