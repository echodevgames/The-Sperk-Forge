
---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-09 — Chronicle Slot Rename, Full-State Duplication, Stable Identity, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-09
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.32.0
**Decision:** ESV-D-031
**Planning baseline:** `07bbd2b`
**Planning/activation commit:** `7d2d987`
**Implementation commit:** `459023f`
**Unity baseline:** 6000.3.8f1
**Prior focused regression floor:** **540 / 540**
**Final focused Chronicle Editor gate:** **562 / 562**, `0` failed
**Net new focused tests:** **22**
**Committed implementation/test scope:** **26 files**, `3100` insertions, `8` deletions

## Outcome

ESV-M4-09 completes Chronicle's bounded non-destructive slot rename and full-state duplication slice under ESV-D-031.

Chronicle now exposes:

```text
RenameSlotAsync(SaveSlotRenameRequest)
DuplicateSlotAsync(SaveSlotDuplicateRequest)
```

Both operations reuse the existing root-local mutation admission authority. Overlap returns Busy immediately and neither operation creates a hidden queue.

## Rename truth

Rename:
- targets one existing canonical healthy slot;
- preserves the exact `SaveSlotId`;
- preserves the physical slot directory/path;
- never edits already committed manifest or payload files in place;
- verifies the current source generation before mutation;
- revalidates bound source provenance before publication;
- publishes the display-name change through one new immutable generation;
- preserves verified payload/state equivalence;
- publishes `head.json` last;
- runs existing M4-06 retention after durable publication;
- refreshes the M4-01 catalog afterward;
- preserves active-slot identity when the renamed slot is active;
- reports durable rename commit separately from retention/catalog maintenance truth.

The durable rename boundary is successful new-head publication. A later retention or catalog failure cannot fabricate rollback.

## Duplicate truth

Duplicate:
- targets one canonical healthy source slot;
- refreshes catalog/capacity truth before destination mutation;
- applies canonical M4-02 slot-count capacity semantics;
- creates a new package-generated `SaveSlotId`;
- uses bounded collision retry;
- creates a new `SaveGenerationId`;
- copies the fully verified current source state without participant callbacks;
- keeps source slot and committed source generation bytes unchanged;
- revalidates the exact bound source before destination publication;
- publishes the destination immutable generation and then destination `head.json` last;
- reconciles the catalog afterward;
- never auto-selects the duplicate.

The durable duplicate boundary is successful destination-head publication. A later catalog failure reports a committed-but-unreconciled duplicate rather than fictional rollback.

## ESV-T-019 / ESV-T-020

The registry-aligned M4-09 proofs are now implemented:

- **ESV-T-019 — Rename slot:** display metadata changes while technical slot identity/path remain stable.
- **ESV-T-020 — Duplicate slot:** a new technical slot identity receives state equivalent to the fully verified source.

Focused coverage additionally proves:
- missing/degraded source rejection before mutation;
- stale-source revalidation rejection;
- source byte preservation;
- rename retention bounds;
- duplicate capacity rejection with no destination mutation;
- new destination slot/generation identity;
- duplicate no-auto-select;
- Busy / ServiceNotReady / AdmissionClosed lifecycle truth;
- catalog-failure committed truth;
- zero participant callbacks.

## Boundary preservation

M4-09 did not change the base `ISaveStorageBackend` contract and did not add:
- direct filesystem authority to the slot-mutation core;
- participant capture/apply/default/migration authority;
- delete/trash APIs;
- generic operation queues;
- automatic recovery;
- automatic autosave timers;
- scene travel;
- service-locator behavior;
- Chronicle-owned/project-wide DDOL.

## Closeout

ESV-M4-09 is **complete** at implementation commit `459023f`.

Final focused evidence is **562 / 562 passed, 0 failed**, preserving the prior **540 / 540** Chronicle regression floor.

No follow-on M4 checkpoint is activated by this closeout.
