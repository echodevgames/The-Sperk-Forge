# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-04`
- Title: Read-Only Validator and Project Health Report
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.10.0
- ADR: EchoLaunch-ADR-007
- Authority baseline: `638e676`
- Previous authority: `6615c8f`
- Previous implementation: `dd15768`
- Previous documentation: `638e676`
- Status: Authority prepared; implementation locked
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `236` passed
- Runtime Play Mode baseline: `479` passed
- Total automated baseline: `715` passed

## Approved Outcome

First Light gains a dedicated read-only Validator window that explicitly inspects
the installed canonical foundation and returns one immutable deterministic
project-health report.

The Validator reports. It does not create, repair, migrate, save, delete, move,
rename, or dirty project content.

## Validator Boundary

- Dedicated `Tools > Sperk's Forge > First Light > Validator`
- Explicit `Validate Project`
- Canonical default root with editable project-root field
- No auto-run on window open/repaint/import/reload/Play Mode
- Read-only asset, prefab, scene, and Build Settings inspection
- Open/active/dirty scene-state preservation
- Immutable schema-1 request/finding/report contracts
- Stable health and severity model
- Deterministic request/evidence/report fingerprints
- Project-relative copyable text report
- One active validation run
- Per-rule exception containment
- No Setup mutation invocation

## Rule Boundary

FL-M5-04 implements stable validation codes `ELAUNCH-VAL-001` through
`ELAUNCH-VAL-015`, except:

```text
ELAUNCH-VAL-009
```

is reserved for FL-M5-05 direct-scene release safety and is not emitted in this
checkpoint.

## Preserved Boundary

FL-M5-04 does not authorize:

- Runtime or presentation changes.
- Apply or Repair from the Validator.
- Auto-fix.
- Migration or stable-ID regeneration.
- Asset/prefab/scene/Build Settings writes.
- Duplicate-root deletion.
- Direct-scene initializer.
- Build hooks or automatic build blocking.
- Simulator or Laboratory.
- Runtime Observatory integration.
- JSON/support-bundle export.
- Receipt, uninstall/reset, move, rename, or delete tools.

## Next Action

Commit and push:

```text
Approve FL-M5-04 read-only validator authority
```

Then implement only the approved Editor validation surface and tests.
