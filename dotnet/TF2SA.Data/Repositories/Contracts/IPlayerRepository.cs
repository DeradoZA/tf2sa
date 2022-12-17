using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data.Repositories.Contracts;

public interface IPlayerRepository
{
	public List<Player> GetAllPlayers();

	public Player UpdatePlayer(ulong SteamId, string name);

	public Player RetrievePlayer(ulong SteamId);

	public void DeletePlayer(ulong SteamId);
}
