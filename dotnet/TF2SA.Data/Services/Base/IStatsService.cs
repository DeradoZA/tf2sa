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

        public List<PlayerPerformanceStats> AllTimeStats();

        public List<PlayerPerformanceStats> RecentStats();

        public List<ScoutPerformanceStats> ScoutStatsAllTime();

        public List<ScoutPerformanceStats> ScoutStatsRecent();

        public List<ExplosiveClassStats> SoldierStatsAllTime();

        public List<ExplosiveClassStats> SoldierStatsRecent();

        public List<ExplosiveClassStats> DemomanStatsAllTime();

        public List<ExplosiveClassStats> DemomanStatsRecent();

        public List<MedicPerformanceStats> MedicStatsAllTime();

        public List<MedicPerformanceStats> MedicStatsRecent();
    }
}