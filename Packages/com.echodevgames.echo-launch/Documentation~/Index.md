# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoint: `FL-M2-01`
- Implemented capability: Authority Claim and Static Reset Core
- Unity baseline: `6000.3.8f1`

## User Documentation

- [Installation](User/Installation.md)
- [Quick Start](User/Quick%20Start.md)

## Developer Documentation

- [Architecture](Developer/Architecture.md)
- [Current Notes](Developer/Current%20Notes.md)
- [FL-M1-01 Package Skeleton](Developer/Checkpoints/FL-M1-01_Package_Skeleton.md)
- [FL-M2-01 Authority Claim and Static Reset Core](Developer/Checkpoints/FL-M2-01_Authority_Claim_and_Static_Reset_Core.md)
- [FL-M2-01 Runtime Test Report](Developer/Test%20Reports/FL-M2-01_Authority_Runtime_Test_Report.md)

## Package Root Documents

- [README](../README.md)
- [Changelog](../CHANGELOG.md)
- [License](../LICENSE.md)
- [Third-Party Notices](../Third%20Party%20Notices.md)

## Architectural Authority

The complete suite architecture and approved First Light specification live in the repository's `Plan Documentation` Obsidian vault.

Package documentation applies those authorities. It does not replace them.

## Current Runtime Boundary

First Light currently proves only:

- One valid launch authority
- Duplicate rejection
- Owner-only release
- Static reset
- Seven passing authority tests

Startup configuration and execution remain outside the implemented scope.
