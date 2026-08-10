# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-08 — Prepared-Load Handle Lifecycle and Session Ownership Foundation** with a focused Chronicle Editor gate of **332 / 332**. The active checkpoint is **ESV-M3-09 — Deterministic Participant Apply and Missing-Payload Policy Foundation**.

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

M3-01 through M3-05 provide open-ended participant registration, detached capture, participant generation publication, opaque unknown preservation, and source-fresh collision-safe unknown carry-forward.

M3-06 prepares fully validated current-schema known participant payloads into detached DTO batches.

M3-07 adds explicit duplicate-safe participant migration steps, complete contiguous in-memory migration-chain execution, migration provenance, and older-known-payload integration back into current-version preparation.

M3-08 encapsulates one exact-source validated/migrated prepared load in a bounded public opaque disposable `PreparedSaveLoad` with package/session ownership, deterministic expiry/disposal/invalidation, defensive unknown snapshot retention, and live-handle resource bounds.

ESV-M3-09 may now perform complete apply preflight and deterministic current-registration participant mutation. Missing payloads use explicit `InitializeDefault`, `Ignore`, or `Fail` semantics. Default initialization uses the approved additive optional `ISaveDefaultableParticipant.InitializeDefault()` capability; the base `ISaveParticipant` contract remains unchanged and `Apply(null)` is never a protocol signal.

It still does **not** authorize rollback/compensation fiction, production async operation admission, convenience loading, scene travel, document migration, slot catalog behavior, recovery/retention/autosave, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
