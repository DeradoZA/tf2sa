using Microsoft.EntityFrameworkCore;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data;

public partial class TF2SADbContext : DbContext
{
	public TF2SADbContext() { }

	public TF2SADbContext(DbContextOptions<TF2SADbContext> options)
		: base(options) { }

	public virtual DbSet<ClassStat> ClassStats { get; set; } = null!;
	public virtual DbSet<Game> Games { get; set; } = null!;
	public virtual DbSet<Player> Players { get; set; } = null!;
	public virtual DbSet<PlayerStat> PlayerStats { get; set; } = null!;
	public virtual DbSet<WeaponStat> WeaponStats { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.UseCollation("utf8mb4_unicode_ci").HasCharSet("utf8mb4");

		modelBuilder.Entity<ClassStat>(entity =>
		{
			entity.HasKey(e => e.ClassStatsId).HasName("PRIMARY");

			entity.HasIndex(e => e.PlayerStatsId, "fk_playerstats_id");

			entity
				.Property(e => e.ClassStatsId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("ClassStatsID");

			entity
				.Property(e => e.Assists)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.ClassId)
				.HasColumnType("tinyint(3) unsigned")
				.HasColumnName("ClassID");

			entity
				.Property(e => e.Damage)
				.HasColumnType("mediumint(8) unsigned");

			entity.Property(e => e.Deaths).HasColumnType("tinyint(3) unsigned");

			entity.Property(e => e.Kills).HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.PlayerStatsId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("PlayerStatsID");

			entity
				.Property(e => e.Playtime)
				.HasColumnType("smallint(5) unsigned");

			entity
				.HasOne(d => d.PlayerStats)
				.WithMany(p => p.ClassStats)
				.HasForeignKey(d => d.PlayerStatsId)
				.HasConstraintName("fk_playerstats_id");
		});

		modelBuilder.Entity<Game>(entity =>
		{
			entity
				.Property(e => e.GameId)
				.HasColumnType("int(10) unsigned")
				.ValueGeneratedNever()
				.HasColumnName("GameID");

			entity
				.Property(e => e.BlueScore)
				.HasColumnType("tinyint(3) unsigned");

			entity.Property(e => e.Date).HasColumnType("int(10) unsigned");

			entity
				.Property(e => e.Duration)
				.HasColumnType("smallint(5) unsigned");

			entity.Property(e => e.HasAdscoring).HasColumnName("HasADScoring");

			entity.Property(e => e.HasHp).HasColumnName("HasHP");

			entity.Property(e => e.HasHpreal).HasColumnName("HasHPReal");

			entity.Property(e => e.InvalidStatsReason).HasMaxLength(1000);

			entity.Property(e => e.Map).HasMaxLength(255);

			entity.Property(e => e.Notifications).HasMaxLength(1000);

			entity
				.Property(e => e.RedScore)
				.HasColumnType("tinyint(3) unsigned");

			entity.Property(e => e.Title).HasMaxLength(255);

			entity
				.Property(e => e.UploaderId)
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("UploaderID");

			entity.Property(e => e.UploaderInfo).HasMaxLength(255);

			entity.Property(e => e.UploaderName).HasMaxLength(255);

			entity
				.Property(e => e.Version)
				.HasColumnType("tinyint(3) unsigned");
		});

		modelBuilder.Entity<Player>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.HasColumnType("bigint(20) unsigned")
				.ValueGeneratedNever()
				.HasColumnName("SteamID");

			entity.Property(e => e.PlayerName).HasMaxLength(255);
		});

		modelBuilder.Entity<PlayerStat>(entity =>
		{
			entity.HasKey(e => e.PlayerStatsId).HasName("PRIMARY");

			entity.HasIndex(e => e.GameId, "fk_game_id");

			entity.HasIndex(e => e.SteamId, "fk_player_id");

			entity
				.Property(e => e.PlayerStatsId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("PlayerStatsID");

			entity
				.Property(e => e.Airshots)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.Backstabs)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.CapturePointsCaptured)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.DamageTaken)
				.HasColumnType("mediumint(8) unsigned");

			entity.Property(e => e.Drops).HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.GameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("GameID");

			entity
				.Property(e => e.Headshots)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.HeadshotsHit)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.Heals)
				.HasColumnType("mediumint(8) unsigned");

			entity
				.Property(e => e.HealsReceived)
				.HasColumnType("mediumint(8) unsigned");

			entity
				.Property(e => e.IntelCaptures)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.LongestKillStreak)
				.HasColumnType("mediumint(8) unsigned");

			entity
				.Property(e => e.Medkits)
				.HasColumnType("mediumint(8) unsigned");

			entity
				.Property(e => e.MedkitsHp)
				.HasColumnType("mediumint(8) unsigned")
				.HasColumnName("MedkitsHP");

			entity
				.Property(e => e.SentriesBuilt)
				.HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.SteamId)
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");

			entity
				.Property(e => e.TeamId)
				.HasColumnType("tinyint(3) unsigned")
				.HasColumnName("TeamID");

			entity.Property(e => e.Ubers).HasColumnType("tinyint(3) unsigned");

			entity
				.HasOne(d => d.Game)
				.WithMany(p => p.PlayerStats)
				.HasForeignKey(d => d.GameId)
				.HasConstraintName("fk_game_id");

			entity
				.HasOne(d => d.Steam)
				.WithMany(p => p.PlayerStats)
				.HasForeignKey(d => d.SteamId)
				.HasConstraintName("fk_player_id");
		});

		modelBuilder.Entity<WeaponStat>(entity =>
		{
			entity.HasKey(e => e.WeaponStatsId).HasName("PRIMARY");

			entity.HasIndex(e => e.ClassStatsId, "fk_class_stats");

			entity
				.Property(e => e.WeaponStatsId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("WeaponStatsID");

			entity
				.Property(e => e.ClassStatsId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("ClassStatsID");

			entity
				.Property(e => e.Damage)
				.HasColumnType("mediumint(8) unsigned");

			entity.Property(e => e.Hits).HasColumnType("mediumint(8) unsigned");

			entity.Property(e => e.Kills).HasColumnType("tinyint(3) unsigned");

			entity
				.Property(e => e.Shots)
				.HasColumnType("mediumint(8) unsigned");

			entity.Property(e => e.WeaponName).HasMaxLength(255);

			entity
				.HasOne(d => d.ClassStats)
				.WithMany(p => p.WeaponStats)
				.HasForeignKey(d => d.ClassStatsId)
				.HasConstraintName("fk_class_stats");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
