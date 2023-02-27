DROP PROCEDURE IF EXISTS UpdateDemomanAllTimeStats;

DELIMITER //

CREATE PROCEDURE UpdateDemomanAllTimeStats()
BEGIN
    
DELETE FROM DemomanAllTime;
INSERT INTO DemomanAllTime (
	SteamID,
	PlayerName,
	Avatar,
	NumberOfGames,
	Wins,
	WinPercentage,
	Draws,
	Losses,
	AverageDPM,
	AverageKills,
	AverageAssists,
	AverageDeaths,
	AverageAirshots,
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
WITH Games AS (
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
				WHEN (g.BlueScore = g.RedScore) THEN 1
				ELSE 0
			END
		) AS Draw,
		ps.DamageTaken AS DamageTaken,
		ps.HealsReceived AS HealsReceived,
		ps.LongestKillStreak AS LongestKillStreak,
		ps.Airshots AS Airshots,
		ps.Ubers AS Ubers,
		ps.Drops AS Drops,
		ps.Medkits AS Medkits,
		ps.MedkitsHP AS MedkitsHP,
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
		cs.ClassID = 4
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
			FROM Games
			ORDER BY SteamID, Kills DESC, Damage DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
),
MaxDamageGames AS (
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
			FROM Games
			ORDER BY SteamID, Damage DESC, Kills DESC, Date DESC
			LIMIT 9999999
		) AS SortedKills
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedKills
	WHERE rn <= 1
),
MaxAirshotGames AS (
	SELECT
		SteamID,
		PlayerName,
		GameID AS MaxAirshotsGameID,
		Airshots AS MaxAirshots
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
			FROM Games
			ORDER BY SteamID, Airshots DESC, Kills DESC, Date DESC
			LIMIT 9999999
		) AS SortedAirshots
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedAirshots
	WHERE rn <= 1
)
SELECT 
	sg.SteamID AS SteamID,
	sg.PlayerName AS PlayerName,
	sg.Avatar AS Avatar,
	COUNT(sg.ClassStatsID) AS NumberOfGames,
	SUM(sg.Win) AS Wins,
	ROUND(SUM(sg.Win)/COUNT(sg.ClassStatsID)*100,1) AS WinPercentage,
	SUM(sg.Draw) AS Draws,
	SUM(sg.Loss) AS Losses,
	ROUND(AVG(sg.Damage/sg.Playtime*60),1) AS AverageDPM,
 	ROUND(AVG(sg.Kills),1) AS AverageKills,
 	ROUND(AVG(sg.Kills),1) AS AverageAssists,
 	ROUND(AVG(sg.Deaths),1) AS AverageDeaths,
 	ROUND(AVG(sg.Airshots),1) AS AverageAirshots,
 	ROUND(AVG(sg.DamageTaken/sg.Playtime*60),1) AS AverageDamageTakenPM,
 	ROUND(AVG(sg.HealsReceived/sg.Playtime*60),1) AS AverageHealsReceivedPM,
 	ROUND(AVG(sg.MedkitsHP),1) AS AverageMedKitsHP,
	ROUND(AVG(sg.CapturePointsCaptured),1) AS AverageCapturePointsCaptured,
	kg.MaxKills AS TopKills,
	kg.MaxKillsGameID AS TopKillsGameID,
	dg.MaxDamage AS TopDamage,
	dg.MaxDamageGameID AS TopDamageGameID,
	ag.MaxAirshots AS TopAirshots,
	ag.MaxAirshotsGameID AS TopAirshotsGameID
FROM
	Games sg
	LEFT JOIN MaxKillGames kg on kg.SteamID = sg.SteamID
	LEFT JOIN MaxDamageGames dg on dg.SteamID = sg.SteamID
	LEFT JOIN MaxAirshotGames ag on ag.SteamID = sg.SteamID
GROUP BY sg.SteamID
HAVING NumberOfGames >= 20
ORDER BY AverageDPM DESC;

END //

DELIMITER ;