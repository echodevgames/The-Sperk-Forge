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

### Added

#### ESV-M2-03 — Generation Identity, Integrity, and Commit-Document Foundation

- Canonical package-generated `SaveSlotId`.
- Unique sortable `SaveGenerationId`.
- `SaveManifest`, `SavePayloadDocument`, `SavePayloadEntry`, `SavePayloadInventoryEntry`, and `SaveHeadPointer`.
- Explicit package document kinds and independent document versions.
- Structured commit-document agreement validation.
- Replaceable `IIntegrityProvider`.
- Default `Sha256IntegrityProvider`.
- Focused technical-ID, integrity, and commit-document tests.

### Verified

#### ESV-M2-03 Closeout

- Implementation committed at `ad3b646`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **87 / 87**, with `0` failed.
- M2-03 performs no physical generation publication and does not mutate `head.json`.
- All prior Chronicle lifecycle, storage, path-safety, and serializer regressions remained green.

### Added

#### ESV-M2-04 — Immutable Generation Publication and Head-Last Commit Foundation

- Optional provider-neutral storage publication capability seam.
- Explicit publication capability description without universal atomicity claims.
- Local same-root candidate-to-final directory publication.
- Local small-current-object temp + move/replace publication.
- Package generation storage-key construction.
- Bounded package-owned empty/transport generation publication coordinator.
- Generation-first / head-last transaction behavior.
- Previous-known-good head preservation across injected pre-head failures.
- Verified orphan-generation behavior when head publication fails.
- Sandboxed publication and failure-injection tests.

### Verified

#### ESV-M2-04 Closeout

- Implementation committed at `01b7ad3`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **102 / 102**, with `0` failed.
- All prior **87 / 87** Chronicle regressions remained green.
- Published generations remain create-only and duplicate generation publication is rejected.
- A failed final head publication does not silently make an orphaned generation current.
- The local backend explicitly does not claim universal power-loss atomicity.
