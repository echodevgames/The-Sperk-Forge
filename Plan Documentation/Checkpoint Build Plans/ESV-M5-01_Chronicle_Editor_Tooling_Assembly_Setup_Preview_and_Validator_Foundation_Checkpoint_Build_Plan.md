# ESV-M5-01 — Chronicle Editor Tooling Assembly, Setup Preview, and Validator Foundation Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-01
**Status:** ACTIVE / AUTHORIZED
**Planning baseline:** `e63d83f` — `Close out ESV-M4-R4 and Chronicle M4`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.44.0 / ESV-D-037
**Incoming focused Chronicle regression floor:** **660 / 660 passed, 0 failed**
**Unity baseline:** 6000.3.8f1
**Owner:** Jesse “Echo” Adams / EchoDevGames

---

## 1. Checkpoint purpose

Start Chronicle M5 with the smallest safe tooling foundation: isolate all UnityEditor dependencies behind a package Editor assembly, establish preview-before-mutation Setup behavior, create new current-schema configuration assets without clobbering existing project content, and establish deterministic read-only validation.

M5-01 treats the completed M4 runtime as frozen input. It does not use the tooling milestone as permission to reshape runtime save semantics.

## 2. Why this checkpoint is first

The approved M5 outcome includes Setup, Validator, Browser, Simulator, and the standalone Save Laboratory. All later tools depend on two safety properties first:

1. Editor code must be physically incapable of leaking into the runtime assembly.
2. Tooling must distinguish preview/inspection from explicit mutation before it is allowed near project assets or sandbox save data.

M5-01 establishes those rails before inspection, corruption simulation, repair previews, or Laboratory controls are introduced.

## 3. Authority decision — ESV-D-037

M5 begins with a strict Editor-only Setup/Validator foundation.

- `EchoDevGames.EchoSave.Editor` is Editor-only and references Runtime; Runtime never references Editor.
- Setup computes a deterministic plan before mutation.
- M5-01 Setup may create a **new** project-owned schema-2 `EchoSaveConfiguration` only after explicit Apply.
- Existing assets are never silently overwritten, upgraded, moved, or repaired.
- M5-01 authors only fields already represented by runtime schema 2.
- Validator execution is read-only.
- Later M5 checkpoints own broader configuration assets, Browser/Inspector, simulation, support export, sandbox mutation, and the Save Laboratory.

## 4. In scope

### 4.1 Editor assembly

Create the package Editor assembly boundary:

```text
Packages/com.echodevgames.echo-save/Editor/
└── EchoDevGames.EchoSave.Editor.asmdef
```

Required assembly rules:
- include only Editor;
- explicit reference to `EchoDevGames.EchoSave.Runtime`;
- no Runtime assembly reference back to Editor;
- no peer Echo package dependency;
- no sample dependency;
- tooling API may use UnityEditor because the assembly is Editor-only.

### 4.2 Setup plan model and service

Add Editor-only types sufficient to express a deterministic setup preview, for example:

```text
Editor/Setup/
  EchoSaveSetupWindow.cs
  EchoSaveSetupService.cs
  EchoSaveSetupRequest.cs
  EchoSaveSetupPlan.cs
  EchoSaveSetupResult.cs
```

Exact internal type split may vary if behavior remains equivalent.

The setup request must cover:
- explicit target asset path under `Assets/`;
- storage-root directory segment;
- slot-policy mode;
- fixed/configured/profile bound values relevant to the selected mode.

Preview must report at minimum:
- normalized destination path;
- whether the destination is available or already occupied;
- current configuration schema that will be created;
- normalized storage root;
- selected slot policy and effective capacity;
- files/assets that Apply would create;
- blockers/advisories;
- whether Apply would be a create, no-change/reuse, or rejection.

Preview performs **zero project mutation**.

### 4.3 Setup Apply

Apply is explicit user action.

M5-01 Apply may only:
- create a new `EchoSaveConfiguration` asset at the exact previewed project path;
- serialize schema 2 values already approved by R2;
- save/import the new asset through Unity Editor APIs;
- select/ping the created asset as UX convenience if desired.

Apply must not:
- overwrite an existing asset;
- silently edit an existing configuration;
- create a production save directory;
- create a root GameObject/prefab;
- edit scenes;
- create fixed slot identities/templates;
- create participant scripts;
- create Laboratory/sandbox data.

Repeat invocation on an occupied target must be bounded and truthful, never destructive.

### 4.4 Setup window

Menu path:

```text
Tools > Sperk’s Forge > The Chronicle > Setup
```

The window must visibly separate:
- authored inputs;
- Preview;
- previewed plan/result;
- Apply.

Apply remains unavailable when preview contains a blocker or no longer matches current inputs/project state.

### 4.5 Validator foundation

Add Editor-only validation types, for example:

```text
Editor/Validation/
  EchoSaveValidatorWindow.cs
  EchoSaveValidationRule.cs
  EchoSaveValidationIssue.cs
  EchoSaveValidationReport.cs
  EchoSaveValidationService.cs
```

Menu path:

```text
Tools > Sperk’s Forge > The Chronicle > Validator
```

Validation is deterministic and read-only.

Initial rule coverage:
- `ESV-VAL-001` — missing configuration selection;
- `ESV-VAL-002` — unsafe/empty storage-root subpath;
- `ESV-VAL-003` — duplicate Chronicle roots in the analyzed loaded-scene setup;
- `ESV-VAL-009` — Runtime assembly references `UnityEditor`;
- `ESV-VAL-015` — invalid current slot-policy mode/bounds.

The Validator may surface schema-1 compatibility as an advisory, but M5-01 may not rewrite the asset.

### 4.6 Deterministic report behavior

Validation issues must carry bounded structured truth:
- stable check ID;
- severity;
- concise message;
- optional project object/path context;
- whether a fix exists;
- whether automatic mutation is permitted.

Ordering must be deterministic, preferably severity then check ID then context.

Running validation twice without project changes must produce equivalent issue truth and perform zero writes.

## 5. Explicitly out of scope

M5-01 does **not** authorize:
- `EchoSaveConfiguration` schema 3;
- serializer/backend provider selection authoring;
- full retention/limit/recovery-policy authoring;
- fixed slot templates/default IDs;
- root prefab or scene instance creation;
- repair of existing configuration references;
- test-participant script generation;
- sandbox profile creation;
- `ESV-VAL-004` through `ESV-VAL-008` or `ESV-VAL-010` through `ESV-VAL-014` unless a rule is already fully representable without inventing new runtime/configuration authority;
- unsupported DTO-shape analyzer completion for ESV-T-081;
- Save Browser / Generation Inspector / Migration Graph;
- Failure Simulator / Recovery Planner / Test Data Generator;
- redacted support snapshot exporter;
- automatic/configured recovery fallback;
- persistent catalog cache;
- quarantine/incomplete-generation cleanup;
- unknown-payload prune tooling;
- public restore-from-trash or permanent erase;
- direct-scene initializer;
- sample scene/UI or any LAB-001 through LAB-032 execution;
- clean-project/distribution/performance/stress/integration/adoption/release qualification;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

## 6. Runtime freeze rule

M5-01 is expected to require **no Runtime C# behavior changes**.

If implementation discovers that a runtime change is genuinely required, stop before editing Runtime and reconcile authority. A trivial compile-access seam is not automatically pre-approved merely because it would make Editor code easier. Prefer Editor serialization/inspection APIs over widening runtime surface.

`IEchoSaveService`, save document versions, participant contracts, storage base contracts, publication/recovery semantics, slot policy behavior, and M4 diagnostic truth remain unchanged.

## 7. Asset safety rules

1. Preview before Apply.
2. No overwrite by default.
3. No asset mutation during Validator runs.
4. No production save-directory creation from Setup preview/apply.
5. No Laboratory path exists yet, so M5-01 cannot claim sandbox collision proof.
6. Use Unity Undo where an Editor operation creates/modifies Unity project objects and Undo is meaningful.
7. Report exact created/reused/rejected truth.

## 8. Focused automated evidence

At minimum add focused Editor tests for:

### Assembly / independence
- Editor asmdef is Editor-only;
- Runtime asmdef has no Editor reference;
- no peer Echo package references are introduced.

### Setup preview
- preview performs zero asset writes;
- invalid/non-Assets target rejects;
- occupied target rejects/no-clobbers;
- safe root and each slot-policy mode produce deterministic preview truth;
- invalid active policy bound blocks Apply;
- previewed effective capacity matches runtime schema-2 policy resolution.

### Setup apply
- valid Apply creates exactly one new configuration asset;
- created asset is current schema 2;
- authored root/slot policy deserialize to expected values;
- second Apply to the same path does not overwrite;
- Apply does not create storage, root GameObject, scene object, prefab, or Laboratory data.

### Validator
- missing configuration produces ESV-VAL-001;
- unsafe/empty root produces ESV-VAL-002;
- duplicate loaded-scene roots produce ESV-VAL-003;
- valid Runtime assembly isolation passes ESV-VAL-009;
- invalid slot policy produces ESV-VAL-015;
- deterministic issue ordering;
- validation performs zero project mutation.

### Regression
- full focused `EchoDevGames.EchoSave.Tests.Editor` run remains at or above **660 / 660** with zero failures.

Do not predict the final discovered count. Record Unity’s actual result.

## 9. Registry disposition rule

M5-01 does not automatically change the R4 registry totals.

In particular:
- ESV-T-004 remains Deferred until the authoritative packaging/player assembly qualification is actually run, even if M5-01 adds useful assembly-audit evidence;
- ESV-T-013 and ESV-T-014 remain Deferred because configured serializer/backend provider selection is not yet authored by M5-01;
- ESV-T-081 remains Deferred until an explicit unsupported-shape validator/analyzer proves the required actionable failure;
- Laboratory, performance/stress, integration/adoption, and release rows remain Deferred.

## 10. Implementation target shape

Expected implementation is Editor/test-only, approximately:

```text
Packages/com.echodevgames.echo-save/Editor/
  EchoDevGames.EchoSave.Editor.asmdef
  Setup/...
  Validation/...

Packages/com.echodevgames.echo-save/Tests/Editor/
  EchoSaveSetupTests.cs
  EchoSaveValidationTests.cs
  EchoSaveEditorAssemblyBoundaryTests.cs
```

Exact file count is not authority. Behavioral scope is.

## 11. Manual Unity check

After automated tests are green:
1. open Chronicle Setup;
2. preview a valid configuration under a temporary project test path;
3. confirm no asset exists before Apply;
4. Apply and confirm exactly one configuration asset is created;
5. repeat preview/apply and confirm no overwrite;
6. open Validator and confirm a deterministic clean/issue report;
7. remove only the temporary test asset created for this manual proof.

Do not create or touch production save data.

## 12. Closeout requirements

M5-01 closes only when:
- implementation is committed;
- Unity compiles cleanly;
- focused Chronicle Editor suite is green at the actual discovered total and not below 660;
- manual Setup/Validator sanity proof is recorded if performed;
- package README/CHANGELOG/Documentation Index and both Current Notes are reconciled as applicable;
- this checkpoint records implementation commit and actual evidence;
- no unrelated later-M5 capability is smuggled into the checkpoint.

M5-01 completion does not complete M5.

## 13. Next-gate rule

No M5-02 implementation is activated by M5-01 activation or implementation success. The next slice requires a separate bounded authority/activation commit after M5-01 closeout.
