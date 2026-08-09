# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 8, 2026
**Current focus:** First Light FL-M6-01 documentation closeout
**Current checkpoint:** FL-M6-01 — First Light Production Reference Showcase — **Complete**

> Durable First Light decisions live in SFGSS-PKG-ECHOLAUNCH-001 v1.16.0 and the committed checkpoint/amendment records. Git history preserves the longer working trail.

---

## Current State

- First Light package version remains `0.1.0`.
- First Light package specification remains **SFGSS-PKG-ECHOLAUNCH-001 v1.16.0**.
- FL-M5-07 Standalone Test Laboratory is complete at `710aec3` with retained automated evidence `809 / 809` and manual Laboratory evidence `12 / 12`.
- FL-M6-01 Production Reference Showcase implementation and in-repository consumer proof are complete.
- A1 splash presentation/authoring is committed across `1b7ab84`, `d36b5cc`, `90e038c`, `9b24121`, and `4bdc264`.
- Slice E Setup creation-time splash authoring is committed at `9e6df00`.
- A1-E1 project-owned foundation resolution is authorized at `a70e478` and implemented at `e66b9fd`.
- The permanent First Light Gallery is committed at `ccb1d59`; obsolete pre-gallery folder metadata is removed at `ad12b27`.
- The final `EchoLaunchSetup` filtered EditMode gate passed **224 / 224**.
- No post-A1 full-suite aggregate is claimed by this closeout. The retained FL-M5-07 full-suite baseline remains historical evidence and should be rerun at the next release-qualification gate.

## Permanent First Light Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
│   └── First Light Splashs/
│       ├── Art/
│       ├── Audio/
│       ├── Configuration/
│       ├── Prefabs/
│       └── Scenes/
└── UMBRA Example/
    └── UMBRA Splashs/
        ├── Art/
        ├── Audio/
        ├── Configuration/
        ├── Prefabs/
        └── Scenes/
```

The Gallery is project-owned consumer/showcase content. It is not package content, a UPM sample, a required dependency, or evidence that The Sperk’s Forge is an Isekai Studios product.

### First Light Example

The canonical in-house example demonstrates the normal First Light happy path:

```text
FirstLight_Showcase_Boot
→ EchoDevGames splash
→ First Light splash
→ startup settles
→ destination validates and loads
→ FirstLight_Showcase_MainMenu
```

It uses normal public Setup, project-owned configuration, normal Inspector authoring, stable splash identities, optional stored audio intent, and the public uGUI presentation path.

### UMBRA Example

The second Gallery example is the former Slice E proof promoted into a permanent consumer example. It proves the package is not secretly tied to the canonical First Light branding or foundation.

Observed proof:

- `Foundation > Asset Resolution = Create Project-Owned Setup`;
- fresh requested Configuration, LaunchDestination, SplashSequence, StartupSequence, RootPrefab, and Boot scene planned as `Create`;
- explicitly selected existing destination scene remained `Reuse`;
- first Apply created the requested project-owned foundation;
- generated SplashSequence retained **3** authored entries: `The Sperk`, `Isekai Studios`, and `UMBRA`;
- each generated entry received a non-empty stable ID;
- project-owned images, optional `PreferredAudioClip` intent, timings, motion, and advancement settings serialized correctly;
- presentation used Splash Only, black background, and advancement enabled;
- the Isekai entry retained Pulse (`1.05` maximum scale, `2.5` second cycle);
- the generated Boot experience played successfully and looked correct;
- identical second Preview reused the requested local targets;
- identical second Apply returned **`NoChanges`** with **Created paths: None** and unchanged Build Settings evidence.

Audio remains intent metadata only. First Light does not own audio playback.

## FL-M6-01 Defects Resolved by the Real Consumer Workflow

### H1 — Splash Entry Authoring Identity

Normal Inspector authoring originally left hidden blank `SplashEntry.entryId` values and runtime correctly blocked with `ELAUNCH-SPLASH-001`.

Resolved result:

- Editor-only blank-ID generation;
- existing non-empty IDs preserved;
- no Runtime rewrite;
- no schema bump;
- focused gate **5 / 5**;
- public Inspector workflow advanced beyond `ELAUNCH-SPLASH-001`.

### H2 — Destination Build Settings Conformance

Setup originally reported success while the configured destination was absent from Build Settings and runtime correctly blocked with `ELAUNCH-DEST-001`.

Resolved result:

- Setup ensures Boot and configured destination are enabled exactly once;
- unrelated Build Settings order remains preserved;
- repeat Apply settles `NoChanges`;
- focused H2 gate **35 / 35**;
- public Boot → splashes → destination path succeeded without manual Build Settings editing.

### A1 / A1-E1 — Presentation and Independent Setup Authoring

The Showcase then justified and proved:

- Splash Only / Splash + Status;
- project-owned background color;
- Allow Advancement;
- None / Pulse motion;
- Automatic / Skippable After Minimum / Wait For Input After Minimum;
- normal Inspector authoring;
- Setup creation-time authoring for newly-created sequences;
- backward-compatible `Reuse Compatible Assets`;
- explicit `Create Project-Owned Setup` for an independent requested foundation;
- request/plan freshness participation and repeat-safe convergence.

Schemas remain unchanged: SplashSequence schema `1`, EchoLaunchConfiguration schema `4`.

## Acceptance

FL-M6-01 acceptance is complete for the in-repository Package Reference Showcase stage.

```text
Learning / authority
→ implementation
→ Standalone Test Lab                PASS
→ Package Reference Showcase         PASS
→ clean-project reproduction         NOT RUN in this closeout
→ release qualification              NOT RUN
→ private beta / external adoption   NOT RUN
```

The permanent Gallery now supplies two consumer examples that can be extended later without widening First Light package authority.

## Evidence Boundary

This closeout does **not** claim:

- a post-A1 complete EditMode aggregate;
- a post-A1 complete Runtime Play Mode aggregate;
- clean-project reproduction;
- Git URL, tag, tarball, registry, or public-package installation support;
- player-build qualification;
- performance qualification;
- release tag/catalog readiness;
- private beta or external adoption;
- First Light-owned audio playback.

Those remain future release-qualification work if/when First Light returns to the release queue.

## Stop Point

**First Light implementation and in-repository Gallery work are frozen for this pass.**

Do not begin FL-M6-02 automatically. Do not add more First Light features merely because the Gallery can host more examples.

## Next Action

1. Commit this FL-M6-01 documentation closeout.
2. Confirm the repository is clean and synchronized.
3. Select the next package deliberately through the suite's just-in-time package learning/checkpoint workflow.
4. Treat future First Light clean-project reproduction and release qualification as a separate explicitly activated return to the package.
