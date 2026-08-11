# The Chronicle — Save Infrastructure

`com.echodevgames.echo-save`

The Chronicle is the durable save/load infrastructure package for The Sperk's Forge.

## Current implementation boundary

Version `0.1.0` has completed Chronicle M4, including ESV-M4-R1 through R4. R4 — Final 100-Case Registry, Documentation Evidence Reconciliation, and M4 Closeout — is the closing M4 checkpoint; activation is retained at `81c53dd`.

The R4 evidence pass has reconciled every ESV-T-001 through ESV-T-100 row individually: **61 Complete**, **39 Deferred**, **0 Blocked**. Complete means retained direct evidence exists. Deferred means the exact scenario remains owned by later M5 Laboratory/Setup, clean-project/distribution, performance/stress, integration/adoption, or release qualification. No M4-applicable evidence gap was found.

The fresh final M4 focused Chronicle Editor rerun passed **660 / 660**, with **0 failed**. Chronicle M4 remains complete. **ESV-M5-01 is now complete**: the Editor-only Setup/Validator foundation is implemented at `69721af`, and the closing focused Chronicle Editor gate passed **697 / 697**, with **0 failed**. The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.

## Current persistence boundary

Chronicle M2 provides path-safe local storage, package serialization, technical slot/generation identity, commit documents, integrity, immutable generation publication, and head-last current selection.

M3 is complete. It provides open-ended participant registration, detached capture, participant-backed publication, opaque unknown preservation/carry-forward, trusted current/migrated participant preparation, bounded prepared-load lifetime/session ownership, and deterministic ownership-revalidated participant application with explicit `InitializeDefault` / `Ignore` / `Fail` semantics.

M4-01 adds provider-neutral bounded technical slot discovery, payload-free `head.json` + current `manifest.json` metadata reconstruction, healthy/degraded immutable catalog snapshots, prior-snapshot preservation on untrustworthy refresh failure, and session-only active-slot selection.

M4-02 adds bounded technical slot creation and capacity enforcement. A successful new slot owns one real verified empty immutable generation selected through `head.json` last. Every discovered canonical technical slot, including degraded entries, counts against capacity. Package-generated `SaveSlotId` identity remains independent from display/project/build metadata. Creation does not auto-select. If publication succeeds but catalog reconciliation fails, Chronicle reports the durable publication truth instead of deleting the committed slot or pretending rollback.

M4-03 composes the complete internal manual-save transaction for an explicitly selected healthy slot. It validates the exact current source generation, captures fresh known participants, carries valid opaque unknown payloads forward, rejects stale source/ownership collisions, publishes one participant-backed immutable generation with `head.json` last, preserves ordinary display-name metadata, and reconciles the slot catalog truthfully.

M4-04 exposes that proven transaction through public active-slot `SaveAsync`, adds one root-local mutating-operation admission authority, returns Busy immediately for overlapping manual saves without queueing, honors safe pre-publication cancellation, reports Too Late after durable publication begins, and closes new admission during shutdown while preserving truthful committed outcomes.

M4-05 adds explicit caller-triggered `RequestAutosave`, retains at most one pending latest autosave while admission is occupied, supersedes older pending metadata instead of growing a queue, preserves manual-save Busy semantics, drains at most the latest pending autosave after admission release, and prevents pending work from starting after shutdown admission closure. Chronicle still does not decide when gameplay should autosave.

M4-06 adds bounded total committed-generation retention after successful publication. It discovers history through provider-neutral storage capability, protects the current generation and immediate recovery predecessor, deletes only excess verified committed history oldest-first through optional tree-deletion capability, and reports cleanup maintenance separately from committed save truth.

M4-07 adds read-only recovery planning. It classifies missing/invalid/broken-current head state, discovers retained generations through bounded provider-neutral reads, fully verifies candidate documents and integrity, preserves/excludes untrustworthy evidence, orders valid candidates newest-first deterministically, returns immutable payload-free plan/candidate truth, and fingerprints exact technical source provenance for later stale-plan rejection without mutating storage.

M4-08 adds explicit recovery execution. It reuses root-local mutation admission, rebuilds and provenance-checks the M4-07 plan before mutation, requires the selected candidate to remain fully verified, republishes only `head.json`, preserves immutable generation bytes, and reports post-head catalog reconciliation without fabricated rollback.

M4-09 adds non-destructive public slot rename and full-state duplication. Rename preserves technical slot identity/path and commits display metadata through a new immutable generation with source-freshness protection, retention, and catalog reconciliation. Duplicate enforces canonical slot capacity, creates new package-generated slot/generation identities, copies only a fully verified current source state without participant callbacks, revalidates the source before publication, publishes head last, preserves source bytes, and does not auto-select the duplicate.

M4-10 adds read-only deletion planning and admitted confirmed recoverable trash. Deletion plans bind package/session/source provenance, expire, reject replay, and perform zero mutation until explicit confirmation. Confirm-delete reuses root-local admission, revalidates exact source truth, moves the complete live slot tree into recoverable `trash/`, clears active selection only after durable removal, reconciles the live catalog, and applies bounded fail-closed trash retention. The current focused Chronicle regression floor is **724 / 724**.


R2 adds project-owned slot-policy configuration schema 2. `SingleSlot`, `FixedMultiSlot`, `ConfigurableMultiSlot`, and `BoundedProfiles` resolve to one immutable finite service-session capacity shared by create and duplicate. Schema-1 configurations remain non-mutating compatible at the historical capacity 64.

R3 adds Chronicle-owned package-document migration as deterministic read-time, in-memory contiguous version chains ahead of strict current-version validation. Missing/ambiguous/failed/invalid/newer paths fail closed, source generations are never rewritten merely because migration was required, participant migration remains separate, and production package-document versions remain `1.0.0`.

M5-01 now provides the package Editor-only Setup and Validator foundation. Setup previews before mutation, creates only a new current schema-2 `EchoSaveConfiguration` after explicit Apply, and refuses occupied targets rather than overwriting. Validator is read-only and covers the initial `ESV-VAL-001`, `002`, `003`, `009`, and `015` checks.

**ESV-M5-02 is complete** at implementation commit `d2e9252`. `EchoSaveConfiguration` now authors schema 3 while schema-1/schema-2 remain runtime-readable through deterministic in-memory compatibility defaults and are never silently rewritten. Runtime resolves one immutable session policy covering slot policy/capacity, retention, provider IDs, bounded discovery limits, and the currently truthful manual recovery mode. Setup supports create/edit/upgrade Preview plus explicit Apply, safe path normalization, and selected `EchoSaveRoot` configuration-reference repair with Undo recording. Validator adds retention, provider, fixed-slot-template, and bounded discovery-limit checks. The closing focused Chronicle Editor gate passed **724 / 724**, with **0 failed**.

**ESV-M5-03 is complete** at implementation commit `9c3771c`. Chronicle now includes a strictly read-only Save Browser, Generation Inspector, and Migration Graph over existing catalog/slot/generation/package-document/migration truth. Missing production roots are represented as empty without being created; generation inspection reads immutable manifest evidence without changing heads or generation bytes; the Migration Graph describes package-owned version/edge authority without executing migrations. The closing focused Chronicle Editor gate passed **735 / 735**, with **0 failed**.

It still does **not** include automatic recovery fallback, Failure Simulator/Recovery Planner/Test Data/support export, persistent catalog-cache optimization, cleanup/quarantine, permanent erase, public restore-from-trash, direct-scene Laboratory content, LAB-001 through LAB-032 execution, scene travel, peer bridges, or clean-project/release qualification. **M5 remains open and M5-04 is not active.**

## Minimal use

1. Create an `EchoSaveConfiguration` asset.
2. Place one `EchoSaveRoot` in the project's chosen runtime composition.
3. Assign the configuration.
4. Call `InitializeAsync()` explicitly, or enable the optional auto-initialize flag.
5. Call `ShutdownAsync()` when intentionally ending Chronicle authority.

The consumer project owns scene-surviving object composition. `EchoSaveRoot` does not call `DontDestroyOnLoad` and does not own peer systems.
