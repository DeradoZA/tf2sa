#!/usr/bin/env bash
# Example queries for ClickHouse (run from repo root)

# Run a simple TOP players by avg_dpm for last 30 days
docker-compose exec -T tf2sa-clickhouse clickhouse-client --query "SELECT steam_id, avgMerge(avg_dpm) as avg_dpm FROM tf2sa.daily_player_rollup WHERE day >= today() - 30 GROUP BY steam_id ORDER BY avg_dpm DESC LIMIT 20"

# Query raw player_game_stats top damage
docker-compose exec -T tf2sa-clickhouse clickhouse-client --query "SELECT steam_id, sum(damage) AS total_damage FROM tf2sa.player_game_stats WHERE game_date >= today() - 30 GROUP BY steam_id ORDER BY total_damage DESC LIMIT 20"

# Example HTTP query via curl (JSON output)
curl -sS 'http://localhost:8123/?query=SELECT%20steam_id,%20sum(damage)%20AS%20total_damage%20FROM%20tf2sa.player_game_stats%20WHERE%20game_date%20%3E%3D%20today()%20-%2030%20GROUP%20BY%20steam_id%20ORDER%20BY%20total_damage%20DESC%20LIMIT%2020' 
