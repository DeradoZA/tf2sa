# AGENTS.md

Vendor-neutral guidance for AI coding assistants working in this repository. It is
intentionally **thin**: it orients you and points to the canonical documentation
rather than duplicating it. When something here could go stale, it links to the
source of truth instead of restating it.

Humans are the primary audience for this project's docs; you are a secondary
audience. Prefer the durable engineering docs in [`docs/`](docs/README.md) over any
assistant-specific instruction.

## What this project is (30 seconds)

TF2SA ingests Team Fortress 2 match logs from the logs.tf API for a curated South
African community, aggregates per-class and overall leaderboards in MariaDB, and
serves them via an ASP.NET Core 7 API and an Angular 15 SPA. Full overview:
[docs/architecture/overview.md](docs/architecture/overview.md).

## Orient yourself here first

- **Architecture (canonical):** [docs/architecture/overview.md](docs/architecture/overview.md)
- **Write path (ingestion):** [docs/architecture/ingestion-pipeline.md](docs/architecture/ingestion-pipeline.md)
- **Read path (serving):** [docs/architecture/request-flow.md](docs/architecture/request-flow.md)
- **Data model:** [docs/architecture/database.md](docs/architecture/database.md)
- **Deployment/CI-CD:** [docs/architecture/deployment.md](docs/architecture/deployment.md)
- **Why things are the way they are:** [docs/decisions/](docs/decisions/) (ADRs)
- **Build/run/test locally:** [docs/onboarding.md](docs/onboarding.md)
- **Conventions & workflow:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Existing debt (don't be surprised):** [docs/known-issues.md](docs/known-issues.md)

## Conventions you must respect

These trip up newcomers and automated changes most often:

1. **Errors are values, not exceptions.** Return and check `EitherStrict<Error, T>`
   / `OptionStrict<Error>` across boundaries. Don't add exception-based control flow
   at layer boundaries. ([ADR 0002](docs/decisions/0002-functional-error-handling-with-monads.md))
2. **The database is the source of truth; the ORM is generated.** EF Core entities
   and `DbContext` are scaffolded from SQL by `dotnet/TF2SA.Data/scaffold.sh`.
   **Never hand-edit scaffolded entities** — change `scripts/mariadb/*.sql`, apply
   it, then re-scaffold. ([ADR 0004](docs/decisions/0004-database-first-ef-scaffolding.md))
3. **Leaderboard math lives in SQL stored procedures**
   (`scripts/mariadb/update_*_stats.sql`), not C#. Recent and all-time are separate
   files per class — keep them in sync. ([ADR 0005](docs/decisions/0005-sql-stored-procedures-for-aggregation.md))
4. **The read side is CQRS-lite (MediatR).** A new endpoint = query + handler +
   result model + a thin controller action. ([ADR 0003](docs/decisions/0003-cqrs-read-side-with-mediatr.md))
5. **The ETL worker runs inside the backend process**, not as a separate service.
   ([ADR 0006](docs/decisions/0006-cohosted-etl-background-service.md))
6. **Respect the layering:** `Web → Query/ETL → Data → Http → Common`. No cycles, no
   upward references.
7. **Formatting is enforced:** CSharpier for C# ([`dotnet/.csharpierrc`](dotnet/.csharpierrc)),
   Prettier for the frontend. Backend builds with `/warnaserror`.

## Common tasks → where to look

| Task | Canonical guidance |
|------|--------------------|
| Add/modify a read endpoint | [request-flow.md](docs/architecture/request-flow.md) + [ADR 0003](docs/decisions/0003-cqrs-read-side-with-mediatr.md) |
| Change how a stat is computed | [database.md](docs/architecture/database.md) (edit the `update_*_stats.sql` procs) |
| Add/change a table or column | [database.md — scaffold workflow](docs/architecture/database.md#how-to-change-the-schema-the-scaffold-workflow) |
| Change an ingested field | [ingestion-pipeline.md](docs/architecture/ingestion-pipeline.md) + [docs/stats.md](docs/stats.md) |
| Add a community uploader | [ADR 0007](docs/decisions/0007-curated-uploader-allowlist.md) (`LogsTF.Uploaders` config) |
| Build / run / test | [onboarding.md](docs/onboarding.md) |

## Working agreement

- **Verify before asserting.** This repo is database-first and has generated code;
  confirm a file is not scaffolded before editing it, and check the actual code
  rather than assuming. If a doc and the code disagree, trust the code and flag the
  drift.
- **Keep docs in sync.** Each architecture doc lists when it must be updated; honor
  those triggers in the same change. Record material decisions as a new ADR.
- **Don't duplicate knowledge into assistant files.** Put durable knowledge in
  `docs/`; keep this file and the tool-specific shims thin.
- **Match surrounding style.** Follow existing patterns, naming, and comment density
  rather than introducing new idioms.

## Tool-specific notes

Assistant-specific files ([CLAUDE.md](CLAUDE.md),
[.github/copilot-instructions.md](.github/copilot-instructions.md)) exist only so
each tool discovers this guidance; they defer to this file. Keep vendor-neutral
guidance **here**.
