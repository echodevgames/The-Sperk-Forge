# SFGSS-ADR-005 — Standalone Release Before Optional Adoption

**Document ID:** SFGSS-ADR-005
**Status:** Accepted
**Decision date:** August 7, 2026
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Decision scope:** Package release sequencing, clean-project proof, and existing-project adoption
**Related authorities:** SFGSS-000, SFGSS-001, SFGSS-004, SFGSS-006, SFGSS-009, SFGSS-ADR-004

> Prove the package on clean ground before asking an existing game to carry it.

## Context

The original suite paperwork mixed two different goals:

1. proving that a package is independently installable and usable; and
2. replacing or integrating a working system in an existing game.

Several milestone tables placed adoption before release. SFGSS-000 also said
packages should prove themselves in isolation first, while SFGSS-004 already
defined clean-project beta evidence independently from real-project adoption.
That disagreement made it appear that First Light had to enter Echo Systems
Lab before its own installation and setup documentation had been proven in a
new project.

Jesse corrected the intended order on August 7, 2026: finish the package
documentation, validate the candidate in a genuinely new project, conduct a
private test, close a private pre-release, and then move to the next standalone
package. No existing-project adoption was requested.

## Decision

1. Package-local implementation and Standalone Laboratory proof come first.
2. A clean-project candidate test must follow the written installation,
   setup, validation, Laboratory, removal, reinstall, and build instructions.
3. A package may enter an honestly labeled alpha or beta pre-release after its
   applicable SFGSS-004 gate passes, without first replacing a system in an
   existing game.
4. Passing a private pre-release gate is sufficient to close the package's
   current standalone development cycle and begin the next package's
   just-in-time learning review.
5. Existing-project adoption, project adapters, and optional bridges remain
   valuable later evidence. They are required before making the corresponding
   adoption, bridge, parity, or integration claim—not before every package
   pre-release.
6. A stable release requires real-project adoption only when the active package
   specification explicitly retains that requirement for the stable claim.
7. An Integration Laboratory is required when a bridge or provider artifact
   ships. It is not a substitute for the package's clean-project Standalone
   Laboratory.
8. Existing project systems remain intact until the later adoption path proves
   parity and rollback under PATH-110.
9. First Light M6 is redefined as documentation reconciliation, clean-project
   candidate validation, private tester handoff, and private beta closeout.
   Optional adoption moves to M7 and is not currently authorized.

## Consequences

### Positive

- Documentation and installation defects are found before a working game is
  exposed to the package.
- Each package can reach a bounded, testable pre-release and then stop cleanly.
- The suite may advance package by package without creating premature
  integration work.
- Later adoption evidence remains reversible and project-specific.

### Costs

- A private beta is not proof of real-project parity or stable public support.
- Integration evidence arrives later and must not be implied by standalone
  success.
- Living roadmaps and package milestone tables must distinguish release evidence
  from adoption evidence.

## First Light application

The approved remaining path is:

1. `FL-M6-01` — Documentation and Release-Plan Reconciliation.
2. `FL-M6-02` — Clean-Project Private-Beta Candidate Validation.
3. `FL-M6-03` — Private Tester Handoff and Findings.
4. `FL-M6-04` — Private Beta Closeout and Tag.
5. `M7` — Optional adoption or bridge work only after a separate decision.

No Echo Systems Lab, Rescuers2D, or Don’t Get Vince’d integration is authorized
by this ADR.

## Review trigger

Revisit if a package cannot be meaningfully tested outside a real production
environment, if a stable release explicitly requires adoption, or if later
private betas repeatedly fail because standalone proof omitted a necessary
project condition.

## Approval

**Decision:** Accepted
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 7, 2026

## Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard|SFGSS-004]]
- [[SFGSS-006_New-Project_Guided_Pathways|SFGSS-006]]
- [[Current Notes]]
