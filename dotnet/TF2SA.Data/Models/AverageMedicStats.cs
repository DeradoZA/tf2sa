using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
	public class AverageMedicStats
	{
		public AverageMedicStats(
			ulong steamId,
			double? Drops,
			double? Ubers,
			double? Heals
		)
		{
			this.SteamId = steamId;
			this.Drops = Drops;
			this.Ubers = Ubers;
			this.Heals = Heals;
		}

		public ulong SteamId { get; set; }
		public double? Drops { get; set; }
		public double? Ubers { get; set; }
		public double? Heals { get; set; }
	}
}
