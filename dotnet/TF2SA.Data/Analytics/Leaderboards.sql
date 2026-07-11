-- DuckDB leaderboard aggregations — parity-verified equivalents of the MariaDB
-- stored procs in scripts/mariadb/update_*_stats.sql (see ADR 0008 and
-- docs/architecture/analytics-store.md). Embedded into TF2SA.Data and loaded by
-- DuckDbAnalyticsStore; the macros run against the raw tables it copies from MariaDB.
--
-- These three table macros reproduce all 10 stored-proc outputs EXACTLY
-- (row-for-row, value-for-value) on identical raw data — verified by the Phase 1
-- dual-run harness. The read path (DuckDbAnalyticsStore.GetPageAsync) wraps a board
-- with the whitelisted sort + name filter + LIMIT/OFFSET; columns and types are
-- aligned to the EF entities (e.g. SteamID, AverageDPM, Wins as BIGINT).
--
-- PARITY NOTE — replicated bug: the per-class and medic boards compute
-- `AverageAssists = AVG(Kills)` because the stored procs do (a copy-paste bug).
-- Kept for equivalence; fix at Phase 3 (change AVG(Kills) -> AVG(Assists) here).
-- The Overall board already uses real assists.
--
-- Class ids: scout=1, soldier=2, demoman=4, medic=7, sniper=8, spy=9,
-- projectile (soldier/demoman)=2,4. Thresholds: class/medic recent >=5, alltime
-- >=20; overall recent >10, alltime >20. Recent window = last 365 days.
-- MariaDB `/` is float division; DuckDB integer-divides, hence the ::DOUBLE casts.
-- The min-games threshold is applied in an outer WHERE (DuckDB rejects a macro
-- parameter in HAVING).

-- ---- per-class board (scout/soldier/demoman): class_board(classId, minGames, recentOnly) ----
CREATE OR REPLACE MACRO class_board(cid, mingames, recentonly) AS TABLE
WITH cg AS (
  SELECT cs.ClassStatsID, cs.Kills, cs.Deaths, cs.Damage, cs.Playtime,
         ps.GameID, ps.SteamID, ps.DamageTaken, ps.HealsReceived, ps.Airshots,
         ps.MedkitsHP, ps.CapturePointsCaptured, g.Date, p.PlayerName, p.Avatar,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM ClassStats cs
  JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
  JOIN Players p ON p.SteamID=ps.SteamID
  JOIN Games g ON g.GameID=ps.GameID
  WHERE cs.ClassID=cid
    AND (recentonly=0 OR g.Date > epoch(now())::BIGINT - 365*24*60*60)
),
tk AS (SELECT SteamID, GameID TopKillsGameID, Kills TopKills FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Kills DESC, Damage DESC, Date DESC) rn FROM cg) WHERE rn=1),
td AS (SELECT SteamID, GameID TopDamageGameID, Damage TopDamage FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Damage DESC, Kills DESC, Date DESC) rn FROM cg) WHERE rn=1),
ta AS (SELECT SteamID, GameID TopAirshotsGameID, Airshots TopAirshots FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Airshots DESC, Kills DESC, Date DESC) rn FROM cg) WHERE rn=1)
SELECT * FROM (
  SELECT cg.SteamID,
    any_value(cg.PlayerName) PlayerName, any_value(cg.Avatar) Avatar,
    COUNT(*)::BIGINT NumberOfGames, SUM(Win)::BIGINT Wins, ROUND(SUM(Win)*100.0/COUNT(*),1) WinPercentage,
    SUM(Draw)::BIGINT Draws, SUM(Loss)::BIGINT Losses,
    ROUND(AVG(Damage::DOUBLE/Playtime*60),1) AverageDPM,
    ROUND(AVG(Kills),1) AverageKills,
    ROUND(AVG(Kills),1) AverageAssists,          -- replicated proc bug (see header)
    ROUND(AVG(Deaths),1) AverageDeaths,
    ROUND(AVG(Airshots),1) AverageAirshots,      -- used by soldier/demoman only
    ROUND(AVG(DamageTaken::DOUBLE/Playtime*60),1) AverageDamageTakenPM,
    ROUND(AVG(HealsReceived::DOUBLE/Playtime*60),1) AverageHealsReceivedPM,
    ROUND(AVG(MedkitsHP),1) AverageMedKitsHP,
    ROUND(AVG(CapturePointsCaptured),1) AverageCapturePointsCaptured,
    any_value(tk.TopKills) TopKills, any_value(tk.TopKillsGameID) TopKillsGameID,
    any_value(td.TopDamage) TopDamage, any_value(td.TopDamageGameID) TopDamageGameID,
    any_value(ta.TopAirshots) TopAirshots, any_value(ta.TopAirshotsGameID) TopAirshotsGameID
  FROM cg
  LEFT JOIN tk ON tk.SteamID=cg.SteamID
  LEFT JOIN td ON td.SteamID=cg.SteamID
  LEFT JOIN ta ON ta.SteamID=cg.SteamID
  GROUP BY cg.SteamID
) WHERE NumberOfGames >= mingames;

-- ---- medic board (ClassID 7): medic_board(minGames, recentOnly) ----
CREATE OR REPLACE MACRO medic_board(mingames, recentonly) AS TABLE
WITH cg AS (
  SELECT cs.ClassStatsID, cs.Kills, cs.Deaths, cs.Playtime,
         ps.GameID, ps.SteamID, ps.Ubers, ps.Drops, ps.Heals, g.Date, p.PlayerName, p.Avatar,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM ClassStats cs
  JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
  JOIN Players p ON p.SteamID=ps.SteamID
  JOIN Games g ON g.GameID=ps.GameID
  WHERE cs.ClassID=7 AND (recentonly=0 OR g.Date > epoch(now())::BIGINT - 365*24*60*60)
),
th AS (SELECT SteamID, GameID TopHealsGameID, Heals TopHeals FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Heals DESC, Date DESC) rn FROM cg) WHERE rn=1),
tu AS (SELECT SteamID, GameID TopUbersGameID, Ubers TopUbers FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Ubers DESC, Date DESC) rn FROM cg) WHERE rn=1),
tdr AS (SELECT SteamID, GameID TopDropsGameID, Drops TopDrops FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Drops DESC, Date DESC) rn FROM cg) WHERE rn=1)
SELECT * FROM (
  SELECT cg.SteamID,
    any_value(cg.PlayerName) PlayerName, any_value(cg.Avatar) Avatar,
    COUNT(*)::BIGINT NumberOfGames, SUM(Win)::BIGINT Wins, ROUND(SUM(Win)*100.0/COUNT(*),1) WinPercentage,
    SUM(Draw)::BIGINT Draws, SUM(Loss)::BIGINT Losses,
    ROUND(AVG(Kills),1) AverageKills,
    ROUND(AVG(Kills),1) AverageAssists,          -- replicated proc bug (see header)
    ROUND(AVG(Deaths),1) AverageDeaths,
    ROUND(AVG(Ubers),1) AverageUbers,
    ROUND(AVG(Drops),1) AverageDrops,
    ROUND(AVG(Heals::DOUBLE/Playtime*60),1) AverageHealsPM,
    any_value(th.TopHeals) TopHeals, any_value(th.TopHealsGameID) TopHealsGameID,
    any_value(tu.TopUbers) TopUbers, any_value(tu.TopUbersGameID) TopUbersGameID,
    any_value(tdr.TopDrops) TopDrops, any_value(tdr.TopDropsGameID) TopDropsGameID
  FROM cg
  LEFT JOIN th ON th.SteamID=cg.SteamID
  LEFT JOIN tu ON tu.SteamID=cg.SteamID
  LEFT JOIN tdr ON tdr.SteamID=cg.SteamID
  GROUP BY cg.SteamID
) WHERE NumberOfGames >= mingames;

-- ---- overall board (two-level, class carve-outs): overall_board(minGames, recentOnly) ----
CREATE OR REPLACE MACRO overall_board(mingames, recentonly) AS TABLE
WITH ag AS (
  SELECT g.GameID, g.Date, ps.PlayerStatsID, p.SteamID, p.PlayerName, p.Avatar,
         ps.Airshots, ps.DamageTaken, ps.HealsReceived, ps.CapturePointsCaptured,
         ps.MedkitsHP, ps.Headshots, ps.Backstabs,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM PlayerStats ps JOIN Players p ON p.SteamID=ps.SteamID JOIN Games g ON g.GameID=ps.GameID
  WHERE (recentonly=0 OR g.Date > epoch(now())::BIGINT - 365*24*60*60)
),
dg AS (  -- per player-game, non-medic classes summed
  SELECT ag.PlayerStatsID, ag.SteamID, ag.GameID, ag.Date, ag.DamageTaken, ag.HealsReceived, ag.Airshots,
         SUM(cs.Playtime) Playtime, SUM(cs.Damage) Damage, SUM(cs.Kills) Kills,
         SUM(cs.Deaths) Deaths, SUM(cs.Assists) Assists,
         SUM(cs.Damage)::DOUBLE/SUM(cs.Playtime)*60 DPM
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID<>7
  GROUP BY ag.PlayerStatsID, ag.SteamID, ag.GameID, ag.Date, ag.DamageTaken, ag.HealsReceived, ag.Airshots
),
pg  AS (SELECT DISTINCT ag.PlayerStatsID, ag.Airshots  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID WHERE cs.ClassID IN (2,4)),
sng AS (SELECT DISTINCT ag.PlayerStatsID, ag.Headshots FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID WHERE cs.ClassID=8),
spg AS (SELECT DISTINCT ag.PlayerStatsID, ag.Backstabs FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID WHERE cs.ClassID=9),
tk AS (SELECT SteamID, GameID TopKillsGameID, Kills TopKills FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Kills DESC, Damage DESC, Date DESC) rn FROM dg) WHERE rn=1),
td AS (SELECT SteamID, GameID TopDamageGameID, Damage TopDamage FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Damage DESC, Kills DESC, Date DESC) rn FROM dg) WHERE rn=1),
ta AS (SELECT SteamID, GameID TopAirshotsGameID, Airshots TopAirshots FROM
  (SELECT *, row_number() OVER (PARTITION BY SteamID ORDER BY Airshots DESC, Damage DESC, Date DESC) rn FROM dg) WHERE rn=1)
SELECT * FROM (
  SELECT ag.SteamID,
    any_value(ag.PlayerName) PlayerName, any_value(ag.Avatar) Avatar,
    COUNT(ag.GameID)::BIGINT NumberOfGames, SUM(ag.Win)::BIGINT Wins, ROUND(SUM(ag.Win)*100.0/COUNT(ag.GameID),1) WinPercentage,
    SUM(ag.Draw)::BIGINT Draws, SUM(ag.Loss)::BIGINT Losses,
    ROUND(AVG(dg.DPM),1) AverageDPM,
    ROUND(AVG(dg.Kills),1) AverageKills,
    ROUND(AVG(dg.Assists),1) AverageAssists,     -- overall uses real assists (no bug)
    ROUND(AVG(dg.Deaths),1) AverageDeaths,
    ROUND(AVG(pg.Airshots),1) AverageAirshots,
    ROUND(AVG(sng.Headshots),1) AverageHeadshots,
    ROUND(AVG(spg.Backstabs),1) AverageBackstabs,
    ROUND(AVG(ag.DamageTaken::DOUBLE/dg.Playtime*60),1) AverageDamageTakenPM,
    ROUND(AVG(ag.HealsReceived::DOUBLE/dg.Playtime*60),1) AverageHealsReceivedPM,
    ROUND(AVG(ag.MedkitsHP),1) AverageMedKitsHP,
    ROUND(AVG(ag.CapturePointsCaptured),1) AverageCapturePointsCaptured,
    any_value(tk.TopKills) TopKills, any_value(tk.TopKillsGameID) TopKillsGameID,
    any_value(td.TopDamage) TopDamage, any_value(td.TopDamageGameID) TopDamageGameID,
    any_value(ta.TopAirshots) TopAirshots, any_value(ta.TopAirshotsGameID) TopAirshotsGameID
  FROM ag
  LEFT JOIN dg  ON dg.PlayerStatsID=ag.PlayerStatsID
  LEFT JOIN pg  ON pg.PlayerStatsID=ag.PlayerStatsID
  LEFT JOIN sng ON sng.PlayerStatsID=ag.PlayerStatsID
  LEFT JOIN spg ON spg.PlayerStatsID=ag.PlayerStatsID
  LEFT JOIN tk  ON tk.SteamID=ag.SteamID
  LEFT JOIN td  ON td.SteamID=ag.SteamID
  LEFT JOIN ta  ON ta.SteamID=ag.SteamID
  GROUP BY ag.SteamID
) WHERE NumberOfGames > mingames;

-- Board invocations that reproduce each stored proc:
--   Scout   Recent/AllTime : class_board(1, 5, 1) / class_board(1, 20, 0)
--   Soldier Recent/AllTime : class_board(2, 5, 1) / class_board(2, 20, 0)
--   Demoman Recent/AllTime : class_board(4, 5, 1) / class_board(4, 20, 0)
--   Medic   Recent/AllTime : medic_board(5, 1)    / medic_board(20, 0)
--   Overall Recent/AllTime : overall_board(10, 1) / overall_board(20, 0)
