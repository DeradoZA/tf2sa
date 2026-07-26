-- DuckDB per-player analytical views (ADR 0008) — the player-detail, progression and
-- multi-player comparison views that docs/architecture/analytics-store.md lists under
-- "Views this unlocks". Embedded into TF2SA.Data and loaded by DuckDbAnalyticsStore
-- alongside Leaderboards.sql; runs against the same raw tables it copies from MariaDB.
--
-- Unlike the leaderboard macros, these are NOT constrained by stored-proc parity, so
-- they use real AVG(Assists) rather than replicating the procs' AVG(Kills) copy-paste
-- bug. Otherwise the definitions deliberately match the boards so a player's numbers
-- agree wherever they appear:
--   * overall grain  -> overall_board  (per-game sums over non-medic classes, averaged)
--   * per-class grain-> class_board    (that class's damage/playtime, per class row)
--   * airshots       -> counted only for games with a projectile class (ClassID IN (2,4)),
--                       as overall_board does; PlayerStats.Airshots is per game, not
--                       per class, and is NULL for players who never played 2/4.
--
-- Idioms carried over from Leaderboards.sql: win/loss derived from TeamID + scores,
-- ::DOUBLE casts because DuckDB integer-divides where MariaDB float-divides, and the
-- 1/2/4/7/8/9 class ids (medic=7 excluded from the overall DPM).
--
-- Bucketing note: `AT TIME ZONE 'UTC'` pins bucket boundaries to UTC and yields a
-- naive TIMESTAMP; without it to_timestamp returns TIMESTAMPTZ and buckets would
-- shift with the server's local timezone. The bucket parameter is named `bucketpart`
-- rather than `bucket` because macro parameters shadow column aliases
-- case-insensitively — a parameter named `bucket` breaks `ORDER BY Bucket`.
--
-- The progression macros take a LIST of steam ids (verified: DuckDB table macros accept
-- list parameters) so one macro serves both the single-player page and the comparison
-- page, which charts up to six players.

-- ---- player summary (one row): player_summary(steamid) ----
CREATE OR REPLACE MACRO player_summary(steamid) AS TABLE
WITH ag AS (
  SELECT ps.PlayerStatsID, g.GameID, p.SteamID, p.PlayerName, p.Avatar, ps.Airshots,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM PlayerStats ps
  JOIN Players p ON p.SteamID=ps.SteamID
  JOIN Games g ON g.GameID=ps.GameID
  WHERE ps.SteamID = steamid
),
dg AS (  -- per player-game, non-medic classes summed (matches overall_board)
  SELECT ag.PlayerStatsID,
         SUM(cs.Damage)::DOUBLE / NULLIF(SUM(cs.Playtime),0) * 60 AS DPM,
         SUM(cs.Kills) Kills, SUM(cs.Deaths) Deaths, SUM(cs.Assists) Assists
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID<>7
  GROUP BY ag.PlayerStatsID
),
pg AS (  -- airshots only from games with a projectile class (matches overall_board)
  SELECT DISTINCT ag.PlayerStatsID, ag.Airshots
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID IN (2,4)
)
-- The averages here are the player's career figures, and the progression chart draws
-- them as its reference line. They must come from this macro rather than be re-derived
-- from the progression buckets: weighting bucket averages by NumberOfGames does NOT
-- reproduce them (a bucket's NumberOfGames counts medic-only games whose DPM is NULL
-- and thus absent from that bucket's average, and each bucket is rounded to 1dp).
SELECT any_value(ag.SteamID) SteamID,
  any_value(ag.PlayerName) PlayerName, any_value(ag.Avatar) Avatar,
  COUNT(ag.GameID)::BIGINT NumberOfGames,
  SUM(ag.Win)::BIGINT Wins, SUM(ag.Draw)::BIGINT Draws, SUM(ag.Loss)::BIGINT Losses,
  ROUND(SUM(ag.Win)*100.0/NULLIF(COUNT(ag.GameID),0),1) WinPercentage,
  ROUND(AVG(dg.DPM),1) AverageDPM,
  ROUND(AVG(dg.Kills),1) AverageKills,
  ROUND(AVG(dg.Deaths),1) AverageDeaths,
  ROUND(AVG(dg.Assists),1) AverageAssists,
  ROUND(AVG(pg.Airshots),1) AverageAirshots
FROM ag
LEFT JOIN dg ON dg.PlayerStatsID=ag.PlayerStatsID
LEFT JOIN pg ON pg.PlayerStatsID=ag.PlayerStatsID
-- Without this an unknown SteamID still yields one all-NULL row (a bare aggregate
-- always returns a row), which the caller can't distinguish from a real player and
-- which would fail to map NULL onto the non-nullable SteamID. Zero rows = not found.
HAVING COUNT(ag.GameID) > 0;

-- ---- overall progression: players_progression_overall(steamids, bucketpart, recentonly) ----
-- One row per (player, bucket). Every stat is returned per bucket so the frontend's stat
-- selector needs no refetch and never reaches SQL. Cross-class grain, matching
-- overall_board: a game contributes one summed non-medic figure. recentonly=1 applies
-- the same 365-day window as class_board/overall_board's own Recent boards.
CREATE OR REPLACE MACRO players_progression_overall(steamids, bucketpart, recentonly) AS TABLE
WITH ag AS (
  -- PlayerName/Avatar are joined here (not looked up separately) so the comparison
  -- endpoint gets everything it needs to label a legend from one macro call, rather
  -- than one call per selected player.
  SELECT date_trunc(bucketpart, to_timestamp(g.Date) AT TIME ZONE 'UTC') AS Bucket,
         ps.PlayerStatsID, ps.SteamID, p.PlayerName, p.Avatar, g.GameID, ps.Airshots,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM PlayerStats ps
  JOIN Players p ON p.SteamID=ps.SteamID
  JOIN Games g ON g.GameID=ps.GameID
  WHERE list_contains(steamids, ps.SteamID)
    AND (recentonly=0 OR g.Date > epoch(now())::BIGINT - 365*24*60*60)
),
dg AS (
  SELECT ag.PlayerStatsID,
         SUM(cs.Damage)::DOUBLE / NULLIF(SUM(cs.Playtime),0) * 60 AS DPM,
         SUM(cs.Kills) Kills, SUM(cs.Deaths) Deaths, SUM(cs.Assists) Assists
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID<>7
  GROUP BY ag.PlayerStatsID
),
pg AS (
  SELECT DISTINCT ag.PlayerStatsID, ag.Airshots
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID IN (2,4)
)
SELECT ag.SteamID, any_value(ag.PlayerName) PlayerName, any_value(ag.Avatar) Avatar,
  ag.Bucket,
  COUNT(ag.GameID)::BIGINT NumberOfGames,
  SUM(ag.Win)::BIGINT Wins, SUM(ag.Draw)::BIGINT Draws, SUM(ag.Loss)::BIGINT Losses,
  ROUND(SUM(ag.Win)*100.0/NULLIF(COUNT(ag.GameID),0),1) WinPercentage,
  ROUND(AVG(dg.DPM),1) AverageDPM,
  ROUND(AVG(dg.Kills),1) AverageKills,
  ROUND(AVG(dg.Deaths),1) AverageDeaths,
  ROUND(AVG(dg.Assists),1) AverageAssists,
  ROUND(AVG(pg.Airshots),1) AverageAirshots
FROM ag
LEFT JOIN dg ON dg.PlayerStatsID=ag.PlayerStatsID
LEFT JOIN pg ON pg.PlayerStatsID=ag.PlayerStatsID
GROUP BY ag.SteamID, ag.Bucket
ORDER BY ag.SteamID, ag.Bucket;

-- ---- per-class progression: players_progression_class(steamids, bucketpart, classid, recentonly) ----
-- Per-class grain, matching class_board: one row per ClassStats row, DPM from that
-- class's own damage/playtime, and games counted only where that class was played. Same
-- column set as the overall macro so one row type maps both. recentonly=1 applies the
-- same 365-day window as class_board's own Recent boards.
CREATE OR REPLACE MACRO players_progression_class(steamids, bucketpart, classid, recentonly) AS TABLE
WITH cg AS (
  SELECT date_trunc(bucketpart, to_timestamp(g.Date) AT TIME ZONE 'UTC') AS Bucket,
         ps.SteamID, p.PlayerName, p.Avatar,
         cs.Kills, cs.Deaths, cs.Assists, cs.Damage, cs.Playtime,
         ps.Airshots,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win,
         CASE WHEN (ps.TeamID=0 AND g.RedScore<g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore<g.RedScore) THEN 1 ELSE 0 END AS Loss,
         CASE WHEN g.RedScore=g.BlueScore THEN 1 ELSE 0 END AS Draw
  FROM ClassStats cs
  JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
  JOIN Players p ON p.SteamID=ps.SteamID
  JOIN Games g ON g.GameID=ps.GameID
  WHERE cs.ClassID = classid AND list_contains(steamids, ps.SteamID)
    AND (recentonly=0 OR g.Date > epoch(now())::BIGINT - 365*24*60*60)
)
SELECT cg.SteamID, any_value(cg.PlayerName) PlayerName, any_value(cg.Avatar) Avatar,
  cg.Bucket,
  COUNT(*)::BIGINT NumberOfGames,
  SUM(cg.Win)::BIGINT Wins, SUM(cg.Draw)::BIGINT Draws, SUM(cg.Loss)::BIGINT Losses,
  ROUND(SUM(cg.Win)*100.0/NULLIF(COUNT(*),0),1) WinPercentage,
  ROUND(AVG(cg.Damage::DOUBLE/NULLIF(cg.Playtime,0)*60),1) AverageDPM,
  ROUND(AVG(cg.Kills),1) AverageKills,
  ROUND(AVG(cg.Deaths),1) AverageDeaths,
  ROUND(AVG(cg.Assists),1) AverageAssists,
  -- Airshots is a per-GAME PlayerStats field, not per-class: a player who played two
  -- classes in one match would have their other class's airshots bleed into this row.
  -- Only meaningful for the two projectile classes (matches player_class_splits, which
  -- omits Airshots entirely for this reason); NULL elsewhere rather than a misleading
  -- number for e.g. Scout or Medic.
  CASE
    WHEN classid IN (2, 4) THEN ROUND(AVG(cg.Airshots), 1)
    ELSE NULL
  END AverageAirshots
FROM cg
GROUP BY cg.SteamID, cg.Bucket
ORDER BY cg.SteamID, cg.Bucket;

-- ---- per-class splits: player_class_splits(steamid) ----
-- Per-class grain, so DPM here is per-class (cs.Damage/cs.Playtime) rather than the
-- cross-class per-game DPM used above; medic is included (a medic row is meaningful
-- on a player's own page even though medic is excluded from overall DPM).
CREATE OR REPLACE MACRO player_class_splits(steamid) AS TABLE
SELECT cs.ClassID,
  COUNT(*)::BIGINT NumberOfGames,
  SUM(CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END)::BIGINT Wins,
  ROUND(AVG(cs.Damage::DOUBLE/NULLIF(cs.Playtime,0)*60),1) AverageDPM,
  ROUND(AVG(cs.Kills),1) AverageKills,
  ROUND(AVG(cs.Assists),1) AverageAssists,
  ROUND(AVG(cs.Deaths),1) AverageDeaths
FROM ClassStats cs
JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
JOIN Games g ON g.GameID=ps.GameID
WHERE ps.SteamID = steamid
GROUP BY cs.ClassID
ORDER BY NumberOfGames DESC;
