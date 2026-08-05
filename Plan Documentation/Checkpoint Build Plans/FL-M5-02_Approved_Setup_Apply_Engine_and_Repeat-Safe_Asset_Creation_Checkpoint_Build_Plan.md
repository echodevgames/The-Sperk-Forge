# FL-M5-02 — Approved Setup Apply Engine and Repeat-Safe Asset Creation

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | FL-M5-02 |
| Version | 1.0.0 |
| Status | Approved |
| Package | First Light (`EchoLaunch`) |
| Package specification | SFGSS-PKG-ECHOLAUNCH-001 v1.8.0 |
| ADR | EchoLaunch-ADR-005 |
| Milestone | M5 — Tooling and Direct Scene |
| Repository | The-Sperk-Forge |
| Branch | `main` |
| Required baseline | `4c4d168` |
| Unity baseline | `6000.3.8f1` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Last updated | August 5, 2026 |
| Approved by | Jesse “Echo” Adams / EchoDevGames |

## 2. Purpose and Observable Outcome

The Setup window can apply one fresh executable plan and create the missing
canonical project-owned First Light foundation without overwriting existing
project content.

Successful result:

```text
Assets/EchoDevGames/FirstLight/
├── Configuration/
│   ├── EchoLaunchConfiguration.asset
│   ├── StartupSequence.asset
│   ├── LaunchDestination.asset
│   └── SplashSequence.asset      optional
├── Prefabs/
│   └── EchoLaunchRoot.prefab     prefab variant
└── Scenes/
    └── Boot.unity
```

Second and third Apply produce no new files, no duplicate Build Settings entry,
no GUID change, and `NoChanges`.

## 3. Starting Conditions

- Branch: `main`
- HEAD: `4c4d168`
- Working tree: clean
- `main` equals `origin/main`
- Package version: `0.1.0`
- FL-M5-01 complete and documented
- Setup planning window available
- EditMode baseline: `93` passed
- Runtime Play Mode baseline: `479` passed
- Compilation: `0` errors and `0` warnings
- Stable package root template present
- No compile/import/play transition active
- No unresolved architecture blocker

## 4. Authority Set

1. SFGSS-000
2. SFGSS-PKG-ECHOLAUNCH-001 v1.8.0
3. EchoLaunch-ADR-004
4. EchoLaunch-ADR-005
5. SFGSS-005
6. Root and package Current Notes
7. FL-M5-01 checkpoint and test report
8. Existing Editor setup implementation/tests
9. FL-M4-05 prefab assets/tests

## 5. Constraints

- Runtime remains unchanged.
- Apply consumes the approved plan and does not invent intent.
- Existing assets are never overwritten, moved, renamed, deleted, repaired, or
  migrated.
- Package assets are never modified.
- Stale plans abort before writes.
- Build Settings changes follow the selected policy.
- Place-first requires explicit approval.
- Build Settings mutation occurs last.
- Apply is synchronous and single-authority.
- Failure rollback deletes only active-attempt content.
- Existing open scenes, active scene, and dirty states remain unchanged.
- Destination scene is never opened or modified.
- Successful reruns are no-ops.

## 6. Scope

- Deterministic request/snapshot/plan fingerprints
- Immutable apply request/result/change contracts
- Eligibility and stale-plan gates
- Single-active-apply gate
- Folder creation
- Startup sequence creation
- Destination creation and scene-path binding
- Optional splash-sequence creation
- Configuration creation and reference binding
- Project root prefab variant creation
- Boot scene creation with one root instance
- Build Settings append or approved promotion
- In-memory rollback journal
- Deterministic failure-injection seam
- Setup-window Apply, approval, confirmation, result, and Copy Result
- Focused EditMode temporary-asset integration tests
- Retained full EditMode and Runtime Play Mode runs

## 7. Explicit Exclusions

- Runtime production changes
- Existing-reference repair
- Existing-asset modification
- Migration
- Move/rename/delete tools
- Persistent setup receipt or manifest
- Uninstall/reset
- Crash-persistent recovery
- Direct-scene initializer
- Validator window
- Scene-content repair
- Adding startup steps
- Branding/input configuration
- Standalone Test Lab
- Package version change
- Player-build, clean-install, or external-adoption claims

## 8. Files and Assets

### 8.1 Modify

| Path | Change |
|---|---|
| `Editor/Setup/EchoLaunchProjectSnapshot.cs` | Deterministic relevant-evidence fingerprint |
| `Editor/Setup/EchoLaunchSetupDiagnosticCodes.cs` | Add `008` through `012` |
| `Editor/Setup/EchoLaunchSetupEnums.cs` | Apply statuses and change kinds |
| `Editor/Setup/EchoLaunchSetupPlanModels.cs` | Carry request/evidence/plan fingerprints |
| `Editor/Setup/EchoLaunchSetupPlanTextFormatter.cs` | Approval/apply information |
| `Editor/Setup/EchoLaunchSetupPlanner.cs` | Produce stable executable fingerprints |
| `Editor/Setup/EchoLaunchSetupWindow.cs` | Approval, Apply, confirmation, result, Copy Result |
| `Tests/Editor/Setup/EchoLaunchProjectSnapshotCollectorTests.cs` | Fingerprint/preservation proof |
| `Tests/Editor/Setup/EchoLaunchSetupPlanTextFormatterTests.cs` | Apply/fingerprint report proof |
| `Tests/Editor/Setup/EchoLaunchSetupPlannerTests.cs` | Eligibility/fingerprint proof |
| `Tests/Editor/Setup/EchoLaunchSetupWindowTests.cs` | Apply controls and disabled-state proof |

### 8.2 Create

| Path | Responsibility |
|---|---|
| `Editor/Setup/EchoLaunchSetupApplyModels.cs` | Immutable apply request/status/result/change values |
| `Editor/Setup/EchoLaunchSetupFingerprint.cs` | Deterministic fingerprint generation |
| `Editor/Setup/EchoLaunchSetupApplyService.cs` | Freshness, single-active gate, ordered execution |
| `Editor/Setup/EchoLaunchSetupAssetWriter.cs` | Folder and ScriptableObject create-only operations |
| `Editor/Setup/EchoLaunchSetupPrefabWriter.cs` | Project-owned root prefab variant |
| `Editor/Setup/EchoLaunchSetupSceneWriter.cs` | Boot scene and Editor-state preservation |
| `Editor/Setup/EchoLaunchSetupBuildSettingsWriter.cs` | Append/promotion with order preservation |
| `Editor/Setup/EchoLaunchSetupRollbackJournal.cs` | Compensating active-attempt ledger |
| `Editor/Setup/EchoLaunchSetupApplyResultFormatter.cs` | Deterministic copyable result |
| `Tests/Editor/Setup/EchoLaunchSetupApplyModelTests.cs` | Contract immutability/value tests |
| `Tests/Editor/Setup/EchoLaunchSetupFingerprintTests.cs` | Fingerprint proof |
| `Tests/Editor/Setup/EchoLaunchSetupApplyServiceTests.cs` | Eligibility/stale/re-entry/rollback orchestration |
| `Tests/Editor/Setup/EchoLaunchSetupApplyIntegrationTests.cs` | Real temporary asset/prefab/scene/Build Settings tests |
| `Tests/Editor/Setup/EchoLaunchSetupApplyResultFormatterTests.cs` | Result formatting |
| `Tests/Editor/Setup/EchoLaunchSetupFailureInjector.cs` | Test-only failure seam |

Unity creates matching `.meta` files.

### 8.3 Test-created assets

Tests may create only beneath a unique root:

```text
Assets/__EchoLaunch_FL_M5_02_Tests_<unique>
```

`TearDown` must remove it and restore Build Settings.

No permanent project asset belongs in the implementation commit.

## 9. Implementation Sequence

### Phase 1 — Apply contracts

1. Add status/change vocabulary.
2. Add immutable request/result/change values.
3. Defensively copy collections.
4. Keep mutable Unity objects out of result values.
5. Add value comparison needed by tests.

### Phase 2 — Fingerprints

1. Canonicalize request fields.
2. Canonicalize asset facts by ordinal path.
3. Canonicalize Build Settings by index.
4. Include package-template GUID.
5. Produce stable lowercase fingerprints.
6. Store them in the plan.
7. Prove enumeration order cannot change them.

### Phase 3 — Eligibility and freshness

1. Reject blocked plans.
2. Reject unresolved manual decisions.
3. Require place-first approval.
4. Reject re-entry.
5. Recollect/replan before writes.
6. Abort stale plans with `ELAUNCH-SETUP-008`.
7. Reject unauthorized operation kinds with `ELAUNCH-SETUP-012`.

### Phase 4 — Asset writer

1. Create missing folder chain.
2. Create a fully initialized empty startup sequence.
3. Create destination with selected scene path/display name.
4. Create optional empty valid splash sequence.
5. Create configuration with resolved references.
6. Save and validate each created asset.
7. Record each created path immediately.
8. Never mutate reused assets.

### Phase 5 — Prefab writer

1. Load package root template.
2. Instantiate temporarily.
3. Assign project configuration.
4. Save at project path.
5. Verify prefab type `Variant`.
6. Verify one root.
7. Verify nested presenter connection.
8. Destroy temporary object.
9. Record path.
10. Verify package template not dirty.

### Phase 6 — Scene writer

1. Capture existing scene setup and active scene.
2. Create additive empty temporary scene.
3. Instantiate project root prefab.
4. Save Boot scene.
5. Close only temporary Boot scene.
6. Restore active scene.
7. Verify pre-existing scene setup/dirty states.
8. Record scene path.
9. Never open destination scene.

### Phase 7 — Build Settings writer

1. Capture original scene array.
2. `DoNotChange`: no write.
3. Default append: one enabled Boot entry when missing.
4. Approved place-first: one Boot entry at index zero.
5. Preserve unrelated order and enabled states.
6. Record before/after.
7. Write last.

### Phase 8 — Rollback

1. Add failure injection between phases.
2. Restore Build Settings.
3. Close temporary scenes.
4. Delete created files in reverse order.
5. Remove new empty folders deepest first.
6. Refresh and verify.
7. Return complete/incomplete rollback status.
8. Name manual-recovery paths.

### Phase 9 — Setup window

1. Add place-first approval control.
2. Add `Apply Plan...`.
3. Disable when ineligible.
4. Show final confirmation.
5. Execute service.
6. Refresh after settlement.
7. Display result.
8. Add Copy Result.
9. Keep Repair and Migrate absent.

### Phase 10 — Validation

1. Compile.
2. Run focused contract/service tests.
3. Run focused temporary-asset integration tests.
4. Run all EditMode tests.
5. Run all Runtime Play Mode tests.
6. Verify Console counts.
7. Restore generated solution noise.
8. Inspect Git scope.
9. Stage only package Editor/test code and metadata.

## 10. Visible Code and Learning Rule

Show every new or modified C# file completely with:

- Exact path and create/modify state.
- Responsibility and Editor-assembly boundary.
- Inputs/outputs.
- Freshness and no-overwrite behavior.
- Failure/rollback path.
- Test seam.
- Reason for the file split.

Generated bundles supplement the visible explanation; they do not authorize
hidden edits.

## 11. Unity Editor Setup

1. Open `Tools > Sperk's Forge > First Light > Setup`.
2. Select a real existing destination scene.
3. Keep the default root unless testing a temporary root.
4. Choose splash behavior.
5. Choose Build Settings policy.
6. Refresh and review every operation.
7. Enable explicit place-first approval only when intended.
8. Press `Apply Plan...`.
9. Review confirmation.
10. Apply.
11. Verify generated assets.
12. Verify the prior active/open scenes remain unchanged.
13. Verify Build Settings.
14. Refresh and Apply twice more.
15. Confirm both return `NoChanges`.

Acceptance does not use manually created foundation assets.

## 12. Validation and Tests

Minimum focused count: `50` Editor tests. Final discovered count is evidence.

### Contract and fingerprint

1. Defensive apply-result collections.
2. Stable status vocabulary.
3. Equivalent request fingerprints.
4. Equivalent snapshot fingerprints despite input order.
5. Asset GUID change alters fingerprint.
6. Build Settings order change alters fingerprint.
7. Enabled-state change alters fingerprint.
8. Template GUID change alters fingerprint.
9. Destination path change alters fingerprint.
10. Plan fingerprint deterministic.

### Eligibility and service

11. Ready plan executable.
12. ReadyWithWarnings needs approval.
13. Blocked rejected.
14. Ambiguous rejected.
15. Unsupported rejected.
16. Place-first without approval rejected.
17. Stale plan invokes no writer.
18. Re-entry rejected.
19. Confirmation cancellation writes nothing.
20. No-create plan returns `NoChanges`.
21. Create phases ordered.
22. Build Settings writer last.
23. Failure triggers rollback.
24. Complete rollback status.
25. Incomplete rollback paths.

### Integration

26. Empty target creates folders.
27. Startup sequence valid.
28. Destination points to selected scene.
29. Splash omitted when unrequested.
30. Splash created when requested.
31. Configuration references resolved assets.
32. Root is prefab variant.
33. Root variant bound to configuration.
34. Nested presenter preserved.
35. Package template not dirty.
36. Boot has exactly one root.
37. Boot has no EventSystem.
38. Destination scene not opened.
39. Open-scene set preserved.
40. Active scene preserved.
41. Dirty scene state preserved.
42. Default Build Settings append.
43. Unrelated Build Settings order preserved.
44. Unrelated enabled states preserved.
45. Place-first requires approval.
46. Place-first preserves unrelated order.
47. Second Apply `NoChanges`.
48. Third Apply `NoChanges`.
49. No duplicate Build Settings entry.
50. GUIDs stable across reruns.
51. Existing compatible assets reused/not dirty.
52. Wrong-type conflict writes nothing.
53. Stale integration case writes nothing.
54. Injected prefab failure removes prior creations.
55. Injected scene failure restores state.
56. Teardown restores Build Settings and removes temp root.

### Retained gates

- EditMode baseline: `93`.
- Runtime Play Mode baseline: `479`.
- Compilation: `0` errors, `0` warnings.
- No permanent `Assets/` content after tests.

## 13. Failure Symptoms and Fixes

| Symptom | Cause | Response |
|---|---|---|
| Apply enabled on blocked plan | Eligibility split across UI | Centralize eligibility |
| Apply writes after project change | Freshness gate missing | Recollect/replan before writer |
| Existing asset modified | Writer touched reused asset | Restrict writer to new instances |
| Root not a variant | Prefab connection lost | Preserve template instance connection |
| Destination scene opens | Scene writer exceeded scope | Remove destination-scene opening |
| User scene saved/cleaned | Scene restoration mutated it | Close only temporary scene |
| Build order changes | Unrelated entries rebuilt incorrectly | Copy exact entries |
| Duplicate Boot entry | Missing deduplication | Resolve Boot before write |
| Rerun changes GUID | Existing target still planned Create | Fix snapshot/type resolution |
| Failure leaves files | Journal missed a path | Record after each successful create |
| Rollback deletes old asset | Pre-existing path recorded | Record only active-attempt creates |
| Package prefab dirty | Template edited directly | Edit temporary variant instance only |
| Tests leave temp root | Teardown not failure-safe | Centralize cleanup |

## 14. Rollback and Recovery

Before commit, restore only checkpoint-owned Editor/test files.

Tests must delete their temporary root and restore Build Settings even after
failure.

During apply failure, use ADR-005 compensation and report complete/incomplete
rollback.

After successful apply, generated assets are project-owned. Uninstall is not in
scope.

After a pushed package-source commit, use `git revert`; do not delete
project-owned generated content merely because source is reverted.

## 15. Documentation Reconciliation

Update:

- Package specification status
- ADR index
- Architecture
- Package checkpoint/test report
- Package and root Current Notes
- README
- Changelog
- Documentation index
- Root completion record

Record focused/full test totals, repeatability, Build Settings preservation,
rollback, compiler counts, commit hashes, and unrun evidence.

## 16. Commit Plan

Authority:

```text
echo-launch: approve FL-M5-02 repeat-safe setup apply
```

Implementation:

```text
echo-launch: complete FL-M5-02 repeat-safe setup apply
```

Documentation:

```text
echo-launch: document FL-M5-02 completion
```

No permanent test-created `Assets/` content or ProjectSettings drift may enter
the implementation commit.

## 17. Completion Criteria

- [ ] v1.8.0 and ADR-005 committed before implementation
- [ ] Fresh-plan gate before writes
- [ ] Single-active apply
- [ ] Required foundation created
- [ ] Existing compatible assets reused without modification
- [ ] Conflicts perform no writes
- [ ] Root is valid bound prefab variant
- [ ] Boot contains one root
- [ ] Open/active/dirty scene state preserved
- [ ] Default append exact
- [ ] Place-first approval required
- [ ] Unrelated Build Settings preserved
- [ ] Failure rollback proven
- [ ] Second and third Apply are `NoChanges`
- [ ] No duplicates
- [ ] Package template not dirty
- [ ] No Runtime production change
- [ ] Focused tests pass
- [ ] Full EditMode passes
- [ ] All 479 Runtime Play Mode tests pass
- [ ] Compilation has 0 errors/warnings
- [ ] Git has no permanent generated project assets
- [ ] Documentation reconciled and pushed

## 18. Stop Point

Stop after create-only apply, exact Build Settings policy, compensating
rollback, repeat-safe no-op reruns, focused Editor proof, and retained tests.

Do not implement Repair, Migration, receipts, uninstall, Direct Scene,
Validator, or Laboratory work.

## 19. Next Recommended Checkpoint

**FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation**

Tentative only.

## 20. Handoff Record

- Checkpoint: FL-M5-02
- Authority baseline: `4c4d168`
- Specification: v1.8.0
- ADR: EchoLaunch-ADR-005
- Runtime changes expected: none
- Permanent project assets in implementation commit: none
- Focused test target: at least 50
- Retained EditMode baseline: 93
- Retained Runtime Play Mode baseline: 479

## 21. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Condition:** Commit and push v1.8.0, ADR-005, and this plan before apply code.
