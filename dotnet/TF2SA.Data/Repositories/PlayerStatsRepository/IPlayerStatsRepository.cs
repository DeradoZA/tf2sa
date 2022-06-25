using TF2SA.Data.Entities;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.PlayerStatsRepository
{
    public interface IPlayerStatsRepository : ICrudRepository<PlayerStat, uint>
    {

    }
}