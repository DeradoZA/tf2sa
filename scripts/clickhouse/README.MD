# ClickHouse — Local dev quickstart

Run these from the repo root.

Prereqs
- Docker (or Docker Desktop with WSL integration).

Start a local ClickHouse server
```bash
# manually
docker run -d --name clickhouse-server \
  --ulimit nofile=262144:262144 \
  -p 9000:9000 -p 8123:8123 \
  -v clickhouse_data:/var/lib/clickhouse \
  clickhouse/clickhouse-server:latest
```

Apply the SQL migration (choose one)

- Using the server's client (recommended)
```bash
docker exec -i clickhouse-server clickhouse-client --multiquery < ./scripts/clickhouse/migrations/01_StoreRawLogs.SQL
```

- Using a temporary client container (mounts migrations)
```bash
docker run --rm -i -v "$PWD/scripts/clickhouse:/scripts" --network host clickhouse/clickhouse-client --multiquery < /scripts/migrations/01_StoreRawLogs.SQL
```

- Using the HTTP API (curl)
```bash
curl 'http://localhost:8123/?user=default' --data-binary @./scripts/clickhouse/migrations/01_StoreRawLogs.SQL
```

Troubleshooting notes (short)
- "No such file": ensure you point to `scripts/clickhouse/migrations/01_StoreRawLogs.SQL` (not the mounted folder root).
- "the input device is not a TTY": don't use `-t` when redirecting stdin; use `-i` (or omit `-t`).
- Server log: `/entrypoint.sh: neither CLICKHOUSE_USER nor CLICKHOUSE_PASSWORD is set...` — this disables external network access for the `default` user; use `docker exec` (client inside container) or set `CLICKHOUSE_USER`/`CLICKHOUSE_PASSWORD` env when starting the container if you need remote access.
- Function errors (e.g., `toUnixTimestamp64`): ClickHouse version differences exist; replace unsupported functions with compatible ones (e.g., `toUnixTimestamp`).

Verify migration
```bash
docker exec clickhouse-server clickhouse-client --query "SHOW CREATE TABLE default.logs_tf"
docker exec clickhouse-server clickhouse-client --query "SELECT count() FROM default.logs_tf"
```