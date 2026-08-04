# Plan Documentation

This folder is the living planning and architecture record for **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

The Markdown files are committed to Git and opened directly in Obsidian. Git is the durable history; Obsidian is the authoring and navigation surface. Do not maintain a second copied vault.

## Authority order

1. `Echo_Game_Systems_Suite_Bible.md`
2. Approved package specifications
3. Accepted ADRs and integration specifications
4. Approved standards, roadmaps, checkpoint plans, test reports, and release records
5. `Current Notes.md`

When documents disagree, stop and reconcile the higher authority. Current Notes never silently overrides approved architecture.

## Reading order

1. This README.
2. SFGSS-000.
3. `Current Notes.md`.
4. `Full_Suite_Documentation_Program_Roadmap.md`.
5. SFGSS-002 when dependency, bridge, provider, assembly, sample, or removal behavior is relevant.
6. SFGSS-003 when definitions, IDs, Unity GUIDs, serialization, versions, migrations, unknown data, transactions, or durable removal behavior is relevant.
7. SFGSS-004 when tests, validators, Laboratories, evidence, compatibility, defects, performance, or release gates are relevant.
8. The standard, package specification, ADR, integration record, or research record active for the checkpoint.
9. SFGSS-001 when drafting package specifications.
10. SFGSS-005 before implementation planning or code.
11. Relevant test/readiness reports and prior checkpoint closeout.

## Current approved objective

The ten Foundation specifications and Foundation collision/readiness records are approved. The owner has clarified that Expansion and Advanced package foundations are the next priority before the remaining general standards.

```text
Completed standards: SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, and SFGSS-004 v1.0.0
Active checkpoint: SUITE-DOC-05 — EchoFeedback: Impact Package Specification
Implementation: locked
First queued implementation: FL-M1-01 — First Light Package Skeleton
Final unlock gate: SUITE-DOC-33
```

Do not create package manifests, asmdefs, C# scripts, scenes, prefabs, ScriptableObjects, setup tools, samples, bridges, or provider adapters before SUITE-DOC-33.

## Learning-oriented implementation rule

When code is eventually authorized:

- Show complete compile-ready files in the conversation.
- State each exact file path and purpose.
- Explain the architecture, important code sections, Unity lifecycle, failure behavior, and chosen alternatives.
- Provide exact Editor setup and proof tests.
- Jesse enters the code himself by default.
- Stop at compile/test boundaries before continuing.

Generated files may supplement the visible code only when Jesse explicitly requests them.

## Repository structure

Create folders only when the first real document exists:

```text
Plan Documentation/
├── Package Specifications/
├── Architecture Decision Records/
├── Integration Specifications/
├── Checkpoint Build Plans/
├── Research Records/
├── Test Reports/
└── Release Records/
```

## Fresh ChatGPT handoff prompt

> We are continuing the documentation-first development of The Sperk’s Forge — EchoDevGames Game Systems Suite. Read README.md, SFGSS-000, Current Notes, Full_Suite_Documentation_Program_Roadmap.md, SFGSS-002, SFGSS-003, SFGSS-004, SFGSS-001 or SFGSS-005 as applicable, and every authority named by the active checkpoint. Package implementation is locked until SUITE-DOC-36. Current checkpoint: SUITE-DOC-05 — EchoFeedback: Impact Package Specification. Treat SFGSS-002 as the dependency/assembly authority, SFGSS-003 as the data/identity/migration authority, and SFGSS-004 as the test/evidence/release authority. Design package-selection pathways, decision trees, recommended combinations, exclusions, visible dependencies, save/settings choices, controller choices, and Workshop-facing guidance without turning a pathway into a hidden mandatory framework. When implementation eventually begins, show complete code and explain every step so Jesse can enter and understand it himself.

## Checkpoint rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Promote durable decisions into the correct authority.
3. Update roadmap, tests, research, setup, changelog, or release records as applicable.
4. Confirm documentation describes approved or observed truth accurately.
5. Commit and push the checkpoint before advancing when practical.
