# The Sperk’s Forge – Suite Health Check and Remaining Documentation

**Updated:** August 15, 2026
**Completed package checkpoint:** Chronicle M5 complete; First Light FL-M5-R1 sealed; Looking Glass EUI-M2-02 complete
**Current implementation state:** First Light sealed at FL-M5-R1; Chronicle M5 complete; Looking Glass EUI-M2-02 complete at closeout `7f5ad40`; EUI-M3-01 ACTIVE / AUTHORIZED from baseline `0b7622c`; Runtime implementation not started at activation

## Current health

| Area | Status |
|---|---|
| Suite Bible | **SFGSS-000 v0.27.0 Approved** — adds the additive Unity-default Input Actions compatibility profile without transferring input authority |
| Standards | SFGSS-001 through SFGSS-010 complete |
| Package authorities | 28 of 28 approved |
| Learning workflow | **Learn → Declare → Authorize** just-in-time gate + **Green Path** self-validating execution |
| First Light learning gate | Passed |
| First Light package version | `0.1.0` |
| First Light specification | SFGSS-PKG-ECHOLAUNCH-001 v1.17.0 |
| First Light package sample / Standalone Test Lab | **Complete** — `FirstLight_Boot_Splash_Laboratory`; final package/import parity synchronized |
| First Light Package Reference Showcase | **Complete** |
| Latest First Light Setup-focused gate | **224 / 224** |
| Latest First Light post-reconciliation EditMode gate | **1106 / 1106**, `0` failed |
| First Light showcase separation | **Complete** — package Laboratory separate; UMBRA retained as project-owned First Light showcase |
| Clean-project reproduction | Not run in FL-M6-01 closeout |
| Release qualification/private beta | Not run |
| Looking Glass learning | **PKG-LEARN-008 Complete** — bounded EUI-M3-01 revisit reconciles EventSystem coordination, transient focus memory/restoration, Modal containment, independent Window focus memory, explicit revalidation, stale-request protection, and optional suite input-profile defaults |
| Looking Glass authority | **SFGSS-PKG-ECHOUI-001 v1.5.0 Approved** |
| Looking Glass completed checkpoint | **EUI-M2-02 COMPLETE** — activation `e2145ab`; clarification `b6fc160`; implementation `5ab34b3`; focused M2-02 **28 / 28**; EchoUI **75 / 75**; manual Laboratory **12 / 12**; final full EditMode **1181 / 1181** |
| Looking Glass active checkpoint | **EUI-M3-01 ACTIVE / AUTHORIZED** — EventSystem Coordination, Focus Memory/Restoration, and Modal Focus Containment; Runtime implementation not started at activation |
| Chronicle implementation | **M4 and M5 complete; ESV-M5-01 through M5-06 complete; implementation `4bcfbf1`; final focused Chronicle `761 / 761`; M6 not activated** |
| Game Shell initiative | Chronicle → Accord → Resonance → Looking Glass; sequence is planning, not a hard dependency chain |
| Other package implementations | No additional package checkpoint activated by Looking Glass learning |
| Release-blocking architecture conflicts | None recorded by FL-M6-01 |

## Looking Glass EUI-M3-01 activation health

- Planning baseline: `0b7622c`, clean synchronized repository-hygiene boundary after EUI-M2-02 closeout `7f5ad40`.
- Incoming full EditMode floor to re-establish before Runtime edits: **1181 / 1181 passed, 0 failed**.
- Retained EchoUI **75 / 75**; focused M2-02 **28 / 28**; manual M2-02 Laboratory **12 / 12 PASS**.
- EventSystem policy: `AdoptAssigned`, deterministic `AdoptExisting`, `CreateIfMissing`, or `RequireExternal`; never silently destroy/disable external systems.
- Multiple eligible active EventSystems produce degraded/blocking focus status.
- Focus memory is per entry with optional transient stable-surface session memory and designer-controlled fresh/remembered reopen behavior.
- Restoration is policy-aware and may legally resolve to no focus.
- Blocking Modals contain Looking Glass focus while lower-entry memory survives.
- Independent Windows may retain distinct focus memory without activating the full Window manager.
- Focus work is event-driven with explicit revalidation; no universal per-frame scan is required.
- Optional input adapters may default to SFGSS-000 v0.27.0's Unity-default action-name profile without a generated-wrapper/peer-package dependency or action-map ownership.
- Transitions, Motifs/accessibility presentation, HUD/transients, Window LIFO/pinning/layout, persistence, Builder, primitive/9-slice work, peer bridges, and gameplay-input ownership remain inactive.
- No new ADR is required beyond the approved SFGSS-000/package authority reconciliation.

## Looking Glass EUI-M2-01 completion health

- Activation `0c11262`; implementation `8dc9c71`.
- Incoming full EditMode floor was re-established at **1130 / 1130** before Runtime edits.
- Focused EchoUI **47 / 47**; EUI-M2-01 focused **23 / 23**; retained M1 **24 / 24**.
- Final full EditMode **1153 / 1153**, `0` failed, `0` skipped, `0` inconclusive.
- Manual Laboratory **10 / 10 PASS**.
- Project-defined variable ordered layer topology works without fixed starter-name/count requirements.
- RootOwned / SceneOwned / ExternalOwned lifecycle and ownership boundaries are proven.
- Push/Replace/Reset/Back/Close and strict FIFO settlement are proven.
- Suspended-screen visibility remains designer-controlled while interaction exclusivity is preserved.
- M1 context, selection, Back, and independent Window behavior remains green.
- Runtime has no peer Echo dependency; no package-spec revision or new suite ADR was required.
- EUI-M2-02 and all modal results, transitions, richer focus/EventSystem policy, HUD/transients, Motifs, Builder, persistence, and peer bridges remain separately gated and inactive.


## Looking Glass EUI-M2-02 completion health

- Planning baseline: `d5b9a73`, clean synchronized EUI-M2-01 closeout.
- Activation: `e2145ab`; Modal/Window clarification: `b6fc160`; implementation: `5ab34b3`.
- Incoming full EditMode floor before Runtime edits: **1153 / 1153 passed, 0 failed**.
- Focused EUI-M2-02 Modal lifecycle: **28 / 28 passed, 0 failed**.
- Final EchoUI EditMode assembly: **75 / 75 passed, 0 failed**.
- Final Foundry EditMode regression: **1181 / 1181 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory: **12 / 12 PASS**; retained M2-01 Screens tab **PASS**; retained M1 tab **PASS**.
- Blocking modals may stack; only the top eligible modal is Looking Glass-interactive.
- Normal completion returns project-defined stable result IDs.
- First valid terminal completion wins exactly once; stale later attempts cannot settle again.
- Unexpected admitted owner/view loss or shutdown settles as structural `Aborted`, distinct from semantic Cancel.
- RootOwned / SceneOwned / ExternalOwned modal ownership mirrors the proven Screen lifetime model.
- Back is modal-first and designer-authored as disabled or mapped to a stable result ID.
- Blocking applies to lower Looking Glass pointer/navigation/submit/Back interaction; gameplay input, pause/time, cursor, and project action maps remain external.
- Blocking Modal semantics are role-specific; independent Windows remain non-blocking/coexistent by default.
- Future independent-window Back/Escape dismissal is explicitly separate from M2-01 FIFO execution and may use most-recent-eligible LIFO history with authored/runtime pin-lock exclusions; this remains outside M2-02 Runtime scope.
- Screen mutation while blocked defaults to explicit Reject, with optional bounded FIFO DeferUntilModalStackClears.
- Modal visuals/backdrops remain project-owned; no mandatory dim/blur style.
- Full focus/EventSystem policy, transitions, HUD/transients, Motifs, Builder, primitive expansion, arbitrary modal domain payload transport, persistence, peer bridges, and project-wide lifetime composition remain inactive.
- No new suite ADR is required by this package-local refinement.
- Package authority remains **SFGSS-PKG-ECHOUI-001 v1.4.1**.
- **EUI-M2-02 is COMPLETE. No follow-on Looking Glass checkpoint is activated.**

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

The project now separates package proof from showcase proof:

```text
Package source:
  Packages/com.echodevgames.echo-launch/Samples~/FirstLight_Boot_Splash_Laboratory/
Imported package proof:
  Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/
Project-owned First Light showcase:
  Assets/EchoDevGames/SuiteShowcase/First Light/UMBRA Example/
```

The Boot Splash Laboratory carries the canonical First Light engineering/happy-path proof and ships with the package. UMBRA remains independently-authored project showcase content. Additional future showcase examples may be project-owned content exercises; they do not automatically authorize package changes.

## Evidence carried forward

- FL-M5-07 retained full automated baseline: `809 / 809`.
- FL-M5-07 manual Laboratory matrix: `12 / 12`.
- FL-M6-01-H1 focused identity gate: `5 / 5`.
- FL-M6-01-H2 focused destination-Build-Settings gate: `35 / 35`.
- Final FL-M6-01 `EchoLaunchSetup` filtered EditMode gate: `224 / 224`.
- UMBRA fresh-root Create Project-Owned Setup proof created the requested foundation and serialized three authored splashes.
- UMBRA runtime Boot presentation succeeded.
- Identical second Apply returned `NoChanges`, created no paths, and preserved Build Settings.

FL-M5-R1 post-M5 reconciliation additionally proved:
- canonical package sample path `Samples~/FirstLight_Boot_Splash_Laboratory/`;
- Package Manager identity **First Light Boot Splash Laboratory**;
- final package/imported Laboratory parity;
- serialized destination/test reconciliation after the rename;
- optional `None / Subtle / Medium / Nightmare` Splash Shake with Reduced Motion suppression and no schema bump;
- post-reconciliation full EditMode **1106 / 1106**, `0` failed;
- manual visible `Nightmare` runtime proof;
- implementation commit `cea876e`.

FL-M5-R1 records the fresh full EditMode aggregate above. A fresh Runtime Play Mode/full release-qualification regression must still be collected when that later gate is activated.

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

If First Light returns to active development, the next release-oriented work remains separate from completed FL-M6-01 and FL-M5-R1:

- clean-project reproduction of the proven happy path;
- full current regression totals;
- supported installation-route evidence;
- player-build qualification where required;
- release checklist/version/tag/catalog decisions;
- private beta/external adoption.

No such work is active merely because FL-M6-01 and FL-M5-R1 closed.

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

## Chronicle M5-06 closeout / M5 completion

`ESV-M5-06 — Minimal Direct-Scene Save Laboratory` is **Complete**.

Evidence:
- activation `d6f079a`;
- implementation `4bcfbf1`;
- adjacent project organization `b43e6bf`;
- focused Chronicle Editor **761 / 761**, `0` failed;
- **8** net-new focused tests;
- **21 package files / 2341 insertions / 0 deletions**;
- Package Manager import/reimport completed after closing the sample-only `async void` compile defect;
- human direct-scene proof completed for create/select, Save -> unsaved mutation -> Load exact restoration, prepared load, rename, duplicate, delete Preview/Confirm, and ownership-verified Laboratory reset;
- LAB-001 through LAB-032 reconciled from retained automated, prerequisite, and direct-scene evidence;
- no Looking Glass or Resonance dependency;
- no Chronicle Runtime source file changed by M5-06.

Chronicle **M5 — Tooling and Laboratory is Complete**. M6 First Integration remains inactive and requires separate activation.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked** because exact later clean-project/distribution, performance/stress, integration/adoption, and release cases remain deferred to their own gates.

## Package sample / showcase organization

The adjacent `b43e6bf` project commit separates retained project-owned showcase material from imported package samples. This is organizational project work, not EchoSave runtime authority.

- UMBRA remains project-owned **First Light showcase** content.
- The Chronicle Save Laboratory is package-distributed engineering/sample content and also exists as an imported project sample for human proof.
- UMBRA does **not** replace First Light's package-sample obligation.
- FL-M5-R1 completed the separate follow-up and restored/hardened the First Light package sample as `FirstLight_Boot_Splash_Laboratory`; UMBRA remains a showcase only.
- Polished Chronicle save-format examples remain deferred to the later Chronicle Reference Showcase after Looking Glass and preferably Resonance are packaged.

This follows the existing suite requirement that release-ready scene-visible packages retain an independently importable sample/Standalone Test Lab; polished combined/project showcases do not substitute for isolated package proof.

### Looking Glass EUI-M1-02 completion
- activation `f0b97ff`; implementation `1c0a46a`;
- incoming full EditMode floor **1113 / 1113**, `0` failed before Runtime edits;
- focused EchoUI assembly **24 / 24**, `0` failed (**17 M1-02 + 7 retained M1-01**);
- manual Laboratory **10 / 10 PASS**;
- final synchronized full EditMode **1130 / 1130**, `0` failed, `0` skipped, `0` inconclusive;
- package/imported Laboratory parity synchronized to the manually accepted scene;
- project-defined active/inactive context IDs and simultaneous contexts;
- designer-owned ordered per-surface response with independent visibility/interactability/selection resolution;
- no-rule/no-dimension intervention semantics and local external-context opt-out;
- transient runtime overrides without authored-definition mutation or persistence ownership;
- externally supplied pointer/navigation modality with designer-selected default-selection behavior;
- neutral close behavior without implicit selection-history restoration;
- pause/cinematic/loading/input truth remains external and surfaces choose their own response;
- zero hard runtime dependency on another Echo package;
- package authority remains **SFGSS-PKG-ECHOUI-001 v1.2.0**; no contract revision/new ADR required;
- no follow-on Looking Glass checkpoint activated.

### Looking Glass EUI-M1-01 completion

- `EchoUIRoot` provides bounded package-local authority and surface registration.
- `UISurface` supports stable project-authored IDs and the initial `Screen` / `Window` roles used by the proof.
- the `frontend` navigation scope proves `main-menu -> settings -> Back -> main-menu`;
- `default-window` proves independent open/close coexistence without mutating the current frontend screen;
- full EditMode regression evidence is **1113 / 1113 passed, 0 failed**;
- five-item direct-scene manual acceptance passed;
- the finished Laboratory is synchronized between imported project sample and package-owned `Samples~`;
- normal TextMesh Pro UI labels are allowed; the repository carries TMP Essential Resources for the hand-authored project sample, while TMP Examples & Extras are not required;
- At the EUI-M1-01 closeout boundary, Motifs, Builder tooling, external context/visibility rules, input-aware selection, and broader surface capabilities were future work. EUI-M1-02 now activates only the bounded external-context response and input-aware-selection subset; the remaining items stay future work.

**Historical M1-01 closeout state:** EUI-M1-02 was not activated at `57a4fa4`. **Superseding state:** EUI-M1-02 is COMPLETE at implementation `1c0a46a` under EchoUI specification v1.2.0.
