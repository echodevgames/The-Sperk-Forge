# Installing First Light

First Light is a Unity Package Manager package for ordered startup, launch-only
presentation, launch reporting, Direct Scene development entry, and final
destination handoff.

## Current release state

- Package ID: `com.echodevgames.echo-launch`
- Development version: `0.1.0`
- Primary tested development baseline: Unity `6000.3.8f1`
- Declared Unity floor: `6000.0`
- Required dependency: `com.unity.ugui` `2.0.0`
- Package-local evidence: `299` EditMode and `503` Runtime Play Mode tests;
  `802` total passed with no failures or ignored tests
- Distribution state: private-beta preparation; no public release is claimed

The package-local MVP and Standalone Laboratory are complete. External
clean-project tarball installation remains `Not run` until FL-M6-02.

## Installation-route status

| Route | Current status | Claim |
|---|---|---|
| Embedded package in the development repository | Passed | Development only |
| Local `.tgz` tarball | Planned for FL-M6-02 | Private beta route after evidence passes |
| Local folder reference | Not run | Not currently claimed |
| Git URL/tag | Not run | Not currently claimed |
| Scoped registry | Not run | Not currently claimed |
| Workshop install | Not implemented | Not currently claimed |

Do not describe a planned route as supported until its retained report passes.

## Install a private candidate tarball

When EchoDevGames supplies the candidate `.tgz` and checksum:

1. Create or open the intended Unity `6000.3.8f1` project.
2. Open **Window > Package Management > Package Manager**.
3. Select **Add (+) > Install package from tarball**.
4. Choose the supplied `.tgz` file. Unity's file picker recognizes the `.tgz`
   extension for this route.
5. Wait for dependency resolution and compilation.
6. Select **First Light — Startup and Launch** in Package Manager.
7. Confirm the package ID, candidate version, and uGUI dependency match the
   supplied release record.
8. Confirm the Console has no package-related compile error.

Unity documents the tarball workflow here:
[Install a UPM package from a local tarball](https://docs.unity3d.com/6000.3/Documentation/Manual/upm-ui-tarball.html).

## Embedded development package

The suite repository currently develops First Light at:

```text
Packages/com.echodevgames.echo-launch
```

This editable embedded source is not the consumer artifact. Use it only while
developing the package in The Sperk's Forge repository.

## Verify the installed package

Package Manager should show:

```text
Display name: First Light — Startup and Launch
Package ID:   com.echodevgames.echo-launch
Version:      supplied candidate version
Dependency:   com.unity.ugui 2.0.0
```

The package contains current Runtime, Presentation.UGUI, Editor, Tests,
Documentation~, and Samples~ surfaces. It is not an assembly-definition-only
skeleton.

## Import the Standalone Test Lab

1. Select First Light in Package Manager.
2. Find **First Light Standalone Test Lab** under Samples.
3. Press **Import**.
4. Follow the imported `README.md`.

Unity copies imported samples beneath:

```text
Assets/Samples/First Light — Startup and Launch/<version>/First Light Standalone Test Lab
```

Sample import is explicit. It does not automatically run Setup, modify Build
Settings/Build Profiles, open a scene, enter Play Mode, or make the sample the
project's canonical First Light foundation.

## Next

- [Quick Start](Quick%20Start.md)
- [Setup and Validation](Setup%20and%20Validation.md)
- [Troubleshooting and Known Limitations](Troubleshooting%20and%20Known%20Limitations.md)
- [Removal and Reinstallation](Removal%20and%20Reinstallation.md)
- [Private Beta Test Guide](Private%20Beta%20Test%20Guide.md)
