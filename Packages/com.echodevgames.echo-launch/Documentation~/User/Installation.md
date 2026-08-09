# Installing First Light

## Current Development Status

Package version:

```text
0.1.0
```

First Light currently contains implemented Runtime, Presentation.UGUI, Editor tooling, tests, documentation, prefabs, and one separately importable Standalone Test Laboratory sample.

The verified repository-development installation is an **embedded package**. A versioned `0.1.0` tarball is now prepared in the repository Distribution Kit for handoff/evaluation, but external clean-project tarball qualification and public registry support are not claimed yet.

## Official Repository Distribution Kit

The versioned companion kit lives at:

```text
Distributions/First Light/0.1.0/
```

Artifact:

```text
com.echodevgames.echo-launch-0.1.0.tgz
```

Before using the artifact, compare its SHA-256 against `SHA256SUMS.txt`.

For evaluation in Unity Package Manager:

1. Open **Window > Package Management > Package Manager**.
2. Use the **+** menu.
3. Choose **Add package from tarball...**.
4. Select `com.echodevgames.echo-launch-0.1.0.tgz`.
5. Allow Unity to resolve dependencies and compile.
6. Confirm package ID/version and uGUI dependency.
7. Continue with `Tools > Sperk's Forge > First Light > Setup`.

This is an artifact-availability instruction, not a release-support claim. SFGSS-004 still requires external clean-project install, quick-start/Lab proof, removal, and reinstall evidence before the tarball route becomes Supported.

For the full setup/capability/troubleshooting reference, read [Complete User Handout](Complete%20User%20Handout.md).

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
- Clean-project tarball installation support / removal-reinstall qualification (the `0.1.0` tarball artifact exists, but this route has not yet passed its external proof)
- Scoped registry installation
- Public Package Manager registry installation
- Separate clean-project reproduction of the final A1/A1-E1 happy path
- Player-build qualification

Do not describe these as supported until their evidence is recorded in a later release-qualification checkpoint.
