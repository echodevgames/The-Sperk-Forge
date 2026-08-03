# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only; SFGSS-000, approved package specifications, and accepted ADRs remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 3, 2026  
**Current focus:** Foundation Wave package specifications  
**Current checkpoint:** FW-DOC-02 — Draft The Observatory (`EchoDiagnostics`) package specification

> Capture quickly here. Promote deliberately at checkpoint closeout.

---

## How to Use This Page

Use this page for information discovered while designing, implementing, testing, or reviewing the suite:

- `[NOTE]` — useful observation or context.
- `[QUESTION]` — unresolved question requiring research or approval.
- `[PROPOSAL]` — suggested change that is not yet authoritative.
- `[DECISION]` — approved decision awaiting or confirming documentation promotion.
- `[TEST]` — test result, reproduction, or validation evidence.
- `[BUG]` — defect or regression awaiting issue-log placement.
- `[RISK]` — dependency, compatibility, schedule, or architecture concern.
- `[HANDOFF]` — context the next work session must see.

Keep entries dated. Link to the affected specification, ADR, checkpoint, test, issue, guide, or source file whenever possible.

Do not leave durable decisions only on this page. At checkpoint closeout, promote each material entry into the document that owns it and record the destination below.

---

## Current Focus

### Goal

Draft, reconcile, and approve complete SFGSS-001 package specifications for all ten Foundation Wave packages before any Foundation Wave runtime implementation begins.

### Active source documents

- `Echo_Game_Systems_Suite_Bible.md` — SFGSS-000 v0.6.0.
- `SFGSS-001_Package_Specification_Template.md` — v1.1.0.
- `Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification.md` — v1.0.0 Approved.
- `Foundation_Wave_Specification_Roadmap.md` — active Level 4 planning record.

### Next action

Draft the complete **The Observatory — Diagnostics (`EchoDiagnostics`) Package Specification** using SFGSS-001. Reconcile its contracts against First Light’s launch report and optional diagnostics bridge without making either package depend on the other.

---

## Open Questions

- None blocking the start of the EchoDiagnostics specification.
- Licensing remains a later suite-wide release decision and does not block the documentation pass.

---

## Active Notes

### August 3, 2026 — Living repository documentation

- `[DECISION]` Suite and package documentation will live in the Git repository beside development work.
- `[DECISION]` The repository documentation folder will be opened directly in Obsidian rather than copied into a separate vault.
- `[DECISION]` Every active repository will expose a linked `Current Notes.md` page for ongoing observations, proposals, tests, questions, and handoff context.
- `[DECISION]` At meaningful checkpoints, durable notes will be promoted into the bible, package specification, ADR, issue/test record, guide, changelog, or checkpoint status that owns them.
- `[DECISION]` Major documentation changes will be committed with the related code when practical, or in an immediately adjacent documentation commit.

**Promoted to:** SFGSS-000 v0.5.0 decision 31 and SFGSS-001 v1.1.0 documentation requirements.

### August 3, 2026 — Foundation Specification Pass

- `[DECISION]` Complete and approve all ten Foundation Wave package specifications before beginning Foundation Wave runtime implementation.
- `[DECISION]` Run a cross-package consistency review after the tenth specification and before opening any M1 package skeleton checkpoint.
- `[DECISION]` First Light specification v1.0.0 is approved as the Level 2 package authority, but its implementation remains deferred by the suite documentation gate.
- `[DECISION]` First Light uses Unity `Awaitable<T>` for startup execution.
- `[DECISION]` First Light startup authoring uses immutable `StartupStepDefinition` ScriptableObjects that create separate single-use runtime executors.
- `[DECISION]` First Light ships a default uGUI status/image presenter isolated from its launch core.
- `[DECISION]` First Light root lifetime is configurable and defaults to `UntilHandoff`.
- `[DECISION]` The initial public Foundation package floor is Unity 6000.0, with Unity 6000.3.8f1 as the primary development baseline.

**Promoted to:** SFGSS-000 v0.6.0 decisions 32–33, First Light specification v1.0.0, and the Foundation Wave Specification Roadmap.

---

## Promotion Queue

| Date | Entry | Destination | Status |
|---|---|---|---|
| 2026-08-03 | Repository/Obsidian living-documentation workflow | SFGSS-000 and SFGSS-001 | Promoted |
| 2026-08-03 | Foundation Specification Pass before implementation | SFGSS-000 v0.6.0 and roadmap | Promoted |
| 2026-08-03 | First Light implementation-shaping choices | First Light specification v1.0.0 | Promoted |
| 2026-08-03 | Unity 6 package floor | SFGSS-000 v0.6.0 | Promoted |

---

## Latest Validation Snapshot

| Area | Result | Evidence/notes |
|---|---|---|
| Suite bible | Approved baseline updated | v0.6.0; decisions 32–33 added |
| Package specification template | Approved and unchanged | v1.1.0 |
| First Light specification | Approved | v1.0.0; no release-blocking design questions |
| Foundation documentation gate | Active | 1 of 10 package specifications approved |
| Implementation | Not started by design | No package code begins before FW-DOC-12 passes |

---

## Checkpoint Closeout Checklist

- [x] Review every note added during the checkpoint.
- [x] Separate confirmed facts from proposals and unresolved questions.
- [x] Promote architecture changes into the bible/specification and an ADR when needed.
- [x] Move bugs and test evidence into their permanent records. No implementation defects exist yet.
- [x] Update guides and changelog for user-visible changes. Not applicable before package implementation.
- [x] Update the current checkpoint, blockers, and next action.
- [x] Confirm documentation matches committed implementation and tests. Implementation has not started.
- [ ] Commit and push the documentation update.
- [x] Condense or remove resolved notes after promotion.

---

## Handoff Snapshot

**Current program:** Foundation Specification Pass  
**Completed package specification:** First Light (`EchoLaunch`) v1.0.0 Approved  
**Current package:** The Observatory (`EchoDiagnostics`)  
**Current stage:** Specification not yet drafted  
**Last completed documentation change:** First Light choices approved; documentation-first Foundation gate promoted into SFGSS-000 v0.6.0  
**Known blockers:** None  
**Next checkpoint:** FW-DOC-02 — Draft and review the complete EchoDiagnostics package specification
