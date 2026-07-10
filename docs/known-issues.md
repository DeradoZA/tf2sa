# Known Issues & Technical Debt

> An honest register of known problems, debt, and rough edges, derived from reading
> the code as of 2026-07-05. This is documentation, not a promise of work — but it
> gives contributors (human and AI) a truthful map of where the sharp edges are so
> they don't rediscover them the hard way or build on a false assumption.
>
> Severity is a rough guide: **High** = can cause wrong data or outages; **Medium**
> = maintainability/lifecycle risk; **Low** = cosmetic/cleanup.

## Correctness

### `StatisticsUpdater` copy-paste guard bugs — Medium
`TF2SA.StatsETLService/LogsTFIngestion/Services/StatisticsUpdater.cs` has
guard clauses that check the wrong variable:
- After the Scout **all-time** update, the code checks `updateScoutRecent.HasValue`
  again instead of `updateScoutAllTime.HasValue`.
- In `LogsTFIngestionHandler.ProcessLogs`, the aggregation-failure branch logs
  `updatePlayersResult` instead of the statistics update result.

Effect: some aggregation errors may be swallowed or misreported. The happy path
(successful updates) is unaffected. Fix is low-risk but should come with a test.

### Repositories don't fully use cancellation tokens / monads — Medium
Several data-access methods use try/catch internally and don't thread
`CancellationToken`s or return monadic errors, unlike the rest of the codebase.
There is an explicit `// TODO` to this effect in `LogsTFIngestionHandler`. This
weakens cooperative cancellation during long ingestion cycles and is inconsistent
with [ADR 0002](./decisions/0002-functional-error-handling-with-monads.md).

## Lifecycle / dependencies

### End-of-life runtimes — High (over time)
- **.NET 7** is out of support (ended May 2024). The whole backend targets `net7.0`.
- **Angular 15** is out of support.
- **MariaDB 10.6** should be tracked against its support window.

These receive no security updates; an upgrade path (to a current LTS .NET and a
supported Angular) is not yet documented.

## Architecture / maintainability

### Unused standalone ETL worker entrypoint — Low
`TF2SA.StatsETLService/Program.cs` defines a standalone `Host` for the ETL, but the
deployed image publishes `TF2SA.Web`, which co-hosts the ETL as a
`BackgroundService`. The standalone entrypoint is never deployed and can mislead
readers into thinking ETL is a separate service. See
[ADR 0006](./decisions/0006-cohosted-etl-background-service.md).

### Aggregation SQL uses session-variable ranking — Medium
The "top game" CTEs in `scripts/mariadb/update_*_stats.sql` use MariaDB
session-variable ranking (`@rn`/`@prev`, `LIMIT 9999999`) instead of window
functions. It is terse, version-sensitive, and harder to maintain. Recent and
all-time procedures are duplicated per class (10 files), so stat-definition changes
must be applied in multiple places.

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
