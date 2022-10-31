using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
	public class AverageAirshots
	{
		public AverageAirshots(ulong steamId, double? airshots)
		{
			this.SteamId = steamId;
			this.Airshots = airshots;
		}

		public ulong SteamId { get; set; }
		public double? Airshots { get; set; }
	}
}
