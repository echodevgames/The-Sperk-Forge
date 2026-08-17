# Changelog

## [Unreleased]

### Added

- EUI-M1-01 installable Looking Glass package foundation.
- Duplicate-safe package-local UI authority.
- Stable surface registry with Screen, Window, HUD, and Overlay roles.
- Optional exclusive navigation scopes with history-based Back.
- Independent surface open/close/toggle operations.
- Thin uGUI navigation-button adapter.
- Focused Editor tests and minimal Foundation Laboratory instructions.
- EUI-M1-02 project-defined active/inactive UI context state and simultaneous-context support.
- Designer-ordered per-surface context response rules with independent visibility, interaction, and selection dimensions.
- Per-surface external-context opt-out and transient runtime overrides without persistence ownership.
- Externally supplied pointer/navigation modality with configurable selection-on-open behavior.
- Neutral close-selection behavior with no implicit historical focus restoration.
- Expanded Looking Glass Laboratory proof with top-right proof-console safe zone and a real `Button_DefaultClose` selection target.
- Seventeen focused EUI-M1-02 tests while retaining all seven EUI-M1-01 foundation tests.

- EUI-M2-01 project-defined ordered layer topology addressed by stable IDs with no fixed runtime layer count.
- Authoritative Screen lifecycle with explicit `RootOwned`, `SceneOwned`, and `ExternalOwned` view ownership.
- Deterministic Push/Navigate, Replace, Reset/Return-to-root, Back, and Close operations with structured terminal results.
- Designer-controlled suspended-Screen visibility while suspended Screens remain non-interactive within their navigation scope.
- Bounded strict-FIFO structural Screen operation admission and settlement with explicit rejection rather than silent reorder/coalesce/drop.
- Expanded Looking Glass Laboratory proof covering Screen ownership/lifetime, suspension policies, history semantics, custom layers, FIFO settlement, and retained M1 behavior.
- Twenty-three focused EUI-M2-01 tests, bringing the focused EchoUI assembly to 47 / 47 and the final full EditMode floor to 1153 / 1153.
- EUI-M2-02 deterministic stacked blocking Modal lifecycle with top-only Looking Glass interaction.
- Project-defined stable Modal/result IDs, fresh per-opening handles, first-terminal-wins exact-once settlement, and structural `Aborted` outcomes.
- `RootOwned`, `SceneOwned`, and `ExternalOwned` Modal lifetime behavior without stealing external object lifetime.
- Designer-authored modal Back policy plus explicit Screen mutation `Reject` and bounded `DeferUntilModalStackClears` behavior.
- Lower Looking Glass UI blocking without claiming gameplay-input, pause/time-scale, cursor, or simulation authority.
- Explicit separation between blocking Modal behavior and independent coexistent Window behavior.
- Twenty-eight focused EUI-M2-02 tests, bringing the EchoUI EditMode assembly to 75 / 75 and the final Foundry EditMode floor to 1181 / 1181.
- Expanded Laboratory proof with 12 / 12 manual Modal acceptance plus retained M2-01 Screens and M1 behavior.
- EUI-M3-01 explicit/non-destructive EventSystem coordination through `AdoptAssigned`, deterministic `AdoptExisting`, `CreateIfMissing`, and `RequireExternal`, including structured ambiguity/missing diagnostics.
- Policy-aware focus lifecycle with per-live-entry memory, optional transient stable-surface session memory, designer-controlled fresh/reopen behavior, deterministic fallback, and legal no-focus.
- Screen resume/Back and blocking-Modal focus restoration/containment while independent Windows retain distinct focus memory without enabling the future Window manager.
- Event-driven focus maintenance with explicit revalidation and stale-generation rejection; no universal per-frame focus scan, peer Echo dependency, or generated `InputSystem_Actions` wrapper dependency.
- Twenty-four focused EUI-M3-01 tests, bringing the EchoUI EditMode assembly to 99 / 99 and the final Foundry EditMode floor to 1205 / 1205.
- Expanded Laboratory proof with 12 / 12 manual focus/EventSystem acceptance, bounded idle performance evidence, retained M2-02/M2-01/M1 smoke checks, and synchronized package/imported Laboratory parity.


## EUI-M3-02 FINAL CLOSEOUT COMPLETE

- Added replaceable authoritative enter/exit transition drivers for Screens, blocking Modals, and independent Windows.
- Added deterministic transition failure, timeout, cancellation, and stale recovery.
- Added root/default, definition-profile, and transient override policy layering.
- Added reduced-motion Immediate substitution and unscaled CanvasGroup fade timing.
- Preserved Modal exact-once terminal results and deferred Screen wait-through-exit semantics.
- Corrected synchronous token-cancellation settlement racing fallback Unity `Awaitable` cancellation.
- Added retained automated coverage and a 14-check Laboratory transition proof.
- Final evidence: **1246 / 1246** full EditMode and **140 / 140** EchoUI Editor.
- Implementation sealed at `c919238`; no next checkpoint activated.

## EUI-M4-01 FINAL CLOSEOUT COMPLETE

- Added stable project-defined named HUD regions with deterministic lookup, ordering, and bounded capacity.
- Added generation-safe widget registration leases and reason/owner visibility leases with idempotent disposal and stale-handle protection.
- Added deterministic effective visibility, owner-loss/shutdown cleanup, listener isolation, status snapshots, and events while preserving Screen/Modal/Window independence.
- Added focused Editor coverage. Runtime/tests landed at `df9e2be`; bounded corrections run through `e47d43b`.
- Added the mirrored Laboratory proof at `29573ef`.
- The requested focused/full automated gate is user-confirmed green; exact post-M4 NUnit totals were not captured, and retained `1246 / 1246` remains the pre-M4 floor.
- Manual HUD Laboratory **5 / 5 PASS**, retained M3-02/M3-01/M2-02/M2-01/M1 smoke user-confirmed green, and package/imported parity verified.
- EUI-M4-01 is complete. Its historical stop point did not activate EUI-M4-02; the later EUI-M4-02 section supersedes that status.

## EUI-M4-02 FINAL CLOSEOUT COMPLETE

- Added project-defined stable notification channels with independent visible/pending bounds.
- Added deterministic higher-priority/FIFO-tie pending promotion without visible preemption.
- Added channel-scoped fresh-generation coalescing, stale-handle safety, and visible lifetime restart.
- Added `RejectNewest`, `DropOldestPending`, and strict-outrank `ReplaceLowestPriorityPending` overflow policies.
- Added unscaled automatic lifetime, manual lifetime/dismissal, owner/presentation-loss cleanup, reset/shutdown settlement, status snapshots/events, root integration, and a replaceable presenter seam.
- Runtime/root/presenter implementation is accepted through `d93d0bd`.
- Activation baseline: full Foundry EditMode **1258 / 1258** and EchoUI Editor **152 / 152**.
- Final automated evidence: full Foundry EditMode **1383 / 1383**, EchoUI Editor **277 / 277**, aggregate notification fixtures **125 / 125**, presenter fixture **17 / 17**, zero failed/skipped/inconclusive.
- Added mirrored Laboratory configuration and a sample-owned plain reference presenter at `bde34f2`, with six bounded manual checks and all retained tabs.
- Manual notification Laboratory **6 / 6 PASS**; 180-frame idle notification/presenter quiescence PASS; structural UI truth unchanged.
- Retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke user-confirmed green; package/imported parity verified; submitted Unity screenshots show zero Console errors/warnings.
- EUI-M4-02 is complete. No successor Looking Glass checkpoint is activated.
