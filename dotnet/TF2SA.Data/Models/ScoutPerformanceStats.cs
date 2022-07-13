using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class ScoutPerformanceStats
    {
        public ScoutPerformanceStats(
            ulong steamId,
            string steamName, 
            int numberOfGames, 
            double? averageDPM, 
            double? averageKills, 
            double? averageAssists, 
            double? averageDeaths)
        {
            SteamId = steamId;
            SteamName = steamName;
            NumberOfGames = numberOfGames;
            AverageDPM = averageDPM;
            AverageKills = averageKills;
            AverageAssists = averageAssists;
            AverageDeaths = averageDeaths;
        }

        public ulong SteamId { get; set; }
        public string SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? AverageDPM { get; set; }
        public double? AverageKills { get; set; }
        public double? AverageAssists { get; set; }
        public double? AverageDeaths { get; set; }
    }
}