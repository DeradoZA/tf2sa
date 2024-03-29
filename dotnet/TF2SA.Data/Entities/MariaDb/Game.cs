﻿namespace TF2SA.Data.Entities.MariaDb;

public partial class Game
{
	public Game()
	{
		PlayerStats = new HashSet<PlayerStat>();
	}

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
	public bool? HasHpreal { get; set; }
	public bool HasHeadshots { get; set; }
	public bool HasHeadshotsHit { get; set; }
	public bool HasBackstabs { get; set; }
	public bool HasCapturePointsCaptured { get; set; }
	public bool HasSentriesBuilt { get; set; }
	public bool HasDamageTaken { get; set; }
	public bool HasAirshots { get; set; }
	public bool HasHealsReceived { get; set; }
	public bool HasIntelCaptures { get; set; }
	public bool? HasAdscoring { get; set; }
	public string? Notifications { get; set; }
	public string? Title { get; set; }
	public uint Date { get; set; }
	public ulong? UploaderId { get; set; }
	public string? UploaderName { get; set; }
	public string? UploaderInfo { get; set; }
	public bool? Success { get; set; }

	public virtual ICollection<PlayerStat> PlayerStats { get; set; }
}
