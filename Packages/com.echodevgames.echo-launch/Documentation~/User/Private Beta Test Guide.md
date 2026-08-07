# First Light Private Beta Test Guide

This guide is for an invited private tester using the exact candidate artifact
and test instructions supplied by EchoDevGames. It is not a public release or
permission to redistribute the package.

## What the tester receives

- One First Light `.tgz` candidate.
- Its SHA-256 checksum.
- The expected package version and source commit/tag.
- This documentation set.
- A short report template or evidence collector.

## Required environment

- Windows.
- Unity `6000.3.8f1`.
- A genuinely new Unity project.
- No other Sperk’s Forge package or copied project code.
- Enough disk space to create one Windows player build.

## Tester path

1. Verify the supplied checksum.
2. Install the candidate through **Package Manager > Add (+) > Install package
   from tarball**.
3. Confirm First Light appears at the supplied version and Unity resolves uGUI.
4. Follow [Quick Start](Quick%20Start.md) without undocumented assistance.
5. Copy the first Validator report.
6. Run canonical Boot and confirm the destination is reached.
7. Import **First Light Standalone Test Lab**.
8. Run the documented success, warning/recoverable, blocking, duplicate-root,
   invalid-destination, and Direct Scene checks.
9. Remove and reimport the sample.
10. Run Setup again and confirm `NoChanges` or explain every displayed
    difference.
11. Build and run a non-development Windows player beginning at canonical Boot.
12. Remove and reinstall the candidate, preserving project-owned setup content,
    then validate and run Boot again.

## Report every point of confusion

Report an issue even when the package eventually works if:

- a menu or field name differs from the guide;
- a step requires help not present in the guide;
- the accepted result is unclear;
- an expected warning looks like a defect;
- removal or reinstall ownership is confusing; or
- evidence cannot be copied or identified.

For each issue include:

- Unity version and Windows version;
- candidate filename/checksum;
- exact step being followed;
- expected result;
- actual result;
- Console message or diagnostic code;
- screenshot when visual state matters; and
- whether the issue repeats in a new project.

## Stop conditions

Stop and report immediately for:

- any compile error;
- unexpected package-source mutation;
- package import that changes unrelated project settings/content;
- duplicate launch side effects;
- an unhandled exception;
- loss or overwrite of unrelated project content; or
- a non-development player creating a Direct Scene development authority.
