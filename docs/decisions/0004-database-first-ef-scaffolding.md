# 4. Database-first EF Core scaffolding

- **Status:** Accepted
- **Date:** 2026-07-05 (documented retroactively)

## Context

The data model is expressed most naturally in SQL: precise MariaDB column types
(`tinyint unsigned`, `mediumint unsigned`), collations for non-ASCII player names,
foreign-key cascade rules, and — crucially — the **aggregation logic in stored
procedures**. Managing all of that through EF Core code-first migrations would fight
the tooling and duplicate knowledge that already lives cleanly in SQL.

## Decision

TF2SA is **database-first**. The canonical schema is the SQL in
`scripts/mariadb/`. The EF Core `DbContext` and entities in
`TF2SA.Data/Entities/MariaDb` are **generated** from the live database by
`TF2SA.Data/scaffold.sh` (`dotnet ef dbcontext scaffold ... --force`, restricted to
an explicit table list). We do **not** use EF migrations.

## Consequences

- SQL is the single source of truth for the data model; the schema and the exact
  MariaDB types are reviewable in one place.
- Stored-procedure-based aggregation ([ADR 0005](0005-sql-stored-procedures-for-aggregation.md))
  fits naturally.
- **The generated entities and the scaffolded parts of `TF2SADbContext` must not be
  hand-edited** — `scaffold.sh --force` overwrites them. Model changes follow the
  scaffold workflow documented in
  [database.md](../architecture/database.md#how-to-change-the-schema-the-scaffold-workflow).
- Scaffolding requires a reachable database with the target schema, and the
  `--table` list in `scaffold.sh` must be kept current as tables are added.
- New tables need manual follow-up: `DbSet` wiring, repository, DI registration, and
  mapping profiles.
