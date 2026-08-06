ClickHouse integration for TF2SA

This directory contains helper scripts to spin up ClickHouse, load the analytics schema, and run a simple batch ETL from the existing MariaDB instance.

Files:
- schema/player_game_stats.sql  - ClickHouse DDL for player_game_stats, weapon_stats, and an example daily rollup materialized view
- init_schema.sh               - helper to start ClickHouse and load the schema using docker-compose
- etl_batch.py                 - example Python batch ETL that pulls new rows from MariaDB and inserts into ClickHouse
- query_examples.sh            - sample queries you can run against ClickHouse

Quickstart
1) Start ClickHouse with docker-compose:
   docker-compose up -d tf2sa-clickhouse

2) Load the schema:
   ./scripts/clickhouse/init_schema.sh

3) Configure ETL environment variables (example):
   export MYSQL_HOST=tf2sa-db
   export MYSQL_USER=${TF2SA_MYSQL_USR}
   export MYSQL_PASSWORD=${TF2SA_MYSQL_PWD}
   export MYSQL_DB=${TF2SA_MYSQL_DB}
   export CLICKHOUSE_HOST=localhost

4) Run the batch ETL (from host; it will connect to services in docker-compose network if hostnames resolve):
   python3 scripts/clickhouse/etl_batch.py

Notes & caveats
- The ETL script is an example and intentionally simple. For production consider a CDC / streaming approach (Debezium -> Kafka -> ClickHouse) or at least a robust incremental loader that handles retries, schema changes and batching.
- The chosen ClickHouse image is pinned to a 23.12.x release in docker-compose. Adjust the version to match your requirements.
- The schema uses LowCardinality(String) for textual dimensions to reduce memory and index costs.
