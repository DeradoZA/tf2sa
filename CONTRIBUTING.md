# Contributing to TF2SA

Thanks for contributing. This document covers the workflow and coding conventions.
For how to build and run the project, see [docs/onboarding.md](docs/onboarding.md);
for how the system is structured and why, see
[docs/architecture/overview.md](docs/architecture/overview.md).

## Workflow

TF2SA is **issue-driven** and trunk-based around `main`.

1. **Start from an issue.** Most branches map to a GitHub issue. `TODO` comments on
   `main` are auto-converted to issues by the CD pipeline.
2. **Branch** off `main` using the convention `<issue-number>-<short-slug>`
   (e.g. `201-architecture`, `192-stored-proc-failing-for-non-overall`).
3. **Make the change** following the conventions below. Update documentation in the
   same change (see "Documentation expectations").
4. **Open a pull request.** CI (`ci.yml`) builds the Docker images on every PR; it
   must pass.
5. **Merge to `main`.** CD (`cd.yml`) builds, unit-tests (warnings-as-errors),
   publishes coverage to Coveralls, pushes images to Docker Hub, and deploys to the
   EC2 host. Treat a merge to `main` as a production deploy.

## Coding conventions

### C#
- **Formatting:** [CSharpier](dotnet/.csharpierrc) — tabs, print width 80. Format
  before committing.
- **Warnings are errors.** Builds use `/warnaserror`; don't merge code that warns.
- **Errors are values, not exceptions.** Return `EitherStrict<Error, T>` or
  `OptionStrict<Error>` across boundaries and check the result. See
  [ADR 0002](docs/decisions/0002-functional-error-handling-with-monads.md).
- **Respect the layering.** Dependencies flow inward: `Web → Query/ETL → Data →
  Http → Common`. Don't introduce cycles or upward references. See the dependency
  graph in [overview.md](docs/architecture/overview.md#net-solution-structure).
- **Read side is CQRS-lite.** New endpoints = query + handler + result model +
  thin controller action, registered directly in `TF2SA.Query/DependencyInjection.cs`
  (no mediator library). See
  [ADR 0003](docs/decisions/0003-cqrs-read-side-with-mediatr.md).
- **Thread `CancellationToken`s** through async work.

### Database
- **DuckDB is the only database; there's no ORM.** The schema is hand-maintained SQL
  in [`TF2SA.Data/Analytics/Schema.sql`](dotnet/TF2SA.Data/Analytics/Schema.sql),
  applied idempotently on every connection open. Edit it directly — there's nothing
  generated and nothing to overwrite your changes. See
  [ADR 0009](docs/decisions/0009-drop-mariadb.md).
- **Aggregation lives in DuckDB macros.** One macro per board in
  [`Leaderboards.sql`](dotnet/TF2SA.Data/Analytics/Leaderboards.sql)/
  [`PlayerDetail.sql`](dotnet/TF2SA.Data/Analytics/PlayerDetail.sql) — no per-class/
  per-window files to keep in sync. See
  [analytics-store.md](docs/architecture/analytics-store.md).
- If you change ingested fields, update
  [docs/stats.md](docs/stats.md) (the logs.tf → DB field mapping) and the
  hand-written mapping in `LogIngestionRepositoryUpdater`.

### Frontend
- **Formatting:** Prettier — `cd frontend && npm run format`.
- **Atomic design:** components are organized as `molecules/`, `organisms/`, and
  `pages/`. New pages also need routes in `routes.ts` and `app-routing.module.ts`.

## Testing

- Backend: xUnit + Moq + AutoFixture in `TF2SA.Tests.Unit`. Run with
  `dotnet test dotnet/TF2SA.Tests.Unit/TF2SA.Tests.Unit.csproj`.
- Frontend: Karma/Jasmine — `cd frontend && npm test`.
- Add tests for new behavior, especially query handlers and ingestion logic (current
  gaps are listed in [known-issues.md](docs/known-issues.md)).

## Documentation expectations

- Each architecture doc has a **"When to update this document"** section — honor it.
  If your change matches a trigger, update the doc in the same PR.
- Record material design decisions as a new ADR in
  [docs/decisions/](docs/decisions/). If a decision reverses an existing one,
  supersede it rather than editing history.
- Keep the AI-assistant files ([AGENTS.md](AGENTS.md), [CLAUDE.md](CLAUDE.md),
  [.github/copilot-instructions.md](.github/copilot-instructions.md)) thin — they
  point at the canonical docs; put durable knowledge in `docs/`.
- If you fix something listed in [known-issues.md](docs/known-issues.md), remove it
  from that file.

## Commit messages

Write clear, imperative-mood messages describing intent. Reference the issue number
where relevant.
