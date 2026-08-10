# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M4-01 — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation** with a focused Chronicle Editor gate of **403 / 403**. **M3 — Participants and Loading is complete.** **M4 — Slots / Autosave / Recovery is active.** The active checkpoint is **ESV-M4-02 — Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation**.

## Current persistence boundary

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, integrity, immutable generation publication, and head-last current selection.

M3 is complete. It provides open-ended participant registration, detached capture, participant-backed publication, opaque unknown preservation/carry-forward, trusted current/migrated participant preparation, bounded prepared-load lifetime/session ownership, and deterministic ownership-revalidated participant application with explicit `InitializeDefault` / `Ignore` / `Fail` semantics.

M4-01 is complete at `62e8a54` with **403 / 403** focused Chronicle Editor tests. It adds provider-neutral bounded technical slot discovery, payload-free `head.json` + current `manifest.json` metadata reconstruction, healthy/degraded immutable catalog snapshots, prior-snapshot preservation on untrustworthy refresh failure, and session-only active-slot selection.

ESV-M4-02 may now add bounded technical slot creation and capacity enforcement. A successfully created slot must own one real verified empty immutable generation selected through `head.json` last; directory existence alone is not successful creation. Display names remain manifest metadata, discovered degraded technical slots still count against capacity, creation does not auto-select, and publication/catalog-reconciliation outcomes remain truthful.

It still does **not** authorize persistent catalog-cache optimization, rename/duplicate/delete, full slot-policy configuration expansion, production operation admission/coalescing/cancellation, autosave, retention, recovery, document migration, scene travel, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
