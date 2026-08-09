# FL-M6-01 — First Light Production Reference Showcase Test Report

**Decision:** PASS for the Package Reference Showcase stage
**Date:** August 8, 2026
**Unity baseline:** `6000.3.8f1`
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.16.0
**Repository evidence baseline:** `ad12b27`

## Summary

First Light's public consumer workflow now supports and demonstrates both a canonical in-house example and an independently-created UMBRA example without package-internal shortcuts.

## Automated Evidence

| Evidence | Result |
|---|---:|
| H1 SplashEntry identity authoring | `5 / 5` |
| H2 destination Build Settings conformance | `35 / 35` |
| Final `EchoLaunchSetup` filtered EditMode | `224 / 224` |
| Retained FL-M5-07 full automated baseline | `809 / 809` |
| Retained FL-M5-07 manual Laboratory | `12 / 12` |

The retained `809 / 809` is a historical full-suite baseline from FL-M5-07. FL-M6-01 does not claim a new complete post-A1 EditMode/Runtime aggregate.

## Manual Showcase Evidence

### Canonical First Light Example

PASS:

```text
Boot
→ ordered project-owned splash presentation
→ valid startup settlement
→ destination validation/load
→ clean MainMenu-style destination
```

### UMBRA Example

PASS:

- fresh requested root;
- Create Project-Owned Setup selected;
- requested foundation targets created instead of off-root substitution;
- existing explicit destination reused;
- three authored stable-ID entries serialized;
- project-owned images/audio intent/timing/motion/advancement preserved;
- runtime Boot presentation succeeded;
- identical second Apply returned `NoChanges`;
- Created paths: None;
- Build Settings unchanged on repeat.

## Final Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/First Light Splashs/**
└── UMBRA Example/UMBRA Splashs/**
```

Gallery content is consumer/project-owned evidence and not a package dependency.

## Safety / Authority Result

PASS:

- First Light retains startup-only coordination authority;
- audio references remain metadata only;
- no save/persistence ownership added;
- no project input/EventSystem ownership added;
- no reusable menu or normal scene-flow authority added;
- no schema bump required for A1/A1-E1;
- reused SplashSequence assets remain non-overwritten;
- independent creation requires explicit user selection.

## Deferred Release Evidence

Not claimed:

- clean-project reproduction;
- fresh complete post-A1 regression totals;
- external distribution-route support;
- player builds;
- performance;
- release tag/catalog;
- private beta.

These require later explicit release-qualification work.
