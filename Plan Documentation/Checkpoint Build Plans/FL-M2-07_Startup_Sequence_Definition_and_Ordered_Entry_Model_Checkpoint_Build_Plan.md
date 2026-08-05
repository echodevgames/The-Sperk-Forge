# FL-M2-07 — Startup Sequence Definition and Ordered Entry Model Checkpoint Build Plan

**Document ID:** FL-M2-07
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M2 — Runtime Core
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository/workspace:** The Sperk Forge
**Unity baseline:** Unity 6000.3.8f1
**Public Unity floor:** Unity 6000.0
**Workflow authority:** SFGSS-005 v1.4.0
**Implementation baseline:** `1d15399`
**Starting Runtime Play Mode result:** 117 passed, 0 failed, 0 ignored
**Last updated:** August 4, 2026

> FL-M2-06 gave First Light the project’s sealed launch dossier. FL-M2-07 places an ordered checklist inside it, but nobody executes the checklist yet.

---

## 1. Purpose and Observable Outcome

Create the immutable authored model for one ordered startup sequence.

When this checkpoint is complete:

- Unity can create a project-owned `StartupSequence` asset.
- Unity can create concrete project or bridge step-definition assets derived from `StartupStepDefinition`.
- Every new step definition receives one stable step ID and schema version.
- Every new sequence receives one stable sequence ID and schema version.
- Each sequence contains an explicitly ordered list of embedded `StartupSequenceEntry` records.
- Every sequence entry receives one stable entry ID.
- Each entry stores:
  - enabled state;
  - one immutable step-definition reference.
- `EchoLaunchConfiguration` can reference one startup sequence.
- Public reads preserve authored order.
- Shared assets remain unchanged during Play Mode inspection and root lifetime.
- No executor, runner, lifecycle advancement, preflight, timeout, policy, report, or scene transition exists yet.
- Twenty-four new Runtime Play Mode tests pass.
- Full expected Runtime Play Mode total is 141 passed, 0 failed, 0 ignored.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 — suite authority and standalone package rules.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 — approved First Light package contract.
3. SFGSS-003 v1.1.0 — stable domain IDs, display-name separation, schema behavior, and immutable authored assets.
4. SFGSS-005 v1.4.0 — checkpoint planning, visible code, testing, evidence, and stop rules.
5. PKG-LEARN-001 — First Light learning review.
6. FL-M2-06 implementation and documentation closeout.
7. Current package architecture and 117-test baseline.

No authority conflict was found.

---

## 3. Starting Conditions

Before implementation:

- `main` and `origin/main` are synchronized at `1d15399`.
- The working tree is clean.
- Unity compiles with zero errors.
- FL-M2-01 through FL-M2-06 are complete.
- Runtime Play Mode baseline is:
  - Passed: `117`
  - Failed: `0`
  - Ignored: `0`
- `EchoLaunchConfiguration` currently contains only:
  - configuration identity;
  - configuration schema version.
- `EchoLaunchRoot` passively exposes the accepted configuration.
- No startup-sequence type exists.
- No startup-step-definition base type exists.
- No sequence entry, step policy, executor, runner, or preflight exists.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Architectural Constraints

The checkpoint must preserve these boundaries:

- Definitions are project-owned authored ScriptableObjects.
- Mutable execution state never lives in definition assets.
- Stable identity is independent from:
  - asset name;
  - display name;
  - file path;
  - list index;
  - Unity runtime instance ID.
- List order controls execution order later, but list index is not durable identity.
- Sequence, step, and entry IDs are canonical lowercase 32-character hexadecimal strings.
- Runtime code detects malformed identity or unsupported schema but does not repair it.
- Display labels may change without changing stable IDs.
- A duplicated asset or copied list entry may duplicate its domain ID; collision validation and explicit repair belong to later Editor/preflight tooling.
- An empty sequence is legal authored data at this checkpoint.
- A null step reference is legal authored data at this checkpoint.
- Duplicate step references and duplicate IDs are not repaired at runtime.
- No other Sperk’s Forge package becomes a dependency.
- No `UnityEditor` API enters Runtime.
- Existing lifecycle and notification behavior remains unchanged.

---

## 5. Authorized Scope

FL-M2-07 authorizes:

1. `StartupStepDefinition` as an abstract immutable ScriptableObject base.
2. Stable step identity.
3. Step-definition schema version `1`.
4. Separate authored display label.
5. `StartupSequenceEntry` as one embedded serializable ordered record.
6. Stable entry identity.
7. Enabled/disabled authored state.
8. One `StartupStepDefinition` reference per entry.
9. `StartupSequence` as a project-owned ScriptableObject.
10. Stable sequence identity.
11. Sequence schema version `1`.
12. Serialized ordered entry list.
13. Read-only indexed access preserving authored order.
14. Passive `StartupSequence` reference on `EchoLaunchConfiguration`.
15. Runtime Play Mode tests for identity, schema, order, binding, null/empty data, and immutability.
16. Unity Create menu verification for `StartupSequence`.
17. Unity-generated `.meta` files.
18. The established checkpoint documentation family at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- `IStartupStepExecutor`
- `StartupStepContext`
- `StartupSequenceRunner`
- `StartupStepExecution`
- `StartupStepPolicy`
- required/optional failure action
- timeout
- retry
- skip policy beyond the authored enabled flag
- automatic lifecycle advancement
- configuration preflight
- sequence preflight
- duplicate-ID collision scans
- runtime repair
- Editor migration/repair tools
- custom inspectors
- setup windows
- launch report construction
- step started/progress/completed events
- splash sequence
- launch destination
- scene loading
- root lifetime
- direct-scene initializer
- Test Lab scenes
- peer-package bridges

Stop after the sequence and entry definitions can be authored, bound, read in order, and proven immutable.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Steps/
│       ├── StartupStepDefinition.cs
│       ├── StartupStepDefinition.cs.meta
│       ├── StartupSequenceEntry.cs
│       ├── StartupSequenceEntry.cs.meta
│       ├── StartupSequence.cs
│       └── StartupSequence.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── StartupSequenceDefinitionTests.cs
            └── StartupSequenceDefinitionTests.cs.meta
```

### Modify

```text
Packages/com.echodevgames.echo-launch/
└── Runtime/
    └── Configuration/
        └── EchoLaunchConfiguration.cs
```

### Checkpoint Plan

```text
Plan Documentation/
└── Checkpoint Build Plans/
    └── FL-M2-07_Startup_Sequence_Definition_and_Ordered_Entry_Model_Checkpoint_Build_Plan.md
```

### Closeout Documentation

Use the established nine-file pattern:

```text
Packages/com.echodevgames.echo-launch/
├── CHANGELOG.md
├── README.md
└── Documentation~/
    ├── Index.md
    └── Developer/
        ├── Architecture.md
        ├── Current Notes.md
        ├── Checkpoints/
        │   └── FL-M2-07_Startup_Sequence_Definition_and_Ordered_Entry_Model.md
        └── Test Reports/
            └── FL-M2-07_Startup_Sequence_Definition_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M2-07_First_Light_Startup_Sequence_Definition_Completion.md
```

No other file is authorized.

---

## 8. Definition Contracts

### 8.1 `StartupStepDefinition`

The abstract base must serialize:

- `stepId`
- `schemaVersion`
- `displayName`

It must expose:

- `StepId`
- `SchemaVersion`
- `DisplayName`
- internal `HasValidIdentity`
- internal `HasSupportedSchema`

It must not expose execution behavior yet.

Canonical step ID:

- 32 characters;
- lowercase hexadecimal;
- ASCII only;
- no separator;
- no whitespace;
- generated once for a newly created definition instance.

Display name:

- is presentation metadata;
- is not identity;
- may change without breaking references;
- falls back to the Unity object name when the authored label is blank.

### 8.2 `StartupSequenceEntry`

The embedded serializable entry must serialize:

- `entryId`
- `enabled`
- `stepDefinition`

It must expose read-only:

- `EntryId`
- `IsEnabled`
- `StepDefinition`
- internal `HasValidIdentity`

The entry does not own schema version because its serialized shape is governed by the containing sequence schema.

The entry ID remains separate from list index. Reordering entries changes order, not identity.

### 8.3 `StartupSequence`

The asset must serialize:

- `sequenceId`
- `schemaVersion`
- ordered `List<StartupSequenceEntry>`

It must expose:

- `SequenceId`
- `SchemaVersion`
- `EntryCount`
- `GetEntry(index)`
- internal `HasValidIdentity`
- internal `HasSupportedSchema`

`GetEntry` must:

- preserve authored order;
- return the stored entry reference;
- reject negative and out-of-range indices clearly;
- avoid exposing the mutable backing `List`.

An empty list is valid data during this checkpoint.

### 8.4 `EchoLaunchConfiguration`

Add one passive serialized reference:

```csharp
[SerializeField]
private StartupSequence startupSequence;
```

Expose:

```csharp
public StartupSequence StartupSequence =>
    startupSequence;
```

This binding does not validate, execute, clone, repair, or mutate the sequence.

---

## 9. Implementation Phases

### Phase A — Step Definition Identity

Create:

- `StartupStepDefinition.cs`

Verify:

- Unity compiles with zero errors.
- No Create menu item is expected because the base is abstract.
- Existing 117 Play Mode tests remain available.

### Phase B — Entry and Sequence Definitions

Create:

- `StartupSequenceEntry.cs`
- `StartupSequence.cs`

Verify:

- Unity compiles.
- Create menu contains:
  - `EchoDevGames > First Light > Startup Sequence`
- A temporary sequence asset can be created.
- Empty sequence asset causes no runtime behavior.

### Phase C — Configuration Binding

Modify:

- `EchoLaunchConfiguration.cs`

Verify:

- The configuration Inspector shows a Startup Sequence field.
- Assigning a sequence does not create a root or change lifecycle.
- No preflight or warning runs.

### Phase D — Automated Tests

Create:

- `StartupSequenceDefinitionTests.cs`

Run all Runtime Play Mode tests.

Expected:

- Passed: `141`
- Failed: `0`
- Ignored: `0`

### Phase E — Git and Documentation Closeout

1. Review only checkpoint-owned files.
2. Clean Unity-generated folder metadata automatically when needed.
3. Commit and push implementation.
4. Generate one-command nine-file documentation closeout.
5. Commit and push documentation.
6. Confirm clean synchronized repository.
7. Stop before executor or runner work.

---

## 10. Planned Test Registry

| ID | Test | Expected |
|---|---|---|
| FL-M2-07-T-001 | New step ID | Canonical format |
| FL-M2-07-T-002 | Separate step definitions | Different IDs |
| FL-M2-07-T-003 | Repeated step ID reads | Stable |
| FL-M2-07-T-004 | Step schema | Current version |
| FL-M2-07-T-005 | Step display label | Preserved and separate from ID |
| FL-M2-07-T-006 | Malformed step ID | Invalid without repair |
| FL-M2-07-T-007 | Unsupported step schema | Unsupported without rewrite |
| FL-M2-07-T-008 | New entry ID | Canonical format |
| FL-M2-07-T-009 | Separate entries | Different IDs |
| FL-M2-07-T-010 | Entry default state | Enabled |
| FL-M2-07-T-011 | Entry step reference | Preserved |
| FL-M2-07-T-012 | Malformed entry ID | Invalid without repair |
| FL-M2-07-T-013 | New sequence ID | Canonical format |
| FL-M2-07-T-014 | Separate sequences | Different IDs |
| FL-M2-07-T-015 | Repeated sequence ID reads | Stable |
| FL-M2-07-T-016 | Sequence schema | Current version |
| FL-M2-07-T-017 | Generated sequence validity | Valid and supported |
| FL-M2-07-T-018 | Malformed sequence ID | Invalid without repair |
| FL-M2-07-T-019 | Unsupported sequence schema | Unsupported without rewrite |
| FL-M2-07-T-020 | Empty sequence | Count zero |
| FL-M2-07-T-021 | Ordered entries | Indexed reads preserve order |
| FL-M2-07-T-022 | Invalid index | Clear range exception |
| FL-M2-07-T-023 | Configuration binding | Assigned sequence exposed |
| FL-M2-07-T-024 | Definition lifecycle | Assets remain unchanged |
| FL-M2-07-T-025 | Full Runtime Play Mode suite | 141 / 0 / 0 |
| FL-M2-07-T-026 | Create menu | Temporary sequence asset created |
| FL-M2-07-T-027 | Git scope | Only authorized files |

The automated count is 24 new tests. The final three rows include full-suite, manual, and Git evidence.

---

## 11. Unity Editor Verification

### Phase A

No asset is created because `StartupStepDefinition` is abstract.

### Phase B

Create:

```text
Assets/Settings/FL-M2-07_TestStartupSequence.asset
```

through:

```text
Create
-> EchoDevGames
-> First Light
-> Startup Sequence
```

Expected:

- Empty ordered entry list.
- No GameObject.
- No root.
- No lifecycle state.
- No execution.
- No warning.

### Phase C

Create or reuse a temporary launch configuration and assign the temporary sequence.

Expected:

- One visible Startup Sequence reference.
- No automatic validation or execution.

Delete all temporary verification assets before Git review.

---

## 12. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Step base appears in Create menu | Base received `CreateAssetMenu` | Remove the attribute from abstract base |
| Sequence menu is missing | Attribute or compile failure | Correct `StartupSequence` attribute |
| IDs include uppercase or hyphens | Wrong GUID format | Use `ToString("N")` |
| Reordering changes ID | ID derived from index | Keep serialized domain ID |
| Sequence exposes mutable `List` | Public list property | Use count plus indexed getter |
| Asset changes during Play Mode | Runtime writes to definitions | Remove runtime mutation |
| Assigning sequence starts launch | Binding crossed into execution | Remove execution/preflight |
| Existing 117 tests fail | Prior behavior changed | Restore prior behavior |
| Duplicate IDs auto-repair | Runtime guessed ownership | Remove repair behavior |
| Temporary assets appear in Git | Manual evidence not deleted | Delete only temporary assets |

---

## 13. Rollback

To return to `1d15399` behavior:

1. Restore `EchoLaunchConfiguration.cs`.
2. Remove:
   - `StartupStepDefinition.cs`
   - `StartupSequenceEntry.cs`
   - `StartupSequence.cs`
   - `StartupSequenceDefinitionTests.cs`
   - their `.meta` files.
3. Remove temporary sequence/configuration assets.
4. Refresh Unity.
5. Confirm 117 Runtime Play Mode tests pass.
6. Revert only FL-M2-07 documentation if abandoned.

No save data, scenes, prefabs, build settings, or project settings require recovery.

---

## 14. Commit Plan

Preferred implementation commit:

```text
Add EchoLaunch startup sequence definitions
```

Preferred adjacent documentation commit:

```text
Close FL-M2-07 startup sequence documentation
```

No commit or remote state is claimed until CMD evidence exists.

---

## 15. Completion Criteria

- [ ] Clean starting repository at `1d15399`.
- [ ] `StartupStepDefinition` exists.
- [ ] Stable step ID and schema exist.
- [ ] Display label remains separate from identity.
- [ ] `StartupSequenceEntry` exists.
- [ ] Stable entry ID exists.
- [ ] Enabled state and step reference are authored data.
- [ ] `StartupSequence` exists.
- [ ] Stable sequence ID and schema exist.
- [ ] Ordered indexed access preserves authored order.
- [ ] Mutable backing list is not publicly exposed.
- [ ] `EchoLaunchConfiguration` references one sequence.
- [ ] No execution behavior exists.
- [ ] No runtime repair exists.
- [ ] Temporary assets are removed.
- [ ] Twenty-four new tests pass.
- [ ] Full suite is 141 passed, 0 failed, 0 ignored.
- [ ] Implementation commit and push are confirmed.
- [ ] Nine-file documentation closeout is committed and pushed.
- [ ] Working tree is clean.
- [ ] Work stops before executor/runner implementation.

---

## 16. Next Recommended Checkpoint

**FL-M2-08 — Startup Step Policy and Executor Contract**

Expected future scope:

- `StartupStepPolicy`
- MVP failure actions
- `IStartupStepExecutor`
- fresh executor creation contract
- no sequence runner yet

FL-M2-07 does not authorize FL-M2-08.

---

## 17. Approval

**Decision:** Active and authorized
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 4, 2026
**Conditions:** Keep all assets immutable, preserve authored order without treating index as identity, bind the sequence passively, prove 141 Runtime Play Mode tests, and stop before policy, executor, runner, or preflight behavior.
