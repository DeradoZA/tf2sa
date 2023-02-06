USE tf2sa_db;

SET
	FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS WeaponStats,
Weapons,
ClassStats,
PlayerStats,
Players,
Games,
BlacklistGames,
Progress,
PollVotes,
PollOptions,
Polls,
Images,
Comments,
Threads,
Users,
OverallStatsAllTime,
OverallStatsRecent,
ScoutAllTime,
ScoutRecent,
SoldierAllTime,
SoldierRecent,
DemomanAllTime,
DemomanRecent,
MedicAllTime,
MedicRecent;

SET
	FOREIGN_KEY_CHECKS = 1;

CREATE TABLE
	IF NOT EXISTS Games (
		GameID INT UNSIGNED NOT NULL,
		IsValidStats BOOLEAN NOT NULL,
		InvalidStatsReason VARCHAR(10000) NULL,
		Version TINYINT UNSIGNED NULL,
		RedScore TINYINT UNSIGNED NULL,
		BlueScore TINYINT UNSIGNED NULL,
		Duration SMALLINT UNSIGNED NULL,
		Map VARCHAR(255) NULL,
		IsSupplemental BOOLEAN NULL,
		HasRealDamage BOOLEAN NULL,
		HasWeaponDamage BOOLEAN NOT NULL,
		HasAccuracy BOOLEAN NOT NULL,
		HasHP BOOLEAN NULL,
		HasHPReal BOOLEAN NULL,
		HasHeadshots BOOLEAN NOT NULL,
		HasHeadshotsHit BOOLEAN NOT NULL,
		HasBackstabs BOOLEAN NOT NULL,
		HasCapturePointsCaptured BOOLEAN NOT NULL,
		HasSentriesBuilt BOOLEAN NOT NULL,
		HasDamageTaken BOOLEAN NOT NULL,
		HasAirshots BOOLEAN NOT NULL,
		HasHealsReceived BOOLEAN NOT NULL,
		HasIntelCaptures BOOLEAN NOT NULL,
		HasADScoring BOOLEAN NULL,
		Notifications VARCHAR(1000) NULL,
		Title VARCHAR(255) NULL,
		Date INT UNSIGNED NOT NULL,
		UploaderID BIGINT UNSIGNED NULL,
		UploaderName VARCHAR(255) NULL,
		UploaderInfo VARCHAR(255) NULL,
		Success BOOLEAN NULL,
		PRIMARY KEY (GameID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS Players (
		SteamID BIGINT UNSIGNED NOT NULL,
		LocalCountryCode VARCHAR(10) NULL,
		ProfileUrl VARCHAR(1000) NULL,
		Avatar VARCHAR(1000) NULL,
		AvatarMedium VARCHAR(1000) NULL,
		AvatarFull VARCHAR(1000) NULL,
		AvatarHash VARCHAR(255) NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			RealName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB CHARACTER
SET
	= utf8mb4 COLLATE = utf8mb4_unicode_ci;

CREATE TABLE
	IF NOT EXISTS PlayerStats (
		PlayerStatsID INT UNSIGNED NOT NULL AUTO_INCREMENT,
		GameID INT UNSIGNED NOT NULL,
		SteamID BIGINT UNSIGNED NOT NULL,
		TeamID TINYINT UNSIGNED NOT NULL,
		DamageTaken MEDIUMINT UNSIGNED NULL,
		HealsReceived MEDIUMINT UNSIGNED NULL,
		LongestKillStreak MEDIUMINT UNSIGNED NOT NULL,
		Airshots TINYINT UNSIGNED NULL,
		Ubers TINYINT UNSIGNED NULL,
		Drops TINYINT UNSIGNED NULL,
		Medkits MEDIUMINT UNSIGNED NOT NULL,
		MedkitsHP MEDIUMINT UNSIGNED NULL,
		Backstabs TINYINT UNSIGNED NULL,
		Headshots TINYINT UNSIGNED NULL,
		HeadshotsHit TINYINT UNSIGNED NULL,
		SentriesBuilt TINYINT UNSIGNED NULL,
		Heals MEDIUMINT UNSIGNED NULL,
		CapturePointsCaptured TINYINT UNSIGNED NULL,
		IntelCaptures TINYINT UNSIGNED NULL,
		PRIMARY KEY (PlayerStatsID),
		CONSTRAINT `fk_game_id` FOREIGN KEY (GameID) REFERENCES Games (GameID) ON DELETE CASCADE ON UPDATE CASCADE,
		CONSTRAINT `fk_player_id` FOREIGN KEY (SteamID) REFERENCES Players (SteamID) ON DELETE CASCADE ON UPDATE CASCADE
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS ClassStats (
		ClassStatsID INT UNSIGNED NOT NULL AUTO_INCREMENT,
		PlayerStatsID INT UNSIGNED NOT NULL,
		ClassID TINYINT UNSIGNED NOT NULL,
		-- STATS
		Kills TINYINT UNSIGNED NOT NULL,
		Assists TINYINT UNSIGNED NOT NULL,
		Deaths TINYINT UNSIGNED NOT NULL,
		Damage MEDIUMINT UNSIGNED NOT NULL,
		Playtime SMALLINT UNSIGNED NOT NULL CHECK (Playtime > 0),
		PRIMARY KEY (ClassStatsID),
		CONSTRAINT `fk_playerstats_id` FOREIGN KEY (PlayerStatsID) REFERENCES PlayerStats (PlayerStatsID) ON DELETE CASCADE ON UPDATE CASCADE
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS WeaponStats (
		WeaponStatsID INT UNSIGNED NOT NULL AUTO_INCREMENT,
		ClassStatsID INT UNSIGNED NOT NULL,
		WeaponName VARCHAR(255) NOT NULL CHECK (TRIM(WeaponName) != ''),
		Kills TINYINT UNSIGNED NOT NULL,
		Damage MEDIUMINT UNSIGNED NULL,
		Shots MEDIUMINT UNSIGNED NULL,
		Hits MEDIUMINT UNSIGNED NULL,
		PRIMARY KEY (WeaponStatsID),
		CONSTRAINT `fk_class_stats` FOREIGN KEY (ClassStatsID) REFERENCES ClassStats (ClassStatsID) ON DELETE CASCADE ON UPDATE RESTRICT
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS OverallStatsAllTime (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			AverageAirshots FLOAT,
			AverageHeadshots FLOAT,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS OverallStatsRecent (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED NOT NULL,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			AverageAirshots FLOAT,
			AverageHeadshots FLOAT,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS ScoutAllTime (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS ScoutRecent (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS SoldierAllTime (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAirshots FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			TopAirshots SMALLINT UNSIGNED,
			TopAirshotsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS SoldierRecent (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAirshots FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			TopAirshots SMALLINT UNSIGNED,
			TopAirshotsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS DemomanAllTime (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAirshots FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			TopAirshots SMALLINT UNSIGNED,
			TopAirshotsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS DemomanRecent (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageDPM FLOAT,
			AverageKills FLOAT,
			AverageAirshots FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			TopKills SMALLINT UNSIGNED,
			TopKillsGameID INT UNSIGNED,
			TopDamage SMALLINT UNSIGNED,
			TopDamageGameID INT UNSIGNED,
			TopAirshots SMALLINT UNSIGNED,
			TopAirshotsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS MedicAllTime (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			AverageUbers FLOAT,
			AverageDrops FLOAT,
			AverageHPM FLOAT,
			TopHeals SMALLINT UNSIGNED,
			TopHealsGameID INT UNSIGNED,
			TopUbers SMALLINT UNSIGNED,
			TopUbersGameID INT UNSIGNED,
			TopDrops SMALLINT UNSIGNED,
			TopDropsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS MedicRecent (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(255) CHARACTER
		SET
			utf8mb4 COLLATE utf8mb4_unicode_ci,
			NumberOfGames SMALLINT UNSIGNED,
			AverageKills FLOAT,
			AverageAssists FLOAT,
			AverageDeaths FLOAT,
			AverageUbers FLOAT,
			AverageDrops FLOAT,
			AverageHPM FLOAT,
			TopHeals SMALLINT UNSIGNED,
			TopHealsGameID INT UNSIGNED,
			TopUbers SMALLINT UNSIGNED,
			TopUbersGameID INT UNSIGNED,
			TopDrops SMALLINT UNSIGNED,
			TopDropsGameID INT UNSIGNED,
			PRIMARY KEY (SteamID)
	) ENGINE = InnoDB;