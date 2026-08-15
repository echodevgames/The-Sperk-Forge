---
tags:
  - sfgss/checkpoint
  - sfgss/wave/foundation
  - sfgss/ui
status: active
checkpoint: EUI-M2-02
updated: 2026-08-14
---

# EUI-M2-02 — The Looking Glass Blocking Modal Lifecycle, Exact-Once Results, and UI-Scoped Interaction Blocking — Checkpoint Build Plan

**Checkpoint:** `EUI-M2-02`
**Status:** **ACTIVE / AUTHORIZED**
**Package:** The Looking Glass (`EchoUI`)
**Package ID:** `com.echodevgames.echo-ui`
**Package authority:** `SFGSS-PKG-ECHOUI-001` v1.4.0 Approved
**Planning baseline:** `d5b9a733c0fb68fe91e2fa73185c61bbc8ff81e6`
**Unity baseline:** 6000.3.8f1
**Incoming full EditMode floor:** **1153 / 1153 passed, 0 failed**
**Runtime implementation at activation:** **Not started**

> This checkpoint is the second and final currently declared M2 Runtime Core slice. It adds authoritative blocking-modal lifecycle on top of the completed M2-01 Screen lifecycle. It does not authorize M3 focus/presentation work or later MVP surface families.

## 1. Outcome

Deliver the smallest independently provable blocking-modal runtime that:

1. opens and stacks modal entries deterministically;
2. allows only the top eligible modal to receive normal Looking Glass interaction;
3. uses project-defined stable result IDs for normal semantic completion;
4. settles each admitted modal opening exactly once;
5. distinguishes structural `Aborted` from semantic Cancel;
6. reuses RootOwned / SceneOwned / ExternalOwned lifetime rules;
7. returns a fresh awaitable/handle generation for every admitted opening;
8. routes Back to the top modal according to designer-authored dismissal policy;
9. blocks lower Looking Glass UI interaction without claiming gameplay-input/pause/time/cursor authority;
10. applies explicit safe Screen-mutation policy while blocking modals exist;
11. retains all M1 and M2-01 behavior.

## 2. Declared contract

### 2.1 Modal stack

- Blocking modals may stack.
- The stack is ordered by admitted opening.
- Only the top eligible modal receives normal Looking Glass interaction.
- Lower entries remain live and handle-addressable.
- A lower modal may be completed/aborted by its owner out of order without closing, reordering, or stealing interaction from the current top modal.
- Capacity is bounded. Overflow is rejected before partial view/entry mutation.

### 2.2 Stable result identity

Normal semantic completion uses a project-defined stable `UIModalResultId` or equivalent neutral value type.

The package reserves no game meaning for IDs such as:

```text
confirm
cancel
delete
easy
normal
hard
```

Display text, localization, gameplay meaning, and domain commands remain project-owned.

EUI-M2-02 does **not** require arbitrary typed domain payload transport. A later checkpoint may add such transport only if a real use case justifies it.

### 2.3 Exact-once settlement

Each admitted modal opening owns one runtime generation and one fresh completion channel.

The rule is:

> The first valid terminal completion wins.

After terminal commit:

- its awaiter settles once;
- completion notification fires at most once;
- the entry cannot settle again;
- stale or repeated completion attempts return a harmless structured stale/already-completed result;
- a stale handle from an older generation cannot complete a later reopening of the same definition.

### 2.4 Structural `Aborted`

Semantic Cancel and structural teardown are different truths.

Normal project/user cancellation is represented by a project-authored result ID such as `cancel`.

Unexpected post-admission lifecycle loss uses a structural outcome such as:

```text
Outcome = Aborted
Reason  = OwnerLost | ViewLost | RootShutdown | ...
```

Owner/view loss or shutdown must not fabricate a semantic result ID.

Validation/factory failure **before successful admission** returns an open-operation failure and leaves:

- no live modal entry;
- no leaked awaiter;
- no orphaned RootOwned instance;
- no blocking residue.

### 2.5 View ownership

EUI-M2-02 reuses the M2-01 lifetime model:

- `RootOwned` — Looking Glass creates and releases the modal instance.
- `SceneOwned` — Looking Glass coordinates the scene-authored view but does not destroy it.
- `ExternalOwned` — Looking Glass coordinates an explicitly supplied project-owned view but does not destroy it.

No fourth ownership model is introduced by this checkpoint.

### 2.6 Back/dismiss policy

Back routes to the top blocking modal before Screen history.

A modal definition may choose:

- **Disabled** — Back leaves the modal active and returns a structured blocked/unhandled result.
- **Complete With Result ID** — Back completes the modal with one configured project-defined stable result ID.

EUI-M2-02 does not introduce general historical focus restoration.

### 2.7 UI/input authority boundary

A blocking modal guarantees that lower **Looking Glass UI** does not receive normal interaction through:

- uGUI pointer/raycast eligibility;
- UI navigation;
- submit/cancel routing owned by the Looking Glass modal path;
- ordinary Looking Glass Back routing.

It does **not**:

- disable gameplay action maps;
- own or suppress project WASD;
- set `Time.timeScale`;
- decide pause;
- own project cursor mode/lock;
- switch Will/Vessel/Input System gameplay contexts;
- freeze simulation.

Project code or optional future bridges may observe read-only modal blocking state and decide whether gameplay input should continue.

The Laboratory may use a tiny sample-owned external-action simulator to prove that program/gameplay-like behavior may continue while lower uGUI is blocked.

### 2.8 Screen mutation while modal-blocked

Designer/project policy supports two bounded modes:

**Reject** — default / simple path

- Screen Push/Replace/Reset/Back/Close requests that would mutate Screen history while a blocking modal stack is active are rejected before mutation.
- Result is structured and explicit, conceptually `BlockedByModal`.

**DeferUntilModalStackClears** — advanced opt-in

- eligible Screen structural requests may be accepted into a bounded deferred path;
- original submission order is retained;
- requests execute only after the blocking modal stack becomes empty;
- settlement still obeys the existing M2-01 strict FIFO Screen rules;
- overflow/rejection is explicit.

No policy in EUI-M2-02 silently mutates Screen history underneath an active blocking modal.

### 2.9 Visual presentation

Modal visuals remain project/designer authored.

Looking Glass may coordinate visibility/interactivity needed for modal lifecycle, but EUI-M2-02 does not prescribe:

- dim percentage;
- gray overlay;
- blur;
- animated backdrop;
- transition style;
- production art.

A RootOwned/SceneOwned/ExternalOwned modal view may include its own project-authored backdrop.

## 3. Authorized Runtime scope

Expected package-local additions/refinements are bounded to areas such as:

```text
Runtime/
├── Core/
│   └── EchoUIRoot.cs                         [integration only]
├── Screens/
│   ├── UIScreenNavigator.cs                  [modal-block admission integration]
│   └── related operation result/policy types [only as required]
└── Modals/
    ├── UIModalId.cs
    ├── UIModalResultId.cs
    ├── UIModalOutcome.cs
    ├── UIModalAbortReason.cs
    ├── UIModalHandle.cs
    ├── UIModalDefinition.cs
    ├── UIModalEntry.cs
    ├── UIModalResult.cs
    ├── UIModalBackPolicy.cs
    ├── UIModalScreenMutationPolicy.cs
    ├── IUIModalFactory.cs
    └── UIModalService.cs
```

Exact file names may be refined during implementation when they do not alter the declared contract.

The implementation should reuse existing M2-01 ownership/factory concepts where practical rather than create a competing lifetime framework.

## 4. Explicitly not authorized

EUI-M2-02 does **not** authorize:

- full EventSystem adoption policy;
- general focus-history restoration;
- generalized modal focus graph/containment system beyond the interaction blocking required here;
- transition drivers or animation sequencing;
- generalized dim/blur/backdrop effects;
- HUD region service;
- notifications;
- prompts/tooltips;
- Motif runtime/editor work;
- Looking Glass Builder;
- broad primitive/prefab warehouse expansion;
- arbitrary typed/domain modal payload transport;
- persistence of modal state/results;
- Chronicle/Accord/Will/Pulse/Vessel/Resonance/First Light bridges;
- automatic gameplay-input context switching;
- automatic pause/time-scale/cursor ownership;
- project-wide `DontDestroyOnLoad` composition;
- polished Reference Showcase work.

These capabilities remain visible in the package roadmap/backlog. They are intentionally **not forgotten**, only deferred.

## 5. Runtime invariants

The completed Runtime must preserve:

1. zero hard dependency on another Echo package;
2. project-owned lifetime composition;
3. no gameplay/pause/input authority transfer into Looking Glass;
4. one and only one terminal result per admitted modal generation;
5. no stale-handle completion of a later generation;
6. top-only modal interaction;
7. safe out-of-order lower-modal disposal;
8. bounded active/deferred capacity;
9. no partial modal stack mutation on validation/factory/capacity failure;
10. no Screen history mutation under `Reject`;
11. FIFO preservation under `DeferUntilModalStackClears`;
12. retained M1/M2-01 Screen/Window/context/selection behavior.

## 6. Automated proof

### 6.1 Incoming gate

Before Runtime edits, on the activation commit:

```text
Full EditMode
1153 / 1153 passed
0 failed
0 skipped
0 inconclusive
```

Any red result stops implementation.

### 6.2 Focused M2-02 families

Automated coverage must include at minimum:

- stable modal/result ID validity/equality;
- open one blocking modal;
- nested modal top-only interaction;
- RootOwned create/release;
- SceneOwned no lifetime theft;
- ExternalOwned no lifetime theft;
- active capacity overflow rejection;
- lower-modal out-of-order cleanup;
- stable result ID completion;
- repeated/racing completion first-wins exact once;
- owner/view loss -> `Aborted`;
- shutdown -> `Aborted`;
- failed admission leaves no live entry/blocking residue;
- fresh awaiter/handle generation per opening;
- stale handle cannot complete reopened modal;
- Back disabled;
- Back completes using configured stable result ID;
- Screen mutation `Reject` leaves history unchanged;
- Screen mutation `DeferUntilModalStackClears`;
- multiple deferred Screen requests preserve FIFO;
- deferred capacity overflow/rejection;
- retained M2-01 Screen strict FIFO behavior;
- retained M1 context/selection/window behavior.

Focused tests must not be relaxed to make an implementation pass.

## 7. Laboratory proof

Extend the existing plain Looking Glass UI Foundation Laboratory only enough to prove M2-02.

Keep:

- the top-right proof/debug safe zone;
- package/imported sample parity;
- the ordinary uGUI path;
- sample-only simulation clearly separated from package authority.

The Laboratory should use simple project-neutral modal examples, e.g. a confirmation modal and nested confirmation, not polished production UI.

### 7.1 Manual acceptance

Record Pass/Fail for each:

1. **Open blocking modal** — open one modal over Main Menu; modal is active and underlying Looking Glass UI cannot be clicked/navigated normally.
2. **Stable result ID** — complete it with an authored result such as `confirm`; the displayed/awaited terminal result reports exactly that stable ID.
3. **Exact once** — attempt completion again; the first result remains authoritative and no second completion fires.
4. **Nested top-only interaction** — open two modals; only the top modal is interactive.
5. **Out-of-order lower cleanup** — abort/close the lower modal by handle while the top remains active; stack remains valid and top stays interactive.
6. **Back policy** — prove one dismissible modal maps Back to its configured result ID and one non-dismissible modal remains active.
7. **Structural abort** — simulate owner/view loss; result reports `Aborted`, not semantic `cancel`.
8. **Ownership lifetime** — prove RootOwned runtime instance releases; SceneOwned/ExternalOwned supplied objects remain alive after modal settlement.
9. **Screen Reject policy** — while modal is open, request a Screen change; it is explicitly blocked and Screen history remains unchanged.
10. **Screen Defer policy** — queue two Screen mutations, settle the modal stack, and confirm deferred requests execute afterward in original FIFO order.
11. **Gameplay/input separation** — while modal blocks lower uGUI, trigger the Laboratory's external project-action simulator and confirm that external behavior may continue because Looking Glass did not seize gameplay input/pause authority.
12. **Retained behavior** — reconfirm representative M2-01 Screen navigation plus M1 context/selection/independent Window behavior.

Any failed item stops closeout.

## 8. Green Path / collaboration fast path

To reduce conversational toll booths while retaining rigor:

1. activation documentation is one bounded apply/commit/push;
2. one full incoming EditMode run establishes `1153 / 1153` on the activation commit;
3. after a green incoming gate, one Runtime/test apply bundle may implement the bounded M2-02 core without another design approval;
4. compile or focused-test red stops immediately;
5. successful focused tests advance directly to the Laboratory apply/proof slice;
6. successful Laboratory + final full regression advance to implementation sealing;
7. implementation and documentation closeout remain separate commits;
8. no next checkpoint activates automatically.

Generated solution churn such as `The Sperk Forge.slnx` is not package work. Green Path helpers should restore/ignore only known generated solution churn rather than turning it into another approval prompt.

## 9. Documentation reconciliation at closeout

Closeout must reconcile as applicable:

- this Checkpoint Build Plan;
- `Plan Documentation/Current Notes.md`;
- package Developer `Current Notes.md`;
- package README;
- package CHANGELOG;
- Suite Graph Roadmap;
- Suite Health Check;
- package specification only if implementation discovers a genuine contract correction.

No new suite ADR is expected unless implementation reveals a cross-package/suite authority change.

## 10. Stop conditions

Stop and return to authority if implementation discovers a need to:

- own gameplay input/pause/time/cursor behavior;
- add a hard peer Echo dependency;
- persist modal state/results;
- add general domain payload transport as a core requirement;
- alter the established RootOwned/SceneOwned/ExternalOwned lifetime meaning;
- silently mutate Screen history under a blocking modal;
- weaken exact-once result semantics;
- add full focus/transition/HUD/transient/Motif/Builder work;
- change serialized/public compatibility beyond this declared slice.

Routine compile corrections, test maintenance, sample wiring fixes, and exact-contract implementation details remain Green Path work.

## 11. Activation record

- JIT learning/intake: **Complete**
- Package declaration: **Complete**
- Package authority reconciliation: **SFGSS-PKG-ECHOUI-001 v1.4.0**
- Checkpoint authorization: **EUI-M2-02 ACTIVE / AUTHORIZED**
- Starting repository baseline: `d5b9a733c0fb68fe91e2fa73185c61bbc8ff81e6`
- Retained full EditMode proof: **1153 / 1153 passed, 0 failed**
- Retained focused EchoUI: **47 / 47 passed, 0 failed**
- Retained EUI-M2-01 focused: **23 / 23 passed, 0 failed**
- Retained manual Laboratory: **10 / 10 PASS**
- Runtime implementation at activation: **Not started**
- Activation Git commit: **pending authority/activation apply**
- Next gate: **re-establish 1153 / 1153 on activation commit before Runtime edits**

**Stop after activation. Do not begin Runtime implementation until the incoming gate is green.**
