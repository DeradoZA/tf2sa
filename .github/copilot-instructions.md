# GitHub Copilot Instructions

This project keeps its guidance for AI assistants **vendor-neutral**. See
[AGENTS.md](../AGENTS.md) for how to work in this repository — conventions, common
tasks, and links to the canonical documentation in [`docs/`](../docs/README.md).

Key rules that most affect generated code (details in AGENTS.md):

- Errors are returned as values (`EitherStrict`/`OptionStrict`), not thrown across
  boundaries.
- DuckDB is the only database and there's no ORM — the schema is hand-maintained SQL
  (`TF2SA.Data/Analytics/Schema.sql`); aggregation logic lives in DuckDB macros
  (`Leaderboards.sql`/`PlayerDetail.sql`).
- The read side is CQRS-lite via direct DI dispatch — no mediator library (query +
  handler + result + thin controller).
- Format C# with CSharpier; the backend builds with warnings-as-errors.
