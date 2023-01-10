DROP PROCEDURE IF EXISTS ScoutStatCalculator;

DELIMITER $$
$$
CREATE PROCEDURE ScoutStatCalculator(OUT ResultSet VARCHAR(5000))
BEGIN
    
    SELECT
        PlayerStats.SteamID,Players.PlayerName, COUNT(GameID) as NumberOfGames, AVG(Damage / (Playtime / 60)) as AverageDPM,
        AVG(Kills) AS AverageKills, AVG(Assists) AS AverageAssists, AVG(Deaths) AS AverageDeaths, MAX(Kills) as MaxKills, MAX(Damage) as MaxDamage
        FROM Players 
            INNER JOIN PlayerStats ON Players.SteamID = PlayerStats.SteamID
            INNER JOIN ClassStats on PlayerStats.PlayerStatsID = ClassStats.PlayerStatsID 
                GROUP BY PlayerStats.SteamID
                    ORDER BY AverageKills

END$$
