# 8. Embedded DuckDB analytics read layer

- **Status:** Accepted
- **Date:** 2026-07-10

## Context

Leaderboards are served from **10 pre-aggregated tables**
(`{Scout,Soldier,Demoman,Medic}{Recent,AllTime}`, `OverallStats{Recent,AllTime}`)
rebuilt by **10 near-duplicate MariaDB stored procedures**
(`scripts/mariadb/update_*_stats.sql`) at the end of every ingestion cycle
([ADR 0005](0005-sql-stored-procedures-for-aggregation.md)).

That design was a sound response to a real latency problem. The fact set is large
(~15 years × ≥10 games/night ≈ 55k–150k games → ~660k `PlayerStats`,
**~1–3M `ClassStats`** rows), and aggregating it per request on a single small
EC2 host that also runs ingestion and the API was slow — made worse by two schema
facts still true in `create_stats_schema.sql`: **no secondary indexes** on the
filter columns (`ClassStats.ClassID`, `Games.Date`) so class/date filters are full
scans, and a **`@rn` session-variable ranking** for "top game" that does a full
per-class sort. Pre-computing tiny result tables turned an expensive multi-million
-row aggregation into a trivial indexed read.

The cost of that choice is **rigidity, not correctness**. The 365-day window and
the min-games threshold are baked into SQL; Recent and All-Time are separate files;
adding a single stat touches SQL + proc (×2) + a re-scaffolded entity + repository +
query + handler + result + frontend model. The product wants leaderboards that vary
by stat/window/filter *dynamically*, plus **new views that don't exist yet**
(player-detail, progression-over-time, per-map, per-weapon) — none of which the
fixed-table model accommodates without more tables and procs.

A row store forces a choice: cheap reads (pre-aggregate) **or** flexible reads
(aggregate on the fly, slowly). We do **not** need transactional accuracy on the
read side — the aggregated tables are already eventually consistent, refreshed once
per ingestion cycle.

## Decision

Keep **MariaDB as the system of record** for the raw ingested tables, and add an
**embedded DuckDB analytics read layer** that serves all leaderboard/statistics
reads by aggregating on the fly.

- DuckDB runs **in-process inside `tf2sa-backend`** — no new container or service
  (unlike a standalone ClickHouse), which suits a single-community, single-host
  deployment.
- DuckDB holds a **derived, columnar fact table** at `(player, game, class)` grain,
  refreshed from MariaDB at the end of each ingestion cycle (the same trigger point
  the stored procs use today). Being derived, it is a **rebuildable cache**, not a
  source of truth.
- Reads become **parametrized aggregation queries**: class, time-window, and
  min-games are query parameters, not schema; "top game" highlights use DuckDB's
  `arg_max()` instead of the `@rn` hack. Columnar vectorized aggregation over 1–3M
  rows runs in single-digit-to-low-tens of milliseconds.
- The **CQRS read side stays** ([ADR 0003](0003-cqrs-read-side-with-mediatr.md)):
  MediatR queries/handlers/result models and the HTTP/JSON contract are unchanged;
  only the data-access repositories are swapped from EF-over-pre-agg-tables to a
  DuckDB-backed repository.

This **supersedes [ADR 0005](0005-sql-stored-procedures-for-aggregation.md)** (the
pre-aggregated tables and stored procedures are retired).
[ADR 0004](0004-database-first-ef-scaffolding.md) still governs the **raw** tables;
[ADR 0006](0006-cohosted-etl-background-service.md) is unaffected (the refresh runs
in the same co-hosted background service). The design detail lives in
[architecture/analytics-store.md](../architecture/analytics-store.md).

## Consequences

- **Flexibility.** Stat definitions, the recent-window length, and the games
  threshold change by editing one query, not schema + 10 procs + a scaffold cycle.
  New views (player-detail, progression, per-map, per-weapon) are new queries over
  the same fact table, not new tables.
- **No slowness regression.** Columnar aggregation restores read latency to the
  pre-agg range without materializing 10 tables; the `@rn` sort and the missing-index
  full scans go away.
- **No new infrastructure.** DuckDB is a library, not a server; nothing added to
  `docker-compose.yml`. The `.duckdb` file is a rebuildable cache, which also
  sidesteps the `docker-compose down -v` volume concern (it can be rebuilt from
  MariaDB on startup).
- **Eventual consistency, unchanged.** The fact table is refreshed per ingestion
  cycle — identical freshness semantics to the tables it replaces.
- **Runtime prerequisite met.** A Phase 0 spike confirmed the native library, the
  MySQL scanner extension, and fast on-the-fly aggregation. DuckDB.NET 1.5.x ships
  only `net8.0`/`net10.0` assemblies; the backend has since been upgraded from
  `net7.0` to **.NET 10**, so this no longer blocks adoption. Remaining risks: refresh
  (single writer) vs. concurrent API reads coordination, and Overall-board latency at
  peak volume. Tracked in [known-issues.md](../known-issues.md).
- **Two SQL dialects in the codebase** — MariaDB for ingestion, DuckDB for reads.
  Accepted for the analytical payoff.
