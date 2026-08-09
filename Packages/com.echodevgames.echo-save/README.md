# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M2-02 — Document Contracts and Unity JSON Serializer Foundation** with a focused Chronicle Editor gate of **57 / 57**. The active checkpoint is **ESV-M2-03 — Generation Identity, Integrity, and Commit-Document Foundation**.

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

ESV-M2-01 provides path-safe local byte storage beneath the configured Chronicle root. ESV-M2-02 adds package-owned in-memory document/version contracts and the default Unity JSON serializer.

ESV-M2-03 may now add stable slot/generation technical IDs, manifest/payload/head commit-document contracts, and SHA-256 integrity primitives. It still does **not** authorize physical immutable generation publication, head mutation, slot/catalog operations, participant state capture/apply, migration/recovery/autosave, prepared loads, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
