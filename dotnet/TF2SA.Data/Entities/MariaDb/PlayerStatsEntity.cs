using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace TF2SA.Data.Entities.MariaDb;

[Table("PlayerStats")]
public partial class PlayerStatsEntity
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

	public virtual ICollection<ClassStatsEntity> ClassStatsEntities { get; } =
		new List<ClassStatsEntity>();

	public virtual GamesEntity GameEntity { get; set; } = null!;

	public virtual PlayersEntity SteamEntity { get; set; } = null!;
}
