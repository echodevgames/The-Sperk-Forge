# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M2-04 — Immutable Generation Publication and Head-Last Commit Foundation** with a focused Chronicle Editor gate of **102 / 102**. Chronicle **M2 — Document / Storage Core** is complete for its bounded implementation path. The active checkpoint is **ESV-M3-01 — Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation**.

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

ESV-M2-01 provides path-safe local byte storage beneath the configured Chronicle root. ESV-M2-02 adds package-owned in-memory document/version contracts and the default Unity JSON serializer. ESV-M2-03 adds stable slot/generation technical IDs, manifest/payload/head commit-document contracts, and SHA-256 integrity primitives. ESV-M2-04 adds verified immutable-generation publication and head-last selection while preserving the previous known-good head across failure.

ESV-M3-01 may now define participant identity, descriptors, registration lifetime, and duplicate-safe deterministic registry behavior. It still does **not** authorize participant capture/save orchestration, participant apply/load orchestration, unknown-payload preservation, migrations, slot/catalog behavior, recovery/retention/autosave, prepared loads, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
