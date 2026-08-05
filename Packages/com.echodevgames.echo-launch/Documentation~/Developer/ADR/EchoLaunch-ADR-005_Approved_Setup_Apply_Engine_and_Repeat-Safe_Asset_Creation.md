# EchoLaunch ADR-005 — Approved Setup Apply Engine and Repeat-Safe Asset Creation

## Metadata

- ADR: `EchoLaunch-ADR-005`
- Status: Approved
- Date: August 5, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.8.0
- Checkpoint: FL-M5-02
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `4c4d168`

## Context

FL-M5-01 separated observation and planning from mutation. The Setup window can
inspect project evidence and produce a deterministic immutable plan without
changing a project file or setting.

FL-M5-02 owns the smallest safe mutation layer capable of creating:

- Project-owned folders.
- Project-owned definition assets.
- One project-owned root prefab variant.
- One project-owned Boot scene.
- One explicitly approved Build Settings change.

The apply layer must remain non-destructive. It must not overwrite, repair,
migrate, move, rename, or delete existing project-owned content. It must be safe
to run again after success and safe to resume when a prior compatible creation
attempt left only some targets present.

## Decision

### Apply only an approved executable plan

The apply service accepts the immutable request, displayed immutable plan, and
explicit approval for every operation marked as requiring approval.

It may execute only when:

- The plan is `Ready`, or is `ReadyWithWarnings` with all approvals present.
- No unresolved conflict, unsupported operation, or ambiguous decision remains.
- No other apply operation is active.

The Setup window exposes one explicit action:

```text
Apply Plan...
```

It remains disabled while the plan is blocked, stale, incomplete, or missing
required approval.

### Freshness gate before every write

Immediately before mutation, the service must:

1. Recollect the project snapshot.
2. Rebuild the plan from the same request.
3. Compare the rebuilt plan and deterministic evidence fingerprint with the
   displayed plan.
4. Abort with no writes when evidence changed.

Stable diagnostic:

```text
ELAUNCH-SETUP-008
```

A stale plan is never applied silently.

### Create-only and reuse-only policy

FL-M5-02 may execute only:

```text
Create
Reuse
NoChange
```

`Reuse` and `NoChange` perform no project write.

These remain non-executable:

```text
ManualDecision
Conflict
Unsupported
```

No operation may overwrite, move, rename, delete, migrate, replace, or repair
an existing project asset.

### Deterministic creation order

1. Project-owned folder chain.
2. `StartupSequence`.
3. `LaunchDestination`.
4. Optional `SplashSequence`.
5. `EchoLaunchConfiguration`.
6. Project-owned `EchoLaunchRoot.prefab` variant.
7. Project-owned `Boot.unity`.
8. Build Settings mutation.

Every definition is fully configured in memory before its asset file is
created.

The configuration references the resolved sequence, destination, and optional
splash.

The destination stores the selected existing destination-scene path and a plain
display name derived from that scene asset.

### Project-owned root prefab variant

Package template:

```text
Packages/com.echodevgames.echo-launch/Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

The apply service:

- Instantiates the package template temporarily.
- Assigns the resolved project-owned configuration.
- Saves a project-owned prefab variant at the planned target.
- Preserves the nested package status-view prefab connection.
- Destroys the temporary instance.
- Never edits or dirties the package template.

The resulting asset must be a valid prefab variant containing one
`EchoLaunchRoot`.

### Boot scene creation

The apply service creates the planned Boot scene only when the path is missing.

The scene contains:

- One instance of the resolved project-owned root prefab.
- No project-specific gameplay object.
- No setup-created EventSystem.
- No destination-scene content.

Creation must:

- Avoid opening the selected destination scene.
- Preserve the existing open-scene set.
- Preserve the active scene.
- Preserve dirty states of pre-existing scenes.
- Close the temporary Boot scene after saving.

### Build Settings mutation

Approved policies:

```text
DoNotChange
AddIfMissingAtEnd
PlaceFirstAfterApproval
```

Rules:

- `DoNotChange` performs no write.
- `AddIfMissingAtEnd` appends one enabled Boot entry only when missing.
- `PlaceFirstAfterApproval` requires explicit approval.
- Promotion removes duplicate Boot entries before inserting one enabled entry
  at index zero.
- Every unrelated entry keeps its relative order and enabled state.
- Build Settings mutation occurs last.

`ELAUNCH-SETUP-004` continues to represent missing explicit place-first
approval.

### Single apply authority

Only one apply operation may be active.

A second invocation is rejected before writes with:

```text
ELAUNCH-SETUP-009
```

Apply is synchronous on the Unity Editor main thread. It does not run mutation
in the background or survive an Editor/domain restart.

### Failure rollback journal

Before mutation, the apply service records:

- Every folder created by this attempt.
- Every asset, prefab, and scene created by this attempt.
- The complete original Build Settings scene array.
- Pre-apply scene state needed for verification.

On failure, it:

1. Restores Build Settings when changed.
2. Closes any temporary scene.
3. Deletes only paths created by this attempt, in reverse order.
4. Removes only newly created empty folders, deepest first.
5. Saves and refreshes the AssetDatabase.
6. Reports whether rollback completed.

Diagnostics:

```text
ELAUNCH-SETUP-010
```

Apply failed and rollback completed.

```text
ELAUNCH-SETUP-011
```

Rollback was incomplete and named paths require manual recovery.

The journal never deletes or rewrites a pre-existing asset.

### Repeat-safe success

After a successful apply:

- Refresh produces only `Reuse` and `NoChange` for the foundation.
- Second and third Apply return `NoChanges`.
- No duplicate definition, prefab, scene, or Build Settings entry appears.
- Existing GUIDs remain unchanged.
- Existing assets remain not dirty.

Compatible partial creation can be resumed by refreshing and applying only
still-missing targets.

### Immutable apply result

Result includes:

- Apply status.
- Stable diagnostic and plain message.
- Created paths.
- Reused paths.
- Build Settings before/after summaries.
- Rollback result.
- Manual-recovery paths.
- Final refreshed plan status.

Statuses:

```text
Succeeded
NoChanges
Cancelled
Blocked
StalePlan
AlreadyRunning
FailedRolledBack
FailedRollbackIncomplete
```

The Setup window may display and copy the result.

### Confirmation boundary

`Apply Plan...` opens a final confirmation listing:

- Every path to create.
- The Build Settings action.
- Whether place-first approval is active.
- A statement that existing project assets will not be overwritten.

Cancelling produces `Cancelled` and performs no write.

### Stable apply diagnostics

FL-M5-02 adds:

- `ELAUNCH-SETUP-008` displayed plan is stale.
- `ELAUNCH-SETUP-009` another apply operation is active.
- `ELAUNCH-SETUP-010` apply failed and rollback completed.
- `ELAUNCH-SETUP-011` rollback incomplete; manual recovery required.
- `ELAUNCH-SETUP-012` plan contains an operation outside this apply authority.

Existing `ELAUNCH-SETUP-001` through `007` retain their meanings.

## Rejected Alternatives

### Apply without recollection

Rejected. Project state may change after preview.

### Modify compatible existing assets

Rejected. That is repair, not create-only setup.

### Delete conflicts automatically

Rejected. Project ownership outranks package convenience.

### Copy an ordinary unpacked prefab

Rejected. A variant preserves the stable package-template relationship.

### Replace the user’s open-scene setup

Rejected. Setup must not save, close, or reopen unrelated scenes.

### Rely only on Unity Undo

Rejected. File and Build Settings changes need explicit compensation.

### Persist a receipt or manifest now

Rejected for FL-M5-02. Receipts, uninstall, and crash recovery require later
authority.

### Repair incompatible partial content

Rejected. Repair requires a separate checkpoint.

## Consequences

### Positive

- First Light can create its canonical foundation.
- Re-running setup becomes a provable no-op.
- Existing content is never overwritten.
- Build Settings order is protected.
- Package templates remain immutable.
- Failures can compensate for active-attempt changes.
- A stale preview cannot mutate a changed project.

### Costs

- Apply cannot repair invalid existing references.
- Successful setup has no persistent receipt/uninstall manifest.
- A hard process crash may require replan or manual recovery.
- Prefab/scene/Build Settings integration tests use temporary project assets.

## Implementation Boundary

FL-M5-02 may add or modify:

- Apply request/result/status contracts.
- Deterministic plan/evidence fingerprinting.
- Eligibility and freshness validation.
- Create-only folder and asset writers.
- Project-owned prefab-variant writer.
- Boot scene writer with scene-state preservation.
- Build Settings writer.
- In-memory rollback journal.
- Apply controls, confirmation, result display, and result copying.
- Focused EditMode integration tests under a unique temporary root.

FL-M5-02 may not add:

- Runtime changes.
- Existing-asset repair.
- Migration.
- Move/rename/delete tools.
- Persistent setup receipt or manifest.
- Uninstall tooling.
- Direct-scene initializer.
- Validator window beyond apply diagnostics.
- Project branding/input configuration.
- Standalone Test Lab.
- Player-build claims.
- Package version change.

## Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
