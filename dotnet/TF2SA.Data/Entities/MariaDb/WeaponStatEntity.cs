using System;

namespace TF2SA.Data.Entities.MariaDb;

public partial class WeaponStatEntity
{
	public uint WeaponStatsId { get; set; }

	public uint ClassStatsId { get; set; }

	public string WeaponName { get; set; } = null!;

	public byte Kills { get; set; }

	public uint? Damage { get; set; }

	public uint? Shots { get; set; }

	public uint? Hits { get; set; }

	public virtual ClassStatEntity ClassStatsEntity { get; set; } = null!;
}
