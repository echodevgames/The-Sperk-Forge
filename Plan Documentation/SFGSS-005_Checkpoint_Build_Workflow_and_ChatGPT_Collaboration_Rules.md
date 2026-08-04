# SFGSS-005 — Checkpoint Build Workflow and ChatGPT Collaboration Rules

**Document ID:** SFGSS-005  
**Version:** 1.2.0  
**Status:** Approved workflow standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 — The Sperk’s Forge Game Systems Suite Bible  
**Related standards:** SFGSS-001, SFGSS-ADR-002, SFGSS-ADR-003, approved package specifications, accepted ADRs, integration specifications, and repository `Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> Build one proven span at a time. The Forge remembers what changed, why it changed, how it was tested, and where to stop.

### Revision history

| Version | Date | Status | Summary |
|---|---|---|---|
| 1.0.0 | August 3, 2026 | Approved | Initial checkpoint workflow and collaboration rules |
| 1.1.0 | August 3, 2026 | Approved | Added manual-entry learning workflow, mandatory visible complete code, file-by-file explanations, and evidence-based teaching stops |
| 1.2.0 | August 4, 2026 | Approved | Added graph-roadmap navigation and mandatory package-by-package learning reviews before implementation authorization |


---

## 1. Purpose

This document defines the required planning, execution, validation, documentation, and handoff workflow for implementation checkpoints in **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

A Checkpoint Build Plan converts an approved package specification into one bounded, testable implementation outcome. It is not a loose task list, a feature wishlist, or permission to implement the remainder of a milestone. The checkpoint authorizes only the files, behavior, Editor work, tests, and documentation explicitly named by the plan.

The workflow exists to ensure that:

- Architecture is approved before code.
- Each checkpoint has one observable result and one explicit stop point.
- Package independence is tested continuously rather than assumed at release time.
- Working project systems remain available until replacement parity is proven.
- Code, Unity Editor setup, tests, and documentation describe the same committed state.
- A fresh collaborator or ChatGPT conversation can continue from repository evidence instead of reconstructing decisions from chat history.
- Jesse understands the purpose, boundaries, lifecycle, and practical use of each package before manually entering its implementation code.

---

## 2. Authority and applicability

This is a Level 4 workflow standard beneath:

1. SFGSS-000.
2. The active approved package specification.
3. Applicable accepted ADRs and integration specifications.

A Checkpoint Build Plan must not change a higher-authority contract silently. When implementation reveals a genuine architecture change:

1. Stop the affected implementation work.
2. Record the finding in `Current Notes.md`.
3. Update the owning package specification, SFGSS-000, or an ADR as appropriate.
4. Approve and commit the documentation change.
5. Revise the checkpoint before continuing.

This standard applies to:

- Package skeletons.
- Runtime and Editor implementation.
- Test Labs and samples.
- Setup, repair, migration, and validation tooling.
- Bridge and provider-adapter work.
- Existing-project adoption and parity work.
- Release preparation.
- Documentation-only gates that authorize later implementation.

---

## 3. Checkpoint principles

### 3.1 One verified outcome

A checkpoint must produce one primary outcome that can be demonstrated and tested. Supporting files are allowed only when they are necessary for that outcome.

Good examples:

- The package installs and exposes compile-safe assemblies, with no runtime behavior.
- One authority claims ownership and rejects a duplicate before side effects.
- One ordered sequence executes synchronous test steps and produces a report.
- One Test Lab proves one complete package loop in isolation.

Bad examples:

- “Build most of the audio system.”
- “Create all managers.”
- “Set up the entire package and polish it.”
- “Refactor anything related while we are here.”

### 3.2 Explicit exclusions

Every plan must list what is not authorized. Exclusions protect the package from scope drift and make the stop point enforceable.

### 3.3 Package independence first

Standalone behavior is tested before integration behavior. A bridge or showcase cannot substitute for the package’s independent proof.

### 3.4 Documentation is an implementation artifact

A checkpoint is incomplete when code works but the package specification, Current Notes, setup guide, test record, changelog, or status record is stale.

### 3.5 No hidden implementation authorization

A milestone name does not authorize all of its possible work. Only the active Checkpoint Build Plan authorizes implementation.

---

## 4. Checkpoint identity and naming

Use this pattern:

```text
<PACKAGE-CODE>-M<MILESTONE>-<SEQUENCE> — <OUTCOME>
```

Examples:

```text
FL-M1-01 — Package Skeleton
FL-M2-01 — Authority Claim and Static Reset
JB-M2-01 — Two-Source Music Transport Core
EUI-M3-02 — Modal Result Lifecycle
```

Documentation-wide gates may use the suite checkpoint ID already established by their roadmap:

```text
FW-DOC-12 — Documentation Readiness Gate
```

Every Checkpoint Build Plan records:

- Document ID.
- Version.
- Status.
- Package and package-specification version.
- Milestone.
- Current Unity baseline.
- Repository/workspace.
- Owner.
- Last updated date.

---

## 5. Required repository scan before planning

Before drafting or executing a checkpoint, read in this order:

1. Repository README/documentation index.
2. SFGSS-000 when suite boundaries are relevant.
3. The active package specification.
4. Applicable ADRs and integration specifications.
5. SFGSS-005.
6. `Current Notes.md`.
7. The current roadmap/status record.
8. The previous checkpoint closeout and test report.
9. Relevant implementation, manifests, scenes, prefabs, assets, and automated tests.

The plan must list the exact authority set used. Chat history may supply context but is not a substitute for these repository sources.

---


## 5A. Pre-implementation package learning review

Before the final documentation readiness gate may authorize implementation, every package in SFGSS-000 Sections 7.1 through 7.3 receives an individual learning review.

Each review must cover:

1. Plain-English purpose.
2. A real-world analogy.
3. One practical game application.
4. What the package owns and explicitly refuses to own.
5. Definition/configuration versus mutable runtime state.
6. Lifecycle and failure behavior.
7. Important public concepts without requiring memorization of the whole API.
8. Optional bridges and which authority remains in control.
9. The package's Standalone Laboratory.
10. A teach-back check in which Jesse explains the package in his own words.

The review follows `Package_Learning_Review_Catalog.md` and SFGSS-ADR-003. It is documentation and education, not implementation authorization. Tiny pseudocode or diagrams may illustrate a concept, but complete production code waits for an approved Checkpoint Build Plan.

The suite maintains `Suite_Graph_Roadmap.md` as the navigation hub for these reviews. Every current package specification must link back to that graph note.

## 6. Required Checkpoint Build Plan structure

Every implementation plan uses the following sections. A section may say **Not applicable**, but it must explain why.

### 6.1 Document control

Record identity, status, version, authorities, repository, Unity baseline, and approval.

### 6.2 Purpose and observable outcome

State what a user, developer, or tester can observe when the checkpoint is complete.

### 6.3 Starting conditions

List the required repository state, package version, prior checkpoints, Unity packages, clean compile condition, and known blockers.

### 6.4 Authority and constraints

Summarize the package ownership boundary, independence rules, relevant decisions, and any limits inherited from ADRs or integration specifications.

### 6.5 Scope

List the exact behavior and artifacts authorized.

### 6.6 Explicit exclusions

List adjacent work that remains forbidden during this checkpoint.

### 6.7 Files and assets

Provide one table with every file or asset to create, modify, move, or delete. Include ownership and purpose. Do not use “and related files” as a substitute for a complete manifest.

### 6.8 Implementation sequence

Give exact steps in dependency order. Separate code/file operations from Unity Editor operations.

### 6.9 Visible code and learning rule

When a checkpoint authorizes scripts, the default delivery is **manual, visible, and educational**:

1. Show the complete compile-ready contents of every new or changed code file in the conversation.
2. State the exact repository path and whether the file is created, replaced, or modified.
3. Explain the file's responsibility, authority boundary, collaborators, and reason for existing before presenting it.
4. After the code, explain the important fields, methods, events, lifecycle callbacks, async/cancellation rules, failure paths, and extension seams.
5. Explain why the selected design was chosen and briefly identify the most relevant rejected alternative when that choice is not obvious.
6. Provide the exact Unity Editor setup and the expected Inspector or runtime state.
7. Stop at a compile/test boundary so Jesse can enter the code, observe the result, and understand the evidence before the next step.

Complete code means no ellipses, hidden edits, omitted helper methods, or fragments that rely on unstated changes. Pseudocode may be used to teach a concept, but it never substitutes for the authorized compile-ready file.

Jesse implements the code himself by default. Generated source files or direct repository edits may supplement the visible code only when he explicitly asks for them; they never replace showing and explaining the code.

Do not create scripts merely to hold an empty namespace when an assembly definition already proves the skeleton.

### 6.10 Unity Editor setup

Provide exact menu paths, object names, components, serialized assignments, asset locations, Build Settings or Build Profile changes, and expected Inspector state.

### 6.11 Validation and tests

List test IDs, setup, action, expected result, evidence, and whether each test is automated or manual.

### 6.12 Failure symptoms and fixes

Document likely compile, package, scene, serialization, lifecycle, and setup failures. Fixes must remain inside the checkpoint scope.

### 6.13 Rollback and recovery

Explain how to return to the starting state without deleting project-owned content or relying on memory.

### 6.14 Documentation reconciliation

Name every document that must change at closeout, including Current Notes, status, test report, changelog, setup guides, specification, or ADR.

### 6.15 Commit and push plan

State the expected commit boundary and suggested commit message. The assistant never claims a commit or push occurred without evidence from the user or repository tooling.

### 6.16 Completion criteria

Use measurable checkboxes. “Looks good” is not a criterion.

### 6.17 Stop point

State the first tempting next action that is not allowed yet.

### 6.18 Next recommended checkpoint

Name the next checkpoint without authorizing it.

### 6.19 Handoff record

Provide the status fields needed by a fresh conversation.

### 6.20 Approval

Record `Approve`, `Revise`, or `Deferred`, approver, date, and conditions.

---

## 7. Scope and file-control rules

- Create only files listed in the plan.
- Preserve committed `.meta` files and GUIDs after Unity creates them.
- Do not add empty directories solely to match a future anatomy diagram.
- Do not move public assets or scripts unless the checkpoint explicitly covers migration and GUID preservation.
- Do not create a shared utility, core, or manager merely to reduce a few lines.
- Do not add peer-package references to a standalone core.
- Do not mix project-specific code into package runtime assemblies.
- Do not modify unrelated scenes, prefabs, settings, manifests, or packages.
- Generated or setup-created project assets remain project-owned.
- Destructive work requires preview, explicit approval, backup where practical, and rollback instructions.

---

## 8. Runtime and Editor implementation rules

### 8.1 Authority before side effects

Persistent authorities must claim or reject ownership before subscriptions, object creation, file access, mixer changes, input enablement, scene work, UI focus changes, or asynchronous operations.

### 8.2 Definitions and runtime state

Keep immutable definitions/configuration separate from mutable session state. Shared ScriptableObjects must not become live player or service state.

### 8.3 Public API discipline

- Prefer narrow interfaces and structured request/result types.
- Return meaningful failure results.
- Raise events after authoritative state changes.
- Keep convenience access from becoming the only test path.
- Include cancellation, timeout, teardown, and re-entry policy for asynchronous work.

### 8.4 Editor isolation

Runtime assemblies must not reference `UnityEditor`. Setup, validation, migration, simulation, and Workshop facades remain in Editor assemblies.

### 8.5 No speculative expansion

Deferred features stay deferred even when they appear easy during implementation.

---

## 9. Unity Editor workflow rules

- State the supported Unity version and exact package versions observed during execution.
- Record whether Enter Play Mode Options or domain reload settings affect the checkpoint.
- Use project-relative paths in documentation and reports.
- Name scenes, GameObjects, assets, and components exactly.
- Validate Build Settings and active Build Profiles when scene behavior is involved.
- Keep direct-scene helpers development-only by default.
- Run setup/repair operations repeatedly to prove idempotency.
- Never depend on a sample asset from `Samples~` in runtime assemblies.

---

## 10. Testing and evidence rules

A checkpoint test record distinguishes:

- **Not run** — defined but not executed.
- **Pass** — expected result observed with evidence.
- **Pass with advisory** — core requirement passed; a non-blocking limitation is recorded.
- **Fail** — expected result not met.
- **Blocked** — test could not run because a named prerequisite is missing.

Evidence may include:

- Automated test output.
- Unity Console result.
- Package Manager state.
- Inspector or scene state.
- Generated report.
- Clean-project import result.
- Reproduction steps and observed behavior.
- Commit hash supplied by the user or repository tooling.

A package checkpoint does not pass merely because it compiles. It must pass the checkpoint-specific behavior and independence tests.

---

## 11. Failure handling and rollback

When a failure appears:

1. Stop expanding scope.
2. Reproduce it with the smallest checkpoint-owned setup.
3. Record it in Current Notes with a stable issue/test reference.
4. Decide whether it is an implementation defect, setup defect, documentation defect, or architecture conflict.
5. Fix only checkpoint-owned defects.
6. Escalate architecture conflicts to the owning specification or ADR.
7. Re-run the entire checkpoint acceptance set after the fix.

Rollback must identify:

- Git files to restore.
- Unity-generated files or `.meta` files affected.
- Manifest or lockfile changes.
- Scene, prefab, asset, and project-setting changes.
- Project-owned data that must be preserved.

---

## 12. Current Notes and documentation closeout

During implementation, record provisional observations in `Current Notes.md` using the approved note labels.

At closeout:

1. Review every note added during the checkpoint.
2. Promote durable architecture or behavior into the package specification or an ADR.
3. Move test evidence into a test report.
4. Move defects into the issue record.
5. Update setup, troubleshooting, API, migration, and known-limitations documentation as needed.
6. Update the package changelog for user-visible or API changes.
7. Update the current status and next checkpoint.
8. Condense resolved notes after promotion.
9. Confirm documentation describes the committed implementation exactly.

The closeout document must not mark commit/push complete until the user confirms it or repository tooling provides evidence.

---

## 13. Git checkpoint rules

Preferred commit boundaries:

- One implementation checkpoint in one commit when practical.
- One immediately adjacent documentation/test commit when code and documentation cannot reasonably be committed together.
- A separate architecture commit when an ADR or higher-authority revision must precede implementation.

Suggested commit format:

```text
<package>: complete <checkpoint-id> <outcome>
```

Examples:

```text
echo-launch: complete FL-M1-01 package skeleton
echo-launch: document FL-M2-01 authority claim results
sfgss: approve FW-DOC-12 implementation readiness gate
```

Do not rewrite or discard unrelated working-tree changes. Do not claim remote state without confirmation.

---

## 14. ChatGPT collaboration rules

Before writing code, ChatGPT must:

1. Summarize the relevant package authority and exclusions.
2. State the active checkpoint and stop point.
3. Identify conflicts or missing decisions that materially affect the work.
4. Use the repository sources rather than relying only on chat memory.
5. Keep optional integrations behind bridges or project adapters.
6. Preserve working project systems until parity is proven.

During work, ChatGPT must:

- Provide concise progress updates during multi-step work.
- Surface discovered blockers early.
- Avoid inventing architecture merely to keep moving.
- Provide exact Unity Editor setup when required.
- Keep code compile-ready and within checkpoint scope.
- Show every authorized code file completely in the chat before or alongside any generated artifact.
- Teach the implementation in dependency order: purpose, architecture, complete file, important sections, Editor setup, expected behavior, and proof test.
- Define unfamiliar Unity/C# terms at first use and connect them to the package's ownership model.
- Use bounded learning stops so Jesse can implement and compile the current span before receiving the next span.
- Distinguish source-derived decisions from proposals or external research.
- Avoid claiming tests, commits, pushes, or Unity behavior were observed when they were not.

At closeout, ChatGPT must:

- Report what was created or changed.
- List tests actually run and tests still pending.
- Reconcile documentation artifacts.
- State the stop point and next checkpoint.
- Provide downloadable artifacts when files were generated outside the user’s repository.

---

## 15. Checkpoint execution loop

```text
Scan authorities
    ↓
Confirm starting state
    ↓
Approve bounded plan
    ↓
Create only listed artifacts
    ↓
Perform exact Editor setup
    ↓
Run acceptance tests
    ↓
Fix checkpoint-owned defects
    ↓
Re-run acceptance tests
    ↓
Reconcile Current Notes and documentation
    ↓
Commit and push
    ↓
Record handoff and stop
```

A failed gate returns to the smallest earlier step that owns the failure. It does not authorize unrelated work.

---

## 16. Checkpoint status record

Use this compact record at checkpoint closeout:

| Field | Value |
|---|---|
| Package | `<PACKAGE>` |
| Package version | `<VERSION>` |
| Specification version | `<VERSION>` |
| Checkpoint | `<ID AND NAME>` |
| Outcome | `<COMPLETE / PARTIAL / FAILED / BLOCKED>` |
| Files/assets created | `<LIST>` |
| Files/assets modified | `<LIST>` |
| Tests passed | `<LIST>` |
| Tests failed/blocked | `<LIST>` |
| Known issues | `<LIST>` |
| Decisions/ADRs added | `<LIST>` |
| Documentation reconciled | `<LIST>` |
| Commit/push evidence | `<HASH OR USER CONFIRMATION / PENDING>` |
| Next checkpoint | `<ID>` |

---

## 17. New-conversation handoff prompt

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the suite-wide authority, the approved <PACKAGE> Specification
as the package authority, applicable ADRs/integration specifications as Level 3
authority, and SFGSS-005 as the checkpoint workflow authority.

Current package: <PACKAGE>
Current specification: <VERSION>
Current checkpoint: <ID AND NAME>
Current Unity version: <VERSION>
Current repository/workspace: <PATH OR NAME>
Current implementation status: <STATUS>
Known blockers: <BLOCKERS>
Current Notes reviewed through: <DATE/COMMIT>

Before writing code:
1. Summarize the package authority and exclusions.
2. Confirm the checkpoint scope and stop point.
3. Identify any conflict that requires documentation approval first.
4. Preserve package independence and existing working systems.
5. Follow the approved Checkpoint Build Plan exactly.
6. Reconcile tests and documentation at closeout.
```

---

## 18. Prohibited checkpoint anti-patterns

- One checkpoint implements multiple milestones.
- A manager accumulates unrelated responsibilities because it is convenient.
- An optional bridge is compiled into a standalone core.
- A sample becomes a runtime dependency.
- A ScriptableObject stores mutable session state.
- A setup tool overwrites project-owned content silently.
- A public API is changed in code before its specification or ADR.
- A test is marked passed because the code “should work.”
- A commit is reported without evidence.
- Current Notes becomes the only home of a durable decision.
- The next checkpoint begins before the current stop point is documented and committed.

---

## 19. Workflow approval checklist

- [x] Authority order is explicit.
- [x] Checkpoint identity and required sections are defined.
- [x] Scope, file, code, and Editor setup rules are defined.
- [x] Testing and evidence states are defined.
- [x] Rollback and documentation reconciliation are required.
- [x] Git claims require evidence.
- [x] ChatGPT collaboration and handoff rules are defined.
- [x] The workflow preserves package independence and design-before-implementation.

---

## 20. Approval

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 3, 2026  
**Conditions:** SFGSS-005 governs implementation after the Full Suite Documentation Readiness Gate. A package specification or ADR remains required for architecture changes; a Checkpoint Build Plan cannot overrule higher authority. Code delivery follows the visible, complete, manual-entry learning workflow unless Jesse explicitly requests another delivery method.


---

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
