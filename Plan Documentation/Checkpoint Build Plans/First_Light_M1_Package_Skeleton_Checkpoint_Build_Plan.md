# First Light M1 — Package Skeleton Checkpoint Build Plan

**Checkpoint ID:** FL-M1-01  
**Version:** 1.3.0  
**Status:** Active and authorized by SUITE-DOC-33; implementation not started  
**Package:** First Light (`EchoLaunch`)  
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0  
**Milestone:** M1 — Skeleton  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository/workspace:** Clean Sperk’s Forge Unity development repository  
**Unity baseline:** Unity 6000.3.8f1  
**Public Unity floor:** Unity 6000.0  
**Workflow authority:** SFGSS-005 v1.4.0  
**Last updated:** August 3, 2026

> This checkpoint creates the labeled crate, shelves, and inventory card. It does not yet place a launch engine inside.

> **Activation notice:** SUITE-DOC-33 passed with advisory on August 4, 2026 and explicitly activated this checkpoint. Execute only the scope below. Later package checkpoints require their own just-in-time learning reviews under SFGSS-ADR-004.


---

## 1. Purpose and observable outcome

Create the smallest valid, installable **First Light — Startup and Launch** UPM package skeleton.

When this checkpoint is complete:

- Unity recognizes `com.echodevgames.echo-launch` as a local embedded package.
- The package has valid Runtime, Editor, Runtime Test, and Editor Test assembly definitions.
- The package contains its required root documentation and a routed documentation shell.
- The project compiles with zero new errors or warnings caused by the package.
- The package contains **no launch behavior**.

The user-visible result is a package that appears correctly in Package Manager and is ready for the next checkpoint without introducing scene objects, menus, prefabs, ScriptableObjects, or runtime side effects.

---

## 2. Starting conditions

- SUITE-DOC-33 — Initial Implementation Readiness Gate passed with advisory and explicitly activated FL-M1-01.
- PKG-LEARN-001 — First Light learning review is complete.
- The current approved SFGSS-000 is present.
- SFGSS-005 v1.4.0 or later is present.
- First Light specification v1.1.0 is present.
- SFGSS-ADR-001 and SFGSS-INT-FOUNDATION-001 are present.
- The Unity project opens in Unity 6000.3.8f1 with no existing compile error.
- `Packages/com.echodevgames.echo-launch/` does not already contain an implementation, or any existing contents have been reviewed before the plan is applied.
- The working tree is reviewed so unrelated changes are not overwritten.

If any starting condition is false, stop and reconcile it before creating package files.

---

## 3. Authority and architectural constraints

First Light owns initial runtime claim, ordered startup, launch-only presentation, structured launch reporting, direct-scene development initialization, and final launch handoff. None of that behavior is implemented in this checkpoint.

The skeleton must preserve these future constraints:

- Package ID: `com.echodevgames.echo-launch`.
- Runtime namespace: `EchoDevGames.EchoLaunch`.
- Runtime assembly: `EchoDevGames.EchoLaunch.Runtime`.
- Editor assembly: `EchoDevGames.EchoLaunch.Editor`.
- No peer Sperk’s Forge runtime dependency.
- No `UnityEditor` reference in the runtime assembly.
- uGUI remains the approved future default presenter dependency, but no presentation code is created here.
- No mandatory shared `EchoCore` package.
- No project-specific scene, save, audio, UI, or gameplay references.

---

## 4. Scope

FL-M1-01 authorizes only:

1. The UPM package root.
2. `package.json`.
3. Runtime, Editor, Runtime Test, and Editor Test assembly definitions.
4. Root package documentation files.
5. A minimal `Documentation~` routing shell.
6. A package-local development `Current Notes.md` page.
7. Unity-generated `.meta` files for the authorized package files and directories.
8. Compile, Package Manager, removal/re-add, and documentation-route validation.

---

## 5. Explicit exclusions and stop point

Do **not** create during this checkpoint:

- C# runtime or Editor scripts.
- `EchoLaunchRoot` or any authority-claim code.
- Startup-step interfaces, definitions, executors, sequences, runners, reports, or diagnostic codes.
- ScriptableObjects or configuration assets.
- Prefabs, scenes, samples, splash art, or the Standalone Test Lab.
- uGUI presenters or TextMeshPro components.
- Setup windows, menu items, validators, simulators, migration tools, or Workshop facades.
- First Light bridges to any peer package.
- Project Boot scene or Build Settings changes.
- Runtime singletons, `DontDestroyOnLoad`, static mutable state, or service-locator access.

**Stop immediately after the package skeleton passes the acceptance tests.** The next tempting action, creating `EchoLaunchRoot.cs`, belongs to FL-M2-01 and is not authorized here.

---

## 6. Exact file manifest

Create only the following repository files during this checkpoint:

```text
Packages/com.echodevgames.echo-launch/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   │   ├── Installation.md
│   │   └── Quick Start.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Current Notes.md
│       └── Checkpoints/
│           └── FL-M1-01_Package_Skeleton.md
├── Runtime/
│   └── EchoDevGames.EchoLaunch.Runtime.asmdef
├── Editor/
│   └── EchoDevGames.EchoLaunch.Editor.asmdef
└── Tests/
    ├── Runtime/
    │   └── EchoDevGames.EchoLaunch.Tests.Runtime.asmdef
    └── Editor/
        └── EchoDevGames.EchoLaunch.Tests.Editor.asmdef
```

Unity will create matching `.meta` files. Commit them. Do not create empty `Core`, `Configuration`, `Steps`, `Reporting`, `Presentation`, `SceneLoading`, `Development`, `Setup`, `Validation`, `Samples~`, or Test Lab directories yet.

### 6.1 File purposes

| File | Purpose |
|---|---|
| `package.json` | UPM identity, Unity floor, description, author, and exact dependencies |
| `README.md` | Concise package boundary, development status, install route, and documentation links |
| `CHANGELOG.md` | Starts `0.1.0` development history with the skeleton checkpoint |
| `LICENSE.md` | Development-only rights notice until the suite licensing decision is approved |
| `Third Party Notices.md` | States that FL-M1-01 includes no third-party content |
| `Documentation~/Index.md` | Routes user and developer readers |
| User docs | Honest installation and “no behavior yet” quick-start state |
| Developer docs | Architecture boundary, current package notes, and checkpoint record |
| Runtime asmdef | Declares the future runtime namespace boundary without adding behavior |
| Editor asmdef | Declares Editor isolation and runtime reference |
| Test asmdefs | Reserve isolated test assemblies without adding tests prematurely |

---

## 7. Manifest decisions

### 7.1 Initial package version

Use:

```text
0.1.0
```

Version `0.x` communicates active initial development. It does not claim MVP completion or public stability.

### 7.2 Required manifest fields

`package.json` must include:

- `name`: `com.echodevgames.echo-launch`
- `version`: `0.1.0`
- `displayName`: `First Light — Startup and Launch`
- A plain technical description.
- `unity`: `6000.0`
- Author name: `Jesse "Echo" Adams / EchoDevGames`
- Documentation and repository metadata only when the final URLs are known and valid.
- An exact `com.unity.ugui` dependency version observed in the Unity 6000.3.8f1 baseline project.

Do not guess the uGUI version. During execution:

1. Inspect `Packages/packages-lock.json` and Package Manager.
2. Record the exact resolved `com.unity.ugui` version in the checkpoint evidence.
3. Use that exact version in `package.json`.
4. If uGUI is not installed, add the baseline-compatible released package through Package Manager, record the manifest/lockfile change, and re-run compile validation.

Do not add peer Echo package dependencies.

---

## 8. Assembly-definition contract

### 8.1 Runtime assembly

`EchoDevGames.EchoLaunch.Runtime.asmdef` must:

- Use name `EchoDevGames.EchoLaunch.Runtime`.
- Use root namespace `EchoDevGames.EchoLaunch`.
- Be auto-referenced.
- Include all runtime platforms by default.
- Exclude no platform at M1.
- Allow no unsafe code.
- Use no test flag.
- Reference no Editor assembly and no peer Echo assembly.

### 8.2 Editor assembly

`EchoDevGames.EchoLaunch.Editor.asmdef` must:

- Use name `EchoDevGames.EchoLaunch.Editor`.
- Use root namespace `EchoDevGames.EchoLaunch.Editor`.
- Include only `Editor`.
- Reference `EchoDevGames.EchoLaunch.Runtime`.
- Allow no unsafe code.

### 8.3 Test assemblies

Both test assemblies must:

- Set `autoReferenced` to `false`.
- Use `optionalUnityReferences: ["TestAssemblies"]`.
- Reference the assembly they are intended to test.
- Contain no test scripts in M1.

The Editor test assembly includes only `Editor` and references both Runtime and Editor. The Runtime test assembly references Runtime and keeps platform inclusion open for later EditMode/PlayMode decisions.

---

## 9. Documentation content requirements

### 9.1 README

The package README must say:

- What First Light will own.
- What it will not own.
- Current package version and development status.
- FL-M1-01 contains no runtime behavior.
- Supported Unity floor and tested baseline.
- Where the complete documentation lives.
- Where `Current Notes.md` lives.
- That installation is currently for development/testing only.

### 9.2 Changelog

Add a `0.1.0` entry for:

- UPM package skeleton.
- Assembly definitions.
- Documentation shell.
- No runtime behavior.

### 9.3 License notice

Until the suite license is approved, state clearly:

- Copyright belongs to Jesse “Echo” Adams / EchoDevGames.
- All rights are reserved.
- No public reuse or redistribution license is granted by the development repository yet.
- The notice will be replaced or revised before public distribution.

### 9.4 Third-party notice

State that no third-party code, art, audio, fonts, or sample content is included in FL-M1-01.

### 9.5 Package Current Notes

Initialize package-local notes with:

- Current checkpoint: FL-M1-01.
- Package implementation: skeleton only.
- Known blockers: none.
- Next action: run skeleton acceptance tests.
- Reminder to promote durable findings before closeout.

---

## 10. Implementation sequence

1. Confirm the starting conditions and clean compile.
2. Record the current Git status without modifying unrelated files.
3. Inspect the exact baseline `com.unity.ugui` version.
4. Create the package root and only the directories containing authorized files.
5. Create `package.json` with the verified dependency.
6. Create Runtime and Editor asmdefs.
7. Create Runtime Test and Editor Test asmdefs.
8. Allow Unity to import and create `.meta` files.
9. Resolve any skeleton-owned compile/import error without adding behavior.
10. Create root package documentation.
11. Create the `Documentation~` shell and package-local checkpoint record.
12. Reopen or refresh Package Manager and confirm package identity.
13. Run all acceptance tests in Section 12.
14. Reconcile suite and package Current Notes, test evidence, roadmap/status, and changelog.
15. Stop. Do not begin FL-M2-01.

---

## 11. Unity Editor setup

1. Open the clean Sperk’s Forge Unity project in Unity 6000.3.8f1.
2. Confirm the Console has no pre-existing compile error.
3. Open **Window → Package Manager**.
4. Confirm the exact installed uGUI version and record it.
5. Create the embedded package beneath the project `Packages/` folder.
6. Return to Unity and wait for package refresh and script compilation.
7. In Package Manager, select **In Project** and locate **First Light — Startup and Launch**.
8. Confirm:
   - Package ID is correct.
   - Version is `0.1.0`.
   - Description is technical and accurate.
   - No sample is listed.
9. Confirm no First Light menu item, scene object, prefab, asset, or runtime behavior exists.
10. Close and reopen the project once to confirm the package imports consistently.

No scene or Build Profile setup is authorized.

---

## 12. Validation and acceptance tests

| Test ID | Setup and action | Expected result | Evidence | Type |
|---|---|---|---|---|
| FL-M1-T-001 | Open project after adding package | Zero package-owned compile errors | Console screenshot/log or recorded result | Manual |
| FL-M1-T-002 | View Package Manager entry | Correct ID, display name, version, Unity floor, description | Package Manager observation | Manual |
| FL-M1-T-003 | Inspect package directory | Only authorized files and Unity `.meta` files exist | File-tree record | Manual/automated |
| FL-M1-T-004 | Inspect Runtime asmdef | No Editor or peer Echo reference | JSON review | Automated/manual |
| FL-M1-T-005 | Inspect Editor asmdef | Editor-only and references Runtime | JSON review | Automated/manual |
| FL-M1-T-006 | Inspect test asmdefs | TestAssemblies flag, no test scripts, correct references | JSON review | Automated/manual |
| FL-M1-T-007 | Search package for `.cs`, `.unity`, `.prefab`, `.asset` | No unauthorized behavior/content files | File search | Automated |
| FL-M1-T-008 | Restart Unity | Package imports and compiles consistently | Console result | Manual |
| FL-M1-T-009 | Temporarily move package outside `Packages/`, open/refresh, then restore | Unrelated project code remains compilable; restore returns package cleanly | Manual record; preserve Git state | Manual |
| FL-M1-T-010 | Open every README/documentation link | No broken route among files created in M1 | Link check | Automated/manual |
| FL-M1-T-011 | Inspect Git diff | No unrelated file changed; all package `.meta` files present | `git status`/diff review | Manual |
| FL-M1-T-012 | Compare package against checkpoint exclusions | No runtime authority, config, scene, prefab, sample, setup tool, or bridge exists | Closeout checklist | Manual |

All twelve tests must pass before FL-M1-01 closes.

---

## 13. Common failure symptoms and bounded fixes

| Symptom | Likely cause | Allowed fix |
|---|---|---|
| Package does not appear | Invalid JSON, wrong package path, duplicate package name | Validate `package.json`, package root, and Console |
| Package Manager reports invalid dependency | Guessed or unavailable uGUI version | Use the exact baseline-resolved version and refresh lockfile |
| Editor assembly compiles for Player | Missing `includePlatforms: ["Editor"]` | Correct the Editor asmdef |
| Runtime assembly references Editor | Incorrect asmdef reference | Remove Editor reference; Runtime may not depend upward |
| Test assembly is auto-referenced | Missing `autoReferenced: false` or TestAssemblies flag | Correct the test asmdef |
| Unity regenerates missing `.meta` files | Files were copied without committed metadata | Allow generation once, inspect, and commit stable `.meta` files |
| Unexpected `.cs`, prefab, or scene appears | Scope drift | Remove it from this checkpoint and document the correction |
| Documentation links break | Wrong relative path or missing file | Fix only the M1 documentation route |

Do not solve M1 failures by adding runtime scripts or setup code.

---

## 14. Rollback and recovery

To return to the starting state:

1. Preserve any unrelated working-tree changes.
2. Remove only `Packages/com.echodevgames.echo-launch/` and its `.meta` files if Unity created an external package-folder meta.
3. Restore `Packages/manifest.json` and `Packages/packages-lock.json` only if FL-M1-01 changed them for uGUI.
4. Refresh or reopen Unity.
5. Confirm the project returns to its original compile state.
6. Revert only the FL-M1-01 documentation updates if the checkpoint is abandoned.

No project-owned scene, prefab, configuration asset, or save data should exist to recover in this checkpoint.

---

## 15. Documentation reconciliation at closeout

Update:

- Root `Plan Documentation/Current Notes.md`.
- Package `Documentation~/Developer/Current Notes.md`.
- `Foundation_Wave_Specification_Roadmap.md` or the active implementation roadmap/status record.
- `Documentation~/Developer/Checkpoints/FL-M1-01_Package_Skeleton.md` with actual results.
- Package `CHANGELOG.md`.
- A new FL-M1-01 test report under `Plan Documentation/Test Reports/` when the tests are executed.
- The First Light specification only if implementation reveals a real design mismatch.
- SFGSS-000 or an ADR only if a suite-level architecture conflict appears.

Do not mark tests, commit, or push complete before evidence exists.

---

## 16. Commit and push plan

Preferred commit:

```text
echo-launch: complete FL-M1-01 package skeleton
```

The commit should contain:

- The complete authorized package skeleton and `.meta` files.
- Exact dependency/lockfile changes caused by M1.
- Package documentation and changelog.
- Checkpoint test report and reconciled Current Notes.

A separate immediately adjacent documentation commit is allowed when necessary. Push confirmation remains pending until supplied by the user.

---

## 17. Completion criteria

- [ ] Starting conditions verified.
- [ ] Exact uGUI baseline version recorded.
- [ ] Package Manager recognizes the package.
- [ ] Package version is `0.1.0`.
- [ ] Four assembly definitions are valid.
- [ ] Zero package-owned compile errors.
- [ ] No runtime or Editor scripts exist.
- [ ] No scene, prefab, asset, sample, menu, or setup tool exists.
- [ ] Root documentation and `Documentation~` routes work.
- [ ] Stable `.meta` files are present.
- [ ] FL-M1-T-001 through FL-M1-T-012 pass.
- [ ] Current Notes, changelog, checkpoint record, and test report are reconciled.
- [ ] Git diff contains no unrelated change.
- [ ] Commit and push evidence is recorded.
- [ ] Work stops before `EchoLaunchRoot.cs` is created.

---

## 18. Next recommended checkpoint

**FL-M2-01 — Authority Claim and Static Reset Core**

Expected future outcome: define only the authority-claim state, duplicate rejection before side effects, and safe static reset across Play sessions. That checkpoint requires its own approved plan and is not authorized by FL-M1-01.

---

## 19. Handoff record

| Field | Planned value before execution |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` skeleton |
| Specification | v1.3.0 |
| Checkpoint | FL-M1-01 — Package Skeleton |
| Implementation status | Authorized; not started |
| Tests | Defined, not run |
| Known blockers | None; exact uGUI version resolved during execution |
| Stop point | Before any C# script or runtime behavior |
| Next checkpoint | FL-M2-01, not yet authorized |

---

## 20. Approval

**Decision:** Active and authorized  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 3, 2026  
**Conditions:** SUITE-DOC-33 has activated this checkpoint. PKG-LEARN-001 must remain complete. Execute only the file manifest and tests in this plan. Verify the live Unity project, working tree, package path, and exact uGUI version before file creation. No launch behavior, scene setup, prefab, ScriptableObject, sample, setup tool, bridge, or C# file may enter FL-M1-01.
