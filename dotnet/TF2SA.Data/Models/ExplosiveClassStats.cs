using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class ExplosiveClassStats
    {
        public ExplosiveClassStats(ulong steamId, string steamName, int numberOfGames, double? averageDPM, double? averageKills, double? averageAssists, double? averageDeaths, double? averageAirshots)
        {
            SteamId = steamId;
            SteamName = steamName;
            NumberOfGames = numberOfGames;
            AverageDPM = averageDPM;
            AverageKills = averageKills;
            AverageAssists = averageAssists;
            AverageDeaths = averageDeaths;
            AverageAirshots = averageAirshots;
        }

        public ulong SteamId { get; set; }
        public string SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? AverageDPM { get; set; }
        public double? AverageKills { get; set; }
        public double? AverageAssists { get; set; }
        public double? AverageDeaths { get; set; }
        public double? AverageAirshots { get; set; }
    }
}