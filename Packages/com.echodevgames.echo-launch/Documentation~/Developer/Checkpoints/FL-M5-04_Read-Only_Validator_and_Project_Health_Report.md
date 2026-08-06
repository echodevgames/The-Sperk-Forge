# FL-M5-04 — Read-Only Validator and Project Health Report

## Record Status

- Checkpoint: `FL-M5-04`
- Status: Completed
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.10.0
- ADR: EchoLaunch-ADR-007
- Authority commit: `c2397c9`
- Implementation commit: `26732ea`
- Unity baseline: `6000.3.8f1`
- Completion date: August 6, 2026

## Delivered Outcome

First Light now includes a dedicated Editor-only Validator at:

```text
Tools > Sperk's Forge > First Light > Validator
```

Validation is explicit and read-only. Opening or repainting the window does not
run validation, and the Validator exposes no Apply, Repair, migration, or
auto-fix command.

The accepted implementation includes:

- Immutable validation request, evidence, finding, and schema-1 report values.
- Stable validation severity and health vocabulary.
- Deterministic request, evidence, and report fingerprints.
- Deterministic project-relative copied text.
- Read-only canonical asset, prefab, scene, and Build Settings evidence.
- Enabled-build-scene duplicate-root inspection.
- Scene-safe additive inspection and active-scene restoration.
- Single-active validation-run protection.
- Sanitized evidence failure containment.
- Stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`.
- Reserved, non-emitted `ELAUNCH-VAL-009` for later Direct Scene authority.

## Stable Health Model

```text
Blocker -> Blocked
Error   -> Invalid
Warning -> NeedsAttention
Info    -> Healthy
```

The report uses the highest accepted severity. Information findings do not
reduce health.

## Implemented Validation Boundary

The Validator evaluates:

- Canonical Boot-scene existence and scene validity.
- Effective launch-root count across Boot and enabled build scenes.
- Canonical project root-prefab lineage, root count, configuration binding, and
  visual presentation capability.
- Configuration type, identity, schema, and canonical references.
- Startup sequence identity, entries, definitions, duplicate IDs, and policy.
- Destination identity, scene path, and unique enabled Build Settings entry.
- Canonical Boot unique enabled Build Settings entry.
- Optional splash identity, entry references, timing, and policy.
- Project-owned configuration references that resolve inside package source.
- Evidence collection and concurrent-run failure conditions.

The Validator does not:

- Create or delete project content.
- Save scenes or assets.
- Modify Build Settings.
- Invoke Apply or Repair.
- Migrate schemas.
- Regenerate IDs.
- Delete duplicate roots.
- Implement Direct Scene release safety.
- Hook build preprocessors.
- Export JSON or support bundles.

## Automated Evidence

```text
Compilation:       0 errors, 0 warnings
Focused Validator: 25 EditMode passed
Complete EditMode: 261 passed
Runtime PlayMode:  479 passed
Total automated:   740 passed
Failed:            0
Ignored:           0
```

## Manual Acceptance

The first canonical validation returned:

```text
Health: Healthy
Information: 0
Warnings: 0
Errors: 0
Blockers: 0
```

Stable fingerprints:

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
a847886c1303998c51e47cba2f697dc102cb9574dad5302de72a19333a055803

Report:
287af851bf779eff65bc4791d9d33048851871e53a164edae5e3819d30f6f74c
```

A second unchanged validation produced the exact same report.

The acceptance then deliberately:

1. Cleared the canonical project root-prefab configuration.
2. Added one extra `EchoLaunchRoot` to Boot.
3. Removed Boot from Build Settings.

Validation returned:

```text
Health: Blocked
Blockers: 4
```

Findings:

- `ELAUNCH-VAL-002` multiple effective launch roots.
- `ELAUNCH-VAL-003` invalid canonical root-prefab binding.
- `ELAUNCH-VAL-003` ambiguous Boot-scene root count.
- `ELAUNCH-VAL-008` missing Boot Build Settings entry.

After explicit restoration, validation returned the exact original healthy
request, evidence, and report fingerprints.

## Corrections During Acceptance

- Corrected a C# accessibility mismatch in one public NUnit test method without
  widening internal Editor enum visibility.
- Cleared one unrelated temporary Visual Studio UDP-port warning before the
  final Console gate.
- Corrected trailing spaces in generated Validator `.meta` files before
  staging.
- Replaced a Python-dependent local delivery helper with a CMD-only helper.
  Failed delivery attempts did not change the committed repository baseline.

## Preservation Proof

- Runtime code unchanged.
- Presentation code unchanged.
- Setup Apply and Repair implementation unchanged.
- Generated acceptance assets removed.
- Editor Build Settings restored.
- Solution-file drift restored.
- Working tree clean after commit `26732ea`.
- `main` synchronized with `origin/main`.

## Deferred Work

FL-M5-04 does not authorize:

- Direct Scene initialization.
- Direct Scene release-safety implementation under reserved `ELAUNCH-VAL-009`.
- Build hooks or automatic build blocking.
- Simulator or Laboratory.
- Migration, receipt, uninstall, reset, or crash-persistent recovery.
- Clean-project distribution or external adoption claims.

## Stop Point

FL-M5-04 stops after this documentation closeout. The next candidate is
FL-M5-05 Direct Scene Development Initializer, but it requires a separate
just-in-time learning review, authority decision, and committed Checkpoint Build
Plan before implementation.
