# 1. Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-07-05

## Context

TF2SA has accumulated several non-obvious design choices (functional error
handling, database-first ORM, aggregation in SQL, a co-hosted ETL worker) whose
rationale lived only in the maintainers' heads and in scattered pull requests. New
contributors — human or AI — repeatedly have to reverse-engineer *why* the code is
shaped the way it is, which is slow and risks well-meaning changes that violate an
unstated intent.

## Decision

We keep a lightweight log of **Architecture Decision Records (ADRs)** in
`docs/decisions/`, one Markdown file per significant decision, numbered
sequentially. Each ADR captures the context, the decision, and its consequences at
the time it was made.

We use the format popularized by Michael Nygard: short, immutable once accepted. A
decision that is later reversed gets a **new** ADR that supersedes the old one; the
old ADR stays as history (its status updated to "Superseded by ADR-XXXX") rather
than being deleted or edited.

## Consequences

- The "why" behind structural choices is durable and reviewable.
- Reviewers can point to an ADR instead of re-explaining a convention.
- There is a small ongoing cost: material decisions should be accompanied by an ADR
  in the same change. If a decision turns out wrong, we supersede rather than
  silently drift.
