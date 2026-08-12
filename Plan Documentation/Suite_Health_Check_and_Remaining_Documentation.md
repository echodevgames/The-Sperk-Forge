# The Sperk’s Forge – Suite Health Check and Remaining Documentation

**Updated:** August 11, 2026
**Completed package checkpoint:** ESV-M5-05 – Chronicle Unknown-Payload Prune and Derived Catalog Cache/Rebuild Prerequisites
**Current implementation state:** First Light complete/frozen; Chronicle M4 complete; ESV-M5-01 through M5-05 complete; ESV-M5-06 minimal direct-scene Save Laboratory active from `868b17f`; incoming focused Chronicle `753 / 753`; registry remains `61 Complete / 39 Deferred / 0 Blocked`; M5 open

## Current health

| Area | Status |
|---|---|
| Suite Bible | Approved; no FL-M6-01 closeout authority change required |
| Standards | SFGSS-001 through SFGSS-010 complete |
| Package authorities | 28 of 28 approved |
| Learning workflow | Just-in-time package-local gate remains authoritative |
| First Light learning gate | Passed |
| First Light package version | `0.1.0` |
| First Light specification | SFGSS-PKG-ECHOLAUNCH-001 v1.16.0 |
| First Light Standalone Test Lab | Complete; retained `809 / 809` automated and `12 / 12` manual evidence |
| First Light Package Reference Showcase | **Complete** |
| Latest First Light Setup-focused gate | **224 / 224** |
| Permanent First Light Gallery | **Complete** — First Light Example + UMBRA Example |
| Clean-project reproduction | Not run in FL-M6-01 closeout |
| Release qualification/private beta | Not run |
| Next package learning | **PKG-LEARN-009 – The Chronicle (`EchoSave`) complete** |
| Chronicle implementation | **M4 complete; ESV-M5-01 through M5-05 complete; ESV-M5-06 minimal direct-scene Save Laboratory active; incoming focused Chronicle `753 / 753`; M5 open** |
| Game Shell initiative | Chronicle → Accord → Resonance → Looking Glass; sequence is planning, not a hard dependency chain |
| Other package implementations | Not activated |
| Release-blocking architecture conflicts | None recorded by FL-M6-01 |

## First Light graduation state

```text
Learning / authority                 PASS
Implementation                       PASS for current approved scope
Standalone Test Lab                  PASS
Package Reference Showcase           PASS
Clean-project reproduction           NOT RUN
Release qualification                NOT RUN
Private beta / external adoption     NOT RUN
```

First Light is therefore **complete for its current in-repository implementation and Reference Showcase pass**, but this document does not call the package release-qualified.

## Permanent consumer proof

The project now retains:

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
└── UMBRA Example/
```

The Gallery demonstrates both the canonical EchoDevGames happy path and an independently-authored UMBRA consumer foundation. Additional future Gallery examples may be project-owned content exercises; they do not automatically authorize package changes.

## Evidence carried forward

- FL-M5-07 retained full automated baseline: `809 / 809`.
- FL-M5-07 manual Laboratory matrix: `12 / 12`.
- FL-M6-01-H1 focused identity gate: `5 / 5`.
- FL-M6-01-H2 focused destination-Build-Settings gate: `35 / 35`.
- Final FL-M6-01 `EchoLaunchSetup` filtered EditMode gate: `224 / 224`.
- UMBRA fresh-root Create Project-Owned Setup proof created the requested foundation and serialized three authored splashes.
- UMBRA runtime Boot presentation succeeded.
- Identical second Apply returned `NoChanges`, created no paths, and preserved Build Settings.

No post-A1 complete EditMode or Runtime Play Mode aggregate is invented here. Full-suite regression must be collected again when release qualification is activated.

## Active next-package work

The next package is **The Chronicle (`EchoSave`)**.

Chronicle M1, M2, and **M3 — Participants and Loading** are complete.

`ESV-M4-01` is complete at `62e8a54`; the focused Chronicle Editor gate passed **403 / 403**. Chronicle can now discover existing technical slots through an additive provider-neutral capability, rebuild payload-free lightweight metadata from authoritative heads/current manifests, preserve degraded technical slots honestly, and maintain explicit session-only active-slot selection.

`ESV-M4-02` is complete at `d8d5c18`; the focused Chronicle Editor gate passed **425 / 425**. Chronicle can create a bounded technical slot as one real empty immutable generation, enforce capacity across healthy and degraded canonical slots, reject generated-ID collisions within a positive bound, publish `head.json` last, reconcile the catalog without auto-selecting, and report published-but-reconciliation-failed truth without fictional rollback.

`ESV-M4-03` is complete at `c8ea742`; the focused Chronicle Editor gate passed **439 / 439**. Chronicle can perform one bounded internal manual-save transaction against the explicitly selected healthy slot, validate exact source provenance, capture fresh known participant state, preserve valid opaque unknown payloads, reject stale source/ownership collisions, publish one participant-backed immutable generation with `head.json` last, preserve ordinary display-name metadata, and reconcile the catalog with truthful partial durable/head/catalog outcomes.

`ESV-M4-04` is complete at implementation commit `2732aaa` with bounded lifecycle-status hotfix `09ae8f1`; the final focused Chronicle Editor gate passed **456 / 456**. Chronicle now exposes public active-slot `SaveAsync`, admits one root-local mutating operation at a time, rejects overlapping manual save as Busy without queueing, honors safe pre-publication cancellation, reports Too Late after durable publication begins, and closes new admission during shutdown while preserving truthful durable settlement.

`ESV-M4-05` is complete at `9917f1b`; the focused Chronicle Editor gate passed **473 / 473**. Chronicle now accepts explicit caller-triggered autosave requests, retains at most one latest pending request, coalesces/supersedes without growing a queue, reuses the same M4-04 root-local admission and M4-03/M4-04 durable active-slot save path, preserves manual-save Busy behavior, drains pending autosave at most once after admission release, and prevents pending work from starting after shutdown admission closure.

`ESV-M4-06` is complete at `e714a90`; the focused Chronicle Editor gate passed **497 / 497**. Chronicle now bounds ordinary committed-generation history after successful publication, protects current and immediate predecessor generations, fails closed on untrustworthy history, deletes only oldest excess verified committed generations through optional provider-neutral tree deletion, and reports retention maintenance separately from committed save truth.

`ESV-M4-07` is complete at `9f68555`; the focused Chronicle Editor gate passed **524 / 524**. Chronicle can now build immutable read-only recovery plans that diagnose source state, discover retained generations through bounded provider-neutral reads, admit only fully verified committed candidates, order them deterministically newest-valid first, preserve/exclude bad evidence, and carry technical source provenance for later stale-plan rejection without mutating durable storage.

`ESV-M4-08` is complete at `1985fb0`; the focused Chronicle Editor gate passed **540 / 540**. Chronicle can now explicitly execute one M4-07 recovery choice through shared mutation admission, reject stale plan/candidate evidence before mutation, repoint only `head.json` to an already verified immutable generation, preserve generation bytes, and reconcile catalog truth after the durable head commit.

`ESV-M4-09` is complete at `459023f`; the focused Chronicle Editor gate passed **562 / 562**. Chronicle can now rename one canonical healthy slot while preserving technical slot identity/path and publishing display metadata through a new immutable generation, and can duplicate one fully verified current source state into a new package-generated slot/generation identity. Both operations reuse root-local admission, source revalidation, head-last publication, and truthful post-publication catalog semantics; rename reuses retention, duplicate honors canonical capacity, participant callbacks remain absent, and the new duplicate is not auto-selected.


`ESV-M4-10` is complete at `01e4cdd`; the focused Chronicle Editor gate passed **587 / 587**. Chronicle now provides read-only zero-mutation deletion planning, immutable expiring one-use source-bound plans, root-local Busy/no-queue confirmed deletion, fresh source revalidation, complete-tree recoverable trash publication, active-slot/catalog reconciliation, bounded fail-closed trash retention, and truthful post-commit failure states.

`ESV-M4-R1` is complete at `ab18361`; the focused Chronicle Editor gate passed **618 / 618** and closed audit gaps A-01/A-02 through bounded public runtime composition.

`ESV-M4-R2` is complete at `8a8e7e7`; the focused Chronicle Editor gate passed **636 / 636**, adding **18** focused policy tests. Chronicle now resolves schema-2 project-owned `SingleSlot`, `FixedMultiSlot`, `ConfigurableMultiSlot`, and `BoundedProfiles` policy into one immutable finite service-session capacity shared by create and duplicate, while schema-1 remains non-mutating compatible at historical capacity 64. ESV-T-015 through ESV-T-018 are complete and audit gap A-03 is closed.

`ESV-M4-R3` is complete at `c6ba1ad`; the focused Chronicle Editor gate passed **660 / 660**, adding **24** focused package-document migration tests over the incoming **636 / 636** floor. Chronicle now owns deterministic read-time package-document migration through an internal version probe, package-owned step registry, contiguous chain coordinator, and migration-aware reader integrated into current-generation, catalog, and recovery reads. Missing/ambiguous/failed/invalid/newer paths fail closed, source generations remain immutable, participant migration stays separate, and current production package-document versions remain `1.0.0`. Audit gap **A-04 / CAP-014 is closed**.

Chronicle M4 is complete and the reconciled registry remains **61 Complete / 39 Deferred / 0 Blocked**. ESV-M5-01 is now complete at `69721af` with focused Chronicle Editor evidence **697 / 697** and manual Setup/Validator proof. Permanent erase, restore-from-trash public API, quarantine/cleanup, persistent catalog-cache optimization, broader authoring/Browser/Simulator/Laboratory work, scene travel, peer bridges, and release qualification remain later bounded work.

SFGSS-ADR-006 and the Laboratory → Reference Showcase → clean-project → Distribution Kit/release-evidence loop remain authoritative.

The planned follow-on Game Shell sequence is Accord, Resonance, then Looking Glass. That order is a development plan, not a hard dependency graph.

## First Light future return gates

If First Light returns to active development, the next release-oriented work remains separate from FL-M6-01:

- clean-project reproduction of the proven happy path;
- full current regression totals;
- supported installation-route evidence;
- player-build qualification where required;
- release checklist/version/tag/catalog decisions;
- private beta/external adoption.

No such work is active merely because FL-M6-01 closed.

## Current stop point

`ESV-M4-R3` is **complete** at implementation commit `c6ba1ad`.

Final R3 focused Chronicle Editor evidence: **660 / 660 passed, 0 failed**, preserving the incoming **636 / 636** floor and adding **24** focused migration tests.

R3 closes **A-04 / CAP-014**. Package-document migration is Chronicle-owned, read-time, in-memory, deterministic, source-immutable, and separate from participant migration. Current production package document versions remain `1.0.0`.

Still mandatory before M4 close:
- final applicable 100-case registry/document evidence reconciliation;
- final M4 capability/evidence matrix and stale-document repair;
- final focused Chronicle regression evidence at the actual closing total.

The final reconciliation is now **ESV-M4-R4 ACTIVE / AUTHORIZED** from clean baseline `e3d7a2e`.

**M4 remains open. M5 remains locked. R4 must reconcile all 100 registry rows individually and rerun the focused Chronicle suite before M4 can close.**

## R4 evidence reconciliation state

The final registry pass has individually reconciled all ESV-T-001 through ESV-T-100 rows:

- **61 Complete** from retained direct evidence;
- **39 Deferred** to their actual later package-graduation gate;
- **0 Blocked** as an unresolved M4-applicable evidence gap.

This does not make the package release-qualified and does not waive later Laboratory, clean-project, distribution, performance/stress, integration/adoption, or release work.

The fresh R4 closing focused Chronicle Editor rerun passed **660 / 660**, with **0 failed**. **M4 is complete. M5 is eligible for separate activation and is not automatically active.**

## Chronicle M4 closeout state

ESV-M4-R4 closed the milestone with:
- **61 Complete / 39 Deferred / 0 Blocked** registry reconciliation;
- **660 / 660 passed, 0 failed** fresh focused Chronicle Editor evidence;
- no R4 runtime/test-code repair;
- no unresolved M4-applicable evidence blocker.

Chronicle M4 is **Complete**. M5 is now eligible for a separate activation checkpoint. Later Laboratory, clean-project, distribution, performance/stress, integration/adoption, and release obligations remain open where the registry says Deferred.

## Chronicle M5 activation state

ESV-M5-01 is active from clean M4 closeout baseline `e63d83f` under v1.44.0 / ESV-D-037.

The first M5 slice is intentionally Editor-only and non-destructive by default. It establishes the tooling assembly, Setup preview/create-only current-schema configuration path, and initial Validator. Browser, simulation, support export, persistent-cache/cleanup work, and the 32-scenario Save Laboratory remain separately gated.

The focused Chronicle regression floor carried into M5 is **660 / 660 passed, 0 failed**.


## Chronicle M5-01 closeout state

`ESV-M5-01` is **Complete**.

Evidence:
- activation `affe3ae`;
- implementation `69721af`;
- focused Chronicle Editor **697 / 697 passed, 0 failed**;
- **37** net-new focused tests;
- **21-file** committed implementation/test scope;
- no Runtime C# changes;
- manual Setup preview/apply/no-overwrite proof;
- Validator **Issues: 0**;
- clean temporary-asset cleanup.

M5 remains open. No M5-02 implementation is active until a separate bounded authority/activation checkpoint.


## Chronicle M5-02 activation state

`ESV-M5-02 — Full Setup/Configuration Authoring and Safe Repair Previews` is **Active / Authorized** from clean baseline `8774dd2`.

It extends the M5-01 Editor safety model rather than weakening it: preview first, explicit Apply, compatibility-preserving schema evolution, no silent overwrite/upgrade, and no production save-data mutation.

Browser/Inspector, Simulator/Recovery Planner, support/privacy tooling, persistent cache/cleanup, and the standalone Save Laboratory remain later M5 gates.


## Chronicle M5-02 closeout state

`ESV-M5-02 — Full Setup/Configuration Authoring and Safe Repair Previews` is **Complete**.

Evidence:
- activation `3456489`;
- implementation `d2e9252`;
- focused Chronicle Editor **724 / 724 passed, 0 failed**;
- **27** net-new focused tests;
- **23-file** implementation scope, `3268` insertions, `281` deletions;
- schema-2 → schema-3 Preview/Apply manual proof;
- selected-root reference Preview/Apply/Undo proof;
- Validator **Issues: 0**;
- disposable proof state removed.

Repository-hygiene commit `423fac1` repaired a pre-existing zero-byte First Light Example folder `.meta` discovered during cleanup. It changed no Chronicle implementation file and is not part of M5-02 feature scope.

M5 remains open. M5-03 — Browser, Generation Inspector, and Migration Graph — is not active until a separate bounded authority/activation checkpoint.


## Chronicle M5-03 activation state

`ESV-M5-03 — Save Browser, Generation Inspector, and Migration Graph` is **Active / Authorized** from clean baseline `b4d4d0b`.

This checkpoint is inspection-only. It may expose additive read-only query/DTO surfaces where necessary, but it may not execute recovery or mutate project/save state.

Failure simulation, recovery planning, support export, persistent cache/cleanup, and the Save Laboratory remain later M5 gates.


## Chronicle M5-03 closeout state

`ESV-M5-03 — Save Browser, Generation Inspector, and Migration Graph` is **Complete**.

- activation `e805ae3`;
- implementation `9c3771c`;
- focused Chronicle Editor **735 / 735**, `0` failed;
- **11** net-new focused tests over the `724 / 724` floor;
- exact implementation scope **26 files / 2419 insertions**;
- Browser missing-root/no-create manual proof complete;
- Migration Graph valid-current-only/zero-edge manual proof complete;
- Generation Inspector real committed-generation manual proof complete;
- temporary proof state and tooling removed;
- final repository verification clean at `9c3771c`.

M5 remains open. Failure Simulator, Recovery Planner, Test Data, support/privacy tooling, persistent cache/cleanup, and the Save Laboratory remain later separately activated gates.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.


## Chronicle M5-04 activation state

`ESV-M5-04 — Failure Simulator, Recovery Planner, bounded Test Data, and support/privacy tooling` is **Active / Authorized** from clean baseline `ffff18f`.

The slice is intentionally divided by mutation boundary:
- Failure Simulator/Test Data may mutate **sandbox-only** records.
- Recovery Planner may inspect production or sandbox recovery truth but remains preview-only.
- Redacted support export may read bounded diagnostic/manifest health truth but exports no participant payload contents and redacts filesystem/slot identity by default.

The earlier shorthand that M5-05 would directly be Save Laboratory is superseded: M5-05 now closes LAB-016 unknown-prune and LAB-029 catalog-cache prerequisites; the full Save Laboratory moves to M5-06.


## Chronicle M5-04 closeout state

`ESV-M5-04 — Failure Simulator, Recovery Planner, bounded Test Data, and Redacted Support Tooling` is **Complete**.

- activation `df3c30b`;
- implementation `577dc01`;
- focused Chronicle Editor **746 / 746**, `0` failed;
- **11** net-new focused tests over the `735 / 735` floor;
- exact implementation scope **31 files / 3206 insertions**;
- bounded Test Data manual proof complete;
- Failure Simulator Preview/Apply/verified cleanup manual proof complete;
- Recovery Planner preview-only/no Recover control manual proof complete;
- Redacted Snapshot preview/export/privacy proof complete;
- implementation committed and pushed from a clean tree.

M5 remains open. Persistent cache/cleanup and direct-scene Save Laboratory work remain separately gated. M5-05 is not active.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.


## Chronicle M5-05 activation state

`ESV-M5-05 — Explicit Unknown-Payload Prune and Derived Catalog Cache/Rebuild Prerequisites` is **Active / Authorized** from clean baseline `1111b46`.

This sequence correction is required by the existing Laboratory acceptance matrix:
- LAB-016 cannot pass until explicit unknown-prune exists;
- LAB-029 cannot pass until derived catalog-cache rebuild exists.

M5-05 therefore closes those already-approved MVP prerequisites before scene/sample Laboratory work begins.

The full Chronicle Save Laboratory is now **M5-06**, remains inactive, and still must prove LAB-001 through LAB-032 without touching production saves.


## Chronicle M5-05 closeout state

`ESV-M5-05 — Explicit Unknown-Payload Prune and Derived Catalog Cache/Rebuild Prerequisites` is **Complete**.

- activation `94c33a3`;
- implementation `ad715c3`;
- focused Chronicle Editor **753 / 753**, `0` failed;
- **7** net-new focused tests over the `746 / 746` floor;
- exact implementation scope **33 files / 4118 insertions / 3 deletions**;
- LAB-016 prerequisite exact unknown-prune Preview/Confirm proof complete;
- historical source generation remained byte-immutable;
- unnamed unknown transport remained byte-for-byte preserved;
- known payload remained preserved;
- LAB-029 prerequisite cache proof completed `Missing -> Rebuild -> Valid -> Stale -> Rebuild -> Valid`;
- owned proof-fixture cleanup and post-cleanup absence verified;
- temporary proof state removed;
- repository clean/synchronized after implementation commit.

M5 remains open. The full direct-scene Chronicle Save Laboratory is **ESV-M5-06**, remains inactive, and requires separate activation before LAB-001 through LAB-032 execution.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.


## Chronicle M5-06 activation state

`ESV-M5-06 — Minimal Direct-Scene Save Laboratory` is **Active / Authorized** from clean baseline `868b17f`. The Lab is an engineering control panel, not a consumer save-menu showcase. Polished Chronicle save-format examples remain deferred to the later Reference Showcase after Looking Glass and preferably Resonance exist.
