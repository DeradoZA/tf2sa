using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Repositories.Base
{
    public interface IGamesRepository <TGame, TGameId> : ICrudRepository<TGame, TGameId>
    {
        
    }
}