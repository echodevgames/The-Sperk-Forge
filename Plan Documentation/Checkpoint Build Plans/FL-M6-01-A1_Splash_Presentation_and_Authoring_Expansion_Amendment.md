# FL-M6-01-A1 — Splash Presentation and Authoring Expansion Amendment

**Document role:** Bounded SFGSS-005 implementation-authority amendment
**Parent checkpoint:** FL-M6-01 — First Light Production Reference Showcase
**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.15.0
**Unity baseline:** `6000.3.8f1`
**Repository baseline:** exact clean `55a3204`
**Status:** Approved for bounded implementation
**Date:** August 8, 2026

## Approved Public Target

```text
FIRST LIGHT SETUP

Launch
  Destination .......... MainMenu

Splash Sequence
  ☑ Create Splash Sequence

Presentation
  Mode ................ Splash Only
  Background .......... Black
  Allow Advancement ... Yes

Splashes
  ┌ Studio Logo
  │ Image ............. EchoDevGamesBanner3
  │ Audio Intent ...... StudioStinger
  │ Motion ............ Pulse
  │ Advance ........... Skippable After Minimum
  │ Minimum ........... 1.5 sec
  │
  └ First Light
    Image ............. FirstLightLogo
    Audio Intent ...... FirstLightChime
    Motion ............ None
    Advance ........... Automatic
```

## Exact Authority

A1 may implement optional sequence-owned presentation settings, Splash Only / Splash + Status, project-owned background color, a neutral Allow Advancement gate, None/Pulse per-entry motion, Pulse scale/cycle values, additive Wait For Input After Minimum advancement, deterministic player/frame support, reduced-motion suppression, default uGUI rendering, normal Inspector authoring, and Setup creation-time authoring of a new SplashSequence.

## Compatibility

- SplashSequence schema stays `1`.
- EchoLaunchConfiguration schema stays `4`.
- Existing sequences without A1 settings remain valid and resolve to SplashAndStatus / black / advancement allowed.
- New Setup-created sequences default to SplashOnly / black / advancement allowed.
- Existing SplashSkipPolicy numeric meanings stay unchanged.
- Runtime never rewrites authored assets.

## Advancement

- Automatic: complete authored timeline; neutral input cannot shorten.
- Skippable After Minimum: neutral request may end at/after minimum.
- Wait For Input After Minimum: never auto-complete after natural timeline; wait for neutral request.
- Early requests remain latched until minimum.
- Global advancement disabled + Wait For Input is invalid and must block instead of hang.

## SplashOnly

Routine success-path status/progress is hidden. Splash presentation appears only while active and clears before destination. Blocking/interrupted terminal failure remains readable.

## Setup Non-Destructive Rule

Setup may author A1 values only while creating a new SplashSequence. Reused existing sequences are never overwritten and remain editable through the normal SplashSequence Inspector.

## Explicit Non-Authority

No audio playback/Jukebot integration, save/persistence, first-run-only behavior, project input binding, package-owned EventSystem/input module, video, Timeline, particles, material effects, generalized tween/effects framework, branded preset content, menu/navigation ownership, clean-project FL-M6-02, package version bump, or release qualification.

## Implementation Slices

A. Data compatibility and validation.
B. Deterministic player behavior.
C. Default uGUI presentation.
D. SplashSequence Inspector authoring.
E. Setup creation-time authoring.
F. Showcase proof + complete retained regression.

## Stop Conditions

Stop before widening A1 if schema migration becomes necessary, input backend ownership becomes necessary, reused sequences must be overwritten, Pulse starts becoming a general effects framework, failure visibility would be suppressed, or retained behavior regresses for unexplained reasons.
