using Microsoft.EntityFrameworkCore;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data;

public partial class TF2SADbContext : DbContext
{
	public TF2SADbContext(DbContextOptions<TF2SADbContext> options)
		: base(options) { }

	public virtual DbSet<ClassStatsEntity> ClassStatsEntities { get; set; } =
		null!;

	public virtual DbSet<DemomanAllTimeEntity> DemomanAllTimeEntities { get; set; } =
		null!;

	public virtual DbSet<DemomanRecentEntity> DemomanRecentEntities { get; set; } =
		null!;

	public virtual DbSet<GamesEntity> GamesEntities { get; set; } = null!;

	public virtual DbSet<MedicAllTimeEntity> MedicAllTimeEntities { get; set; } =
		null!;

	public virtual DbSet<MedicRecentEntity> MedicRecentEntities { get; set; } =
		null!;

	public virtual DbSet<OverallStatsAllTimeEntity> OverallStatsAllTimeEntities { get; set; } =
		null!;

	public virtual DbSet<OverallStatsRecentEntity> OverallStatsRecentEntities { get; set; } =
		null!;

	public virtual DbSet<PlayerStatsEntity> PlayerStatsEntities { get; set; } =
		null!;

	public virtual DbSet<PlayersEntity> PlayersEntities { get; set; } = null!;

	public virtual DbSet<ScoutAllTimeEntity> ScoutAllTimeEntities { get; set; } =
		null!;

	public virtual DbSet<ScoutRecentEntity> ScoutRecentEntities { get; set; } =
		null!;

	public virtual DbSet<SoldierAllTimeEntity> SoldierAllTimeEntities { get; set; } =
		null!;

	public virtual DbSet<SoldierRecentEntity> SoldierRecentEntities { get; set; } =
		null!;

	public virtual DbSet<WeaponStatsEntity> WeaponStatsEntities { get; set; } =
		null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.UseCollation("utf8mb4_unicode_ci").HasCharSet("utf8mb4");

		modelBuilder.Entity<ClassStatsEntity>(entity =>
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
				.HasOne(d => d.PlayerStatsEntity)
				.WithMany(p => p.ClassStatsEntities)
				.HasForeignKey(d => d.PlayerStatsId)
				.HasConstraintName("fk_playerstats_id");
		});

		modelBuilder.Entity<DemomanAllTimeEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<DemomanRecentEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<GamesEntity>(entity =>
		{
			entity.HasKey(e => e.GameId).HasName("PRIMARY");

			entity
				.Property(e => e.GameId)
				.ValueGeneratedNever()
				.HasColumnType("int(10) unsigned")
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
			entity.Property(e => e.InvalidStatsReason).HasMaxLength(10000);
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

		modelBuilder.Entity<MedicAllTimeEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageHealsPm)
				.HasColumnName("AverageHealsPM");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopDrops)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDropsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDropsGameID");
			entity
				.Property(e => e.TopHeals)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopHealsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopHealsGameID");
			entity
				.Property(e => e.TopUbers)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopUbersGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopUbersGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<MedicRecentEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageHealsPm)
				.HasColumnName("AverageHealsPM");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopDrops)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDropsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDropsGameID");
			entity
				.Property(e => e.TopHeals)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopHealsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopHealsGameID");
			entity
				.Property(e => e.TopUbers)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopUbersGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopUbersGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<OverallStatsAllTimeEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<OverallStatsRecentEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<PlayerStatsEntity>(entity =>
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
				.HasOne(d => d.GameEntity)
				.WithMany(p => p.PlayerStatsEntities)
				.HasForeignKey(d => d.GameId)
				.HasConstraintName("fk_game_id");

			entity
				.HasOne(d => d.SteamEntity)
				.WithMany(p => p.PlayerStatsEntities)
				.HasForeignKey(d => d.SteamId)
				.HasConstraintName("fk_player_id");
		});

		modelBuilder.Entity<PlayersEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity.Property(e => e.AvatarFull).HasMaxLength(1000);
			entity.Property(e => e.AvatarHash).HasMaxLength(255);
			entity.Property(e => e.AvatarMedium).HasMaxLength(1000);
			entity.Property(e => e.LocalCountryCode).HasMaxLength(10);
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity.Property(e => e.ProfileUrl).HasMaxLength(1000);
			entity.Property(e => e.RealName).HasMaxLength(255);
		});

		modelBuilder.Entity<ScoutAllTimeEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<ScoutRecentEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<SoldierAllTimeEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<SoldierRecentEntity>(entity =>
		{
			entity.HasKey(e => e.SteamId).HasName("PRIMARY");

			entity
				.Property(e => e.SteamId)
				.ValueGeneratedNever()
				.HasColumnType("bigint(20) unsigned")
				.HasColumnName("SteamID");
			entity.Property(e => e.Avatar).HasMaxLength(1000);
			entity
				.Property(e => e.AverageDamageTakenPm)
				.HasColumnName("AverageDamageTakenPM");
			entity.Property(e => e.AverageDpm).HasColumnName("AverageDPM");
			entity
				.Property(e => e.AverageHealsReceivedPm)
				.HasColumnName("AverageHealsReceivedPM");
			entity
				.Property(e => e.AverageMedKitsHp)
				.HasColumnName("AverageMedKitsHP");
			entity.Property(e => e.Draws).HasColumnType("int(10) unsigned");
			entity.Property(e => e.Losses).HasColumnType("int(10) unsigned");
			entity
				.Property(e => e.NumberOfGames)
				.HasColumnType("smallint(5) unsigned");
			entity.Property(e => e.PlayerName).HasMaxLength(255);
			entity
				.Property(e => e.TopAirshots)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopAirshotsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopAirshotsGameID");
			entity
				.Property(e => e.TopDamage)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopDamageGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopDamageGameID");
			entity
				.Property(e => e.TopKills)
				.HasColumnType("smallint(5) unsigned");
			entity
				.Property(e => e.TopKillsGameId)
				.HasColumnType("int(10) unsigned")
				.HasColumnName("TopKillsGameID");
			entity.Property(e => e.Wins).HasColumnType("int(10) unsigned");
		});

		modelBuilder.Entity<WeaponStatsEntity>(entity =>
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
				.HasOne(d => d.ClassStatsEntity)
				.WithMany(p => p.WeaponStatsEntities)
				.HasForeignKey(d => d.ClassStatsId)
				.HasConstraintName("fk_class_stats");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
