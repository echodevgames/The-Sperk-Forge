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

### Added

#### ESV-M3-01 — Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation

- Canonical `SaveParticipantId`.
- Required/Optional participant criticality.
- InitializeDefault/Ignore/Fail missing-payload policy.
- Validated `SaveParticipantDescriptor` with bounded prior-ID aliases.
- Public open-ended `ISaveParticipant` capture/apply-facing contract.
- Participant capture/apply result contracts.
- Structured registration status/result.
- Disposable/idempotent `SaveParticipantRegistration`.
- Immutable participant registry snapshots.
- Duplicate-safe deterministic `SaveParticipantRegistry`.
- Canonical/alias collision rejection.
- Stale-registration ownership-token protection.
- Explicit proof that a future participant can register without a Chronicle compile-time catalog.

### Verified

#### ESV-M3-01 Closeout

- Implementation committed at `b3b5f9f`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **147 / 147**, with `0` failed.
- All prior **102 / 102** Chronicle regressions remained green.
- Registry operations perform no durable I/O and do not invoke participant capture/apply.
- Chronicle core contains no predefined participant catalog.

### Added

#### ESV-M3-02 — Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation

- Optional `IRuntimeTypeSaveSerializer` capability.
- Optional `ISaveTypedParticipant` capability.
- Trusted live-code runtime DTO type routing for Unity JSON.
- Deterministic participant capture coordinator.
- Explicit/default serializer-provider resolution.
- Detached DTO null/type/live-Unity-object validation.
- Exact UTF-8 participant payload byte-length calculation.
- Per-entry integrity checksum calculation.
- `SavePayloadEntry` and matching `SavePayloadInventoryEntry` construction.
- All-or-nothing capture-batch failure semantics.
- Defensive-copy capture-batch results.
- Future-participant same-pipeline proof.

### Verified

#### ESV-M3-02 Closeout

- Implementation committed at `e34d6d7`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **171 / 171**, with `0` failed.
- All prior **147 / 147** Chronicle regressions remained green.
- Save data cannot request CLR type activation.
- Capture/type/serializer/integrity failure never exposes a publishable partial participant batch.
- Capture performs no storage or generation/head mutation.

### Added

#### ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation

- Publication-boundary participant-batch revalidation.
- Duplicate/order/schema/serializer/flags validation.
- Exact inline UTF-8 byte-length and checksum verification before storage.
- Payload/inventory agreement revalidation.
- Participant-backed generation publication entry point.
- Shared M2/M3 generation-first/head-last transaction core.
- Participant-bearing payload and manifest construction.
- Stored participant-entry revalidation after candidate and final reads.
- Participant-backed first/second generation publication tests.
- Injected candidate/generation/final-verification/head failure proofs.
- Zero-storage-mutation proof for invalid participant batches.

### Verified

#### ESV-M3-03 Closeout

- Implementation committed at `6970127`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **197 / 197**, with `0` failed.
- All prior **171 / 171** Chronicle regressions remained green.
- Candidate/generation/reverification failures preserve the previous known-good head.
- Head serialization/publication failures leave the new generation non-current/orphaned.
- Existing M2 empty/transport publication remains green.
- One Editor-test accessibility hotfix was required before the final gate; no runtime behavior changed.

### Added

#### ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation

- Read-only current head/generation resolution.
- Current immutable payload/manifest read and complete structural/integrity validation.
- Canonical and alias participant recognition.
- Package-owned opaque `SaveUnknownPayloadStore`.
- Field-for-field unknown transport preservation.
- Exact serialized payload text preservation.
- Defensive-copy deterministic unknown snapshots.
- Bounded unknown count and aggregate-byte safeguards.
- Atomic successful store replacement.
- Prior valid store preservation across failed read/classification.

### Verified

#### ESV-M3-04 Closeout

- Implementation committed at `aa78e07`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **218 / 218**, with `0` failed.
- All prior **197 / 197** Chronicle regressions remained green.
- Unknown payload classification resolves no serializer and invokes no participant capture/apply.
- Current-generation inspection performs zero storage mutation.
- Failed reads/classification preserve the previous valid unknown store.

### Added

#### ESV-M3-05 — Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation

- Unknown snapshot source slot/generation provenance.
- Atomic provenance refresh/preservation/reset behavior.
- Stale-source preflight before publication mutation.
- Canonical/alias ownership collision fail-closed checks.
- Deterministic fresh-known + opaque-unknown merge.
- Exact unknown payload-body and transport-metadata carry-forward.
- In-transaction source freshness recheck.
- Merged immutable generation publication through candidate/verify/publish/reverify/head-last.

### Verified

#### ESV-M3-05 Closeout

- Implementation committed at `af28c96`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **243 / 243**, with `0` failed.
- All prior **218 / 218** Chronicle regressions remained green.

### Added

#### ESV-M3-06 — Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation

- Fully validated current-generation participant snapshot exposure.
- Defensive-copy participant transport entries with source slot/generation provenance.
- Canonical/alias live participant owner resolution.
- Persisted-ID plus current canonical-owner provenance.
- Trusted live detached DTO `Type` authority.
- Current-schema-only participant preparation.
- Structured migration-required result for older schemas.
- Structured unsupported-newer result for newer schemas.
- Already-registered runtime-Type serializer resolution.
- Unknown-payload skip before serializer lookup.
- Deterministic all-or-nothing prepared participant batches.

### Verified

#### ESV-M3-06 Closeout

- Implementation committed at `050bfa0`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **261 / 261**, with `0` failed.
- All prior **243 / 243** Chronicle regressions remained green.
- 18 new focused M3-06 preparation tests passed.
