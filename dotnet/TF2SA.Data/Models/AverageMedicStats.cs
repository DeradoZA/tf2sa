using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class AverageMedicStats
    {
        public AverageMedicStats(ulong steamId, double? averageDrops, double? averageUbers, double? averageHeals)
        {
            this.SteamId = steamId;
            this.AverageDrops = averageDrops;
            this.AverageUbers = averageUbers;
            this.AverageHeals = averageHeals;
        }

        public ulong SteamId { get; set; }
        public double? AverageDrops { get; set; }
        public double? AverageUbers { get; set; }
        public double? AverageHeals { get; set; }
    }
}
