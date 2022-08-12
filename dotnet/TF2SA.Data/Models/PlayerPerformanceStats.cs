using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class PlayerPerformanceStats
    {
        public ulong SteamId { get; set; }
        public string? SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? DPM { get; set; }
        public double? Kills { get; set; }
        public double? Assists { get; set; }
        public double? Deaths { get; set; }
        public double? Airshots { get; set; }
        public double? Headshots { get; set; }
        public PlayerPerformanceStats(
            ulong steamId,
            string? steamName,
            int numberofGames,
            double? DPM,
            double? Kills,
            double? Assists,
            double? Deaths,
            double? Airshots,
            double? Headshots)
        {
            this.SteamId = steamId;
            this.SteamName = steamName;
            this.NumberOfGames = numberofGames;
            this.DPM = DPM;
            this.Kills = Kills;
            this.Assists = Assists;
            this.Deaths = Deaths;
            this.Airshots = Airshots;
            this.Headshots = Headshots;
        }


    }
}