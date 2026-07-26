# Deployment

> How TF2SA is packaged, delivered, and run in production. For the container
> responsibilities see [overview.md](./overview.md).

## Runtime topology

Production runs on a single **AWS EC2** host (Ubuntu) as four containers defined in
[`docker-compose.yml`](../../docker-compose.yml), on a shared `tf2sa-net` bridge
network. The Angular build container and nginx communicate through a shared Docker
volume (`tf2sa-web`) rather than the network. There is no separate database
container — DuckDB is embedded in `tf2sa-backend`
([ADR 0009](../decisions/0009-drop-mariadb.md)) and its data file lives on a host
bind mount.

```mermaid
flowchart TB
    user["Browser → tf2sa.co.za"]

    subgraph ec2["AWS EC2 host (Ubuntu)"]
        direction TB
        proxy["t2sa-reverse-proxy<br/><b>nginx:latest</b><br/>:80 → 301 https, :443 TLS"]
        subgraph net["tf2sa-net (bridge)"]
            backend["tf2sa-backend<br/><b>ASP.NET Core 10</b><br/>listens :80 (internal)<br/>embedded DuckDB"]
        end
        frontend["tf2sa-frontend<br/><b>Angular build-only</b>"]
        certbot["certbot<br/>renew every 12h"]
        webvol[["volume: tf2sa-web<br/>compiled SPA"]]
        certvol[["host: /home/ubuntu/certbot<br/>certs + ACME webroot"]]
        datavol[["host: /home/ubuntu/tf2sa-data<br/>analytics.duckdb"]]
    end

    user -->|443| proxy
    proxy -->|"location /api → http://tf2sa-backend:80"| backend
    backend -->|"reads/writes"| datavol
    frontend -.->|writes dist to| webvol
    webvol -.->|"mounted read-only at /usr/share/nginx/html"| proxy
    certbot -.-> certvol
    certvol -.->|mounted| proxy
```

### Container roles

| Container | Image | Ports | Notes |
|-----------|-------|-------|-------|
| `tf2sa-backend` | `danielcoetzee/tf2sa-backend` (`dotnet/Dockerfile.tf2sa-backend`) | internal :80 | Publishes `TF2SA.Web`; runs API + co-hosted ETL + embedded DuckDB, the only database. Data file bind-mounted at `/data`. |
| `tf2sa-frontend` | `danielcoetzee/tf2sa-frontend` (`frontend/Dockerfile.tf2sa-frontend`) | none | **Build-only** — compiles the SPA into `/out` and exits; nginx serves the volume. |
| `t2sa-reverse-proxy` | `nginx:latest` | `80`, `443` | TLS termination, static SPA, `/api` proxy. Periodically reloads for renewed certs. |
| `certbot` | `certbot/certbot:latest` | none | Renews Let's Encrypt certs on a 12h loop. |

> **Frontend is compile-then-exit.** `tf2sa-frontend` is not a running web server;
> it exists only to produce static assets into the shared `tf2sa-web` volume, which
> nginx mounts read-only. Rebuilding the frontend means rebuilding this container so
> fresh assets land in the volume.

## nginx and TLS

[`scripts/iac/nginx/nginx.conf`](../../scripts/iac/nginx/nginx.conf):

- Port 80 serves the ACME challenge (`/.well-known/acme-challenge/`) and otherwise
  `301`-redirects to `https://tf2sa.co.za`.
- Port 443 terminates TLS (certs under `/etc/nginx/ssl/live/tf2sa.co.za/`), serves
  the SPA with SPA fallback (`try_files $uri $uri/ /index.html`), and proxies
  `location /api` to `http://tf2sa-backend:80`.

The backend trusts forwarding headers (`app.UseForwardedHeaders(...)` for
`X-Forwarded-For`/`X-Forwarded-Proto`) because it sits behind this proxy.

## Configuration and secrets

All runtime configuration is injected as environment variables (see
[onboarding.md](../onboarding.md#environment-variables) for the full list and local
setup). In CI/CD these come from GitHub Actions secrets.

| Variable | Purpose |
|----------|---------|
| `TF2SA_DUCKDB_PATH` | Path to the DuckDB data file — the only database. `/data/analytics.duckdb` in production (bind-mounted); a temp-dir default locally if unset. |
| `TF2SA_STEAM_API_KEY` | Steam Web API key for player enrichment. |

The frontend's API base URL is compiled in, not env-injected:
`frontend/src/environments/environment.ts` (`https://tf2sa.co.za/api/v1`) for
production, `environment.development.ts` (`https://localhost:5001/api/v1`) for dev.

## CI/CD pipelines (GitHub Actions)

Two workflows in `.github/workflows/`:

```mermaid
flowchart LR
    subgraph ci["ci.yml — on pull_request"]
        pr[PR opened/updated] --> build1["docker compose build"]
    end
    subgraph cd["cd.yml — on push to main"]
        push[push to main] --> restore["dotnet restore/build (/warnaserror)"]
        restore --> test["dotnet test + coverage"]
        test --> cov["publish → Coveralls"]
        cov --> todo["TODO-to-Issue"]
        todo --> images["docker compose build --no-cache + push → Docker Hub"]
        images --> ssh["SSH to EC2:<br/>git pull, compose pull, down -v, up -d"]
    end
```

- **CI** (`ci.yml`) runs on every pull request and only verifies that the Docker
  images build.
- **CD** (`cd.yml`) runs on push to `main`: it builds and unit-tests the .NET
  solution (failing on warnings), publishes coverage to Coveralls, converts new
  `TODO` comments to GitHub issues, builds and pushes the three images to Docker
  Hub, then SSHes into the EC2 host and redeploys with `docker-compose pull` +
  `down -v` + `up -d`.

> **`down -v` drops volumes on each deploy.** This no longer risks the database:
> since [ADR 0009](../decisions/0009-drop-mariadb.md), DuckDB's data file lives on a
> **host bind mount** (`/home/ubuntu/tf2sa-data` → `/data`), not a compose-managed
> volume, and `down -v` only removes what's declared in `docker-compose.yml`'s
> top-level `volumes:` block (currently just `tf2sa-web`, the compiled SPA — cheap to
> rebuild). See [scripts/backup-duckdb.sh](../../scripts/backup-duckdb.sh) for
> periodic backup of the data file itself, which is now load-bearing rather than a
> rebuildable cache.

## Host provisioning

`scripts/iac/iac.sh` is a minimal bootstrap that installs Docker and
docker-compose on a fresh Ubuntu host. TLS material lives under
`/home/ubuntu/certbot` on the host and is bind-mounted into nginx and certbot;
DuckDB's data file lives under `/home/ubuntu/tf2sa-data` the same way. Neither
directory needs pre-creation — Docker creates an empty one on first bind mount, and
`tf2sa-backend` self-initializes the (empty) DuckDB schema on first connection.

## One-time migration (existing deployments only)

A **fresh** deployment needs no migration step — an empty `tf2sa-data` directory
self-initializes and fills up from real ingestion. A deployment that already has
production data in a still-running MariaDB (i.e. upgrading *through*
[ADR 0009](../decisions/0009-drop-mariadb.md) rather than starting clean) needs a
one-time bulk copy before cutover, run once via the DuckDB CLI against the live
MariaDB — reusing the exact mechanism the old per-cycle refresh used
([analytics-store.md](./analytics-store.md)):

```sql
INSTALL mysql; LOAD mysql;
ATTACH '<production MariaDB connection>' AS maria (TYPE mysql, READ_ONLY);
CREATE TABLE Players SELECT * FROM maria.Players;
CREATE TABLE Games SELECT * FROM maria.Games;
CREATE TABLE PlayerStats SELECT * FROM maria.PlayerStats;
CREATE TABLE ClassStats SELECT * FROM maria.ClassStats;
CREATE TABLE WeaponStats SELECT * FROM maria.WeaponStats;
DETACH maria;

-- Bootstrap the surrogate-key sequences past the copied data. This DuckDB version
-- does not support ALTER SEQUENCE ... RESTART WITH — drop and recreate instead,
-- then re-bind each table's DEFAULT (schema init only does this for a fresh table).
DROP SEQUENCE seq_playerstats; DROP SEQUENCE seq_classstats; DROP SEQUENCE seq_weaponstats;
CREATE SEQUENCE seq_playerstats START (SELECT max(PlayerStatsID) + 1 FROM PlayerStats);
CREATE SEQUENCE seq_classstats START (SELECT max(ClassStatsID) + 1 FROM ClassStats);
CREATE SEQUENCE seq_weaponstats START (SELECT max(WeaponStatsID) + 1 FROM WeaponStats);
ALTER TABLE PlayerStats ALTER COLUMN PlayerStatsID SET DEFAULT nextval('seq_playerstats');
ALTER TABLE ClassStats ALTER COLUMN ClassStatsID SET DEFAULT nextval('seq_classstats');
ALTER TABLE WeaponStats ALTER COLUMN WeaponStatsID SET DEFAULT nextval('seq_weaponstats');
```

Run this against a file at the target `/home/ubuntu/tf2sa-data/analytics.duckdb`
path (create the directory first if migrating before the first deploy of the new
image), confirm row counts match MariaDB, then deploy — MariaDB can be decommissioned
once the new backend is confirmed healthy against the migrated file.

## One-time data backfill: player descriptions

`Players.Description` (added by a schema migration, so a normal deploy adds the
column automatically — `tf2sa-backend` runs `Schema.sql` unconditionally on its
first connection) is populated separately, since the text itself is hand-authored
data, not schema. `scripts/player-descriptions.sql` needs to run once against
production after the first deploy that includes the column:

```sh
# DuckDB requires exclusive access for writes, and tf2sa-backend holds the file
# open continuously — stop it first, run the script directly against the file,
# then start it again. A few seconds of downtime, no data at risk (the script is
# UPDATE-only, and this can be re-run safely if interrupted).
sudo docker-compose stop tf2sa-backend

# duckdb CLI version must match DuckDB.NET.Data.Full's pinned version
# (dotnet/TF2SA.Data/TF2SA.Data.csproj) for file-format compatibility.
curl -L https://github.com/duckdb/duckdb/releases/download/v1.5.3/duckdb_cli-linux-amd64.zip -o /tmp/duckdb_cli.zip
unzip /tmp/duckdb_cli.zip -d /tmp
/tmp/duckdb /home/ubuntu/tf2sa-data/analytics.duckdb < scripts/player-descriptions.sql

sudo docker-compose start tf2sa-backend
```

Re-run this any time new players cross the 20-game threshold used when the
descriptions were authored (see `scripts/player-narrative-facts.sql`) and you want
them to get a hand-written description instead of the generic fallback.

## Historical note

Production previously ran on **Oracle Cloud** and was migrated to **AWS EC2**. A
leftover Oracle Cloud IP still appears in the backend CORS allowlist
(`TF2SA.Web/Program.cs`); it is harmless but noted in
[known-issues.md](../known-issues.md) for cleanup.

## When to update this document

Update this document when:

- The container topology, ports, networks, or volumes change.
- The hosting provider or host provisioning changes.
- The nginx routing/TLS setup changes.
- The CI or CD workflow steps change (build, test, publish, or deploy).
- The set of required environment variables/secrets changes (also update
  [onboarding.md](../onboarding.md)).
