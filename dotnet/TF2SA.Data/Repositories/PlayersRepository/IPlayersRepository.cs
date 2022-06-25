using TF2SA.Data.Entities;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.PlayersRepository
{
    public interface IPlayersRepository : ICrudRepository<Player, ulong>
    {

    }
}