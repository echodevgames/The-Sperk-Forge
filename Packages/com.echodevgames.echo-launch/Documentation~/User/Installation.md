# Installing First Light

## Current Development Status

Package version:

```text
0.1.0
```

First Light currently contains implemented Runtime, Presentation.UGUI, Editor tooling, tests, documentation, prefabs, and one separately importable Standalone Test Laboratory sample.

The current repository-development installation is an **embedded package**. This document does not claim public registry or clean-project distribution support yet.

## Verified Embedded Development Method

The package is embedded at:

```text
Packages/com.echodevgames.echo-launch
```

Unity recognizes the folder through `package.json`, and the repository records the embedded package in `Packages/packages-lock.json`.

In Unity Package Manager confirm:

- Display name: First Light - Startup and Launch
- Package ID: `com.echodevgames.echo-launch`
- Version: `0.1.0`
- Source: Custom or Embedded
- uGUI dependency: `2.0.0`

## Unity Requirements

- Minimum declared Unity version: `6000.0`
- Primary development baseline: `6000.3.8f1`
- Required uGUI package version: `2.0.0`

The declared minimum is a package compatibility target. The retained development evidence in this repository is from Unity `6000.3.8f1`.

## Package Structure

The package currently includes:

```text
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Presentation.UGUI/
Editor/
Tests/
Samples~/
Documentation~/
```

## After Installation

Use:

```text
Tools > Sperk's Forge > First Light > Setup
```

Setup can preview/create a project-owned First Light foundation, reuse compatible project assets, or explicitly create an independent project-owned foundation. See [Quick Start](Quick%20Start.md).

The package itself does not require the repository's `Assets/EchoDevGames/SuiteShowcase/First Light Gallery/**` content. That Gallery is project-owned reference material in The Sperk's Forge development repository.

## Standalone Test Laboratory

Unity Package Manager exposes one sample:

```text
First Light Standalone Test Lab
```

Importing the sample does not automatically run Setup, Repair, Validator, Simulator, Play Mode, or modify Build Settings. Standard imported `Assets/Samples/**` content is excluded from automatic Setup candidate discovery unless explicitly selected.

## Removal Evidence

The embedded package has previously been removed from and restored to the development project while preserving its package identity and assembly-definition GUIDs.

Project-owned content created by Setup lives under the consumer project's `Assets/**`. Removing the package does not mean that user-authored project assets should be silently deleted.

## Not Yet Claimed as Supported

The following routes/qualification steps are not claimed by the FL-M6-01 closeout:

- Git URL installation
- Git tag installation
- Local package reference outside the development project
- Tarball installation
- Scoped registry installation
- Public Package Manager registry installation
- Separate clean-project reproduction of the final A1/A1-E1 happy path
- Player-build qualification

Do not describe these as supported until their evidence is recorded in a later release-qualification checkpoint.
