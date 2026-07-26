# Known Issues & Technical Debt

> An honest register of known problems, debt, and rough edges, derived from reading
> the code as of 2026-07-05. This is documentation, not a promise of work — but it
> gives contributors (human and AI) a truthful map of where the sharp edges are so
> they don't rediscover them the hard way or build on a false assumption.
>
> Severity is a rough guide: **High** = can cause wrong data or outages; **Medium**
> = maintainability/lifecycle risk; **Low** = cosmetic/cleanup.

## Correctness

### `StatisticsUpdater` copy-paste guard bugs — ✅ fixed (2026-07-11)
Two guard clauses checked the wrong variable and have been corrected:
`StatisticsUpdater` now checks `updateScoutAllTime.HasValue` (was
`updateScoutRecent.HasValue`), and `LogsTFIngestionHandler` now logs
`updateStatisticsResult` (was `updatePlayersResult`) on aggregation failure. Fixed
in passing during the DuckDB analytics build-out
([ADR 0008](./decisions/0008-duckdb-analytics-read-layer.md)); still no dedicated
test for the aggregation-trigger path (see *Thin automated coverage* below). The
per-proc guard logic will disappear entirely once the stored procs are retired.

### Repositories don't fully use cancellation tokens / monads — Medium
Several data-access methods use try/catch internally and don't thread
`CancellationToken`s or return monadic errors, unlike the rest of the codebase.
There is an explicit `// TODO` to this effect in `LogsTFIngestionHandler`. This
weakens cooperative cancellation during long ingestion cycles and is inconsistent
with [ADR 0002](./decisions/0002-functional-error-handling-with-monads.md).

### Leaderboard sort/top-game ties have no secondary key — Low (pre-existing, confirmed 2026-07-26)
`class_board`/`overall_board`/etc. `ORDER BY` and their `arg_max(GameID, stat)`
"top game" picks have no tiebreaker, so two players (or two games) with an exactly
equal sort value can resolve to a different order/row across runs — including two
consecutive calls on the same live connection with no code change in between.
Confirmed pre-existing (not introduced by the MediatR/AutoMapper/MariaDB removal
work) by reproducing it repeatedly on the same known-tied players throughout that
work. Would need an `ORDER BY ..., SteamID` / `arg_max(GameID, stat, GameID)`-style
tiebreaker in `Leaderboards.sql` to close, if it's ever worth doing.

### `PlayersRepository.UpdatePlayers`' quirks, preserved verbatim in the DuckDB port — Low
Two pre-existing behaviours in the Steam-profile-refresh step, carried forward
unchanged into `DuckDbAnalyticsStore.UpdatePlayersAsync`
([ADR 0009](./decisions/0009-drop-mariadb.md)) rather than silently fixed mid-port:
- **`AvatarHash` is set from the incoming record's `Avatar` field, not its
  `AvatarHash`** — a copy-paste bug, so every player's stored `AvatarHash` has always
  held a URL, not a hash.
- **A player absent from the Steam API refresh batch has every profile column
  (`Avatar`, `AvatarFull`, `AvatarHash`, `AvatarMedium`, `LocalCountryCode`,
  `PlayerName`, `RealName`, `ProfileUrl`) nulled out**, not left alone. In normal
  operation the batch covers every player already in the table, so this only bites
  when the Steam API doesn't return data for someone (e.g. a deleted account, a
  partial API failure).

### Player-list sort order changed for symbol-prefixed names — Low (disclosed, not a bug)
Since [ADR 0009](./decisions/0009-drop-mariadb.md), the default player-list sort
(`playerName asc`) orders by DuckDB's plain-codepoint `VARCHAR` comparison, not
MariaDB's `utf8mb4_unicode_ci` collation (which gave punctuation near-last weight, so
e.g. a name starting with `` ` `` or `_` could sort near names starting with the same
letter ignoring the symbol). Confirmed by hand: Python's default string sort matches
DuckDB's output exactly. Total row count and every other field are unaffected — pure
ordering, caught by the pre-cutover endpoint diff.

## Lifecycle / dependencies

### End-of-life runtimes — Medium
- **.NET:** ✅ resolved — the backend was upgraded from the end-of-life `net7.0` to
  **.NET 10 (LTS)** (2026-07-10); the Docker base images are `sdk:10.0` /
  `aspnet:10.0`. **MediatR, AutoMapper, EF Core, Pomelo, and MariaDB have since been
  removed entirely** (2026-07-26 — [ADR 0009](./decisions/0009-drop-mariadb.md)),
  so their versions/EOL status are no longer a concern at all: DuckDB (embedded, no
  server to track) is the only remaining data-layer dependency, `csharp-monad` for
  error handling, and hand-written mapping in place of AutoMapper.
- **Angular 15** is still out of support — a frontend upgrade path is not yet
  documented.

### CD pipeline still targets .NET 7 — ✅ resolved (2026-07-26)
Was: `.github/workflows/cd.yml` did `setup-dotnet` with `dotnet-version: "7.0.x"`,
which would have failed outright the first time `cd.yml` ran after the .NET 10
upgrade (`global.json` pins `10.0.100` with `rollForward: latestMinor`, same-major
only). Fixed ahead of the first post-upgrade push to `main`, alongside the new-stats
work in this branch — `setup-dotnet` now requests `10.0.x`, matching the Docker
images (already on `sdk:10.0`) and `global.json`.

## Architecture / maintainability

### Unused standalone ETL worker entrypoint — Low
`TF2SA.StatsETLService/Program.cs` defines a standalone `Host` for the ETL, but the
deployed image publishes `TF2SA.Web`, which co-hosts the ETL as a
`BackgroundService`. The standalone entrypoint is never deployed and can mislead
readers into thinking ETL is a separate service. See
[ADR 0006](./decisions/0006-cohosted-etl-background-service.md).

### Overall-board latency at peak volume — Medium (open)
The Overall leaderboard's two-level aggregation (per-game sum, then average across
games) measured ~0.5s at 3M `ClassStats` rows on a 2-vCPU box during the Phase 0 spike
([ADR 0008](./decisions/0008-duckdb-analytics-read-layer.md),
[architecture/analytics-store.md](./architecture/analytics-store.md)). Materializing a
per-game rollup, or a short-TTL cache, would mitigate this if it becomes a problem in
production; neither is implemented. The refresh-vs-concurrent-reads question from the
same spike is settled — a single shared connection plus a semaphore, adequate at this
scale — leaving this as the one open item from that investigation.

### Database-first entities are regenerated — ✅ resolved (2026-07-26)
Was: `DbContext`/entities were produced by `scaffold.sh --force`, a common source of
"my change disappeared." Moot since [ADR 0009](./decisions/0009-drop-mariadb.md) —
there is no EF Core, no scaffolding, and no generated entities anymore; the DuckDB
row POCOs in `TF2SA.Data/Analytics/Rows/` are hand-written and never regenerated.

## Deployment / configuration

### Leftover Oracle Cloud CORS entry — Low
`TF2SA.Web/Program.cs` still whitelists an Oracle Cloud IP (`129.151.174.184`) from
before the migration to AWS EC2. Harmless but should be cleaned up. CORS origins and
IPs are hardcoded in `Program.cs` rather than configured.

### `docker-compose down -v` on every deploy — ✅ resolved (2026-07-26)
Was: the CD pipeline's `docker-compose down -v` removed volumes, and because MariaDB
had no named volume, data persistence across deploys depended entirely on host
provisioning. Resolved by [ADR 0009](./decisions/0009-drop-mariadb.md) — MariaDB is
gone, and DuckDB's data file lives on a host **bind mount**
(`/home/ubuntu/tf2sa-data`), which `down -v` cannot touch (it only removes volumes
declared in `docker-compose.yml`'s `volumes:` block). `down -v` still removes
`tf2sa-web` (the compiled SPA), which is cheap to rebuild. See
[deployment.md](./architecture/deployment.md) and `scripts/backup-duckdb.sh` for the
new backup story now that the file is load-bearing.

### Structurizr rendered artifacts are stale — Low
`docs/structurizr/workspace.dsl` was updated to reflect DuckDB
([ADR 0009](./decisions/0009-drop-mariadb.md)), but the rendered exports
(`workspace.json`, the `.structurizr/images/*.png` thumbnails) were not regenerated —
no Structurizr CLI available in this environment. The Mermaid diagrams in
`docs/architecture/*.md` are current and are the actual quick-reference source;
regenerate the Structurizr exports next time someone has the CLI available.

### Compose/tooling drift — Low
`docker-compose.yml` declares `version: "3.4"` (obsolete for Compose v2), and the
pipeline mixes `docker compose` (v2, build/push) with `docker-compose` (v1, on the
host). Worth standardizing.

## Testing

### Thin automated coverage — Medium
Backend tests cover serialization, validation, the HTTP client, and log query
params, but not the query handlers or the ingestion orchestration. The frontend has
only default component specs. High-value untested areas: `DuckDbAnalyticsStore`'s
write path and the read-path handlers — currently verified manually (real-data
diffs against a scratch DuckDB file) rather than by an automated test suite.

## How to use this file

- When you fix one of these, remove it here (and, if it reflected a decision, update
  or supersede the relevant ADR).
- When you discover new debt, add it here with a severity and a pointer to the code,
  rather than letting it live only in memory or a PR comment.
