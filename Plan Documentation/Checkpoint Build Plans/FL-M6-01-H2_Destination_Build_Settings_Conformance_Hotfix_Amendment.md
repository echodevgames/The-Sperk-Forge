# FL-M6-01-H2 — Destination Build Settings Conformance Hotfix Amendment

**Document role:** Bounded SFGSS-005 implementation-authority amendment
**Parent checkpoint:** FL-M6-01 — First Light Production Reference Showcase
**Package:** First Light — Startup and Launch (`EchoLaunch`)
**Specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.14.0
**Unity baseline:** `6000.3.8f1`
**Repository baseline:** `40ac6ac`
**Status:** Approved for bounded implementation
**Date:** August 8, 2026

---

## 1. Trigger

FL-M6-01-H1 is proven:

```text
SplashEntryIdentityAuthoringTests: 5 / 5
two blank Showcase entryId values generated through normal Inspector
ELAUNCH-SPLASH-001 no longer blocks
```

The same real Showcase Boot run then advanced to destination validation and blocked:

```text
[ELAUNCH-DEST-001] The initial destination scene is not included in the player build settings.
```

The public Setup request had previously reported `Succeeded`.

Observed Build Settings:

```text
0:On:Assets/OutdoorsScene.unity
1:On:Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_Boot.unity
```

Configured destination:

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_MainMenu.unity
```

The destination was absent from Build Settings.

## 2. Defect Classification

Runtime behavior is correct.

A configured launch destination must be build-loadable. Setup must not report success while omitting the selected destination from the enabled player Build Settings set.

This is a Setup conformance defect discovered by the Production Reference Showcase.

## 3. Exact Authorized Change

Change only First Light Setup planning/apply/repair behavior and focused Editor tests necessary to enforce:

```text
successful Setup
    implies
Boot scene enabled exactly once
AND
selected destination scene enabled exactly once
```

For `Add If Missing At End`:

- preserve all unrelated existing Build Settings scene entries and their relative order;
- if Boot is missing, append Boot;
- if destination is missing, append destination;
- if both are missing, append Boot first, destination second;
- if both already exist enabled exactly once, produce `NoChanges`;
- preview and result evidence must make destination Build Settings handling explicit.

## 4. Non-Authority

H2 does not authorize:

- Runtime destination-validation changes;
- schema changes or migrations;
- arbitrary Build Settings cleanup;
- deletion of existing scenes;
- disabling unrelated scenes;
- reordering unrelated existing scenes;
- manual MainMenu addition as the accepted Showcase workaround;
- peer-package dependencies.

Disabled/duplicate required entries may only be changed where existing approved repair proof safely authorizes that exact correction. Otherwise the planner must block or surface explicit repair evidence.

## 5. Required Focused Tests

At minimum:

1. both Boot and destination missing → append Boot then destination;
2. destination already enabled, Boot missing → append only Boot;
3. Boot already enabled, destination missing → append only destination;
4. both already enabled exactly once → `NoChanges`;
5. unrelated scene relative order preserved;
6. destination Build Settings action visible in plan/result evidence;
7. Runtime source unchanged and destination validation preserved.

## 6. Showcase Acceptance

Using the existing project-owned Showcase WIP:

1. open First Light Setup;
2. use project root `Assets/EchoDevGames/SuiteShowcase/FirstLight`;
3. use Boot `Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_Boot.unity`;
4. use destination `Assets/EchoDevGames/SuiteShowcase/FirstLight/Scenes/FirstLight_Showcase_MainMenu.unity`;
5. Refresh Plan and confirm destination addition is explicit;
6. Apply;
7. Apply the identical request again and confirm `NoChanges`;
8. verify Boot and MainMenu are both enabled exactly once;
9. play Boot and confirm `ELAUNCH-DEST-001` is gone.

## 7. Project WIP Preservation

Do not stage, overwrite, restore, or delete:

```text
Assets/EchoDevGames/SuiteShowcase/FirstLight/Configuration/SplashSequence.asset
Assets/EchoDevGames/SuiteShowcase/FirstLight/Art/**
```

during authority or implementation preparation.

## 8. Stop Point

If satisfying the invariant requires Runtime changes, arbitrary Build Settings reordering/cleanup, scene deletion, schema migration, or manual user editing outside the Setup contract:

**STOP.**

Return to authority review before widening FL-M6-01-H2.
