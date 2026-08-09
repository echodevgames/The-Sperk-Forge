# First Light 0.1.0 Distribution Manifest

## Identity

| Field | Value |
|---|---|
| Public title | First Light – Startup and Launch |
| Technical package | `EchoLaunch` |
| Package ID | `com.echodevgames.echo-launch` |
| Package version | `0.1.0` |
| Artifact | `com.echodevgames.echo-launch-0.1.0.tgz` |
| Source baseline | `6bd268d` plus this Distribution Kit documentation/packaging checkpoint |
| Kit path | `Distributions/First Light/0.1.0/` |
| Publisher | Jesse "Echo" Adams / EchoDevGames |

## Kit Contents

```text
README.md
com.echodevgames.echo-launch-0.1.0.tgz
FIRST_LIGHT_COMPLETE_USER_HANDOUT.md
DISTRIBUTION_MANIFEST.md
DISTRIBUTION_BUILD_RECORD.txt
SHA256SUMS.txt
```

## Artifact Scope

The tarball is assembled from:

```text
Packages/com.echodevgames.echo-launch/
```

and stored with one top-level:

```text
package/
```

UPM package content includes the package manifest, Runtime, Presentation.UGUI, Editor tooling, tests, Samples~, Documentation~, license/notices, README, and changelog present in the package tree at kit build time.

The complete user handout is included in package documentation and copied beside the tarball for direct recipient access.

## Explicit Exclusions

The tarball does not contain the repository-owned production Reference Gallery:

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
```

That Gallery contains project-owned First Light and UMBRA examples. It is consumer evidence/reference material, not a package runtime dependency or default project content.

The tarball does not include unrelated Sperk's Forge packages or project assets.

## Integrity

Use:

```text
SHA256SUMS.txt
DISTRIBUTION_BUILD_RECORD.txt
```

The build record is generated when the tarball is assembled and records artifact size and SHA-256.

## Current Qualification State

| Gate | State | Evidence |
|---|---|---|
| Package implementation / Reference Gallery | Pass | FL-M6-01 closeout |
| Final Setup-focused gate | Pass | `224 / 224` |
| Retained automated baseline | Pass | `809 / 809` |
| Retained manual Standalone Lab | Pass | `12 / 12` |
| UMBRA independent consumer proof | Pass | Create -> runtime -> repeated `NoChanges` |
| Versioned distribution artifact prepared | Pass after this kit builder completes | tarball + hash/build record |
| External clean-project tarball install | Not run | future release-qualification return |
| Tarball removal/reinstall | Not run | future release-qualification return |
| Fresh complete post-A1 suite regression | Not run | future release-qualification return |
| Player build qualification | Not run | future release-qualification return |
| Performance qualification | Not run | future release-qualification return |
| Git tag/catalog/private beta | Not run | future release-qualification return |

## Support-Language Rule

This is an official repository distribution snapshot and handoff artifact.

It is not yet evidence that the tarball installation route is release-qualified. Support claims must follow retained evidence under SFGSS-004.
