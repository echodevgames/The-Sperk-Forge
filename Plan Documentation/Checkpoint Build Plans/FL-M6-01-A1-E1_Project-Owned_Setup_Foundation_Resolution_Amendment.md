# FL-M6-01-A1-E1 — Project-Owned Setup Foundation Resolution Amendment

**Parent checkpoint:** FL-M6-01 — First Light Production Reference Showcase
**Parent expansion:** FL-M6-01-A1 — Splash Presentation & Authoring Expansion
**Authority:** SFGSS-PKG-ECHOLAUNCH-001 v1.16.0
**Status:** Approved
**Date:** 2026-08-08
**Unity baseline:** 6000.3.8f1

## 1. Trigger

A1 Slice E creation-time splash authoring reached focused green, and the retained `EchoLaunchSetup` EditMode filter passed `214 / 214`.

The real consumer proof then used a fresh requested root:

```text
Assets/EchoDevGames/SliceEProof/FirstLight
```

with three project-authored splash entries and `Create Splash Sequence` enabled.

The preview was `Ready`, but automatic compatible-candidate discovery resolved the existing committed SuiteShowcase foundation:

```text
Reuse EchoLaunchConfiguration
Reuse LaunchDestination
Reuse SplashSequence
Reuse StartupSequence
Reuse EchoLaunchRoot
Create fresh Boot scene
```

No Apply was performed.

## 2. Problem

The plan is internally legal under the pre-A1 compatible-reuse contract, but it is misleading once Setup accepts creation-time authoring.

A1 intentionally writes the Presentation/Splashes payload only to a **newly-created** SplashSequence and never re-authors a reused sequence. Therefore a `Ready` plan may currently accept user-authored entries while guaranteeing that those entries will not be written because an off-root SplashSequence is reused.

The same preview also demonstrates that a developer requesting a fresh project root may unintentionally receive a Boot scene bound to an older project-owned foundation elsewhere in `Assets/**`.

## 3. Approved Correction

Add one explicit Editor-only Setup choice:

```text
Foundation
  Asset Resolution .... Reuse Compatible Assets
                         Create Project-Owned Setup
```

### Reuse Compatible Assets

This is the default and preserves existing behavior.

When a requested canonical target is absent, exactly one compatible eligible project candidate may be reused according to the existing planner rules.

### Create Project-Owned Setup

When a requested canonical foundation target is absent, compatible candidates outside the requested target root do not substitute for that target.

The planner creates the missing requested target for:

- `EchoLaunchConfiguration`;
- `LaunchDestination`;
- `StartupSequence`;
- `SplashSequence` when `Create Splash Sequence` is enabled;
- the project-owned `EchoLaunchRoot` prefab variant.

The explicit destination scene is not cloned and may be reused normally. Boot scene and Build Settings behavior remain unchanged.

## 4. Safety Rules

- Existing compatible requested target assets remain authoritative.
- Incompatible requested target paths still block.
- The mode never overwrites or re-authors an existing/reused SplashSequence.
- The mode never mutates, copies, reparents, or repairs off-root candidates.
- `Create Splash Sequence` plus `Create Project-Owned Setup` guarantees that a missing requested sequence is created and receives the creation-time authoring payload.
- Stable entry IDs are generated through the existing H1 authoring utility.
- Identical reruns settle `NoChanges`.
- Existing programmatic requests default to `Reuse Compatible Assets`.
- The resolution mode participates in request fingerprinting and preview/apply freshness comparison.
- Repair remains separate and proof-gated.
- No Runtime API, Runtime assembly dependency, serialized Runtime schema, launch report schema, or player behavior changes.

## 5. Proof Requirements

Focused tests must prove:

1. legacy/default requests preserve compatible-candidate reuse;
2. Create Project-Owned Setup ignores eligible off-root candidates when the requested target is missing;
3. explicit destination scenes remain reusable;
4. existing compatible requested targets are not recreated;
5. incompatible requested targets still block;
6. request fingerprints differ when the resolution mode differs;
7. stale preview/apply mode changes are rejected;
8. newly-created SplashSequence receives the complete authoring payload;
9. identical rerun settles `NoChanges`;
10. retained repair/rollback candidate isolation remains green.

Manual consumer proof must show:

```text
fresh requested root
→ Create Project-Owned Setup
→ Create requested foundation
→ authored UMBRA/Isekai splash sequence retained
→ launch proof
→ identical request = NoChanges
```

## 6. Project Boundary

The Isekai/UMBRA images and placeholder audio used during this proof are project-owned consumer/showcase media. They do not become package defaults, samples, required dependencies, or evidence that The Sperk's Forge is an Isekai Studios product.

## 7. Explicit Non-Authority

A1-E1 does not authorize:

- overwriting reusable project assets;
- copying destination scenes;
- a generic project-template generator;
- arbitrary asset duplication;
- Runtime migration;
- automatic Repair;
- audio playback;
- branded package presets;
- clean-project release qualification.

Those remain outside this correction.
