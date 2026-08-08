# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 8, 2026
**Current focus:** First Light FL-M6-01 Production Reference Showcase authority
**Current checkpoint:** FL-M6-01 — First Light Production Reference Showcase

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current State

- Suite Package Reference Showcase graduation authority is committed at `8c3f3b3`.
- First Light FL-M5-07 is complete and closed at `710aec3`.
- FL-M5-07 final automated evidence: `809 / 809`.
- FL-M5-07 manual Laboratory evidence: `12 / 12`.
- First Light package version remains `0.1.0`.
- First Light package specification advances to v1.14.0 in this authority checkpoint.
- FL-M6-01 is authority/documentation only until its commit is synchronized and a fresh implementation drift audit passes.

## First Light Display-Case Goal

```text
FirstLight_Showcase_Boot
→ EchoDevGames / studio image splash
→ First Light — Startup and Launch image splash
→ valid startup sequence settles
→ destination validation + load
→ FirstLight_Showcase_MainMenu
```

The display case should be simple to watch because the complexity now lives underneath it.

## Reference Showcase Boundary

Canonical project-owned root:

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/
```

The Showcase:

- is project-owned and remains outside `Packages/**` and `Samples~/**`;
- uses documented First Light Setup and public configuration/presentation surfaces;
- uses two project-owned image splashes;
- uses an empty valid startup sequence for the smallest real consumer happy path;
- uses a project-owned main-menu-style destination without transferring menu authority to First Light;
- keeps diagnostics secondary;
- uses no sample-only helper, test assembly, hidden API, reflection, or peer package;
- contains no First Light-owned audio playback. Each splash entry may carry an optional project-owned `PreferredAudioClip` as stored presentation intent for a future Jukebot bridge.

## Splash Audio Intent

`[DECISION]` A `SplashEntry` may remember which project-owned `AudioClip` the designer intends to accompany that splash even before Jukebot exists.

```text
SplashEntry
├── Image
├── Timing / fade / skip policy
└── PreferredAudioClip   ← optional content intent only
```

First Light owns the **choice/reference**, not playback.

A future First Light ↔ Jukebot bridge can read the entry stable ID and preferred clip, create/resolve the appropriate Jukebot cue, then ask Jukebot to play it. Mixer routing, volume, variations, concurrency, fades, voice management, and actual `AudioSource` work remain Jukebot authority.

The field is optional backward-compatible metadata. Null means “no audio selected yet” and changes no current launch behavior.

## Starter Splash Convenience Question

A one-click starter splash template/preset is **not** pre-authorized.

We first use the real consumer path:

```text
Setup
→ project-owned SplashSequence
→ assign project-owned images/timing
→ project-owned root/presentation
→ Play
```

If our own display case cannot be built comfortably through that workflow, stop and capture the usability evidence before authorizing a convenience improvement.

## Acceptance

`SHOW-001` through `SHOW-009` prove the display case, public Setup/no-op rerun, ordered splashes, clean destination handoff, stored optional splash-audio intent, project ownership, and retained automated regression.

## Official Graduation Loop

```text
Learning / authority
→ implementation
→ Standalone Test Lab
→ Package Reference Showcase   ← First Light is here
→ clean-project reproduction
→ release qualification
→ private beta / external adoption
```

FL-M6-02 will reproduce the same happy path in a genuinely clean consumer project.

## Suite Cosmology Note

The future suite showcase/navigation identity remains intentionally unnamed. Emerging visual language is a constellation forming a humanoid-ish computational/cosmic entity, with completed systems mapping to functions of that body. This is creative direction only, not FL-M6-01 implementation authority.

## Next Action

1. Commit/push First Light specification v1.14.0 and FL-M6-01 authority.
2. Verify `main == origin/main` and a clean tree.
3. Run the fresh FL-M6-01 implementation drift audit.
4. Only then create showcase assets.
