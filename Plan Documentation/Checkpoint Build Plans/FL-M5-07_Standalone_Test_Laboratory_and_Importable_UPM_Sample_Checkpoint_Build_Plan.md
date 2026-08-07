# FL-M5-07 — Standalone Test Laboratory and Importable UPM Sample

**Package:** First Light (`EchoLaunch`)
**Checkpoint:** FL-M5-07
**Status:** Approved for implementation after authority commit
**Specification authority:** SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
**Starting implementation baseline:** FL-M5-06 closeout commit `e28ff09`, followed only by the bounded two-note post-rewind drift reconciliation
**Unity baseline:** `6000.3.8f1`
**Date:** August 7, 2026

---

## 1. Purpose

Deliver the already-approved First Light Standalone Test Laboratory as one clean,
separately importable Unity Package Manager sample that proves the complete MVP
launch loop without adding a second launch pipeline or a second setup system.

The checkpoint exists to turn already-implemented runtime and Editor contracts
into trustworthy isolated user evidence. The Laboratory demonstrates First
Light. It does not become another First Light authority.

---

## 2. Starting Conditions and Drift Gate

Implementation may begin only when all of the following are true:

- `main` and `origin/main` are synchronized.
- The working tree is clean.
- FL-M5-06 closeout commit `e28ff09` is an ancestor of `HEAD`.
- The only repository change between `e28ff09` and the authority baseline before
  this checkpoint is the bounded two-file `Current Notes.md` reconciliation and
  the FL-M5-07 authority commit itself.
- Unity compiles with `0` errors and `0` warnings.
- Fresh baseline evidence remains `290` EditMode + `503` Runtime Play Mode =
  `793` passing automated tests.
- No discarded FL-M5-07 `Samples~`, imported `Assets/Samples` Laboratory scene,
  sample test folder, or other post-`e28ff09` implementation residue exists.

If any condition fails, stop. Do not repair by copying files from discarded
history. Reconcile the current repository first.

### 2.1 Known baseline documentation drift

The `e28ff09` closeout correctly updated the package README, package documentation
index, architecture, changelog, specification, checkpoint record, test report,
and completion record. Two living `Current Notes.md` files retained stale text
that still described the FL-M5-06 documentation closeout as pending.

That bounded drift is corrected in a separate no-code reconciliation immediately
before this authority. No other baseline change is carried forward.

---

## 3. User-Visible Outcome

After FL-M5-07:

1. Package Manager displays exactly one sample named **First Light Standalone Test Lab**.
2. Clicking **Import** copies a complete Laboratory into Unity's normal
   `Assets/Samples/<package display name>/<package version>/...` location.
3. The imported sample compiles without another Sperk's Forge runtime package.
4. No generator or setup transaction runs automatically.
5. The user can open authored Boot/destination scenes and run the approved
   success/failure/direct-scene/duplicate/splash cases.
6. Removing the imported sample leaves EchoLaunch compiling and its Editor tools
   available.

---

## 4. Authoritative Design

### 4.1 One fully-authored UPM sample

The source-of-distribution is:

```text
Packages/com.echodevgames.echo-launch/
└── Samples~/
    └── First Light Standalone Test Lab/
```

`package.json` declares exactly one sample entry for FL-M5-07.

The sample's scenes, ScriptableObject assets, prefab(s), placeholder art, scripts,
and `.meta` files are committed in their final distributable form. Unity Package
Manager performs the normal copy into `Assets/Samples`; First Light does not run
an additional sample generator after import.

### 4.2 No import-time side effects

Importing the sample must not automatically:

- open or save a scene;
- add or reorder Build Settings scenes;
- call Setup Apply or Setup Repair;
- run Validator;
- run Simulator;
- enter Play Mode;
- create additional project assets outside the imported sample root;
- mutate package source;
- register a scripting define or build hook.

Any Build Settings changes needed for manual canonical-Boot testing are explicit
acceptance-test setup and are restored afterward.

### 4.3 Sample assembly isolation

Sample-only executable code lives in a dedicated sample runtime assembly, for
example:

```text
EchoDevGames.EchoLaunch.Samples.StandaloneLab
```

It may reference only the EchoLaunch assemblies required by the sample and the
same declared Unity dependency surface already owned by EchoLaunch.

The Laboratory must not reference:

- Jukebot;
- EchoUI;
- EchoSave;
- EchoSettings;
- EchoSceneFlow;
- EchoGameState;
- EchoInput;
- EchoDiagnostics;
- any game/project runtime assembly.

No sample Editor assembly is approved unless implementation proves one is
strictly required and authority is amended first.

### 4.4 Serialized reference integrity

Every shipped sample asset is reference-complete before distribution.

At minimum, acceptance must verify after normal Package Manager import:

- Boot root -> success configuration;
- each scenario configuration -> intended startup sequence/destination/splash;
- startup-sequence entries -> intended sample step definitions;
- `LaunchDestination` -> valid Laboratory destination scene path;
- destination `EchoDirectSceneInitializer` -> imported Laboratory direct-scene configuration;
- direct-scene configuration -> intended imported Laboratory root prefab/configuration;
- presentation references -> valid status/splash surfaces;
- duplicate fixture -> disabled by default;
- canonical success configuration -> restored before evidence capture/commit.

A missing reference is a checkpoint failure. The user must never be instructed to
hand-assign a missing shipped reference merely to make the sample pass.

### 4.5 Existing production contracts remain authoritative

The sample uses the existing:

- `EchoLaunchRoot` authority and lifecycle;
- `StartupSequenceRunner` semantics;
- configuration/destination/splash schemas;
- plain uGUI presentation;
- initial destination loader;
- `EchoDirectSceneInitializer`;
- Setup/Repair tools;
- Validator;
- Simulator only as an independent diagnostic comparison tool, never as the
  Laboratory runtime.

FL-M5-07 does not invent sample-specific equivalents of these systems.

### 4.6 Conditional imported-sample candidate isolation

No production Editor change is assumed in advance.

If importing the Laboratory demonstrably causes `Assets/Samples/**` scenes or
assets to become automatic Setup candidates and this changes normal Setup
planning or regression results, FL-M5-07 may make one bounded correction:

- exclude standard imported-sample roots from **automatic** candidate discovery;
- keep explicit user selection of those assets possible;
- add focused tests proving both behaviors;
- do not broaden the exclusion to arbitrary user folders;
- do not change Setup ownership, write behavior, or repair authority.

If no such defect is reproduced, no Setup code changes are authorized.

---

## 5. Planned Sample Source Tree

The exact leaf names may adjust during implementation only when Unity serialization
requires it, but responsibility may not expand beyond this shape:

```text
Samples~/First Light Standalone Test Lab/
├── README.md
├── Runtime/
│   ├── EchoDevGames.EchoLaunch.Samples.StandaloneLab.asmdef
│   ├── Readout/
│   │   └── LaboratoryReadout.cs
│   └── Steps/
│       ├── LaboratoryImmediateSuccessStep.cs
│       ├── LaboratoryTimedProgressStep.cs
│       ├── LaboratoryWarningStep.cs
│       ├── LaboratoryRecoverableFailureStep.cs
│       └── LaboratoryBlockingFailureStep.cs
├── Scenes/
│   ├── FirstLight_Boot_Lab.unity
│   └── FirstLight_Destination_Lab.unity
├── Configuration/
│   ├── SuccessConfiguration.asset
│   ├── TimedProgressConfiguration.asset
│   ├── WarningConfiguration.asset
│   ├── RecoverableConfiguration.asset
│   ├── BlockingConfiguration.asset
│   ├── InvalidDestinationConfiguration.asset
│   ├── LaboratoryDirectSceneConfiguration.asset
│   ├── <startup sequences and step definitions>
│   ├── <valid and invalid LaunchDestination assets>
│   └── <Laboratory SplashSequence>
├── Prefabs/
│   └── EchoLaunchRoot_Laboratory.prefab
└── Art/
    └── <redistributable placeholder splash image>
```

A separate direct-scene scene is not created unless the destination scene cannot
cleanly prove LAB-008 and LAB-009.

---

## 6. Package Files Authorized to Change

### Always authorized

- `Packages/com.echodevgames.echo-launch/package.json`
- `Packages/com.echodevgames.echo-launch/Samples~/First Light Standalone Test Lab/**`
- package-owned static/source-contract tests needed to prove sample declaration,
  distribution shape, forbidden dependencies, and removable boundary
- Unity `.meta` files for those new package/sample/test assets

### Conditionally authorized only after reproduced evidence

- `Packages/com.echodevgames.echo-launch/Editor/Setup/EchoLaunchProjectSnapshotCollector.cs`
- focused tests for the same collector

The conditional files may change only for the narrow imported-`Assets/Samples`
automatic-discovery correction in section 4.6.

### Not authorized

- `Runtime/Core/**`
- `Runtime/Reporting/**`
- `Runtime/SceneLoading/**`
- `Runtime/Presentation/**`
- `Runtime/Development/**`
- `Presentation.UGUI/**`
- `Editor/Simulation/**`
- `Editor/Validation/**`
- Setup Apply/Repair write services
- project `Assets/**` as committed production content
- `ProjectSettings/**`

If a manual Laboratory run exposes a defect in any not-authorized surface, stop
and record the reproduction before expanding the checkpoint.

---

## 7. Implementation Sequence

### Step A — Manifest and static sample shell

1. Add exactly one `samples` declaration to `package.json`.
2. Add the sample root, README, sample assembly, and source folders.
3. Add static/package contract tests that prove:
   - exactly one sample entry;
   - correct display name/path;
   - required authored source files exist;
   - no shipped sample generator/Editor authoring service exists;
   - sample assembly has no peer-package reference.
4. Compile and run focused EditMode tests.

**Stop gate:** package still compiles with no sample imported into `Assets`.

### Step B — Fully authored sample assets

Create the final serialized sample scenes, configurations, sequences, step
assets, destination assets, splash asset/art, root prefab, direct-scene
configuration, and duplicate fixture.

These assets may be produced with a disposable development-only process outside
the committed package if convenient, but no generator used solely to manufacture
the shipped Laboratory may remain in the distributable package.

**Stop gate:** source YAML/meta references are stable and the package itself
still compiles before sample import.

### Step C — Normal Package Manager import

1. Open Package Manager.
2. Confirm exactly one sample is listed.
3. Import it normally.
4. Allow Unity to compile.
5. Confirm no automatic Play Mode, Validator, Simulator, Setup/Repair, or Build
   Settings mutation occurred.
6. Verify the imported reference-integrity checklist from section 4.4.

**Stop gate:** any missing serialized reference blocks the checkpoint and is
fixed in the shipped sample source, never by preserving a local hand repair.

### Step D — Regression gate with sample imported

Run:

```text
Compilation: expected 0 errors / 0 warnings
Complete EditMode: baseline 290 + approved new tests, 0 failures
Runtime Play Mode: retained 503, 0 failures
```

If existing tests change merely because the imported sample exists, first
identify whether that is a valid sample-isolation defect. Only section 4.6 may
authorize a Setup collector correction.

### Step E — Manual LAB-001 through LAB-010

Run the user-visible Laboratory matrix exactly as specified in section 8.
Restore canonical sample state after each deliberate mutation.

### Step F — Removal and repeatability

- LAB-011: remove the imported sample and prove package compilation/tool access.
- Reimport the sample and prove reference integrity again.
- LAB-012: run Setup Apply/Repair repeatability against their approved test
  project state three times and confirm no duplicate/silent overwrite behavior.
- Restore Build Settings and any project acceptance residue.

### Step G — Final regression and repository cleanup

- Complete EditMode green.
- Complete Runtime Play Mode green.
- `git diff --check` clean.
- No project `Assets/Samples` acceptance copy is staged or committed.
- No Build Settings or solution-file residue is staged.
- Only authorized package/test files are staged.

---

## 8. Manual Acceptance Matrix

| ID | Setup / action | Required result |
|---|---|---|
| LAB-001 | Import sample; explicitly place Boot Lab and destination in temporary acceptance Build Settings; play Boot Lab with success configuration | One authority; ordered success steps; destination activates; terminal report `Completed` |
| LAB-002 | Use timed-progress configuration | UI remains responsive; determinate progress advances; timing/progress evidence is visible and final report completes |
| LAB-003 | Use warning configuration | Warning is retained; traversal continues; destination activates; report completes |
| LAB-004 | Temporarily clear required root configuration | Blocks before step execution with `ELAUNCH-CFG-001`; no destination load |
| LAB-005 | Use blocking-failure configuration | Blocking result stops traversal; later step remains unvisited; destination does not load |
| LAB-006 | Enable authored duplicate-root fixture | First claimant runs once; duplicate emits `ELAUNCH-ROOT-001` and performs zero launch side effects |
| LAB-007 | Use invalid-destination configuration | Preflight blocks with `ELAUNCH-DEST-001`; startup executors do not begin |
| LAB-008 | Open destination scene directly with no pre-existing authority; Play | Exactly one `DirectSceneDevelopment` authority is created; active destination is not reloaded; report completes truthfully |
| LAB-009 | Enter destination with an existing valid authority | Initializer reuses authority and creates no duplicate |
| LAB-010 | Attempt splash skip before authored minimum duration | Early skip is latched/blocked until policy allows it; accepted timing matches existing splash contract |
| LAB-011 | Remove imported sample from a clean supported project | EchoLaunch Runtime/Editor still compile and Setup/Validator/Simulator remain available |
| LAB-012 | Re-run approved Setup and Repair workflow three times | No duplicate root/configuration/Boot/build entry and no silent overwrite; repeated settled state is `NoChanges` where applicable |

Expected simulated or duplicate warnings must match the existing contracts. No
unexpected Console error or warning is accepted.

---

## 9. Automated Evidence Mapping

FL-M5-07 should reuse existing behavior tests rather than clone production
semantics into sample tests.

- LAB-001/003/005/007: existing root/runner/preflight/handoff tests plus new
  sample source/reference checks.
- LAB-002: existing multi-frame/progress/timing tests plus manual imported view.
- LAB-004: existing configuration preflight tests plus manual imported mutation.
- LAB-006: existing duplicate authority tests plus manual authored fixture.
- LAB-008/009: existing Direct Scene runtime tests plus manual imported scenes.
- LAB-010: existing splash timing/skip PlayMode tests plus manual imported view.
- LAB-011: manifest/sample-removal clean-project proof.
- LAB-012: retained Setup Apply/Repair repeatability tests plus manual reruns.

New automated tests should test new sample/distribution facts. They should not
reimplement already-proven runner policy solely to inflate test counts.

---

## 10. Drift-Control Rules During Implementation

At every failure:

1. Record the exact failing asset/test/diagnostic.
2. Identify whether the failure belongs to new sample content or an existing
   First Light contract.
3. Fix sample content first when the package behavior is already correct.
4. Do not broaden production code to make a sample easier to author.
5. Do not add an automatic generator to repair a serialization/reference mistake.
6. Do not carry forward code from the archived discarded branch by default.
7. If an existing production contract must change outside section 4.6, stop and
   authorize that correction separately before editing it.

This is the central anti-drift rule for FL-M5-07.

---

## 11. Completion Criteria

FL-M5-07 is complete only when:

- Package Manager shows exactly one First Light Standalone Test Lab sample.
- Normal import produces a reference-complete Laboratory without hand repair.
- Import causes no hidden authoring or Build Settings side effects.
- All twelve LAB cases pass.
- Complete automated suites pass with no failures or ignored tests introduced by
  the checkpoint.
- Sample removal leaves the package healthy.
- Reimport works again.
- Setup/Repair repeatability remains intact.
- No unrelated package Runtime/Editor behavior changed.
- No project acceptance residue is staged.
- Documentation is reconciled to the implementation actually shipped.

---

## 12. Rollback

Before implementation, the authority commit is the rollback point.

During implementation:

- do not commit imported `Assets/Samples` acceptance copies;
- do not commit temporary Build Settings changes;
- keep each conditional production-code correction isolated and evidence-backed;
- if the Laboratory cannot pass without broad runtime/setup redesign, discard the
  implementation attempt and return to the authority checkpoint instead of
  widening scope in place.

---

## 13. Explicit Stop Point

After the Laboratory is fully validated, stop for documentation closeout.

Do not continue automatically into:

- release packaging;
- beta versioning;
- external adoption;
- report export;
- migration/receipt/uninstall/recovery;
- another package.

The next checkpoint is selected only after FL-M5-07 implementation evidence and
documentation are reconciled.
