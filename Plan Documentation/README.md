# Plan Documentation

This folder is the living planning and architecture record for **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

The Markdown files in this folder are committed to Git and opened directly in Obsidian. Do not maintain a second copied vault. Git preserves the documentation history; Obsidian is the editing and navigation surface.

## Initial Source Set

The repository begins with exactly these four files:

1. `README.md` — this index and handoff guide.
2. `Echo_Game_Systems_Suite_Bible.md` — SFGSS-000, the approved suite-wide source of truth.
3. `SFGSS-001_Package_Specification_Template.md` — the required structure for every package specification.
4. `Current Notes.md` — the active working notes, checkpoint status, questions, test evidence, and handoff context.

Do not add old ChatGPT transcripts, superseded drafts, temporary exports, or duplicate copies of these files to the repository.

## Document Authority

When documents disagree, use this authority order:

1. `Echo_Game_Systems_Suite_Bible.md`
2. An approved individual package specification
3. An accepted Architecture Decision Record or integration specification
4. A checkpoint build plan, setup guide, test plan, or release record
5. `Current Notes.md`

`Current Notes.md` provides the freshest working context but does not silently override approved architecture. Durable decisions captured there must be promoted into the authoritative document that owns them.

## Reading Order for a New Work Session

1. Read this `README.md`.
2. Read `Echo_Game_Systems_Suite_Bible.md` completely.
3. Read `Current Notes.md` completely.
4. Read `SFGSS-001_Package_Specification_Template.md` before drafting or changing a package specification.
5. Read the active package specification and checkpoint plan once those files exist.

## First Approved Objective

The active design objective is the **Foundation Specification Pass**. Draft and approve a complete SFGSS-001 package specification for every package in SFGSS-000 Section 7.1 before beginning Foundation Wave runtime implementation.

The ten required specifications are:

1. First Light (`EchoLaunch`)
2. The Observatory (`EchoDiagnostics`)
3. The Accord (`EchoSettings`)
4. The Passage (`EchoSceneFlow`)
5. The Pulse (`EchoGameState`)
6. Resonance (`Jukebot`)
7. The Will (`EchoInput`)
8. The Looking Glass (`EchoUI`)
9. The Chronicle (`EchoSave`)
10. The Workshop (`EchoGameStarter`)

First Light v1.0.0, The Observatory v1.0.0, and The Accord v1.0.0 are approved package specifications. The Passage (`EchoSceneFlow`) is next. After all ten are approved, run the cross-package consistency review before creating runtime package code.

## Folders Added Later

Create subfolders only when the first real file for that category exists:

```text
Plan Documentation/
├── Package Specifications/
├── Architecture Decision Records/
├── Checkpoint Build Plans/
├── Test Reports/
└── Release Records/
```

Do not add empty placeholder folders solely for appearance; Git does not preserve empty directories.

## Fresh ChatGPT Handoff Prompt

Upload the four initial source files and begin the new conversation with:

> We are beginning development of The Sperk’s Forge — EchoDevGames Game Systems Suite in a clean Unity repository. Read every uploaded Markdown file completely. Treat SFGSS-000 as the suite-wide authority, treat Current Notes as working context rather than authority, and follow SFGSS-001 for all package specifications. Preserve package independence, isolated Test Lab scenes, neutral technical APIs, Verse-flavored user-facing names, and the repository/Obsidian documentation workflow. Before writing code, summarize the approved architecture, identify the current checkpoint from Current Notes, and continue the Foundation Specification Pass using SFGSS-001. Do not begin runtime implementation until every Foundation Wave specification and the cross-package consistency review are approved. Do not invent unresolved architecture without recording it for approval.

## Checkpoint Rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Promote durable decisions into the correct authoritative document.
3. Update test, issue, setup, changelog, or release records as applicable.
4. Confirm documentation matches the committed implementation.
5. Commit and push the documentation update with the related code when practical, or in an immediately adjacent documentation commit.
