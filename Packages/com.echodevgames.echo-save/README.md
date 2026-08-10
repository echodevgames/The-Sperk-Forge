# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation** with a focused Chronicle Editor gate of **218 / 218**. The active checkpoint is **ESV-M3-05 — Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation**.

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

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, SHA-256 integrity, verified immutable generation publication, and head-last current selection.

ESV-M3-01 adds an open-ended participant registry. ESV-M3-02 adds trusted detached capture and verified in-memory participant transport batches. ESV-M3-03 joins those batches to the established generation-first/head-last durable transaction. ESV-M3-04 adds read-only current-generation validation plus opaque session preservation for unclaimed participant payloads.

ESV-M3-05 may now bind unknown snapshots to their source slot/generation, prove that source is still current, merge fresh known captures with still-unclaimed unknown payloads, fail closed on canonical/alias ownership collisions, and carry preserved unknown participant payload bytes/metadata into the next immutable generation.

It still does **not** authorize silent unknown pruning, automatic collision winners, participant deserialization/apply, prepared loads, migrations, production `SaveAsync` admission/coalescing/cancellation, concurrent save ownership, slot/catalog behavior, recovery/retention/autosave, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
