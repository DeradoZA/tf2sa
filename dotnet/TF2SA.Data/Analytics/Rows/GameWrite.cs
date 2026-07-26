namespace TF2SA.Data.Analytics.Rows;

/// <summary>
/// A fully-populated game ready for <see cref="IAnalyticsStore.InsertGameAsync"/> —
/// the write-side counterpart to the old EF <c>Game</c> entity graph. GameId is always
/// caller-supplied (the logs.tf log ID), never generated. PlayerStatsID/ClassStatsID/
/// WeaponStatsID have no field here at all — those surrogate keys are assigned inside
/// the store during insert, the same way EF used to fix them up automatically.
/// </summary>
public class GameWrite
{
	public uint GameId { get; set; }
	public bool IsValidStats { get; set; }
	public string? InvalidStatsReason { get; set; }
	public byte? Version { get; set; }
	public byte? RedScore { get; set; }
	public byte? BlueScore { get; set; }
	public ushort? Duration { get; set; }
	public string? Map { get; set; }
	public bool? IsSupplemental { get; set; }
	public bool? HasRealDamage { get; set; }
	public bool HasWeaponDamage { get; set; }
	public bool HasAccuracy { get; set; }
	public bool? HasHp { get; set; }
	public bool? HasHpReal { get; set; }
	public bool HasHeadshots { get; set; }
	public bool HasHeadshotsHit { get; set; }
	public bool HasBackstabs { get; set; }
	public bool HasCapturePointsCaptured { get; set; }
	public bool HasSentriesBuilt { get; set; }
	public bool HasDamageTaken { get; set; }
	public bool HasAirshots { get; set; }
	public bool HasHealsReceived { get; set; }
	public bool HasIntelCaptures { get; set; }
	public bool? HasAdScoring { get; set; }
	public string? Notifications { get; set; }
	public string? Title { get; set; }
	public uint Date { get; set; }
	public ulong? UploaderId { get; set; }
	public string? UploaderName { get; set; }
	public string? UploaderInfo { get; set; }
	public bool? Success { get; set; }
	public List<PlayerStatWrite> PlayerStats { get; set; } = new();
}

public class PlayerStatWrite
{
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

	// Medic uber-depth fields (logs.tf's per-player medic_stats block) — null for
	// non-medic players and for logs where logs.tf didn't compute them.
	public ushort? AdvantagesLost { get; set; }
	public ushort? BiggestAdvantageLost { get; set; }
	public byte? DeathsWith9599Uber { get; set; }
	public byte? DeathsWithin20sAfterUber { get; set; }
	public double? AvgTimeBeforeHealing { get; set; }
	public double? AvgTimeToBuild { get; set; }
	public double? AvgTimeBeforeUsing { get; set; }
	public double? AvgUberLength { get; set; }
	public List<ClassStatWrite> ClassStats { get; set; } = new();
}

public class ClassStatWrite
{
	public byte ClassId { get; set; }
	public byte Kills { get; set; }
	public byte Assists { get; set; }
	public byte Deaths { get; set; }
	public uint Damage { get; set; }
	public ushort Playtime { get; set; }
	public List<WeaponStatWrite> WeaponStats { get; set; } = new();
}

public class WeaponStatWrite
{
	public string WeaponName { get; set; } = string.Empty;
	public byte Kills { get; set; }
	public uint? Damage { get; set; }
	public uint? Shots { get; set; }
	public uint? Hits { get; set; }
}
