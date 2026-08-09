---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-01 — Chronicle Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-01
**Implementation commit:** `b3b5f9f`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 147 / 147**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 147 |
| Passed | 147 |
| Failed | 0 |
| Ignored | 0 reported |

## Added M3-01 coverage

The focused suite added **45** passing participant tests over the prior **102**-test regression floor.

Coverage includes:
- canonical participant-ID acceptance;
- malformed/noncanonical/path-like ID rejection;
- descriptor schema/criticality/missing-payload validation;
- bounded alias validation and defensive copying;
- unique registration;
- arbitrary future participant registration without a central catalog;
- duplicate canonical-ID rejection;
- canonical/alias, alias/canonical, and alias/alias collision rejection;
- canonical and alias lookup;
- deterministic canonical-ID snapshot ordering;
- aliases not creating duplicate entries;
- immutable snapshot behavior;
- registration disposal/unregister;
- idempotent disposal;
- stale registration unable to eject a replacement participant;
- registry clear invalidating old handles safely;
- registry registration/lookup never invoking participant capture/apply.

## Evidence boundary

This report qualifies participant identity, descriptor validation, and runtime registry behavior only. It does not qualify detached capture serialization, participant-backed physical generation publication, `SaveAsync`, loading/apply, unknown-payload carry-forward, migration, slot policy, recovery, retention, autosave, or release readiness.
