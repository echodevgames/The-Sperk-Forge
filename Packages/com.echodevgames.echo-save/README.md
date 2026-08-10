# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-09 — Deterministic Participant Apply and Missing-Payload Policy Foundation** with a focused Chronicle Editor gate of **366 / 366**. **M3 — Participants and Loading is complete.** The active checkpoint is **ESV-M4-01 — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation**.

ESV-M1-01 proved:

- the package installs without another Echo package;
- one `EchoSaveRoot` claims package-local Chronicle authority;
- a duplicate root loses before Chronicle initialization side effects;
- configuration can be assigned as a project-owned `ScriptableObject`;
- initialization and shutdown have explicit structured results;
- shutdown releases authority so a later valid root may claim;
- neutral storage/serializer/clock seams compile;
- focused tests can prove the lifecycle without touching real durable storage.

## Current persistence boundary

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, integrity, immutable generation publication, and head-last current selection.

M3 is complete. It provides open-ended participant registration, detached capture, participant-backed publication, opaque unknown preservation/carry-forward, trusted current/migrated participant preparation, bounded prepared-load lifetime/session ownership, and deterministic ownership-revalidated participant application with explicit `InitializeDefault` / `Ignore` / `Fail` semantics.

M3-09 is complete at `568fa3a` with **366 / 366** focused Chronicle Editor tests. Default initialization is the additive optional `ISaveDefaultableParticipant.InitializeDefault()` capability; base `ISaveParticipant` remains unchanged and `Apply(null)` is not protocol.

ESV-M4-01 begins the slot/catalog milestone. It may add provider-neutral technical slot discovery, payload-free lightweight metadata reconstruction from authoritative heads/current manifests, deterministic immutable catalog snapshots, and session-only active-slot selection.

It still does **not** authorize persistent catalog-cache optimization, physical slot creation/rename/duplicate/delete, production save/load operation admission, autosave, retention, recovery, document migration, scene travel, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
