# FL-M6-01 — Documentation and Release-Plan Reconciliation Checkpoint Build Plan

**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Checkpoint:** FL-M6-01
**Type:** Documentation, authority, and release-planning checkpoint
**Baseline:** Clean `main` at `daa40c3`
**Unity baseline:** 6000.3.8f1
**Status:** Approved and executed through the documentation bundle
**Owner:** Jesse “Echo” Adams / EchoDevGames

## Purpose

Reconcile the suite and package paperwork after FL-M5-07, replace obsolete
user setup documents, and authorize clean-project/private-beta validation
before any optional existing-project adoption.

## Starting truth

- FL-M5-07 implementation commit: `583b91a`.
- FL-M5-07 documentation commit: `daa40c3`.
- `main`, `origin/main`, and `origin/HEAD` were synchronized at `daa40c3`.
- Working tree was clean.
- Automated evidence: `299` EditMode plus `503` Runtime Play Mode tests,
  `802` total, with `0` failed and `0` ignored.
- Package-local Laboratory cases `ELAUNCH-LAB-001` through
  `ELAUNCH-LAB-012` passed.
- Clean-project tarball installation, Windows player build, and private tester
  evidence remained `Not run`.

## Authorized scope

- Audit living suite, package, user, release, handoff, and status documents.
- Record SFGSS-ADR-005.
- Rebaseline First Light M6 and defer adoption to optional M7.
- Replace obsolete Installation and Quick Start pages.
- Add setup/validation, step-authoring, troubleshooting/limitations,
  removal/reinstall, and private-beta test guidance.
- Update release gates and current evidence honestly.
- Add audit, checkpoint, completion, and release-plan records.

## Explicit exclusions

- No Runtime, Editor, test, prefab, scene, sample, or package-manifest changes.
- No package version change or `.tgz` artifact generation.
- No Unity execution or new empirical claim.
- No Echo Systems Lab, Rescuers2D, or Don’t Get Vince’d integration.
- No staging, commit, tag, release, or push by the apply bundle.

## Acceptance gates

1. Every living status surface names `daa40c3` as the closed FL-M5-07 baseline.
2. No living user guide claims startup execution is absent.
3. Suite release policy clearly separates clean-project pre-release from later
   optional adoption.
4. First Light M6 consistently names FL-M6-01 through FL-M6-04.
5. User setup guidance matches implemented menu paths and Editor behavior.
6. All new relative Markdown links resolve.
7. Historical checkpoint plans and reports remain unchanged.
8. The bundle is exact-baseline guarded, checksum verified, non-staging, and
   non-committing.

## Stop point

After the documentation commit is pushed, stop before changing `package.json`,
creating a tarball, or opening a consumer project. FL-M6-02 begins only from a
separate approved plan against the clean documentation baseline.
