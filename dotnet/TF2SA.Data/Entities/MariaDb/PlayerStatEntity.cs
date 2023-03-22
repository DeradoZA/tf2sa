﻿using System;

namespace TF2SA.Data.Entities.MariaDb;

public partial class PlayerStatEntity
{
	public uint PlayerStatsId { get; set; }

	public uint GameId { get; set; }

	public ulong SteamId { get; set; }

	public byte TeamId { get; set; }

	public uint? DamageTaken { get; set; }

	public uint? HealsReceived { get; set; }

	public uint LongestKillStreak { get; set; }

	public byte? Airshots { get; set; }

	public byte? Ubers { get; set; }

	public byte? Drops { get; set; }

	public uint Medkits { get; set; }

	public uint? MedkitsHp { get; set; }

	public byte? Backstabs { get; set; }

	public byte? Headshots { get; set; }

	public byte? HeadshotsHit { get; set; }

	public byte? SentriesBuilt { get; set; }

	public uint? Heals { get; set; }

	public byte? CapturePointsCaptured { get; set; }

	public byte? IntelCaptures { get; set; }

	public virtual ICollection<ClassStatEntity> ClassStatsEntities { get; } =
		new List<ClassStatEntity>();

	public virtual GameEntity GameEntity { get; set; } = null!;

	public virtual PlayerEntity SteamEntity { get; set; } = null!;
}