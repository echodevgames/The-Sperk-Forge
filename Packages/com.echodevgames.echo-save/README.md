# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-05 — Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation** with a focused Chronicle Editor gate of **243 / 243**. The active checkpoint is **ESV-M3-06 — Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation**.

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

M3-01 through M3-05 now provide open-ended participant registration, detached capture, participant generation publication, opaque unknown preservation, and source-fresh collision-safe unknown carry-forward.

ESV-M3-06 may prepare **current-version known participant payloads** into detached runtime DTOs using trusted live registration type authority and already-registered serializer providers.

It still does **not** authorize participant migrations, participant apply, `PreparedSaveLoad`, production operation admission, slots, recovery/retention/autosave, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Unknown payloads remain opaque and are never deserialized by the preparation path.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
