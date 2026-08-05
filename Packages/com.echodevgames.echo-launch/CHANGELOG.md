# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

#### FL-M2-03 - Launch Session and Read-Only Progress Surface

- Internal `LaunchSession`
- One fresh session per authoritative root
- Canonical initial `AuthorityClaimed` snapshot
- `LaunchProgressSnapshot.Empty`
- Public read-only `EchoLaunchRoot.State`
- Public read-only `EchoLaunchRoot.Progress`
- Internal controlled `PublishProgress`
- Duplicate and stale-root state hiding
- Mode-mismatch validation
- `None`-status publication rejection
- Undefined-mode rejection
- Fourteen Runtime Play Mode session and progress tests

#### FL-M2-02 - Neutral Launch-State Vocabulary

- `LaunchMode`
- `LaunchStatus`
- `StartupStepStatus`
- Immutable `StartupStepResult`
- Immutable `LaunchProgressSnapshot`
- Thirty-nine Runtime Play Mode vocabulary tests

#### FL-M2-01 - Authority Claim and Static Reset Core

- Internal `LaunchAuthorityClaim`
- Public `EchoLaunchRoot`
- Duplicate rejection
- Stable diagnostic code `ELAUNCH-ROOT-001`
- Seven Runtime Play Mode authority tests

### Tested

Runtime Play Mode totals:

- Passed: `60`
- Failed: `0`
- Ignored: `0`

FL-M2-03 coverage:

- Fresh authority session
- Canonical initial progress
- Direct-scene mode session
- Safe empty snapshot
- Duplicate state hiding
- Snapshot replacement
- Same-state publication
- Previous snapshot immutability
- Mode mismatch rejection
- `None` status rejection
- Undefined mode rejection
- Duplicate publication rejection
- Static-reset stale-state hiding
- Fresh session after authority destruction

### Not Included

- Startup configuration
- Startup sequence execution
- Lifecycle transition rules
- Public progress events
- Launch reports
- Splash presentation
- Scene loading
- Persistent root lifetime
- Direct-scene initializer behavior
- Editor tooling
- Standalone Laboratory
- Peer-package bridges

## [0.1.0] - 2026-08-04

### Added

- Initial Unity Package Manager manifest
- Embedded package registration
- Runtime assembly boundary
- Editor assembly boundary
- Runtime test assembly boundary
- Editor test assembly boundary
- Initial package documentation shell
