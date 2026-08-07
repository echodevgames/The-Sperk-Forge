# FL-M6-02 — Clean-Project Private-Beta Candidate Validation Checkpoint Build Plan

**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Checkpoint:** FL-M6-02
**Type:** Candidate identity, packaging, and external validation checkpoint
**Baseline:** Clean synchronized `main` at `5c21ea4`
**Unity baseline:** Windows Unity `6000.3.8f1`
**Candidate version:** `0.1.0-beta.1`
**Claimed route after acceptance:** local `.tgz` only
**Status:** Approved; candidate preparation active
**Owner:** Jesse “Echo” Adams / EchoDevGames

## Purpose

Prove that the completed First Light package can leave its embedded development
repository as one immutable tarball, install in a genuinely new Unity project,
and complete the documented setup, runtime, Laboratory, removal, reinstall, and
Windows player paths without undocumented assistance.

## Starting truth

- FL-M5-07 implementation: `583b91a`.
- FL-M5-07 documentation: `daa40c3`.
- FL-M6-01 documentation reconciliation: `5c21ea4`.
- `main`, `origin/main`, and `origin/HEAD` equal `5c21ea4`.
- Working tree is clean.
- Package manifest remains development version `0.1.0` before preparation.
- Retained package-local evidence is `299` EditMode plus `503` Runtime Play
  Mode tests, `802` total passed, with `0` failed and `0` ignored.
- Clean-project tarball, player-build, and private-tester evidence is `Not run`.

## Authorized work

### Phase A — Candidate preparation

1. Set `package.json` to `0.1.0-beta.1`.
2. Set `LaunchReport.CurrentPackageVersion` and its retained Runtime Play Mode
   assertions to the same candidate identity.
3. Align the changelog, package status, living Current Notes, installation
   status, release plan, and package specification with that candidate identity.
4. Add this Checkpoint Build Plan.
5. Commit and synchronize the preparation before Unity execution or packaging.

### Phase B — Candidate repository gate

1. Open the synchronized candidate commit in Unity `6000.3.8f1`.
2. Require zero package-related compile errors or unexpected warnings.
3. Run the full EditMode matrix and require all `299` retained tests to pass.
4. Run the full Runtime Play Mode matrix and require all `503` retained tests
   to pass.
5. Record exact result files and candidate source commit.

### Phase C — Immutable artifact

1. Require clean `main` at the accepted candidate source commit.
2. Create one `.tgz` whose archive root contains the package's `package.json`.
3. Name the artifact `com.echodevgames.echo-launch-0.1.0-beta.1.tgz`.
4. Record creation tool, byte size, SHA-256, source commit, date, and operating
   system.
5. Inspect the archive for the intended package surface and absence of
   machine-local paths, generated test results, repository metadata, and
   unrelated suite files.
6. Use that exact artifact for every FL-M6-02 consumer-project gate.

### Phase D — New-project documented path

1. Create a genuinely new Windows Unity `6000.3.8f1` project with no unrelated
   Sperk's Forge package or copied project code.
2. Install the exact checksummed tarball through Package Manager.
3. Confirm package ID, version, uGUI `2.0.0`, and clean compilation.
4. Create one project-owned Destination scene.
5. Follow only shipped Installation, Quick Start, and Setup and Validation
   guidance.
6. Require the first Setup Apply to succeed and the repeated plan/apply path to
   settle at `NoChanges`.
7. Require Validator to reach the documented accepted state.
8. Require canonical Boot to reach Destination and finish `Completed`.

### Phase E — Laboratory, lifecycle, and player proof

1. Import the First Light Standalone Test Lab explicitly.
2. Execute the required `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` registry
   and retain the results.
3. Remove the imported sample and prove the project still compiles.
4. Reimport the sample and prove its documented authoring path remains usable.
5. Remove and reinstall the tarball package; prove project-owned First Light
   content remains and Setup reuses it without duplication.
6. Create a non-development Windows player with Boot first and require it to
   reach Destination.
7. Confirm Direct Scene development root creation remains prohibited in that
   non-development player.

### Phase F — Evidence and closeout

1. Retain candidate commit, artifact identity, project environment, Console,
   test, Setup, Validator, Laboratory, removal/reinstall, and player evidence.
2. Record every defect or documentation mismatch before correction.
3. After a correction, create a new candidate artifact and repeat every affected
   gate; never reuse evidence from different candidate bytes.
4. Reconcile Current Notes, changelog, package checkpoint/test report, suite
   completion record, and release checklist.
5. Commit and synchronize the FL-M6-02 closeout separately from candidate
   preparation.

## Explicit exclusions

- No runtime behavior or public-API shape change is planned. Candidate
  preparation changes only the existing report package-version constant and
  its assertions within Runtime/tests.
- No Editor, prefab, scene, sample, assembly, or dependency change.
- No public release, stable release, registry, Git URL, Workshop, or broad Unity
  compatibility claim.
- No annotated release tag; tagging belongs to FL-M6-04.
- No private tester handoff; that belongs to FL-M6-03.
- No Echo Systems Lab, Rescuers2D, Don't Get Vince'd, peer-package, adoption, or
  bridge work.
- No historical-schema migration, performance, persistent-root lifetime,
  automatic uninstall/reset, or crash-recovery claim.

## Acceptance gates

1. Manifest, changelog, docs, artifact name, and installed Package Manager entry
   agree on `0.1.0-beta.1`.
2. The source commit and artifact SHA-256 uniquely identify the tested bytes.
3. The full development matrix passes from the candidate source commit.
4. A new Unity `6000.3.8f1` project installs and compiles the exact `.tgz`.
5. Shipped documentation alone completes Setup, repeatability, Validator, and
   canonical Boot handoff.
6. The complete Laboratory registry passes after tarball installation.
7. Sample removal/reimport and package removal/reinstall preserve the documented
   ownership boundary without duplicates.
8. A non-development Windows player launches through Boot to Destination and
   prohibits Direct Scene development creation.
9. Every correction invalidates and regenerates affected artifact evidence.
10. All external, tester, tag, and release claims remain honest until their
    named gate passes.

## Stop points

- Stop after candidate preparation and commit verification before creating the
  tarball.
- Stop on any compile error, failed required test, documentation mismatch,
  archive mismatch, setup/validation failure, duplicate content, or player
  failure.
- Stop after FL-M6-02 closeout before sending the package to a tester. FL-M6-03
  requires its own handoff and evidence record.
