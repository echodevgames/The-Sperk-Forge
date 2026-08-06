# FL-M5-06 Launch Simulator and Deterministic Failure Injection Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-06`
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Unity: `6000.3.8f1`
- Date: August 6, 2026
- Result: Passed

## Compilation

```text
Errors:   0
Warnings: 0
```

Compilation passed after initial implementation and after the cancellation
determinism correction.

## Automated Tests

### Focused Simulator EditMode

```text
Passed:   24
Failed:    0
Ignored:   0
Errors:    0
Warnings:  0
```

### Complete EditMode

```text
Passed:   290
Failed:     0
Ignored:    0
Errors:     0
Warnings:   0
```

### Complete Runtime PlayMode

```text
Passed:   503
Failed:     0
Ignored:    0
Errors:     0
Warnings:   0
```

### Combined

```text
Total automated: 793
Failed:            0
Ignored:           0
```

## Manual Scenario Matrix

| Scenario | Authored | Attempted | Unvisited | Accepted outcome |
|---|---:|---:|---:|---|
| Immediate Success | 1 | 1 | 0 | Succeeded |
| Timed Progress Success | 1 | 1 | 0 | Succeeded with four ordered samples |
| Warning Continues | 2 | 2 | 0 | Warning then Succeeded |
| Recoverable Failure Continues | 2 | 2 | 0 | Policy-converted Warning then Succeeded |
| Blocking Failure Stops | 2 | 1 | 1 | `ELAUNCH-SIM-STEP-003` BlockingFailure |
| Timeout Stops | 2 | 1 | 1 | canonical `ELAUNCH-STEP-003` |
| Executor Exception Stops | 2 | 1 | 1 | canonical `ELAUNCH-STEP-004` |
| Cancellation | 2 | 1 | 1 | `ELAUNCH-SIM-003` plus canonical `ELAUNCH-STEP-005` |

## Deterministic Progress Evidence

Timed Progress Success used:

```text
Logical duration: 1
Progress samples: 4
```

Accepted samples:

```text
0.25s: 25%
0.50s: 50%
0.75s: 75%
1.00s: 100%
```

## Warning and Failure Evidence

- Simulated warning remained inside the report as
  `ELAUNCH-SIM-STEP-001`.
- Optional recoverable failure remained identifiable as
  `ELAUNCH-SIM-STEP-002` after policy conversion.
- Blocking failure used `ELAUNCH-SIM-STEP-003`.
- Timeout used existing canonical `ELAUNCH-STEP-003`.
- Executor exception used existing canonical `ELAUNCH-STEP-004`.
- Caller cancellation used existing canonical `ELAUNCH-STEP-005`.

Expected scenario outcomes did not pollute the Unity Console.

## Cancellation Determinism Finding

### Initial observation

The first manual cancellation report contained:

```text
Logical elapsed: 439.5
ElapsedSeconds: 439.5
```

The cancellation behavior was correct, but the copied evidence depended on how
long the user waited before pressing Cancel.

### Correction

The Simulator report-copy layer now emits:

```text
Logical elapsed: 0
Details: ExecutorCompletedWithoutException: False
```

The report does not contain `ElapsedSeconds:`.

The Runtime runner and canonical cancellation semantics were not changed.

### Repeat proof

Three manual cancellation reruns produced the exact same fingerprints:

```text
Request:
9194366c11d2aadf1ec110389a6a5f2645f30f9c17bfa137da4ac43a06065aa5

Plan:
ac3aac48c8ea0724566666627281194242aa4c7a7eddb6d16a7c4560e8ca1e45

Report:
e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b
```

## Mutation and Cleanup Evidence

- No authored configuration was edited.
- No authored startup sequence was edited.
- No project asset was created.
- No scene was modified.
- Build Settings remained unchanged.
- ProjectSettings remained unchanged.
- Solution-file drift was restored before staging.
- Transient cleanup tests passed.
- Implementation staging contained only the approved Editor, test, metadata,
  and Runtime friend-access paths.

## Final Console Gate

```text
Errors:   0
Warnings: 0
```

## Conclusion

FL-M5-06 satisfies EchoLaunch-ADR-009 and the approved Checkpoint Build Plan.
The Simulator is deterministic, Editor-only, non-destructive, and based on the
real startup runner.
