using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class MedicPerformanceStats
    {
        public MedicPerformanceStats(ulong steamId, string steamName, int numberOfGames, double? averageDPM, double? averageKills, double? averageAssists, double? averageDeaths, double? averageDrops, double? averageUbers, double? averageHeals)
        {
            this.SteamId = steamId;
            this.SteamName = steamName;
            this.NumberOfGames = numberOfGames;
            this.AverageDPM = averageDPM;
            this.AverageKills = averageKills;
            this.AverageAssists = averageAssists;
            this.AverageDeaths = averageDeaths;
            this.AverageDrops = averageDrops;
            this.AverageUbers = averageUbers;
            this.AverageHeals = averageHeals;
        }

        public ulong SteamId { get; set; }
        public string SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? AverageDPM { get; set; }
        public double? AverageKills { get; set; }
        public double? AverageAssists { get; set; }
        public double? AverageDeaths { get; set; }
        public double? AverageDrops { get; set; }
        public double? AverageUbers { get; set; }
        public double? AverageHeals { get; set; }
    }
}