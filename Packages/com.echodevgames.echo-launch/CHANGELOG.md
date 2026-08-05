# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

#### FL-M2-02 - Neutral Launch-State Vocabulary

- `LaunchMode`
- `LaunchStatus`
- `StartupStepStatus`
- Immutable `StartupStepResult`
- Immutable `LaunchProgressSnapshot`
- Named result factories for success, warning, failure, skipped, timed out, and cancelled outcomes
- Diagnostic code and message validation
- Policy-neutral convenience classification
- Progress, elapsed-time, step-count, and active-index validation
- Null-string normalization
- Thirty-nine Runtime Play Mode vocabulary tests

#### FL-M2-01 - Authority Claim and Static Reset Core

- Internal `LaunchAuthorityClaim` kernel
- Public `EchoLaunchRoot` scene component
- Single-authority claim behavior
- Duplicate rejection before future startup side effects
- Stable duplicate diagnostic code `ELAUNCH-ROOT-001`
- Owner-only authority release
- Static reset using `RuntimeInitializeLoadType.SubsystemRegistration`
- Runtime test access through `InternalsVisibleTo`
- Seven Runtime Play Mode authority tests

### Tested

Runtime Play Mode totals:

- Passed: `46`
- Failed: `0`
- Ignored: `0`

Vocabulary coverage includes:

- Stable enum values
- Factory status mapping
- Result classification
- Diagnostic validation
- Text normalization
- Valid inactive and active snapshots
- Invalid counts, indices, progress, and elapsed time
- Snapshot immutability

Authority coverage remains green:

- First root claims authority
- Duplicate root is rejected
- Duplicate destruction preserves authority
- Authority destruction releases the claim
- Static reset clears authority
- Fresh root claims after reset
- Deferred destruction permits a fresh claim

### Not Included

- Startup configuration
- Startup sequence execution
- Launch-session mutation
- Report aggregation
- Progress publication
- Splash presentation
- Scene loading
- Persistent root lifetime
- Direct-scene initialization
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

### Evidence

- Unity recognized the embedded package in Unity `6000.3.8f1`.
- Unity resolved `com.unity.ugui` version `2.0.0`.
- All four assembly definitions parsed successfully.
- The package compiled with zero Console errors.
- Unity restart verification passed.
- Embedded-package removal and reinstallation passed.
- Assembly-definition GUIDs remained stable after reinstallation.
- No C# implementation files were included in FL-M1-01.
