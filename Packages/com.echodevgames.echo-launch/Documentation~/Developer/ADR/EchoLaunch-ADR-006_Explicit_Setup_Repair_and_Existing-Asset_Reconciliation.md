# EchoLaunch ADR-006 — Explicit Setup Repair and Existing-Asset Reconciliation

## Metadata

- ADR: `EchoLaunch-ADR-006`
- Status: Approved
- Date: August 5, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.9.0
- Checkpoint: FL-M5-03
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `2ef594c`

## Context

FL-M5-01 established read-only project evidence and deterministic planning.
FL-M5-02 established fresh-plan-gated create-only mutation. Existing compatible
assets are reused, incompatible assets block, and successful reruns are no-ops.

A real project can later contain a partial or damaged First Light foundation:
configuration references may be cleared, a verified root prefab variant may
lose its configuration binding, the canonical Boot scene may exist without a
root, or the canonical Build Settings entry may be missing/disabled.

Create-only Apply must not silently expand into editing existing content. The
repair workflow therefore needs separate authority, stronger identity/shape
proof, an explicit confirmation surface, and recovery that protects files that
already existed before the operation.

## Decision

### Repair is a separate explicit transaction

The Setup window exposes:

```text
Repair Plan...
```

It is distinct from:

```text
Apply Plan...
```

Create Apply may execute only `Create`, `Reuse`, and `NoChange`. Repair may
execute approved `Repair` operations plus the create/reuse/no-change operations
needed to complete one partial foundation.

Refresh, planning, package import, inspector drawing, Play Mode entry, and
create-only Apply never perform repair.

### Freshness and single Setup mutation authority remain mandatory

Immediately before backup or writes, Repair must:

1. Recollect the project snapshot.
2. Rebuild the plan from the same immutable request.
3. Recompute request/evidence/plan/repair fingerprints.
4. Compare the fresh result with the displayed approved repair plan.
5. Abort before backup or mutation when evidence changed.

`ELAUNCH-SETUP-008` remains the stale-plan diagnostic.

Apply and Repair share one single-active mutation gate. Re-entry is rejected
with `ELAUNCH-SETUP-009` before writes.

### Every repair requires proven ownership and shape

A repair candidate is executable only when the package can prove the facts
required for that surface:

- Exact planned project-relative path.
- Expected Unity type.
- Supported current schema where versioned.
- Unique role resolution.
- Stable package-template lineage for the project root prefab.
- Safe exact root-count and prefab-instance shape for Boot-scene work.

Matching only a filename, label, or folder is insufficient. Ambiguity blocks
repair with `ELAUNCH-SETUP-015`; it never authorizes a guess.

### Authorized configuration repair

For an existing valid current-schema `EchoLaunchConfiguration`, Repair may
reconcile only these references:

- `StartupSequence`
- `LaunchDestination`
- Optional `SplashSequence`

The targets must be uniquely resolved by the fresh plan. The service preserves
stable ID, schema, root-lifetime policy, reduced-motion default, and every
unrelated serialized value.

### Authorized destination repair

For an existing valid current-schema `LaunchDestination`, Repair may reconcile
only the runtime scene path to the explicitly selected existing destination
scene.

The display label is filled only when empty. A non-empty project-authored label
is preserved. Stable identity and schema are never regenerated or changed.

The selected destination scene itself is never opened or modified.

### Authorized project-root prefab repair

The existing project root prefab must:

- Be a prefab variant.
- Resolve through variant lineage to the stable package root template.
- Contain exactly one `EchoLaunchRoot`.

Repair may rebind only that root’s configuration reference. It preserves all
other overrides, nested presenter connection, and prefab structure. It never
replaces, rebases, unpacks, or modifies the package template.

### Authorized Boot-scene repair

The Boot scene must exist at the exact planned project path.

When it contains zero `EchoLaunchRoot` components, Repair may add one instance
of the uniquely resolved project root prefab. It preserves every unrelated
scene object and the user’s open-scene set, active scene, and dirty states.

These conditions block repair:

- More than one root.
- An unpacked or wrong-lineage root.
- An ambiguous hierarchy.
- A root shape that would require deletion or replacement.

FL-M5-03 does not delete or consolidate roots.

### Authorized Build Settings repair

Repair follows the selected policy:

- `DoNotChange`: no write.
- `AddIfMissingAtEnd`: add one enabled canonical Boot entry when missing; enable
  one uniquely identified disabled canonical entry in place.
- `PlaceFirstAfterApproval`: only after explicit placement approval.

Unrelated entries retain relative order and enabled state. Duplicate or
ambiguous canonical entries block unless an already approved place-first
operation defines the exact normalization.

Build Settings is written after project-asset repair succeeds.

### Byte-preserving backup before modifying existing content

Before the first modification of an existing asset, prefab, or scene, Repair
copies the exact asset and matching `.meta` bytes to:

```text
Library/EchoDevGames/FirstLight/RepairBackups/<repair-id>/
```

The backup is outside `Assets` and is not imported as project content.

If any required backup cannot be secured, Repair aborts before writes with:

```text
ELAUNCH-SETUP-014
```

The complete ordered Build Settings scene array is captured independently.

### Rollback

On failure, Repair:

1. Restores modified asset and `.meta` bytes.
2. Restores Build Settings when changed.
3. Removes only paths created by the same transaction using the approved
   active-attempt journal.
4. Refreshes/reimports restored assets.
5. Restores temporary scene state.
6. Reports whether rollback completed.

Diagnostics:

```text
ELAUNCH-SETUP-016
```

Repair failed and rollback completed.

```text
ELAUNCH-SETUP-017
```

Repair rollback was incomplete. The backup is retained and its path is reported
for manual recovery.

A successful repair removes its temporary backup directory. Automatic recovery
after an Editor/process crash remains deferred.

### Immutable result and repeatability

The result records status, final fingerprints, created/reused/repaired/unchanged
paths, sanitized per-repair before/after summaries, Build Settings before/after,
rollback state, and retained backup/manual-recovery paths.

After successful reconciliation:

- Second and third Repair return `NoChanges`.
- Asset GUIDs and stable IDs do not change.
- No duplicate Boot root or Build Settings entry is created.
- Package templates remain not dirty.

## Explicitly rejected for FL-M5-03

- Schema migration or downgrade.
- Stable-ID regeneration.
- Type replacement.
- Startup- or splash-sequence content editing.
- Duplicate-root deletion or arbitrary scene cleanup.
- Prefab replacement, rebase, unpack, or structural rewrite.
- Move, rename, delete, or relocation operations.
- Destination-scene modification.
- Persistent setup receipts.
- Uninstall/reset.
- Automatic crash recovery.
- Runtime/Play Mode auto-repair.
- Direct Scene, Validator, or Laboratory implementation.

## Consequences

### Positive

- Create-only Apply remains honest and non-destructive.
- Common canonical drift can be recovered without rebuilding the foundation.
- Existing files gain explicit backup and rollback protection.
- Repair behavior is previewable, testable, and repeat-safe.
- Ambiguous or project-authored structures remain under human control.

### Costs

- The planner/snapshot must collect deeper serialized, prefab-lineage, and
  Boot-scene evidence.
- Repair requires more integration tests than creation because pre-existing
  bytes and GUIDs must survive failure.
- Some seemingly simple problems remain blocked because safe ownership cannot
  be proven.

## Validation obligations

FL-M5-03 must prove:

- Create Apply cannot execute Repair operations.
- Repair requires explicit per-plan approval and fresh fingerprints.
- Configuration, destination, prefab, scene, and Build Settings repairs touch
  only the approved surface.
- Unsupported schema and ambiguous shape block before backup/writes.
- Backup failure blocks before writes.
- Injected failure restores exact asset/meta bytes and Build Settings.
- Incomplete rollback retains and reports backup paths.
- Second and third Repair return `NoChanges` with stable GUIDs/IDs.
- Package templates and selected destination scene remain unmodified.

## Supersession

This ADR extends EchoLaunch-ADR-004 and EchoLaunch-ADR-005. It does not weaken
the read-only planner, create-only Apply, freshness gate, package-independence,
or migration boundaries.
