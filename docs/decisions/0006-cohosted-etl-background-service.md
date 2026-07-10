# 6. Co-hosted ETL background service

- **Status:** Accepted (with known caveats)
- **Date:** 2026-07-05 (documented retroactively)

## Context

The ingestion pipeline needs to run periodically. Options included a separate worker
container/process, an external scheduler (cron, a queue), or running it inside the
existing web application. The project is small, single-host, and operated by a
tiny team, so operational simplicity was weighted heavily.

## Decision

The ETL pipeline runs **in-process inside the `tf2sa-backend` (`TF2SA.Web`)
application** as an ASP.NET Core `BackgroundService` (`StatsETLServiceRunner`),
registered via `AddStatsETLService()` → `AddHostedService()` in the web composition
root. There is a self-scheduling loop (`IngestionIntervalSeconds`) rather than an
external scheduler.

The `TF2SA.StatsETLService` project also contains a **standalone Worker
`Program.cs`** (a generic `Host`), but the deployed `Dockerfile.tf2sa-backend`
publishes `TF2SA.Web` only, so that standalone entrypoint is **not deployed**.

## Consequences

- One process and one image to build, deploy, and operate; ingestion and API share
  configuration and DI wiring.
- The API and ingestion share process resources; a heavy ingestion cycle competes
  with request serving (bounded somewhat by `MAX_CONCURRENT_HTTP_THREADS = 2`).
- Ingestion cannot be scaled or restarted independently of the API, and running
  multiple backend instances would run multiple ingestion loops.
- The unused standalone worker entrypoint is latent confusion — a reader may assume
  ETL is a separate service. This, and the eventual option to split it out, is noted
  in [known-issues.md](../known-issues.md). If it is ever extracted into its own
  container/host, supersede this ADR and update
  [deployment.md](../architecture/deployment.md) and
  [ingestion-pipeline.md](../architecture/ingestion-pipeline.md).
