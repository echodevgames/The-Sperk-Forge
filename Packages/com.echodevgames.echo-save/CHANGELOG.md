# Changelog

All notable changes to The Chronicle — Save Infrastructure will be documented here.

## [Unreleased]

### Added

#### ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim

- Initial `com.echodevgames.echo-save` package shell.
- Project-owned `EchoSaveConfiguration` schema version 1.
- Package-local `EchoSaveRoot` authority claim.
- Duplicate rejection before Chronicle service construction or initialization.
- Explicit initialize and shutdown lifecycle.
- Structured lifecycle result/status contracts and stable M1 diagnostics.
- Neutral storage, serializer, and clock provider identity seams.
- Test-only lifecycle probe seam with no real storage implementation.
- Focused Editor tests for authority, duplicate safety, initialization, shutdown, re-claim, and zero-storage-side-effect proof.
- No durable save I/O, serializer implementation, slot/generation behavior, peer bridge, or project-wide DDOL composition.

### Verified

#### ESV-M1-01 Closeout

- Unity compile/import reported green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate reported all green.
- Exact numeric test count was not captured, so no count is claimed.
- Implementation committed at `ecfa922`.
- Embedded Package Manager resolution committed at `2c70b1d`.
- No real durable save I/O was introduced by M1.

### Added

#### ESV-M2-01 — Storage Root, Path Safety, and Local Backend Foundation

- Safe relative `SaveStorageKey` validation and separator normalization.
- Root/path containment checks that reject traversal and physical root escapes.
- Configured production root resolution beneath `Application.persistentDataPath`.
- Injectable storage-backend factory seam for sandbox tests.
- Replaceable `LocalFileSaveStorageBackend`.
- Structured storage results and byte-read results.
- Initialize, exists, exact-byte read, create-only write, delete, and shutdown primitives.
- Duplicate-before-storage-side-effect lifecycle coverage.
- Sandboxed path/backend/lifecycle Editor tests.

### Verified

#### ESV-M2-01 Closeout

- Implementation committed at `e4ef76c`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **40 / 40**, with `0` failed.
- Initial `29 / 40` run exposed an EditMode lifecycle-test activation assumption; the narrow test seam was corrected and the final rerun passed.
- No save-document serializer, slot catalog, immutable generation publication, participant persistence, recovery/autosave, peer bridge, or Chronicle-owned DDOL behavior was added.

### Added

#### ESV-M2-02 — Document Contracts and Unity JSON Serializer Foundation

- Package-owned document identity/version contracts.
- `SaveDocumentEnvelope`.
- Structured serializer result/status contracts and diagnostics.
- Replaceable in-memory `ISaveSerializer` operations.
- Package-local `SaveSerializerRegistry`.
- Default package-owned `UnityJsonSaveSerializer` using Unity `JsonUtility`.
- Focused registry, DTO/envelope round-trip, malformed-input, and unsupported-version tests.

### Verified

#### ESV-M2-02 Closeout

- Implementation committed at `6404037`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **57 / 57**, with `0` failed.
- Serializer/document work performs no filesystem I/O and does not publish generations or mutate heads.
- All prior Chronicle lifecycle, path-safety, and local-backend regressions remained green.
