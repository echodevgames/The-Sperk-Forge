# First Light Complete User Handout

**Product:** First Light – Startup and Launch
**Technical package:** `EchoLaunch`
**Package ID:** `com.echodevgames.echo-launch`
**Package version:** `0.1.0`
**Publisher:** Jesse "Echo" Adams / EchoDevGames
**Suite:** The Sperk's Forge – EchoDevGames Game Systems Suite
**Unity baseline used for retained development evidence:** `6000.3.8f1`
**Minimum Unity version declared by the package:** `6000.0`
**Required Unity package:** uGUI `2.0.0`
**Handout status:** Complete product/use reference for the `0.1.0` implementation and Reference Gallery pass
**Distribution status:** Versioned tarball snapshot prepared; external clean-project tarball qualification is still pending

---

## 1. What First Light Is

First Light gives a Unity project one controlled startup path.

Its job is to coordinate the period between "the application has started" and "the project is ready to enter its first real destination." It can present startup splashes, execute an ordered startup sequence, publish structured progress and reports, validate and load one initial destination scene, and expose Editor tooling for creating, checking, repairing, simulating, and directly testing that startup foundation.

The normal production flow is:

```text
Boot scene
→ claim one launch authority
→ validate configuration
→ optional splash presentation
→ ordered startup steps
→ initial destination transition
→ Completed launch report/event
```

First Light is intentionally narrow. It coordinates startup. It does not become the owner of every system that starts during that period.

---

## 2. What First Light Owns

First Light owns:

- exactly-one launch authority for the startup session;
- launch lifecycle state and progress;
- ordered execution of authored startup steps;
- startup-step failure policy, timeout handling, and cooperative cancellation;
- immutable launch-result/report data;
- optional startup-only image splash playback;
- a replaceable startup status-presentation contract;
- one project-authored initial destination contract;
- the final startup handoff to that destination;
- preview-first project Setup;
- narrowly bounded explicit Repair;
- read-only project validation;
- development-only Direct Scene initialization;
- deterministic Editor-only launch simulation;
- the separately importable First Light Standalone Test Lab.

First Light also owns the rules that keep those surfaces repeat-safe, duplicate-safe, and honest about failures.

---

## 3. What First Light Does Not Own

First Light deliberately does **not** own:

- ordinary mid-game scene travel;
- save files, save slots, or persistence;
- global settings/preferences;
- audio playback or mixing;
- project input bindings;
- EventSystem or input-module selection;
- menus and ordinary UI navigation;
- gameplay rules;
- networking authority;
- project-specific services;
- general-purpose animation/VFX/feedback orchestration;
- automatic retry/backoff systems;
- project content or branding.

A project can initialize any of those systems from First Light, but the peer/project system remains responsible for its own behavior.

### Audio Intent is metadata only

A splash entry may store an optional `PreferredAudioClip`. First Light does not play that clip.

The field is a project-owned intent seam for a future or project-specific audio bridge. A First Light splash can therefore say "this is the preferred stinger" without creating an audio-runtime dependency.

---

## 4. Current Product and Qualification Status

The `0.1.0` implementation and in-repository Package Reference Showcase pass are complete and frozen.

Retained evidence includes:

```text
H1 splash identity authoring gate ............. 5 / 5
H2 destination Build Settings gate ............ 35 / 35
Final EchoLaunchSetup filtered EditMode gate .. 224 / 224

Retained FL-M5-07 automated baseline .......... 809 / 809
Retained FL-M5-07 manual Laboratory ........... 12 / 12

Independent UMBRA foundation creation ......... PASS
Three authored UMBRA splashes serialized ...... PASS
UMBRA runtime presentation .................... PASS
Identical second Setup Apply .................. NoChanges
```

The following are **not** claimed merely because this distribution snapshot exists:

- external clean-project tarball installation support;
- Git URL/tag installation support;
- public/scoped registry support;
- fresh complete post-A1 full-suite totals;
- player-build qualification;
- performance qualification;
- release tag/catalog readiness;
- private-beta or stable-release qualification.

The tarball is an official repository distribution artifact. Support language remains evidence-driven: the tarball route becomes a supported release route only after its clean-project proof is recorded.

---

# Part I - Distribution and Installation

## 5. Official Distribution Kit

The repository-owned First Light kit lives at:

```text
Distributions/First Light/0.1.0/
```

The kit contains:

```text
README.md
DISTRIBUTION_MANIFEST.md
DISTRIBUTION_BUILD_RECORD.txt
SHA256SUMS.txt
FIRST_LIGHT_COMPLETE_USER_HANDOUT.md
com.echodevgames.echo-launch-0.1.0.tgz
```

The tarball contains the UPM package beneath a single `package/` root.

The project-owned First Light Gallery is **not** inside the tarball. It remains reference material in The Sperk's Forge development repository.

---

## 6. Verify the Artifact Before Using It

The kit includes `SHA256SUMS.txt`.

On Windows Command Prompt, you can independently calculate the tarball hash with:

```text
certutil -hashfile "com.echodevgames.echo-launch-0.1.0.tgz" SHA256
```

Compare the result to the tarball line in `SHA256SUMS.txt`.

`DISTRIBUTION_BUILD_RECORD.txt` also records the artifact size, hash, package version, and source baseline used when the kit was assembled.

If the hash does not match, do not use the artifact.

---

## 7. Install the Tarball for Evaluation

The distribution artifact is suitable for internal/evaluation handoff. Its external clean-project route is not yet release-qualified.

In Unity:

1. Open the target project.
2. Open **Window > Package Management > Package Manager**.
3. Use the **+** menu.
4. Choose **Add package from tarball...**.
5. Select:

```text
com.echodevgames.echo-launch-0.1.0.tgz
```

6. Allow Unity to resolve package dependencies and compile.
7. Confirm the installed package reports:
   - package ID `com.echodevgames.echo-launch`;
   - version `0.1.0`;
   - uGUI dependency `2.0.0`.
8. Inspect the Console before doing project setup.
9. Continue with **Tools > Sperk's Forge > First Light > Setup**.

### Important qualification note

A successful local import is not by itself proof that the route is officially supported. The release standard still requires external clean-project install, quick-start/Laboratory proof, removal, and reinstall evidence before the tarball route may be advertised as release-qualified.

---

## 8. Embedded Development Installation

Inside The Sperk's Forge development repository, First Light is embedded at:

```text
Packages/com.echodevgames.echo-launch
```

The embedded route is the implementation/development surface used for the retained package evidence.

A consumer using the tarball does not need the entire Sperk's Forge repository.

---

## 9. Package Contents

The package contains:

```text
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Presentation.UGUI/
Editor/
Tests/
Samples~/
Documentation~/
```

Key boundaries:

- `Runtime/` contains neutral runtime authority and contracts.
- `Presentation.UGUI/` contains the default uGUI presentation implementation.
- `Editor/` contains Setup, Repair, Validator, Simulator, and authoring tooling.
- `Tests/` contains package regression tests.
- `Samples~/` contains the importable Standalone Test Lab.
- `Documentation~/` contains user and developer documentation.

---

# Part II - The Five-Minute Path

## 10. The Fastest Successful Setup

Open:

```text
Tools > Sperk's Forge > First Light > Setup
```

For the smallest independent setup:

1. Pick a project-owned root under `Assets/**`.
2. Pick a Boot scene path beneath that root.
3. Select an existing project destination scene.
4. Leave the default Build Settings policy unless you have a reason to change it.
5. Choose **Create Project-Owned Setup** if this root should own a fresh independent foundation.
6. Optionally enable **Create Splash Sequence**.
7. Configure any startup presentation.
8. Click **Refresh Plan**.
9. Read every operation and diagnostic.
10. Apply only when the plan is `Ready`.
11. Refresh again after Apply.
12. An identical converged request should produce `NoChanges`.
13. Play the Boot scene.

A project may use an empty valid StartupSequence for the smallest happy path.

---

## 11. What Setup Creates

For an independent project-owned foundation, Setup can create:

```text
<Project Root>/
├── Configuration/
│   ├── EchoLaunchConfiguration.asset
│   ├── LaunchDestination.asset
│   ├── StartupSequence.asset
│   └── SplashSequence.asset       (optional)
├── Prefabs/
│   └── EchoLaunchRoot.prefab
└── Scenes/
    └── <Boot Scene>.unity
```

It can also add the Boot scene and configured destination to Build Settings according to the selected policy.

Setup is preview-first. Creation is not supposed to be a black box.

---

# Part III - Setup in Depth

## 12. Project Root

The default proposed root is:

```text
Assets/EchoDevGames/FirstLight
```

You may choose another project-owned location under `Assets/**`.

The root is where First Light's generated project-owned configuration, prefab, and Boot-scene content lives.

Package source remains under `Packages/**`. Setup does not convert generated consumer content into immutable package content.

---

## 13. Boot Scene

The Boot scene is the startup scene that contains the project-owned `EchoLaunchRoot` prefab variant.

The Boot scene should be the place from which the startup pipeline begins.

First Light can add the Boot scene to Build Settings using the approved policy selected in Setup.

---

## 14. Destination Scene

You select an existing project scene as the initial destination.

First Light stores destination metadata in a project-owned `LaunchDestination` asset and validates loadability before startup side effects proceed.

The destination scene remains project-owned. Setup does not open and rewrite it as part of ordinary Apply.

First Light owns only this initial startup handoff. Ordinary scene transitions after startup belong elsewhere.

---

## 15. Build Settings Policy

The normal default is:

```text
Add If Missing At End
```

This preserves unrelated existing Build Settings scene order and adds required startup entries only when missing.

The alternate "place Boot first" path requires explicit approval in the Setup flow.

The final project should have:

- one enabled Boot entry;
- one enabled configured destination entry;
- no duplicate First Light entries created by repeated Setup.

---

## 16. Foundation Asset Resolution

Setup exposes:

```text
Foundation
  Asset Resolution .... Reuse Compatible Assets
                         Create Project-Owned Setup
```

### 16.1 Reuse Compatible Assets

This is the backward-compatible default.

If a requested target is missing and exactly one compatible eligible project asset exists elsewhere, Setup may reuse it.

This is useful when a project already has a valid First Light foundation and is intentionally connecting another Boot/setup request to it.

### 16.2 Create Project-Owned Setup

Use this when the requested root should own a fresh, independent First Light foundation.

When a canonical target is missing, Setup creates the requested:

- `EchoLaunchConfiguration`;
- `LaunchDestination`;
- `StartupSequence`;
- optional `SplashSequence`;
- `EchoLaunchRoot` prefab variant.

Compatible off-root candidates do not silently replace those missing targets.

The explicitly selected destination scene can still be reused.

Existing compatible assets already at the requested targets remain authoritative.

Incompatible requested targets still block rather than being overwritten.

### 16.3 Why this choice matters

Creation-time splash authoring is applied only to a newly-created SplashSequence.

If you authored new splashes but Setup reused an existing off-root sequence, your new entry payload would have nowhere appropriate to be written. `Create Project-Owned Setup` is the explicit answer when you want independent new content.

---

## 17. Create Splash Sequence

Enable **Create Splash Sequence** when you want Setup to create a new project-owned SplashSequence during foundation creation.

Setup can then author the sequence's presentation settings and entries before Apply.

Setup does not overwrite or re-author a reused SplashSequence.

If you want to edit an existing sequence, use its normal Inspector.

---

## 18. Presentation Mode

A SplashSequence can be authored as:

```text
Splash Only
Splash + Status
```

### Splash Only

Only the splash presentation is intended to dominate the startup surface during splash playback.

This is appropriate for logo/title-card presentations.

### Splash + Status

Splash presentation can coexist with the ordinary startup status presentation.

Use this when you want the player/developer to retain startup-state visibility alongside branding or visual cards.

---

## 19. Presentation Background

The sequence stores a project-owned background color.

A common logo/startup presentation uses black, but the project owns the choice.

The background setting belongs to presentation data. It does not alter project-wide graphics settings.

---

## 20. Allow Advancement

`Allow Advancement` is the sequence-level user-advance gate.

If advancement is disabled, entry-level user-advance choices cannot be used to advance the presentation.

If advancement is enabled, individual entries can choose policies that permit a request after their minimum-display rule is satisfied.

First Light provides the neutral request surface. It does not own your project's input binding.

---

## 21. Splash Entry Fields

Each splash entry can author:

- **Image**
- **Audio Intent**
- **Display Label**
- **Fade In**
- **Hold**
- **Fade Out**
- **Minimum Display**
- **Motion**
- **Pulse Maximum Scale**
- **Pulse Cycle Seconds**
- **Advance policy**

Each entry also has a stable hidden identity generated by Editor authoring when blank.

Existing non-empty identities are preserved.

---

## 22. Splash Image

The image is project-owned presentation content.

The package does not require EchoDevGames branding.

A consumer can use studio marks, publisher logos, product titles, accessibility notices, legal cards, or any other appropriate startup image.

The image is required for a normal visual splash entry.

---

## 23. Audio Intent

`PreferredAudioClip` stores the AudioClip the project would prefer to pair with that splash.

Again:

```text
First Light stores the intent.
First Light does not play the sound.
```

A project adapter or future audio-package bridge can choose whether and how to consume the metadata.

This keeps startup coordination independent from Resonance/Jukebot or any project-specific audio manager.

---

## 24. Splash Timing

Each entry has:

- fade-in seconds;
- hold seconds;
- fade-out seconds;
- minimum-display seconds.

Minimum display is a guard against advancing an entry before the project-defined minimum exposure has elapsed.

Reduced-motion playback removes fade behavior while preserving the deterministic sequence contract.

---

## 25. Splash Motion

Supported motion:

```text
None
Pulse
```

### None

The splash uses its normal authored presentation scale.

### Pulse

The image uses deterministic pulse metadata:

- maximum scale;
- cycle seconds.

Reduced-motion mode suppresses Pulse.

Pulse is intentionally small in scope. First Light is not a general-purpose animation framework.

---

## 26. Advancement Policies

The current authoring choices are:

```text
Automatic
Skippable After Minimum
Wait For Input After Minimum
```

### Automatic

The entry completes through authored timing with no user-advance requirement.

### Skippable After Minimum

The entry can advance early after its minimum-display time if an advance/skip request is accepted.

A request made too early can be latched and honored after the minimum gate, according to the deterministic splash player behavior.

### Wait For Input After Minimum

After the minimum-display rule is satisfied, the entry waits for an advancement request.

Your project is responsible for deciding what input action generates that request.

---

## 27. Supplying a Skip/Advance Request

The default uGUI status view exposes:

```text
RequestSplashSkip()
```

The package does not bind that method to a specific keyboard key, controller button, touch gesture, or input package.

Project code can route an input action to the presenter/request surface without giving First Light input authority.

---

## 28. Preview Plan

Always use **Refresh Plan** before Apply.

A plan tells you:

- whether the request is Ready or Blocked;
- what paths would be created;
- what compatible paths would be reused;
- whether Build Settings would change;
- what destination would be used;
- whether a new SplashSequence will receive creation-time authoring;
- any blockers or information diagnostics;
- deterministic request/evidence/plan fingerprints.

Treat Preview as a safety surface, not ceremony.

---

## 29. Apply Plan

Apply:

- recollects project evidence;
- replans immediately before mutation;
- rejects stale or non-executable plans;
- executes only approved Create/Reuse/NoChange operations;
- creates missing project-owned folders/assets/prefab/Boot scene;
- preserves the selected destination scene;
- writes Build Settings last;
- returns structured created/reused/rollback/recovery evidence.

Apply does not perform general destructive cleanup.

---

## 30. Repeat Apply and Idempotence

After a successful Apply:

1. refresh the identical request;
2. inspect the converged plan;
3. apply again if you are verifying setup repeatability.

The expected settled result is:

```text
Status: NoChanges
Created paths: None
```

Repeated setup should not manufacture duplicate foundations or duplicate Build Settings entries.

---

# Part IV - Repair

## 31. When to Use Repair

Repair is separate from ordinary Apply.

Use it only when First Light detects narrowly approved current-schema drift that it knows how to reconcile safely.

Repair is not a "make everything look like the template" button.

---

## 32. What Repair Can Reconcile

Current Repair authority is intentionally limited to proven cases such as:

- configuration references;
- destination scene path metadata;
- verified root-prefab configuration binding;
- a canonical zero-root Boot-scene condition;
- canonical Boot Build Settings entry state.

The exact available repair actions come from the refreshed plan and current evidence.

---

## 33. Repair Safety

Repair:

- recollects evidence before mutation;
- compares fresh fingerprints;
- shares one mutation gate with Apply;
- requires explicit confirmation;
- backs up affected existing assets and matching `.meta` bytes;
- hash-verifies backup/restoration;
- writes Build Settings last;
- attempts rollback on failure;
- reports retained backup/recovery locations when automatic rollback cannot finish.

Backups are stored beneath:

```text
Library/EchoDevGames/FirstLight/RepairBackups/<repair-id>
```

---

## 34. What Repair Will Not Do

Repair does not:

- migrate unsupported schemas;
- regenerate stable identities;
- replace arbitrary project asset types;
- rewrite StartupSequence contents;
- rewrite SplashSequence contents;
- delete duplicate roots as arbitrary cleanup;
- restructure arbitrary prefabs;
- clean unrelated scene content;
- move, rename, or delete user assets;
- modify the selected destination scene.

If the evidence cannot prove a safe supported repair, the correct behavior is to block.

---

# Part V - Validator

## 35. Open the Validator

Use:

```text
Tools > Sperk's Forge > First Light > Validator
```

The Validator runs only when you explicitly choose **Validate Project**.

Opening the window, repainting, importing, reloading, or entering Play Mode does not silently start validation.

---

## 36. What the Validator Checks

The Validator can inspect:

- canonical project-owned First Light root;
- configuration identity/schema;
- startup sequence;
- launch destination;
- optional splash sequence;
- root prefab;
- Boot scene;
- enabled Build Settings scenes;
- relevant Build Settings entries;
- Direct Scene configuration safety.

Closed scenes may be opened additively for read-only inspection while preserving the user's scene state.

---

## 37. Validator Is Read-Only

The Validator never:

- applies Setup;
- repairs;
- migrates;
- saves project assets;
- deletes assets;
- moves or renames assets;
- changes Build Settings.

It reports. It does not "helpfully" mutate the project behind your back.

---

## 38. Health Mapping

Project health is derived from the most severe finding:

```text
Blocker -> Blocked
Error   -> Invalid
Warning -> NeedsAttention
Info    -> Healthy
```

The report uses stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015` diagnostics.

---

# Part VI - Runtime Startup Model

## 39. One Launch Authority

Only one active `EchoLaunchRoot` is accepted as the startup authority.

Duplicate roots are rejected before they can begin side effects.

The stable duplicate-authority diagnostic is:

```text
ELAUNCH-ROOT-001
```

This is fundamental to First Light. A duplicate is not allowed to become a second bootstrap pipeline.

---

## 40. Launch Lifecycle

The root publishes controlled lifecycle state and progress.

The approved forward path culminates in:

```text
AuthorityClaimed
→ Validating
→ Running
→ Transitioning
→ Completed
```

Active work can also end in failure or interruption.

Terminal states are frozen.

Backward/skipped transitions are rejected.

---

## 41. Lifecycle Notifications

Consumers can observe accepted state/progress changes.

Important behavior:

- state is accepted before related callbacks observe it;
- state notification precedes progress notification when both are involved;
- listener exceptions are contained per listener;
- listener failure uses stable diagnostic `ELAUNCH-EVENT-001`;
- duplicate roots remain silent;
- destruction clears subscriptions.

---

# Part VII - Startup Sequence Authoring

## 42. StartupSequence

A project-owned `StartupSequence` stores the ordered startup work.

Current StartupSequence schema:

```text
2
```

The sequence contains private ordered entries with stable identities.

---

## 43. StartupSequenceEntry

An entry connects authored order/activation/policy metadata to a startup-step definition.

Entries can be enabled or disabled.

Disabled entries are skipped without creating an executor.

An empty valid sequence is legal.

---

## 44. StartupStepDefinition

`StartupStepDefinition` is the reusable authored definition contract for one startup task.

A project or package can create its own concrete step-definition type.

The definition stores stable authored identity and configuration. Mutable active execution state belongs in the executor/runtime attempt, not the ScriptableObject definition.

---

## 45. IStartupStepExecutor

Each enabled step creates a fresh `IStartupStepExecutor`.

The executor performs the actual asynchronous startup work.

The current runtime uses Unity `Awaitable<StartupStepResult>`.

A fresh executor is intended for each attempt so mutable execution state is not shared through the definition asset.

---

## 46. Startup Step Context

The execution context supplies the information a step needs to behave as part of the startup run, including:

- launch mode;
- stable authored identities;
- step position/count;
- cooperative cancellation token;
- package-owned progress reporter.

The context does not give the step launch authority.

---

## 47. StartupStepResult

Steps settle with structured results instead of relying on arbitrary Console text.

The runner preserves meaningful status/code/message/details through policy evaluation.

Warnings and failures therefore remain visible in launch evidence.

---

## 48. Required and Optional Intent

Startup policy can express whether work is required or optional.

The approved MVP failure actions are:

```text
BlockLaunch
ContinueWithWarning
```

This lets a project distinguish "startup cannot safely continue" from "record the problem and continue."

---

## 49. Timeout

A step can author timeout metadata.

Important rules:

- timeout `0` disables timeout;
- deadlines use the package's monotonic launch clock;
- completion-vs-timeout ordering is deterministic;
- timeout uses stable `ELAUNCH-STEP-003`;
- timed-out executors are allowed to settle before traversal proceeds.

---

## 50. Cooperative Cancellation

Steps can declare cancellation capability.

The root can request launch cancellation.

Cancellation reaches the linked executor token.

The active executor settles before the runner returns the cancelled outcome.

Caller cancellation uses stable `ELAUNCH-STEP-005`.

Cancellation is not downgraded into a warning by an authored warning policy.

---

## 51. Progress

A running step can publish:

- determinate progress;
- indeterminate progress;
- messages.

Determinate progress is normalized to the inclusive `0..1` range.

Late progress after settlement is contained.

The root translates accepted step progress into launch progress snapshots and presentation.

---

## 52. Preflight

Before executor factories run, First Light validates the authored configuration/sequence/entry/step graph.

Preflight checks include:

- configuration identity/schema;
- sequence identity/schema;
- null entries;
- enabled entries with missing definitions;
- entry identities;
- activation metadata;
- duplicate entry IDs;
- step identities/schemas;
- duplicate step IDs.

This prevents startup side effects from beginning on an invalid authored graph.

---

## 53. Runner Re-entry Protection

One runner instance permits one active traversal.

Concurrent re-entry is rejected through:

```text
ELAUNCH-RUN-001
```

After the active attempt settles, sequential reuse is allowed.

---

# Part VIII - Launch Reports and Terminal Events

## 54. LaunchReport

First Light produces an immutable final launch report.

Current report schema:

```text
2
```

The report records structured startup evidence such as:

- producing package version;
- launch outcome;
- ordered step reports;
- authored policy metadata;
- progress/result/timing evidence;
- warning/failure/cancellation summaries;
- destination metadata for completed handoff;
- total launch elapsed time.

Reports are runtime evidence, not a durable save system.

---

## 55. Terminal Events

The authoritative root exposes matching terminal events:

```text
LaunchCompleted
LaunchFailed
LaunchInterrupted
```

The root accepts terminal state/report data before dispatching the matching event.

`LastReport` is available to the accepted authority.

Listener exceptions remain isolated.

---

# Part IX - Destination Handoff

## 56. LaunchDestination

`LaunchDestination` is a project-owned ScriptableObject.

Current schema:

```text
1
```

It stores:

- stable destination identity;
- display label;
- runtime-safe scene path.

---

## 57. Destination Validation

The configured destination is validated before startup-step side effects.

Key destination diagnostics include:

```text
ELAUNCH-DEST-001
ELAUNCH-DEST-002
```

The first covers destination preflight conditions; the second covers destination-load failure conditions.

---

## 58. Destination Loading

The default loader uses Unity asynchronous single-scene loading.

First Light publishes transition progress while the root remains in `Transitioning`.

Successful activation completes the startup handoff and finalizes the completed report.

Again, normal mid-game travel is outside First Light.

---

# Part X - Startup Presentation

## 59. Neutral Presentation Contract

The neutral Runtime assembly does not require uGUI.

It exposes `ILaunchStatusPresenter`.

A project can supply a different presenter without changing launch authority.

A null/headless presenter is supported so launch behavior is not dependent on presentation.

---

## 60. Default uGUI Presentation

The package supplies a separate uGUI presentation assembly and `EchoLaunchStatusView`.

It can present:

- lifecycle state;
- message text;
- active step position;
- stable step ID;
- determinate progress;
- percentage;
- indeterminate progress;
- elapsed time;
- warnings/failures;
- destination/completion information;
- splash image and label.

The package prefab is intended as a replaceable starting presentation, not mandatory art direction.

---

## 61. Package Presentation Prefabs

The package provides stable templates:

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

The root prefab contains one `EchoLaunchRoot` and a nested status view with the presenter reference wired.

Project configuration is intentionally not hard-coded into the immutable package template.

Setup creates a project-owned root prefab variant.

---

# Part XI - Direct Scene Development

## 62. Why Direct Scene Exists

During development, a developer often wants to press Play directly inside a gameplay/Test Lab scene instead of navigating through the production Boot every time.

First Light supports that workflow without creating a second startup architecture.

---

## 63. EchoDirectSceneInitializer

Add `EchoDirectSceneInitializer` to the development scene and assign a project-owned `DirectSceneConfiguration`.

Supported policies:

```text
EditorOnly
EditorAndDevelopmentBuilds
BootRequired
```

`EditorOnly` is the default.

A non-development release player is never allowed to create a Direct Scene root.

---

## 64. Direct Scene Settlement

On Play:

1. scene-authored roots get first opportunity to claim authority in `Awake`;
2. the initializer settles once in `Start`;
3. it reuses an existing accepted authority when present;
4. otherwise it may create one approved `DirectSceneDevelopment` root;
5. if the configured destination is already active, startup can complete without reloading that scene.

Direct Scene uses the normal root, splash, startup sequence, report, destination, duplicate-safety, and lifetime rules.

---

## 65. Direct Scene Validation

`ELAUNCH-VAL-009` is used for Direct Scene safety findings.

A valid `EditorOnly` helper can remain Healthy.

Explicit Development-Build opt-in is intentionally visible as a warning/NeedsAttention condition.

---

# Part XII - Launch Simulator

## 66. Open the Simulator

Use:

```text
Tools > Sperk's Forge > First Light > Simulator
```

The Simulator is Editor-only and explicit.

Opening the window does not run a scenario.

---

## 67. Built-In Simulator Presets

Current presets:

```text
ImmediateSuccess
TimedProgressSuccess
WarningContinues
RecoverableFailureContinues
BlockingFailureStops
TimeoutStops
ExecutorExceptionStops
Cancellation
```

These are deterministic startup-step scenarios for understanding/reporting First Light behavior.

---

## 68. What the Simulator Does

For an accepted request, the Simulator:

- builds transient `HideAndDontSave` configuration/sequence objects;
- runs the real sequence runner/policy/progress/timeout/cancellation behavior;
- produces one immutable simulation report;
- destroys transient objects after the run.

It can copy deterministic evidence.

---

## 69. What the Simulator Does Not Do

The Simulator does not:

- edit project-authored configuration;
- create persistent assets;
- add scene objects;
- modify Build Settings;
- claim a launch root;
- play splash/status presentation;
- load a destination;
- run in players.

It is a diagnostic execution surface, not a second production launch mode.

---

# Part XIII - Standalone Test Lab

## 70. Import the Lab

Unity Package Manager exposes one sample:

```text
First Light Standalone Test Lab
```

Import it through the package's **Samples** section.

Importing the sample does not automatically:

- run Setup;
- run Repair;
- run Validator;
- run Simulator;
- enter Play Mode;
- modify Build Settings.

---

## 71. What the Lab Contains

The Standalone Test Lab includes package-owned/sample-safe proof content for:

- canonical Boot handoff;
- destination handoff;
- successful startup;
- timed progress;
- warning continuation;
- recoverable failure;
- blocking failure;
- invalid destination;
- duplicate authority;
- Direct Scene behavior;
- splash behavior and minimum-duration skip proof.

It also contains redistributable placeholder splash art and sample-only helpers.

---

## 72. Sample Isolation

Imported `Assets/Samples/**` content is excluded from automatic Setup candidate discovery unless the user explicitly selects it.

This prevents the imported Lab from silently changing how a consumer's real project foundation is resolved.

---

# Part XIV - First Light Gallery

## 73. Repository-Only Reference Gallery

The Sperk's Forge repository contains:

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
```

This Gallery is not required by the package and is not shipped inside the tarball.

It exists as project-owned production-style consumer evidence.

---

## 74. First Light Example

The canonical example demonstrates:

```text
Boot
→ EchoDevGames splash
→ First Light splash
→ startup settlement
→ MainMenu destination
```

It uses public Setup, project-owned configuration, normal Inspector authoring, and the default uGUI presentation path.

---

## 75. UMBRA Example

The UMBRA example proves the package is not secretly tied to the canonical First Light branding or foundation.

The proof used:

```text
Foundation > Asset Resolution = Create Project-Owned Setup
Presentation = Splash Only
Background = Black
Allow Advancement = Yes
Entries = 3
```

The generated sequence retained:

- `The Sperk`;
- `Isekai Studios`;
- `UMBRA`;
- unique stable entry IDs;
- project-owned images;
- optional audio-intent references;
- authored timing/advancement;
- Pulse on the Isekai entry.

The generated Boot experience played correctly.

An identical second Apply returned `NoChanges`.

---

# Part XV - Data, Schemas, and Compatibility

## 76. Current Serialized Schemas

Important current schema versions:

```text
EchoLaunchConfiguration .... 4
StartupSequence ............ 2
LaunchDestination .......... 1
SplashSequence ............. 1
LaunchReport ............... 2
```

Historical schemas are not silently rewritten by Runtime.

Unsupported/older data is blocked or handled through explicit Editor authority rather than runtime mutation.

---

## 77. Stable Identities

First Light uses stable identities for authored configuration/sequence/entry/step/destination data.

Editor tooling generates blank splash entry identities during authoring.

Existing non-empty IDs are preserved.

Do not casually regenerate IDs merely to make serialized text look different.

---

## 78. Project-Owned Content

Generated configuration and consumer content belongs to the project.

Package updates must not silently overwrite a game's:

- splash content;
- startup sequence;
- destination choice;
- branding;
- scenes;
- project-owned root prefab variant.

This is one of the reasons Setup distinguishes package templates from project-owned generated assets.

---

# Part XVI - Diagnostics Quick Reference

## 79. Important Diagnostic Families

| Family | Purpose |
|---|---|
| `ELAUNCH-ROOT-*` | launch authority / duplicate root |
| `ELAUNCH-EVENT-*` | listener callback containment |
| `ELAUNCH-CFG-*` | launch configuration |
| `ELAUNCH-SEQ-*` | startup sequence |
| `ELAUNCH-STEP-*` | startup step/preflight/timeout/cancellation/execution |
| `ELAUNCH-RUN-*` | runner concurrency |
| `ELAUNCH-LIFE-*` | root lifecycle/start/cancellation |
| `ELAUNCH-DEST-*` | initial destination |
| `ELAUNCH-VIEW-*` | presentation/presenter |
| `ELAUNCH-SPLASH-*` | splash definition/playback/presenter |
| `ELAUNCH-SETUP-*` | Setup/Apply/Repair |
| `ELAUNCH-VAL-*` | read-only Validator |
| `ELAUNCH-DIRECT-*` | Direct Scene development |
| `ELAUNCH-SIM-*` | Simulator |
| `ELAUNCH-SIM-STEP-*` | simulated step outcomes |

---

## 80. Frequently Important Exact Codes

| Code | Meaning/use |
|---|---|
| `ELAUNCH-ROOT-001` | duplicate launch authority rejected |
| `ELAUNCH-EVENT-001` | listener failure isolated |
| `ELAUNCH-STEP-003` | startup-step timeout |
| `ELAUNCH-STEP-004` | invalid executor/factory/result/clock contract containment |
| `ELAUNCH-STEP-005` | structured caller cancellation |
| `ELAUNCH-RUN-001` | concurrent runner re-entry |
| `ELAUNCH-LIFE-001` | lifecycle interruption/cancellation |
| `ELAUNCH-LIFE-002` | root start-gate rejection |
| `ELAUNCH-DEST-001` | destination preflight/loadability condition |
| `ELAUNCH-DEST-002` | destination load failure |
| `ELAUNCH-VIEW-001` | invalid presenter component/fallback |
| `ELAUNCH-VIEW-002` | presenter callback failure containment |
| `ELAUNCH-SPLASH-001` | invalid splash definition/preflight |
| `ELAUNCH-SPLASH-002` | unexpected splash playback failure |
| `ELAUNCH-SPLASH-003` | missing visual presenter/headless warning |
| `ELAUNCH-VAL-009` | Direct Scene safety/policy finding |

For Setup/Repair/Validator/Simulator ranges, the tool output includes the exact current message and affected path. Prefer that structured report over guessing from the numeric suffix alone.

---

# Part XVII - Common Workflows

## 81. Workflow: Minimal Game Boot

Use this when you only need a controlled Boot and handoff:

1. Create Project-Owned Setup.
2. Leave StartupSequence empty but valid.
3. Omit SplashSequence.
4. Select a valid destination.
5. Preview.
6. Apply.
7. Play Boot.

Result:

```text
Boot
→ validation
→ destination
→ Completed
```

---

## 82. Workflow: Branded Startup Logos

1. Enable Create Splash Sequence.
2. Choose Splash Only.
3. Pick a project-owned background.
4. Add logo entries.
5. Choose Automatic or Skippable After Minimum per entry.
6. Optionally add Pulse.
7. Preview and Apply.
8. Bind project input to the skip-request surface if desired.

Do not rely on `PreferredAudioClip` for playback unless your project supplies an audio bridge/adapter.

---

## 83. Workflow: Startup Services

For each startup concern:

1. create a concrete project/package `StartupStepDefinition`;
2. make it create a fresh executor;
3. add the definition through a StartupSequence entry;
4. choose required/optional intent;
5. choose BlockLaunch or ContinueWithWarning;
6. configure timeout/cancellation support;
7. report progress where useful.

Examples of concerns that *might* be coordinated through steps include loading project-owned services, reading configuration, establishing a session, or warming a system. The peer system still owns its own work.

---

## 84. Workflow: Optional Service That Must Not Block Launch

Use an optional entry with `ContinueWithWarning`.

The step can fail, have its structured outcome retained as a warning, and allow later entries to continue.

Do not convert a truly required dependency into optional merely to make the startup screen turn green.

---

## 85. Workflow: Required Service

Use required intent with `BlockLaunch`.

If the step cannot settle successfully under policy, traversal stops and the root fails rather than handing the project into an invalid destination state.

---

## 86. Workflow: Directly Play a Gameplay Scene During Development

1. Add `EchoDirectSceneInitializer`.
2. Assign a project-owned `DirectSceneConfiguration`.
3. Keep `EditorOnly` unless you have a deliberate Development-Build reason.
4. Play the scene.
5. Validate the project with the First Light Validator.
6. Remove/disable the helper where it is not wanted.

Do not use Direct Scene as a reason to skip the real Boot path in production qualification.

---

## 87. Workflow: Diagnose a Startup Failure

Recommended order:

1. read the current `EchoLaunchRoot` status;
2. inspect `LastReport` when terminal;
3. run **Validator** for project-shape problems;
4. use **Simulator** if you need to understand a failure-policy/timeout/cancellation scenario;
5. use the Standalone Lab to compare known-good package behavior;
6. inspect Console only after structured First Light evidence;
7. use Repair only when the refreshed plan explicitly offers an approved repair.

---

# Part XVIII - Troubleshooting

## 88. Setup Says Blocked

Check:

- destination scene selected and valid;
- requested paths under `Assets/**`;
- incompatible asset already occupying a requested target;
- package root template available;
- unsupported schema;
- ambiguous/reuse candidates;
- invalid splash creation-time authoring;
- Build Settings safety diagnostics.

Do not Apply a Blocked plan.

---

## 89. Setup Reuses the Wrong Existing Foundation for What You Intended

If you actually want a brand-new independent root, select:

```text
Create Project-Owned Setup
```

Then refresh the plan.

The missing canonical foundation targets should plan as `Create` under your requested root.

---

## 90. My New Splash Entries Are Not Being Written

Creation-time authoring is only for a newly-created SplashSequence.

If `ResolveSplashSequence` is `Reuse`, Setup will not overwrite that sequence.

Options:

- edit the reused sequence through its Inspector; or
- choose **Create Project-Owned Setup** and a fresh requested target.

---

## 91. Runtime Blocks With a Splash Diagnostic

Inspect the sequence/entry identities, images, timing, and serialized values.

Common causes include invalid definition data or blank/duplicate identities.

Normal Inspector authoring generates blank entry IDs. Existing IDs are preserved.

---

## 92. Runtime Blocks Before Startup Steps

Destination and authored startup data are preflighted before side effects.

A bad destination or invalid startup graph intentionally prevents executor factories from beginning.

Use Validator and inspect the structured failure report.

---

## 93. A Step Times Out

Inspect:

- authored timeout;
- whether timeout should be disabled (`0`);
- whether the executor yields/settles correctly;
- whether cancellation is supported;
- step result code/message/details;
- `ELAUNCH-STEP-003`.

A timeout is a real terminal attempt outcome, not just a UI timer.

---

## 94. A Step Throws

Executor/factory exceptions are contained and converted into structured failure evidence.

Inspect `ELAUNCH-STEP-004` and the sanitized exception type/message.

First Light does not copy stack traces into immutable launch data.

Use the Unity Console/source debugger for the underlying implementation fault.

---

## 95. A Presentation Component Fails

Presenter failures are isolated through the view diagnostics.

The neutral runtime can fall back without turning the presentation implementation into startup authority.

Inspect:

```text
ELAUNCH-VIEW-001
ELAUNCH-VIEW-002
```

---

## 96. Direct Scene Creates Unexpected Warnings

Check the Direct Scene policy.

`EditorAndDevelopmentBuilds` is an explicit opt-in and is intentionally visible to Validator.

Use `EditorOnly` when you only need Editor convenience.

---

## 97. I Imported the Standalone Lab and Setup Started Seeing Its Assets

Current Setup excludes ordinary imported `Assets/Samples/**` content from automatic candidate discovery.

If you intentionally selected a sample asset, explicit selection still counts.

If imported sample content is being picked up automatically, capture the Setup plan and report it as a regression.

---

## 98. Repeated Apply Keeps Creating Things

That violates the intended convergence contract.

Capture:

- request fingerprint;
- evidence fingerprint;
- plan fingerprint;
- first Apply result;
- second plan;
- second Apply result;
- created/reused paths;
- Build Settings before/after.

A converged identical request should settle `NoChanges`.

---

# Part XIX - Known Limitations and Deferred Capabilities

## 99. Not Implemented in First Light 0.1.0

First Light does not currently provide:

- automatic retry;
- retry count;
- retry backoff;
- interactive retry;
- retry/skip failure UI;
- public per-step lifecycle events;
- warning aggregation outside the run result;
- dependency validation between arbitrary startup steps;
- Editor migration from historical configuration schemas;
- automatic Direct Scene helper installation;
- Direct Scene build hooks or automatic build blocking;
- a persistent-root lifetime policy;
- peer-package bridges.

These are not hidden unfinished features. They are outside the current implemented boundary.

---

## 100. Additional Scope Boundaries

Do not assume First Light provides:

- audio playback for splash Audio Intent;
- an input action for splash advance;
- project EventSystem;
- save/load;
- global preferences;
- menu navigation;
- mid-game scene-flow authority;
- networking;
- gameplay-state ownership;
- general loading-screen framework for every scene transition.

---

# Part XX - Removal, Reinstall, and Updating

## 101. Removing the Package

Project-owned assets generated beneath `Assets/**` belong to the project.

Removing the package should not silently delete them.

However, scenes/prefabs that contain First Light scripts will naturally have missing script references while the package is absent.

Before removal:

1. commit/back up your project;
2. record the package version;
3. decide whether generated project-owned content should remain;
4. remove optional project adapters that depend on First Light where appropriate.

---

## 102. Reinstalling the Same Version

After reinstall:

1. allow Unity to compile;
2. run Validator;
3. inspect package version;
4. open Setup;
5. refresh the existing project request;
6. confirm compatible project-owned assets are reused rather than duplicated;
7. play the Boot path.

Release qualification will eventually record formal external tarball removal/reinstall evidence.

---

## 103. Updating to a Future Version

Do not assume future package updates may rewrite project-owned content.

Read:

- CHANGELOG;
- migration guide/release notes when applicable;
- known limitations;
- schema changes;
- distribution manifest.

If a future version requires migration, migration should be explicit Editor authority rather than silent Runtime rewriting.

---

# Part XXI - Developer Extension Guide

## 104. Add a Custom Startup Step

At a high level:

1. create a project/package concrete `StartupStepDefinition`;
2. make the definition create a fresh `IStartupStepExecutor`;
3. put the definition in a `StartupSequenceEntry`;
4. author policy/timeout/activation;
5. report progress through the provided context;
6. honor cooperative cancellation where declared;
7. return structured `StartupStepResult`;
8. test success, failure, warning, timeout, cancellation, and re-entry expectations appropriate to the step.

Keep mutable attempt state in the executor, not in the ScriptableObject definition.

---

## 105. Replace the Status Presenter

Implement `ILaunchStatusPresenter` in project code or another presentation assembly.

First Light's neutral runtime does not require uGUI.

A custom presenter can consume accepted lifecycle/progress/report data while the root remains authoritative.

Do not move startup rules into the presenter.

---

## 106. Supply Project Input for Splash Advancement

Your input layer can call the exposed splash skip/advance request surface.

The project decides:

- which action;
- which device;
- whether input is enabled;
- whether accessibility remapping applies;
- whether an EventSystem is needed.

First Light only decides whether the current splash policy can accept the request.

---

## 107. Integrate Audio Later

A project can observe the current splash/presentation flow and consume `PreferredAudioClip` metadata through its own bridge or audio authority.

A future official bridge should depend visibly on both peers instead of making First Light directly depend on the audio package.

---

# Part XXII - Evidence and Testing

## 108. What the Current Evidence Proves

The retained evidence demonstrates substantial in-repository correctness for:

- runtime authority/lifecycle;
- sequence execution;
- timeout/cancellation;
- reports/events;
- destination handoff;
- uGUI presentation;
- splash playback;
- Setup;
- Repair;
- Validator;
- Direct Scene;
- Simulator;
- Standalone Lab;
- consumer-style Gallery setup;
- independent project-owned foundation creation;
- repeat-safe Setup convergence.

---

## 109. What It Does Not Yet Prove

The current distribution snapshot does not by itself prove:

- this exact tarball installs cleanly in a fresh external project;
- removal/reinstall of this exact artifact;
- final player builds;
- performance targets;
- release-tag/catalog integration;
- public registry delivery;
- beta/stable support status.

Those claims require later retained evidence.

---

## 110. Recommended Evaluation Checklist

When handing First Light to another developer for evaluation:

1. verify SHA-256;
2. record Unity version/template/OS;
3. install tarball;
4. confirm compile;
5. inspect Package Manager identity/version/dependency;
6. import the Standalone Test Lab;
7. run its documented happy path;
8. create a new project-owned Setup foundation;
9. author at least one splash;
10. preview and Apply;
11. verify destination Build Settings;
12. play Boot;
13. repeat identical Apply and require `NoChanges`;
14. run Validator;
15. remove imported sample;
16. remove/reinstall the package if the evaluation scope includes route qualification;
17. report findings with logs/screenshots/report text.

Until that evidence is officially retained, treat the route as evaluation rather than a stable support claim.

---

# Part XXIII - Support and Bug Reporting

## 111. What to Include in a Useful Report

Please include:

- First Light package version;
- Unity Editor version;
- operating system;
- target platform if relevant;
- installation route;
- package source/tarball checksum;
- whether this is Boot, Direct Scene, Simulator, Lab, or Setup/Repair/Validator;
- exact diagnostic code;
- copied First Light report/plan/result where available;
- relevant project-relative asset paths;
- exact reproduction steps;
- expected result;
- observed result;
- whether the issue reproduces after Unity restart;
- whether the issue reproduces in the Standalone Test Lab;
- Console exception/stack trace when an actual Unity exception occurred.

Do not include credentials, secrets, or private player data.

---

## 112. Before Reporting a Setup/Repair Problem

Capture:

```text
Request fingerprint
Evidence fingerprint
Plan fingerprint
Plan status
Diagnostics
Created paths
Reused paths
Build Settings before/after
Rollback status
Manual recovery paths
```

That evidence often makes the difference between "something went weird" and a reproducible tooling defect.

---

# Part XXIV - Quick Reference

## 113. Main Menus

```text
Tools > Sperk's Forge > First Light > Setup
Tools > Sperk's Forge > First Light > Validator
Tools > Sperk's Forge > First Light > Simulator
```

Direct Scene is configured through scene components/assets rather than a separate production launch pipeline.

---

## 114. Important Package Assets

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
Samples~/First Light Standalone Test Lab/
Documentation~/
```

---

## 115. Default Project Root

```text
Assets/EchoDevGames/FirstLight
```

---

## 116. Normal Production Order

```text
optional splash
→ startup sequence
→ destination
```

All three phases remain under one root-owned launch session.

---

## 117. Setup Golden Rule

```text
Refresh Plan first.
Apply only when Ready.
Repeat the same request and expect NoChanges.
```

---

## 118. Repair Golden Rule

```text
Repair only what the refreshed evidence proves is an approved repair candidate.
```

---

## 119. Validator Golden Rule

```text
Validation reports.
Validation does not mutate.
```

---

## 120. Distribution Golden Rule

```text
Artifact exists ≠ route is release-qualified.
Evidence decides the support claim.
```

---

# Part XXV - Glossary

## Boot scene

The startup scene containing the project-owned First Light root used for the production launch path.

## Destination

The one project-authored scene First Light hands control to after startup succeeds.

## Launch authority

The single accepted root that owns the active First Light startup session.

## StartupSequence

The ordered project-owned list of startup entries.

## StartupStepDefinition

An authored reusable definition that creates a fresh runtime executor for one startup task.

## Executor

The mutable single-attempt object that performs startup work.

## SplashSequence

Project-owned startup-presentation definition containing ordered splash entries and sequence-level presentation settings.

## Audio Intent

Optional `PreferredAudioClip` metadata stored on a splash. It is not playback.

## Setup

Preview-first Editor workflow for creating/reusing a project-owned First Light foundation.

## Repair

Explicit, backup-protected reconciliation of narrowly supported current-schema drift.

## Validator

Read-only project-health inspection.

## Direct Scene

Development-only convenience for entering the real First Light startup authority from a non-Boot development scene.

## Simulator

Editor-only deterministic sequence-runner diagnostic surface.

## Standalone Test Lab

Separately importable UPM sample used as isolated engineering/user-visible proof.

## Package Reference Showcase / First Light Gallery

Project-owned production-style consumer examples in The Sperk's Forge repository. They are not package dependencies.

## Distribution Kit

The repository-owned versioned bundle containing the exact tarball plus handout, manifest, checksums, and build record.

## NoChanges

The expected Setup/Repair settlement when the project already matches the approved request and no mutation is needed.

---

# Closing Summary

First Light is a startup coordinator, not a universal game framework.

Use it when you want one understandable startup authority that can:

- present optional branded splashes;
- run ordered asynchronous startup work;
- enforce required/optional failure policy;
- report progress and terminal evidence;
- handle timeout/cancellation;
- hand off to one initial destination;
- create and validate its project-owned foundation safely;
- support direct development workflows without inventing a second bootstrap architecture.

The package is designed to make startup predictable while leaving each game's systems, presentation, content, and rules in the game's hands.

For the shortest path, start with **Setup**, inspect the Preview, create an independent foundation, play the Boot scene, and verify an identical second Apply returns **NoChanges**.
