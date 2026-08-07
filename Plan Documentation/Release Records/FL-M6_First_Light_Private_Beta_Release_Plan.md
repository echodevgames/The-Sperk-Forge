# FL-M6 — First Light Private Beta Release Plan

**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Current package version:** `0.1.0-beta.1` candidate identity after preparation
**Planned private beta:** `0.1.0-beta.1`
**Baseline before candidate work:** `5c21ea4` — FL-M6-01 documentation closeout
**Release audience:** Jesse and specifically invited private testers
**Public release:** Not authorized

## Release claim

The target is a private beta proving that the implemented First Light MVP can
be installed from one checksummed `.tgz`, configured by following only the
shipped documentation, exercised in a clean Unity project, built for Windows,
removed, reinstalled, and handed to an invited tester.

The target does not claim stable API support, public redistribution rights,
real-project adoption, bridge compatibility, broad Unity 6 compatibility,
historical schema migration, performance certification, or public registry
availability.

## FL-M6 sequence

| Checkpoint | Outcome | Required evidence |
|---|---|---|
| FL-M6-01 | Documentation and release plan reconciled | Authority audit, current user guides, SFGSS-ADR-005, clean baseline |
| FL-M6-02 | Clean-project candidate validated | Matching manifest/candidate version, checksummed `.tgz`, new Unity 6000.3.8f1 project, import/setup/Validator/Lab/removal/reinstall, Windows player build |
| FL-M6-03 | Invited tester completes documented path | Tester environment, results, confusion/defects, retained evidence, issue disposition |
| FL-M6-04 | Private beta closed | Full regression matrix, final docs/changelog/limitations, matching annotated tag, artifact/checksum, release record, clean synchronized repository |

## Candidate installation route

The private beta claims only the local tarball route. Embedded development is
retained as development evidence. Git URL, registry, Workshop, and public
distribution routes remain unclaimed.

## Clean-project baseline

- Unity Editor: `6000.3.8f1`.
- Operating system: Windows.
- Project: genuinely new project with no unrelated Sperk’s Forge package or
  copied project code.
- Package source: exact candidate `.tgz` and SHA-256 checksum.
- Required dependency: `com.unity.ugui` `2.0.0` as resolved by the candidate.
- Destination: one project-owned scene created in the clean project.
- Production proof: canonical Boot scene and non-development Windows player
  build.
- Development proof: imported Standalone Test Lab and documented Direct Scene
  behavior.

## Release blockers

- Any compile error or unexpected package warning.
- Any failed required automated test.
- Any mismatch between docs and actual menu/field names.
- Validator cannot reach the documented accepted state.
- Canonical Boot cannot reach the selected destination.
- A non-development Windows player cannot complete the canonical launch.
- Tarball removal/reinstall duplicates or corrupts project-owned content.
- Invited tester cannot complete the path without undocumented help.
- Package manifest version, tag, changelog, artifact, or checksum disagrees.

## Private licensing boundary

The existing all-rights-reserved development license remains in force. Invited
testers receive limited permission from the owner to install and test the
provided candidate. They do not receive permission to publish, redistribute,
relicense, or represent it as a public release.

## Exit

FL-M6-04 may close as `0.1.0-beta.1` only after every applicable private-beta
gate passes. After closeout, First Light stops feature work and the suite may
begin the next selected package’s just-in-time learning review. Optional First
Light adoption remains an M7 decision.
