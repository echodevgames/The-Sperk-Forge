# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.6.0
**Completed checkpoint:** ESV-M2-03 — Generation Identity, Integrity, and Commit-Document Foundation
**Current checkpoint:** ESV-M2-04 — Immutable Generation Publication and Head-Last Commit Foundation
**Status:** ESV-M2-03 complete; ESV-M2-04 active / authorized

## ESV-M2-03 closeout

Implementation commit: `ad3b646`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **87 / 87 passed, 0 failed**;
- canonical package-generated slot identity;
- unique sortable generation identity;
- package manifest/payload/head document shapes;
- package transport inventory shape;
- detached-byte SHA-256 calculation and verification;
- manifest/payload/head agreement validation;
- serializer compatibility for new package documents;
- no physical generation/head mutation;
- all prior 57 Chronicle regressions remain green.

## Active ESV-M2-04 boundary

Authorized:
- provider-neutral publication capabilities/primitives;
- local backend candidate-to-final publication;
- local small-head publication/replacement with accurately advertised capability semantics;
- package generation publication coordinator;
- candidate path under `incomplete/<generation-id>`;
- verified immutable final path under `generations/<generation-id>`;
- payload + manifest write/verify before final publication;
- head publication **last**;
- old known-good head/generation preserved on pre-head failure;
- failure/interruption tests in sandbox storage.

Still absent:
- slot catalog/cache and active-slot policy;
- participant capture/apply;
- gameplay payload ownership;
- recovery selection/execution;
- retention cleanup;
- autosave;
- prepared loads;
- migrations;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

M2-04 may physically publish the already-proven empty/transport payload generation. That is real save-publication infrastructure, but it is not yet the complete player-facing save pipeline.
