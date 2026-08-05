# FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | FL-M5-03 |
| Version | 1.0.0 |
| Status | Approved |
| Package | First Light (`EchoLaunch`) |
| Package specification | SFGSS-PKG-ECHOLAUNCH-001 v1.9.0 |
| ADR | EchoLaunch-ADR-006 |
| Milestone | M5 — Tooling and Direct Scene |
| Repository | The-Sperk-Forge |
| Branch | `main` |
| Required baseline | `2ef594c` |
| Unity baseline | `6000.3.8f1` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Last updated | August 5, 2026 |
| Approved by | Jesse “Echo” Adams / EchoDevGames |

## 2. Purpose and Observable Outcome

The First Light Setup window can detect a narrow set of safe canonical drift in
existing current-schema project assets, preview every change, require explicit
Repair confirmation, back up every existing file before modification, reconcile
the approved surfaces, and settle to `NoChanges` on repeat.

Observable example:

```text
Before
- Configuration exists but its sequence/destination/splash references are wrong.
- Verified root prefab variant exists but is bound to the wrong configuration.
- Canonical Boot scene exists with unrelated project objects but no launch root.
- Boot Build Settings entry is missing or uniquely disabled.

After explicit Repair
- Only approved references/path/binding/root presence/Build Settings state changed.
- Existing GUIDs and stable IDs are unchanged.
- Unrelated asset values, prefab overrides, and scene objects are preserved.
- Second and third Repair return NoChanges.
```

## 3. Starting Conditions

- Branch: `main`
- HEAD: `2ef594c`
- Working tree: clean
- `main` equals `origin/main`
- Package version: `0.1.0`
- FL-M5-02 implemented, accepted, documented, and pushed
- EditMode baseline: `197` passed
- Runtime Play Mode baseline: `479` passed
- Compilation baseline: `0` errors and `0` warnings
- Manual FL-M5-02 Apply sequence: `Succeeded`, `NoChanges`, `NoChanges`
- No unresolved architecture blocker

## 4. Authority Set

1. SFGSS-000
2. SFGSS-PKG-ECHOLAUNCH-001 v1.9.0
3. EchoLaunch-ADR-004
4. EchoLaunch-ADR-005
5. EchoLaunch-ADR-006
6. SFGSS-005
7. Root and package Current Notes
8. FL-M5-01 and FL-M5-02 checkpoint/test/completion records
9. Existing Editor setup implementation and tests

## 5. Checkpoint Learning Review

FL-M5-03 does not reopen EchoLaunch’s package-level learning review. Startup
ownership, runtime architecture, and package independence do not change.

Before code, review this repair-specific model:

```text
Create-only Apply builds the approved house when rooms are missing.
Repair does not remodel the house. It verifies the deed, checks the exact room,
photographs the original, fixes one approved wire, and proves everything else
remained where it was.
```

Implementation must preserve four ideas:

1. **Proof before permission:** matching names are not ownership evidence.
2. **Separate buttons, separate authority:** Apply never silently repairs.
3. **Backup before touching history:** existing files receive byte/meta backup.
4. **Narrow repair, broad preservation:** change only the approved surface.

## 6. Constraints

- Runtime and presentation assemblies remain unchanged.
- Refresh/planning remains read-only.
- Create-only Apply remains create-only.
- Repair requires a fresh plan and explicit confirmation.
- Apply and Repair share one active-mutation gate.
- Only current supported schemas are repairable.
- Stable IDs and GUIDs remain unchanged.
- Ambiguous ownership/shape blocks before backup/writes.
- Package assets and destination scene remain unmodified.
- Existing files are backed up with matching `.meta` bytes before modification.
- Build Settings writes occur after asset/prefab/scene repair succeeds.
- Failure rollback restores existing bytes/settings and removes only same-attempt creations.
- Successful reruns are no-ops.

## 7. Scope

- Snapshot evidence for repairable serialized references and scene/prefab shape
- `Repair` plan disposition and repair eligibility
- Deterministic repair fingerprint
- Immutable repair approval/candidate/change/backup/result contracts
- Separate Repair service/action
- Shared single-active mutation gate with Apply
- Configuration-reference repair
- Destination-scene-path repair with authored-label preservation
- Verified root-prefab configuration binding repair
- Zero-root canonical Boot-scene repair
- Missing/uniquely-disabled Boot Build Settings repair
- Byte/meta backup store under `Library`
- Repair rollback and retained-backup result
- Copyable repair report
- Focused EditMode unit/integration tests
- Retained full EditMode and Runtime Play Mode gates
- Manual repair/repeatability acceptance

## 8. Explicit Exclusions

- Runtime production changes
- Schema migration/downgrade
- Stable-ID regeneration
- Type replacement
- Sequence-entry or splash-entry content repair
- Duplicate-root deletion/consolidation
- Arbitrary scene cleanup
- Prefab rebase/unpack/replacement/structural rewrite
- Move/rename/delete/relocation tools
- Destination-scene modification
- Persistent receipt/manifest
- Uninstall/reset
- Automatic crash recovery
- Direct-scene initializer
- Validator window
- Standalone Laboratory
- Package version change
- Player-build, clean-install, performance, or external-adoption claims

## 9. Authorized Repair Matrix

| Surface | Required proof | Permitted change | Must preserve | Block when |
|---|---|---|---|---|
| `EchoLaunchConfiguration` | Exact path/type, current schema, valid stable ID, unique canonical refs | Rebind sequence/destination/optional splash only | ID, schema, lifetime, reduced-motion/defaults, unrelated fields | Unsupported schema, invalid ID, ambiguous refs |
| `LaunchDestination` | Exact path/type, current schema, valid stable ID, selected existing scene | Reconcile scene path; fill label only if empty | ID, schema, non-empty authored label | Unsupported schema, invalid ID, missing/ambiguous scene |
| Root prefab | Variant lineage reaches package root template; exactly one root | Rebind root configuration only | Variant identity, presenter connection, unrelated overrides/structure | Wrong/unprovable lineage, root count != 1, structural issue |
| Boot scene | Exact planned path; safe scene load; zero roots for add case | Add one project-root-prefab instance | Unrelated objects, scene state, active/open/dirty state | Multiple roots, wrong/unpacked root, ambiguity |
| Build Settings | Canonical Boot path uniquely identifiable; policy approved | Add missing, enable unique disabled, or approved place-first | Unrelated order/enabled state | Duplicate/ambiguous entry without exact approved normalization |

## 10. Files and Assets

### 10.1 Modify

| Path | Change |
|---|---|
| `Editor/Setup/EchoLaunchProjectSnapshot.cs` | Repair evidence and immutable shape/reference facts |
| `Editor/Setup/EchoLaunchProjectSnapshotCollector.cs` | Collect supported serialized refs, prefab lineage/root count, Boot root evidence, enabled Build Settings identity |
| `Editor/Setup/EchoLaunchSetupEnums.cs` | `Repair` disposition/status/change kinds |
| `Editor/Setup/EchoLaunchSetupDiagnosticCodes.cs` | Add `013` through `017` |
| `Editor/Setup/EchoLaunchSetupPlanModels.cs` | Immutable repair candidates and fingerprint |
| `Editor/Setup/EchoLaunchSetupPlanner.cs` | Produce only authorized repair operations; block ambiguity/migration |
| `Editor/Setup/EchoLaunchSetupFingerprint.cs` | Include canonical repair facts and planned before/after values |
| `Editor/Setup/EchoLaunchSetupApplyModels.cs` | Keep create Apply rejecting Repair; shared mutation result seams where appropriate |
| `Editor/Setup/EchoLaunchSetupApplyService.cs` | Share active-mutation gate/fresh replan seam without weakening create-only execution |
| `Editor/Setup/EchoLaunchSetupApplyResultFormatter.cs` | Shared status wording where needed; no repair masquerading as create |
| `Editor/Setup/EchoLaunchSetupAssetWriter.cs` | Narrow current-schema configuration/destination field writes |
| `Editor/Setup/EchoLaunchSetupPrefabWriter.cs` | Verified variant binding repair |
| `Editor/Setup/EchoLaunchSetupSceneWriter.cs` | Canonical zero-root Boot repair with state preservation |
| `Editor/Setup/EchoLaunchSetupBuildSettingsWriter.cs` | Missing/unique-disabled/approved place-first repair |
| `Editor/Setup/EchoLaunchSetupRollbackJournal.cs` | Coordinate same-attempt creations with repair restore |
| `Editor/Setup/EchoLaunchSetupWindow.cs` | Separate Repair button, explanation, approval, confirmation, result/copy |
| `Tests/Editor/Setup/EchoLaunchProjectSnapshotCollectorTests.cs` | Repair evidence and preservation tests |
| `Tests/Editor/Setup/EchoLaunchSetupPlannerTests.cs` | Repair matrix, blockers, ambiguity, migration tests |
| `Tests/Editor/Setup/EchoLaunchSetupFingerprintTests.cs` | Repair fingerprint determinism/sensitivity |
| `Tests/Editor/Setup/EchoLaunchSetupApplyServiceTests.cs` | Create Apply continues to reject Repair; shared gate/freshness |
| `Tests/Editor/Setup/EchoLaunchSetupBuildSettingsWriterTests.cs` | Disabled/missing/ambiguous repair cases |
| `Tests/Editor/Setup/EchoLaunchSetupRollbackIntegrationTests.cs` | Existing-file restore and mixed create+repair rollback |
| `Tests/Editor/Setup/EchoLaunchSetupWindowTests.cs` | Separate action/confirmation/result behavior |

### 10.2 Create

| Path | Responsibility |
|---|---|
| `Editor/Setup/EchoLaunchSetupRepairModels.cs` | Immutable approval/candidate/change/backup/result contracts |
| `Editor/Setup/EchoLaunchSetupRepairBackupStore.cs` | Byte/meta backup, restore, cleanup, retained-path reporting |
| `Editor/Setup/EchoLaunchSetupRepairService.cs` | Eligibility, freshness, approval, ordered repair, rollback, result |
| `Editor/Setup/EchoLaunchSetupRepairResultFormatter.cs` | Deterministic copyable repair report |
| `Tests/Editor/Setup/EchoLaunchSetupRepairModelTests.cs` | Immutability/value/copy tests |
| `Tests/Editor/Setup/EchoLaunchSetupRepairBackupStoreTests.cs` | Backup/restore/cleanup/failure tests |
| `Tests/Editor/Setup/EchoLaunchSetupRepairServiceTests.cs` | Gates, ordering, approval, diagnostics, result tests |
| `Tests/Editor/Setup/EchoLaunchSetupRepairIntegrationTests.cs` | Real asset/prefab/scene/Build Settings repair/repeatability tests |
| `Tests/Editor/Setup/EchoLaunchSetupRepairResultFormatterTests.cs` | Stable text report tests |

Unity creates and commits matching `.meta` files for every new source/test file.

### 10.3 Test-created assets

Tests may create only beneath unique roots:

```text
Assets/__EchoLaunch_FL_M5_03_Tests_<unique>
Library/EchoDevGames/FirstLight/RepairBackups/<test-repair-id>
```

Every test must restore Build Settings, close temporary scenes, delete temporary
Assets content, and remove its backup directory when the test does not
intentionally verify retention.

No permanent generated project asset belongs in the implementation commit.

## 11. Implementation Sequence

### Phase 1 — Evidence model

1. Add immutable repair facts to the project snapshot.
2. Collect references through `SerializedObject` or typed current-schema access
   without mutating assets.
3. Collect prefab asset type, variant-source lineage, and root count.
4. Inspect Boot scene additively while preserving Editor scene state.
5. Record unique Build Settings identity/enabled state.
6. Prove collection is side-effect free and deterministic.

### Phase 2 — Planner and fingerprint

1. Add `Repair` disposition.
2. Produce repair candidates only for the approved matrix.
3. Generate sanitized before/after descriptions.
4. Mark every Repair as explicitly approved.
5. Block unsupported schema, wrong type, ambiguous refs, unsafe prefab/scene shape.
6. Extend deterministic fingerprints with repair facts and planned values.
7. Prove enumeration order cannot alter fingerprints.

### Phase 3 — Immutable repair contracts

1. Add repair approval/candidate/change/backup/result values.
2. Defensively copy all collections.
3. Exclude mutable Unity objects.
4. Carry final freshness fingerprints.
5. Carry rollback and retained-backup paths.

### Phase 4 — Backup store

1. Resolve exact asset and `.meta` filesystem paths.
2. Create unique backup root under `Library`.
3. Copy bytes before modification.
4. Hash/verify copied bytes.
5. Abort the transaction when any required backup fails.
6. Restore bytes and force import/refresh.
7. Delete backup on successful completion.
8. Retain and report backup on incomplete rollback.

### Phase 5 — Repair writers

1. Configuration writer changes only three approved refs.
2. Destination writer changes only scene path and optionally empty label.
3. Prefab writer proves lineage/root count and changes only config binding.
4. Scene writer adds one prefab root only to a zero-root canonical Boot scene.
5. Build Settings writer handles missing/unique-disabled/approved place-first.
6. Every writer returns exact change evidence and refuses broader edits.

### Phase 6 — Repair service

1. Reject no-repair plans and missing approval.
2. Share one active mutation gate with Apply.
3. Recollect/replan and compare all fingerprints.
4. Build backup list before writes.
5. Execute create/reuse/no-change prerequisites in approved order.
6. Back up and execute existing-asset repairs.
7. Write Build Settings last.
8. Refresh/replan and verify terminal `NoChanges`/expected state.
9. Return immutable result.

### Phase 7 — Rollback

1. Inject failures before and after every repair surface.
2. Restore modified bytes/meta/settings.
3. Remove only same-attempt created paths.
4. Verify GUIDs/IDs and unrelated values after rollback.
5. Report `ELAUNCH-SETUP-016` on complete rollback.
6. Inject restore failure and verify `ELAUNCH-SETUP-017` plus retained backup path.

### Phase 8 — Setup window

1. Keep Apply disabled when Repair exists.
2. Display repair summary and why each item is safe.
3. Add `Repair Plan...` only when executable.
4. Require final confirmation listing modified/created paths and backup policy.
5. Present/copy deterministic result.
6. Refresh after settlement.

### Phase 9 — Tests and acceptance

1. Run focused new EditMode tests.
2. Run complete EditMode suite.
3. Run complete Runtime Play Mode suite.
4. Perform manual current-schema drift repair.
5. Perform second and third Repair expecting `NoChanges`.
6. Verify Console/compiler counts.
7. Inspect Git scope and remove generated acceptance residue.

## 12. Test Matrix

| ID | Proof | Expected |
|---|---|---|
| FL-M5-03-T01 | Planning remains read-only | Snapshot/plan does not dirty assets/scenes/settings |
| FL-M5-03-T02 | Create/repair separation | Apply rejects Repair; Repair requires explicit approval |
| FL-M5-03-T03 | Freshness | Evidence drift after preview aborts before backup/write |
| FL-M5-03-T04 | Shared re-entry gate | Concurrent Apply/Repair invocation rejected |
| FL-M5-03-T05 | Config repair narrowness | Only approved refs change; ID/schema/policies preserved |
| FL-M5-03-T06 | Destination repair narrowness | Scene path reconciles; authored label/ID/schema preserved |
| FL-M5-03-T07 | Prefab repair narrowness | Only config binding changes; lineage/presenter/overrides survive |
| FL-M5-03-T08 | Boot repair narrowness | One prefab root added to zero-root scene; unrelated objects survive |
| FL-M5-03-T09 | Build Settings repair | Missing/unique-disabled fixed; unrelated order/state preserved |
| FL-M5-03-T10 | Unsafe shape rejection | Multiple roots/wrong lineage/wrong type/unsupported schema block |
| FL-M5-03-T11 | Backup precondition | Backup failure produces 014 and zero writes |
| FL-M5-03-T12 | Complete rollback | Exact bytes/meta/settings restored; 016 |
| FL-M5-03-T13 | Incomplete rollback | Backup retained/reported; 017 |
| FL-M5-03-T14 | Mixed partial foundation | Creates missing targets and repairs only approved existing targets |
| FL-M5-03-T15 | Repeatability | Repair 1 succeeds; Repair 2/3 NoChanges; stable GUIDs/IDs |
| FL-M5-03-T16 | Package/destination preservation | Package templates and destination scene unchanged/not dirty |
| FL-M5-03-T17 | Result immutability | Defensive copies, sanitized paths/values, deterministic text |
| FL-M5-03-T18 | Full regression | Complete EditMode and Runtime Play Mode suites green |

## 13. Manual Acceptance Scenario

Use a temporary generated foundation, not permanent repository content.

1. Start from a clean project state and generate the canonical foundation.
2. Record GUIDs/stable IDs and a hash/copy of unrelated values.
3. Introduce only approved drift:
   - Clear/change configuration references.
   - Set destination path stale while keeping current schema/identity.
   - Clear root-prefab configuration binding.
   - Remove the sole root instance from Boot while leaving an unrelated marker object.
   - Remove or uniquely disable the Boot Build Settings entry.
4. Refresh Setup.
5. Confirm `Apply Plan...` does not perform repair.
6. Inspect every Repair operation and before/after explanation.
7. Press `Repair Plan...` and confirm once.
8. Verify the repair succeeds and the marker/unrelated values remain.
9. Run Repair a second and third time.
10. Expect `NoChanges` both times.
11. Confirm GUIDs/stable IDs unchanged, no duplicate root/Build Settings entry,
    package template not dirty, destination scene untouched.
12. Record Console/compiler/test totals.
13. Remove generated acceptance assets and restore Build Settings before staging.

A separate failure-injection integration test proves backup/rollback. Do not
manually corrupt the real repository foundation to test incomplete rollback.

## 14. Expected Automated Baseline

Starting baseline:

```text
EditMode: 197 passed
PlayMode: 479 passed
Errors: 0
Warnings: 0
```

The implementation may increase the EditMode discovery count. The final report
must record the exact new count rather than guessing it in advance.

Runtime Play Mode discovery is expected to remain `479` because FL-M5-03 is
Editor-only. Any change must be explained and reviewed.

## 15. Stop Conditions

Stop immediately when:

- Runtime/presentation code appears necessary.
- Safe ownership/shape cannot be proven from current evidence.
- Repair would require schema migration or stable-ID regeneration.
- A writer must replace/delete/move/rename project content.
- Exact backup/meta restore cannot be guaranteed.
- Repair dirties package templates or the selected destination scene.
- Existing open/active/dirty scene state cannot be preserved.
- A test leaves Assets, Library backup, Build Settings, or scene residue.
- Compilation or retained regression gates fail.

Record the blocker in Current Notes before changing authority.

## 16. Documentation and Checkpoint Closeout

At closeout:

1. Reconcile root and package Current Notes.
2. Update package specification status/evidence without changing authority silently.
3. Update Architecture, README, documentation index, and Changelog.
4. Add FL-M5-03 package checkpoint record.
5. Add FL-M5-03 repair/reconciliation test report.
6. Add suite-level implementation completion record.
7. Record exact EditMode/PlayMode/compiler/manual totals.
8. Record implementation and documentation commits.
9. Confirm generated acceptance assets and temporary backups are absent.
10. Commit/push documentation adjacent to implementation.

## 17. Commit Path

Authority commit:

```text
echo-launch: approve FL-M5-03 explicit setup repair
```

Implementation commit, after green tests and acceptance:

```text
Implement explicit First Light setup repair
```

Documentation closeout commit:

```text
Close out FL-M5-03 setup repair checkpoint
```

## 18. Completion Criteria

FL-M5-03 is complete only when:

- [ ] Authority commit is pushed before implementation.
- [ ] Refresh/planning remains read-only.
- [ ] Apply remains create-only.
- [ ] Repair is a separate explicit action.
- [ ] Only the approved repair matrix is executable.
- [ ] Current-schema/type/identity/lineage/shape gates block ambiguity.
- [ ] Required backups are secured before existing-file writes.
- [ ] Complete rollback restores exact bytes/meta/settings.
- [ ] Incomplete rollback retains and reports backup paths.
- [ ] Stable IDs and GUIDs survive success and rollback.
- [ ] Second and third Repair return `NoChanges`.
- [ ] Package template and destination scene remain unmodified.
- [ ] Full EditMode and Runtime Play Mode suites pass.
- [ ] Compilation is 0 errors and 0 warnings.
- [ ] Manual acceptance is recorded.
- [ ] No generated acceptance or backup residue enters the implementation commit.
- [ ] Documentation is reconciled, committed, and pushed.
