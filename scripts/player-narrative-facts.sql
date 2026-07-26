-- One-off authoring tooling for hand-written player descriptions (not embedded in
-- the app — the running app only ever serves the persisted Players.Description text,
-- ADR 0009's Phase 4). Run manually via the DuckDB CLI to gather structured facts
-- about a player, which a human (or Claude, acting as one) reads and turns into 3-4
-- lines of prose, persisted via a plain UPDATE Players SET Description = ... script.
--
-- Usage (against a copy of the .duckdb file, never the one the running backend holds
-- open):
--   duckdb copy.duckdb < TF2SA.Data/Analytics/Leaderboards.sql
--   duckdb copy.duckdb < TF2SA.Data/Analytics/PlayerDetail.sql
--   duckdb copy.duckdb -c "
--     CREATE TABLE _cmp_scout   AS SELECT * FROM class_board(1,5,0);
--     CREATE TABLE _cmp_soldier AS SELECT * FROM class_board(2,5,0);
--     CREATE TABLE _cmp_demoman AS SELECT * FROM class_board(4,5,0);
--     CREATE TABLE _cmp_medic   AS SELECT * FROM medic_board(5,0);
--   "
--   duckdb copy.duckdb < scripts/player-narrative-facts.sql
--   duckdb copy.duckdb -c "SELECT * FROM player_narrative_overview(<steamid>);
--                           SELECT * FROM player_narrative_classes(<steamid>);"
--
-- Requires _cmp_scout/_cmp_soldier/_cmp_demoman/_cmp_medic static comparison tables
-- (one-time materialization of class_board(cid,5,0)/medic_board(5,0) — calling those
-- board macros more than once inside a single query/macro body hits a DuckDB binder
-- error, "Conflicting column names", so the population is snapshotted once instead).

CREATE OR REPLACE MACRO player_narrative_overview(steamid) AS TABLE
SELECT p.SteamID, p.PlayerName,
  COUNT(DISTINCT ps.GameID)::BIGINT TotalGames,
  MIN(g.Date) FirstGameDate,
  MAX(g.Date) LastGameDate,
  SUM(CASE WHEN g.Date > epoch(now())::BIGINT - 180*24*60*60 THEN 1 ELSE 0 END)::BIGINT GamesLast180d,
  SUM(CASE WHEN g.Date > epoch(now())::BIGINT - 365*24*60*60 THEN 1 ELSE 0 END)::BIGINT GamesLast365d
FROM Players p
JOIN PlayerStats ps ON ps.SteamID = p.SteamID
JOIN Games g ON g.GameID = ps.GameID
WHERE p.SteamID = steamid
GROUP BY p.SteamID, p.PlayerName;

CREATE OR REPLACE MACRO player_narrative_classes(steamid) AS TABLE
WITH totals AS (
  SELECT COUNT(DISTINCT ps.GameID) AS TotalGames
  FROM PlayerStats ps WHERE ps.SteamID = steamid
),
mine AS (
  SELECT cs.ClassID,
    COUNT(*)::BIGINT NumberOfGames,
    ROUND(AVG(cs.Damage::DOUBLE/NULLIF(cs.Playtime,0)*60),1) AverageDpm,
    ROUND(AVG(cs.Kills),1) AverageKills,
    ROUND(AVG(cs.Deaths),1) AverageDeaths,
    ROUND(AVG(ps.Airshots),1) AverageAirshots,
    ROUND(AVG(ps.Ubers),1) AverageUbers,
    ROUND(AVG(ps.Heals::DOUBLE/NULLIF(cs.Playtime,0)*60),1) AverageHealsPm,
    ROUND(AVG(ps.Drops),1) AverageDrops
  FROM ClassStats cs JOIN PlayerStats ps ON ps.PlayerStatsID=cs.PlayerStatsID
  WHERE ps.SteamID = steamid
  GROUP BY cs.ClassID
)
SELECT mine.ClassID, mine.NumberOfGames,
  ROUND(mine.NumberOfGames*100.0/totals.TotalGames,1) SharePercent,
  mine.AverageDpm, mine.AverageKills, mine.AverageDeaths,
  mine.AverageAirshots, mine.AverageUbers, mine.AverageHealsPm, mine.AverageDrops,
  CASE mine.ClassID
    WHEN 1 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageDpm < mine.AverageDpm THEN 1 ELSE 0 END)*100,0) FROM _cmp_scout c)
    WHEN 2 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageDpm < mine.AverageDpm THEN 1 ELSE 0 END)*100,0) FROM _cmp_soldier c)
    WHEN 4 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageDpm < mine.AverageDpm THEN 1 ELSE 0 END)*100,0) FROM _cmp_demoman c)
    WHEN 7 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageHealsPm < mine.AverageHealsPm THEN 1 ELSE 0 END)*100,0) FROM _cmp_medic c)
    ELSE NULL
  END DpmOrHealsPercentile,
  CASE mine.ClassID
    WHEN 2 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageAirshots < mine.AverageAirshots THEN 1 ELSE 0 END)*100,0) FROM _cmp_soldier c WHERE c.AverageAirshots IS NOT NULL)
    WHEN 4 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageAirshots < mine.AverageAirshots THEN 1 ELSE 0 END)*100,0) FROM _cmp_demoman c WHERE c.AverageAirshots IS NOT NULL)
    ELSE NULL
  END AirshotsPercentile,
  CASE mine.ClassID
    WHEN 7 THEN (SELECT ROUND(AVG(CASE WHEN c.AverageUbers < mine.AverageUbers THEN 1 ELSE 0 END)*100,0) FROM _cmp_medic c)
    ELSE NULL
  END UbersPercentile
FROM mine, totals
ORDER BY mine.NumberOfGames DESC;
