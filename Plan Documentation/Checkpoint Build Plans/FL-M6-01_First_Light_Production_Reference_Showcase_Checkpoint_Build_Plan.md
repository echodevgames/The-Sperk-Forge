# FL-M6-01 — First Light Production Reference Showcase Checkpoint Build Plan

**Document role:** SFGSS-005 Checkpoint Build Plan
**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Checkpoint:** `FL-M6-01`
**Status:** Approved for implementation after authority commit
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.14.0
**Suite showcase authority:** SFGSS-ADR-005
**Unity baseline:** `6000.3.8f1`
**Repository baseline:** exact clean `8c3f3b3` before this authority commit
**Previous completed checkpoint:** FL-M5-07 at `710aec3`
**Retained automated baseline:** `809 / 809`
**Manual Standalone Laboratory baseline:** `12 / 12`
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 8, 2026

---

## 1. Contract

Create First Light's first project-owned Package Reference Showcase and prove that a normal developer can understand the package's real happy path through its existing public consumer surfaces.

This checkpoint is the display case, not another engineering Laboratory.

## 2. Visible Happy Path

```text
FirstLight_Showcase_Boot
        ↓
EchoDevGames / studio image splash
        ↓
First Light — Startup and Launch image splash
        ↓
valid startup sequence settles
        ↓
destination validation + load
        ↓
FirstLight_Showcase_MainMenu
```

## 3. Project-Owned Root

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/
├── README.md
├── Art/
│   ├── EchoDevGames_StudioSplash.png
│   └── FirstLight_ShowcaseSplash.png
├── Configuration/
│   ├── EchoLaunchConfiguration.asset
│   ├── StartupSequence.asset
│   ├── LaunchDestination.asset
│   └── SplashSequence.asset
├── Prefabs/
│   └── EchoLaunchRoot.prefab
└── Scenes/
    ├── FirstLight_Showcase_Boot.unity
    └── FirstLight_Showcase_MainMenu.unity
```

## 4. Consumer-Workflow Rule

Allowed:

- public First Light Setup;
- public ScriptableObject/Inspector authoring;
- supported project-owned prefab-variant customization;
- ordinary project-owned Unity scene/UI/art authoring;
- public Validator/report surfaces.

Forbidden:

- Laboratory sample-only helpers;
- test assemblies;
- hidden/internal package methods;
- hand-editing YAML to bypass a consumer workflow problem;
- reflection against package internals;
- repository-only GUID/prefab tricks;
- peer Sperk’s Forge runtime packages;
- package Runtime/Editor edits other than the exact additive `SplashEntry.PreferredAudioClip` metadata field and focused tests authorized below.

If the display case cannot be built comfortably without a forbidden path, stop. That is usability evidence.

## 5. Presentation

Create two project-owned in-house splash images:

1. EchoDevGames / studio identity.
2. First Light — Startup and Launch display identity.

They are showcase art, not package defaults.

The successful path should visually prioritize the splash images. Diagnostics may remain available for development but must not dominate the default display.

No First Light-owned audio playback is added. A splash entry may store one optional project-owned `PreferredAudioClip` reference so the intended sound can be selected now and consumed later by a Resonance/Jukebot bridge.

## 5.1 Deferred Splash Audio Intent

Add exactly one optional Runtime data field to `SplashEntry`:

```text
AudioClip PreferredAudioClip
```

Semantics:

- nullable and optional;
- project-owned asset reference;
- robust Unity object reference rather than a filesystem path string;
- expresses “this is the sound intended for this splash”;
- does not cause EchoLaunch to play audio;
- does not create a Jukebot package dependency;
- does not add volume, mixer, routing, variation, concurrency, loop, spatial, or fade ownership to First Light;
- remains null on existing assets unless a designer chooses a clip.

`SplashSequence` remains schema version `1` because the new field is optional, backward-compatible metadata that current playback does not require. Existing schema-1 assets remain valid and require no migration.

Focused tests must prove:

1. a `SplashEntry` accepts/returns a project-owned `AudioClip` reference;
2. null remains valid;
3. assigning a preferred clip does not create playback or change splash timing/result behavior;
4. existing Laboratory/sample assets remain valid without audio references.

Future Jukebot integration is expected to use the entry stable ID plus this preferred clip to author/resolve a real Jukebot cue, then request playback through Jukebot. First Light never performs that playback itself.

## 6. Startup Sequence

Use an empty but valid project-owned `StartupSequence`.

Reason: this is the smallest real First Light consumer use case. The Standalone Laboratory already proves step behaviors.

## 7. Destination

Create a clean project-owned main-menu-style destination scene at:

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_MainMenu.unity
```

It is presentation only. Do not introduce reusable menu architecture, save selection, settings, or normal scene-flow authority.

## 8. Setup Request

Use public First Light Setup with:

```text
Project Root:
Assets/EchoDevGames/SuiteShowcase/FirstLight

Boot Scene:
Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_Boot.unity

Destination Scene:
Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_MainMenu.unity

Create Splash Sequence:
Enabled

Build Settings Policy:
Add If Missing At End
```

Preview first, apply once, then repeat the identical request. The repeat result must be `NoChanges`.

## 9. Starter Splash Convenience Question

Do not build a generator/preset in advance.

First use:

```text
Setup
→ SplashSequence
→ assign project-owned images/timing/policy
→ project-owned presentation
→ Play
```

Classify the experience:

- Good enough: document and proceed.
- Awkward but understandable: record for later Quick Start/preset consideration.
- Requires hidden manipulation or is unreasonable for the intended novice path: stop and draft bounded consumer-ergonomics authority before FL-M6-02.

## 10. Implementation Slices

### Slice A — Deferred audio-intent data seam
Add only `SplashEntry.PreferredAudioClip` and focused tests. Run focused splash tests and inspect the package diff before any showcase asset work.

### Slice B — Destination + shell
Create the project-owned showcase root, README scaffold, art folders, and `FirstLight_Showcase_MainMenu.unity`.

### Slice C — Public Setup foundation
Use First Light Setup to create/reuse configuration, startup sequence, launch destination, splash sequence, root prefab, Boot scene, and reviewed Build Settings entry. Prove identical rerun `NoChanges`.

### Slice D — Splash art + authoring
Create the two project-owned images and configure them through the public SplashSequence authoring surface.

### Slice E — Front-facing acceptance
Play the Boot scene and prove ordered splashes, completed handoff, and clean destination presentation.

### Slice F — Regression + cleanup
Run retained focused tests, complete EditMode, complete Runtime Play Mode, inspect Git ownership, remove temporary acceptance residue, and preserve the actual Reference Showcase.

## 11. Acceptance Matrix

| ID | Action | Expected result |
|---|---|---|
| SHOW-001 | Inspect starting repository/showcase path | Clean synchronized baseline; no accidental prior display case |
| SHOW-002 | Preview/apply public Setup request | Ready plan; only approved project-owned foundation created/reused |
| SHOW-003 | Repeat identical Setup | `NoChanges`; no duplicates or Build Settings drift |
| SHOW-004 | Play showcase Boot | Studio splash then First Light splash, visibly ordered |
| SHOW-005 | Continue launch | Startup settles; destination validates/loads; handoff completes |
| SHOW-006 | Inspect destination | Clean main-menu-style display; no Laboratory diagnostic wall |
| SHOW-007 | Inspect Git ownership | Showcase under project `Assets/**`; package Runtime/Editor unchanged |
| SHOW-008 | Run retained regression | Existing First Light automated baseline stays green; additive focused tests may raise totals |
| SHOW-009 | Assign/inspect preferred audio intent | Optional project-owned `AudioClip` reference is retained per splash entry; no playback occurs without a later bridge/provider |

## 12. Automated Baseline

At minimum retain:

```text
Focused Standalone Laboratory package tests: 6 / 6
Focused Standalone Laboratory asset tests:   8 / 8

Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
```

Additive project-owned showcase tests may increase totals. Existing tests may not regress.

## 13. Explicitly Out of Scope

- package Runtime/Editor code changes other than `SplashEntry.PreferredAudioClip` plus focused tests;
- new schema or diagnostic code;
- starter splash generator/preset;
- package sample changes;
- clean-project reproduction;
- tarball/Git install proof;
- player builds;
- performance claims;
- package version bump;
- release tag/catalog;
- private beta/external adoption;
- audio integration;
- Passage/Looking Glass/other package integration;
- reusable Main Menu implementation;
- Suite Showcase Hub implementation or final lore naming.

## 14. Stop Conditions

Stop if:

- Setup cannot target the project-owned showcase root;
- splash authoring requires hidden API/YAML manipulation;
- successful presentation cannot reasonably become front-facing through public serialized surfaces;
- Setup mutates package source/unrelated content;
- package Runtime/Editor changes beyond the exact `PreferredAudioClip` metadata seam appear necessary;
- retained regression turns red for unexplained reasons;
- another Sperk’s Forge package becomes required.

## 15. Closeout

On success:

- update root and package Current Notes;
- update package Documentation Index;
- add package checkpoint/test-showcase records;
- add Plan Documentation completion record;
- update README/Quick Start only from the proven real workflow;
- record any consumer ergonomics issue separately;
- commit/push implementation and docs in bounded adjacent commits.

## 16. Explicit Stop Point

When `SHOW-001` through `SHOW-009` pass and the Reference Showcase is committed:

**STOP.**

Do not begin FL-M6-02 automatically.

FL-M6-02 reproduces this exact happy path in a genuinely clean consumer project.
