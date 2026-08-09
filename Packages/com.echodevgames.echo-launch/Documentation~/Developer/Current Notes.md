# First Light - Current Notes

## Package State

- Package: First Light - Startup and Launch (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.16.0
- Unity baseline: `6000.3.8f1`
- Latest completed checkpoint: `FL-M6-01` — First Light Production Reference Showcase
- Status: **Implementation and Reference Gallery pass complete/frozen; versioned 0.1.0 Distribution Kit being prepared as a handoff artifact, with tarball route qualification still pending**

## Implemented Boundary

First Light now provides the approved startup authority, ordered startup execution, immutable launch reporting, startup-only presentation, image splashes, initial destination handoff, Setup preview/Apply/Repair, project Validator, Direct Scene development entry, Launch Simulator, one importable Standalone Test Laboratory, creation-time splash authoring, and explicit project-owned foundation resolution.

A1 additionally provides:

- Splash Only / Splash + Status;
- project-owned background;
- Allow Advancement;
- None / Pulse motion;
- Automatic / Skippable After Minimum / Wait For Input After Minimum;
- normal SplashSequence Inspector authoring;
- Setup creation-time authoring for newly-created SplashSequence assets.

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

## Final FL-M6-01 Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
│   └── First Light Splashs/
└── UMBRA Example/
    └── UMBRA Splashs/
```

### First Light Example

Proves the normal public Boot → EchoDevGames splash → First Light splash → startup settlement → MainMenu destination happy path.

### UMBRA Example

Proves independent consumer authoring through Create Project-Owned Setup. The generated SplashSequence retained three stable-ID entries (`The Sperk`, `Isekai Studios`, `UMBRA`), project-owned visual/audio-intent references, authored timing/advancement, and Pulse on the Isekai entry. The experience played successfully. The identical second Apply returned `NoChanges` with no created paths.

The Gallery is project-owned and remains outside the distributed package and Standalone Test Lab.

## Retained Evidence

```text
FL-M5-07 retained automated baseline: 809 / 809
FL-M5-07 manual Laboratory matrix:     12 / 12
FL-M6-01-H1 focused identity gate:       5 / 5
FL-M6-01-H2 focused destination gate:   35 / 35
Final EchoLaunchSetup filtered gate:   224 / 224
UMBRA repeated Apply:                 NoChanges
```

No post-A1 complete EditMode or Runtime Play Mode total is claimed by this closeout. Collect fresh full-suite totals at the next release-qualification gate.

## Completed FL-M6-01 Commits

- `a70e478` — authorize A1-E1 project-owned Setup foundation resolution
- `9e6df00` — add First Light Setup splash creation authoring
- `e66b9fd` — add project-owned foundation resolution to First Light Setup
- `ccb1d59` — organize First Light reference examples into gallery
- `ad12b27` — remove obsolete First Light showcase folder metadata

A1 implementation also includes the preceding committed presentation/data/Inspector slices.

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

The kit is deliberately separated from the project-owned First Light Gallery. Gallery art/audio/scenes are not package dependencies and are not silently bundled into the UPM artifact.

**Qualification boundary:** artifact preparation is complete when the kit is built and retained. External clean-project tarball installation/removal/reinstall is still `Not run` until a later explicit release-qualification return.

## Release Boundary

FL-M6-01 closes the Package Reference Showcase stage only.

Still not claimed by this closeout:

- clean-project reproduction;
- supported Git/tag/public-registry installation;
- supported clean-project tarball installation (artifact prepared separately; route proof still pending);
- player-build qualification;
- performance qualification;
- release version/tag/catalog;
- private beta/external adoption.

## Next Action

No additional First Light feature implementation is active. Finish and commit the versioned `0.1.0` Distribution Kit, then deliberately select the next suite package through its just-in-time learning/checkpoint workflow. Future First Light clean-project tarball/release work requires an explicit return to this package.
