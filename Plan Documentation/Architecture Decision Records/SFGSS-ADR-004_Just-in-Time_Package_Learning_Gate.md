# SFGSS-ADR-004 — Just-in-Time Package Learning Gate

**Document ID:** SFGSS-ADR-004  
**Status:** Accepted  
**Decision date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Decision scope:** Package learning sequence and implementation authorization  
**Related authorities:** SFGSS-000, SFGSS-005, SFGSS-ADR-002, SFGSS-ADR-003, Full Suite Documentation Program Roadmap

> Learn the tool when it is about to enter the hand.

## Context

The full suite documentation program and its twenty-eight package foundations are complete. The original learning gate required all twenty-eight package reviews before any implementation could begin. After completing the First Light review and beginning The Observatory, Jesse determined that reviewing many packages long before their implementation would reduce retention and separate the lesson from its practical application.

The learning purpose remains unchanged: Jesse must understand each package before manually entering its code. The sequencing changes from one large pre-code curriculum to a package-local, just-in-time gate.

## Decision

1. Every package retains its required `PKG-LEARN-###` review.
2. A package's learning review must be complete before that package's first implementation checkpoint may be authorized.
3. Reviews occur immediately before the related package implementation by default, not as one twenty-eight-package block.
4. Completing one package review does not unlock another package.
5. The initial implementation readiness gate may authorize First Light because `PKG-LEARN-001` is complete and the suite documentation program has passed.
6. The partially started Observatory review is paused and resumes before EchoDiagnostics implementation. It is not marked complete.
7. A completed review must be revisited before implementation when the package authority changed materially after the review.
8. The graph roadmap, catalog, and tracker remain the navigation and status system for all twenty-eight reviews.
9. Learning completion remains educational evidence only. It does not promote implementation tests, compatibility, performance, or release evidence beyond `Not run`.
10. Visible complete code, manual entry, file-by-file explanation, Editor setup, validation, and stop points remain mandatory under SFGSS-005.

## Supersession

This ADR supersedes only:

- SFGSS-ADR-003 decisions 6 and 7 to the extent they required all twenty-eight learning reviews before any code.
- The corresponding all-reviews-before-implementation sequencing in SFGSS-ADR-002, SFGSS-005 v1.3.0, the roadmap, tracker, health check, graph, and handoff guide.

SFGSS-ADR-003 remains accepted for the Graph Roadmap, package navigation, progressive teaching format, teach-back requirement, and the existence of all twenty-eight learning reviews.

## Consequences

### Positive

- Learning occurs close to practical use, improving retention.
- Each implementation checkpoint begins with a refreshed mental model.
- First Light can proceed after the suite gate without forcing unrelated package lessons first.
- Package authority changes can be taught at the moment they matter.

### Costs

- The learning phase and implementation phase interleave.
- Roadmaps and trackers must distinguish suite readiness from package-local readiness.
- A package cannot be implemented impulsively; its learning review remains a hard local gate.

## Review trigger

Revisit if just-in-time reviews repeatedly interrupt implementation momentum, if a multi-package checkpoint requires several package reviews together, or if Jesse prefers a different teaching cadence.

## Approval

**Decision:** Accepted  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026

## Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
