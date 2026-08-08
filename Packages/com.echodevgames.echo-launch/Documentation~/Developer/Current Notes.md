# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M6-01`
- Title: First Light Production Reference Showcase
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.14.0
- Unity baseline: `6000.3.8f1`
- Suite showcase authority: SFGSS-ADR-005 / `8c3f3b3`
- Previous checkpoint: FL-M5-07 complete at `710aec3`
- Status: Authority prepared; implementation not started

## Retained Baseline

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8
Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
Manual LAB matrix:      12 / 12
```

## FL-M6-01 Visible Goal

```text
Boot
→ project-owned EchoDevGames splash
→ project-owned First Light display splash
→ valid startup sequence
→ destination load
→ clean main-menu-style destination
```

Target root:

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/
```

## Boundaries

- Existing public Setup/configuration/presentation surfaces first.
- Exact bounded package code authority: additive nullable `SplashEntry.PreferredAudioClip` metadata plus focused tests only; EchoLaunch still performs no audio playback.
- No sample change required.
- No clean-project proof yet.
- No Git/tarball/player/performance/release proof yet.
- No package version bump.
- No audio playback ownership. `SplashEntry.PreferredAudioClip` may store optional project-owned audio intent for a future Jukebot bridge.
- No starter splash generator/preset assumed.

## Acceptance

Run `SHOW-001` through `SHOW-009` and preserve every retained test; focused audio-intent tests may increase totals above `809`.

## Next Action

Commit/push FL-M6-01 authority, then perform a fresh drift audit before creating project-owned showcase content.
