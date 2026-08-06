# FL-M5-07 — Standalone Test Laboratory and Importable Package Sample Checkpoint Build Plan

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-07`
- Milestone: M5 closure / standalone MVP evidence before M6
- Title: Standalone Test Laboratory and Importable Package Sample
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Authority baseline: `e28ff09`
- Unity baseline: `6000.3.8f1`
- Status: Authority prepared; implementation locked until authority commit

## Purpose

Implement one explicit importable First Light package sample that proves the
complete visible MVP launch loop in isolation and remains removable without
affecting the production package.

## Starting Evidence

```text
Branch:            main
HEAD:              e28ff09
origin/main:       e28ff09
Working tree:      clean
Compilation:       0 errors, 0 warnings
EditMode:          290 passed
Runtime PlayMode:  503 passed
Total automated:   793 passed
```

## Authorized Outcome

FL-M5-07 may deliver:

- one `package.json` sample declaration
- one `Samples~/First Light Standalone Test Lab` payload
- Boot and Destination Laboratory scenes
- optional third Direct Scene only if evidence requires it
- neutral splash media
- visible status/destination readout
- sample-owned configuration/sequence/destination/splash/direct assets
- immediate, timed-progress, warning, recoverable-failure, and blocking
  public-API sample steps
- visible pre-authored scenario selection
- duplicate-root fixture
- sample README and reset/removal/reimport guide
- package-level manifest/inventory/dependency tests
- focused Laboratory tests where practical
- package-qualified `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012`
- a narrow existing-contract correction only if the Laboratory exposes a
  reproducible checkpoint-owned defect

## Explicitly Unauthorized

- M6 project adoption or adapter work
- optional package bridges
- automatic sample import/installation
- automatic Build Settings modification
- automatic Setup/Apply/Repair invocation
- production Runtime dependency on sample code/content
- internal friend access for sample assemblies
- reflection or hidden discovery
- project-specific runtime references
- persistent Laboratory service
- normal mid-game scene-flow ownership
- report export
- build hooks
- migration
- receipts
- uninstall/reset implementation
- crash-persistent recovery
- player-build/external-adoption/performance claims
- unrelated refactors

## Approved Package Shape

```text
Packages/com.echodevgames.echo-launch/
├── package.json
├── Samples~/
│   └── First Light Standalone Test Lab/
│       ├── README.md
│       ├── Runtime/
│       │   ├── EchoDevGames.EchoLaunch.Samples.StandaloneLab.asmdef
│       │   ├── Steps/
│       │   ├── Readout/
│       │   └── Support/
│       ├── Scenes/
│       │   ├── FirstLight_Boot_Lab.unity
│       │   └── FirstLight_Destination_Lab.unity
│       ├── Configuration/
│       ├── Prefabs/
│       └── Art/
└── Tests/
    └── Editor/
        └── Samples/
```

Exact subfolders may be refined for Unity serialization, but ownership and
dependency boundaries may not change.

## Build Sequence

### Phase 0 — Authority and baseline gate

1. Confirm `HEAD == e28ff09`.
2. Confirm `main == origin/main`.
3. Confirm clean working tree.
4. Commit/push authority.
5. Record authority commit.
6. Reconfirm `0` compile errors/warnings.
7. Run baseline `290` EditMode and `503` Runtime PlayMode.
8. Stop on unexplained baseline drift.

### Phase 1 — Manifest and inventory contract

1. Add exactly one `samples` entry to `package.json`.
2. Use display name `First Light Standalone Test Lab`.
3. Point to one stable sample path.
4. Keep package version `0.1.0` unless separately authorized.
5. Add package-level EditMode tests for:
   - valid JSON
   - exactly one sample declaration
   - approved display name/path/description
   - required README and scenes
   - required sample-step source inventory
   - forbidden peer-package identifiers
   - forbidden project-assembly references
   - no core assembly reference to sample assembly

### Phase 2 — Public-API sample assembly

1. Create the sample-owned runtime assembly.
2. Default `autoReferenced` to `false` unless imported-sample behavior proves
   a documented exception necessary.
3. Reference only public First Light Runtime and approved Unity modules.
4. Add no `InternalsVisibleTo`.
5. Implement sample steps:
   - immediate success
   - timed progress
   - warning
   - recoverable failure
   - blocking failure
6. Use stable sample IDs/display names.
7. Use Laboratory-specific sample diagnostics.
8. Implement a narrow visible readout using public lifecycle/report data.
9. Add focused step/readout tests where practical.

### Phase 3 — Temporary Unity authoring workspace

1. Create a temporary project-owned authoring folder outside final payload.
2. Author through Unity:
   - configurations
   - startup sequences
   - destination assets
   - splash sequence
   - Direct Scene configuration
   - root/status sample prefab if needed
   - Boot scene
   - Destination scene
3. Use package templates/public components.
4. Confirm current schemas and project-relative destination identity.
5. Confirm Destination can demonstrate Direct Scene entry.
6. Add a third scene only after documenting why two scenes fail.
7. Validate references in Unity.
8. Produce/copy final distribution payload.
9. Do not stage temporary workspace.

### Phase 4 — Visible scenario fixtures

Prepare explicit pre-Play fixtures for:

1. canonical success
2. timed progress
3. warning continuation
4. recoverable-failure continuation
5. blocking failure
6. missing configuration
7. invalid destination
8. duplicate root
9. Direct Scene creation
10. Direct Scene reuse
11. early splash skip

Scenario switching must use Inspector references, separate configurations, or
documented fixture activation. Runtime asset mutation is prohibited.

### Phase 5 — Sample README

Document:

1. Package Manager import steps.
2. Imported-location pattern.
3. Boot/Destination scene locations.
4. Explicit Build Settings order.
5. Scenario selection.
6. Direct Scene tests.
7. Duplicate fixture controls.
8. Splash skip test.
9. Report/readout evidence.
10. Reset procedure.
11. Removal procedure.
12. Reimport procedure.
13. Sample-assets-not-canonical warning.
14. No-peer-package requirement.

### Phase 6 — Package Manager import gate

1. Open Package Manager and select First Light.
2. Confirm exactly one sample.
3. Import explicitly.
4. Record actual imported path.
5. Confirm expected inventory.
6. Require clean compile.
7. Confirm import alone changed neither Build Settings nor ProjectSettings.
8. Confirm no canonical setup assets were created automatically.

### Phase 7 — Focused automated evidence

Run:

- manifest/inventory/dependency tests
- sample step contract tests
- focused imported Laboratory tests where practical
- affected root/presentation/destination/Direct Scene/setup/repair/Validator/
  Simulator tests

Tests may not rely on machine-specific absolute imported paths.

### Phase 8 — Manual Laboratory acceptance

#### ELAUNCH-LAB-001 — Canonical success

- Put Boot first and Destination second in Build Settings.
- Use success configuration.
- Play Boot.
- Confirm one root, splash/status, ordered steps, destination activation, and
  successful report.

#### ELAUNCH-LAB-002 — Timed progress

- Use timed configuration.
- Confirm responsive visible progress and ordered timing evidence.

#### ELAUNCH-LAB-003 — Warning continuation

- Use warning configuration.
- Confirm warning evidence and successful continuation/destination.

#### ELAUNCH-LAB-004 — Missing configuration

- Remove required root configuration.
- Confirm block before steps and `ELAUNCH-CFG-001`.
- Restore reference.

#### ELAUNCH-LAB-005 — Blocking failure

- Use blocking configuration.
- Confirm later work stops, destination does not load, and report blocks.

#### ELAUNCH-LAB-006 — Duplicate root

- Enable second root fixture.
- Confirm one accepted authority, zero duplicate side effects, and
  `ELAUNCH-ROOT-001`.

#### ELAUNCH-LAB-007 — Invalid destination

- Use invalid destination fixture.
- Confirm preflight block, `ELAUNCH-DEST-001`, and no load.

#### ELAUNCH-LAB-008 — Direct Scene creation

- Open Destination with no root.
- Play.
- Confirm one development root, no scene reload, Direct Scene report mode.

#### ELAUNCH-LAB-009 — Direct Scene reuse

- Enable existing valid root in Destination.
- Play.
- Confirm reuse and no duplicate.

#### ELAUNCH-LAB-010 — Splash minimum duration

- Start skippable splash with positive minimum.
- Attempt early skip and confirm it remains.
- Skip after minimum and confirm exactly one advance.

#### ELAUNCH-LAB-011 — Sample removal

- Exit Play.
- Delete imported sample.
- Restore test Build Settings.
- Confirm Runtime/Editor compile and Setup/Validator/Simulator open.
- Confirm package tests do not require imported sample.

#### ELAUNCH-LAB-012 — Reimport and repeatability

- Reimport one clean sample copy.
- Run approved Setup/Repair repeatability three times.
- Confirm no duplicates/overwrite and clear sample/canonical separation.

### Phase 9 — Complete regression

After any accepted change/fix:

1. Run complete EditMode.
2. Run complete Runtime PlayMode.
3. Require final Console `0` errors/warnings.
4. Record actual totals.
5. Re-run affected manual cases.

### Phase 10 — Repository cleanup

Before staging:

1. Exit Play and close Unity.
2. Remove imported acceptance copy.
3. Remove temporary authoring workspace.
4. Restore Build Settings and ProjectSettings.
5. Restore generated solution/project files.
6. Remove temporary reports/backups.
7. Run `git diff --check`.
8. Confirm only authorized implementation scope remains.

### Phase 11 — Implementation staging

Stage only:

- `package.json`
- approved sample payload
- approved package-level sample tests
- narrowly justified checkpoint-owned existing-package fix
- required metadata for final supported layout

Do not stage:

- authority/closeout docs in implementation commit
- imported sample under `Assets`
- temporary authoring assets
- Build/ProjectSettings
- generated solution/project files
- unrelated package code

Inspect:

```text
git diff --cached --check
git status --short
git diff --cached --stat
```

### Phase 12 — Implementation commit

Suggested message:

```text
Implement First Light standalone laboratory sample
```

Push and confirm clean synchronized `main`.

### Phase 13 — Documentation closeout

Reconcile changelog, README, package Index/Architecture/Current Notes, suite
Current Notes, specification status, ADR log, package checkpoint record, test
report, and suite completion record.

Record actual commits, totals, imported path, all Laboratory results,
removal/reimport evidence, any narrow fix, and final scope.

Suggested message:

```text
Close out FL-M5-07 standalone laboratory checkpoint
```

## Stop Conditions

Stop if:

- baseline/working tree is wrong
- Package Manager does not show exactly one sample
- import mutates Build/ProjectSettings automatically
- sample requires internals, peer packages, or project assemblies
- serialized references break after import
- scenarios require Play-time asset mutation
- duplicate roots both produce side effects
- Direct Scene reloads destination
- sample removal breaks package/tools
- setup/repair overwrites sample assets
- proposed fix exceeds approved contract
- test residue appears in staging
- any failure is unexplained

## Completion Criteria

FL-M5-07 closes only when:

- authority precedes implementation
- exactly one sample is declared/importable
- Boot/Destination scenes work
- public-API sample steps work
- `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` pass or receive explicit
  approved issue disposition
- removal preserves package/tools
- reimport succeeds
- complete regressions pass
- no peer/core-sample dependency exists
- no test residue enters commit
- implementation and documentation commits are pushed
- documentation matches evidence
