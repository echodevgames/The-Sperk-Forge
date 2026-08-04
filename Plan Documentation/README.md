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
5. SFGSS-002 through SFGSS-005.
6. The package specifications and research records active for the checkpoint.
7. Applicable ADRs and integration specifications.
8. Relevant audit reports and prior closeouts.
9. SFGSS-001 when drafting or revising a package specification.

## Current approved state

```text
SFGSS-000: v0.13.0
Foundation specifications: 10 of 10 approved
Expansion specifications: 13 of 13 approved
Advanced foundations: 5 of 5 approved
Foundation collision matrix: approved
Expansion collision matrix: SFGSS-INT-EXPANSION-001 v1.0.0 approved
ADR-001 setup-facade protocol: v1.1.0
Active checkpoint: SUITE-DOC-24 — Advanced Cross-Package and Research Review
Implementation: locked
First queued implementation: FL-M1-01 — First Light Package Skeleton
Final unlock gate: SUITE-DOC-33
```

Do not create package manifests, asmdefs, C# scripts, scenes, prefabs, ScriptableObjects, setup tools, samples, bridges, provider adapters, or executable prototypes before SUITE-DOC-33.

## Important integration authorities

- `Integration Specifications/Foundation_Cross-Package_Contract_Matrix.md`
- `Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix.md`
- `Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol.md`
- `Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation.md`

## Learning-oriented implementation rule

When code is eventually authorized:

- Show complete compile-ready files in the conversation.
- State every exact file path and purpose.
- Explain architecture, important code sections, Unity lifecycle, failure behavior, and rejected alternatives.
- Provide exact Editor setup and proof tests.
- Jesse enters the code himself by default.
- Stop at compile/test boundaries before continuing.

Generated files may supplement the visible code only when Jesse explicitly requests them.

## Repository structure

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

Create folders only when the first real document exists.

## Fresh ChatGPT handoff prompt

> We are continuing documentation-first development of The Sperk’s Forge — EchoDevGames Game Systems Suite. Read README.md, SFGSS-000, Current Notes, the Full Suite Documentation Program Roadmap, SFGSS-001 through SFGSS-005, all approved package specifications, ADR-001/ADR-002, the Foundation and Expansion cross-package matrices, and the Advanced research records relevant to the checkpoint. Package implementation is locked until SUITE-DOC-33. Current checkpoint: SUITE-DOC-24 — Advanced Cross-Package and Research Review. Reconcile The Convergence, Instinct, Clash, Arcana, and The Atlas against Foundation and Expansion authorities without selecting a networking provider or inventing prototype evidence. Keep every unexecuted result `Not run`. When implementation eventually begins, show complete code and explain every step so Jesse can enter and understand it himself.

## Checkpoint rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Promote durable decisions into the correct authority.
3. Update roadmap, tests, research, setup, changelog, or release records as applicable.
4. Confirm documentation describes approved or observed truth accurately.
5. Commit and push the checkpoint before advancing when practical.
