DROP PROCEDURE IF EXISTS UpdateMedicAllTimeStats;

DELIMITER //

CREATE PROCEDURE UpdateMedicAllTimeStats()
BEGIN

DELETE FROM MedicAllTime;
INSERT INTO MedicAllTime (
	SteamID,
	PlayerName,
	Avatar,
	NumberOfGames,
	Wins,
	Draws,
	Losses,
	AverageKills,
	AverageAssists,
	AverageDeaths,
	AverageUbers,
	AverageDrops,
	AverageHealsPM,
	TopHeals,
	TopHealsGameID,
	TopUbers,
	TopUbersGameID,
	TopDrops,
	TopDropsGameID
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
		cs.ClassID = 7 
),
MaxHealGames AS (
	SELECT
		SteamID,
		GameID AS MaxHealsGameID,
		Heals AS MaxHeals,
		rn
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Heals,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Heals
			FROM Games
			ORDER BY SteamID, Heals DESC, Date DESC
			LIMIT 9999999
		) AS SortedHeals
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedHeals
	WHERE rn <= 1
),
MaxUberGames AS (
	SELECT
		SteamID,
		PlayerName,
		GameID AS MaxUbersGameID,
		Ubers AS MaxUbers
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Ubers,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Ubers
			FROM Games
			ORDER BY SteamID, Ubers DESC, Date DESC
			LIMIT 9999999
		) AS SortedUbers
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedUbers
	WHERE rn <= 1
),
MaxDropGames AS (
	SELECT
		SteamID,
		PlayerName,
		GameID AS MaxDropsGameID,
		Drops AS MaxDrops
	FROM (
		SELECT
			SteamID,
			PlayerName,
			GameID,
			Drops,
			@rn := IF(@prev = SteamID, @rn + 1, 1) AS rn,
			@prev := SteamID
		FROM (
			SELECT
				SteamID,
				PlayerName,
				GameID,
				Date,
				Drops
			FROM Games
			ORDER BY SteamID, Drops DESC, Date DESC
			LIMIT 9999999
		) AS SortedDrops
		JOIN (select @prev:=NULL, @rn :=0) as vars
	) as GroupedDrops
	WHERE rn <= 1
)
SELECT 
	sg.SteamID AS SteamID,
	sg.PlayerName AS PlayerName,
	sg.Avatar AS Avatar,
	COUNT(sg.ClassStatsID) AS NumberOfGames,
	SUM(sg.Win) AS Wins,
	SUM(sg.Draw) AS Draws,
	SUM(sg.Loss) AS Losses,
 	ROUND(AVG(sg.Kills),1) AS AverageKills,
 	ROUND(AVG(sg.Kills),1) AS AverageAssists,
 	ROUND(AVG(sg.Deaths),1) AS AverageDeaths,
 	ROUND(AVG(sg.Ubers),1) AS AverageUbers,
 	ROUND(AVG(sg.Drops),1) AS AverageDrops,
 	ROUND(AVG(sg.Heals/sg.Playtime*60),1) AS AverageHealsPM,
	hg.MaxHeals AS TopHeals,
	hg.MaxHealsGameID AS TopHealsGameID,
	ug.MaxUbers AS TopUbers,
	ug.MaxUbersGameID AS TopUbersGameID,
	dg.MaxDrops AS TopDrops,
	dg.MaxDropsGameID AS TopDropsGameID
FROM
	Games sg
	LEFT JOIN MaxHealGames hg on hg.SteamID = sg.SteamID
	LEFT JOIN MaxUberGames ug on ug.SteamID = sg.SteamID
	LEFT JOIN MaxDropGames dg on dg.SteamID = sg.SteamID
GROUP BY sg.SteamID
HAVING NumberOfGames >= 20
ORDER BY AverageHealsPM DESC;

END //

DELIMITER ;