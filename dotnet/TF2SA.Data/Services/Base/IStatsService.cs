using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Models;

namespace TF2SA.Data.Services.Base
{
    public interface IStatsService
    {
        public IQueryable<JoinedStats> PlayerStatsJoinQueryable();

        public List<JoinedStats> PlayerStatsJoinList();

        public List<AllTimeStats> AllTimeStats();
    }
}