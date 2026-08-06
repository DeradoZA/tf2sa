#!/usr/bin/env python3
"""
Simple batch ETL example: extract player stats from MariaDB and insert into ClickHouse.
This is a minimal example for demonstration. Configure via environment variables.

Requirements:
  pip install clickhouse-driver mysql-connector-python python-dotenv

Usage:
  export MYSQL_HOST=tf2sa-db
  export MYSQL_USER=root
  export MYSQL_PASSWORD=passwd
  export MYSQL_DB=tf2sa
  export CLICKHOUSE_HOST=localhost
  python3 etl_batch.py
"""
import os
import sys
from dotenv import load_dotenv
load_dotenv()

import mysql.connector
from clickhouse_driver import Client

MYSQL_HOST = os.getenv("MYSQL_HOST", "127.0.0.1")
MYSQL_USER = os.getenv("MYSQL_USER", "root")
MYSQL_PASSWORD = os.getenv("MYSQL_PASSWORD", "")
MYSQL_DB = os.getenv("MYSQL_DB", "tf2sa")

CLICKHOUSE_HOST = os.getenv("CLICKHOUSE_HOST", "127.0.0.1")
CLICKHOUSE_PORT = int(os.getenv("CLICKHOUSE_PORT", "9000"))

STATE_FILE = os.getenv("ETL_STATE_FILE", "./scripts/clickhouse/state/last_player_stats_id.txt")
BATCH_SIZE = int(os.getenv("ETL_BATCH_SIZE", "1000"))

os.makedirs(os.path.dirname(STATE_FILE), exist_ok=True)


def read_last_id():
    try:
        with open(STATE_FILE, "r") as f:
            return int(f.read().strip())
    except Exception:
        return 0


def write_last_id(val):
    with open(STATE_FILE, "w") as f:
        f.write(str(val))


def fetch_new_player_stats(mysql_conn, last_id, limit):
    cur = mysql_conn.cursor(dictionary=True)
    query = (
        "SELECT ps.PlayerStatsID AS player_stats_id, ps.GameID AS game_id, g.Date as game_date, "
        "ps.SteamID as steam_id, p.PlayerName as player_name, g.Map as map_name, ps.TeamID as team, "
        "ps.ClassID as class_id, ps.Damage as damage, ps.Kills as kills, ps.Assists as assists, ps.Deaths as deaths, "
        "ps.Airshots as airshots, ps.Headshots as headshots, ps.Backstabs as backstabs, ps.Heals as heals, ps.Medkits as medkits, "
        "ps.CapturePointsCaptured as capture_points, ps.SentriesBuilt as sentries_built "
        "FROM PlayerStats ps "
        "LEFT JOIN Game g ON g.GameID = ps.GameID "
        "LEFT JOIN Player p ON p.SteamID = ps.SteamID "
        "WHERE ps.PlayerStatsID > %s "
        "ORDER BY ps.PlayerStatsID ASC "
        "LIMIT %s"
    )
    cur.execute(query, (last_id, limit))
    rows = cur.fetchall()
    cur.close()
    return rows


def transform_row(row):
    # Map to ClickHouse player_game_stats columns
    return {
        'game_id': int(row['game_id']),
        'game_date': row['game_date'],
        'steam_id': int(row['steam_id']),
        'player_name': row.get('player_name') or '',
        'map': row.get('map_name') or '',
        'team': int(row.get('team') or 0),
        'class_id': int(row.get('class_id') or 0),
        'duration': 0,
        'dpm': float(row.get('damage') or 0) / max(1.0, (row.get('Duration') or 1)),
        'kills': int(row.get('kills') or 0),
        'assists': int(row.get('assists') or 0),
        'deaths': int(row.get('deaths') or 0),
        'airshots': int(row.get('airshots') or 0),
        'headshots': int(row.get('headshots') or 0),
        'backstabs': int(row.get('backstabs') or 0),
        'damage': int(row.get('damage') or 0),
        'heals': int(row.get('heals') or 0),
        'medkits': int(row.get('medkits') or 0),
        'capture_points': int(row.get('capture_points') or 0),
        'sentries_built': int(row.get('sentries_built') or 0),
        'is_valid': 1
    }


def main():
    last_id = read_last_id()
    print(f"Last processed PlayerStatsID: {last_id}")

    mysql_conn = mysql.connector.connect(
        host=MYSQL_HOST,
        user=MYSQL_USER,
        password=MYSQL_PASSWORD,
        database=MYSQL_DB,
    )

    ch_client = Client(host=CLICKHOUSE_HOST, port=CLICKHOUSE_PORT)

    while True:
        rows = fetch_new_player_stats(mysql_conn, last_id, BATCH_SIZE)
        if not rows:
            print("No new rows. Exiting.")
            break

        records = [transform_row(r) for r in rows]

        # Prepare column order matching ClickHouse table
        columns = [
            'game_id','game_date','steam_id','player_name','map','team','class_id','duration','dpm',
            'kills','assists','deaths','airshots','headshots','backstabs','damage','heals','medkits','capture_points','sentries_built','is_valid'
        ]
        values = [[rec[col] for col in columns] for rec in records]

        ch_client.execute(
            'INSERT INTO tf2sa.player_game_stats ({}) VALUES'.format(','.join(columns)),
            values
        )

        last_id = rows[-1]['player_stats_id']
        write_last_id(last_id)
        print(f'Inserted {len(rows)} rows up to PlayerStatsID {last_id}')

    mysql_conn.close()


if __name__ == '__main__':
    main()
