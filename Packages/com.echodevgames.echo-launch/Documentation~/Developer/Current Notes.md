# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-05`
- Title: Direct Scene Development Initializer
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.11.0
- ADR: EchoLaunch-ADR-008
- Authority baseline: `4e3bf34`
- Previous authority: `c2397c9`
- Previous implementation: `26732ea`
- Previous documentation: `4e3bf34`
- Status: Authority prepared; implementation locked
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `261` passed
- Runtime Play Mode baseline: `479` passed
- Total automated baseline: `740` passed

## Approved Outcome

A developer may open an explicitly configured gameplay or Test Lab scene and press Play. The helper reuses an existing authority or creates exactly one project-owned root that runs the normal First Light pipeline in `DirectSceneDevelopment` mode.

## Boundary

- Start-time settlement
- Existing-authority reuse
- Project-owned immutable direct configuration
- Pre-authored direct root prefab
- Matching containing-scene destination
- Active-destination no reload
- Editor-only default
- Development-Build opt-in
- Unconditional non-development release prohibition
- Report schema version `2`
- Activated read-only `ELAUNCH-VAL-009`

No second runner, hidden discovery, runtime asset mutation, auto-installation, build hook, Simulator, Laboratory, migration, or distribution work is authorized.

## Next Action

Commit and push:

```text
Approve FL-M5-05 direct scene initializer authority
```
