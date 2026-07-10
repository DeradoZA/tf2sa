# 3. CQRS read side with MediatR

- **Status:** Accepted
- **Date:** 2026-07-05 (documented retroactively)

## Context

The API surface is a set of read endpoints (per-class and overall leaderboards,
players), each needing similar but distinct paging, sorting, and filtering. We
wanted controllers to stay thin and each read to be independently testable, without
controllers accumulating data-access logic.

## Decision

The read side follows a **CQRS-lite** pattern using **MediatR**:

- Each read is a **query** object (e.g. `GetScoutRecentQuery`) handled by exactly
  one **handler** (`GetScoutRecentQueryHandler`).
- Controllers only bind HTTP parameters, `mediator.Send(query)`, and map the result
  to an HTTP result model.
- Handlers build the queryable, apply filter/sort/paging via the repository, project
  entities to domain models with AutoMapper, and return
  `EitherStrict<Error, ...>`.

MediatR is registered in `TF2SA.Query/DependencyInjection.cs` (assembly scan). The
name "CQRS" is aspirational here: writes do **not** go through MediatR commands —
the ingestion pipeline writes directly through repositories. This is a deliberate
read-side-only application of the pattern.

## Consequences

- Controllers are trivial and uniform; adding an endpoint is a predictable recipe
  (query + handler + result model + controller action).
- Each query handler is unit-testable in isolation.
- There is boilerplate: every leaderboard needs its own query/handler/result trio.
  This is accepted for clarity and consistency.
- Because only reads use MediatR, don't assume a symmetric command bus exists on the
  write side — see [ingestion-pipeline.md](../architecture/ingestion-pipeline.md).
