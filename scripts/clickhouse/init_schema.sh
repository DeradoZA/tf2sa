#!/usr/bin/env bash
set -euo pipefail

# Init ClickHouse schema helper
# Usage: ./init_schema.sh [path/to/schema.sql]

SCHEMA_PATH="scripts/clickhouse/schema/player_game_stats.sql"
COMPOSE_CMD="docker-compose"

if [ ! -f "$SCHEMA_PATH" ]; then
  echo "Schema file not found: $SCHEMA_PATH"
  exit 1
fi

echo "Starting clickhouse container (if not already running)..."
$COMPOSE_CMD up -d tf2sa-clickhouse

# Wait for ClickHouse to be ready
echo "Waiting for ClickHouse HTTP interface to be ready..."
until curl -sSf http://localhost:8123/ >/dev/null 2>&1; do
  echo "Waiting..."
  sleep 1
done

# Execute schema SQL using clickhouse-client container
echo "Loading schema into ClickHouse..."
cat "$SCHEMA_PATH" | $COMPOSE_CMD exec -T tf2sa-clickhouse clickhouse-client --multiquery

echo "Schema loaded."
