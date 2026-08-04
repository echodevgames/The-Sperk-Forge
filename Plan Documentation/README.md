# Plan Documentation

This folder is the living planning and architecture record for **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

The Markdown files are committed to Git and opened directly in Obsidian. Git is the durable history; Obsidian is the authoring, navigation, and Graph View surface. Do not maintain a second copied vault.

## Start here

1. [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
2. [[Suite_Health_Check_and_Remaining_Documentation|Suite Health Check]]
3. [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
4. [[Current Notes]]
5. [[Full_Suite_Documentation_Program_Roadmap|Full Suite Documentation Program Roadmap]]
6. The active package, integration, research, or standard documents

## Authority order

1. `Echo_Game_Systems_Suite_Bible.md`
2. Approved package specifications
3. Accepted ADRs and integration specifications
4. Approved standards, roadmaps, checkpoint plans, test reports, and release records
5. `Current Notes.md`

The graph roadmap is navigation, not authority.

## Current approved state

```text
SFGSS-000: v0.16.0
SFGSS-005: v1.2.0
SFGSS-006: v1.0.0
SFGSS-007: v1.0.0
Foundation specifications: 10 of 10 approved
Expansion specifications: 13 of 13 approved
Advanced foundations: 5 of 5 approved
Foundation collision matrix: approved
Expansion collision matrix: approved
Advanced collision matrix: approved
Graph roadmap: active
Package learning reviews: 0 of 28 complete
Active checkpoint: SUITE-DOC-27 - SFGSS-008 Suite Glossary and Naming Registry
Implementation: locked
First queued implementation: FL-M1-01 — First Light Package Skeleton
Final unlock gate: SUITE-DOC-33, after documentation and learning reviews
```

Do not create package manifests, asmdefs, C# scripts, scenes, prefabs, ScriptableObjects, setup tools, samples, bridges, provider adapters, or executable prototypes before SUITE-DOC-33.

## Important integration and workflow authorities

- [[Integration Specifications/Foundation_Cross-Package_Contract_Matrix|Foundation Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-ADVANCED-001_Advanced_Cross-Package_and_Research_Contract_Matrix|Advanced Cross-Package and Research Matrix]]
- [[Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|ADR-001 Setup Facade Protocol]]
- [[Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation|ADR-002 Documentation Gate]]
- [[Architecture Decision Records/SFGSS-ADR-003_Graph_Roadmap_and_Pre-Implementation_Learning_Review|ADR-003 Graph and Learning Review]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 Checkpoint and Learning Workflow]]
- [[SFGSS-006_New-Project_Guided_Pathways|SFGSS-006 New-Project Guided Pathways]]
- [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007 ADR Standard]]
- [[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]

## Learning-oriented implementation rule

When code is eventually authorized:

- Show complete compile-ready files in the conversation.
- State every exact file path and purpose.
- Explain architecture, important code sections, Unity lifecycle, failure behavior, and rejected alternatives.
- Provide exact Editor setup and proof tests.
- Jesse enters the code himself by default.
- Stop at compile/test boundaries before continuing.

Before implementation, every package receives a plain-language learning review with an analogy, practical example, ownership boundary, lifecycle, bridges, and teach-back check.

## Repository structure

```text
Plan Documentation/
├── Package Specifications/
├── Architecture Decision Records/
├── Integration Specifications/
├── Checkpoint Build Plans/
├── Research Records/
├── Test Reports/
├── Release Records/
├── Suite_Graph_Roadmap.md
├── Suite_Health_Check_and_Remaining_Documentation.md
└── Package_Learning_Review_Catalog.md
```

## Fresh ChatGPT handoff prompt

> We are continuing documentation-first development of The Sperk’s Forge — EchoDevGames Game Systems Suite. Begin with Suite_Graph_Roadmap.md, README.md, SFGSS-000, Current Notes, the Full Suite Documentation Program Roadmap, SFGSS-001 through SFGSS-007, all approved package specifications, ADR-001 through ADR-003, and the Foundation, Expansion, and Advanced cross-package matrices. Package implementation is locked until SUITE-DOC-33 and all 28 package learning reviews are complete. Current checkpoint: SUITE-DOC-27 - SFGSS-008 Suite Glossary and Naming Registry. Keep every unexecuted result `Not run`. When implementation eventually begins, show complete code and explain every step so Jesse can enter and understand it himself.

## Checkpoint rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Update `Suite_Graph_Roadmap.md`.
3. Promote durable decisions into the correct authority.
4. Update roadmap, tests, research, setup, changelog, or release records as applicable.
5. Confirm documentation describes approved or observed truth accurately.
6. Commit and push the checkpoint before advancing when practical.
