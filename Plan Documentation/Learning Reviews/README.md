---
tags:
  - sfgss/learning
  - sfgss/navigation
status: active
updated: 2026-08-04
---

# Package Learning Reviews

**Document role:** Learning-phase index and operating guide  
**Authority:** Navigation and educational workflow only  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Current progress:** 1 of 28 complete  
**Completed:** PKG-LEARN-001 – First Light (`EchoLaunch`)
**Active review:** None; PKG-LEARN-002 is paused until EchoDiagnostics implementation

## Purpose

This folder holds one educational review for each package authority before implementation begins. The reviews translate large architectural specifications into a practical mental model without replacing those specifications.

## Review sequence

The permanent IDs are `PKG-LEARN-001` through `PKG-LEARN-028`. IDs follow the approved learning order, not separate wave-local numbering.

## Required source set

Each review records the exact sources used. At minimum:

1. The package specification or foundation.
2. SFGSS-000.
3. SFGSS-005.
4. The Full Suite Matrix entries involving the package.
5. Applicable wave matrix, ADRs, standards, and research records.
6. Current Notes and the package-learning catalog.

## Required review layers

1. Plain-English purpose.
2. Real-world analogy.
3. Practical game example.
4. Owns and does not own.
5. Definitions/configuration versus mutable runtime state.
6. Lifecycle and failure behavior.
7. Important public concepts.
8. Optional bridges and commit authority.
9. Standalone Laboratory proof.
10. Jesse’s teach-back and remaining questions.

## File rule

Completed and in-progress records use:

```text
PKG-LEARN-###_<TechnicalIdentifier>_Learning_Review.md
```

Create a review file from `PKG-LEARN-TEMPLATE.md`. Do not create twenty-eight empty review files in advance.

## Status sources

- Human-readable catalog: [[../Package_Learning_Review_Catalog|Package Learning Review Catalog]]
- Machine-readable tracker: `PKG-LEARN-TRACKER.json`
- Current active handoff: [[../Current Notes|Current Notes]]

## Stop rule

A review may use diagrams, examples, and tiny pseudocode for teaching. It must not authorize or supply production implementation code.


## Current gate mode

- PKG-LEARN-001 is complete.
- PKG-LEARN-002 is paused until EchoDiagnostics implementation approaches.
- SUITE-DOC-33 may activate First Light only; other packages remain locally locked.
