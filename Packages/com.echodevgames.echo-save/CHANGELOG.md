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


### Added

#### ESV-M4-04 — Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation

- Public active-slot `SaveRequest` / `SaveOperationResult`.
- Additive `IEchoSaveService.SaveAsync(...)`.
- One root-local mutating-operation admission authority.
- Immediate Busy rejection for overlapping manual saves with no hidden queue.
- Safe pre-publication cancellation checks.
- Too-Late cancellation truth once durable publication begins.
- Shutdown closure of new manual-save admission before backend shutdown.
- Faithful public mapping of M4-03 generation/head/catalog truth.
- Main-thread public completion.
- Focused public-save, admission, cancellation, shutdown, and result-mapping tests.
- Autosave/coalescing, generic queued multi-operation scheduling, retention/recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy assets, scene travel, peer bridges, and DDOL remain deferred.

### Verified

#### ESV-M4-04 Closeout

- Planning/activation committed at `91dcb62`.
- Implementation committed at `2732aaa`.
- Bounded pre-Ready lifecycle-status hotfix committed at `09ae8f1`.
- Final effective runtime baseline is `09ae8f1`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **456 / 456**, with `0` failed.
- All prior **439 / 439** Chronicle regressions remained green.
- M4-04 adds **17** net focused tests over the prior floor.
- The initial focused run had one failure: pre-Ready save returned `AdmissionClosed` rather than `ServiceNotReady`.
- Two patch helpers refused safely and changed nothing; the final v3 hotfix changed only `EchoSaveService.cs`.
- Overlapping manual saves return Busy immediately and do not queue.
- Safe pre-publication cancellation cannot advance the head.
- Cancellation after durable publication begins reports Too Late without pretending rollback.
- Shutdown closes new admission and preserves settlement of an already-committing operation.
- No autosave/coalescing, generic queued scheduler, retention/recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy expansion, scene/bridge/DDOL scope was introduced.


### Added

#### ESV-M4-05 — Autosave Request Coalescing and Latest-Wins Pending Admission Foundation

- Public caller-triggered `AutosaveRequest`.
- Additive `IEchoSaveService.RequestAutosave(...)`.
- Bounded autosave submission/result/ticket truth.
- Exactly zero-or-one pending latest autosave request.
- Latest-wins coalescing/supersession instead of an unbounded queue.
- Reuse of the M4-04 root-local mutating-operation admission authority.
- Reuse of the M4-03/M4-04 durable active-slot save path.
- Preservation of manual-save Busy behavior.
- At-most-once pending drain after admission release.
- Shutdown rejection/discard semantics that prevent pending work from starting after admission closure.
- Focused autosave submission, coalescing, drain, shutdown, and durable-result tests.
- No automatic autosave timer, generic operation queue, retention/recovery, rename/duplicate/delete, persistent catalog cache, scene, bridge, or DDOL ownership.

### Verified

#### ESV-M4-05 Closeout

- Planning/activation committed at `8504ed4`.
- Implementation committed at `9917f1b`.
- Final effective runtime baseline is `9917f1b`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **473 / 473**, with `0` failed.
- All prior **456 / 456** Chronicle regressions remained green after one authorized regression-test maintenance update.
- M4-05 adds **17** net focused tests over the prior floor.
- Final committed implementation/test scope is **22 files**, including the updated M4-04 public-service surface test.
- The stale M4-04 assertion that `RequestAutosave` must be absent was replaced with a bounded signature/return-type assertion because M4-05 explicitly authorizes that API.
- Autosave remains caller-triggered; Chronicle does not own gameplay timing rules.
- At most one pending latest autosave is retained.
- Manual save remains Busy rather than queued.
- Retention, generic queued operation scheduling, recovery, destructive slot operations, persistent catalog cache, and broader slot-policy work remain deferred.


### Added

#### ESV-M4-06 — Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation

- Project-owned bounded `SaveRetentionPolicy`.
- Minimum safe total-generation retention of two.
- Provider-neutral bounded generation-directory discovery.
- Additive optional `ISaveStorageTreeDeletionBackend`.
- Unchanged base `ISaveStorageBackend`.
- Fail-closed canonical committed-generation classification.
- Protection of current and immediate predecessor generations.
- Deterministic oldest-first excess-history deletion.
- Retention maintenance only after successful generation/head publication.
- Shared manual-save and autosave retention path.
- Public `SaveOperationResult.RetentionResult` maintenance truth.
- Focused policy, provider, coordinator, manual/autosave integration, failure, and boundary tests.

### Verified

#### ESV-M4-06 Closeout

- Planning/activation committed at `3d8e0b8`.
- Implementation committed at `e714a90`.
- Final effective runtime baseline is `e714a90`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **497 / 497**, with `0` failed.
- All prior **473 / 473** Chronicle regressions remained green.
- M4-06 adds **24** net focused tests over the prior floor.
- Final committed implementation/test scope is **33 files**, `2136` insertions and `12` deletions.
- Initial focused run was **495 / 497** because two new manual-retention integration tests registered no participant and therefore failed before reaching publication/retention.
- The third injected-publication-failure integration test also stopped early and passed for the wrong reason.
- One test-only setup correction registered a normal participant in all three integration cases; runtime implementation, API, architecture, authority, and discovery count did not change.
- Final rerun passed **497 / 497**.
- Recovery execution/quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred.


### Added

#### ESV-M4-07 — Recovery Candidate Discovery, Immutable Recovery Plan Truth, and Deterministic Fallback Selection Foundation

- Public read-only `BuildRecoveryPlanAsync(SaveSlotId)`.
- Immutable payload-free `SaveRecoveryPlan` and recovery-candidate summaries.
- Explicit healthy/missing/unreadable/invalid/current-missing/current-invalid source-state truth.
- Bounded provider-neutral generation discovery.
- Full manifest/payload/package-version/slot-generation/integrity/committed-state candidate verification.
- Preservation and exclusion of unsupported, malformed, incomplete, mismatched, uncommitted, corrupt, and noncanonical evidence.
- Deterministic newest-valid candidate ordering with generation-ID tie-break.
- Preferred fallback candidate only when recovery is actually required.
- Technical source-provenance fingerprint for later stale-plan execution rejection.
- Zero durable mutation during planning.
- Focused recovery-planning, public-service, failure, provenance, preservation, and boundary tests.

### Verified

#### ESV-M4-07 Closeout

- Planning/activation committed at `7b00503`.
- Implementation committed at `9f68555`.
- Final effective runtime baseline is `9f68555`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **524 / 524**, with `0` failed.
- All prior **497 / 497** Chronicle regressions remained green.
- M4-07 adds **27** net focused tests over the prior floor.
- Final committed implementation/test scope is **22 files**, `2912` insertions and `6` deletions.
- One compile-only test fixture correction changed `SaveDocumentVersions.HeadMajor` to the authoritative `HeadPointerMajor` constant.
- The first focused run passed **522 / 524**; two unsupported-document fixture tests failed before the recovery planner ran because Chronicle's production serializer correctly rejected intentionally unsupported package-document versions.
- The fixture was corrected so supported documents continue through Chronicle's production serializer while intentionally future-version JSON is authored directly with Unity `JsonUtility` for runtime preserve/exclude testing.
- Runtime implementation, public API, architecture, ESV-D-029 authority, recovery behavior, test intent, and discovery count remained unchanged by both fixture corrections.
- Final rerun passed **524 / 524**.
- Recovery execution/head rewrite/catalog reconciliation, automatic fallback, quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred.


### Added
#### ESV-M4-08 — Explicit Recovery Execution, Stale-Plan Revalidation, Head Repointing, and Catalog Reconciliation Foundation
- Public `ExecuteRecoveryAsync(SaveRecoveryPlan, SaveRecoveryCandidate)`.
- Bounded `SaveRecoveryResult` / `SaveRecoveryExecutionStatus` truth.
- Reuse of the root-local mutating-operation admission authority.
- Immediate Busy rejection with no hidden recovery queue.
- Fresh M4-07 plan rebuild after admission.
- Exact source-provenance stale-plan rejection before mutation.
- Exact selected-candidate membership/revalidation.
- `head.json`-only recovery publication to an already verified immutable committed generation.
- Recovery-created `previousGenerationId` remains empty rather than blessing damaged source history.
- Post-head catalog reconciliation.
- Truthful committed-head / catalog-unreconciled partial result.
- Focused coordinator and public-service recovery-execution tests.

### Verified
#### ESV-M4-08 Closeout
- Planning/activation committed at `c324aa4`.
- Implementation committed at `1985fb0`.
- Final effective runtime baseline is `1985fb0`.
- Unity compile/import green after one test-only call-counter compile correction.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **540 / 540**, with `0` failed.
- All prior **524 / 524** Chronicle regressions remained green.
- M4-08 adds **16** net focused tests over the prior floor.
- Final committed implementation/test scope is **18 files**, `1846` insertions and `10` deletions.
- Stale plans and invalid/no-longer-valid candidates reject before head mutation.
- Successful recovery republishes only `head.json`; selected generation bytes remain unchanged.
- Catalog failure after a successful head publication reports recovery committed without fabricating rollback.
- Active-slot selection is preserved and participant callbacks remain absent.
- The compile-only correction changed two new test references from `FakeManualSaveTransactionExecutor.CallCount` to the existing `Calls` property; runtime/API/architecture/authority/test intent/discovery shape were unchanged.
- Automatic/configured fallback, recovery-on-load, quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader recovery/configuration tooling remain deferred.

### Added
#### ESV-M4-09 — Slot Rename, Full-State Duplication, Stable Identity, and Catalog Reconciliation Foundation
- Public bounded `SaveSlotRenameRequest` / `SaveSlotRenameResult` / `SaveSlotRenameStatus`.
- Public bounded `SaveSlotDuplicateRequest` / `SaveSlotDuplicateResult` / `SaveSlotDuplicateStatus`.
- Additive `IEchoSaveService.RenameSlotAsync(...)` and `DuplicateSlotAsync(...)`.
- Reuse of the M4-04 root-local mutating-operation admission authority with immediate Busy rejection and no rename/duplicate queues.
- Rename preserves `SaveSlotId` and physical slot path while publishing display metadata through a new immutable generation.
- Rename reuses expected-current-generation stale-source protection, head-last publication, M4-06 retention maintenance, and M4-01 catalog reconciliation.
- Duplicate reuses canonical catalog-count capacity truth, bounded package-generated slot-ID collision retry, fully verified source-state cloning, source-provenance revalidation, new destination slot/generation identity, and head-last publication.
- Rename and duplicate perform no participant capture/apply/default/migration callbacks.
- Duplicate does not auto-select the newly created slot.
- Post-publication maintenance/catalog failures preserve durable committed truth instead of fabricating rollback.
- Focused ESV-T-019 / ESV-T-020, source-race, capacity, lifecycle/admission, active-slot, retention, and partial-result tests.

### Verified
#### ESV-M4-09 Closeout
- Planning/activation committed at `7d2d987`.
- Implementation committed at `459023f`.
- Final effective runtime baseline is `459023f`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **562 / 562**, with `0` failed.
- All prior **540 / 540** Chronicle regressions remained green.
- M4-09 adds **22** net focused tests over the prior floor.
- Final committed implementation/test scope is **26 files**, `3100` insertions and `8` deletions.
- The base `ISaveStorageBackend` contract remains unchanged.
- No M4-09 runtime/test hotfix was required after the implementation payload; the first reported focused gate was green.
- Prepare-delete/confirm-delete, trash/trash retention, quarantine/cleanup, persistent catalog cache, automatic/configured recovery fallback, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred.

### Added

#### ESV-M4-10 — Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation

- Public read-only `PrepareDeleteSlotAsync(SaveSlotId)`.
- Public admitted `ConfirmDeleteSlotAsync(SaveDeletionPlan)`.
- Immutable deletion plans bound to package/session authority and exact source provenance.
- Bounded five-minute default plan lifetime and one-use/replay protection.
- Payload-free head/current-manifest deletion-source provenance.
- Root-local Busy/no-queue destructive admission reuse.
- Fresh exact-source revalidation before destructive storage mutation.
- Recoverable package-owned trash as the durable safe-delete boundary.
- Active-slot clearing only after durable removal.
- Live catalog reconciliation after durable delete truth.
- Bounded deterministic oldest-first trash retention.
- Fail-closed trash-record classification.
- Truthful committed-but-unreconciled and committed-but-maintenance-failed result states.
- Focused ESV-T-021 / ESV-T-022 / ESV-T-023 coverage.
- Zero participant callbacks and unchanged base `ISaveStorageBackend`.

### Verified

#### ESV-M4-10 Closeout

- Planning/activation committed at `2244e3c`.
- Implementation committed at `01e4cdd`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **587 / 587**, with `0` failed.
- All prior **562 / 562** Chronicle regressions remained green.
- M4-10 adds **25** net focused tests over the prior floor.
- Final committed implementation/test scope is **28 files**, `2863` insertions and `6` deletions.
- Apply-side guards confirmed no one-step destructive public API, no direct filesystem authority in the deletion core, no participant callbacks, no scene/DDOL authority, and no base storage-contract widening.
- No M4-10 runtime/test hotfix was required after the implementation payload; the first reported focused gate was green.
- M4 is **not** declared complete by this checkpoint closeout. The next step is a dedicated M4 milestone reconciliation before M5 is activated.

### Added

#### ESV-M4-R1 — Public Runtime Composition and Consumer Facade Reconciliation

- Public participant registration through `IEchoSaveService`.
- Immutable public catalog snapshot access.
- Explicit public catalog refresh.
- Consumer-facing slot creation over the proven M4-02 technical creation path.
- Public session-only active-slot selection.
- Public `PrepareLoadAsync(SaveLoadRequest)`.
- Public `ApplyPreparedLoadAsync(PreparedSaveLoad)`.
- Public same-scene `LoadAndApplyAsync(SaveLoadRequest)`.
- Consumer-facing slot-create and load request/result/status truth.
- Existing participant registry, catalog, technical creation, current-generation read, participant preparation/migration, prepared-load lifetime, and apply authorities reused rather than duplicated.

### Verified

#### ESV-M4-R1 Closeout

- Planning/authority activation commit: `bdb0c00`.
- Implementation commit: `ab18361`.
- Unity compile/import green.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **618 / 618**, `0` failed.
- Prior focused Chronicle floor **587 / 587** remained green.
- Net new focused tests: **31**.
- Implementation/test scope: **29 files**, `2995` insertions, `18` deletions.
- Base `ISaveStorageBackend` remained unchanged.
- Base `ISaveParticipant` remained unchanged.
- `EchoSaveConfiguration` remained schema 1 for R2.
- R1 retained technical slot capacity `64`.
- No automatic recovery fallback, generic operation queue, scene travel, DDOL, or M5 tooling entered the facade.
- No R1 runtime/test hotfix was required after apply; the first reported focused run was green.

### Reconciliation state

- R1 is complete.
- R2 slot-policy runtime configuration remains the next M4 reconciliation gate but is **not automatically activated** by this closeout.
- R3 package-document migration remains required after R2.
- M5 remains locked until R2, R3, and final registry/document reconciliation are complete.

### Added
#### ESV-M4-R2 — Slot Policy Runtime Configuration and CAP-002 Reconciliation
- `EchoSaveConfiguration` schema 2 slot-policy authority.
- `SingleSlot`, `FixedMultiSlot`, `ConfigurableMultiSlot`, and `BoundedProfiles` finite effective capacities.
- Non-mutating schema-1 compatibility at historical capacity 64.
- One frozen service-session capacity shared by create and duplicate.
- ESV-T-015 through ESV-T-018 retained as Complete.

### Verified
#### ESV-M4-R2 Closeout
- Activation committed at `428369e`.
- Implementation committed at `8a8e7e7`.
- Closeout committed at `0ebf1a1`.
- Focused Chronicle Editor gate passed **636 / 636**, with `0` failed.
- **18** net new focused R2 tests.
- A-03 / CAP-002 closed.

### Added
#### ESV-M4-R3 — Package-Document Migration and CAP-014 Reconciliation
- Internal Chronicle-owned package-document version probe, migration registry, contiguous-chain coordinator, and migration-aware reader.
- Exact-current documents bypass migration.
- Historical migration is detached, read-time, in-memory, deterministic, and source-immutable.
- Missing, ambiguous, failed, invalid, overshooting, and newer paths fail closed.
- Participant migration remains a separate authority.
- Production package-document versions remain `1.0.0`.

### Verified
#### ESV-M4-R3 Closeout
- Activation committed at `2dcae91`.
- Implementation committed at `c6ba1ad`.
- Closeout committed at `e3d7a2e`.
- Focused Chronicle Editor gate passed **660 / 660**, with `0` failed.
- **24** net new focused R3 tests over the **636 / 636** floor.
- A-04 / CAP-014 closed.

### Reconciled
#### ESV-M4-R4 — Final 100-Case Registry and Documentation Evidence Pass
- Activation committed at `81c53dd`.
- Every ESV-T-001 through ESV-T-100 row was reviewed individually.
- **61** rows are Complete from retained direct evidence.
- **39** rows are Deferred to their actual later M5/Laboratory, clean-project/distribution, performance/stress, integration/adoption, or release gate.
- **0** M4-applicable rows are Blocked.
- Package README, CHANGELOG, documentation index, Current Notes, Suite Health, M4 audit, R4 checkpoint plan, package specification, and the dedicated evidence matrix are reconciled to the same state.
- No runtime or test-code change is part of R4.
- The incoming **660 / 660** focused Chronicle floor was rerun fresh during R4 and passed **660 / 660**, with **0 failed**.
- R4 closes with **61 Complete / 39 Deferred / 0 Blocked** across ESV-T-001 through ESV-T-100.
- Chronicle M4 is complete. M5 is eligible for separate activation but is not automatically active.

### Verified
#### ESV-M4-R4 / Chronicle M4 Closeout
- Activation commit: `81c53dd`.
- Fresh final focused Chronicle Editor gate: **660 / 660 passed, 0 failed**.
- Project Test Runner discovery during the focused run: **1005 EditMode tests total**, with **345 outside the selected Chronicle assembly not run** in this gate.
- Final registry disposition: **61 Complete / 39 Deferred / 0 Blocked**.
- No runtime or test-code change was required by R4.
- No M4-applicable evidence gap remained.
- Chronicle M4 is **Complete**.
- M5 is **eligible for separate activation** and remains inactive until that authority step occurs.

### Added
#### ESV-M5-01 — Editor Tooling Assembly, Setup Preview, and Validator Foundation
- Package Editor tooling remains isolated behind the existing Editor-only `EchoDevGames.EchoSave.Editor` assembly.
- `Tools > Sperk’s Forge > The Chronicle > Setup`.
- Deterministic preview-before-mutation Setup planning.
- Create-only schema-2 `EchoSaveConfiguration` authoring after explicit Apply.
- Existing storage-root and all four approved slot-policy modes with effective-capacity truth resolved from Runtime policy.
- Stale-preview, unsafe-target, missing-folder, and occupied-target rejection without overwrite.
- `Tools > Sperk’s Forge > The Chronicle > Validator`.
- Read-only deterministic validation for `ESV-VAL-001`, `ESV-VAL-002`, `ESV-VAL-003`, `ESV-VAL-009`, and `ESV-VAL-015`.
- Focused Editor assembly-boundary, Setup, and Validator tests.

### Verified
#### ESV-M5-01 Closeout
- Activation commit: `affe3ae`.
- Implementation commit: `69721af`.
- Final committed implementation/test scope: **21 files**, `2404` insertions, `1` deletion.
- Corrected implementation scope is **1 modified existing test asmdef + 20 new Editor/test/meta files**, with **0 Runtime C# changes**.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **697 / 697**, with **0 failed**, preserving the incoming **660 / 660** floor and adding **37** net-new Chronicle tests.
- Manual Setup preview showed schema 2, `EchoSave`, `ConfigurableMultiSlot`, effective capacity `64`, and the exact asset that Apply would create before mutation.
- Explicit Apply created one `Assets/EchoSaveConfiguration.asset`.
- Re-preview of the occupied target returned `ESV-SETUP-002`, reported destination unavailable, disabled Apply, and did not overwrite the asset.
- Validator ran against the created configuration and reported **Issues: 0** / no M5-01 validation issues.
- The temporary manual-proof asset was removed; final `git status --short` and `git diff --check` were clean.
- An initial manual path entry exposed a minor path-field clarity advisory (`Assets/Assets/...`); Setup correctly rejected the missing target folder without mutation. This is UX polish, not an M5-01 blocker.
- The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.
- M5-01 completion does **not** complete M5 and does **not** activate M5-02.

### Added
#### ESV-M5-02 — Full Setup/Configuration Authoring and Safe Repair Previews
- `EchoSaveConfiguration` current authoring schema 3.
- Non-mutating schema-1 and schema-2 runtime compatibility through deterministic in-memory defaults.
- Immutable `EchoSaveRuntimePolicy` session truth covering slot policy/capacity, retention, provider IDs, bounded discovery limits, and recovery mode.
- Configured bounded retention and discovery limits consumed by runtime composition.
- Truthful default serializer/storage provider IDs with fail-closed missing-provider handling.
- `ManualOnly` recovery policy authoring only; no decorative automatic-fallback switch.
- Optional `SaveSlotTemplate` authoring metadata with duplicate-ID validation.
- Setup create/edit modes with explicit schema-2 → schema-3 upgrade Preview/Apply.
- Exact before/after change reporting, stale-target fingerprinting, and safer `Assets/` path normalization.
- Selected `EchoSaveRoot` configuration-reference repair Preview/Apply using serialized project-reference truth and Unity Undo recording.
- Validator coverage for retention, providers, fixed-slot-template identity, and bounded discovery limits.
- Focused schema-3 compatibility, Setup edit/upgrade/repair, and Validator tests.

### Verified
#### ESV-M5-02 Closeout
- Activation commit: `3456489`.
- Implementation commit: `d2e9252`.
- Final implementation scope: **23 files**, `3268` insertions, `281` deletions.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **724 / 724**, with **0 failed**.
- Incoming M5-02 floor was **697 / 697**; M5-02 adds **27** net-new focused Chronicle tests.
- Manual create Preview showed schema-3 target truth and fixed the earlier `Assets/Assets/...` path ambiguity.
- Controlled schema-2 → schema-3 Edit Preview showed `Source Schema: 2`, `Target Schema: 3`, and exact `Schema Version: "2" -> "3"` change while the Inspector remained schema 2 before Apply.
- Explicit Apply advanced the disposable configuration to schema 3; immediate re-preview reported `NoChanges`.
- Selected-root repair Preview showed exact `EchoSaveRoot.configuration: "(none)" -> "Assets/EchoSaveConfiguration.asset"` mutation before Apply.
- Explicit repair Apply assigned the configuration and reported an Undo-recorded repair.
- Unity Undo restored the selected root reference to `None`.
- M5-02 Validator reported **Issues: 0** / no M5-02 Chronicle validation issues for the disposable schema-3 configuration.
- Disposable Chronicle test state was removed.
- Cleanup exposed an unrelated pre-existing zero-byte tracked `First Light Example.meta`; it was repaired separately in repository-hygiene commit `423fac1`. No Chronicle implementation file changed in that hygiene commit.
- Final repository state after the hygiene repair was clean and synchronized with `origin/main`.
- The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.
- **ESV-M5-02 is Complete. M5 remains open. M5-03 is not activated.**


### Added
#### ESV-M5-03 — Save Browser, Generation Inspector, and Migration Graph
- Read-only `EchoSaveInspectionSession` over existing production Chronicle configuration/root/catalog truth.
- No-create local-file inspection initialization; an absent production root is represented as an empty inspection state and is not created by inspection.
- Chronicle Save Browser using real catalog/slot truth with deterministic refresh and selection.
- Generation Inspector over immutable committed-generation manifest evidence, including current-head relationship, support/health state, manifest source/current version, commit state, participant count, and payload byte length.
- Migration Graph over package-owned package-document version authority and registered migration edges without executing production migration.
- Copied read-only generation and migration-graph snapshots with no mutation handles.
- Focused inspection-session, Editor-service, and migration-graph tests.

### Verified
#### ESV-M5-03 Closeout
- Activation commit: `e805ae3`.
- Implementation commit: `9c3771c`.
- Final implementation scope: **26 files**, `2419` insertions, `0` deletions.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **735 / 735**, with **0 failed**.
- Incoming M5-03 floor was **724 / 724**; M5-03 adds **11** net-new focused Chronicle tests.
- Save Browser missing-root proof reported `SucceededEmpty`, `Slots (0)`, and explicitly confirmed that the production save root was absent and no directory was created.
- Migration Graph proof reported a **Valid** production registry with **0 registered edges**; `echosave.envelope`, `echosave.manifest`, `echosave.payload`, and `echosave.head` each reported current version `1.0.0` and zero registered edges.
- Generation Inspector proof used disposable slot `c4623739-1627-4556-af58-77a5fb7df34b` and generation `20260811T2226264786596Z-0000000000000001-b565851fd2294a49b10043e48139435f`.
- The inspected generation reported **CURRENT**, **Healthy**, manifest `1.0.0 -> 1.0.0`, **Migrated In Memory: No**, **Committed**, and `0 participants / 238 bytes`.
- The temporary Editor-only proof seeder and disposable production slot/root were removed after evidence capture.
- The disposable configuration asset was removed; First Light scene noise and generated solution noise were restored.
- Final `git status --short` and `git diff --check` were silent at clean implementation baseline `9c3771c`.
- The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.
- **ESV-M5-03 is Complete. M5 remains open. M5-04 is not activated.**


### Added
#### ESV-M5-04 — Failure Simulator, Recovery Planner, bounded Test Data, and Redacted Support Tooling
- Sandbox-only Failure Simulator with Preview-before-Apply, exact bounded targets, stale-preview fingerprint refusal, and ownership-verified cleanup.
- Deterministic bounded synthetic Test Data Generator with slot/generation/byte caps and explicit sandbox ownership markers.
- Recovery Planner Editor surface over Chronicle's existing immutable `SaveRecoveryPlan` authority with no Recover/Apply action.
- Payload-free Redacted Snapshot Exporter using bounded catalog/generation metadata and hashed root/slot/generation identity.
- Canonical sandbox-path guard refusing equality with, nesting inside, or containment of the production Chronicle root.
- Focused M5-04 tests for sandbox collision refusal, bounded generation, cleanup, preview zero-write behavior, stale preview rejection, recovery-planner no-write behavior, redaction, and deterministic support output.

### Fixed
#### ESV-M5-04 pre-commit compile correction
- Corrected the support snapshot's `SaveSlotPolicy` capacity member reference from the nonexistent `EffectiveTechnicalSlotCapacity` name to the existing `EffectiveCapacity` property before the implementation commit and final test gate.

### Verified
#### ESV-M5-04 Closeout
- Activation commit: `df3c30b`.
- Implementation commit: `577dc01`.
- Final implementation scope: **31 files**, `3206` insertions.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **746 / 746**, with **0 failed**.
- Incoming M5-04 floor was **735 / 735**; M5-04 adds **11** net-new focused Chronicle tests.
- Test Data Generator manual proof: Preview reported **2 slots / 4 generations / 4352 estimated bytes** using `64` payload-padding bytes and deterministic seed `504`; Generate succeeded; a repeat Preview correctly refused to clobber the existing sandbox; Cleanup removed the owned fixture.
- Failure Simulator manual proof: `Truncate Manifest` Preview identified exactly one `manifest.json` target inside the owned M5-04 sandbox; Apply mutated that exact sandbox target; Cleanup reported that the owned fixture was removed and post-cleanup absence was verified.
- Recovery Planner manual proof: with production root absent, Preview reported `ServiceNotReady`, `Head Condition: Invalid`, zero candidates, `Recovery Required: No`, preferred candidate `(none)`, and explicitly exposed no Apply/Recover control.
- Redacted Snapshot manual proof: explicit JSON export used schema `echosave.support.snapshot.v1`, configuration schema `3`, provider IDs, slot capacity `64`, hashed `rootToken`/`selectedSlotToken`, bounded empty arrays, and false truncation flags.
- CMD privacy checks passed: raw technical slot ID absent, local `C:\Users\Jesse` path absent, and participant payload-content markers absent.
- Disposable `EchoSaveConfiguration.asset` proof state was removed before implementation commit.
- Repository committed/pushed clean at `577dc01`.
- The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.
- **ESV-M5-04 is Complete. M5 remains open. M5-05 is not activated.**


### Added
#### ESV-M5-05 — Explicit Unknown-Payload Prune and Derived Catalog Cache/Rebuild Prerequisites
- Public/runtime exact-ID unknown-payload prune Preview/Confirm workflow on `IEchoSaveService`.
- Immutable, expiring, one-use prune plans binding slot, source generation, source provenance, and exact requested opaque participant IDs.
- Confirm-time source revalidation across current head/manifest/payload provenance plus rejection when a requested identity becomes claimed/known after Preview.
- New immutable-generation publication for successful prune; committed historical generations are never edited in place.
- Byte-preserving carry-forward of every unnamed stored payload entry.
- Versioned derived `catalog.cache.json` with bounded durable scanner/head freshness validation.
- Missing/corrupt/stale/incompatible cache fallback to durable head/manifest truth and explicit rebuild.
- Catalog cache maintenance truth that cannot convert a successful durable catalog refresh into failure when cache publication fails.
- Chronicle Catalog Cache Editor window with zero-write Preview and explicit Rebuild Catalog Cache.
- Focused M5-05 prune/cache tests.

### Verified
#### ESV-M5-05 Closeout
- Activation commit: `94c33a3`.
- Implementation commit: `ad715c3`.
- Final implementation scope: **33 files**, `4118` insertions, `3` deletions.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **753 / 753**, with **0 failed**.
- Incoming M5-05 floor was **746 / 746**; M5-05 adds **7** net-new focused Chronicle tests.
- Missing-root Catalog Cache Preview reported `Missing`, `0` durable slots, `0` cached entries, performed zero writes, and kept Rebuild disabled.
- Owned-fixture cache proof reported `Missing` with **1 durable slot / 0 cached entries**, explicit Rebuild to `Valid` with **1 / 1** entries and matching durable/cache fingerprints, deliberate durable-head advancement without cache maintenance, `Stale` detection with differing fingerprints, and final explicit Rebuild back to `Valid` with matching fingerprints.
- LAB-016 prerequisite prune Preview was zero-write, exact-ID, source-generation/provenance bound, expiring, and enabled Confirm only after a successful plan.
- LAB-016 Confirm reported `Succeeded`, pruned exactly **1** named unknown entry, left **1** unknown entry, published a different new generation, preserved historical source manifest/payload bytes, preserved the unnamed unknown transport byte-for-byte, preserved the known payload, and published both generation and head successfully.
- The disposable production fixture used **1 known + 2 unknown** payload entries and was ownership-marked.
- Fixture cleanup reported success and verified post-cleanup production-root absence.
- Temporary proof-helper source/meta, disposable configuration asset, and Unity solution noise were removed/restored before implementation commit.
- Repository committed/pushed clean at `ad715c3`.
- The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.
- **ESV-M5-05 is Complete. M5 remains open. M5-06 Save Laboratory is not activated.**

### Added

#### ESV-M5-06 — Minimal Direct-Scene Save Laboratory

- Importable package-owned `Chronicle Save Laboratory` sample declared through `package.json`.
- One isolated direct-scene laboratory using the dedicated `EchoSave-M5-06-Laboratory` storage root.
- Tiny deterministic `SPERK-001` persisted participant with `Sperk Level`, `Galactic Rupees`, `Anvil Temperature`, `Has Forbidden Key`, and `Reality Damage`.
- Sample-only IMGUI engineering controls for create/select/save/load, rename, duplicate, delete Preview/Confirm, prepared-load handling, catalog refresh, deterministic unsaved mutation, and ownership-verified Laboratory reset.
- Evidence console/status readouts that report actual Chronicle results rather than cosmetic pass labels.
- One simple scene-owned camera/background so the human-facing Game view does not show Unity's `Display 1 / No cameras rendering` overlay.
- Eight focused M5-06 package/sample boundary tests.
- No Chronicle Runtime source-file modification and no Looking Glass or Resonance dependency.

### Fixed

#### ESV-M5-06 Imported-Sample Compile Correction

- Package Manager import exposed `CS4008: Cannot await 'void'` in the sample harness because `InitializeLaboratoryAsync()` was declared `async void`.
- Corrected the sample-only method to `async Awaitable` and synchronized the corrected source into the distributable `Samples~` copy.
- The correction changed no Chronicle Runtime API or behavior.

### Verified

#### ESV-M5-06 Closeout

- Activation commit: `d6f079a`.
- Implementation commit: `4bcfbf1`.
- Adjacent project organization commit: `b43e6bf`.
- Final Chronicle package implementation scope: **21 files**, **2341 insertions**, no deletions.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate passed **761 / 761**, with **0 failed** after the imported-sample compile correction and final package-scene synchronization.
- Incoming M5-06 floor was **753 / 753**; M5-06 adds **8** net-new focused Chronicle tests.
- Direct-scene readiness proved `Service: Ready` and one authoritative Chronicle root with an initially empty healthy catalog.
- LAB-003 create/select proof produced one healthy slot and a real initial immutable generation.
- Save proof published a new verified generation/head for visible `SPERK-001` state.
- Unsaved mutation changed the visible subject from the saved snapshot; `Load & Apply` restored the exact saved values and reported `RESULT: THE CHRONICLE REMEMBERS.`
- LAB-006 prepared-load proof survived an explicit wait boundary, reapplied the bound generation, and consumed the one-use handle.
- LAB-008 rename preserved technical slot identity while publishing the new display name through a verified immutable generation.
- LAB-009 duplicate produced a second healthy technical slot with equivalent state and a distinct technical ID/generation.
- Delete Preview was zero-write and source-bound; Confirm removed the selected duplicate from the live catalog, cleared active selection, and retained the original certified slot.
- LAB-031 reset shut Chronicle down, removed only the ownership-marked `EchoSave-M5-06-Laboratory` root, and verified post-cleanup absence.
- The full LAB-001 through LAB-032 matrix is reconciled from retained M4/M5 automated evidence, M5-05 prerequisite/manual evidence for LAB-016/LAB-029, and M5-06 direct-scene/manual evidence. M5-06 did not fabricate 32 separate polished workflows.
- Package sample import, direct-scene human proof, camera-backed Game view, and ownership cleanup are complete.
- Chronicle M5 is **Complete**. M6 First Integration remains inactive and requires separate authority/activation.
- Polished player-facing save-menu examples remain deferred to the later Chronicle Reference Showcase after Looking Glass and preferably Resonance are available.
