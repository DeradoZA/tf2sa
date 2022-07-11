using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class AllTimeStats
    {
        public ulong SteamId { get; set; }
        public string SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? AverageDPM { get; set; }
        public double? AverageKills { get; set; }
        public double? AverageAssists { get; set; }
        public double? AverageDeaths { get; set; }
        public double? AverageAirshots { get; set; }
        public double? AverageHeadshots { get; set; }
        public AllTimeStats(ulong steamId,
                            string steamName,
                            int numberofGames,
                            double? averageDPM,
                            double? averageKills,
                            double? averageAssists,
                            double? averageDeaths,
                            double? averageAirshots,
                            double? averageHeadshots)
        {
            this.SteamId = steamId;
            this.SteamName = steamName;
            this.NumberOfGames = numberofGames;
            this.AverageDPM = averageDPM;
            this.AverageKills = averageKills;
            this.AverageAssists = averageAssists;
            this.AverageDeaths = averageDeaths;
            this.AverageAirshots = averageAirshots;
            this.AverageHeadshots = averageHeadshots;
        }


    }
}