# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed **ESV-M3-02 — Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation** with a focused Chronicle Editor gate of **171 / 171**. The active checkpoint is **ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation**.

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

ESV-M3-01 adds an open-ended participant contract and duplicate-safe deterministic runtime registry. ESV-M3-02 adds trusted live-code runtime DTO type routing, deterministic detached capture, serializer selection, exact UTF-8 participant byte lengths, per-entry integrity checksums, and all-or-nothing in-memory participant transport batches.

ESV-M3-03 may now join only a successful verified participant batch to the existing M2 generation-first/head-last transaction. The publication boundary must revalidate participant entry/inventory structure and inline payload integrity before any candidate write.

It still does **not** authorize production `SaveAsync`, request admission/coalescing/cancellation, autosave, participant apply/load orchestration, unknown-payload preservation, migrations, slot/catalog behavior, recovery/retention, prepared loads, project-wide `DontDestroyOnLoad` composition, or peer-package bridges.

Those capabilities remain governed by later Chronicle checkpoints.

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
