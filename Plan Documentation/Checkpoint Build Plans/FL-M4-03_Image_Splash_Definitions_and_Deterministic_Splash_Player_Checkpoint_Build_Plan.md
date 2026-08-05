# FL-M4-03 — Image Splash Definitions and Deterministic Splash Player

**Document ID:** FL-M4-03
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
**Milestone:** M4 — Startup Entry and Presentation
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `0e049ef`
**Starting documentation commit:** `cbaee24`
**Starting Runtime Play Mode:** 414 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> The lantern now has glass. This checkpoint teaches it to show a deliberate
> procession of images without giving presentation control of launch truth.

---

## 1. Purpose and observable outcome

FL-M4-03 creates the project-owned image splash definitions, deterministic
clock-driven player, neutral splash presenter contract, public skip-request
seam, reduced-motion behavior, and default uGUI image projection.

When complete:

1. A project can author a standalone `SplashSequence` asset.
2. Each entry owns stable identity, image, label, fade, hold, minimum time,
   and skip policy.
3. Runtime validates assets without rewriting them.
4. `SplashSequencePlayer` traverses entries against `ILaunchClock`.
5. Fade alpha is deterministic and normalized.
6. Minimum display time can extend the hold.
7. Early skip requests latch but cannot bypass the minimum.
8. Disallowed skip requests do nothing.
9. Reduced motion removes fade phases.
10. Cancellation and player re-entry are contained.
11. Headless presentation remains valid.
12. `EchoLaunchStatusView` presents image, label, alpha, and sequence
    position.
13. `RequestSplashSkip()` requires no EchoInput dependency.
14. Automated proof passes in neutral Runtime and isolated uGUI test
    assemblies.

---

## 2. Authority and constraints

Approved package authority requires:

- Image-only MVP splash entries.
- Fade, hold, minimum display, and skip policy.
- Configurable zero-duration fades.
- Reduced-motion support.
- Text meaning independent from color.
- A public skip request that does not require EchoInput.
- Replaceable presentation.
- Immutable project-owned definitions.
- Video and custom animation adapters deferred.

### Serialization boundary

This checkpoint does **not** add a `SplashSequence` field to
`EchoLaunchConfiguration`.

That later change would advance the serialized configuration schema and
requires an authority-first checkpoint. FL-M4-03 proves definitions and
playback independently before configuration/root integration.

---

## 3. Exact scope

### Runtime definitions

- `SplashSkipPolicy`
- `SplashPlaybackPhase`
- `SplashEntry`
- `SplashSequence`
- `SplashPresentationFrame`
- `SplashPlaybackResult`

### Runtime behavior

- `IImageSplashPresenter`
- `NullImageSplashPresenter`
- `SplashSequencePlayer`

### Default presentation

- `EchoLaunchStatusView` implements `IImageSplashPresenter`.
- Serialized image root, image, and label references.
- Public `RequestSplashSkip()`.
- Showing-splash state copy.
- Alpha application.
- Clear and unbind behavior.

### Proof

- `26` neutral Runtime tests.
- `10` isolated uGUI presentation tests.
- Predicted full suite: `450`.

The predicted total is a target, not evidence.

---

## 4. Files

### Created Runtime/Splash

- `Runtime/Splash.meta`
- `Runtime/Splash/SplashSkipPolicy.cs`
- `Runtime/Splash/SplashPlaybackPhase.cs`
- `Runtime/Splash/SplashEntry.cs`
- `Runtime/Splash/SplashSequence.cs`
- `Runtime/Splash/SplashPresentationFrame.cs`
- `Runtime/Splash/SplashPlaybackResult.cs`
- `Runtime/Splash/SplashSequencePlayer.cs`
- Unity-generated script `.meta` files

### Created Runtime/Presentation

- `Runtime/Presentation/IImageSplashPresenter.cs`
- `Runtime/Presentation/IImageSplashPresenter.cs.meta`
- `Runtime/Presentation/NullImageSplashPresenter.cs`
- `Runtime/Presentation/NullImageSplashPresenter.cs.meta`

### Modified presentation

- `Presentation.UGUI/EchoLaunchStatusView.cs`

### Created tests

- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs`
- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs.meta`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs.meta`

### Created plan

- `Plan Documentation/Checkpoint Build Plans/FL-M4-03_Image_Splash_Definitions_and_Deterministic_Splash_Player_Checkpoint_Build_Plan.md`

---

## 5. Deterministic timing model

Effective timing:

```text
fadeIn = reducedMotion ? 0 : authoredFadeIn
fadeOut = reducedMotion ? 0 : authoredFadeOut
hold = max(
    authoredHold,
    minimumDisplay - fadeIn - fadeOut,
    0)
total = fadeIn + hold + fadeOut
```

The player reads monotonic unscaled time through `ILaunchClock`.

It rejects:

- NaN time.
- Infinite time.
- Negative time.
- Backward clock movement.
- Concurrent playback on the same player.

---

## 6. Skip model

Entry policies:

- `Disallowed`
- `AfterMinimumDisplay`

A skip request is latched.

If received before the minimum:

```text
request remains pending
    -> minimum boundary arrives
        -> entry ends
```

If the policy is disallowed, the request has no effect.

Project input remains outside the package. The default view exposes
`RequestSplashSkip()` and emits a neutral event.

---

## 7. Reduced motion

Reduced-motion playback:

- Removes fade-in.
- Removes fade-out.
- Keeps hold and minimum display timing.
- Uses immediate full-opacity state changes.
- Preserves skip policy.

This checkpoint does not read platform or project preferences. The caller
supplies the reduced-motion choice.

---

## 8. Validation

Playback blocks when:

- Sequence identity is malformed.
- Sequence schema is unsupported.
- Entry collection is missing.
- An entry is null.
- Entry identity is malformed.
- Entry image is missing.
- Timing is negative or nonfinite.
- Skip policy is undefined.
- Entry IDs collide.

Runtime never repairs or rewrites the asset.

---

## 9. Automated proof

Runtime proof includes:

- Stable enum vocabulary.
- Schema and identity.
- Invalid entry and sequence rejection.
- Empty sequence.
- Ordered traversal.
- Fade phases and alpha.
- Minimum display.
- Early, permitted, and disallowed skips.
- Reduced motion.
- Cancellation.
- Re-entry.
- Backward clock.
- Headless fallback.
- Result accounting.
- Definition immutability.

uGUI proof includes:

- Interface implementation.
- Pre-bind no-op.
- Image, label, state, position, and alpha.
- Public skip request.
- Clear behavior.
- Unbind behavior.
- Null-frame rejection.
- Missing-reference safety.

---

## 10. Compile and test gates

- Unity errors: 0.
- Unity compiler warnings: 0.
- Complete Runtime Play Mode suite passes.
- New test total is discovered.
- Existing 414 tests remain green.
- Neutral Runtime asmdef remains dependency-neutral.
- No EchoInput, EchoUI, TextMeshPro, video, or peer-package reference.
- `git diff --check` passes.

---

## 11. Explicit exclusions

FL-M4-03 does not authorize:

- `EchoLaunchConfiguration` schema advancement.
- Serialized splash-sequence binding on configuration.
- Root-owned splash execution.
- Report schema changes.
- Splash results in `LaunchReport`.
- Package prefab YAML.
- Canvas art pass.
- Input binding or EchoInput bridge.
- Legal splash semantics.
- Video playback.
- Custom animation adapters.
- Interactive retry/cancel UI.
- Editor setup or migration.
- Test Lab scenes.
- Player build proof.
- Package version change.

---

## 12. Rollback

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Presentation.UGUI/EchoLaunchStatusView.cs"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Runtime/Splash"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Splash.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation/IImageSplashPresenter.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation/IImageSplashPresenter.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation/NullImageSplashPresenter.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation/NullImageSplashPresenter.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs.meta"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M4-03_Image_Splash_Definitions_and_Deterministic_Splash_Player_Checkpoint_Build_Plan.md"
```

After a pushed implementation commit, use `git revert`.

---

## 13. Commit plan

Implementation:

```text
echo-launch: complete FL-M4-03 deterministic image splashes
```

Documentation:

```text
echo-launch: document FL-M4-03 completion
```

---

## 14. Stop point

Stop after standalone definitions, deterministic playback, neutral skip
requests, default uGUI projection, and isolated automated proof.

Do not bind the splash sequence to configuration or the root.

---

## 15. Tentative next checkpoint

**FL-M4-04 — Splash Configuration Schema and Root Playback Integration**

Tentative only. Because it changes serialized configuration shape, it
requires explicit authority promotion before implementation.

---

## 16. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Preserve Runtime immutability, keep project input outside the
package, honor minimum display timing, preserve reduced-motion behavior, and
stop before configuration schema or root integration.
