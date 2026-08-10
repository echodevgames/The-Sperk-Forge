---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---
# ESV-M4-05 — Chronicle Autosave Request Coalescing and Latest-Wins Pending Admission Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-05
**Unity:** 6000.3.8f1
**Planning/activation commit:** `8504ed4`
**Implementation commit:** `9917f1b`
**Final effective runtime baseline:** `9917f1b`

## Final Result

`EchoDevGames.EchoSave.Tests.Editor`

**473 / 473 passed, 0 failed**

Prior focused regression floor: **456 / 456**

Net new focused tests over the prior floor: **17**

## Evidence Summary

The final green gate covers:
- public explicit caller-triggered autosave submission;
- request/preflight rejection;
- one root-local admission authority shared with manual save;
- zero-or-one pending latest autosave;
- latest-wins coalescing/supersession;
- repeated request pressure remaining bounded;
- manual-save Busy behavior unchanged;
- pending drain after admission release;
- at-most-once pending execution;
- safe pending preflight failure/clear behavior;
- shutdown rejection and pending discard;
- admitted autosave reuse of M4-03/M4-04 durable save semantics;
- deferred-scope boundaries remaining absent.

## Intermediate Regression Failure

The first M4-05 focused run exposed one stale M4-04 test:

`PublicServiceExposesOnlyBoundedManualSaveOperation`

The old test expected `RequestAutosave` to be absent. That expectation was correct under M4-04, when autosave was explicitly deferred, but became stale once M4-05 authorized the public autosave surface.

The test was updated to assert:
- bounded `SaveAsync(SaveRequest)` remains present;
- bounded `RequestAutosave(AutosaveRequest)` is present;
- autosave submission returns `AutosaveSubmissionResult`.

No runtime implementation, public API, architecture, or M4-05 authority changed in that hotfix.

Final rerun:
**473 / 473 passed, 0 failed**

## Scope Integrity

Final committed implementation/test scope:
- **22 files**
- **2074 insertions**
- **43 deletions**

Still outside the checkpoint:
- automatic autosave timers;
- retention / `SaveRetentionPolicy`;
- generic queued operation scheduling;
- recovery;
- rename/duplicate/delete;
- persistent catalog cache;
- full slot-policy assets;
- scene travel;
- peer bridges;
- project-wide DDOL.
