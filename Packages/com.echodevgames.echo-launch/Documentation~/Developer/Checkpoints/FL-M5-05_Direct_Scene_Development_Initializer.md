# FL-M5-05 — Direct Scene Development Initializer

## Record Status

- Checkpoint: `FL-M5-05`
- Status: Completed
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.11.0
- ADR: EchoLaunch-ADR-008
- Authority commit: `d538b5a`
- Implementation commit: `4aa6ce7`
- Unity baseline: `6000.3.8f1`
- Completion date: August 6, 2026

## Delivered Outcome

First Light now supports directly opening an explicitly configured gameplay or
Test Lab scene and entering the existing launch architecture.

Delivered:

- Immutable project-owned `DirectSceneConfiguration` schema version `1`
- Stable direct-entry policy, status, result, and diagnostics
- Start-time existing-authority reuse
- Exactly-one approved direct root creation
- Multiple-initializer convergence
- Editor-only default and explicit Development-Build opt-in
- Unconditional non-development release-player prohibition
- Active-destination success without scene reload
- Truthful `DirectSceneDevelopment` report mode
- Activated read-only `ELAUNCH-VAL-009`
- Custom Inspector evidence without mutation controls

## Automated Evidence

```text
Compilation:                 0 errors, 0 warnings
Focused Direct EditMode:     5 passed
Focused Direct PlayMode:    24 passed
Complete EditMode:         266 passed
Complete Runtime PlayMode: 503 passed
Total automated:           769 passed
Failed:                      0
Ignored:                     0
```

## Manual Acceptance

Healthy `EditorOnly` configuration:

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
64706f20f36d21d21bdb61d826f30c698fe7c9cead86109d3ec2132fe075d82e

Report:
cab6e106a92eda1da382133c809f2bc273c5e36ed279fe7bb37908353106aaa3
```

Direct Play created one authority, emitted one settlement message, kept
`OutdoorsScene` active, and completed because the destination was already
active.

A scene-authored root was reused without another clone. Two initializers
produced one created settlement, one reused settlement, and one accepted
authority.

Development-Build opt-in returned `NeedsAttention` with one
`ELAUNCH-VAL-009` Warning. Restoring `EditorOnly` reproduced the exact original
healthy fingerprints.

## Preservation

Temporary project assets, `OutdoorsScene` edits, Build Settings drift, and
solution drift were removed before commit `4aa6ce7`. Only approved package
runtime, Editor, test, and metadata files were committed.

## Deferred Work

Automatic helper installation, build hooks, Simulator, Laboratory, migration,
receipt, uninstall/reset, recovery, player-build evidence, distribution, and
adoption remain outside FL-M5-05.

## Stop Point

No next checkpoint is authorized by this record.
