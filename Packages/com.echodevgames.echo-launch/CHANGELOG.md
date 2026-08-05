# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

#### FL-M2-04 - Launch Lifecycle Transition Guard

- Internal `LaunchStateTransitionRules`
- Approved lifecycle transition matrix
- Same-state progress publication for active states
- Failure and interruption paths from active states
- Rejection of backward transitions
- Rejection of skipped lifecycle phases
- Permanent freezing of `Completed`, `Failed`, and `Interrupted`
- Transactional `LaunchSession.Publish` behavior
- Twenty-two Runtime Play Mode lifecycle transition cases
- Lifecycle-aligned maintenance of the existing session test suite

#### FL-M2-03 - Launch Session and Read-Only Progress Surface

- Internal `LaunchSession`
- One fresh session per authoritative root
- `LaunchProgressSnapshot.Empty`
- Public read-only root state and progress
- Fourteen Runtime Play Mode session and progress tests

#### FL-M2-02 - Neutral Launch-State Vocabulary

- Launch-mode, lifecycle, step-status, result, and snapshot vocabulary
- Thirty-nine Runtime Play Mode vocabulary tests

#### FL-M2-01 - Authority Claim and Static Reset Core

- Single launch authority
- Duplicate rejection
- Stable diagnostic code `ELAUNCH-ROOT-001`
- Seven Runtime Play Mode authority tests

### Tested

Runtime Play Mode totals:

- Passed: `82`
- Failed: `0`
- Ignored: `0`

FL-M2-04 coverage:

- Terminal-state recognition
- Approved forward transitions
- Same active-state publication
- Failure and interruption paths
- Backward-transition rejection
- Skipped-phase rejection
- Undefined-status rejection
- `None` publication rejection
- Terminal-session freezing
- Snapshot preservation after rejected publication
- Root publication using the lifecycle guard automatically

### Not Included

- Automatic lifecycle advancement
- Startup configuration
- Startup sequence execution
- Public lifecycle events
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
- Runtime, Editor, Runtime-test, and Editor-test assembly boundaries
- Initial package documentation shell
