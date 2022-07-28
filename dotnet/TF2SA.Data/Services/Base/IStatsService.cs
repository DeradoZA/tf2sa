using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF2SA.Data.Models;

namespace TF2SA.Data.Services.Base
{
    public interface IStatsService<TId>
    {
        public IQueryable<JoinedStats> PlayerStatsJoinQueryable();

        public List<JoinedStats> PlayerStatsJoinList();

        public List<PlayerPerformanceStats> AllTimeStats();

        public List<PlayerPerformanceStats> RecentStats();

        public List<ScoutPerformanceStats> ScoutStatsAllTime();

        public List<ScoutPerformanceStats> ScoutStatsRecent();

        public List<ExplosiveClassStats> SoldierStatsAllTime(TId steamid);

        public List<ExplosiveClassStats> SoldierStatsRecent();

        public List<ExplosiveClassStats> DemomanStatsAllTime(TId steamid);

        public List<ExplosiveClassStats> DemomanStatsRecent();

        public List<MedicPerformanceStats> MedicStatsAllTime(TId steamid);

        public List<MedicPerformanceStats> MedicStatsRecent();

        public List<PlayerHighlights> PlayerHighlightCollector(TId steamid);

        public List<AverageMainStats> MainStatsCollector(int timeFrame, int classID, TId steamid, bool avg = true);
    }
}