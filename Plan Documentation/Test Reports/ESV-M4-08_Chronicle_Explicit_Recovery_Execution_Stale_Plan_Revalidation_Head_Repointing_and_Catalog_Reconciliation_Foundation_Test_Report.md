---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: passed
updated: 2026-08-10
---
# ESV-M4-08 — Chronicle Explicit Recovery Execution — Test Report

**Implementation commit:** `1985fb0`
**Unity:** 6000.3.8f1
**Assembly:** `EchoDevGames.EchoSave.Tests.Editor`

## Result

| Measure | Result |
|---|---:|
| Prior focused floor | 524 / 524 |
| Final discovered | 540 |
| Passed | 540 |
| Failed | 0 |
| Net new focused tests | 16 |

## Verified behavior

The focused suite verifies the bounded M4-08 execution contract:
- explicit public recovery execution;
- shared root-local mutation admission;
- Busy/no-queue behavior;
- admission-closed lifecycle truth;
- fresh recovery-plan rebuild;
- source-provenance stale-plan rejection;
- candidate-membership and revalidation checks;
- no mutation on stale/invalid execution attempts;
- head-only durable recovery publication;
- explicit non-preferred candidate selection;
- committed-head truth when catalog reconciliation later fails;
- healthy catalog reconciliation on success;
- selected generation manifest/payload byte immutability;
- active-slot selection preservation;
- no participant execution side effects.

## Compile-only maintenance

Before the final gate, the new service-level test used the nonexistent test-double property `CallCount`.

The established fake exposes `Calls`.

The two references were corrected without changing runtime/API/architecture/authority/test intent.

## Evidence boundary

This report proves the focused Chronicle Editor gate only.

It does not claim:
- full repository aggregate testing;
- Play Mode recovery coverage;
- clean-project reproduction;
- player-build qualification;
- performance/release qualification;
- automatic recovery policy;
- quarantine/destructive cleanup.

Final authoritative focused evidence for ESV-M4-08 is **540 / 540 passed, 0 failed**.
