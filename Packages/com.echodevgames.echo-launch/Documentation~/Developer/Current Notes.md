# First Light - Current Notes

## Completed Checkpoint

- Checkpoint: `FL-M5-04`
- Title: Read-Only Validator and Project Health Report
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.10.0
- ADR: EchoLaunch-ADR-007
- Authority commit: `c2397c9`
- Implementation commit: `26732ea`
- Documentation closeout: pending this adjacent commit
- Status: Implemented, automated-tested, manually accepted, and pushed
- Compilation: `0` errors, `0` warnings
- Focused Validator EditMode: `25` passed
- Complete EditMode: `261` passed
- Runtime Play Mode: `479` passed
- Total automated: `740` passed

## Implemented Outcome

First Light now includes a dedicated explicit read-only Validator window that
inspects the canonical installed foundation and returns one immutable
deterministic schema-1 project-health report.

The Validator does not create, repair, migrate, save, delete, move, rename, or
dirty project content.

## Accepted Evidence

- Window creation does not run validation.
- Explicit `Validate Project` returns immutable report values.
- Health derives from the highest finding severity.
- Stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015` are owned by the Validator.
- `ELAUNCH-VAL-009` remains reserved for FL-M5-05 and was not emitted.
- Canonical assets, root prefab, Boot scene, enabled build scenes, destination,
  splash, sequence, and Build Settings are inspected read-only.
- Re-entry returns `ELAUNCH-VAL-015` without starting a second scan.
- Inspection failures become sanitized `ELAUNCH-VAL-014`.
- The healthy foundation produced no findings.
- Two unchanged healthy runs produced identical fingerprints and copied text.
- Deliberate root-binding, duplicate-root, and Build Settings drift produced
  `Blocked`.
- Findings included `ELAUNCH-VAL-002`, two path-specific `ELAUNCH-VAL-003`
  findings, and `ELAUNCH-VAL-008`.
- Explicit restoration returned the exact original healthy fingerprints.
- Generated acceptance assets and Build Settings drift were removed before the
  implementation commit.
- Runtime and presentation code remained unchanged.

## Stable Healthy Fingerprints

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
a847886c1303998c51e47cba2f697dc102cb9574dad5302de72a19333a055803

Report:
287af851bf779eff65bc4791d9d33048851871e53a164edae5e3819d30f6f74c
```

## Preserved Boundary

FL-M5-04 does not authorize:

- Auto-fix or Validator-triggered Apply/Repair.
- Migration or stable-ID regeneration.
- Asset, prefab, scene, or Build Settings mutation.
- Duplicate-root deletion.
- Direct-scene initializer.
- Build hooks or automatic build blocking.
- Simulator or Laboratory.
- Runtime Observatory integration.
- JSON/support-bundle export.
- Receipt, uninstall/reset, move, rename, or delete tools.

## Implementation Corrections

- Replaced one public parameterized NUnit test using internal enum parameters
  with a public parameterless test that proves all four health mappings.
- Corrected trailing spaces in generated Validator `.meta` files before staging.
- Replaced the original Python-dependent delivery helper with a CMD-only helper;
  no failed delivery attempt changed the committed repository baseline.

## Handoff

- Active checkpoint: None
- Next candidate: `FL-M5-05` Direct Scene Development Initializer
- FL-M5-05 status: Candidate only; not authorized
- Next action: Commit and push this FL-M5-04 documentation closeout, then perform
  a just-in-time learning and authority review before the next implementation
