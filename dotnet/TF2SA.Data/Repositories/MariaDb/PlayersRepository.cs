using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb
{
    public class PlayersRepository : IPlayersRepository<Player, ulong>
    {
        TF2SADbContext tF2SADbContext;

        public PlayersRepository(TF2SADbContext tF2SADbContext)
        {
            this.tF2SADbContext = tF2SADbContext;
        }

        public bool Delete(Player entity)
        {
            throw new NotImplementedException();
        }

        public List<Player> GetAll()
        {
            return GetAllQueryable().ToList();
        }

        public IQueryable<Player> GetAllQueryable()
        {
            return tF2SADbContext.Players.AsQueryable();
        }

        public Player GetById(ulong id)
        {
            throw new NotImplementedException();
        }

        public Player GetPlayerByName(string name)
        {
            throw new NotImplementedException();
        }

        public Player Insert(Player entity)
        {
            throw new NotImplementedException();
        }

        public Player Update(Player entity)
        {
            throw new NotImplementedException();
        }
    }
}