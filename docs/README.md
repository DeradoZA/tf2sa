# TF2SA Documentation

Documentation map. The top-level [`README.md`](../README.md) is the entry point;
this folder holds the canonical technical reference.

## Start here

- **[architecture/overview.md](./architecture/overview.md)** — the canonical
  system overview (what, why, containers, project structure, tech stack,
  conventions). Read this first.

## Architecture (canonical technical reference)

| Document | Covers |
|----------|--------|
| [architecture/overview.md](./architecture/overview.md) | System context, container view, project dependency graph, tech-stack rationale, conventions. |
| [architecture/ingestion-pipeline.md](./architecture/ingestion-pipeline.md) | The write path: logs.tf → validate → insert directly into DuckDB. |
| [architecture/request-flow.md](./architecture/request-flow.md) | The read path: browser → nginx → controller → query handler → DuckDB store. |
| [architecture/database.md](./architecture/database.md) | Raw data model (embedded DuckDB, the system of record), ERD, schema workflow. |
| [architecture/analytics-store.md](./architecture/analytics-store.md) | The DuckDB store: schema, writes, on-the-fly leaderboard queries, migration history. |
| [architecture/deployment.md](./architecture/deployment.md) | Container topology, nginx/TLS, CI/CD, hosting, DuckDB backup. |

## Reference

| Document | Covers |
|----------|--------|
| [onboarding.md](./onboarding.md) | Local dev: prerequisites, env vars, build/test/run, common-task recipes. |
| [stats.md](./stats.md) | Field-by-field mapping of the logs.tf JSON response to the database. |
| [known-issues.md](./known-issues.md) | Honest register of technical debt and rough edges. |
| [structurizr/](./structurizr/) | Structurizr C4 model (DSL source of the architecture diagrams). |

## Decisions (ADRs)

The *why* behind non-obvious choices lives in [decisions/](./decisions/):

- [0001 — Record architecture decisions](./decisions/0001-record-architecture-decisions.md)
- [0002 — Functional error handling with monads](./decisions/0002-functional-error-handling-with-monads.md)
- [0003 — CQRS read side with MediatR](./decisions/0003-cqrs-read-side-with-mediatr.md)
- [0004 — Database-first EF scaffolding](./decisions/0004-database-first-ef-scaffolding.md)
- [0005 — SQL stored procedures for aggregation](./decisions/0005-sql-stored-procedures-for-aggregation.md) *(superseded by 0008)*
- [0006 — Co-hosted ETL background service](./decisions/0006-cohosted-etl-background-service.md)
- [0007 — Curated uploader allowlist](./decisions/0007-curated-uploader-allowlist.md)
- [0008 — Embedded DuckDB analytics read layer](./decisions/0008-duckdb-analytics-read-layer.md)
- [0009 — Drop MariaDB — DuckDB becomes the system of record](./decisions/0009-drop-mariadb.md)

## Keeping docs in sync

Each architecture document ends with a **"When to update this document"** section.
When you change the system, update the matching doc in the same change, and record
material decisions as a new ADR. AI-assistant files
([AGENTS.md](../AGENTS.md), [CLAUDE.md](../CLAUDE.md),
[.github/copilot-instructions.md](../.github/copilot-instructions.md)) intentionally
stay thin and point here — put durable knowledge in these documents, not in the
assistant files.
