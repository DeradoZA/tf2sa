# 2. Functional error handling with monads

- **Status:** Accepted
- **Date:** 2026-07-05 (documented retroactively)

## Context

The backend does a lot of I/O that can fail in expected ways: HTTP calls to logs.tf
and Steam, JSON deserialization, database reads/writes, and validation. Using
exceptions for these *expected* failure modes makes control flow implicit, easy to
forget, and awkward to compose (nested try/catch), and it blurs the line between
"this operation failed" and "the program is broken."

## Decision

Expected failures are represented as **values**, using the `csharp-monad` library:

- `EitherStrict<Error, T>` for operations that return a value **or** an error.
  Callers check `IsLeft` / `IsRight` and read `.Left` / `.Right`.
- `OptionStrict<Error>` for operations that either fail **or** produce nothing to
  return. Callers check `HasValue`.

`Error` (in `TF2SA.Common/Errors`) is the base type, specialized as `HttpError`,
`DatabaseError`, `SerializationError`, `IngestionError`, etc. Errors propagate up
by being returned and inspected, not thrown, across layer boundaries (HTTP client
→ services → repositories → handlers → controllers, and the ingestion pipeline).

## Consequences

- Failure handling is explicit and visible in method signatures; the happy path and
  error path are both first-class.
- Controllers translate a `Left` error into an HTTP `400`; the ingestion loop logs
  and continues past individual failures without aborting a whole cycle.
- Contributors must follow the pattern rather than reaching for exceptions at
  boundaries; new code that throws for expected failures is inconsistent with the
  codebase.
- Some legacy repository methods still use try/catch internally and do not yet fully
  thread `CancellationToken`s or return monads — noted as a TODO in code and in
  [known-issues.md](../known-issues.md).
