# 5. SQL stored procedures for aggregation

- **Status:** Accepted
- **Date:** 2026-07-05 (documented retroactively)

## Context

Leaderboards require aggregating potentially large numbers of `ClassStats` /
`PlayerStats` rows into per-player summaries (averages, win rates, and "top game"
highlights), for five class groupings × two time windows. Doing this in C#/LINQ
would pull large row sets into the application and re-implement set-based math that
databases do well. The results also change infrequently — only after new logs are
ingested — so they can be pre-computed.

## Decision

Aggregation is implemented as **MariaDB stored procedures** — one per aggregated
table (`scripts/mariadb/update_*_stats.sql`, ten in total). Each procedure `DELETE`s
its target table and re-`INSERT`s a freshly computed result set. The application
triggers them at the end of each ingestion cycle via
`StatsRepository<TEntity>.CallUpdateStoredProc()` →
`ExecuteSqlRawAsync(...)`, orchestrated by `StatisticsUpdater`. The API then reads
the pre-computed tables directly (see [request-flow.md](../architecture/request-flow.md)).

## Consequences

- Aggregation runs where the data lives (set-based, no large transfers to the app),
  and reads are cheap because results are materialized.
- Business rules such as the **365-day recent window** and the **20-game minimum**
  live in SQL; changing them means editing SQL, not C#.
- Recent and all-time are **separate files** and must be kept in sync when a stat
  definition changes.
- The "top game" queries use MariaDB session-variable ranking (`@rn`/`@prev`) rather
  than window functions — terse and version-sensitive. Portability to another
  engine, and readability, are limited; see [known-issues.md](../known-issues.md).
- Aggregated tables are eventually consistent with raw data — they reflect the state
  as of the last successful stored-proc run.
