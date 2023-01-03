using Monad;
using TF2SA.Common.Errors;

namespace TF2SA.Data.Repositories.Base;

public interface IPlayersRepository<TPlayer, TPlayerId>
	: ICrudRepository<TPlayer, TPlayerId>
{
	public List<TPlayer> GetPlayerByName(string name);
	public Task<OptionStrict<Error>> InsertPlayersIfNotExists(
		IEnumerable<TPlayer> players,
		CancellationToken cancellationToken
	);
}
