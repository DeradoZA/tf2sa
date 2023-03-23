using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace TF2SA.Data.Entities.MariaDb;

[Table("WeaponStats")]
public partial class WeaponStatsEntity
{
	public uint WeaponStatsId { get; set; }

	public uint ClassStatsId { get; set; }

	public string WeaponName { get; set; } = null!;

	public byte Kills { get; set; }

	public uint? Damage { get; set; }

	public uint? Shots { get; set; }

	public uint? Hits { get; set; }

	public virtual ClassStatsEntity ClassStatsEntity { get; set; } = null!;
}
