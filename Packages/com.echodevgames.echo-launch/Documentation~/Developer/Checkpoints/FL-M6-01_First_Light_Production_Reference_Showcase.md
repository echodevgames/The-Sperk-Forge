# FL-M6-01 — First Light Production Reference Showcase

**Status:** Complete
**Package:** First Light - Startup and Launch (`EchoLaunch`)
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.16.0
**Unity baseline:** `6000.3.8f1`
**Closeout baseline:** `ad12b27`
**Date:** August 8, 2026

## Objective

Prove the real First Light happy path through normal public consumer surfaces, then retain a project-owned reference example without converting showcase content into package dependencies or Laboratory helpers.

## Result

**PASS.**

The checkpoint produced and proved:

- a canonical First Light consumer example;
- optional stored splash audio intent without playback ownership;
- H1 blank SplashEntry identity authoring correction;
- H2 destination Build Settings conformance;
- A1 Splash Presentation & Authoring Expansion;
- Setup creation-time splash authoring;
- A1-E1 explicit project-owned foundation resolution;
- a second independent UMBRA consumer example;
- a permanent project-owned First Light Gallery.

## Permanent Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/First Light Splashs/**
└── UMBRA Example/UMBRA Splashs/**
```

The Gallery is outside `Packages/**` and `Samples~/**`.

## Consumer Defects Discovered and Resolved

### H1

Normal Inspector list authoring exposed blank hidden SplashEntry identities. The bounded Editor-only correction generates only blank IDs and preserves non-empty identities. Runtime and schema remain unchanged.

### H2

Setup could previously succeed while the configured destination was absent from Build Settings. The bounded Setup correction ensures required Boot/destination conformance and repeat-safe no-op behavior without weakening runtime validation.

### A1-E1

Creation-time authored splashes could have been accepted by the window while an unrelated compatible SplashSequence was reused. `Create Project-Owned Setup` now gives the user an explicit independent-foundation path while preserving legacy-compatible reuse as the default.

## Manual Acceptance

Canonical example:

```text
Boot
→ EchoDevGames splash
→ First Light splash
→ startup settles
→ destination validation/load
→ MainMenu
```

UMBRA example:

```text
fresh requested root
→ Create Project-Owned Setup
→ create requested foundation
→ author 3 splashes
→ runtime presentation succeeds
→ identical request
→ NoChanges
```

Generated UMBRA entry labels:

1. The Sperk
2. Isekai Studios
3. UMBRA

The generated sequence retained stable IDs, project-owned image/audio-intent references, presentation settings, timing, advancement, and Pulse metadata.

## Automated Evidence

- H1 focused identity tests: `5 / 5`
- H2 focused destination tests: `35 / 35`
- final `EchoLaunchSetup` filtered EditMode gate: `224 / 224`
- retained FL-M5-07 complete automated baseline: `809 / 809`
- retained FL-M5-07 manual Laboratory matrix: `12 / 12`

No new complete post-A1 EditMode/Runtime aggregate is claimed.

## Commits

- `a70e478` — authorize A1-E1 project-owned Setup foundation resolution
- `9e6df00` — add First Light Setup splash creation authoring
- `e66b9fd` — add project-owned foundation resolution to First Light Setup
- `ccb1d59` — organize First Light reference examples into gallery
- `ad12b27` — remove obsolete First Light showcase folder metadata

A1 presentation/data/Inspector implementation is retained in the immediately preceding committed slices.

## Boundary Preserved

First Light still does not own:

- audio playback;
- project input binding;
- save/persistence;
- EventSystem/input-module selection;
- reusable menus;
- normal mid-game scene flow;
- generalized effects;
- peer-package service behavior.

## Release Boundary

This checkpoint closes the in-repository Package Reference Showcase stage. It does not claim clean-project reproduction, supported external installation routes, player-build qualification, performance qualification, release tagging/catalog, or private beta.

## Stop

First Light implementation and Gallery work are frozen for this pass. Future release qualification requires explicit reactivation.
