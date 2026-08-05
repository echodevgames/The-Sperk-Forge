# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
- Current implemented boundary: authority, launch-state vocabulary, and one live read-only launch session
- Unity baseline: `6000.3.8f1`

## User Documentation

- [Installation](User/Installation.md)
- [Quick Start](User/Quick%20Start.md)

## Developer Documentation

- [Architecture](Developer/Architecture.md)
- [Current Notes](Developer/Current%20Notes.md)
- [FL-M2-03 Launch Session and Read-Only Progress Surface](Developer/Checkpoints/FL-M2-03_Launch_Session_and_Read-Only_Progress_Surface.md)
- [FL-M2-03 Runtime Test Report](Developer/Test%20Reports/FL-M2-03_Launch_Session_and_Progress_Test_Report.md)

## Package Root Documents

- [README](../README.md)
- [Changelog](../CHANGELOG.md)
- [License](../LICENSE.md)
- [Third-Party Notices](../Third%20Party%20Notices.md)

## Current Runtime Boundary

First Light currently proves:

- One valid launch authority
- Duplicate rejection
- Owner-only release
- Static reset
- Stable launch-state vocabulary
- Immutable results and snapshots
- One fresh launch session per authority
- Read-only state and progress
- Controlled snapshot replacement
- Sixty passing Runtime Play Mode tests

Startup execution remains outside the implemented scope.
