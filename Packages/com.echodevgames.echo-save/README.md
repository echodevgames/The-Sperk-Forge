# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation** with a focused Chronicle Editor gate of **197 / 197**. The active checkpoint is **ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation**.

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

ESV-M3-01 adds an open-ended participant registry. ESV-M3-02 adds trusted detached capture and verified in-memory participant transport batches. ESV-M3-03 joins those batches to the established generation-first/head-last durable transaction and proves participant-bearing generations across injected failures.

ESV-M3-04 may now read and fully validate the current committed generation and preserve any unclaimed participant entries as opaque in-memory `UnknownPayloadStore` data. Unknown entries remain byte-for-byte / field-for-field durable records and cannot execute code.

It still does **not** authorize unknown-payload merge/carry-forward publication, prune plans, production `SaveAsync`, participant deserialization/apply, prepared loads, migrations, slot/catalog behavior, recovery/retention/autosave, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
