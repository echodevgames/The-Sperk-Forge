# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
- Current implemented boundary: authority ownership plus neutral launch-state vocabulary
- Unity baseline: `6000.3.8f1`

## User Documentation

- [Installation](User/Installation.md)
- [Quick Start](User/Quick%20Start.md)

## Developer Documentation

- [Architecture](Developer/Architecture.md)
- [Current Notes](Developer/Current%20Notes.md)
- [FL-M1-01 Package Skeleton](Developer/Checkpoints/FL-M1-01_Package_Skeleton.md)
- [FL-M2-01 Authority Claim and Static Reset Core](Developer/Checkpoints/FL-M2-01_Authority_Claim_and_Static_Reset_Core.md)
- [FL-M2-02 Neutral Launch-State Vocabulary](Developer/Checkpoints/FL-M2-02_Neutral_Launch-State_Vocabulary.md)
- [FL-M2-01 Runtime Test Report](Developer/Test%20Reports/FL-M2-01_Authority_Runtime_Test_Report.md)
- [FL-M2-02 Runtime Test Report](Developer/Test%20Reports/FL-M2-02_Launch-State_Vocabulary_Test_Report.md)

## Package Root Documents

- [README](../README.md)
- [Changelog](../CHANGELOG.md)
- [License](../LICENSE.md)
- [Third-Party Notices](../Third%20Party%20Notices.md)

## Architectural Authority

The complete suite architecture and approved First Light specification live in the repository's `Plan Documentation` Obsidian vault.

Package documentation applies those authorities. It does not replace them.

## Current Runtime Boundary

First Light currently proves:

- One valid launch authority
- Duplicate rejection
- Owner-only release
- Static reset
- Stable launch-mode vocabulary
- Stable launch and step-status vocabulary
- Immutable structured step results
- Immutable validated progress snapshots
- Forty-six passing Runtime Play Mode tests

Startup configuration and execution remain outside the implemented scope.
