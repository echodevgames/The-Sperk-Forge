# First Light - Current Notes

## Package State

- Package: First Light - Startup and Launch (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.17.0
- Unity baseline: `6000.3.8f1`
- Latest completed checkpoint: `FL-M5-R1` — package sample identity/hierarchy hardening and Splash Shake reconciliation
- Status: **FL-M5-R1 complete; First Light package implementation/sample is sealed and frozen for this pass; release-route qualification remains separately gated**

## Implemented Boundary

First Light now provides the approved startup authority, ordered startup execution, immutable launch reporting, startup-only presentation, image splashes, initial destination handoff, Setup preview/Apply/Repair, project Validator, Direct Scene development entry, Launch Simulator, one importable Standalone Test Laboratory, creation-time splash authoring, explicit project-owned foundation resolution, and optional bounded per-entry Splash Shake.

A1 additionally provides:

- Splash Only / Splash + Status;
- project-owned background;
- Allow Advancement;
- None / Pulse motion;
- Automatic / Skippable After Minimum / Wait For Input After Minimum;
- normal SplashSequence Inspector authoring;
- Setup creation-time authoring for newly-created SplashSequence assets.
- Splash Shake: `None`, `Subtle`, `Medium`, `Nightmare`; additive to Pulse and disabled by Reduced Motion.

A1-E1 additionally provides:

```text
Foundation > Asset Resolution
  Reuse Compatible Assets
  Create Project-Owned Setup
```

The default preserves prior compatible-candidate reuse. Create Project-Owned Setup creates missing canonical foundation targets beneath the requested Project Root while preserving explicit destination-scene reuse, compatible requested targets, incompatible-target blocking, freshness checks, and repeat-safe `NoChanges` convergence.

## Schema / Ownership Boundary

- SplashSequence schema remains `1`.
- EchoLaunchConfiguration schema remains `4`.
- Launch report schema remains `2`.
- `PreferredAudioClip` is optional project-owned intent metadata only.
- EchoLaunch owns no audio playback, save/persistence, project input binding, EventSystem/input-module choice, general effects framework, menus, or normal mid-game scene travel.

## Reconciled Sample / Showcase Organization

The earlier FL-M6-01 Gallery proof remains historical evidence, but the repository now keeps package-distributed proof separate from polished project-owned showcase content.

Current concrete First Light sample identity:

```text
Packages/com.echodevgames.echo-launch/Samples~/FirstLight_Boot_Splash_Laboratory/
Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/
```

The Package Manager display name is **First Light Boot Splash Laboratory**. Its conceptual role remains the First Light Standalone Test Lab. The final imported copy is synchronized with the package-owned source and contains the EchoDevGames splash, revised First Light art, Boot/Destination Laboratory scenes, camera plumbing, and authored Splash Shake proof.

UMBRA remains separate project-owned **First Light showcase** content. It is not a package sample and does not satisfy or replace First Light's independently importable Laboratory requirement. The current repository retains UMBRA beneath `Assets/EchoDevGames/SuiteShowcase/First Light/UMBRA Example/**`; future polished showcases remain organized by package conceptually under `Assets/Showcases/<Package>/<Showcase>/`.

## Retained Evidence

```text
FL-M5-07 retained automated baseline: 809 / 809
FL-M5-07 manual Laboratory matrix:     12 / 12
FL-M6-01-H1 focused identity gate:       5 / 5
FL-M6-01-H2 focused destination gate:   35 / 35
Final EchoLaunchSetup filtered gate:   224 / 224
UMBRA repeated Apply:                 NoChanges
```

FL-M6-01 did not claim a fresh complete aggregate. FL-M5-R1 later supplied a fresh full EditMode gate; a fresh Runtime Play Mode aggregate still remains deferred to release qualification.

FL-M5-R1 later produced a fresh post-reconciliation full EditMode gate of **1106 / 1106 passed, 0 failed**. No new Runtime Play Mode aggregate is claimed by R1.

## Completed FL-M6-01 Commits

- `a70e478` — authorize A1-E1 project-owned Setup foundation resolution
- `9e6df00` — add First Light Setup splash creation authoring
- `e66b9fd` — add project-owned foundation resolution to First Light Setup
- `ccb1d59` — organize First Light reference examples into gallery
- `ad12b27` — remove obsolete First Light showcase folder metadata

A1 implementation also includes the preceding committed presentation/data/Inspector slices.

## FL-M5-R1 Closeout

- Activation: `93182c5`.
- Final implementation: `cea876e`.
- Commit scope: `174 files`, `3703 insertions`, `47 deletions`.
- Package sample path: `Samples~/FirstLight_Boot_Splash_Laboratory/`.
- Package Manager display name: `First Light Boot Splash Laboratory`.
- Old live sample identity is absent.
- Serialized destination paths and sample-test expectations use the new imported sample identity.
- Final package/imported Laboratory parity was synchronized across `78` files.
- Splash Shake presets: `None`, `Subtle`, `Medium`, `Nightmare`; `None = 0`; no SplashSequence schema bump.
- Shake begins on Hold, affects only the local splash presentation surface, and Reduced Motion suppresses it completely.
- Post-reconciliation full EditMode gate: **1106 / 1106 passed, 0 failed**.
- Final `Nightmare` runtime presentation was manually confirmed visible and working.

## Official Distribution Kit

First Light is the first package using the suite-wide versioned Distribution Kit standard.

Repository path:

```text
Distributions/First Light/0.1.0/
```

The kit contains the exact `com.echodevgames.echo-launch-0.1.0.tgz` artifact plus:

- complete user handout;
- distribution manifest;
- SHA-256 integrity record;
- build record;
- short kit README.

The authoritative complete handout also lives in package documentation:

```text
Documentation~/User/Complete User Handout.md
```

The kit is deliberately separated from project-owned First Light showcase content. UMBRA showcase art/audio/scenes are not package dependencies and are not silently bundled into the UPM artifact.

**Qualification boundary:** artifact preparation is complete when the kit is built and retained. External clean-project tarball installation/removal/reinstall is still `Not run` until a later explicit release-qualification return.

## Release Boundary

FL-M5-R1 closes the bounded post-M5 sample-identity/Splash-Shake reconciliation. The earlier FL-M6-01 Package Reference Showcase evidence remains retained history.

Still not claimed by this closeout:

- clean-project reproduction;
- supported Git/tag/public-registry installation;
- supported clean-project tarball installation (artifact prepared separately; route proof still pending);
- player-build qualification;
- performance qualification;
- release version/tag/catalog;
- private beta/external adoption.

## Next Action

No additional First Light feature implementation is active. FL-M5-R1 is complete at `cea876e`, the package sample is restored and synchronized, and First Light is sealed for this pass. Future clean-project, distribution-route, player-build, release, or private-beta work requires an explicit return to this package.
