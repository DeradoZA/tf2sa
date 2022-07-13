using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class AverageMainStats
    {
        public AverageMainStats(ulong steamId, double? averageDPM, double? averageKills, double? averageAssists, double? averageDeaths)
        {
            this.SteamId = steamId;
            this.AverageDPM = averageDPM;
            this.AverageKills = averageKills;
            this.AverageAssists = averageAssists;
            this.AverageDeaths = averageDeaths;
        }

        public ulong SteamId { get; set; }
        public double? AverageDPM { get; set; }
        public double? AverageKills { get; set; }
        public double? AverageAssists { get; set; }
        public double? AverageDeaths { get; set; }
    }
}