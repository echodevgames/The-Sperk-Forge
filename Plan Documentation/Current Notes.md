# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M3-08 authority promotion
**Current checkpoint:** FL-M3-08 — Initial Destination Contract, Load Result, and Completed Handoff

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Commit the approved destination asset and serialized schema decision before FL-M3-08 runtime implementation begins.

### Starting State

- FL-M3-07 implementation is complete in `a6f6544`.
- FL-M3-07 documentation is complete in `f76b9df`.
- `main` and `origin/main` are synchronized at `f76b9df`.
- Working tree is clean.
- Runtime Play Mode baseline is 336 passed, 0 failed, 0 ignored.
- Compilation baseline is 0 errors and 0 compiler warnings.
- `EchoLaunchConfiguration.CurrentSchemaVersion` is already 2 in the live repository.
- Schema 2 contains no destination reference.
- Runtime destination validation, loading, successful report finalization, `LaunchCompleted`, and `Completed` handoff are not implemented.

---

## Active Notes

### August 5, 2026 — FL-M3-08 destination and schema decision

- `[DECISION]` `LaunchDestination` is a standalone project-owned ScriptableObject.
- `[DECISION]` `LaunchDestination.CurrentSchemaVersion` begins at 1.
- `[DECISION]` `EchoLaunchConfiguration.CurrentSchemaVersion` advances from 2 to 3.
- `[DECISION]` Configuration schema 2 remains the historical startup-sequence-only shape.
- `[DECISION]` Configuration schema 3 adds the serialized initial destination reference.
- `[DECISION]` Runtime blocks older/unknown schema and never silently rewrites assets.
- `[DECISION]` Editor migration from schema 2 to 3 is later work.
- `[DECISION]` The destination loader remains injectable and package-local.
- `[DECISION]` Normal mid-game scene travel remains outside EchoLaunch.
- `[AUTHORITY]` First Light specification advances to v1.4.0.
- `[AUTHORITY]` Package ADR `EchoLaunch-ADR-001` records the durable choice.
- `[PLAN]` FL-M3-08 is bounded through completed handoff and stops before automatic startup or presentation.
- `[EVIDENCE]` Implementation and migration evidence remain not run until the authority commit is pushed.

---

## Promotion Queue

| Entry | Destination | State |
|---|---|---|
| Standalone project-owned destination asset | First Light specification v1.4.0 | Promoted in bundle |
| Configuration schema 3 | First Light specification v1.4.0 | Promoted in bundle |
| Durable rationale and alternatives | EchoLaunch-ADR-001 | Promoted in bundle |
| Runtime/test scope | FL-M3-08 Checkpoint Build Plan | Promoted in bundle |
| Package handoff | Package Current Notes | Promoted in bundle |
| Package navigation | Documentation index | Promoted in bundle |
| Authority commit evidence | Git history | Pending |

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| Repository baseline | `f76b9df` |
| Working tree before authority bundle | Clean |
| Compilation baseline | 0 errors, 0 compiler warnings |
| Runtime Play Mode baseline | 336 passed, 0 failed, 0 ignored |
| Specification decision | Approved |
| Package ADR | Accepted, evidence pending |
| Runtime implementation | Locked until authority commit |
| Known blockers | None after authority commit |

---

## Checkpoint Gate

- [x] Approve standalone project-owned `LaunchDestination`.
- [x] Detect that configuration schema 2 already exists.
- [x] Approve configuration schema 3.
- [x] Preserve schema 2 as historical.
- [x] Draft specification v1.4.0.
- [x] Draft EchoLaunch-ADR-001.
- [x] Draft FL-M3-08 Checkpoint Build Plan.
- [x] Reconcile package and suite Current Notes.
- [ ] Apply and review authority bundle.
- [ ] Commit and push authority update.
- [ ] Confirm clean synchronized repository.
- [ ] Begin FL-M3-08 runtime implementation.

---

## Handoff Snapshot

**Active checkpoint:** FL-M3-08 — Initial Destination Contract, Load Result, and Completed Handoff
**Starting implementation commit:** `a6f6544`
**Starting documentation commit:** `f76b9df`
**Authority update:** Pending commit
**Runtime Play Mode baseline:** 336 passed, 0 failed, 0 ignored
**Compilation baseline:** 0 errors, 0 compiler warnings
**Known blockers:** Authority bundle must be committed before code
**Next action:** Apply, stage, commit, and push the FL-M3-08 authority bundle
