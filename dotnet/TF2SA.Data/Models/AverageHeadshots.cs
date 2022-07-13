using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class AverageHeadshots
    {
        public AverageHeadshots(ulong steamId, double? headshotsAverage)
        {
            this.SteamId = steamId;
            this.HeadshotsAverage = headshotsAverage;
        }

        public ulong SteamId { get; set; }
        public double? HeadshotsAverage { get; set; }
    }
}