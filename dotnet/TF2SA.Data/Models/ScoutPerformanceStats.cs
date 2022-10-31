using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
	public class ScoutPerformanceStats
	{
		public ScoutPerformanceStats(
			ulong steamId,
			string? steamName,
			int numberOfGames,
			double? DPM,
			double? Kills,
			double? Assists,
			double? Deaths
		)
		{
			this.SteamId = steamId;
			this.SteamName = steamName;
			this.NumberOfGames = numberOfGames;
			this.DPM = DPM;
			this.Kills = Kills;
			this.Assists = Assists;
			this.Deaths = Deaths;
		}

		public ulong SteamId { get; set; }
		public string? SteamName { get; set; }
		public int NumberOfGames { get; set; }
		public double? DPM { get; set; }
		public double? Kills { get; set; }
		public double? Assists { get; set; }
		public double? Deaths { get; set; }
	}
}
