using System;
using System.Collections.Generic;

namespace TF2SA.Data.Entities.MariaDb
{
	public partial class ClassStat
	{
		public uint ClassStatsId { get; set; }
		public uint PlayerStatsId { get; set; }
		public byte ClassId { get; set; }
		public ushort? Playtime { get; set; }
		public byte? Kills { get; set; }
		public byte? Assists { get; set; }
		public byte? Deaths { get; set; }
		public uint? Damage { get; set; }

		public virtual PlayerStat PlayerStats { get; set; } = null!;
	}
}
