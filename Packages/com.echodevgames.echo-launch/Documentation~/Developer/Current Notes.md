# First Light — Current Notes

**Last reconciled:** August 7, 2026
**Working status:** FL-M6-01 closed at `5c21ea4`; FL-M6-02 candidate preparation active
**Authority:** Working context only

## Completed baseline

- Latest completed implementation checkpoint: `FL-M5-07`.
- Authority baseline before FL-M5-07: `741b77d`.
- Implementation commit: `583b91a`.
- Documentation closeout: `daa40c3`.
- FL-M6-01 documentation reconciliation: `5c21ea4`.
- Remote state at the FL-M6-01 gate: `main == origin/main == 5c21ea4`.
- Package version: `0.1.0-beta.1` candidate identity after FL-M6-02 preparation applies.
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.15.0 after FL-M6-02 authorization.
- Compilation: `0` errors, `0` warnings.
- EditMode: `299` passed.
- Runtime Play Mode: `503` passed.
- Total automated: `802` passed, `0` failed, `0` ignored.
- Manual Laboratory: `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` passed.

## Active checkpoint

`FL-M6-02 — Clean-Project Private-Beta Candidate Validation`

This checkpoint first gives the package the intended `0.1.0-beta.1` manifest
identity, then requires a fresh development-repository regression, an
exact-commit checksummed `.tgz`, and a genuinely new Windows Unity `6000.3.8f1`
consumer project. The clean project must follow only shipped instructions for
installation, Quick Start, repeat Setup, Validator, canonical Boot, Laboratory,
sample removal/reimport, package removal/reinstall, and a non-development
Windows player.

Candidate preparation aligns the manifest and the existing public launch-report
package-version constant, plus its retained assertions. It changes no runtime
behavior, API shape, Editor code, prefab, scene, sample, assembly, or dependency.
It does not itself prove any external gate.

## Release target

- Candidate identity: `0.1.0-beta.1`.
- Claimed consumer route: local `.tgz` only after FL-M6-02 passes.
- Baseline: new Windows Unity `6000.3.8f1` project.
- Required proof: install, Quick Start, Setup repeatability, Validator,
  Laboratory, removal/reinstall, canonical Boot, and non-development Windows
  player.
- Private tester: invited user only, under the all-rights-reserved development
  license and separate limited testing permission.
- Public/stable release: not authorized.

## Evidence still not run

- Clean-project tarball installation.
- Windows player build.
- Private tester execution.
- Git URL, registry, and Workshop routes.
- Broad Unity 6 compatibility.
- Historical schema migration.
- Automatic uninstall/reset, receipts, and crash-persistent recovery.
- Automatic Direct Scene installation/build hooks.
- Persistent-root lifetime policy.
- Performance/capacity measurements.
- Existing-project adoption and peer bridges.

## Next action

Apply and inspect the FL-M6-02 candidate-preparation bundle against clean
`5c21ea4`. Commit and synchronize the candidate identity and plan before running
Unity or creating the `.tgz`. Do not create a tag or claim a tarball route yet.
