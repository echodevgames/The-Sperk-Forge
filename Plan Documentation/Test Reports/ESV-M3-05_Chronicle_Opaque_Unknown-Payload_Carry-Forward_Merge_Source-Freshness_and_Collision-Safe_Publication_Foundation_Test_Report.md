---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-05
**Implementation commit:** `af28c96`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 243 / 243**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 243 |
| Passed | 243 |
| Failed | 0 |
| Prior regression floor | 218 / 218 preserved |

## Added M3-05 coverage

The focused suite added **25** passing tests over the prior **218**-test regression floor.

Coverage includes provenance refresh/reset/preservation, stale source and slot mismatch rejection, canonical/alias collision rejection, exact opaque carry-forward, deterministic merged ordering, unknown opacity, candidate/final/head failure behavior, successful head-last merged publication, and old-snapshot staleness after head advance.

## Evidence boundary

This report qualifies the bounded no-data-loss unknown carry-forward path only. Participant deserialization/migration/apply, prepared loads, production operation admission, slots, recovery, retention, autosave, and release readiness remain later work.
