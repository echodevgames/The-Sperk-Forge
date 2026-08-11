# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M4-10 — Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation**. Planning/activation is committed at `2244e3c`, implementation is committed at `01e4cdd`, and the final focused Chronicle Editor gate is **587 / 587**. **M3 — Participants and Loading is complete.** **M4 — Slots / Autosave / Recovery remains active pending a dedicated milestone reconciliation.** No M5 checkpoint is activated by this closeout.

## Current persistence boundary

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, integrity, immutable generation publication, and head-last current selection.

M3 is complete. It provides open-ended participant registration, detached capture, participant-backed publication, opaque unknown preservation/carry-forward, trusted current/migrated participant preparation, bounded prepared-load lifetime/session ownership, and deterministic ownership-revalidated participant application with explicit `InitializeDefault` / `Ignore` / `Fail` semantics.

M4-01 adds provider-neutral bounded technical slot discovery, payload-free `head.json` + current `manifest.json` metadata reconstruction, healthy/degraded immutable catalog snapshots, prior-snapshot preservation on untrustworthy refresh failure, and session-only active-slot selection.

M4-02 adds bounded technical slot creation and capacity enforcement. A successful new slot owns one real verified empty immutable generation selected through `head.json` last. Every discovered canonical technical slot, including degraded entries, counts against capacity. Package-generated `SaveSlotId` identity remains independent from display/project/build metadata. Creation does not auto-select. If publication succeeds but catalog reconciliation fails, Chronicle reports the durable publication truth instead of deleting the committed slot or pretending rollback.

M4-03 composes the complete internal manual-save transaction for an explicitly selected healthy slot. It validates the exact current source generation, captures fresh known participants, carries valid opaque unknown payloads forward, rejects stale source/ownership collisions, publishes one participant-backed immutable generation with `head.json` last, preserves ordinary display-name metadata, and reconciles the slot catalog truthfully.

M4-04 exposes that proven transaction through public active-slot `SaveAsync`, adds one root-local mutating-operation admission authority, returns Busy immediately for overlapping manual saves without queueing, honors safe pre-publication cancellation, reports Too Late after durable publication begins, and closes new admission during shutdown while preserving truthful committed outcomes.

M4-05 adds explicit caller-triggered `RequestAutosave`, retains at most one pending latest autosave while admission is occupied, supersedes older pending metadata instead of growing a queue, preserves manual-save Busy semantics, drains at most the latest pending autosave after admission release, and prevents pending work from starting after shutdown admission closure. Chronicle still does not decide when gameplay should autosave.

M4-06 adds bounded total committed-generation retention after successful publication. It discovers history through provider-neutral storage capability, protects the current generation and immediate recovery predecessor, deletes only excess verified committed history oldest-first through optional tree-deletion capability, and reports cleanup maintenance separately from committed save truth.

M4-07 adds read-only recovery planning. It classifies missing/invalid/broken-current head state, discovers retained generations through bounded provider-neutral reads, fully verifies candidate documents and integrity, preserves/excludes untrustworthy evidence, orders valid candidates newest-first deterministically, returns immutable payload-free plan/candidate truth, and fingerprints exact technical source provenance for later stale-plan rejection without mutating storage.

M4-08 adds explicit recovery execution. It reuses root-local mutation admission, rebuilds and provenance-checks the M4-07 plan before mutation, requires the selected candidate to remain fully verified, republishes only `head.json`, preserves immutable generation bytes, and reports post-head catalog reconciliation without fabricated rollback.

M4-09 adds non-destructive public slot rename and full-state duplication. Rename preserves technical slot identity/path and commits display metadata through a new immutable generation with source-freshness protection, retention, and catalog reconciliation. Duplicate enforces canonical slot capacity, creates new package-generated slot/generation identities, copies only a fully verified current source state without participant callbacks, revalidates the source before publication, publishes head last, preserves source bytes, and does not auto-select the duplicate.

M4-10 adds read-only deletion planning and admitted confirmed recoverable trash. Deletion plans bind package/session/source provenance, expire, reject replay, and perform zero mutation until explicit confirmation. Confirm-delete reuses root-local admission, revalidates exact source truth, moves the complete live slot tree into recoverable `trash/`, clears active selection only after durable removal, reconciles the live catalog, and applies bounded fail-closed trash retention. The final focused gate is **587 / 587**.

It still does **not** include automatic/configured fallback, recovery-on-load, quarantine/incomplete-generation cleanup, persistent catalog-cache optimization, permanent erase, public restore-from-trash, full slot-policy/recovery configuration expansion, automatic autosave timers, generic queued multi-operation scheduling, document migration, scene travel, project-wide `DontDestroyOnLoad` composition, peer-package bridges, or M5 Editor tooling/Laboratory qualification.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
