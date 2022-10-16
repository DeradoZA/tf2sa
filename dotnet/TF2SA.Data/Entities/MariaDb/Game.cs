using System;
using System.Collections.Generic;

namespace TF2SA.Data.Entities.MariaDb
{
	public partial class Game
	{
		public Game()
		{
			PlayerStats = new HashSet<PlayerStat>();
		}

		public uint GameId { get; set; }
		public uint? Date { get; set; }
		public short? Duration { get; set; }
		public string? Map { get; set; }
		public byte? BluScore { get; set; }
		public byte? RedScore { get; set; }

		public virtual ICollection<PlayerStat> PlayerStats { get; set; }
	}
}
