using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb
{
    public class PlayerStatsRepository : IPlayerStatsRepository<PlayerStat, uint>
    {
        TF2SADbContext tF2SADbContext;

        public PlayerStatsRepository(TF2SADbContext tF2SADbContext)
        {
            this.tF2SADbContext = tF2SADbContext;
        }

        public PlayerStat Delete(PlayerStat entity)
        {
            throw new NotImplementedException();
        }

        public List<PlayerStat> GetAll()
        {
            return GetAllQueryable().ToList();
        }

        public IQueryable<PlayerStat> GetAllQueryable()
        {
            return tF2SADbContext.PlayerStats.AsQueryable();
        }

        public PlayerStat GetById(uint id)
        {
            throw new NotImplementedException();
        }

        public PlayerStat Insert(PlayerStat entity)
        {
            throw new NotImplementedException();
        }

        public PlayerStat Update(PlayerStat entity)
        {
            throw new NotImplementedException();
        }
    }
}