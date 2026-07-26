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
  SELECT ps.PlayerStatsID, g.GameID, p.SteamID, p.PlayerName, p.Avatar, p.Description, ps.Airshots,
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
  any_value(ag.Description) Description,
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

-- ---- per-map splits: player_map_splits(steamid) ----
-- Overall grain (non-medic classes summed per game), matching player_summary's `dg`
-- CTE, so these DPM/Kills/Deaths figures mean the same thing as everywhere else on
-- the player page — just grouped by map instead of by bucket/class.
CREATE OR REPLACE MACRO player_map_splits(steamid) AS TABLE
WITH ag AS (
  SELECT ps.PlayerStatsID, g.GameID, g.Map,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win
  FROM PlayerStats ps JOIN Games g ON g.GameID=ps.GameID
  WHERE ps.SteamID = steamid
),
dg AS (  -- per player-game, non-medic classes summed (matches overall_board/player_summary)
  SELECT ag.PlayerStatsID,
         SUM(cs.Damage)::DOUBLE / NULLIF(SUM(cs.Playtime),0) * 60 AS DPM,
         SUM(cs.Kills) Kills, SUM(cs.Deaths) Deaths
  FROM ag JOIN ClassStats cs ON cs.PlayerStatsID=ag.PlayerStatsID
  WHERE cs.ClassID<>7
  GROUP BY ag.PlayerStatsID
)
SELECT ag.Map,
  COUNT(ag.GameID)::BIGINT NumberOfGames,
  SUM(ag.Win)::BIGINT Wins,
  ROUND(SUM(ag.Win)*100.0/COUNT(ag.GameID),1) WinPercentage,
  ROUND(AVG(dg.DPM),1) AverageDPM,
  ROUND(AVG(dg.Kills),1) AverageKills,
  ROUND(AVG(dg.Deaths),1) AverageDeaths
FROM ag
LEFT JOIN dg ON dg.PlayerStatsID=ag.PlayerStatsID
GROUP BY ag.Map
ORDER BY NumberOfGames DESC;

-- ---- per-weapon splits: player_weapon_splits(steamid) ----
-- Accuracy is NULL (not 0%) when every game that used this weapon lacked HasAccuracy
-- (ApplyCapabilityFlagNulling nulls Shots/Hits in that case) — same null-propagation
-- idiom used throughout these macros rather than a fabricated number.
CREATE OR REPLACE MACRO player_weapon_splits(steamid) AS TABLE
SELECT ws.WeaponName,
  COUNT(*)::BIGINT NumberOfGames,
  SUM(ws.Kills)::BIGINT TotalKills,
  SUM(ws.Damage)::BIGINT TotalDamage,
  ROUND(SUM(ws.Hits)*100.0/NULLIF(SUM(ws.Shots),0),1) Accuracy
FROM WeaponStats ws
JOIN ClassStats cs ON cs.ClassStatsID=ws.ClassStatsID
JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
WHERE ps.SteamID = steamid
GROUP BY ws.WeaponName
ORDER BY TotalKills DESC;

-- ---- teammate/opponent synergy: player_teammate_synergy(steamid, mingames) /
--      player_opponent_synergy(steamid, mingames) ----
-- Self-join PlayerStats on GameID: teammate requires the same TeamID, opponent the
-- opposite one. A join predicate can't be a macro parameter, so these are two
-- near-identical macros rather than one flagged macro. WinPercentage is always the
-- *querying* player's win rate in the shared games (their own result, not the other
-- player's). mingames filters one-off pairings; applied in an outer WHERE per the
-- macro-parameter-in-HAVING restriction noted in Leaderboards.sql's header.
CREATE OR REPLACE MACRO player_teammate_synergy(steamid, mingames) AS TABLE
WITH me AS (
  SELECT ps.GameID, ps.TeamID,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win
  FROM PlayerStats ps JOIN Games g ON g.GameID=ps.GameID
  WHERE ps.SteamID = steamid
),
other AS (
  SELECT me.Win, ops.SteamID, p.PlayerName, p.Avatar
  FROM me
  JOIN PlayerStats ops ON ops.GameID=me.GameID AND ops.TeamID=me.TeamID AND ops.SteamID<>steamid
  JOIN Players p ON p.SteamID=ops.SteamID
)
SELECT * FROM (
  SELECT other.SteamID,
    any_value(other.PlayerName) PlayerName, any_value(other.Avatar) Avatar,
    COUNT(*)::BIGINT NumberOfGames, SUM(other.Win)::BIGINT Wins,
    ROUND(SUM(other.Win)*100.0/COUNT(*),1) WinPercentage
  FROM other
  GROUP BY other.SteamID
) WHERE NumberOfGames >= mingames;

-- ---- medic depth: medic_depth(steamid) ----
-- One row (or zero — same "no rows means no such thing" convention as player_summary)
-- summarizing uber usage depth from logs.tf's per-player medic_stats block, which
-- ingestion now captures (see PlayerStatWrite's medic fields). Restricted to games
-- where AvgUberLength is non-null, since logs.tf only computes these fields for some
-- medic games and a player's non-medic PlayerStats rows are always fully null here.
CREATE OR REPLACE MACRO medic_depth(steamid) AS TABLE
SELECT
  COUNT(*)::BIGINT NumberOfGames,
  ROUND(AVG(ps.AdvantagesLost),1) AverageAdvantagesLost,
  ROUND(AVG(ps.BiggestAdvantageLost),1) AverageBiggestAdvantageLost,
  ROUND(AVG(ps.AvgUberLength),1) AverageUberLength,
  ROUND(AVG(ps.AvgTimeToBuild),1) AverageTimeToBuild,
  ROUND(AVG(ps.AvgTimeBeforeUsing),1) AverageTimeBeforeUsing,
  ROUND(AVG(ps.DeathsWithin20sAfterUber),1) AverageDeathsWithin20sAfterUber
FROM PlayerStats ps
JOIN ClassStats cs ON cs.PlayerStatsID = ps.PlayerStatsID
WHERE ps.SteamID = steamid AND cs.ClassID = 7 AND ps.AvgUberLength IS NOT NULL
HAVING COUNT(*) > 0;

CREATE OR REPLACE MACRO player_opponent_synergy(steamid, mingames) AS TABLE
WITH me AS (
  SELECT ps.GameID, ps.TeamID,
         CASE WHEN (ps.TeamID=0 AND g.RedScore>g.BlueScore) OR (ps.TeamID=1 AND g.BlueScore>g.RedScore) THEN 1 ELSE 0 END AS Win
  FROM PlayerStats ps JOIN Games g ON g.GameID=ps.GameID
  WHERE ps.SteamID = steamid
),
other AS (
  SELECT me.Win, ops.SteamID, p.PlayerName, p.Avatar
  FROM me
  JOIN PlayerStats ops ON ops.GameID=me.GameID AND ops.TeamID<>me.TeamID AND ops.SteamID<>steamid
  JOIN Players p ON p.SteamID=ops.SteamID
)
SELECT * FROM (
  SELECT other.SteamID,
    any_value(other.PlayerName) PlayerName, any_value(other.Avatar) Avatar,
    COUNT(*)::BIGINT NumberOfGames, SUM(other.Win)::BIGINT Wins,
    ROUND(SUM(other.Win)*100.0/COUNT(*),1) WinPercentage
  FROM other
  GROUP BY other.SteamID
) WHERE NumberOfGames >= mingames;
