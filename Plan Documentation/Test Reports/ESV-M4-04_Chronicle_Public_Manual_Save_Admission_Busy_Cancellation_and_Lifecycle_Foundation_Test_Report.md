---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---
# ESV-M4-04 — Chronicle Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-04
**Unity:** 6000.3.8f1
**Planning/activation commit:** `91dcb62`
**Implementation commit:** `2732aaa`
**Lifecycle-status hotfix:** `09ae8f1`
**Final effective runtime baseline:** `09ae8f1`

## Final Result

`EchoDevGames.EchoSave.Tests.Editor`

**456 / 456 passed, 0 failed**

Prior focused regression floor: **439 / 439**

Net new focused tests over the prior floor: **17**

## Evidence Summary

The final green gate covers:
- public active-slot save service exposure;
- service-not-ready rejection;
- shutdown-admission rejection;
- one root-local admitted mutating operation;
- overlapping manual-save Busy rejection;
- Busy non-queue behavior;
- admission release after terminal operation outcomes;
- pre-canceled request rejection;
- safe pre-publication cancellation;
- Too-Late cancellation after durable publication begins;
- shutdown closure before backend shutdown;
- public success/failure/partial durable result mapping;
- M4-03 transaction reuse;
- absence of autosave/coalescing and other deferred M4 surfaces.

## Intermediate Failure and Correction

Initial focused result after M4-04 implementation:
- **456 discovered**
- **455 passed**
- **1 failed**

Failure:
`SaveBeforeReadyRejectsWithoutTransactionExecution`

Expected:
`ServiceNotReady`

Observed:
`AdmissionClosed`

The implementation incorrectly allowed the operation-admission coordinator's intentionally closed pre-initialization state to override the public lifecycle result.

Final correction at `09ae8f1`:
- pre-Ready lifecycle states return `ServiceNotReady`;
- shutdown states return `AdmissionClosed`;
- actual Closed/Busy admission is evaluated only once the service is Ready.

Final rerun:
**456 / 456 passed, 0 failed**

## Scope Integrity

The checkpoint does not claim proof for:
- autosave/coalescing;
- generic queued multi-operation scheduling;
- retention/recovery;
- rename/duplicate/delete;
- persistent catalog cache;
- full slot-policy assets;
- scene travel;
- peer bridges;
- project-wide DDOL.
