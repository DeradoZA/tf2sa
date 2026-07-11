# Onboarding & Local Development

> Everything you need to build, run, and test TF2SA locally. For *why* the system
> is shaped the way it is, read [architecture/overview.md](./architecture/overview.md).

## Prerequisites

| Tool | Version | Used for |
|------|---------|----------|
| Docker + Docker Compose | recent | Running the full stack the way production does |
| .NET SDK | 10.0.x | Building/testing the backend directly |
| Node.js | 18.x | Building/serving the Angular frontend |
| Angular CLI | 15.x (`npm i -g @angular/cli`) | Frontend dev server |
| MariaDB | 10.6 | Database (via Docker or local) |
| CSharpier | matches `dotnet/.csharpierrc` | Formatting C# (`dotnet tool`/IDE) |

## Environment variables

The backend reads all secrets/connection details from environment variables (see
`TF2SA.Data/DependencyInjection.cs` and `TF2SA.Http/DependencyInjection.cs`). For
Docker Compose, put them in a `.env` file next to `docker-compose.yml`.

| Variable | Required | Example / notes |
|----------|----------|-----------------|
| `TF2SA_MYSQL_HOST` | yes | `tf2sa-db` (compose) or `localhost` |
| `TF2SA_MYSQL_USR` | yes | app DB user |
| `TF2SA_MYSQL_PWD` | yes | app DB password |
| `TF2SA_MYSQL_DB` | yes | `tf2sa_db` (matches `USE tf2sa_db;` in the schema) |
| `TF2SA_STEAM_API_KEY` | yes for player enrichment | Steam Web API key |

Example `.env`:

```dotenv
TF2SA_MYSQL_HOST=tf2sa-db
TF2SA_MYSQL_USR=tf2sa
TF2SA_MYSQL_PWD=changeme
TF2SA_MYSQL_DB=tf2sa_db
TF2SA_STEAM_API_KEY=your-steam-web-api-key
```

> Ingestion is **off by default** in `TF2SA.StatsETLService/appsettings.json`
> (`EnableIngestion: false`) and **on** in `TF2SA.Web/appsettings.json`. Adjust for
> local work so you don't hammer the logs.tf API unintentionally — see
> [ingestion-pipeline.md](./architecture/ingestion-pipeline.md#configuration).

## Option A — run the whole stack with Docker Compose

Mirrors production most closely (nginx, TLS aside).

```bash
# from the repo root, with a .env file present
docker compose build
docker compose up -d
docker compose logs -f tf2sa-backend
```

The database container seeds its schema and stored procedures automatically on a
fresh volume from `scripts/mariadb/*.sql`.

## Option B — run backend and frontend directly (tighter dev loop)

**Database** — start just the DB via compose, or point at a local MariaDB with the
schema applied:

```bash
docker compose up -d tf2sa-db
```

**Backend** (API + co-hosted ETL):

```bash
cd dotnet
dotnet restore
dotnet build /warnaserror           # CI builds with warnings-as-errors
dotnet run --project TF2SA.Web      # serves the API + Swagger UI
```

Swagger UI is enabled in all environments. The dev frontend expects the API at
`https://localhost:5001/api/v1` (`frontend/src/environments/environment.development.ts`).

**Frontend**:

```bash
cd frontend
npm ci
npm start            # ng serve → http://localhost:4200
```

`http://localhost:4200` is already in the backend CORS allowlist
(`TF2SA.Web/Program.cs`).

## Building

| Target | Command |
|--------|---------|
| Backend | `dotnet build dotnet/ /warnaserror` |
| Frontend | `cd frontend && npm run build` |
| Docker images | `docker compose build` |

## Testing

```bash
# backend unit tests (xUnit) with coverage (lcov), as CI runs them
dotnet test dotnet/TF2SA.Tests.Unit/TF2SA.Tests.Unit.csproj \
  -p:CollectCoverage=True -p:CoverletOutput=TestResults/ -p:CoverletOutputFormat=lcov

# frontend unit tests (Karma/Jasmine)
cd frontend && npm test
```

Current backend tests focus on serialization, validation, the HTTP client, and log
query params. See [known-issues.md](./known-issues.md) for coverage gaps.

## Formatting

- **C#:** CSharpier, configured in [`dotnet/.csharpierrc`](../dotnet/.csharpierrc)
  (tabs, print width 80). Run before committing; CI's `/warnaserror` build will also
  reject warnings.
- **Frontend:** Prettier — `cd frontend && npm run format`.

## Common tasks (recipes)

| Task | Where to work | Notes |
|------|---------------|-------|
| Add a read endpoint | `TF2SA.Query` (query + handler + result) → `TF2SA.Web` (controller action + HTTP result) | Follow [ADR 0003](./decisions/0003-cqrs-read-side-with-mediatr.md) and [request-flow.md](./architecture/request-flow.md). |
| Change how a stat is computed | `scripts/mariadb/update_*_stats.sql` | Recent **and** all-time are separate files; keep in sync. See [database.md](./architecture/database.md). |
| Add/change a table or column | `scripts/mariadb/*.sql`, then re-run `TF2SA.Data/scaffold.sh` | DB-first; never hand-edit scaffolded entities. See [ADR 0004](./decisions/0004-database-first-ef-scaffolding.md). |
| Change an ingested field | `scripts/mariadb`, `TF2SA.Common` models, validators, `LogIngestionRepositoryUpdater`; update [`docs/stats.md`](./stats.md) | logs.tf field mapping is documented in `stats.md`. |
| Add a community uploader | `LogsTF.Uploaders` in `appsettings.json` | Config + redeploy. See [ADR 0007](./decisions/0007-curated-uploader-allowlist.md). |
| Add a frontend page | `frontend/src/app/components/pages/...` + `routes.ts` + `app-routing.module.ts` | Atomic-design layout (molecules/organisms/pages). |

## When to update this document

Update this document when the local setup changes: new prerequisites or versions,
new/renamed environment variables, changed build/test/run commands, or a new common
task worth a recipe.
