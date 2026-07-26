# Request Flow (Read Path)

> How a user request for leaderboard/player data travels from the browser to the
> database and back. For the write side see
> [ingestion-pipeline.md](./ingestion-pipeline.md).

## Overview

The read path is a thin, layered CQRS-lite pipeline. Controllers do no business
logic — they bind HTTP into a query object, call its handler directly, and map the
result into an HTTP response. Serving a leaderboard is a **parametrized aggregation**
run on the fly in the embedded **DuckDB store** (columnar, fast enough to aggregate
per request — see [analytics-store.md](./analytics-store.md)); the class,
time-window ("recent" vs "all-time"), and min-games threshold are query parameters.

Every read endpoint, including the player list, now queries DuckDB — it is the only
database ([ADR 0009](../decisions/0009-drop-mariadb.md)). There is no dispatch
library (MediatR was removed — [ADR 0003](../decisions/0003-cqrs-read-side-with-mediatr.md)'s
"Update" note) and no mapping library (AutoMapper was removed — hand-written
extension methods do the row→domain and domain→HTTP conversions).

```mermaid
flowchart LR
    browser["Angular SPA<br/><i>StatsService / PlayersService</i>"]
    nginx["nginx<br/><i>reverse proxy</i>"]
    controller["Controller<br/><i>StatisticsController / PlayersController</i>"]
    handler["Query Handler<br/><i>Get*QueryHandler</i><br/>(injected directly)"]
    store["IAnalyticsStore<br/><i>DuckDB-backed</i>"]
    duck[("DuckDB<br/>the only database")]

    browser -->|"GET /api/v1/... (HTTPS/JSON)"| nginx
    nginx -->|"/api → backend:80"| controller
    controller -->|"handler.Handle(query, ct)"| handler
    handler --> store
    store -->|"parametrized aggregation SQL"| duck
    duck -.->|rows| store
    store -.->|row POCOs| handler
    handler -.->|"ToDomain()"| controller
    controller -.->|"ToHttpResult()"| browser
```

## Sequence: fetch Scout "recent" leaderboard

```mermaid
sequenceDiagram
    autonumber
    participant UI as Angular (StatsService)
    participant NG as nginx
    participant C as StatisticsController
    participant H as GetScoutRecentQueryHandler
    participant S as IAnalyticsStore (DuckDB)

    UI->>NG: GET /api/v1/Statistics/ScoutRecent?count&offset&sort&sortOrder&filterField&filterValue
    NG->>C: proxy /api to backend
    C->>H: handler.Handle(query, ct)
    Note over H,S: class=scout, windowStart=now−365d (recent), minGames=threshold
    H->>S: GetPageAsync<ScoutAllTime>(boardCall, whitelisted sort/filter, offset, count)
    S->>S: GROUP BY SteamID … HAVING games ≥ minGames, arg_max top games, LIMIT/OFFSET
    S-->>H: StatsPage<ScoutAllTime> (rows this page + total)
    H->>H: rows.Select(r => r.ToDomain())
    H-->>C: EitherStrict<Error, GetScoutStatsResult>
    alt result.IsLeft
        C-->>UI: 400 Bad Request (error message)
    else success
        C->>C: result.Right.ToHttpResult()
        C-->>UI: 200 OK (JSON)
    end
```

## Layer responsibilities

| Layer | Type(s) | Responsibility |
|-------|---------|----------------|
| Browser | `StatsService`, `PlayersService` (Angular) | Build query params (page size, offset, sort, filter, stat period), call the API, feed Material tables. |
| Reverse proxy | nginx (`scripts/iac/nginx/nginx.conf`) | Serve SPA static files; proxy `/api` to `tf2sa-backend:80`. |
| Controller | `StatisticsController.*`, `PlayersController` | Bind query-string params (with defaults), call the query handler directly (injected via DI), map the result to an HTTP result model via a hand-written `ToHttpResult()`, return `200` or `400`. No business logic. |
| Query handler | `Get*QueryHandler` | Pass class/period/threshold + paging/sort/filter to `IAnalyticsStore`, project rows to domain models via `ToDomain()`, wrap in `EitherStrict<Error, ...>`. |
| Store | `IAnalyticsStore` / `DuckDbAnalyticsStore` | Build the **parametrized aggregation SQL**; provide **whitelisted** sort/filter (request keys → fixed column names, safe defaults); never interpolate raw input. Also the write path — see [ingestion-pipeline.md](./ingestion-pipeline.md). |
| Data | Embedded DuckDB | Columnar, vectorized aggregation over the raw tables, per request — the only database. See [analytics-store.md](./analytics-store.md). |

## API surface

Routes are versioned under `v1` and proxied beneath `/api` (nginx) with an
additional `/api` path base configured in the app (`app.UsePathBase("/api")`), so
the browser-visible prefix is `/api/v1`.

The endpoint URLs are unchanged; the `Recent` / `AllTime` suffix now selects a
**time-window parameter** on the same fact-table query, not a distinct table.

| Endpoint (browser-visible) | Controller action | Backing query |
|----------------------------|-------------------|---------------|
| `GET /api/v1/Statistics/OverallRecent` / `OverallAllTime` | `StatisticsController.Overall` | fact table, all classes (with per-class carve-outs), window = recent / all-time |
| `GET /api/v1/Statistics/ScoutRecent` / `ScoutAllTime` | `StatisticsController.Scout` | fact table, `ClassID = 1`, window = recent / all-time |
| `GET /api/v1/Statistics/SoldierRecent` / `SoldierAllTime` | `StatisticsController.Soldier` | fact table, `ClassID = 2`, window = recent / all-time |
| `GET /api/v1/Statistics/DemomanRecent` / `DemomanAllTime` | `StatisticsController.Demoman` | fact table, `ClassID = 4`, window = recent / all-time |
| `GET /api/v1/Statistics/MedicRecent` / `MedicAllTime` | `StatisticsController.Medic` | fact table, `ClassID = 7`, window = recent / all-time |
| `GET /api/v1/Players` | `PlayersController` | raw `Players` table (DuckDB, `GetPlayersPageAsync`) |

`StatisticsController` is a **partial class** split across one file per class
(`StatisticsController.Scout.cs`, `.Soldier.cs`, etc.), sharing the constructor and
`[Route("v1/[controller]")]` defined in `StatisticsController.cs`. Swagger UI is
available in all environments (`app.UseSwagger()` / `UseSwaggerUI()`).

## Common query parameters

Statistics endpoints accept: `count` (page size), `offset`, `sort`, `sortOrder`
(`asc`/`desc`), `filterField`, `filterValue`. Defaults live on the controller
actions (e.g. `count=13`, `sort=averageDpm`, `sortOrder=desc`). Invalid `sort` or
`filterField` values fall back to safe defaults in the stats repository rather than
erroring, and only whitelisted properties are sortable/filterable — a small but
important guard against arbitrary column access (and, now that queries are built as
SQL, against injection).

Because aggregation is now dynamic, the **time-window** (recent vs all-time) and the
**min-games threshold** are parameters rather than fixed schema. Today the URL suffix
still encodes the window; exposing an arbitrary window or threshold as an explicit
query parameter is a natural, non-breaking extension.

## When to update this document

Update this document when:

- A new read endpoint or controller is added, or routing/versioning changes.
- The query pipeline changes (e.g. sorting/filtering/paging semantics, or a new
  layer is introduced between controller and repository).
- The mapping strategy between row → domain → HTTP result changes.
- The API path base or nginx proxy path changes.
- The stats read layer moves off DuckDB, or the aggregation query/parameter model
  changes (also update [analytics-store.md](./analytics-store.md)).
