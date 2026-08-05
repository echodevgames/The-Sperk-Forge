# FL-M3-08 — Initial Destination Contract, Load Result, and Completed Handoff

**Document ID:** FL-M3-08
**Version:** 1.0.0
**Status:** Active and authorized after authority commit
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
**Package ADR:** EchoLaunch-ADR-001 v1.0.0
**Milestone:** M3 — Startup Sequence Runtime
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `a6f6544`
**Starting documentation commit:** `f76b9df`
**Authority baseline:** Pending FL-M3-08 authority commit
**Starting Runtime Play Mode:** 336 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> The startup sequence has opened the gate. This checkpoint names the destination, crosses the threshold, and seals one truthful completed report.

---

## 1. Purpose and observable outcome

FL-M3-08 completes the first standalone root-owned launch path.

When complete:

1. A project-owned `LaunchDestination` asset identifies one initial scene.
2. `EchoLaunchConfiguration` schema 3 references that destination.
3. Root preflight rejects missing, invalid, unsupported, or non-loadable destinations before startup-step side effects.
4. One injectable `IInitialDestinationLoader` performs the initial asynchronous handoff.
5. The default Unity loader uses `SceneManager.LoadSceneAsync` for standalone operation.
6. Destination progress is published while the root remains `Transitioning`.
7. A successful activated destination advances `Transitioning -> Completed`.
8. The successful immutable report is finalized exactly once.
9. `LastReport` stores the completed report.
10. `LaunchCompleted` is raised exactly once after completed state and report storage.
11. Destination validation or loading failure produces a failed report and no false completion.
12. Cancellation during the transition settles safely and produces interruption without a completion event.
13. Existing failed and interrupted reports remain unchanged.
14. Authored configuration and destination assets remain immutable.

---

## 2. Accepted architecture

### 2.1 Destination asset

`LaunchDestination` is a public project-owned `ScriptableObject`.

Schema:

```text
LaunchDestination.CurrentSchemaVersion = 1
```

Authored fields:

- Stable lowercase 32-character hexadecimal destination ID.
- Serialized schema version.
- User-facing display name.
- Runtime-safe scene path or build-loadable scene identifier.

Runtime reads but never modifies these fields.

### 2.2 Configuration schema

```text
EchoLaunchConfiguration.CurrentSchemaVersion = 3
```

Historical meaning:

- Schema 2: configuration identity plus startup-sequence reference.
- Schema 3: schema-2 data plus one serialized initial `LaunchDestination` reference.

Runtime rejects schema 2 through `ELAUNCH-CFG-002`. Migration is Editor-owned later work.

### 2.3 Destination loader

Public contract:

```csharp
Awaitable<InitialDestinationLoadResult> LoadAsync(
    LaunchDestination destination,
    IProgress<float> progress,
    CancellationToken cancellationToken);
```

The loader:

- Starts on Unity’s main thread.
- Receives validated immutable destination data.
- Reports finite normalized progress.
- Returns one immutable terminal load result.
- Does not publish root lifecycle or events.
- Does not finalize reports.
- Does not own normal mid-game travel.

### 2.4 Default Unity loader

`UnityInitialDestinationLoader`:

- Verifies build-loadability before starting.
- Calls `SceneManager.LoadSceneAsync` in `LoadSceneMode.Single`.
- Reports normalized progress.
- Waits for the operation to settle.
- Returns success only after the operation is complete and the destination scene is active.
- Converts start/settlement failure to `ELAUNCH-DEST-002`.
- Never claims support for cancelling Unity’s underlying scene operation after it has begun.
- Honors cancellation before load start.
- During an already-started load, waits for safe settlement before returning the observed terminal result.

### 2.5 Root completed handoff

Successful order:

```text
Startup sequence succeeds
    -> Transitioning snapshot
        -> destination loader starts
            -> transition progress snapshots
                -> destination activation confirmed
                    -> Completed snapshot accepted
                        -> completed LaunchReport finalized
                            -> LastReport assigned
                                -> LaunchCompleted dispatched
```

No completion is published before destination activation.

---

## 3. Stable diagnostics

- `ELAUNCH-CFG-002`: configuration schema unsupported.
- `ELAUNCH-DEST-001`: destination missing, invalid, unsupported, or not build-loadable during preflight.
- `ELAUNCH-DEST-002`: destination load failed after transition began.
- `ELAUNCH-LIFE-001`: launch interruption/cancellation.
- `ELAUNCH-EVENT-001`: terminal listener failure isolation.

Diagnostic meanings must not be reused.

---

## 4. Exact implementation scope

### Runtime files created

- `Runtime/SceneLoading.meta`
- `Runtime/SceneLoading/LaunchDestination.cs`
- `Runtime/SceneLoading/LaunchDestination.cs.meta`
- `Runtime/SceneLoading/IInitialDestinationLoader.cs`
- `Runtime/SceneLoading/IInitialDestinationLoader.cs.meta`
- `Runtime/SceneLoading/InitialDestinationLoadStatus.cs`
- `Runtime/SceneLoading/InitialDestinationLoadStatus.cs.meta`
- `Runtime/SceneLoading/InitialDestinationLoadResult.cs`
- `Runtime/SceneLoading/InitialDestinationLoadResult.cs.meta`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs.meta`
- `Runtime/SceneLoading/InitialDestinationProgressRelay.cs`
- `Runtime/SceneLoading/InitialDestinationProgressRelay.cs.meta`

### Runtime files modified

- `Runtime/Configuration/EchoLaunchConfiguration.cs`
- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Reports/LaunchReport.cs`
- `Runtime/Reports/LaunchReportBuilder.cs`

### Automated tests

Create:

- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- Unity-generated `.meta`

Modify only where the accepted schema/handoff contract changes retained expectations:

- `Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`

No other retained test may be changed unless a directly checkpoint-owned compile or contract conflict is proven.

### Authority documents

Already established before implementation:

- SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- EchoLaunch-ADR-001 v1.0.0
- This Checkpoint Build Plan

---

## 5. Detailed runtime requirements

### 5.1 `LaunchDestination`

Required public reads:

- `DestinationId`
- `SchemaVersion`
- `DisplayName`
- `ScenePath`

Required internal validation:

- Canonical ID.
- Supported schema.
- Nonblank normalized display name.
- Nonblank runtime-safe scene path.

No runtime setter, repair, GUID regeneration, or schema rewrite.

### 5.2 `EchoLaunchConfiguration`

Add:

```csharp
[SerializeField]
private LaunchDestination initialDestination;

public LaunchDestination InitialDestination { get; }
```

Advance `CurrentSchemaVersion` to `3`.

Existing identity and startup-sequence behavior remain unchanged.

### 5.3 `InitialDestinationLoadStatus`

Approved values:

```text
Succeeded = 0
Failed = 1
Cancelled = 2
```

Numeric zero remains the safe successful default only because result construction is controlled and not serialized.

### 5.4 `InitialDestinationLoadResult`

Immutable public result containing:

- Status.
- Destination ID.
- Code.
- Message.
- Sanitized details.
- Whether success/failure/cancellation occurred.

Factories:

- `Success`
- `Failed`
- `Cancelled`

A successful result requires no diagnostic code. Failed/cancelled results require code and message.

### 5.5 Destination preflight

Root-owned validation occurs after the root publishes `Validating` and before the runner creates any executor.

Validate:

- Configuration schema 3.
- Assigned destination.
- Destination identity.
- Destination schema 1.
- Destination display name.
- Destination scene path.
- Loader non-null.
- Loader-specific build-loadability where available.

Invalid preflight finalizes a failed report with `ELAUNCH-DEST-001` and calls no startup-step factory or destination loader.

The startup-sequence runner remains destination-neutral.

### 5.6 Loader injection

Production root defaults to `UnityInitialDestinationLoader.Shared`.

Internal test seam:

```csharp
SetInitialDestinationLoaderForTesting(
    IInitialDestinationLoader loader)
```

Allowed only on an idle authoritative root before launch advances.

### 5.7 Destination transition

After a successful sequence:

1. Mark the builder transition-pending with the successful run.
2. Publish `Transitioning`.
3. Invoke the loader once.
4. Relay accepted destination progress through `Transitioning` snapshots.
5. Await loader settlement.
6. Handle cancellation, failure, null result, or contract violation without false completion.
7. On success, publish `Completed`.
8. Finalize the successful report with destination identity.
9. Store `LastReport`.
10. Dispatch `LaunchCompleted`.

### 5.8 Completed report

`LaunchReport` adds immutable destination identity and display metadata.

`LaunchReport` accepts `Completed`, `Failed`, or `Interrupted`.

Completed invariants:

- `WasCancelled == false`.
- Destination ID is nonblank.
- Final result is successful.
- Successful pending run exists.
- Report accounting still balances.
- Finalization occurs after destination success.

Failed/interrupted report behavior remains compatible.

### 5.9 Destruction and cancellation

- Cancellation before destination load begins prevents the load.
- Cancellation during an injected cancellable load waits for settlement.
- Default Unity scene loading is not falsely claimed to be cancellable after start.
- A cancelled transition produces `Interrupted`, not `Completed`.
- Root destruction suppresses unsafe late public events.
- No `LaunchCompleted` after destruction begins.

---

## 6. Explicit exclusions

FL-M3-08 does not authorize:

- Automatic launch from `Awake` or `Start`.
- Splash or status presentation.
- Direct-scene initializer.
- Persistent-root policy changes.
- Normal mid-game scene travel.
- Conditional destination providers.
- Save-aware destination selection.
- EchoSceneFlow bridge.
- Additive scene loading.
- Loading-screen ownership.
- Scene unload policy after handoff.
- Editor migration from schema 2 to 3.
- SceneAsset custom inspector.
- Setup/repair tooling.
- Test Lab scenes or prefabs.
- Player build validation.
- Report export.
- Public step lifecycle events.
- Package version change.

---

## 7. Implementation sequence

### Phase 0 — Authority

1. Commit specification v1.4.0.
2. Commit EchoLaunch-ADR-001.
3. Commit FL-M3-08 plan.
4. Reconcile package and suite Current Notes.
5. Confirm clean synchronized repository.

### Phase 1 — Destination data and result contracts

1. Add `LaunchDestination`.
2. Advance configuration schema to 3.
3. Add load status, result, loader interface, progress relay, and default Unity loader.
4. Compile gate.

### Phase 2 — Root handoff and completed report

1. Add destination validation.
2. Add loader test seam.
3. Extend the root-owned launch through destination settlement.
4. Extend report model/builder for completed outcome and destination identity.
5. Add `LaunchCompleted`.
6. Compile gate.

### Phase 3 — Automated proof

1. Add focused destination and completed-handoff tests.
2. Reconcile retained tests only for approved schema/handoff changes.
3. Run complete Runtime Play Mode suite.
4. Record actual totals.

### Phase 4 — Closeout

1. Commit and push implementation.
2. Batch-generate checkpoint, test, architecture, changelog, README, index, specification evidence, ADR evidence maturity, and Current Notes.
3. Commit and push adjacent documentation.
4. Confirm clean synchronized repository.

---

## 8. Automated test matrix

### Destination asset and configuration

- New destination defaults to schema 1.
- New configuration defaults to schema 3.
- Configuration exposes the assigned destination.
- Schema 2 configuration is unsupported.
- Invalid destination ID is rejected.
- Unsupported destination schema is rejected.
- Blank display name is rejected.
- Blank scene path is rejected.
- Assets remain unchanged after launch.

### Preflight ordering

- Missing destination fails with `ELAUNCH-DEST-001`.
- Invalid destination fails before any startup-step factory.
- Invalid destination fails before loader invocation.
- Null loader contract is rejected before step side effects.
- Valid destination allows sequence execution.

### Load result contract

- Success result is immutable and validated.
- Failed result requires code/message.
- Cancelled result requires code/message.
- Undefined status is rejected.
- Result destination identity is normalized.
- Null load result becomes `ELAUNCH-DEST-002`.

### Successful handoff

- Loader invoked exactly once.
- Destination progress remains in `Transitioning`.
- Completed state is published only after loader success.
- Successful report finalizes after completed state.
- `LastReport` is the exact `LaunchCompleted` payload.
- `LaunchCompleted` fires exactly once.
- Failed/interrupted events do not fire.
- Report final status is `Completed`.
- Report contains destination identity/display name.
- Report retains sequence accounting and steps.
- Completion listener failure does not block later listeners.

### Failure and cancellation

- Destination load failure reaches `Failed`.
- Failed report uses `ELAUNCH-DEST-002`.
- `LaunchCompleted` does not fire on failure.
- Cancellation before load start prevents loader invocation.
- Cancellation during injected load waits for settlement.
- Cancelled load reaches `Interrupted`.
- `LaunchCompleted` does not fire on cancellation.
- Destroyed root publishes no unsafe late completion event.

### Default loader

- Non-loadable scene returns validation/failure result without starting.
- Cancellation before start returns cancelled.
- Progress values remain finite and normalized.
- Actual Boot-to-scene activation remains a later Laboratory proof.

### Retained regression

Starting baseline:

```text
336 passed
0 failed
0 ignored
```

The final closeout records only observed totals.

---

## 9. Compile and evidence gates

### Compile gate

- Unity errors: 0.
- Unity compiler warnings: 0.
- No `UnityEditor` in Runtime.
- No new peer-package dependency.
- No project-assembly reference.

### Runtime gate

- All retained and new Runtime Play Mode tests pass.
- Expected runtime diagnostic warnings are distinguished from compiler warnings.
- No asset is dirtied or mutated by runtime work.

### Git gate

- `git diff --check` passes.
- Only authorized files are staged.
- Implementation and documentation commits are separately evidenced.
- `main` equals `origin/main`.
- Working tree is clean.

---

## 10. Failure symptoms and fixes

### Retained root tests fail at destination preflight

Cause: successful retained tests create schema-3 configuration without assigning a destination.

Fix: update only affected success-path fixtures to create one valid test destination and inject a controlled loader.

### Runner tests fail

Cause: destination validation was incorrectly inserted into the destination-neutral sequence runner.

Fix: keep destination preflight root-owned and preserve runner-only tests.

### Completed report rejects empty sequence

Cause: report final result is missing when no step executed.

Fix: use a successful destination-handoff result as the completed report’s final result.

### Scene load completes but root remains Transitioning

Cause: loader success was not mapped through accepted `Completed` snapshot before report/event publication.

Fix: centralize completed handoff ordering.

### Cancellation fakes success

Cause: cancellation is checked after completion publication or loader result is ignored.

Fix: settle the loader, resolve cancellation ownership, and prohibit completion when cancellation owns the boundary.

### Runtime references `SceneAsset`

Cause: Editor authoring type leaked into Runtime.

Fix: keep runtime scene metadata as string data and reserve `SceneAsset` for later Editor tooling.

---

## 11. Rollback

Before implementation commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Configuration/EchoLaunchConfiguration.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReport.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReportBuilder.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Runtime/SceneLoading"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/SceneLoading.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs.meta"
```

After a pushed implementation commit, use `git revert`.

Do not redefine schema 2 during rollback.

---

## 12. Commit plan

### Authority commit

```text
echo-launch: approve FL-M3-08 destination and schema 3
```

### Implementation commit

```text
echo-launch: complete FL-M3-08 destination handoff
```

### Documentation closeout

```text
echo-launch: document FL-M3-08 completion
```

No commit or push is claimed without CMD evidence.

---

## 13. Stop point

Stop after one validated initial destination completes, one successful immutable report is finalized, `LaunchCompleted` fires exactly once, and the lifecycle reaches `Completed`.

Do not begin automatic startup, presentation, direct-scene initialization, setup/repair tooling, Test Lab scenes, or report export.

---

## 14. Next tentative checkpoint

**FL-M4-01 — Automatic Root Start Gate and Plain Status Presenter Contract**

Tentative only. Not authorized by FL-M3-08.

---

## 15. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Use a project-owned `LaunchDestination` ScriptableObject, destination schema 1, configuration schema 3, explicit runtime blocking for older schema, no silent migration, and no completion before destination activation.
