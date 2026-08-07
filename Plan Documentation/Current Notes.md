# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 7, 2026
**Current focus:** First Light release preparation
**Current checkpoint:** FL-M6-01 — Documentation and Release-Plan Reconciliation

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

## Repository baseline

- Branch: `main`.
- Last completed implementation: `583b91a` — Implement FL-M5-07 standalone
  test laboratory.
- Last completed documentation: `daa40c3` — Close out FL-M5-07 standalone
  test laboratory.
- Remote state at the completed gate: `main == origin/main == daa40c3`.
- Working tree at the completed gate: clean.
- Package version: `0.1.0` development.

## Completed First Light evidence

- Package-local MVP complete through FL-M5-07.
- Standalone Test Lab is one explicit removable Package Manager sample.
- `299` EditMode tests passed.
- `503` Runtime Play Mode tests passed.
- `802` total automated tests passed; `0` failed; `0` ignored.
- `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` passed manually.
- Setup/Repair sample isolation and persistent Direct Scene configuration
  reference defects were corrected and regression-tested.

## FL-M6-01 findings

1. The living suite status documents still pointed to FL-M1-01 and claimed
   First Light implementation had not begun.
2. Package Installation and Quick Start pages still described the skeleton and
   FL-M2-04 boundaries.
3. SFGSS-000 required both clean-project and real-project integration before
   leaving beta, while SFGSS-004's beta gate did not universally require
   adoption.
4. SFGSS-001 and the First Light milestone table placed integration before
   release even though the suite's standalone-first rule requires clean-project
   proof first.
5. No current release plan, private tester guide, removal guide, troubleshooting
   guide, or package-local beta checklist existed.

## Promoted decision

SFGSS-ADR-005 is accepted:

- package-local implementation and Laboratory proof;
- clean-project pre-release proof;
- invited private test;
- private beta closeout;
- optional later adoption only when deliberately selected.

A private beta may close one package cycle and allow the next package's
just-in-time learning review. Adoption remains required before its specific
parity/integration claim, not before every package beta.

## Approved First Light M6 path

1. `FL-M6-01` — Documentation and Release-Plan Reconciliation.
2. `FL-M6-02` — Clean-Project Private-Beta Candidate Validation.
3. `FL-M6-03` — Private Tester Handoff and Findings.
4. `FL-M6-04` — Private Beta Closeout and Tag.
5. Optional M7 adoption/bridge work only after a separate selection.

No Echo Systems Lab, Rescuers2D, or Don’t Get Vince’d integration is active.

## Honest evidence boundary

Clean-project `.tgz` installation, Windows player build, private tester results,
Git/registry routes, broad compatibility, performance, historical migration,
and adoption remain `Not run`.

## Next action

Apply and inspect the FL-M6-01 documentation bundle, commit/push it adjacent to
`daa40c3`, confirm a clean synchronized repository, and stop before package
versioning or candidate artifact generation. FL-M6-02 requires a new plan.
