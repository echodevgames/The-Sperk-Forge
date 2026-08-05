# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

#### FL-M2-06 - Launch Configuration Identity and Root Binding

- Project-owned `EchoLaunchConfiguration` ScriptableObject
- Canonical lowercase 32-character hexadecimal configuration identity
- Serialized configuration schema version `1`
- Internal identity and schema support checks
- Passive serialized configuration binding on `EchoLaunchRoot`
- Read-only authority-filtered `EchoLaunchRoot.Configuration`
- Fifteen Runtime Play Mode configuration-binding tests
- Create menu path under `EchoDevGames/First Light/Launch Configuration`

#### FL-M2-05 - Lifecycle Notifications

- Public `LaunchStateChanged` observer event
- Public `LaunchProgressChanged` observer event
- Previous/current state and progress payloads
- State notification before progress notification
- Per-listener exception containment
- Stable listener-failure diagnostic `ELAUNCH-EVENT-001`
- Notification cleanup when the authoritative root is destroyed
- Twenty Runtime Play Mode notification tests

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

- Passed: `117`
- Failed: `0`
- Ignored: `0`

FL-M2-06 coverage:

- Canonical configuration ID format
- Unique IDs across separate configuration instances
- Stable repeated identity reads
- Current schema version initialization
- Valid identity and supported schema checks
- Malformed identity detection without repair
- Unsupported schema detection without rewrite
- Authoritative configuration exposure
- Null behavior without assignment
- Duplicate-root configuration hiding
- Authority preservation during duplicate creation
- Former-authority hiding after reset
- Fresh-root configuration after reset
- Configuration immutability through root creation and destruction
- Unity Create menu asset creation with no scene side effects

### Not Included

- Automatic lifecycle advancement
- Startup sequence definitions
- Startup sequence execution
- Configuration preflight or blocking diagnostics
- Runtime migration or repair
- Launch reports
- Splash presentation
- Scene loading
- Persistent root lifetime
- Direct-scene initializer behavior
- Editor setup tooling beyond `CreateAssetMenu`
- Standalone Laboratory
- Peer-package bridges

## [0.1.0] - 2026-08-04

### Added

- Initial Unity Package Manager manifest
- Embedded package registration
- Runtime, Editor, Runtime-test, and Editor-test assembly boundaries
- Initial package documentation shell
