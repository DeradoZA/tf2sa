DROP PROCEDURE IF EXISTS UpdateScoutRecentStats;

DELIMITER $$
$$
CREATE PROCEDURE UpdateScoutRecentStats()
BEGIN
    
DELETE FROM ScoutRecent;
INSERT INTO ScoutRecent (
	SteamID,
	PlayerName,
	Avatar,
	NumberOfGames,
	AverageDPM,
	AverageKills,
	AverageAssists,
	AverageDeaths,
	TopKills,
	TopKillsGameID,
	TopDamage,
	TopDamageGameID
)
WITH ScoutGames AS (
	SELECT
		cs.ClassStatsID AS ClassStatsID,
		cs.PlayerStatsID AS PlayerStatsID,
		cs.ClassID AS ClassID,
		cs.Kills AS Kills,
		cs.Assists AS Assists,
		cs.Deaths AS Deaths,
		cs.Damage AS Damage,
		cs.Playtime AS Playtime,
		ps.GameID AS GameID,
		ps.SteamID AS SteamID,
		ps.TeamID AS TeamID,
		ps.DamageTaken AS DamageTaken,
		ps.HealsReceived AS HealsReceived,
		ps.LongestKillStreak AS LongestKillStreak,
		ps.Airshots AS Airshots,
		ps.Ubers AS Ubers,
		ps.Drops AS Drops,
		ps.Medkits AS Medkits,
		ps.MedkitsHP AS MedkisHP,
		ps.Backstabs AS Backstabs,
		ps.Headshots AS Headshots,
		ps.HeadshotsHit AS HeadshotsHit,
		ps.SentriesBuilt AS SentriesBuilt,
		ps.Heals AS Heals,
		ps.CapturePointsCaptured AS CapturePointsCaptured,
		ps.IntelCaptures AS IntelCaptures,
		p.PlayerName AS PlayerName,
		p.Avatar AS Avatar,
		g.Date AS Date
	FROM
		ClassStats cs
		JOIN PlayerStats ps ON ps.PlayerStatsID = cs.PlayerStatsID
		JOIN Players p ON p.SteamID = ps.SteamID
		JOIN Games g ON g.GameID = ps.GameID
	WHERE
		cs.ClassID = 1 AND 
		g.`Date` > UNIX_TIMESTAMP() - 365*24*60*60
),
MaxScoutKillGames AS (
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
			FROM ScoutGames
			ORDER BY SteamID, Kills DESC, Damage DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
),
MaxScoutDamageGames AS (
	SELECT
		SteamID,
		PlayerName,
		GameID AS MaxDamageGameID,
		Damage AS MaxDamage
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
			FROM ScoutGames
			ORDER BY SteamID, Damage DESC, Kills DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
)
SELECT 
	sg.SteamID AS SteamID,
	sg.PlayerName AS PlayerName,
	sg.Avatar AS Avatar,
	COUNT(sg.ClassStatsID) AS NumberOfGames,
	ROUND(AVG(sg.Damage/sg.Playtime*60),1) AS AverageDPM,
 	ROUND(AVG(sg.Kills),1) AS AverageKills,
 	ROUND(AVG(sg.Kills),1) AS AverageAssists,
 	ROUND(AVG(sg.Deaths),1) AS AverageDeaths,
	kg.MaxKills AS TopKills,
	kg.MaxKillsGameID AS TopKillsGameID,
	dg.MaxDamage AS TopDamage,
	dg.MaxDamageGameID AS TopDamageGameID
FROM
	ScoutGames sg
	LEFT JOIN MaxScoutKillGames kg on kg.SteamID = sg.SteamID
	LEFT JOIN MaxScoutDamageGames dg on dg.SteamID = sg.SteamID
GROUP BY sg.SteamID
HAVING NumberOfGames >= 5
ORDER BY AverageDPM DESC;

END$$
