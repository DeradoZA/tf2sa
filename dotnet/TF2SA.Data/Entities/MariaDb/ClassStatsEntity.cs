﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TF2SA.Data.Entities.MariaDb;

[Table("ClassStats")]
public partial class ClassStatsEntity
{
	public uint ClassStatsId { get; set; }

	public uint PlayerStatsId { get; set; }

	public byte ClassId { get; set; }

	public byte Kills { get; set; }

	public byte Assists { get; set; }

	public byte Deaths { get; set; }

	public uint Damage { get; set; }

	public ushort Playtime { get; set; }

	public virtual PlayerStatsEntity PlayerStatsEntity { get; set; } = null!;

	public virtual ICollection<WeaponStatsEntity> WeaponStatsEntities { get; } =
		new List<WeaponStatsEntity>();
}
