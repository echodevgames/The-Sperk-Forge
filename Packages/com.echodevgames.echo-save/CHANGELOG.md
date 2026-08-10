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

### Added

#### ESV-M3-07 — Participant Migration Contracts, Duplicate-Safe Registry, Contiguous-Chain Execution, and Migrated Payload Preparation Foundation

- Stable `SaveParticipantMigrationId`.
- Public explicit participant migration-step contract.
- Structured migration input/output contracts.
- Duplicate-safe runtime migration registration and ownership leases.
- Canonical participant/from-version edge authority.
- Deterministic migration registry snapshot/order.
- Exact contiguous one-version chain planning.
- Positive migration-depth bound.
- Missing-edge failure before migration execution.
- In-memory migration executor with registry ownership recheck.
- Exact target-version, serializer-ID, and migrated-payload validation.
- Ordered stable migration provenance without payload contents.
- Persisted alias → current canonical migration routing.
- Older-known-payload integration into M3-06 trusted DTO preparation.
- All-or-nothing mixed current/migrated participant preparation.

### Verified

#### ESV-M3-07 Closeout

- Implementation committed at `d96936f`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **294 / 294**, with `0` failed.
- All prior **261 / 261** Chronicle regressions remained green.
- 33 new focused M3-07 migration tests passed.
- Migration invokes neither participant `Capture` nor participant `Apply`.
- Source immutable generations remain untouched.

### Added

#### ESV-M3-08 — Prepared-Load Handle Lifecycle and Session Ownership Foundation

- Public opaque sealed/disposable `PreparedSaveLoad`.
- Safe immutable source/lifetime/count metadata.
- Structured prepared-load creation/admission status/result.
- Injected UTC clock seam.
- Runtime-memory-only prepared-load owner/store.
- Exact read/preparation/unknown source-provenance agreement.
- Defensive opaque unknown snapshot ownership.
- Package-internal prepared DTO access only while live.
- Owner token + session epoch isolation.
- Idempotent disposal and deterministic expiry.
- Owner/session invalidate-all.
- Positive live-handle count and aggregate source-byte limits.
- Deterministic capacity release.

### Verified

#### ESV-M3-08 Closeout

- Implementation committed at `798d38d`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **332 / 332**, with `0` failed.
- All prior **294 / 294** Chronicle regressions remained green.
- 38 new focused M3-08 tests passed.
- Participant `Capture` and `Apply` remain unused.
- Storage/publication mutation and scene/DDOL authority remain absent.

### Approved

#### ESV-M3-09 default initialization capability

- Add optional `ISaveDefaultableParticipant.InitializeDefault()`.
- Keep the base `ISaveParticipant` contract unchanged.
- `SaveMissingPayloadPolicy.InitializeDefault` requires the optional capability.
- `Ignore` skips/reports.
- `Fail` blocks during preflight.
- `Apply(null)` is not a hidden default-initialization protocol.

### Added

#### ESV-M3-09 — Deterministic Participant Apply and Missing-Payload Policy Foundation

- Optional public `ISaveDefaultableParticipant.InitializeDefault()` capability.
- Unchanged base `ISaveParticipant` contract.
- Explicit prepared-state/default/ignore apply action kinds.
- Complete zero-callback deterministic apply preflight.
- Current registration owner/token resolution and revalidation.
- Prepared schema/runtime-type compatibility validation.
- Explicit `InitializeDefault` / `Ignore` / `Fail` missing-payload behavior.
- Prepared `Apply(detachedState)` and explicit `InitializeDefault()` execution.
- Payload-free ordered apply reports.
- Structured participant failure/exception conversion.
- Accurate not-attempted tail reporting after terminal failure.
- Live-handle retry after pure preflight rejection.
- Terminal `Consumed` prepared-load state once execution begins.
- Replay rejection after consumption.

### Verified

#### ESV-M3-09 Closeout

- Implementation committed at `568fa3a`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **366 / 366**, with `0` failed.
- All prior **332 / 332** Chronicle regressions remained green.
- 34 new focused M3-09 tests passed.
- Default initialization never routes through `Apply(null)`.
- Source save files remain unchanged by apply.
- Scene/DDOL authority remains absent.
- **M3 — Participants and Loading is complete.**

### Activated

#### ESV-M4-01 — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation

- Provider-neutral technical slot discovery through an additive optional storage capability.
- Base `ISaveStorageBackend` remains unchanged.
- Payload-free metadata reconstruction from authoritative `head.json` + current `manifest.json`.
- Deterministic immutable in-memory catalog snapshots.
- Healthy/degraded slot classification.
- Session-only active-slot selection and stale-selection reconciliation.
- Persistent `catalog.cache.json`, physical slot mutation, autosave, retention, and recovery remain deferred.


### Verified

#### ESV-M4-01 Closeout

- Implementation committed at `62e8a54`.
- Unity compile/import green after one test-only NUnit constraint compile hotfix.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **403 / 403**, with `0` failed.
- All prior **366 / 366** Chronicle regressions remained green.
- 37 new focused M4-01 catalog/discovery/session tests passed.
- Base `ISaveStorageBackend` remained unchanged.
- Catalog refresh reconstructs metadata from provider-neutral discovery plus `head.json` and current `manifest.json`.
- Normal catalog refresh reads zero `payload.json` files.
- Valid technical but unhealthy slots remain degraded/non-selectable instead of disappearing.
- Untrustworthy overall refresh failure preserves the prior complete immutable snapshot.
- Active slot selection is session-only, explicit, non-durable, and never auto-selects.
- No participant callbacks, persistent catalog cache, physical slot mutation, autosave, retention, recovery, scene authority, or DDOL ownership were introduced.

### Activated

#### ESV-M4-02 — Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation

- Add a bounded technical slot-creation coordinator on top of the proven M4-01 catalog.
- Count every discovered canonical technical slot, including degraded entries, against the creation capacity bound.
- Generate a fresh canonical `SaveSlotId` with bounded collision retry.
- Publish a real initial empty immutable generation and `head.json` last rather than treating a directory alone as a created slot.
- Keep display names and project/build metadata as manifest metadata only; never use display names as paths.
- Refresh the catalog after successful publication without auto-selecting the new slot.
- Report truthful publication-versus-refresh outcomes if publication succeeds but catalog reconciliation fails.
- Persistent cache, rename, duplicate, delete, full slot-policy asset expansion, production operation admission, autosave, retention, and recovery remain deferred.

### Verified
#### ESV-M4-02 Closeout

- Implementation committed at `d8d5c18`.
- Unity compile/import green after narrow test-only NUnit accessibility/discovery repairs.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **425 / 425**, with `0` failed.
- All prior **403 / 403** Chronicle regressions remained green; M4-02 added **22** net focused tests.
- Technical creation requires a trustworthy fresh catalog before durable mutation.
- Every discovered canonical technical slot, healthy or degraded, counts against the positive capacity bound; invalid non-slot children remain excluded.
- The normal path generates canonical `SaveSlotId` values independently from display/project/build metadata and retries collisions only within a positive bound.
- Initial creation publishes a real empty immutable generation through candidate verification, immutable publication, final verification, and `head.json` last.
- Create-specific publication rejects an existing current head inside the publication transaction rather than silently becoming an update/save path.
- Post-publication catalog reconciliation returns healthy created metadata on success and truthfully reports published-but-reconciliation-failed state without deleting a committed slot.
- Successful creation does not auto-select the new slot.
- No participant callbacks, persistent catalog cache, rename/duplicate/delete, full slot-policy asset expansion, production operation admission, autosave, retention, recovery, document migration, scene travel, peer bridge, or Chronicle-owned DDOL authority was introduced.
- The final-verification failure test explicitly preserves `generationPublished = true` once immutable generation publication has already succeeded; `head.json` remains unpublished in that failure case.

### Added

#### ESV-M4-03 — Manual Save Transaction Composition, Unknown Carry-Forward, and Catalog Reconciliation Foundation

- Internal bounded manual-save transaction request/status/result/coordinator.
- Explicit selected-active-slot and healthy-catalog preflight.
- Current-generation validation with exact source slot/generation provenance refresh.
- Fresh deterministic known participant capture.
- Opaque unknown-payload carry-forward through existing collision-safe ownership rules.
- Expected-current-generation stale-source rejection.
- Participant-backed immutable generation publication with `head.json` last.
- Ordinary-save display-name preservation.
- Post-publication catalog reconciliation.
- Truthful generation/head/catalog partial-result reporting.
- Focused M4-03 transaction tests.
- Public `SaveAsync`, production operation admission/Busy/cancellation, autosave, retention, recovery, persistent catalog cache, rename/duplicate/delete, full slot-policy assets, scene travel, bridges, and DDOL remain deferred.

### Verified

#### ESV-M4-03 Closeout

- Planning/activation committed at `2c325e9`.
- Implementation committed at `c8ea742`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **439 / 439**, with `0` failed.
- All prior **425 / 425** Chronicle regressions remained green.
- M4-03 added **14** net focused tests.
- Successful manual save preserves valid opaque unknown payloads and current display name while advancing one verified participant-backed generation.
- Stale source, ownership collision, provenance mismatch, participant-capture failure, and pre-head publication failures do not fabricate current-head success.
- Durable head success followed by catalog-refresh failure remains reported as committed durable truth rather than fictional rollback.
- Participant Apply/default callbacks remain absent.
