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
6. The standard, package specification, ADR, integration record, or research record active for the checkpoint.
7. SFGSS-001 when drafting package specifications.
8. SFGSS-005 before implementation planning or code.
9. Relevant test/readiness reports and prior checkpoint closeout.

## Current approved objective

The ten Foundation specifications and Foundation collision/readiness records are approved. The owner has extended the documentation-first gate across the complete planned suite.

```text
Completed standard: SFGSS-002 v1.0.0 — Dependency, Bridge, and Assembly
Active checkpoint: SUITE-DOC-03 — SFGSS-003 Data, IDs, Serialization, and Migration Standard
Implementation: locked
First queued implementation: FL-M1-01 — First Light Package Skeleton
Final unlock gate: SUITE-DOC-36
```

Do not create package manifests, asmdefs, C# scripts, scenes, prefabs, ScriptableObjects, setup tools, samples, bridges, or provider adapters before SUITE-DOC-36.

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

> We are continuing the documentation-first development of The Sperk’s Forge — EchoDevGames Game Systems Suite. Read README.md, SFGSS-000, Current Notes, Full_Suite_Documentation_Program_Roadmap.md, SFGSS-001 or SFGSS-005 as applicable, and every authority named by the active checkpoint. Package implementation is locked until SUITE-DOC-36. Current checkpoint: SUITE-DOC-03 — SFGSS-003 Data, IDs, Serialization, and Migration Standard. Treat approved SFGSS-002 as the dependency/assembly authority. Before drafting, reconcile stable IDs, definitions versus state, serialization DTOs, unknown-data preservation, migrations, aliases, transactions, and removal behavior across all approved package specifications without inventing implementation evidence. When implementation eventually begins, show complete code and explain every step so Jesse can enter and understand it himself.

## Checkpoint rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Promote durable decisions into the correct authority.
3. Update roadmap, tests, research, setup, changelog, or release records as applicable.
4. Confirm documentation describes approved or observed truth accurately.
5. Commit and push the checkpoint before advancing when practical.
