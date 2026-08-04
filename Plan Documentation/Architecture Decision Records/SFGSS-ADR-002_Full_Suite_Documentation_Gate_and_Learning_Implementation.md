# SFGSS-ADR-002 — Full Suite Documentation Gate and Learning-Oriented Implementation

**Document ID:** SFGSS-ADR-002  
**Status:** Accepted  
**Decision date:** August 3, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Parent authority:** SFGSS-000 v0.9.0  
**Related documents:** SFGSS-005 v1.1.0, `Full_Suite_Documentation_Program_Roadmap.md`, FW-DOC-12 readiness report, FL-M1-01 Checkpoint Build Plan

> Finish the map, then learn every road while building it.

---

## 1. Context

FW-DOC-12 proved that the Foundation package set was documented well enough to begin a tiny package-skeleton checkpoint. While reviewing that work, Jesse found the documentation process itself valuable: First Light exposed architecture choices he had not previously known to ask about, and resolving those choices before code improved both confidence and understanding.

Jesse therefore chose to extend the documentation-first method beyond the Foundation Wave. He also established that future code should not arrive as an opaque artifact. He wants to enter the code himself, see the complete files, and understand what each step does and why the architecture was chosen.

## 2. Decision

1. Package implementation is re-locked until the complete pre-code documentation program defined by `Full_Suite_Documentation_Program_Roadmap.md` passes its final readiness gate.
2. The ten Foundation package specifications, Foundation matrix, ADR-001, SFGSS-005, and FW-DOC-12 remain approved. Their work is not discarded.
3. FL-M1-01 remains the first queued implementation checkpoint, but its status changes from active to dormant.
4. The pre-code gate covers the remaining SFGSS architecture/workflow standards, all Expansion package specifications, Crafting design and specification work, Multiplayer research/contract planning, Advanced feasibility specifications, and cross-suite collision reviews.
5. Documentation must remain evidence-honest. Planned architecture and acceptance contracts may be approved before code. Test results, screenshots, performance measurements, verified compatibility, release notes, migration evidence, and prototype-dependent findings may not be fabricated to make the documentation appear complete.
6. When implementation begins, ChatGPT shows complete compile-ready code in the conversation and explains it in dependency order. Jesse enters the code himself by default unless he explicitly asks for generated source files or direct editing.
7. Each implementation span includes: purpose, authority boundary, why the design was chosen, complete file contents, explanation of important sections, exact Unity Editor setup, expected behavior, validation tests, common failures, and a stop point.

## 3. Consequences

### 3.1 Benefits

- More cross-package conflicts are discovered while changes are inexpensive.
- Future conversations can rebuild context from the repository rather than old chat history.
- Jesse gains a systems-level understanding instead of merely receiving working code.
- Implementation checkpoints become smaller and easier to verify because the contracts already exist.
- Expansion and Advanced packages cannot quietly distort Foundation authorities later.

### 3.2 Costs

- Runtime implementation begins later.
- Some specifications may require revision when empirical implementation evidence arrives.
- The documentation program is large and must be kept bounded by SFGSS-000 Section 18.
- Multiplayer provider approval cannot honestly finish before disposable comparison prototypes; the documentation gate can complete the research plan, source-based matrix, neutral contracts, and approval criteria, but not claim prototype results that do not exist.

### 3.3 Supersession

This ADR supersedes only the **immediate activation** portion of SFGSS-000 decision 40 and the FL-M1-01 active-status language. It does not invalidate the Foundation readiness result or the FL-M1-01 plan.

## 4. Documentation completeness boundary

| Complete before implementation | Remains evidence-pending |
|---|---|
| Authority and non-goals | Actual compile/test results |
| Public APIs and data models | Screenshots from implemented tools |
| Lifecycle, failure, cancellation, and teardown contracts | Measured performance and allocations |
| Dependency, bridge, assembly, and removal rules | Verified Unity/package version matrix |
| Setup and repair designs | Actual migration/upgrade evidence |
| Test registries and acceptance criteria | Release notes for behavior not built |
| Research plans and source-based comparisons | Prototype-dependent provider findings |
| Migration policies and versioning rules | Production support incidents and fixes |

Templates and `Not run` records may be prepared for evidence-pending documents, but they must not be presented as completed evidence.

## 5. Alternatives considered

### Begin FL-M1-01 immediately

Rejected for now. It was safe, but it would interrupt a documentation workflow that is producing valuable architectural learning and cross-package clarity.

### Document only packages immediately needed by First Light

Rejected. Later packages such as Inventory, Objectives, Controllers, Crafting, and Multiplayer can introduce contracts that affect IDs, persistence, input, UI, and bridge standards.

### Generate code files without showing them

Rejected as the default. It reduces Jesse's opportunity to learn and makes repository changes harder to audit mentally.

### Require every possible release artifact before code

Rejected as dishonest. Some documents require observed implementation evidence and cannot be truthfully completed in advance.

## 6. Exit criteria

Implementation may be unlocked only when:

- The Full Suite Documentation Program Roadmap is complete.
- Remaining SFGSS standards are approved.
- Expansion package specifications are approved.
- Advanced design/research records reach their honest pre-code completion state.
- Final full-suite authority, dependency, persistence, diagnostics, Test Lab, and removal reviews pass.
- Current Notes, README, roadmaps, and SFGSS-000 agree.
- A final Full Suite Documentation Readiness Report explicitly reactivates FL-M1-01 or selects another first implementation checkpoint.

## 7. Approval

**Decision:** Accepted  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 3, 2026
