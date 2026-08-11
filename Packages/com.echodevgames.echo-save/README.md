# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M4-06 — Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation**. Planning/activation is committed at `3d8e0b8`, implementation is committed at `e714a90`, and the final focused Chronicle Editor gate is **497 / 497**. **M3 — Participants and Loading is complete.** **M4 — Slots / Autosave / Recovery remains active**, but no follow-on M4 checkpoint is automatically activated by this closeout.

## Current persistence boundary

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, integrity, immutable generation publication, and head-last current selection.

M3 is complete. It provides open-ended participant registration, detached capture, participant-backed publication, opaque unknown preservation/carry-forward, trusted current/migrated participant preparation, bounded prepared-load lifetime/session ownership, and deterministic ownership-revalidated participant application with explicit `InitializeDefault` / `Ignore` / `Fail` semantics.

M4-01 adds provider-neutral bounded technical slot discovery, payload-free `head.json` + current `manifest.json` metadata reconstruction, healthy/degraded immutable catalog snapshots, prior-snapshot preservation on untrustworthy refresh failure, and session-only active-slot selection.

M4-02 adds bounded technical slot creation and capacity enforcement. A successful new slot owns one real verified empty immutable generation selected through `head.json` last. Every discovered canonical technical slot, including degraded entries, counts against capacity. Package-generated `SaveSlotId` identity remains independent from display/project/build metadata. Creation does not auto-select. If publication succeeds but catalog reconciliation fails, Chronicle reports the durable publication truth instead of deleting the committed slot or pretending rollback.

M4-03 composes the complete internal manual-save transaction for an explicitly selected healthy slot. It validates the exact current source generation, captures fresh known participants, carries valid opaque unknown payloads forward, rejects stale source/ownership collisions, publishes one participant-backed immutable generation with `head.json` last, preserves ordinary display-name metadata, and reconciles the slot catalog truthfully.

M4-04 exposes that proven transaction through public active-slot `SaveAsync`, adds one root-local mutating-operation admission authority, returns Busy immediately for overlapping manual saves without queueing, honors safe pre-publication cancellation, reports Too Late after durable publication begins, and closes new admission during shutdown while preserving truthful committed outcomes.

M4-05 adds explicit caller-triggered `RequestAutosave`, retains at most one pending latest autosave while admission is occupied, supersedes older pending metadata instead of growing a queue, preserves manual-save Busy semantics, drains at most the latest pending autosave after admission release, and prevents pending work from starting after shutdown admission closure. Chronicle still does not decide when gameplay should autosave.

M4-06 adds bounded total committed-generation retention after successful publication. It discovers history through provider-neutral storage capability, protects the current generation and immediate recovery predecessor, deletes only excess verified committed history oldest-first through optional tree-deletion capability, and reports cleanup maintenance separately from committed save truth.

It still does **not** include persistent catalog-cache optimization, rename/duplicate/delete/trash, full slot-policy configuration expansion, automatic autosave timers, generic queued multi-operation scheduling, recovery execution/quarantine, document migration, scene travel, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
