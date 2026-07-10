# GitHub Copilot Instructions

This project keeps its guidance for AI assistants **vendor-neutral**. See
[AGENTS.md](../AGENTS.md) for how to work in this repository — conventions, common
tasks, and links to the canonical documentation in [`docs/`](../docs/README.md).

Key rules that most affect generated code (details in AGENTS.md):

- Errors are returned as values (`EitherStrict`/`OptionStrict`), not thrown across
  boundaries.
- The database is the source of truth; EF Core entities are **scaffolded** — never
  hand-edit them. Aggregation logic lives in SQL stored procedures.
- The read side is CQRS-lite via MediatR (query + handler + result + thin
  controller).
- Format C# with CSharpier; the backend builds with warnings-as-errors.
