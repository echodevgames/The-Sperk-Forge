# FL-M1-01 - First Light Package Skeleton

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M1-01`
- Checkpoint status: Complete, pending commit and push
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- Resolved uGUI version: `2.0.0`
- Runtime behavior authorized: No
- Stop point: Before the first C# implementation file

## Purpose

Establish the First Light Unity Package Manager identity, assembly boundaries, legal and release documents, and package-local documentation structure without introducing runtime behavior.

## Authorized Scope

This checkpoint created:

- `package.json`
- Runtime assembly definition
- Editor assembly definition
- Runtime test assembly definition
- Editor test assembly definition
- Package README
- Changelog
- Development license notice
- Third-party notices
- `Documentation~` shell
- Unity-generated `.meta` files
- Package Manager, compilation, restart, removal, re-add, and documentation-route evidence

## Explicitly Excluded

This checkpoint does not contain:

- C# implementation files
- `EchoLaunchRoot`
- Startup definitions or executors
- ScriptableObjects
- Scenes
- Prefabs
- Splash presenters
- Setup tools
- Bridges
- Samples
- Runtime startup behavior

## Assembly Boundaries

### Runtime

- Assembly: `EchoDevGames.EchoLaunch.Runtime`
- Root namespace: `EchoDevGames.EchoLaunch`
- Auto Referenced: Yes
- Platform restriction: None
- Runtime asmdef GUID: `6370d00c0cfa8144795d367cb689f221`

### Editor

- Assembly: `EchoDevGames.EchoLaunch.Editor`
- Root namespace: `EchoDevGames.EchoLaunch.Editor`
- Auto Referenced: No
- Platform restriction: Editor
- References: Runtime
- Editor asmdef GUID: `994a9bf984e48cc4a9c5139c901e11f6`

### Runtime Tests

- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Auto Referenced: No
- References: Runtime
- Unity test assembly: Yes

### Editor Tests

- Assembly: `EchoDevGames.EchoLaunch.Tests.Editor`
- Auto Referenced: No
- Platform restriction: Editor
- References: Runtime and Editor
- Unity test assembly: Yes

## Dependency Direction

    Editor -> Runtime
    Runtime Tests -> Runtime
    Editor Tests -> Editor + Runtime

Prohibited directions:

    Runtime -X-> Editor
    Runtime -X-> Tests
    Editor -X-> Tests
    Production Code -X-> Test Assemblies

## Evidence Registry

| Evidence | Status | Notes |
|---|---|---|
| Unity baseline opens | Pass | Unity `6000.3.8f1` |
| Initial Console compile | Pass | Zero red errors |
| Initial Git tree | Pass | Clean before package creation |
| Package path clear | Pass | No prior package folder |
| uGUI version resolved | Pass | `2.0.0` from lock file |
| Package manifest parses | Pass | JSON validation |
| Embedded package recognized | Pass | Visible in Package Manager |
| Package lock updated | Pass | Embedded source and dependency recorded |
| Runtime asmdef parses | Pass | JSON validation |
| Editor asmdef parses | Pass | JSON validation |
| Runtime test asmdef parses | Pass | JSON validation |
| Editor test asmdef parses | Pass | JSON validation |
| Unity assembly compile | Pass | Zero package-related Console errors |
| Runtime GUID generated | Pass | `.meta` preserved |
| Editor GUID generated | Pass | `.meta` preserved |
| No C# files | Pass | Recursive package check |
| Package documentation shell | Pass | Root, user, and developer documents created |
| Documentation links | Pass | All package-local Markdown targets resolve |
| Unity restart | Pass | Package returned after restart with zero red Console errors |
| Removal and reinstallation | Pass | Project compiled without package; restored package compiled with stable GUIDs |
| Git installation | Not run | Later distribution evidence |
| Tarball installation | Not run | Later distribution evidence |
| Clean-project installation | Not run | Later distribution evidence |
| Player builds | Not run | No runtime behavior yet |
| Runtime behavior | Not run | Not authorized |
| Performance | Not run | No runtime behavior yet |

## Current Git Scope

Expected tracked changes for this checkpoint:

- `Packages/packages-lock.json`
- `Packages/com.echodevgames.echo-launch/**`
- Adjacent suite checkpoint documentation

Unrelated changes, including personal Obsidian workspace state, must not be included.

## Closure Result

All FL-M1-01 package-skeleton evidence required for closure has passed.

The checkpoint is ready for final Git review, commit, and push.

Runtime implementation remains locked.
