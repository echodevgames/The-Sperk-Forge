# EUI-M4-01 — Looking Glass Named HUD Regions, Widget Leases, and Deterministic Visibility Authority

**Package:** The Looking Glass (`EchoUI`)
**Milestone:** M4 — Complete MVP Surfaces
**Status:** ACTIVE / AUTHORIZED
**Current phase:** Runtime and automated proof green through `e47d43b`; Laboratory/manual proof pending
**Activation baseline:** `0affb7de757f8acdd35175457f70d00c657b85c3`
**Suite authority:** SFGSS-000 v0.27.0
**Package authority:** SFGSS-PKG-ECHOUI-001 v1.7.0
**Workflow authority:** SFGSS-005 v1.6.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activated:** August 16, 2026

## 1. Purpose and observable outcome

Implement the smallest independently useful M4 surface slice: project-defined named HUD regions with deterministic widget registration, generation-safe disposable leases, project-controlled effective visibility, explicit ownership/loss handling, structured results, bounded capacity, and side-effect-free status reporting.

The Laboratory must prove that multiple widgets can inhabit authored HUD regions; independent visibility reasons can overlap and release out of order; stale handles cannot mutate replacements; and owner loss cleans safely without changing Screen history, Modal order, independent Window state, gameplay input, pause/time scale, persistence, or project domain truth.

## 2. Starting conditions

- Repository and origin `main` are exactly at clean EUI-M3-02 closeout `0affb7d`.
- EUI-M3-02 implementation is sealed at `c919238`.
- Incoming proof: full EditMode **1246 / 1246**, EchoUI Editor **140 / 140**, transition core **21 / 21**, Screen/Window lifecycle **10 / 10**, Modal transitions **10 / 10**, Laboratory **14 / 14**.
- PKG-LEARN-008 is complete.
- EUI-M1, M2, and M3 remain retained authority.
- No peer Echo package is required.

## 3. Learn → Declare → Authorize reconciliation

### 3.1 Learned boundary

HUD is persistent/reactive presentation, not gameplay state. A widget may display health, objectives, saves, dialogue, or diagnostics, but Looking Glass does not calculate, command, or persist that truth.

### 3.2 Declared intent

- Designers author stable HUD region IDs and ordering/capacity policy.
- Regions remain addressable independently of hierarchy paths.
- Widgets use fresh generation-bound registration leases.
- Visibility is reason/lease based so one caller cannot erase another caller's live request.
- Runtime overrides are session state and never mutate authored assets.
- HUD coexists with Screens and independent Windows.
- Existing authored external-context response remains available.

### 3.3 Authorized slice

EUI-M4-01 owns only named HUD regions, widget leases, visibility leases, ownership/loss settlement, bounded capacity, events/status, tests, and Laboratory proof.

## 4. Authority and invariants

1. Region and widget identity are stable-ID based and collision checked.
2. Definitions remain immutable during play.
3. Registration and visibility handles carry fresh generations and are idempotently disposable.
4. A stale handle cannot remove, hide, or reveal a newer replacement.
5. Effective visibility resolves from authored baseline, existing surface/context policy, and live visibility leases.
6. Releasing a lease restores remaining effective policy, not a hardcoded visible state.
7. Region/widget owner loss cleans only the owned live generation.
8. HUD mutations never enter Screen history or Modal/Window ordering.
9. Looking Glass owns no gameplay input, pause/time scale, cursor, domain values, persistence, or project lifetime composition.
10. Listener failure cannot roll back committed HUD truth.
11. Capacity and diagnostic history remain bounded.
12. Shutdown releases every HUD entry and settles handles exactly once.

## 5. Runtime scope

- `UIHudRegionId` and any neutral widget stable identity required by the contract.
- Immutable `UIHudRegionDefinition` with stable identity and ordering/capacity policy.
- Root-owned `UIHudRegionService`.
- Region-host registration within established RootOwned, SceneOwned, and ExternalOwned boundaries.
- Widget registration requests and structured results.
- Fresh generation-safe widget handles.
- Reason/owner visibility leases with deterministic priority/policy resolution.
- Availability/visibility events emitted after committed truth.
- Side-effect-free HUD status in the existing status snapshot.
- Reset, shutdown, and owner-loss cleanup.
- Existing transition seam only where lifecycle settlement requires it; no new driver is authorized.

## 6. Automated proof

At minimum prove stable registration/lookup; duplicate rejection without partial mutation; widget ordering and capacity; idempotent disposal; stale-generation safety; two visibility leases released out of order; authored baseline restoration; SceneOwned/ExternalOwned cleanup; no Screen/Modal/Window mutation; shutdown cleanup; listener isolation; and full retained EchoUI regression.

## 7. Laboratory proof

Add an **M4-01 HUD Regions** tab before retained tabs. Demonstrate named regions, multiple widgets, overlapping visibility reasons, out-of-order release, stale-handle rejection, owner-loss cleanup, idle quiescence, and retained M3-02/M3-01/M2/M1 smoke.

## 8. Explicit exclusions

This checkpoint does not authorize notification queueing/coalescing/timing; tooltip placement; prompt arbitration; Motifs/accessibility; safe-area adaptation; full Window LIFO/pinning/z-order/drag/resize/layout/persistence; Primitive Warehouse; 9-slice library; Panel/Menu templates; Template Catalog; Assembly Utilities; Builder/Composer; peer bridges; project-wide lifetime composition; gameplay input; pause/time scale; cursor; settings/save/scene/audio/localization/domain authority; showcase art; integration; or release qualification.

## 9. Expected files

Exact names may receive compile-safe refinement without changing authority:

- `Packages/com.echodevgames.echo-ui/Runtime/Hud/`
- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- existing status snapshot/runtime-state files as required
- focused Editor tests under `Packages/com.echodevgames.echo-ui/Tests/Editor/`
- Laboratory README/driver in package `Samples~` and imported sample
- closeout documentation named below

Every new Unity asset requires its `.meta`.

## 10. Implementation sequence

1. Re-establish **1246 / 1246** full EditMode before Runtime edits.
2. Add pure IDs, requests, results, definitions, and handles.
3. Add region registration and duplicate/capacity validation.
4. Add generation-safe widget registration/disposal.
5. Add reason/owner visibility leases and effective-state resolution.
6. Integrate root initialization, reset, shutdown, events, and status.
7. Add focused tests; run EchoUI and full EditMode suites.
8. Extend Laboratory while preserving retained tabs and parity.
9. Obtain manual acceptance.
10. Seal implementation, reconcile documentation, commit, and push.

## 11. Validation gates

- Incoming full EditMode **1246 / 1246**
- Focused EUI-M4-01 tests all pass
- EchoUI Editor all pass
- Full EditMode all pass with zero failures
- `git diff --check` and cached equivalent clean
- Package/imported Laboratory parity verified
- Manual HUD Laboratory all checks pass
- Retained M3-02/M3-01/M2/M1 smoke passes
- Repository clean and origin synchronized at closeout

## 12. Failure and bounded fixes

Duplicate/missing regions reject without partial state. Overflow rejects deterministically. Owner loss removes only matching generations. Stale handles preserve current truth. Visibility conflicts expose their effective reasons. Listener exceptions are isolated after state commit. Existing M3-02 recovery governs any transition use. Compile/test corrections inside this contract are pre-approved.

## 13. Rollback

Before implementation sealing, restore only the EUI-M4-01 manifest if a red gate cannot be resolved inside scope. Do not rewrite retained M1–M3 history. No destructive project migration is authorized.

## 14. Documentation reconciliation

At closeout update this plan, package specification, suite/package Current Notes, Suite Graph Roadmap, Suite Health, ChatGPT handoff, package changelog, and Laboratory evidence. Preserve exact hashes and counts.

## 15. Commit plan

- Activation: `Activate EUI-M4-01 named HUD regions and widget leases`
- Implementation: `Implement EUI-M4-01 named HUD regions and widget leases`
- Closeout: `Close out EUI-M4-01 HUD region lifecycle`

## 16. Completion criteria and stop point

EUI-M4-01 completes only after automated and manual proof, retained regression, truthful documentation, clean repository, and push. Stop there. Do not begin notifications, prompts, tooltips, Motifs/accessibility, Window management, authoring libraries, Builder, bridges, integration, or release work without the next activation.

## 17. Named next direction

**EUI-M4-02 — Bounded Notification Channels, Priority, Coalescing, Overflow, and Unscaled Lifetime** is named direction only and is not activated.

## 18. Handoff

EUI-M4-01 is ACTIVE / AUTHORIZED from `0affb7d`. Activation `ce30ac6`, Runtime/tests `df9e2be`, and bounded corrections through `e47d43b` are complete. The requested focused/full automated gate is user-confirmed green. Resume at the Laboratory/manual-proof slice; then seal implementation and reconcile closeout documentation. EUI-M4-02 remains inactive.

## 19. Implementation phase evidence — August 16, 2026

- Activation commit: `ce30ac6`.
- The incoming gate exposed an M3 transition timing regression; bounded test-only stabilization: `dbdf6bd`.
- Runtime and focused Editor tests: `df9e2be`.
- HUD-key compile correction: `81f9625`.
- Test LogAssert import correction: `3992bbc`.
- Observer-exception assertion correction: `e47d43b`.
- Jesse confirmed the requested focused/full automated gate green after `e47d43b`.
- Exact post-M4 NUnit totals are not yet committed; retained `1246 / 1246` evidence belongs to the pre-M4 floor.
- Remaining active scope: EUI-M4-01 Laboratory/manual acceptance, retained smoke/parity, implementation sealing, and documentation closeout.
- Stop before EUI-M4-02 or any adjacent capability.
