DROP PROCEDURE IF EXISTS UpdateOverallAllTimeStats;

DELIMITER $$
$$
CREATE PROCEDURE UpdateOverallAllTimeStats()
BEGIN
    
DELETE FROM OverallStatsAllTime;
INSERT INTO OverallStatsAllTime(
	SteamID,
	PlayerName,
	Avatar,
	NumberOfGames,
	Wins,
	Draws,
	Losses,
	AverageDPM,
	AverageKills,
	AverageAssists,
	AverageDeaths, 
	AverageAirshots,
	AverageHeadshots,
	AverageBackstabs,
	AverageDamageTakenPM,
	AverageHealsReceivedPM,
	AverageMedKitsHP,
	AverageCapturePointsCaptured,
	TopKills,
	TopKillsGameID,
	TopDamage,
	TopDamageGameID,
	TopAirshots,
	TopAirshotsGameID
)
WITH AllGames AS (
	SELECT
		g.GameID,
		g.Date,
		ps.PlayerStatsID,
		p.SteamID,
		p.PlayerName,
		p.Avatar,
		(
			CASE 
				WHEN (ps.TeamID = 0 AND g.RedScore > g.BlueScore) THEN 1
				WHEN (ps.TeamID = 1 AND g.BlueScore > g.RedScore) THEN 1
				ELSE 0
			END
		) AS Win,
		(
			CASE 
				WHEN (ps.TeamID = 0 AND g.RedScore < g.BlueScore) THEN 1
				WHEN (ps.TeamID = 1 AND g.BlueScore < g.RedScore) THEN 1
				ELSE 0
			END
		) AS Loss,
		(
			CASE 
				WHEN (g.BlueScore = g.RedScore) THEN 1 -- draw
				ELSE 0
			END
		) AS Draw,
		ps.Airshots,
		ps.DamageTaken,
		ps.HealsReceived,
		ps.CapturePointsCaptured,
		ps.MedkitsHP,
		ps.Headshots,
		ps.Backstabs
	FROM
		PlayerStats ps
		JOIN Players p ON p.SteamID = ps.SteamID
		JOIN Games g ON g.GameID = ps.GameID
),
DamageGames AS (
	SELECT
		ag.PlayerStatsID,
		ag.SteamID,
		ag.GameID,
		ag.Date,
		ag.PlayerName,
		SUM(cs.Playtime) as Playtime,
		SUM(cs.Damage) AS Damage,
		ag.Airshots AS Airshots,
		SUM(cs.Damage)/SUM(cs.Playtime)*60 AS DPM,
		SUM(cs.Kills) AS Kills,
		SUM(cs.Deaths) AS Deaths,
		SUM(cs.Assists) AS Assists,
		ag.DamageTaken/SUM(cs.Playtime)*60 AS DamageTakenPM,
		ag.HealsReceived/SUM(cs.Playtime)*60 AS HealsReceivedPM
	FROM 
		AllGames ag
		JOIN ClassStats cs on cs.PlayerStatsID = ag.PlayerStatsID
	WHERE
		cs.ClassID != 7
	GROUP BY ag.PlayerStatsID
),
ProjectileGames AS (
	SELECT
		ag.PlayerStatsID,
		ag.Airshots
	FROM 
		AllGames ag
		JOIN ClassStats cs on cs.PlayerStatsID = ag.PlayerStatsID
	WHERE
		cs.ClassID IN (2,4)
	GROUP BY ag.PlayerStatsID
),
SniperGames AS (
	SELECT
		ag.PlayerStatsID,
		ag.Headshots
	FROM 
		AllGames ag
		JOIN ClassStats cs on cs.PlayerStatsID = ag.PlayerStatsID
	WHERE
		cs.ClassID = 8
	GROUP BY ag.PlayerStatsID
),
SpyGames AS (
	SELECT
		ag.PlayerStatsID,
		ag.Backstabs
	FROM 
		AllGames ag
		JOIN ClassStats cs on cs.PlayerStatsID = ag.PlayerStatsID
	WHERE
		cs.ClassID = 9
	GROUP BY ag.PlayerStatsID
),
MaxKillGames AS (
	SELECT
		SteamID,
		GameID AS MaxKillsGameID,
		Kills AS MaxKills,
		rn
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Kills,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Kills
			FROM 
				DamageGames
			ORDER BY SteamID, Kills DESC, Damage DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
),
MaxAirshotGames AS (
	SELECT
		SteamID,
		GameID AS MaxAirshotsGameID,
		Airshots AS MaxAirshots,
		rn
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Airshots,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Airshots
			FROM 
				DamageGames
			ORDER BY SteamID, Airshots DESC, Damage DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
),
MaxDamageGames AS (
	SELECT
		SteamID,
		GameID AS MaxDamageGameID,
		Damage AS MaxDamage,
		rn
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Damage,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Damage
			FROM 
				DamageGames
			ORDER BY SteamID, Damage DESC, Kills DESC, Date DESC
			LIMIT 9999999
		) AS SortedDamage
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedDamage
	WHERE rn <= 1
)
SELECT
	ag.SteamID AS SteamID,
	ag.PlayerName AS PlayerName,
	ag.Avatar AS Avatar,
	COUNT(ag.GameID) AS NumberOfGames,
	ROUND(SUM(ag.Win),1) AS Wins,
	ROUND(SUM(ag.Draw),1) AS Draws,
	ROUND(SUM(ag.Loss),1) AS Losses,
	ROUND(AVG(dg.DPM),1) AS AverageDPM,
	ROUND(AVG(dg.Kills),1) AS AverageKills,
	ROUND(AVG(dg.Assists),1) AS AverageAssists,
	ROUND(AVG(dg.Deaths),1) AS AverageDeaths,
	ROUND(AVG(pg.Airshots),1) AS AverageAirshots,
	ROUND(AVG(sng.Headshots),1) AS AverageHeadshots,
	ROUND(AVG(spg.Backstabs),1) AS AverageBackstabs,
	ROUND(AVG(dg.DamageTakenPM),1) AS AverageDamageTakenPM,
	ROUND(AVG(dg.HealsReceivedPM),1) AS AverageHealsReceivedPM,
	ROUND(AVG(ag.MedKitsHP),1) AS AverageMedKitsHP,
	ROUND(AVG(ag.CapturePointsCaptured),1) AS AverageCapturePointsCaptured,
	mk.MaxKills AS TopKills,
	mk.MaxKillsGameID AS TopKillsGameID,
	md.MaxDamage AS TopDamage,
	md.MaxDamageGameID AS TopDamageGameID,
	ma.MaxAirshots AS TopAirshots,
	ma.MaxAirshotsGameID AS TopAirshotsGameID
FROM 
	AllGames ag
	LEFT JOIN DamageGames dg on dg.PlayerStatsID = ag.PlayerStatsID
	LEFT JOIN ProjectileGames pg on pg.PlayerStatsID = ag.PlayerStatsID
	LEFT JOIN SniperGames sng on sng.PlayerStatsID = ag.PlayerStatsID
	LEFT JOIN SpyGames spg on spg.PlayerStatsID = ag.PlayerStatsID
	LEFT JOIN MaxKillGames mk on mk.SteamID = ag.SteamID
	LEFT JOIN MaxAirshotGames ma on ma.SteamID = ag.SteamID
	LEFT JOIN MaxDamageGames md ON md.SteamID = ag.SteamID
GROUP BY
	ag.SteamID
HAVING NumberOfGames > 20
ORDER BY AverageDPM DESC;

END$$