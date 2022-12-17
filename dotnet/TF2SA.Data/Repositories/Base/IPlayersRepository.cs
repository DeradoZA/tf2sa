namespace TF2SA.Data.Repositories.Base;

public interface IPlayersRepository<TPlayer, TPlayerId>
	: ICrudRepository<TPlayer, TPlayerId>
{
	public List<TPlayer> GetPlayerByName(string name);
}
