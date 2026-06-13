-- ClickHouse schema for TF2SA analytics

-- Fact: one row per player per game
CREATE TABLE IF NOT EXISTS tf2sa.player_game_stats
(
    game_id UInt32,
    game_date Date,
    steam_id UInt64,
    player_name LowCardinality(String),
    map LowCardinality(String),
    team UInt8,
    class_id UInt8,
    duration UInt16,
    dpm Float32,
    kills UInt16,
    assists UInt16,
    deaths UInt16,
    airshots UInt16,
    headshots UInt16,
    backstabs UInt16,
    damage UInt32,
    heals UInt32,
    medkits UInt32,
    capture_points UInt16,
    sentries_built UInt16,
    is_valid UInt8
)
ENGINE = MergeTree()
PARTITION BY toYYYYMM(game_date)
ORDER BY (steam_id, game_date, game_id)
SETTINGS index_granularity = 8192;

-- Fact: one row per player per game per weapon
CREATE TABLE IF NOT EXISTS tf2sa.weapon_stats
(
    game_id UInt32,
    game_date Date,
    steam_id UInt64,
    weapon LowCardinality(String),
    damage UInt32,
    kills UInt16,
    hits UInt32,
    shots UInt32
)
ENGINE = MergeTree()
PARTITION BY toYYYYMM(game_date)
ORDER BY (weapon, game_date, game_id)
SETTINGS index_granularity = 8192;

-- Example aggregated daily player rollup using AggregatingMergeTree
CREATE TABLE IF NOT EXISTS tf2sa.daily_player_rollup
(
    steam_id UInt64,
    day Date,
    games AggregateFunction(count, UInt32),
    avg_dpm AggregateFunction(avg, Float32),
    total_kills AggregateFunction(sum, UInt64),
    total_deaths AggregateFunction(sum, UInt64),
    total_heals AggregateFunction(sum, UInt64)
)
ENGINE = AggregatingMergeTree()
PARTITION BY toYYYYMM(day)
ORDER BY (steam_id, day)
SETTINGS index_granularity = 8192;

-- Materialized view to populate daily_player_rollup from player_game_stats
CREATE MATERIALIZED VIEW IF NOT EXISTS tf2sa.mv_player_daily
TO tf2sa.daily_player_rollup
AS
SELECT
    steam_id,
    toDate(game_date) AS day,
    countState() AS games,
    avgState(dpm) AS avg_dpm,
    sumState(kills) AS total_kills,
    sumState(deaths) AS total_deaths,
    sumState(heals) AS total_heals
FROM tf2sa.player_game_stats
GROUP BY steam_id, toDate(game_date);
