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

## Lifecycle / dependencies

### End-of-life runtimes — Medium
- **.NET:** ✅ resolved — the backend was upgraded from the end-of-life `net7.0` to
  **.NET 10 (LTS)** (2026-07-10). Dependencies moved with it (EF Core 9 + Pomelo 9,
  MediatR 12.5, AutoMapper 13, updated test stack); the Docker base images are now
  `sdk:10.0` / `aspnet:10.0`. Note: EF Core is on **9**, not 10, because Pomelo's
  latest is the EF Core 9 provider — revisit when a Pomelo EF Core 10 release ships.
- **Angular 15** is still out of support — a frontend upgrade path is not yet
  documented.
- **MariaDB 10.6** should be tracked against its support window.

## Architecture / maintainability

### Unused standalone ETL worker entrypoint — Low
`TF2SA.StatsETLService/Program.cs` defines a standalone `Host` for the ETL, but the
deployed image publishes `TF2SA.Web`, which co-hosts the ETL as a
`BackgroundService`. The standalone entrypoint is never deployed and can mislead
readers into thinking ETL is a separate service. See
[ADR 0006](./decisions/0006-cohosted-etl-background-service.md).

### Aggregation SQL uses session-variable ranking — Medium (being retired)
The "top game" CTEs in `scripts/mariadb/update_*_stats.sql` use MariaDB
session-variable ranking (`@rn`/`@prev`, `LIMIT 9999999`) instead of window
functions. It is terse, version-sensitive, and harder to maintain. Recent and
all-time procedures are duplicated per class (10 files), so stat-definition changes
must be applied in multiple places.

> **Being addressed by [ADR 0008](./decisions/0008-duckdb-analytics-read-layer.md).**
> The stored procedures and pre-aggregated tables are slated for removal in favour of
> the embedded DuckDB analytics read layer, where "top game" becomes
> `arg_max(GameID, <stat>)` and there are no per-class/per-window duplicate files.
> The missing raw-table indexes noted below are part of the same story. This item can
> be removed once that migration lands.

### Missing secondary indexes on raw filter columns — Medium
`scripts/mariadb/create_stats_schema.sql` defines no secondary indexes on the columns
the aggregation filters by: `ClassStats.ClassID` and `Games.Date`. Every stored proc
therefore full-scans on `WHERE cs.ClassID = …` and `WHERE g.Date > …`. This was a
significant contributor to the read-latency problem that motivated the pre-aggregated
tables in the first place. If the DuckDB migration
([ADR 0008](./decisions/0008-duckdb-analytics-read-layer.md)) is deferred, adding
these indexes is the cheapest partial mitigation.

### DuckDB analytics read layer — implementation notes (unblocked)
A Phase 0 spike (2026-07-10) validated the planned analytics read layer
([ADR 0008](./decisions/0008-duckdb-analytics-read-layer.md),
[architecture/analytics-store.md](./architecture/analytics-store.md)): the DuckDB.NET
native library loads on linux-x64, the MySQL scanner extension loads, and on-the-fly
aggregation is fast (per-class boards ~single-digit ms; the Overall two-level board
~0.5 s at 3M rows on a 2-vCPU box). DuckDB.NET 1.5.x ships only `net8.0`/`net10.0`
assemblies; the backend's `net7.0` target could not consume it — **now unblocked**, as
the backend is on **.NET 10** (see *End-of-life runtimes* above). Two items still to
settle during implementation: the per-cycle **refresh (single writer)** vs. **concurrent
API reads** coordination, and whether the **Overall board** needs a materialized
per-game rollup or cache at peak volume.

### Database-first entities are regenerated — Low (by design, easy to trip on)
`DbContext`/entities are produced by `scaffold.sh --force` and will overwrite manual
edits. This is intended ([ADR 0004](./decisions/0004-database-first-ef-scaffolding.md))
but is a common source of "my change disappeared."

## Deployment / configuration

### Leftover Oracle Cloud CORS entry — Low
`TF2SA.Web/Program.cs` still whitelists an Oracle Cloud IP (`129.151.174.184`) from
before the migration to AWS EC2. Harmless but should be cleaned up. CORS origins and
IPs are hardcoded in `Program.cs` rather than configured.

### `docker-compose down -v` on every deploy — Medium
The CD pipeline runs `docker-compose down -v`, which removes volumes. Because MariaDB
seeds its schema from `docker-entrypoint-initdb.d` only on a fresh volume, data
persistence across deploys depends entirely on how the DB data volume is provisioned
on the host. Confirm the host setup before assuming data survives a deploy.

### Compose/tooling drift — Low
`docker-compose.yml` declares `version: "3.4"` (obsolete for Compose v2), and the
pipeline mixes `docker compose` (v2, build/push) with `docker-compose` (v1, on the
host). Worth standardizing.

## Testing

### Thin automated coverage — Medium
Backend tests cover serialization, validation, the HTTP client, and log query
params, but not the query handlers, repositories, or the ingestion orchestration.
The frontend has only default component specs. High-value untested areas: the
aggregation trigger logic and the read-path handlers.

## How to use this file

- When you fix one of these, remove it here (and, if it reflected a decision, update
  or supersede the relevant ADR).
- When you discover new debt, add it here with a severity and a pointer to the code,
  rather than letting it live only in memory or a PR comment.
