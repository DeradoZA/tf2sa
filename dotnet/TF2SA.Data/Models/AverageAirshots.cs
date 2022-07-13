using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class AverageAirshots
    {
        public AverageAirshots(ulong steamId, double? airshotsAverage)
        {
            this.SteamId = steamId;
            this.AirshotsAverage = airshotsAverage;
        }

        public ulong SteamId { get; set; }
        public double? AirshotsAverage { get; set; }
    }
}