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
