using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class ExplosiveClassStats
    {
        public ExplosiveClassStats(ulong steamId, string? steamName, int numberOfGames, double? DPM, double? Kills, double? Assists, double? Deaths, double? Airshots)
        {
            SteamId = steamId;
            SteamName = steamName;
            NumberOfGames = numberOfGames;
            this.DPM = DPM;
            this.Kills = Kills;
            this.Assists = Assists;
            this.Deaths = Deaths;
            this.Airshots = Airshots;
        }

        public ulong SteamId { get; set; }
        public string? SteamName { get; set; }
        public int NumberOfGames { get; set; }
        public double? DPM { get; set; }
        public double? Kills { get; set; }
        public double? Assists { get; set; }
        public double? Deaths { get; set; }
        public double? Airshots { get; set; }
    }
}