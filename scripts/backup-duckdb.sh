#!/bin/sh
# Periodic backup of the DuckDB data file (ADR 0009 — DuckDB is the system of
# record, not a rebuildable cache). Not run by CI/CD; install as a cron job or
# systemd timer on the deploy host, e.g.:
#
#   0 3 * * * /home/ubuntu/tf2sa/scripts/backup-duckdb.sh
#
# Deliberately simple (a straight file copy, not a snapshot/export): the app
# serializes every write through a single in-process semaphore
# (DuckDbAnalyticsStore), so this only risks catching a write mid-flight in the
# rare case a cron tick lands during one — acceptable for a low-traffic site,
# and the worst case is a backup that's missing the last few in-flight writes,
# not a corrupt file. Restore by stopping the backend, copying a dated backup
# back over the live file, and starting the backend again.

set -eu

DATA_DIR="${TF2SA_DUCKDB_DATA_DIR:-/home/ubuntu/tf2sa-data}"
BACKUP_DIR="${TF2SA_DUCKDB_BACKUP_DIR:-/home/ubuntu/tf2sa-backups}"
KEEP_DAYS="${TF2SA_DUCKDB_BACKUP_KEEP_DAYS:-14}"
DB_FILE="$DATA_DIR/analytics.duckdb"
STAMP=$(date +%Y%m%d-%H%M%S)

if [ ! -f "$DB_FILE" ]; then
	echo "backup-duckdb: no database file at $DB_FILE, nothing to back up" >&2
	exit 0
fi

mkdir -p "$BACKUP_DIR"
cp -p "$DB_FILE" "$BACKUP_DIR/analytics-$STAMP.duckdb"

# DuckDB may also have a write-ahead-log file alongside the main one; back it
# up too if present, so a restore isn't missing not-yet-checkpointed data.
if [ -f "$DB_FILE.wal" ]; then
	cp -p "$DB_FILE.wal" "$BACKUP_DIR/analytics-$STAMP.duckdb.wal"
fi

find "$BACKUP_DIR" -name 'analytics-*.duckdb*' -mtime "+$KEEP_DAYS" -delete

echo "backup-duckdb: backed up $DB_FILE -> $BACKUP_DIR/analytics-$STAMP.duckdb"
