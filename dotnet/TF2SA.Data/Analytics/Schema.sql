-- Native DuckDB schema (ADR 0009): DuckDB is the system of record, not a derived
-- cache. Column names, types and nullability mirror the MariaDB schema this replaces
-- (scripts/mariadb/create_stats_schema.sql, and what DuckDB's own mysql scanner used
-- to infer when this was a copy — see docs/architecture/database.md) exactly, so
-- Leaderboards.sql and PlayerDetail.sql need no changes at all.
--
-- Run unconditionally (idempotent, IF NOT EXISTS) on every connection open — see
-- DuckDbAnalyticsStore.Connection(). A fresh file gets empty tables ready for
-- ingestion; an existing file is a no-op.
--
-- PlayerStats/ClassStats/WeaponStats carry app-generated surrogate keys (no DuckDB
-- identity/auto-increment concept) — see the seq_* sequences and DEFAULT nextval(...)
-- below. Games.GameID and Players.SteamID are never generated: always supplied by the
-- caller from logs.tf/Steam data, matching the source these tables replace.

CREATE SEQUENCE IF NOT EXISTS seq_playerstats START 1;
CREATE SEQUENCE IF NOT EXISTS seq_classstats START 1;
CREATE SEQUENCE IF NOT EXISTS seq_weaponstats START 1;

CREATE TABLE IF NOT EXISTS Players (
	SteamID UBIGINT PRIMARY KEY,
	LocalCountryCode VARCHAR,
	ProfileUrl VARCHAR,
	Avatar VARCHAR,
	AvatarMedium VARCHAR,
	AvatarFull VARCHAR,
	AvatarHash VARCHAR,
	PlayerName VARCHAR,
	RealName VARCHAR
);

CREATE TABLE IF NOT EXISTS Games (
	GameID UINTEGER PRIMARY KEY,
	IsValidStats BOOLEAN,
	InvalidStatsReason VARCHAR,
	Version UTINYINT,
	RedScore UTINYINT,
	BlueScore UTINYINT,
	Duration USMALLINT,
	Map VARCHAR,
	IsSupplemental BOOLEAN,
	HasRealDamage BOOLEAN,
	HasWeaponDamage BOOLEAN,
	HasAccuracy BOOLEAN,
	HasHP BOOLEAN,
	HasHPReal BOOLEAN,
	HasHeadshots BOOLEAN,
	HasHeadshotsHit BOOLEAN,
	HasBackstabs BOOLEAN,
	HasCapturePointsCaptured BOOLEAN,
	HasSentriesBuilt BOOLEAN,
	HasDamageTaken BOOLEAN,
	HasAirshots BOOLEAN,
	HasHealsReceived BOOLEAN,
	HasIntelCaptures BOOLEAN,
	HasADScoring BOOLEAN,
	Notifications VARCHAR,
	Title VARCHAR,
	Date UINTEGER,
	UploaderID UBIGINT,
	UploaderName VARCHAR,
	UploaderInfo VARCHAR,
	Success BOOLEAN
);

CREATE TABLE IF NOT EXISTS PlayerStats (
	PlayerStatsID UINTEGER PRIMARY KEY DEFAULT nextval('seq_playerstats'),
	GameID UINTEGER REFERENCES Games (GameID),
	SteamID UBIGINT REFERENCES Players (SteamID),
	TeamID UTINYINT,
	DamageTaken UINTEGER,
	HealsReceived UINTEGER,
	LongestKillStreak UINTEGER,
	Airshots UTINYINT,
	Ubers UTINYINT,
	Drops UTINYINT,
	Medkits UINTEGER,
	MedkitsHP UINTEGER,
	Backstabs UTINYINT,
	Headshots UTINYINT,
	HeadshotsHit UTINYINT,
	SentriesBuilt UTINYINT,
	Heals UINTEGER,
	CapturePointsCaptured UTINYINT,
	IntelCaptures UTINYINT
);

CREATE TABLE IF NOT EXISTS ClassStats (
	ClassStatsID UINTEGER PRIMARY KEY DEFAULT nextval('seq_classstats'),
	PlayerStatsID UINTEGER REFERENCES PlayerStats (PlayerStatsID),
	ClassID UTINYINT,
	Kills UTINYINT,
	Assists UTINYINT,
	Deaths UTINYINT,
	Damage UINTEGER,
	Playtime USMALLINT
);

CREATE TABLE IF NOT EXISTS WeaponStats (
	WeaponStatsID UINTEGER PRIMARY KEY DEFAULT nextval('seq_weaponstats'),
	ClassStatsID UINTEGER REFERENCES ClassStats (ClassStatsID),
	WeaponName VARCHAR,
	Kills UTINYINT,
	Damage UINTEGER,
	Shots UINTEGER,
	Hits UINTEGER
);
