# 7. Curated uploader allowlist for ingestion

- **Status:** Accepted
- **Date:** 2026-07-05 (documented retroactively)

## Context

logs.tf hosts logs from the entire global TF2 community. TF2SA is specifically a
**South African** community stats platform. Ingesting all of logs.tf would be a huge
volume of irrelevant data and would dilute leaderboards with players outside the
community the site serves.

## Decision

Ingestion is scoped to a **fixed allowlist of uploader Steam IDs** representing the
SA community, configured as `LogsTF.Uploaders` in `appsettings.json`. The pipeline
fetches logs per uploader (`LogsTFService.GetAllLogs`) and processes only those.

## Consequences

- Ingestion volume and leaderboards are naturally scoped to the intended community.
- Membership is a **configuration** concern: adding or removing a community member's
  uploads means editing the `Uploaders` list, not code — but it does require a
  config change and redeploy, and there is no self-service/admin UI.
- Relevant games uploaded by someone **not** on the allowlist are missed; conversely
  the list must be curated to stay accurate as the community changes.
- The allowlist is public in the repository (Steam IDs are not secret), which is
  acceptable for this data.
