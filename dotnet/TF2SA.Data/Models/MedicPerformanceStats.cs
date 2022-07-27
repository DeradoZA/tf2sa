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
            this.DPM = averageDPM;
            this.Kills = averageKills;
            this.Assists = averageAssists;
            this.Deaths = averageDeaths;
            this.Drops = averageDrops;
            this.Ubers = averageUbers;
            this.Heals = averageHeals;
        }

        public ulong SteamId { get; set; }
        public string SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? DPM { get; set; }
        public double? Kills { get; set; }
        public double? Assists { get; set; }
        public double? Deaths { get; set; }
        public double? Drops { get; set; }
        public double? Ubers { get; set; }
        public double? Heals { get; set; }
    }
}