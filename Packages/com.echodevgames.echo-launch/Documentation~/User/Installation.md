# Installing First Light

## Current Development Status

First Light is currently an embedded development package.

Package version:

    0.1.0

Runtime startup behavior is not implemented in this version.

## Tested Installation Method

The verified installation method is embedding the package directly inside the Unity project at:

    Packages/com.echodevgames.echo-launch

Unity recognizes the folder as a package because it contains:

    package.json

The package is recorded in:

    Packages/packages-lock.json

with the source:

    embedded

## Current Unity Requirements

- Minimum declared Unity version: `6000.0`
- Primary development baseline: `6000.3.8f1`
- Required uGUI package version: `2.0.0`

The declared minimum version is an architectural compatibility target.

Only the primary development baseline has been used during the package-skeleton checkpoint.

## Verifying the Embedded Package

Open Unity Package Manager and confirm that the package appears as:

    First Light - Startup and Launch

The package should report:

- Package ID: `com.echodevgames.echo-launch`
- Version: `0.1.0`
- Source: Custom or Embedded
- uGUI dependency: `2.0.0`

The Unity Console should contain zero package-related errors.

## Current Package Structure

The package currently contains:

    package.json
    README.md
    CHANGELOG.md
    LICENSE.md
    Third Party Notices.md
    Runtime/
    Editor/
    Tests/
    Documentation~/

The Runtime, Editor, and test folders contain assembly definitions only.

## Removal and Reinstallation Evidence

The embedded package was temporarily moved outside the Unity project and then restored from the same folder.

The project compiled with zero red Console errors while First Light was absent.

After restoration:

- First Light returned in Package Manager as version `0.1.0`.
- uGUI remained resolved as `2.0.0`.
- The project compiled with zero red Console errors.
- Runtime asmdef GUID remained `6370d00c0cfa8144795d367cb689f221`.
- Editor asmdef GUID remained `994a9bf984e48cc4a9c5139c901e11f6`.

Because version `0.1.0` contains no runtime configuration, scene objects, prefabs, save data, or generated project content, removal affected only the package skeleton.

## Not Yet Verified

The following installation methods have not been tested:

- Git URL installation
- Git tag installation
- Local package reference outside the project
- Tarball installation
- Scoped registry installation
- Package Manager installation from a public registry
- Installation into a separate clean Unity project

These methods must not be described as supported until their evidence is recorded.
