# First Light — Current Notes

**Last reconciled:** August 7, 2026
**Working status:** FL-M6-01 documentation bundle prepared; commit/push pending
**Authority:** Working context only

## Completed baseline

- Latest completed implementation checkpoint: `FL-M5-07`.
- Authority baseline before FL-M5-07: `741b77d`.
- Implementation commit: `583b91a`.
- Documentation closeout: `daa40c3`.
- Remote state at closeout: `main == origin/main == daa40c3`.
- Package version: `0.1.0` development.
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.14.0 after FL-M6-01.
- Compilation: `0` errors, `0` warnings.
- EditMode: `299` passed.
- Runtime Play Mode: `503` passed.
- Total automated: `802` passed, `0` failed, `0` ignored.
- Manual Laboratory: `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` passed.

## Active checkpoint

`FL-M6-01 — Documentation and Release-Plan Reconciliation`

This checkpoint:

- replaces the FL-M1/FL-M2-era Installation and Quick Start pages;
- documents Setup, Apply, Repair, Validator, Boot, startup-step authoring,
  troubleshooting, removal, reinstall, and private testing;
- accepts SFGSS-ADR-005;
- moves optional project adoption/bridges to M7; and
- defines FL-M6-02 through FL-M6-04 as clean-project candidate, private tester,
  and private beta closeout work.

It changes no Runtime, Editor, test, prefab, scene, sample, or package-manifest
file and claims no new Unity evidence.

## Release target

- Planned candidate: `0.1.0-beta.1`.
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

Apply, inspect, commit, and push the FL-M6-01 documentation bundle. Confirm a
clean synchronized repository, then create the separate FL-M6-02 plan before
changing `package.json` or producing a `.tgz`.
