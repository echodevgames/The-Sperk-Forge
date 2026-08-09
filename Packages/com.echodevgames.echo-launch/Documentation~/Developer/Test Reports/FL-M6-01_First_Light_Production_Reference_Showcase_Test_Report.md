# FL-M6-01 — First Light Production Reference Showcase Test Report

**Result:** PASS for FL-M6-01 Package Reference Showcase scope
**Unity:** `6000.3.8f1`
**Package:** `com.echodevgames.echo-launch` `0.1.0`
**Specification:** v1.16.0
**Evidence baseline:** through `ad12b27`
**Date:** August 8, 2026

## Automated Evidence

| Gate | Result | Notes |
|---|---:|---|
| FL-M6-01-H1 SplashEntry identity authoring | `5 / 5` | blank IDs generated through Editor tooling; existing IDs preserved |
| FL-M6-01-H2 destination Build Settings conformance | `35 / 35` | Setup conformance only; Runtime validation retained |
| Final `EchoLaunchSetup` filtered EditMode gate | `224 / 224` | includes creation-time authoring, foundation resolution, retained Setup/Apply/Repair isolation |
| Retained FL-M5-07 complete EditMode baseline | `306 / 306` | historical retained full-suite baseline before A1 additions |
| Retained FL-M5-07 Runtime Play Mode baseline | `503 / 503` | historical retained full-suite baseline before A1 additions |
| Retained FL-M5-07 automated total | `809 / 809` | historical retained baseline |
| Retained FL-M5-07 manual Laboratory | `12 / 12` | historical retained baseline |

Focused A1 slices were run green during implementation. This report does not invent a new complete post-A1 EditMode or Runtime Play Mode total.

## Canonical First Light Example

Observed successful public consumer path:

```text
FirstLight_Showcase_Boot
→ ordered project-owned splashes
→ startup settles
→ configured destination validates/loads
→ FirstLight_Showcase_MainMenu
```

Result: **PASS**.

## UMBRA Independent Foundation Proof

### Preview

Request:

- fresh project-owned root;
- `Foundation > Asset Resolution = Create Project-Owned Setup`;
- `Create Splash Sequence = enabled`;
- Splash Only;
- black background;
- advancement enabled;
- three authored entries.

Expected/observed plan:

- requested Configuration — Create;
- requested LaunchDestination — Create;
- requested SplashSequence — Create;
- requested StartupSequence — Create;
- requested EchoLaunchRoot — Create;
- requested Boot scene — Create;
- selected existing MainMenu destination — Reuse.

Result: **PASS**.

### First Apply / Serialized Evidence

The requested foundation was created. Generated SplashSequence evidence showed:

- authored presentation settings enabled;
- Splash Only;
- black background;
- Allow Advancement enabled;
- three non-empty stable entry IDs;
- labels `The Sperk`, `Isekai Studios`, `UMBRA`;
- three project-owned `PreferredAudioClip` references;
- authored skip/motion/pulse metadata;
- Isekai Pulse maximum scale `1.05`, cycle `2.5` seconds.

Result: **PASS**.

### Runtime Presentation

The generated UMBRA Boot scene played and presented correctly.

Result: **PASS**.

### Repeat Apply

Second Preview resolved the requested local targets as compatible existing assets and retained the explicit external destination reuse.

Second Apply:

```text
Status: NoChanges
Created paths: None
Rollback completed: No
```

Build Settings before and after were unchanged.

Result: **PASS**.

## Ownership / Safety

- Gallery assets remain under project `Assets/**`.
- Package content does not depend on Gallery branding/media.
- Audio references remain metadata; no audio playback authority was introduced.
- Schemas remain SplashSequence `1` and EchoLaunchConfiguration `4`.
- A1-E1 is Editor Setup behavior and does not widen Runtime authority.

## Qualification Boundary

Not run / not claimed:

- fresh complete post-A1 EditMode aggregate;
- fresh complete post-A1 Runtime Play Mode aggregate;
- separate clean-project reproduction;
- Git/tag/tarball/public-registry installation proof;
- player builds;
- performance;
- release tag/catalog/private beta.

These belong to a later explicitly activated release-qualification stage.
