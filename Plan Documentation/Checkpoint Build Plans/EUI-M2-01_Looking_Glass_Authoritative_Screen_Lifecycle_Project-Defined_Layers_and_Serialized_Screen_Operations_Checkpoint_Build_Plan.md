---
tags:
  - sfgss/checkpoint
  - sfgss/wave/foundation
  - sfgss/ui
status: active
checkpoint: EUI-M2-01
updated: 2026-08-14
---
# EUI-M2-01 — The Looking Glass Authoritative Screen Lifecycle, Project-Defined Layers, and Serialized Screen Operations — Checkpoint Build Plan

**Checkpoint:** `EUI-M2-01`
**Status:** **ACTIVE / AUTHORIZED**
**Package:** The Looking Glass (`EchoUI`)
**Package ID:** `com.echodevgames.echo-ui`
**Package authority:** `SFGSS-PKG-ECHOUI-001` v1.3.0 Approved
**Workflow authority:** SFGSS-005 v1.6.0 + SFGSS-ADR-007 Green Path
**Learning gate:** PKG-LEARN-008 Complete, including bounded EUI-M2-01 revisit
**Starting repository baseline:** `c114ba2` — EUI-M1-02 final closeout
**Incoming retained evidence:** full EditMode **1130 / 1130 passed, 0 failed**; focused EchoUI **24 / 24**; manual Laboratory **10 / 10**
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0

> This checkpoint turns the proven scoped Screen behavior into an authoritative lifecycle and serialized operation pipeline while preserving designer ownership of layer topology, screen presentation, final composition, and project object lifetime.

## 1. Observable outcome

Starting from the completed M1 surface/context foundation, Looking Glass can execute deterministic structured Screen lifecycle operations against stable project-authored definitions and layer IDs. Projects may use variable authored layer topologies and any of three explicit view ownership modes. Rapid structural requests settle in strict FIFO order without racing or partially corrupting screen history.

The checkpoint must prove all of the following together:
- project-defined stable layer IDs and variable ordered authored layer topology;
- no hard runtime dependency on a fixed seven-layer count or reserved layer names;
- package starter layer arrangements are editable convenience rather than runtime law;
- explicit screen definition/runtime-entry separation;
- RootOwned, SceneOwned, and ExternalOwned screen view lifecycle;
- authoritative per-scope Push/Navigate, Replace, Reset/Return-to-root, Back, and Close behavior;
- designer-controlled suspended-screen visibility;
- suspended Screens remain non-interactive while another Screen is top in their scope;
- bounded strict FIFO structural screen mutation order;
- explicit rejection on overflow/invalid admission rather than silent coalescing/reordering;
- failures do not partially mutate history/ownership;
- definitions remain immutable during play;
- M1 independent Window/context/selection behavior remains green;
- zero hard runtime dependency on another Echo package.

## 2. Authorized scope

### 2.1 Project-defined layer topology
- Introduce a stable layer ID/configuration model and a runtime registry of resolved layer hosts.
- Authored projects may add, remove, reorder, or substitute layer definitions.
- Any package-recommended starter topology is convenience/template content only.
- Runtime validates duplicate/invalid IDs, duplicate order conflicts where relevant, and screen references to missing layers before mutation.
- The resolved topology is established at initialization; arbitrary runtime caller reordering is not part of M2-01.
- Display labels/hierarchy names are not stable IDs.

### 2.2 Screen definition and runtime entry
A Screen lifecycle separates immutable authored definition from mutable runtime entry. At minimum the effective definition carries enough information to resolve stable screen ID, navigation scope, target layer, view ownership mode/source, suspension presentation policy, and any immediate lifecycle metadata required by this slice.

Runtime entries hold current ownership/instance/history/active-suspended state. They are session state and are never written back to the definition asset.

### 2.3 View ownership modes
- `RootOwned`: Looking Glass may create and release the view from explicit project-authored factory/prefab data.
- `SceneOwned`: the view already exists in scene content; Looking Glass coordinates lifecycle/visibility/interaction but does not destroy it. Scene loss prunes the entry safely.
- `ExternalOwned`: project code explicitly supplies/registers the view; Looking Glass coordinates while the external owner retains object lifetime/destruction authority. Owner/view loss invalidates or prunes safely.

No ownership mode grants Looking Glass project-wide `DontDestroyOnLoad` or composition authority.

### 2.4 Screen structural operations
M2-01 authorizes structured operations equivalent to:
- Push / Navigate To;
- Replace top;
- Reset / Return To Root (replace scope history with one target root entry);
- Back (history pop/prune-invalid);
- Close under explicit policy/target validity.

Operation results are structured and terminal. Invalid IDs/scopes/layers/ownership/factory state must fail without partial authoritative mutation.

### 2.5 Suspension presentation
When Push makes a previous top Screen suspended, the effective screen policy may choose:
- Hidden;
- Visible;
- Preserve effective/authored visibility.

Regardless of visibility choice, the suspended Screen is non-interactive while another Screen is top in that navigation scope. Returning to it restores eligibility and its direct/context-driven state through the existing M1 mechanisms rather than inventing a second visibility authority.

### 2.6 Bounded strict FIFO serialization
- Screen structural mutations are admitted to one bounded FIFO operation path.
- Accepted requests execute one at a time in request submission order.
- M2-01 does not silently reorder, coalesce, replace, or drop accepted requests.
- Queue/admission capacity is explicit and bounded.
- A request that cannot be admitted returns a structured rejection and performs no mutation.
- The simple Laboratory may expose deterministic delayed/immediate proof hooks only if they are sample/test seams, not production domain behavior.

## 3. Explicit exclusions

EUI-M2-01 does **not** authorize:
- modal stack implementation, blocking policy, modal handles, or exact-once result awaiters;
- EUI-M2-02 implementation merely because it is named as the next likely slice;
- transition-driver/animation execution, cancellation, or timeout machinery;
- general focus-history restoration or focused-window arbitration;
- full EventSystem adoption/create/repair policy;
- automatic input detection;
- HUD region service/leases;
- notifications, prompts, or tooltip services;
- Motif schema/capture/apply;
- Builder tooling or broad primitive-prefab warehouse expansion;
- movable/resizable/persisted MMO layouts;
- Chronicle/Accord persistence or navigation-stack persistence;
- peer-package bridges;
- project-wide service composition or automatic `DontDestroyOnLoad`;
- runtime arbitrary layer topology editing after initialization;
- polished Reference Showcase or production art.

## 4. Exact implementation file families

No Runtime implementation file is changed by the authority/activation commit. Once that documentation commit is clean and synchronized, implementation is authorized only within the following bounded families.

### 4.1 Existing Runtime files allowed to change
- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Surfaces/UISurface.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Surfaces/UISurfaceOperationResult.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Surfaces/UISurfaceOperationStatus.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Navigation/UINavigationAction.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Navigation/UINavigationButton.cs` only as required to expose the newly authorized Laboratory/public screen operations
- `Packages/com.echodevgames.echo-ui/Runtime/EchoDevGames.EchoUI.Runtime.asmdef` only if required for existing Unity/uGUI references; no peer Echo reference may be added.

### 4.2 New layer files allowed
Under `Packages/com.echodevgames.echo-ui/Runtime/Layers/`:
- `UILayerId.cs`
- `UILayerDefinition.cs`
- `UILayerHost.cs`
- `UILayerRegistry.cs`
- related Unity `.meta` files.

### 4.3 New screen lifecycle files allowed
Under `Packages/com.echodevgames.echo-ui/Runtime/Screens/`:
- `UIScreenDefinition.cs`
- `UIScreenOwnershipMode.cs`
- `UIScreenSuspensionVisibility.cs`
- `UIScreenEntry.cs`
- `UIScreenHandle.cs`
- `UIScreenOperationKind.cs`
- `UIScreenOperationRequest.cs`
- `UIScreenOperationResult.cs`
- `UIScreenOperationStatus.cs`
- `UIScreenNavigator.cs`
- `UIScreenOperationQueue.cs`
- `IUIScreenFactory.cs` and one package-local default prefab factory only if RootOwned proof requires it; no DI/service-locator framework may be introduced.
- related Unity `.meta` files.

Exact member names/internal representation remain Level-4 implementation details. If a materially different file/type family is required, stop and reconcile the plan before widening scope.

### 4.4 Tests allowed
- new `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIScreenLifecycleTests.cs`
- new `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUILayerRegistryTests.cs` if separating layer cases improves clarity
- existing `EchoUIRootFoundationTests.cs` and `EchoUIContextAndSelectionTests.cs` only for retained-foundation setup/expectation adjustments directly caused by M2-01
- related `.meta` files.

### 4.5 Laboratory proof files allowed
- existing package Laboratory README/scene/runtime helper under `Samples~/LookingGlass_UI_Foundation_Laboratory/`
- a small additional Laboratory-owned lifecycle driver/helper if needed
- synchronized imported project-sample counterparts under `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/`
- related `.meta` files.

The top-right proof/debug safe zone remains reserved for Laboratory controls, stacking downward.

### 4.6 Closeout documentation allowed
At closeout reconcile only documentation required by SFGSS-005/SFGSS-ADR-007, including this plan, package/suite Current Notes, package README/CHANGELOG as applicable, Suite Health, test evidence, and the package specification only if implementation exposes a genuine contract correction.

## 5. Implementation rules
1. **Incoming regression first.** Before Runtime edits, rerun the full EditMode suite and require **1130 / 1130 passed, 0 failed** from `c114ba2`.
2. **Package independence remains absolute.** Runtime references no peer Echo runtime package.
3. **No fixed layer count.** Do not encode `7`, starter layer names, or hierarchy paths as runtime correctness requirements.
4. **Stable IDs over display names.** Layers/screens/scopes use normalized stable project-authored IDs.
5. **Authored topology is designer-owned.** Runtime validates/resolves; it does not rewrite authoring data.
6. **Definitions stay immutable.** Runtime entries/queues/history never write into definition assets.
7. **Ownership is explicit.** Destroy/release only RootOwned content that the lifecycle truly owns.
8. **SceneOwned/ExternalOwned lifetime remains external.** Lost objects are pruned/reported safely.
9. **One interactive Screen per scope.** Suspension visibility may vary, but lower Screen interaction is gated.
10. **Preserve M1 context semantics.** A screen becoming active/suspended must compose with existing context visibility/interactability rather than create a competing policy system.
11. **Strict FIFO means strict FIFO.** Accepted structural requests settle in submission order.
12. **Bound admission.** Queue capacity cannot grow without limit.
13. **No silent coalescing.** Duplicate/repeated requests may produce NoChange/structured results when processed, but accepted queue entries are not silently reordered/dropped.
14. **Preflight before authoritative mutation.** Invalid target/layer/ownership/factory failures cannot leave half-mutated history.
15. **Structured terminal results.** Success/NoChange/Rejected/Invalid/Failure states are explicit.
16. **M1 independent Windows stay independent.** M2-01 screen operations do not absorb Window/HUD/Overlay state into Screen history.
17. **No modal creep.** Modal types/services/results are not implemented in this slice.
18. **Laboratory helpers remain sample-owned.** Proof delays/commands do not leak domain assumptions into Runtime.
19. **No new suite authority.** If implementation requires one, stop and return to Declare/Authorize.

## 6. Focused automated tests
At minimum add coverage equivalent to:
1. `LayerRegistryAcceptsProjectDefinedVariableCountAndOrder`
2. `RuntimeDoesNotRequireRecommendedStarterLayerNames`
3. `DuplicateOrMissingLayerIdsFailWithoutScreenMutation`
4. `RootOwnedScreenCreatesAndReleasesOwnedView`
5. `SceneOwnedScreenNeverDestroysSceneView`
6. `ExternalOwnedScreenNeverDestroysExternalView`
7. `PushSuspendsPriorScreenAndAddsHistory`
8. `SuspendedVisibilityMayHideRemainVisibleOrPreserveVisibility`
9. `SuspendedScreenIsNonInteractableRegardlessOfVisibilityPolicy`
10. `ReplaceChangesTopWithoutGrowingHistory`
11. `ResetClearsHistoryAndEstablishesOneRootScreen`
12. `BackRestoresPreviousValidScreenAndPrunesInvalidHistory`
13. `CloseHonorsCurrentEntryPolicyWithoutCorruptingHistory`
14. `RapidAcceptedOperationsExecuteInStrictSubmissionOrder`
15. `QueueCapacityRejectsOverflowWithoutMutation`
16. `FactoryOrOwnershipFailureLeavesHistoryAndCurrentEntryUnchanged`
17. `RuntimeEntriesDoNotMutateScreenDefinitions`
18. `M2ScreenLifecyclePreservesM1ContextSelectionAndIndependentWindowBehavior`
19. `RuntimeAssemblyHasNoPeerEchoPackageDependency`

Focused tests should include at least one delayed test seam so FIFO is proved by observed settlement order rather than by synchronous call order alone.

## 7. Manual Laboratory proof
Extend the existing deliberately plain Laboratory only enough to prove lifecycle behavior. Suggested acceptance matrix:
1. Display the resolved authored layer order and prove a non-default custom layer ID works.
2. SceneOwned `main-menu -> settings` Push and Back behave correctly.
3. Prove one suspension policy where prior Screen remains visible but becomes non-interactable.
4. Prove another suspension policy where prior Screen hides.
5. Replace the current Screen and verify history depth does not grow.
6. Reset/Return-to-root and verify prior history is cleared.
7. Prove one RootOwned screen can be created/opened/closed/released without touching scene-owned screens.
8. Prove one ExternalOwned screen can be admitted/closed without Looking Glass destroying the supplied object.
9. Queue a visible rapid sequence such as Push A -> Push B -> Back and display actual settlement order matching submission order.
10. Repeat retained M1 proof: independent `default-window`, pause/context behavior, pointer/controller selection, and normal Back still work.

A small Laboratory-owned evidence console/log may show current scope, top Screen, history depth, ownership mode, layer ID/order, queue depth, operation sequence, and settlement result. Do not turn it into production UI architecture.

## 8. Green Path execution
1. Verify branch/repository state is expected and clean.
2. Verify authority/activation documentation is committed and synchronized.
3. Run incoming full EditMode regression and require **1130 / 1130, 0 failed** before Runtime edits.
4. Apply only exact EUI-M2-01 implementation scope.
5. Allow Unity compile/import.
6. Run focused EUI-M2-01 layer/lifecycle/queue tests.
7. Run full EditMode suite and require zero failures; record actual discovered/passed count.
8. Perform the ten-item manual Laboratory proof and retain evidence.
9. Synchronize package/imported Laboratory content only where intentionally authored.
10. Run whitespace/scope validation without rewriting Unity/vendor assets merely to satisfy formatting.
11. Commit implementation as a distinct boundary.
12. Reconcile package/suite documentation/evidence in a distinct closeout boundary.
13. Push and verify clean synchronized repository state.

Green Path stops on compile/test/manual-proof failure, unexpected path scope, baseline mismatch, peer dependency, authored-definition mutation, ownership violation, non-FIFO accepted settlement, partial history mutation, or authority-changing discovery.

## 9. Stop point
EUI-M2-01 is complete only when:
- incoming **1130 / 1130** floor is re-established before Runtime implementation;
- variable project-authored layer topology works without a fixed-count assumption;
- RootOwned/SceneOwned/ExternalOwned lifecycle is proven;
- Push/Replace/Reset/Back/Close semantics are deterministic;
- suspension visibility remains designer-controlled while interaction exclusivity is preserved;
- bounded FIFO settlement is proven under rapid requests;
- failures/rejections do not partially mutate history/ownership;
- focused tests are green;
- final full EditMode suite has zero failures;
- ten manual Laboratory checks pass;
- M1 context/selection/window behavior remains green;
- Runtime retains no peer Echo dependency;
- documentation matches committed implementation;
- HEAD/origin and working tree are clean at closeout.

**Then stop.** Do not begin EUI-M2-02 modal results, transitions, focus-history restoration, EventSystem policy, HUD/transient systems, Motifs, Builder, primitive-library expansion, persistence, or peer bridges in the same checkpoint.

## 10. Follow-on direction, not authorization
The intended next bounded Runtime Core slice is **EUI-M2-02 — Blocking Modal Lifecycle and Exact-Once Modal Results**. Naming it here does not activate it. It must pass its own Learn → Declare → Authorize gate.

---

## Activation record
- JIT learning/intake: **Complete**
- Package declaration: **Complete**
- Package authority reconciliation: **SFGSS-PKG-ECHOUI-001 v1.3.0**
- Checkpoint authorization: **EUI-M2-01 ACTIVE / AUTHORIZED**
- Starting implementation baseline: `c114ba2`
- Incoming full EditMode floor: **1130 / 1130 passed, 0 failed**
- Runtime implementation at activation: **Not started**
- Activation Git commit: **pending authority/activation commit produced by the apply bundle**
