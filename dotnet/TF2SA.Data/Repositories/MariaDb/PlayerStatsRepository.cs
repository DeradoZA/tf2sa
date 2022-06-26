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

        public bool Delete(PlayerStat entity)
        {
            throw new NotImplementedException();
        }

        public List<PlayerStat> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<PlayerStat> GetAllQueryable()
        {
            throw new NotImplementedException();
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