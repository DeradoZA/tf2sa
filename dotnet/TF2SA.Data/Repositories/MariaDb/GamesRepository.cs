using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Data.Repositories.MariaDb
{
	public class GamesRepository : IGamesRepository<Game, uint>
	{
		TF2SADbContext tF2SADbContext;

		public GamesRepository(TF2SADbContext tF2SADbContext)
		{
			this.tF2SADbContext = tF2SADbContext;
		}

		public Game Delete(Game entity)
		{
			throw new NotImplementedException();
		}

		public List<Game> GetAll()
		{
			return GetAllQueryable().ToList();
		}

		public IQueryable<Game> GetAllQueryable()
		{
			return tF2SADbContext.Games.AsQueryable();
		}

		public Game? GetById(uint id)
		{
			throw new NotImplementedException();
		}

		public Game Insert(Game entity)
		{
			throw new NotImplementedException();
		}

		public Game Update(Game entity)
		{
			throw new NotImplementedException();
		}
	}
}
