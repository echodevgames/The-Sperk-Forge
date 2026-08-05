# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-02`
- Title: Approved Setup Apply Engine and Repeat-Safe Asset Creation
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.8.0
- ADR: EchoLaunch-ADR-005
- Authority baseline: `4c4d168`
- Previous authority: `b6a4f27`
- Previous implementation: `453bc14`
- Previous documentation: `4c4d168`
- Status: Authority prepared; implementation locked
- EditMode baseline: 93 passed
- Runtime Play Mode baseline: 479 passed
- Compilation baseline: 0 errors, 0 warnings

## Approved Outcome

The Setup window may apply one fresh executable plan and create the missing
canonical First Light foundation.

The operation is create-only, reuse-only, and non-destructive.

## Apply Boundary

- Freshness recollection before writes
- Deterministic fingerprints
- Single active apply
- Create/Reuse/NoChange only
- Folder and definition creation
- Configuration binding
- Project root prefab variant
- Boot scene creation
- Explicit Build Settings policy
- In-memory rollback
- Immutable result
- No-op second and third Apply

## Existing Content Policy

Compatible content is reused and not modified.

Incompatible content blocks.

Ambiguous candidates require selection.

Unsupported schemas require migration later.

## Scene and Build Settings Policy

- Destination scene is never opened or modified.
- Existing open, active, and dirty scene states are preserved.
- Build Settings write last.
- Default appends one enabled Boot entry.
- Place-first requires explicit approval.
- Unrelated order/enabled states are preserved.

## Deferred

Repair, migration, persistent receipt, uninstall/reset, crash-persistent
recovery, Direct Scene, Validator, Laboratory, builds, clean/external adoption,
and performance evidence.

## Next Action

Commit and push:

```text
echo-launch: approve FL-M5-02 repeat-safe setup apply
```
