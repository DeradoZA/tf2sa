using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Repositories.Base
{
	public interface IPlayersRepository<TPlayer, TPlayerId>
		: ICrudRepository<TPlayer, TPlayerId>
	{
		public List<TPlayer> GetPlayerByName(string name);
	}
}
