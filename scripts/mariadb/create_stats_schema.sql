CREATE TABLE
	IF NOT EXISTS Games (
		GameID INT UNSIGNED NOT NULL,
		IsValidStats BOOLEAN NOT NULL,
		InvalidStatsReason VARCHAR(255) NULL,
		Version TINYINT UNSIGNED NULL,
		Duration SMALLINT UNSIGNED NULL,
		Map VARCHAR(32) NULL,
		IsSupplemental BOOLEAN NULL,
		HasRealDamage BOOLEAN NULL,
		HasWeaponDamage BOOLEAN NOT NULL,
		HasAccuracy BOOLEAN NOT NULL,
		HasHP BOOLEAN NULL,
		HasHPReal BOOLEAN NULL,
		HasHeadshots BOOLEAN NOT NULL,
		HasHeadshotsHit BOOLEAN NOT NULL,
		HasBackstabs BOOLEAN NOT NULL,
		HasCapturePointsCaptured BOOLEAN NULL,
		HasSentriesBuilt BOOLEAN NULL,
		HasDamageTaken BOOLEAN NOT NULL,
		HasAirshots BOOLEAN NOT NULL,
		HasHealsReceived BOOLEAN NOT NULL,
		HasIntelCaptures BOOLEAN NULL,
		HasADScoring BOOLEAN NULL,
		Notifications VARCHAR(255) NULL,
		Title VARCHAR(32) NULL,
		Date INT UNSIGNED NOT NULL,
		UploaderID BIGINT UNSIGNED NULL,
		UploaderName VARCHAR(32) NULL,
		UploaderInfo VARCHAR(32) NULL,
		Success BOOLEAN NULL,
		PRIMARY KEY (GameID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS Players (
		SteamID BIGINT UNSIGNED NOT NULL,
		PlayerName VARCHAR(32) CHARACTER NULL
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
		--"team": "Blue",
		TeamID TINYINT UNSIGNED NOT NULL,
		-- STATS
		DamageTaken MEDIUMINT UNSIGNED NOT NULL,
		HealsReceived MEDIUMINT UNSIGNED NOT NULL,
		LongestKillStreak MEDIUMINT UNSIGNED NOT NULL,
		Airshots TINYINT UNSIGNED NOT NULL,
		Ubers TINYINT UNSIGNED NOT NULL,
		Drops TINYINT UNSIGNED NOT NULL,
		Medkits MEDIUMINT UNSIGNED NOT NULL,
		MedkitsHP MEDIUMINT UNSIGNED NOT NULL,
		Backstabs TINYINT UNSIGNED NOT NULL,
		Headshots TINYINT UNSIGNED NOT NULL,
		HeadshotsHit TINYINT UNSIGNED NOT NULL,
		SentriesBuilt TINYINT UNSIGNED NOT NULL,
		Heals MEDIUMINT UNSIGNED NOT NULL,
		CapturePointsCaptured TINYINT UNSIGNED NOT NULL,
		IntelCaptures TINYINT UNSIGNED NOT NULL,
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
		Playtime SMALLINT UNSIGNED NOT NULL CHECK (Playtime > 0),
		Kills TINYINT UNSIGNED NOT NULL,
		Assists TINYINT UNSIGNED NOT NULL,
		Deaths TINYINT UNSIGNED NOT NULL,
		Damage MEDIUMINT UNSIGNED NOT NULL,
		PRIMARY KEY (ClassStatsID),
		CONSTRAINT `fk_playerstats_id` FOREIGN KEY (PlayerStatsID) REFERENCES PlayerStats (PlayerStatsID) ON DELETE CASCADE ON UPDATE CASCADE
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS Weapons (
		WeaponID SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
		WeaponName VARCHAR(32),
		PRIMARY KEY (WeaponID)
	) ENGINE = InnoDB;

CREATE TABLE
	IF NOT EXISTS WeaponStats (
		PlayerStatsID INT UNSIGNED NOT NULL,
		WeaponID SMALLINT UNSIGNED NOT NULL,
		Accuracy DOUBLE,
		PRIMARY KEY (PlayerStatsID, WeaponID),
		CONSTRAINT `fk_player_stats` FOREIGN KEY (PlayerStatsID) REFERENCES PlayerStats (PlayerStatsID) ON DELETE CASCADE ON UPDATE RESTRICT,
		CONSTRAINT `fk_weapon_id` FOREIGN KEY (WeaponID) REFERENCES Weapons (WeaponID) ON DELETE CASCADE ON UPDATE RESTRICT
	) ENGINE = InnoDB;