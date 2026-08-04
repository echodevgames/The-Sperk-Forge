# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

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

- First root claims authority
- Second root is rejected without replacing authority
- Duplicate destruction does not release authority
- Authority destruction releases the claim
- Static reset clears current authority
- Fresh root claims after reset
- Deferred Unity destruction permits a fresh claim

Result:

- Passed: `7`
- Failed: `0`
- Ignored: `0`

The two duplicate-root warnings were expected and matched through `LogAssert.Expect`.

### Not Included

- Startup configuration
- Startup sequence execution
- Launch reports
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
