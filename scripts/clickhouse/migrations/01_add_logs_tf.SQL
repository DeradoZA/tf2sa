-- Create table to store LogListItem + raw JSON payload (shapes vary).
-- Includes dedup/idempotency (ReplacingMergeTree), monthly partitioning, TTL, and a few "hot" parsed fields.
-- Sources:
--   ReplacingMergeTree & dedup: https://clickhouse.com/docs/en/engines/table-engines/mergetree-family/replacingmergetree/
--   PARTITION BY / ORDER BY / toYYYYMM: https://clickhouse.com/docs/en/engines/table-engines/mergetree-family/mergetree/#partition-by
--   Date/time functions: https://clickhouse.com/docs/en/sql-reference/functions/date-time-functions/
--   JSON functions: https://clickhouse.com/docs/en/sql-reference/functions/json-functions/
--   TTL: https://clickhouse.com/docs/en/operations/ttl/
-- Notes:
--   - raw_game_log kept as String because JSON shapes vary; use JSONExtract* for typed extraction.
--   - DEFAULT JSONExtract* columns are computed at insert; to populate existing rows you'll need to backfill or rewrite rows.

CREATE TABLE IF NOT EXISTS default.logs_tf
(
    -- fields from LogListItem
    id UInt32,                                  -- LogListItem.Id
    title String,                               -- LogListItem.Title
    map String,                                 -- LogListItem.Map
    date_raw Int32,                             -- original epoch seconds (LogListItem.Date)
    date DateTime MATERIALIZED toDateTime(date_raw), -- human-friendly materialized column
    views Int32,                                -- LogListItem.Views
    players Nullable(Int32),                    -- LogListItem.Players (nullable)

    -- raw JSON payload (shapes vary but always JSON)
    raw_game_log String,

    -- example hot parsed fields (add/remove based on actual query needs)
    -- duration Nullable(Int32) DEFAULT JSONExtractInt(raw_game_log, 'duration'),
    -- server Nullable(String) DEFAULT JSONExtractString(raw_game_log, 'server'),

    -- ingestion metadata for idempotency and ordering
    ingested_at DateTime DEFAULT now(),
    ingestion_ts UInt64 DEFAULT CAST(toUnixTimestamp(ingested_at) AS UInt64)
)
ENGINE = ReplacingMergeTree(ingestion_ts)
PARTITION BY toYYYYMM(date)
ORDER BY (date, id)
TTL date + INTERVAL 1 YEAR
SETTINGS index_granularity = 8192;