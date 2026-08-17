# EUI-M4-02 — Looking Glass Bounded Notification Channels, Priority, Coalescing, Overflow, and Unscaled Lifetime

**Package:** The Looking Glass (`EchoUI`)
**Milestone:** M4 — Complete MVP Surfaces
**Status:** ACTIVE / AUTHORIZED
**Activation baseline:** `5e7ad9211a66ba3dc9f26c4a268febe4ba9c9d3d`
**Suite authority:** SFGSS-000 v0.27.0
**Package authority:** SFGSS-PKG-ECHOUI-001 v1.8.0
**Workflow authority:** SFGSS-005 v1.7.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activated:** August 17, 2026

## 1. Purpose and observable outcome

Implement the smallest independently useful notification slice: project-defined stable channels with bounded visible/pending capacity, deterministic priority and FIFO tie ordering, opt-in coalescing, explicit pending-overflow policy, unscaled automatic lifetime, manual dismissal, generation-safe ownership handles, and side-effect-free status/events.

The Laboratory must prove that independent channels do not interfere; higher-priority pending work promotes before lower priority while equal-priority work remains FIFO; visible notifications are not silently preempted; coalescing replaces one matching live generation without multiplying entries; overflow is deterministic; automatic duration continues while scaled time is paused; manual notifications remain until dismissed; and owner loss, stale handles, reset, and shutdown cannot corrupt newer state or Screen/Modal/Window/HUD authority.

## 2. Starting conditions

- Repository and origin `main` are exactly at EUI-M4-01 documentation closeout `5e7ad92`.
- EUI-M4-01 implementation/Laboratory proof is sealed at `29573ef`.
- EUI-M4-01 automated focused/full gate is user-confirmed green; exact post-M4 NUnit totals were not captured.
- Retained pre-M4 floor `1246 / 1246` is historical evidence only and must not be relabeled as the EUI-M4-02 incoming count.
- EUI-M4-01 manual HUD Laboratory is **5 / 5 PASS**, retained M3/M2/M1 smoke is user-confirmed green, and package/imported parity is verified.
- PKG-LEARN-008 is complete through the bounded EUI-M4-02 JIT revisit.
- No peer Echo package is required.

## 3. Learn → Declare → Authorize reconciliation

### 3.1 Learned boundary

Notifications are transient presentation, not a durable event log, quest/objective history, analytics stream, localization authority, audio command system, save record, or gameplay truth. Projects supply presentation content and react to explicit results; Looking Glass owns only bounded admission, ordering, display lifecycle, settlement, and diagnostics.

### 3.2 Declared intent

- Channels are project-defined, variable-count, stable-ID-addressed, immutable definitions.
- Each channel owns independent visible and pending capacities.
- Higher numeric priority promotes first; equal priority preserves admission FIFO.
- Once visible, an entry is not silently preempted by later priority.
- Coalescing is opt-in through a nonempty stable key scoped to one channel.
- A matching pending or visible entry is superseded by a fresh generation in the same logical slot; the old handle becomes stale and the default coalesced lifetime restarts.
- Pending overflow defaults to `RejectNewest`. Channels may instead author `DropOldestPending` or `ReplaceLowestPriorityPending`.
- `ReplaceLowestPriorityPending` replaces only when the incoming entry strictly outranks the selected pending entry; equal/lower priority rejects.
- Automatic lifetime uses an injected unscaled monotonic clock. Manual lifetime remains until explicit dismissal or structural cleanup.
- Runtime overrides are session state and never mutate authored definitions.
- Notification state remains independent of Screen history, Modal order, Window state, HUD region leases, gameplay input, pause/time scale, cursor, persistence, and project domain truth.

### 3.3 Authorized slice

EUI-M4-02 owns only notification channel definitions, bounded admission/promotion, priority/FIFO ordering, coalescing, pending overflow, unscaled/manual lifetime, generation-safe handles, ownership/loss settlement, status/events, focused tests, and Laboratory proof.

## 4. Authority and invariants

1. Channel and coalescing identity are stable-ID based, normalized, and collision checked.
2. Channel definitions remain immutable during play.
3. Visible and pending collections are separately bounded per channel.
4. Channels schedule independently; one channel cannot consume or reorder another channel's capacity.
5. Priority orders pending promotion only; visible notifications are never silently preempted.
6. Equal priority is strict FIFO by monotonic admission sequence.
7. Coalescing never multiplies matching live entries.
8. Coalescing creates a fresh generation and settles/supersedes the prior generation before the replacement becomes authoritative.
9. A stale handle cannot dismiss or mutate a replacement.
10. Overflow policy applies to pending entries only and produces an explicit result/settlement reason.
11. Automatic lifetime begins only when an entry becomes visible and uses unscaled monotonic time.
12. Manual lifetime never expires automatically.
13. Owner loss removes only matching live generations and promotes pending work deterministically.
14. Listener failure cannot roll back committed notification truth.
15. Status and diagnostics do not retain arbitrary visible text, typed content, or project payloads.
16. Reset/shutdown settles every live generation exactly once and releases all view/presenter state.
17. Notification mutation never changes Screen, Modal, independent Window, HUD, focus, gameplay, pause, cursor, persistence, or domain authority.
18. Capacity, completion history, and diagnostics remain bounded.

## 5. Runtime scope

Exact public names may receive compile-safe refinement without changing authority:

- `UINotificationChannelId` and `UINotificationCoalescingKey`.
- Immutable `UINotificationChannelDefinition` with visible/pending capacities, default lifetime, and overflow policy.
- `UINotificationRequest`, priority, lifetime mode/duration, optional coalescing key, owner, presentation seam, and correlation identity.
- Structured admission and terminal results, including admitted, coalesced, rejected, expired, dismissed, overflow-evicted, owner-lost, reset, shutdown, stale, and already-settled outcomes as appropriate.
- Fresh generation-safe `UINotificationHandle`.
- Root-owned `UINotificationService`.
- Per-channel bounded visible and priority/FIFO pending state.
- Opt-in pending/visible coalescing with fresh-generation replacement.
- `RejectNewest`, `DropOldestPending`, and `ReplaceLowestPriorityPending` policies.
- Injected unscaled monotonic clock and deterministic tick/advance seam without claiming game time authority.
- Project-owned notification presenter/view seam; sample-owned reference presentation remains in the Laboratory.
- Post-commit events and side-effect-free notification status snapshots.
- Root initialization, reset, shutdown, and owner-loss cleanup.
- Existing transition contracts may remain available to future notification presentation, but this checkpoint adds no transition driver and does not require transition wiring to prove queue/lifetime authority.

## 6. Automated proof

At minimum prove valid stable channel initialization/lookup; invalid/duplicate definitions without partial state; independent per-channel bounds; higher-priority pending promotion and FIFO ties; no visible preemption; pending and visible coalescing without multiplication; fresh replacement generation and stale old-handle rejection; lifetime restart; all three pending-overflow policies; automatic unscaled expiry while scaled time is paused; manual lifetime and explicit dismissal; idempotent exact-once settlement; owner loss/reset/shutdown; deterministic promotion; listener isolation; side-effect-free snapshots; no Screen/Modal/Window/HUD/focus mutation; bounded idle state; and full retained EchoUI regression.

## 7. Laboratory proof

Add an **M4-02 Notifications** tab before retained tabs. Use sample-defined channels and plain reference views to demonstrate:

1. independent channels, visible capacity, priority promotion, FIFO ties, and no visible preemption;
2. pending and visible coalescing, fresh replacement generation, lifetime restart, and stale old-handle rejection;
3. `RejectNewest`, `DropOldestPending`, and `ReplaceLowestPriorityPending` without unrelated mutation;
4. automatic unscaled expiry while `Time.timeScale == 0`, plus a manual notification that remains until explicit dismissal;
5. owner-loss cleanup, deterministic promotion, idempotent/stale handle behavior, reset, and baseline restoration;
6. 180-frame idle quiescence plus retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke.

The Laboratory may simulate pause/time-scale only as sample-owned proof. Looking Glass never becomes pause authority.

## 8. Explicit exclusions

This checkpoint does not authorize prompts; tooltips; safe-area placement; Motif/accessibility service implementation beyond the notification manual-lifetime seam; durable notification history; localization content; audio playback; analytics; gameplay/domain commands; scene travel; save/settings authority; gameplay input; action-map switching; pause/time-scale or cursor ownership; full Window LIFO/pinning/z-order/drag/resize/layout/persistence; Primitive Warehouse; 9-slice library; Panel/Menu templates; Template Catalog; Assembly Utilities; Builder/Composer; peer bridges; project-wide lifetime composition; pooling; new transition drivers; showcase art; integration; clean-project reproduction; or release qualification.

## 9. Expected files

- `Packages/com.echodevgames.echo-ui/Runtime/Notifications/`
- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- existing root status/runtime-state files as required
- focused Editor tests under `Packages/com.echodevgames.echo-ui/Tests/Editor/`
- Laboratory README/driver/scene in package `Samples~` and imported sample
- closeout documentation named below

Every new Unity asset requires its `.meta`.

## 10. Implementation sequence

1. On the activation commit, run the full EchoUI Editor and Foundry EditMode suites before Runtime edits; record the exact post-M4 baseline counts.
2. Add pure stable IDs, definitions, policies, requests, results, and handles.
3. Add channel initialization/validation and bounded independent state.
4. Add admission, visible placement, priority/FIFO pending order, and deterministic promotion.
5. Add opt-in coalescing with fresh-generation replacement and lifetime restart.
6. Add pending-overflow policies and exact settlement.
7. Add unscaled automatic lifetime, manual dismissal, owner loss, reset, shutdown, events, and status.
8. Add focused tests; run EchoUI and full EditMode suites.
9. Extend Laboratory while preserving retained tabs and package/imported parity.
10. Obtain manual acceptance.
11. Seal implementation, reconcile documentation, commit, and push.

## 11. Validation gates

- Exact activation-baseline EchoUI and full EditMode totals recorded with zero failures before Runtime edits
- Focused EUI-M4-02 tests all pass
- EchoUI Editor all pass
- Full Foundry EditMode all pass with zero failures
- `git diff --check` and cached equivalent clean
- Package/imported Laboratory parity verified
- Manual notification Laboratory all checks pass
- Retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke passes
- Bounded idle/unscaled-time evidence passes
- Repository clean and origin synchronized at closeout

## 12. Failure and bounded fixes

Invalid/missing channels reject without partial state. Duplicate IDs reject initialization. Full visible capacity queues within the pending bound. Full pending capacity applies exactly the authored overflow policy. Coalescing settles the old generation before the replacement becomes authoritative. Stale handles preserve current truth. Clock/lifetime failure cannot mutate unrelated channels or structural UI. Listener exceptions are isolated after state commit. Compile/test corrections inside this contract are pre-approved.

## 13. Rollback

Before implementation sealing, restore only the EUI-M4-02 manifest if a red gate cannot be resolved inside scope. Do not rewrite retained EUI-M1 through EUI-M4-01 history. No destructive project migration is authorized.

## 14. Documentation reconciliation

At closeout update this plan, package specification, PKG-LEARN-008/tracker, suite/package Current Notes, Suite Graph Roadmap, Suite Health, package changelog, and Laboratory evidence. Preserve exact hashes, test counts, and missing-evidence qualifications.

## 15. Commit plan

- Activation: `Activate EUI-M4-02 bounded notification channels`
- Runtime/tests: `Implement EUI-M4-02 bounded notification channels`
- Laboratory: `Add EUI-M4-02 notification Laboratory proof`
- Closeout: `Close out EUI-M4-02 notification lifecycle`

## 16. Completion criteria and stop point

EUI-M4-02 completes only after exact incoming/final automated evidence, manual proof, retained regression, truthful documentation, clean repository, and push. Stop there. Do not begin prompt, tooltip, Motif/accessibility, safe-area, Window-management, authoring-library, Builder, bridge, integration, or release work without a separate activation.

## 17. Named next direction

Prompt and tooltip work remains a named package capability, but this activation does not assign or authorize a successor checkpoint ID.

## 18. Handoff

EUI-M4-02 is ACTIVE / AUTHORIZED from `5e7ad92` under package authority v1.8.0. Runtime implementation has not started. First gate: run EchoUI Editor and full Foundry EditMode on the activation commit and record exact post-M4 totals before any Runtime edit. Follow the visible slice loop and stop for an unexpected red gate, authority-changing decision, or required manual Unity proof.
