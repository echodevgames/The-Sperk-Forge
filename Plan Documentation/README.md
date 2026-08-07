# Plan Documentation

This folder is the living planning, architecture, test, and handoff record for **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

The Markdown files in this folder are committed to Git and opened directly in Obsidian. Do not maintain a second copied vault. Git preserves documentation history; Obsidian is the editing and navigation surface.

Current work: First Light FL-M6-02 clean-project private-beta candidate
validation after the fully closed `5c21ea4` FL-M6-01 documentation baseline.

---

## Start Here

### Continuing an existing work session

Read:

1. `ChatGPT_Handoff.md`
2. `Current Notes.md`
3. The active package specification
4. The active package architecture and Current Notes
5. The latest completed checkpoint
6. The active Checkpoint Build Plan
7. Relevant implementation and tests

### Starting a new ChatGPT conversation

Use:

- [`ChatGPT_Handoff.md`](ChatGPT_Handoff.md)

That file contains:

- Authority order
- Required upload set
- Repository reading order
- Mandatory first-response fields
- The copy-and-paste opening prompt
- Checkpoint workflow rules
- CMD-first file automation rules
- Git and test evidence requirements
- Conversation rotation guidance

Do not rely on the old conversation transcript as the only source of project truth.

---

## Core Authority Documents

| Document | Role |
|---|---|
| `Echo_Game_Systems_Suite_Bible.md` | SFGSS-000, suite-wide authority |
| `SFGSS-001_Package_Specification_Template.md` | Required package-specification structure |
| `ChatGPT_Handoff.md` | New-conversation and repository-rehydration protocol |
| `Current Notes.md` | Fresh working context, evidence, blockers, and handoff state |

When documents disagree, use the authority order defined in SFGSS-000 and `ChatGPT_Handoff.md`.

`Current Notes.md` provides the freshest working context but does not silently override approved architecture.

---

## Repository Documentation Structure

```text
Plan Documentation/
├── README.md
├── ChatGPT_Handoff.md
├── Echo_Game_Systems_Suite_Bible.md
├── SFGSS-001_Package_Specification_Template.md
├── Current Notes.md
├── Package Specifications/
├── Architecture Decision Records/
├── Checkpoint Build Plans/
├── Implementation Checkpoints/
├── Test Reports/
├── Release Records/
└── Workflow and Standards/
```

Subfolders should contain real records rather than decorative empty placeholders.

---

## Document Authority

When documents disagree:

1. SFGSS-000
2. Approved individual package specification
3. Accepted ADR or integration specification
4. Active Checkpoint Build Plan
5. Package architecture and completed checkpoint records
6. Test, issue, setup, changelog, and release records
7. `Current Notes.md`
8. Chat history

Stop and identify a material conflict rather than silently selecting whichever document appears newest.

---

## Working Notes

Use `Current Notes.md` for:

- `[NOTE]` observations
- `[QUESTION]` unresolved questions
- `[PROPOSAL]` unapproved changes
- `[DECISION]` approved decisions awaiting promotion
- `[TEST]` validation evidence
- `[BUG]` defects
- `[RISK]` architecture, compatibility, or schedule concerns
- `[HANDOFF]` context the next work session must see

Durable decisions must be promoted into the authoritative document that owns them.

Git history is the archive. Resolved working notes may be condensed after promotion.

---

## Checkpoint Rule

At every meaningful checkpoint:

1. Reconcile root and package `Current Notes.md`.
2. Promote durable decisions into the correct authoritative document.
3. Update test, issue, setup, architecture, changelog, README, index, release, or completion records as applicable.
4. Confirm documentation matches the committed implementation.
5. Commit and push documentation with the related code when practical, or in an immediately adjacent documentation commit.
6. Confirm the repository is clean and synchronized.

---

## Historical Bootstrap Note

The repository originally began with four bootstrap documents and a prompt to draft the First Light package specification.

That objective is complete and no longer describes the current work.

Use `ChatGPT_Handoff.md` and the current repository records for all future conversations.
