# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.7.0
**Completed checkpoint:** ESV-M2-04 — Immutable Generation Publication and Head-Last Commit Foundation
**Completed milestone:** M2 — Document / Storage Core
**Current checkpoint:** ESV-M3-01 — Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation
**Status:** M2 complete; ESV-M3-01 active / authorized

## ESV-M2-04 closeout

Implementation commit: `01b7ad3`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **102 / 102 passed, 0 failed**;
- provider-neutral publication capability seam;
- local candidate-to-final same-root publication;
- local small-current-object move/replace publication;
- generation-first / head-last transaction;
- previous head/generation preserved on injected pre-head failure;
- orphaned verified generation remains non-current after failed head publication;
- duplicate generation IDs are rejected;
- committed generation files remain create-only;
- local backend truthfully reports no universal power-loss atomicity guarantee;
- all prior 87 Chronicle regressions remain green.

## Active ESV-M3-01 boundary

Authorized:
- `SaveParticipantId`;
- participant criticality and missing-payload policy;
- validated `SaveParticipantDescriptor`;
- `ISaveParticipant`;
- structured participant registration results/status;
- `SaveParticipantRegistration`;
- `SaveParticipantRegistry`;
- duplicate and alias-collision rejection;
- deterministic canonical-ID ordering;
- idempotent registration disposal/unregister;
- bounded immutable registry snapshot;
- focused registry/contract tests.

Still absent:
- participant capture orchestration;
- participant apply orchestration;
- `SaveAsync`;
- prepared loads;
- unknown-payload carry-forward;
- migrations;
- slot catalog/policy;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

M3-01 establishes the participant registry as runtime session state only. Registration is not durable persistence and does not touch save files.
