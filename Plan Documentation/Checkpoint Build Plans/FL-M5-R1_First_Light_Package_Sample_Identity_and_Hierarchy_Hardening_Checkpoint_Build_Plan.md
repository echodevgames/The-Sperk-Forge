# FL-M5-R1 — First Light Package Sample Identity and Hierarchy Hardening — Checkpoint Build Plan

**Package:** First Light (`EchoLaunch`)
**Package ID:** `com.echodevgames.echo-launch`
**Checkpoint:** FL-M5-R1
**Type:** Bounded post-M5 reconciliation / package-sample identity hardening
**Status:** COMPLETE
**Planning baseline:** `785df73` — `Close out ESV-M5-06 minimal Chronicle Save Laboratory`
**Activation commit:** `93182c5` — `Activate FL-M5-R1 First Light sample identity hardening`
**Implementation commit:** `cea876e` — `Implement FL-M5-R1 sample identity hardening and splash shake`
**Unity baseline:** 6000.3.8f1

## 1. Purpose

Preserve First Light's already-complete package-owned Standalone Test Lab while permanently hardening the package-sample versus project-showcase boundary.

This checkpoint does **not** rebuild the First Light Lab.

The existing Lab remains the engineering proof asset. Its scenes, configurations, prefab, runtime sample assembly, sample steps, art, metadata/GUIDs, test intent, and behavior must be preserved.

## 2. Canonical package-sample identity

The package-owned importable sample becomes:

`Packages/com.echodevgames.echo-launch/Samples~/FirstLight_Boot_Splash_Laboratory/`

`package.json` must declare the same path.

The human-facing Package Manager sample display name becomes:

`First Light Boot Splash Laboratory`

The conceptual role remains the **First Light Standalone Test Lab**. The rename clarifies the concrete sample identity and does not narrow the existing engineering acceptance scope.

## 3. Canonical project organization

Project-owned organization remains conceptually:

```text
Assets/
├─ Showcases/
│  └─ First Light/
│     └─ UMBRA/
└─ Samples/
   └─ FirstLight/
      └─ FirstLight_Boot_Splash_Laboratory/
```

Unity Package Manager may initially import samples beneath its generated `Assets/Samples/<Package>/<Version>/...` wrapper. Project organization may move the imported copy afterward without changing package ownership.

## 4. Package sample versus showcase boundary

Package samples and polished showcases are separate artifacts.

### Package sample
- package-owned under `Samples~`;
- shipped with the package;
- independently importable/removable;
- proves the package in isolation;
- may contain engineering controls, readouts, fixtures, and redistributable sample assets.

### Showcase
- project-owned;
- polished consumer/composition example;
- organized by package under `Assets/Showcases/<Package>/<Showcase>/`;
- may consume multiple packages where appropriate;
- never substitutes for the package-owned sample requirement.

UMBRA is specifically a **First Light showcase**, not First Light's package sample.

## 5. Implementation scope

Implementation is limited to:

1. rename
   `Samples~/First Light Standalone Test Lab/`
   to
   `Samples~/FirstLight_Boot_Splash_Laboratory/`;

2. rename the adjacent folder `.meta` file with the folder so the existing Unity folder GUID is preserved;

3. update `Packages/com.echodevgames.echo-launch/package.json`:
   - sample display name;
   - sample path;

4. preserve every file beneath the Lab unchanged unless a later explicit documentation reconciliation requires wording changes;

5. do not modify First Light Runtime, Editor, Presentation, or test behavior.

## 6. Preservation requirements

The rename must preserve:

- `FirstLight_Boot_Lab.unity`;
- `FirstLight_Destination_Lab.unity`;
- all configuration assets;
- all step assets;
- all sample runtime scripts;
- sample assembly definitions;
- splash art;
- prefab(s);
- README;
- every existing `.meta` file and GUID.

No asset regeneration is authorized.

## 7. Camera convention

Human-inspected sample/Test Lab scenes should contain a basic scene-owned camera when needed to avoid Unity's `No cameras rendering` Game-view state.

This is scene plumbing only. It must not create reusable camera architecture or a new package dependency.

Any camera adjustment is a separately visible scene change and is not required merely to perform the sample-folder rename.

## 8. Explicit exclusions

FL-M5-R1 does not authorize:

- First Light Runtime API changes;
- First Light Editor tooling changes;
- presentation-system redesign;
- Looking Glass integration;
- Resonance integration;
- UMBRA content changes;
- Chronicle content changes;
- new startup features;
- new Lab scenarios;
- sample asset regeneration;
- deleting the Standalone Test Lab;
- replacing the Lab with UMBRA or another showcase.

## 9. Validation

Before commit:

1. clean baseline `785df73`;
2. old sample path absent;
3. new sample path present;
4. package sample folder `.meta` preserved under the new name;
5. `package.json` points only to `Samples~/FirstLight_Boot_Splash_Laboratory`;
6. no First Light Runtime source changed;
7. Git whitespace validation passes;
8. Unity recompiles;
9. Package Manager recognizes `First Light Boot Splash Laboratory`;
10. imported sample still opens and exercises the existing Boot/Destination Lab behavior.

## 10. Stop point

Stop after package sample identity/hierarchy hardening is committed, pushed, and documented.

Do not begin Chronicle M6 or new First Light package work as part of this checkpoint.

## Approved Scope Amendment - Splash Shake

During FL-M5-R1 final sample verification, one bounded optional First Light presentation flourish was explicitly approved.

- `SplashEntry` may author `SplashShakePreset.None`, `Subtle`, `Medium`, or `Nightmare`.
- Shake affects only First Light's startup splash presentation surface, never the gameplay camera.
- Shake is additive to the existing `SplashMotionStyle` setting, so Pulse and Splash Shake may be combined.
- Reduced Motion suppresses Splash Shake completely.
- `None = 0`, preserving all existing serialized entries without a schema bump.
- The normal `SplashSequence` Inspector exposes Splash Shake beside the existing Motion/Pulse controls.
- Setup-generated entries default to None and can be tuned on the resulting `SplashSequence` asset.
- No Looking Glass, Accord, Resonance, feedback-system, camera-system, or input dependency is introduced.

This amendment does not authorize a general-purpose screen-shake service or gameplay feedback architecture.


## 11. Closeout

FL-M5-R1 is **Complete**.

Final evidence:

- activation `93182c5`;
- implementation `cea876e`;
- implementation commit scope `174 files / 3703 insertions / 47 deletions`;
- package source `Samples~/FirstLight_Boot_Splash_Laboratory/`;
- Package Manager display name **First Light Boot Splash Laboratory**;
- imported proof under Unity's normal `Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/` wrapper;
- old live `First Light Standalone Test Lab` concrete identity absent from package sample/test scope;
- package-folder GUID preserved through the rename;
- serialized Laboratory destination paths corrected to the new imported sample identity;
- final imported Laboratory synchronized back into package-owned `Samples~` with `78` files in parity;
- EchoDevGames splash, revised First Light art, scene camera plumbing, and final authored splash sequence retained in the shipped sample;
- `SplashShakePreset.None`, `Subtle`, `Medium`, and `Nightmare` implemented; `None = 0`; SplashSequence schema remains `1`;
- Shake remains additive to Pulse, startup-local, and completely suppressed by Reduced Motion;
- final visibility correction starts the shake on the fully-visible Hold phase and prefers the visible splash image transform;
- fourteen stale sample-identity test failures were reconciled to the hardened package identity;
- post-reconciliation full EditMode gate **1106 / 1106 passed, 0 failed**;
- final manual `Nightmare` playback visibly worked as intended;
- UMBRA remains separate project-owned First Light showcase content and does not replace the package sample;
- no Looking Glass, Accord, Resonance, gameplay-camera, global feedback-service, or input dependency was introduced.

The original no-Runtime/Editor-change exclusion in Sections 5 and 8 was superseded only by the explicitly approved Splash Shake amendment above. No broader presentation-system redesign or general screen-shake authority was authorized.

**Stop point reached:** no further First Light feature implementation is active. Future clean-project, distribution-route, player-build, release, performance, or private-beta work requires separate activation.
