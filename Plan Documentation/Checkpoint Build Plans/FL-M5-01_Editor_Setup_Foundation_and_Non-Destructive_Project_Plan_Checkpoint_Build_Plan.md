
# FL-M5-01 — Editor Setup Foundation and Non-Destructive Project Plan

**Document ID:** FL-M5-01
**Version:** 1.0.0
**Status:** Approved; implementation locked until authority commit
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
**ADR:** EchoLaunch-ADR-004
**Milestone:** M5 — Tooling and Direct Scene
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting commit:** `8bd2a57`
**Starting EditMode evidence:** 27 passed, 0 failed, 0 ignored
**Starting Runtime Play Mode evidence:** 479 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> This checkpoint draws the blueprint on glass. It does not swing the hammer.

---

## 1. Purpose

Create the Editor-only observation and planning layer that can explain exactly
what First Light setup would do before any project asset, scene, or Build
Settings entry can change.

This is a recognizable product span:

```text
Tools > Sperk's Forge > First Light > Setup
```

opens a working preview window, inspects the project, and produces a
deterministic non-destructive setup plan.

---

## 2. Observable outcome

When complete:

1. The package Editor assembly remains isolated and `autoReferenced: false`.
2. The Setup menu opens one preview-only window.
3. The window proposes project-owned default paths.
4. The user can select a destination scene and optional splash creation.
5. The project snapshot collector performs read-only inspection.
6. The planner produces immutable ordered operations.
7. Repeated planning with the same evidence produces equivalent results.
8. Existing compatible assets are reused.
9. Wrong asset types block with a stable diagnostic.
10. Missing assets produce proposed creates, not writes.
11. Ambiguous candidates require manual selection.
12. Default Build Settings planning appends Boot without reordering.
13. Promotion to index zero is explicitly marked for approval.
14. Existing unrelated scene order is preserved in the preview.
15. Unsupported configuration schema blocks migration.
16. Missing package templates block setup.
17. The window can copy a plain-text plan report.
18. The window contains no Apply/Repair/Migrate action.
19. Opening, refreshing, copying, and closing the window create no asset,
    scene, import-setting, EditorPrefs, or Build Settings change.
20. Existing 27 EditMode prefab tests and 479 Runtime Play Mode tests remain
    green.

---

## 3. Authority

- SFGSS-000
- SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
- EchoLaunch-ADR-004
- SFGSS-005
- FL-M4-05 completion at `8bd2a57`

---

## 4. Approved architecture

```text
EchoLaunchSetupWindow
    -> EchoLaunchSetupRequest
    -> EchoLaunchProjectSnapshotCollector
    -> EchoLaunchProjectSnapshot
    -> EchoLaunchSetupPlanner
    -> EchoLaunchSetupPlan
    -> EchoLaunchSetupPlanTextFormatter
```

Only the collector touches Unity Editor read APIs.

The planner and formatter are deterministic plain C# services.

No type in this checkpoint calls Unity write APIs.

---

## 5. Approved Editor contracts

### 5.1 Build Settings policy

```csharp
internal enum EchoLaunchBuildSettingsPolicy
{
    DoNotChange = 0,
    AddIfMissingAtEnd = 1,
    PlaceFirstAfterApproval = 2
}
```

### 5.2 Plan status

```csharp
internal enum EchoLaunchSetupPlanStatus
{
    Ready = 0,
    ReadyWithWarnings = 1,
    Blocked = 2
}
```

### 5.3 Operation disposition

```csharp
internal enum EchoLaunchSetupOperationDisposition
{
    Create = 0,
    Reuse = 1,
    NoChange = 2,
    ManualDecision = 3,
    Conflict = 4,
    Unsupported = 5
}
```

### 5.4 Operation kinds

At minimum:

```text
ValidateRequest
ValidatePackageTemplate
EnsureFolder
ResolveConfiguration
ResolveStartupSequence
ResolveLaunchDestination
ResolveSplashSequence
ResolveRootPrefabVariant
ResolveBootScene
ResolveBuildSettings
```

### 5.5 Request

The immutable request contains:

- Project root path.
- Boot scene path.
- Destination scene path.
- Optional splash creation.
- Build Settings policy.
- Explicit selected existing asset paths when needed.

### 5.6 Snapshot

The immutable snapshot contains:

- Normalized asset facts by path.
- Main asset type names.
- GUIDs.
- Package template availability.
- Existing Build Settings scene entries in order.
- Destination scene availability.
- Existing configuration schema when readable.
- Compatible candidate lists for setup roles.

### 5.7 Plan

The immutable plan contains:

- Request copy.
- Snapshot identity/evidence summary.
- Status.
- Ordered operation copies.
- Ordered diagnostic copies.
- Counts by disposition.
- `HasBlockers`.
- `RequiresExplicitApproval`.
- Plain-language summary.

Public/runtime API is not changed. These types remain internal to the Editor
assembly for FL-M5-01.

---

## 6. Default paths

Project root:

```text
Assets/EchoDevGames/FirstLight
```

Targets:

```text
Assets/EchoDevGames/FirstLight/Configuration/EchoLaunchConfiguration.asset
Assets/EchoDevGames/FirstLight/Configuration/StartupSequence.asset
Assets/EchoDevGames/FirstLight/Configuration/LaunchDestination.asset
Assets/EchoDevGames/FirstLight/Configuration/SplashSequence.asset
Assets/EchoDevGames/FirstLight/Prefabs/EchoLaunchRoot.prefab
Assets/EchoDevGames/FirstLight/Scenes/Boot.unity
```

Package template:

```text
Packages/com.echodevgames.echo-launch/Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

The destination scene must be an existing project scene.

---

## 7. Stable diagnostics

| Code | Condition | Plan result |
|---|---|---|
| `ELAUNCH-SETUP-001` | Invalid/unsafe project path or missing destination selection | Blocked |
| `ELAUNCH-SETUP-002` | Incompatible asset already occupies a target path | Blocked |
| `ELAUNCH-SETUP-003` | Existing configuration requires unsupported migration | Blocked |
| `ELAUNCH-SETUP-004` | Build Settings reorder requested | Warning + explicit approval |
| `ELAUNCH-SETUP-005` | Multiple compatible candidates without selection | Manual decision |
| `ELAUNCH-SETUP-006` | Required package template/script unavailable | Blocked |
| `ELAUNCH-SETUP-007` | Compatible existing project asset will be reused | Informational |

Diagnostics are stable constants with plain-language messages.

---

## 8. Path policy

A valid generated project path:

- Starts with `Assets/`.
- Is not exactly `Assets`.
- Contains no `..` traversal segment.
- Does not target `Packages/`, `Library/`, `ProjectSettings/`, `Temp/`, or an
  absolute filesystem path.
- Uses forward slashes after normalization.
- Has the correct extension for the target role.
- Is compared with ordinal path semantics appropriate to Unity asset paths.

The planner never normalizes an invalid external path into a valid project
path silently.

---

## 9. Planning rules

### 9.1 Existing compatible target

Disposition:

```text
Reuse
```

or `NoChange` when the requested reference already matches.

The plan must not contain overwrite language.

### 9.2 Missing target

Disposition:

```text
Create
```

This is a proposal only.

### 9.3 Wrong asset type

Disposition:

```text
Conflict
```

Diagnostic:

```text
ELAUNCH-SETUP-002
```

### 9.4 Ambiguous candidate

Disposition:

```text
ManualDecision
```

Diagnostic:

```text
ELAUNCH-SETUP-005
```

### 9.5 Unsupported schema

Disposition:

```text
Unsupported
```

Diagnostic:

```text
ELAUNCH-SETUP-003
```

### 9.6 Build Settings

Default policy:

```text
AddIfMissingAtEnd
```

Rules:

- Existing Boot entry: `NoChange`.
- Missing Boot entry: proposed append.
- Existing unrelated order copied exactly into preview.
- `PlaceFirstAfterApproval`: warning and approval flag.
- No policy mutates EditorBuildSettings in this checkpoint.

---

## 10. Preview-only Setup window

Menu:

```text
Tools/Sperk's Forge/First Light/Setup
```

Visible sections:

1. Project paths.
2. Destination selection.
3. Optional splash selection.
4. Build Settings policy.
5. Snapshot status.
6. Plan summary.
7. Operation list.
8. Diagnostics.
9. Copy Plan button.
10. Refresh Plan button.

Not present:

- Apply
- Repair
- Migrate
- Create
- Delete
- Move
- Save Scene
- Change Build Settings

The window displays a clear message:

```text
Preview only. This checkpoint changes nothing in the project.
```

---

## 11. Files

### 11.1 Editor production files

```text
Editor/Setup/
├── EchoLaunchBuildSettingsPolicy.cs
├── EchoLaunchProjectAssetFact.cs
├── EchoLaunchProjectSnapshot.cs
├── EchoLaunchProjectSnapshotCollector.cs
├── EchoLaunchSetupDiagnostic.cs
├── EchoLaunchSetupDiagnosticCodes.cs
├── EchoLaunchSetupOperation.cs
├── EchoLaunchSetupOperationDisposition.cs
├── EchoLaunchSetupOperationKind.cs
├── EchoLaunchSetupPathSet.cs
├── EchoLaunchSetupPathUtility.cs
├── EchoLaunchSetupPlan.cs
├── EchoLaunchSetupPlanner.cs
├── EchoLaunchSetupPlanStatus.cs
├── EchoLaunchSetupPlanTextFormatter.cs
├── EchoLaunchSetupRequest.cs
└── EchoLaunchSetupWindow.cs
```

The exact split may consolidate tiny value types when clarity improves, but
responsibility and contracts must remain visible.

### 11.2 Editor tests

```text
Tests/Editor/
├── EchoDevGames.EchoLaunch.Tests.Editor.asmdef
└── Setup/
    ├── EchoLaunchSetupPathUtilityTests.cs
    ├── EchoLaunchSetupPlannerTests.cs
    ├── EchoLaunchProjectSnapshotCollectorTests.cs
    ├── EchoLaunchSetupPlanTextFormatterTests.cs
    └── EchoLaunchSetupWindowTests.cs
```

No Runtime production file is expected to change.

---

## 12. Implementation sequence

### Phase 1 — Value contracts

1. Add enums and immutable value objects.
2. Use defensive copies for lists.
3. Add equality/value-comparison helpers needed by deterministic tests.
4. Keep Unity objects out of the pure plan model.

### Phase 2 — Path policy

1. Normalize separators.
2. Validate project-root and role-specific extensions.
3. Build the default path set.
4. Prove invalid paths without touching AssetDatabase.

### Phase 3 — Pure planner

1. Validate request.
2. Validate package prerequisites from snapshot facts.
3. Resolve folders and definition assets.
4. Resolve root prefab variant and Boot scene.
5. Resolve Build Settings proposal.
6. Aggregate diagnostics and status.
7. Sort operations deterministically.

### Phase 4 — Read-only collector

1. Read asset existence/type/GUID.
2. Read package template availability.
3. Read Build Settings entries in order.
4. Read configuration schema only from compatible assets.
5. Avoid opening scenes.
6. Prove no dirty assets/settings.

### Phase 5 — Preview window

1. Add menu item.
2. Build request controls.
3. Refresh snapshot/plan explicitly.
4. Render operations and diagnostics.
5. Add Copy Plan.
6. Display preview-only warning.
7. Add no apply path.

### Phase 6 — Tests

1. Run focused Editor tests.
2. Run all EditMode tests.
3. Run retained Runtime Play Mode suite.
4. Verify 0 compile errors and warnings.
5. Review Git scope for Editor/test files only.

---

## 13. Minimum focused test matrix

At least 40 focused Editor tests:

### Path tests

1. Default root/path generation.
2. Backslash normalization.
3. Reject absolute path.
4. Reject Packages path.
5. Reject ProjectSettings path.
6. Reject traversal.
7. Reject wrong extension.
8. Accept nested Assets path.

### Planner tests

9. Empty project produces ordered create proposals.
10. Optional splash omitted by default.
11. Optional splash included when requested.
12. Existing compatible config reuses.
13. Existing compatible sequence reuses.
14. Existing compatible destination reuses.
15. Existing compatible root variant reuses.
16. Existing Boot scene reuses.
17. Wrong type at config path blocks.
18. Wrong type at Boot path blocks.
19. Unsupported schema blocks.
20. Missing package root template blocks.
21. Ambiguous config requires manual decision.
22. Default Build Settings appends.
23. Existing Boot entry produces no change.
24. Promotion policy requires approval.
25. Relative order of unrelated scenes preserved.
26. DoNotChange produces no Build Settings create.
27. Same inputs produce equivalent plan.
28. Operations use stable deterministic order.
29. Blocker produces Blocked status.
30. Warning produces ReadyWithWarnings.
31. Clean plan produces Ready.
32. Plan collections are defensive.

### Collector tests

33. Reads asset fact without modification.
34. Reads GUID.
35. Reads Build Settings order.
36. Detects package template.
37. Does not open scenes.
38. Does not dirty assets.
39. Does not change Build Settings.
40. Handles missing destination.

### Formatter/window tests

41. Text report includes status and operations.
42. Text report is deterministic.
43. Menu item/window exists.
44. Preview-only warning is visible.
45. Refresh creates no assets.
46. Window exposes no Apply button/action.

Final discovered count is evidence.

---

## 14. Validation commands and gates

Compilation gate:

```text
Errors: 0
Warnings: 0
```

Focused Editor gate:

```text
Failed: 0
Ignored: 0
```

Retained gates:

```text
Existing EditMode prefab tests: 27 passed
Runtime Play Mode tests: 479 passed
```

Git scope gate:

- Editor setup source.
- Editor test source/asmdefs/metas.
- Active Checkpoint Build Plan already committed in authority update.
- No `Assets/` project asset.
- No scene.
- No prefab.
- No ProjectSettings change.
- No Runtime production source change unless a blocking authority issue is
  raised first.

---

## 15. Failure symptoms

| Symptom | Likely cause | Response |
|---|---|---|
| Opening Setup creates folders | Write API leaked into window/collector | Stop and remove write path |
| Build Settings changes after refresh | Collector uses mutation API | Restore settings and fix before tests |
| Planner result changes between runs | Unordered collection or time-dependent value | Sort facts/operations and remove clock dependence |
| Existing asset marked Create | Type/GUID fact not captured | Fix snapshot resolution |
| Existing wrong asset silently reused | Type validation missing | Emit ELAUNCH-SETUP-002 |
| Window cannot load because Editor asmdef is not referenced | Test/reference configuration error | Fix explicit asmdef references, not auto-reference policy |
| Runtime assembly gains UnityEditor reference | File placed in wrong assembly | Move to Editor assembly and add assembly proof |
| Scene opens during refresh | Collector exceeded approved inspection scope | Remove scene-content inspection |

---

## 16. Explicit exclusions

FL-M5-01 does not authorize:

- Applying the plan.
- Creating folders, assets, variants, scenes, or Build Settings entries.
- Undo/backup implementation.
- Setup receipts or manifests.
- Configuration schema migration.
- Scene-content/root inspection.
- Direct-scene initializer.
- Validator window beyond setup-plan diagnostics.
- Simulator or report viewer.
- Standalone Test Lab.
- Runtime code changes.
- Project branding/input setup.
- Player builds.
- Package version change.

---

## 17. Rollback

Before commit, the checkpoint is removed by restoring/cleaning only:

```text
Packages/com.echodevgames.echo-launch/Editor/Setup
Packages/com.echodevgames.echo-launch/Tests/Editor
```

No project asset or setting rollback should be necessary because the
implementation is forbidden from writing them.

After a pushed commit, use `git revert`.

---

## 18. Commit plan

Authority:

```text
echo-launch: approve FL-M5-01 non-destructive setup planning
```

Implementation:

```text
echo-launch: complete FL-M5-01 setup planning foundation
```

Documentation:

```text
echo-launch: document FL-M5-01 completion
```

---

## 19. Stop point

Stop after the preview-only window, read-only snapshot, immutable deterministic
plan, stable diagnostics, focused Editor proof, and retained existing tests.

Do not implement Apply, Repair, Migration, Direct Scene, Boot scene generation,
or Build Settings mutation.

---

## 20. Tentative next checkpoint

**FL-M5-02 — Approved Setup Apply Engine and Repeat-Safe Asset Creation**

Tentative only. It requires a separate authority decision covering Undo,
backups, project-owned prefab variants, scene creation, Build Settings changes,
receipts, interruption, and recovery.

---

## 21. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Condition:** Commit specification v1.7.0 and ADR-004 before adding Editor
setup implementation.
