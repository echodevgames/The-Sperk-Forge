# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim**. The active checkpoint is **ESV-M2-01 — Storage Root, Path Safety, and Local Backend Foundation**.

ESV-M1-01 proved:

- the package installs without another Echo package;
- one `EchoSaveRoot` claims package-local Chronicle authority;
- a duplicate root loses before Chronicle initialization side effects;
- configuration can be assigned as a project-owned `ScriptableObject`;
- initialization and shutdown have explicit structured results;
- shutdown releases authority so a later valid root may claim;
- neutral storage/serializer/clock seams compile;
- focused tests can prove the lifecycle without touching real durable storage.

## Not implemented yet

ESV-M1-01 does **not** write or read save files. ESV-M2-01 may introduce storage-root resolution, path validation, and local-backend primitives, but it still does not authorize Chronicle save documents, slots, generations, participant payloads, migrations, recovery, autosaves, prepared loads, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
