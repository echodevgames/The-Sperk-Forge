# FL-M5-04 Validator and Project Health Test Report

## Metadata

- Checkpoint: `FL-M5-04`
- Package: First Light (`EchoLaunch`)
- Unity: `6000.3.8f1`
- Authority commit: `c2397c9`
- Implementation commit: `26732ea`
- Test date: August 6, 2026
- Final result: Passed

## Compilation Gate

```text
Errors:   0
Warnings: 0
```

The first compilation identified one test-only accessibility problem:

```text
CS0051
```

A public parameterized NUnit test accepted internal
`EchoLaunchValidationSeverity` and `EchoLaunchProjectHealth` parameters.

The correction retained internal Editor visibility and changed the test to one
public parameterless method that verifies all four mappings.

A temporary Visual Studio integration warning for UDP port `56670` was unrelated
to the package. Closing competing tooling and reopening Unity restored the final
zero-warning Console gate.

## Focused Validator EditMode Gate

```text
Passed:   25
Failed:    0
Ignored:   0
Errors:    0
Warnings:  0
```

Covered classes:

- `EchoLaunchValidationContractTests`
- `EchoLaunchValidationFingerprintTests`
- `EchoLaunchValidationRuleTests`
- `EchoLaunchValidationServiceTests`
- `EchoLaunchValidationTextFormatterTests`
- `EchoLaunchValidationIntegrationTests`
- `EchoLaunchValidatorWindowTests`

## Complete EditMode Gate

```text
Passed:   261
Failed:     0
Ignored:    0
Errors:     0
Warnings:   0
```

Breakdown:

- Existing setup, apply, and repair tests: `209`
- New Validator tests: `25`
- Retained prefab asset tests: `27`

## Runtime PlayMode Regression Gate

```text
Passed:   479
Failed:     0
Ignored:    0
Errors:     0
Warnings:   0
```

No Runtime or presentation implementation changed in FL-M5-04.

## Deterministic Healthy Validation

First report:

```text
Health: Healthy
Information: 0
Warnings: 0
Errors: 0
Blockers: 0
Findings: None
```

Fingerprints:

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
a847886c1303998c51e47cba2f697dc102cb9574dad5302de72a19333a055803

Report:
287af851bf779eff65bc4791d9d33048851871e53a164edae5e3819d30f6f74c
```

A second validation with unchanged evidence reproduced the complete report and
all fingerprints exactly.

## Deliberate Blocked Validation

Authored temporary faults:

- Canonical root-prefab configuration cleared.
- One extra `EchoLaunchRoot` added to Boot.
- Boot removed from Build Settings.

Result:

```text
Health: Blocked
Information: 0
Warnings: 0
Errors: 0
Blockers: 4
```

Blocked fingerprints:

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
c268d1a455829d8ef3d076c6cf991a8b959ab5861193e631b7f5a56c2764b24e

Report:
5887b21595b331cea3e475d0c266bab76a4619ae9da8fe13590899075585df77
```

Findings:

1. `ELAUNCH-VAL-002` effective root count `2`.
2. `ELAUNCH-VAL-003` canonical root-prefab configuration missing.
3. `ELAUNCH-VAL-003` Boot root count `2`.
4. `ELAUNCH-VAL-008` Boot Build Settings total/enabled count `0`.

The unchanged request fingerprint and changed evidence/report fingerprints prove
that project evidence, rather than UI state, controls validation settlement.

## Restored Healthy Validation

After the extra root was removed and the canonical prefab/Build Settings state
was restored, the Validator returned:

```text
Health: Healthy
Information: 0
Warnings: 0
Errors: 0
Blockers: 0
Findings: None
```

The request, evidence, and report fingerprints exactly matched the original
healthy baseline.

## Mutation and Cleanup Evidence

Validation itself produced no authored project mutation.

Before staging:

- Generated `Assets/EchoDevGames` acceptance content removed.
- `ProjectSettings/EditorBuildSettings.asset` restored.
- `The Sperk Forge.slnx` restored.
- No acceptance backup residue retained.
- Only `Editor/Validation`, `Tests/Editor/Validation`, and matching metadata
  remained.
- Staged whitespace validation passed after metadata cleanup.

## Final Assessment

FL-M5-04 passed its compile, focused EditMode, complete EditMode, Runtime
PlayMode, deterministic healthy, truthful blocked, exact restoration, and Git
scope gates.
