# FL-M2-06 — Launch Configuration Identity and Root Binding Checkpoint Build Plan

**Document ID:** FL-M2-06
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
**Implementation baseline:** `c842cef`
**Last updated:** August 4, 2026

> The launch root already owns the front door. This checkpoint gives it the project’s sealed instruction card, but does not let it begin reading or executing the launch plan.

---

## 1. Purpose and Observable Outcome

Create the smallest project-owned First Light configuration asset and bind one configuration reference to the authoritative `EchoLaunchRoot`.

When this checkpoint is complete:

- Unity can create an `EchoLaunchConfiguration` asset from the Create menu.
- Every newly created configuration receives one canonical runtime-safe stable ID.
- Every configuration declares its serialized schema version.
- The authoritative root exposes its assigned configuration through a read-only property.
- Duplicate and stale roots expose no configuration authority.
- Binding or destroying a root does not mutate the configuration asset.
- All existing 102 Runtime Play Mode tests remain green.
- Fifteen new configuration-binding tests pass.

The observable result is a root that can identify which project-owned launch configuration belongs to the accepted launch session, while startup validation and execution remain deliberately absent.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 — Suite authority and package independence.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 — First Light package authority.
3. SFGSS-003 v1.1.0 — Stable IDs, configuration immutability, schema versions, and migration boundaries.
4. SFGSS-005 v1.4.0 — Checkpoint planning, visible code, tests, evidence, and stop rules.
5. PKG-LEARN-001 — First Light learning review, complete.
6. FL-M2-05 implementation and documentation closeout.
7. Current `EchoLaunchRoot`, `LaunchSession`, authority tests, and 102-test Runtime Play Mode baseline.

No authority conflict was found.

---

## 3. Starting Conditions

Before creating code:

- `main` and `origin/main` are synchronized at `c842cef`.
- The working tree is clean.
- Unity opens with zero compile errors.
- FL-M2-01 through FL-M2-05 are complete.
- Runtime Play Mode baseline is:
  - Passed: `102`
  - Failed: `0`
  - Ignored: `0`
- `EchoLaunchRoot` still claims authority before creating its session.
- No `Runtime/Configuration/` directory currently exists.
- No startup configuration, sequence, executor, report, presentation, or scene-loading behavior exists.
- Package version remains `0.1.0`.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Authority and Architectural Constraints

First Light owns project launch configuration, but the project owns each concrete configuration asset.

The checkpoint must preserve these rules:

- `EchoLaunchConfiguration` is immutable authored input during Play Mode.
- Active launch state remains in `LaunchSession`, never in the ScriptableObject.
- The configuration uses a runtime-safe domain ID, not `GetInstanceID`, asset path, filename, display name, or Unity `AssetDatabase`.
- The ID is generated once when a new configuration instance is created.
- Runtime code never repairs, regenerates, or rewrites an invalid released ID.
- Schema version is serialized and observable.
- Migration is not performed at runtime.
- Missing or unsupported configuration does not yet trigger preflight because preflight is outside this checkpoint.
- The first valid `EchoLaunchRoot` remains the only authority.
- A duplicate root may hold a serialized reference internally, but its public `Configuration` property must return `null`.
- No peer Sperk’s Forge package dependency is allowed.
- No Editor API may enter the Runtime assembly.

---

## 5. Scope

FL-M2-06 authorizes only:

1. One project-owned `EchoLaunchConfiguration` ScriptableObject type.
2. Canonical lowercase 32-character hexadecimal configuration IDs.
3. Serialized configuration schema version `1`.
4. Read-only identity and schema properties.
5. Internal identity/schema validity surfaces for later preflight.
6. One serialized configuration reference on `EchoLaunchRoot`.
7. One read-only authoritative `EchoLaunchRoot.Configuration` property.
8. Duplicate/stale-root configuration hiding.
9. Runtime Play Mode tests for identity, schema, binding, authority, reset, and immutability.
10. Unity-generated `.meta` files for the new folder and files.
11. Checkpoint-owned documentation and test evidence at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- `StartupSequence`
- `StartupSequenceEntry`
- `StartupStepDefinition`
- `IStartupStepExecutor`
- Startup-step policies
- Startup execution
- Automatic lifecycle advancement
- Configuration preflight or blocking diagnostics
- Runtime migration or repair
- Editor migration tools
- Configuration inspectors
- Setup windows or menu commands beyond `CreateAssetMenu`
- Splash sequence or presentation
- Launch destination
- Scene loading
- Launch report
- Root lifetime policy
- `DontDestroyOnLoad`
- Direct-scene initializer
- Prefabs, scenes, samples, or Test Lab content
- Peer-package bridges

Stop after the configuration can be created, bound, exposed only by the authority, proven immutable, and validated by the complete Runtime Play Mode suite.

The next tempting action, adding `StartupSequence`, belongs to a separate checkpoint.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   ├── Configuration.meta
│   └── Configuration/
│       ├── EchoLaunchConfiguration.cs
│       └── EchoLaunchConfiguration.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── LaunchConfigurationBindingTests.cs
            └── LaunchConfigurationBindingTests.cs.meta
```

### Modify

```text
Packages/com.echodevgames.echo-launch/
└── Runtime/
    └── Core/
        └── EchoLaunchRoot.cs
```

### Closeout Documentation

The established closeout bundle updates:

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
        │   └── FL-M2-06_Launch_Configuration_Identity_and_Root_Binding.md
        └── Test Reports/
            └── FL-M2-06_Launch_Configuration_Binding_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M2-06_First_Light_Launch_Configuration_Completion.md
```

No other file is authorized.

---

## 8. Design Contract

### 8.1 `EchoLaunchConfiguration`

The asset must:

- Derive from `ScriptableObject`.
- Use `CreateAssetMenu`.
- Declare `CurrentSchemaVersion = 1`.
- Serialize:
  - `configurationId`
  - `schemaVersion`
- Generate the ID with `Guid.NewGuid().ToString("N")`.
- Expose:
  - `ConfigurationId`
  - `SchemaVersion`
- Expose internal validation:
  - `HasValidIdentity`
  - `HasSupportedSchema`
- Never write to its fields during root binding or destruction.
- Never silently repair an empty, malformed, duplicated, older, or future value.

Canonical ID rules for this checkpoint:

- Exactly 32 characters.
- Lowercase ASCII only.
- Characters `0-9` and `a-f` only.
- No whitespace.
- No separator or punctuation.
- Empty and `null` are invalid.

### 8.2 `EchoLaunchRoot`

The root must add:

```csharp
[SerializeField]
private EchoLaunchConfiguration configuration;
```

and expose:

```csharp
public EchoLaunchConfiguration Configuration
```

The property returns:

- The assigned asset when this root is authoritative.
- `null` when this root is duplicate, stale after reset, or otherwise non-authoritative.
- `null` when no asset is assigned.

The property does not:

- Validate the asset.
- Start preflight.
- Create a default asset.
- Clone the asset.
- Copy values into mutable state.
- Change lifecycle state.
- Log a missing-configuration warning.

---

## 9. Implementation Sequence

### Phase A — Configuration Definition

1. Confirm clean Git and Unity compile state.
2. Create `Runtime/Configuration/`.
3. Create the complete `EchoLaunchConfiguration.cs`.
4. Return to Unity.
5. Wait for compilation.
6. Create one temporary configuration asset through:
   - `Assets > Create > EchoDevGames > First Light > Launch Configuration`
7. Confirm the Inspector shows the asset without exposing mutable session state.
8. Do not bind it to a scene yet.
9. Run the configuration-definition tests added in Phase C.

### Phase B — Root Binding

1. Modify `EchoLaunchRoot.cs`.
2. Add the serialized configuration field.
3. Add the read-only authoritative property.
4. Do not change `Awake`, `PublishProgress`, lifecycle notifications, or authority order beyond the new passive reference.
5. Return to Unity and confirm zero compile errors.

### Phase C — Automated Tests

1. Create `LaunchConfigurationBindingTests.cs`.
2. Use inactive GameObjects plus reflection to assign the private serialized field before `Awake`.
3. Cover configuration identity, schema, authority visibility, duplicate hiding, reset behavior, and immutability.
4. Run all Runtime Play Mode tests.
5. Expected total:
   - Passed: `117`
   - Failed: `0`
   - Ignored: `0`
6. Yellow `ELAUNCH-ROOT-001` warnings remain expected in duplicate-root tests.
7. No `ELAUNCH-EVENT-001` warning is expected from the new configuration tests.

### Phase D — Git and Documentation Closeout

1. Review only checkpoint-owned source and `.meta` files.
2. Commit and push the implementation.
3. Generate the established one-command documentation closeout bundle.
4. Review the staged nine-file closeout pattern.
5. Commit and push the adjacent documentation closeout.
6. Confirm clean working tree and synchronized remote.
7. Stop before `StartupSequence`.

---

## 10. Visible Code and Learning Order

Code delivery remains manual and visible.

The conversation presents files in this order:

1. `EchoLaunchConfiguration.cs`
   - Why configuration is a ScriptableObject.
   - Stable ID versus Unity asset GUID.
   - Schema version versus package version.
   - Why the asset never stores session state.
2. `EchoLaunchRoot.cs`
   - Passive serialized binding.
   - Authority-filtered public access.
   - Why missing configuration does not yet block launch.
3. `LaunchConfigurationBindingTests.cs`
   - Creating test-only configuration instances.
   - Assigning serialized references before `Awake`.
   - Proving duplicate/stale roots expose no authority.
   - Proving root lifecycle does not mutate the asset.

Each file is shown completely, with no hidden fragments.

---

## 11. Unity Editor Setup

No production scene setup is authorized.

For bounded verification only:

1. Open Unity 6000.3.8f1.
2. Wait for zero compile errors.
3. In the Project window, choose:
   - `Assets`
   - `Create`
   - `EchoDevGames`
   - `First Light`
   - `Launch Configuration`
4. Name the temporary project asset:
   - `FL-M2-06_TestLaunchConfiguration`
5. Confirm:
   - The asset can be selected.
   - Unity does not show active progress, current state, timing, retries, or scene-object fields.
   - No root or scene object is created automatically.
   - No lifecycle state changes merely because the asset exists.
6. Delete the temporary project asset after the creation path is verified unless it is deliberately retained as test evidence.
7. Do not create a Boot scene, prefab, sequence, or destination.

---

## 12. Validation and Acceptance Tests

| Test ID | Test | Expected Result | Type |
|---|---|---|---|
| FL-M2-06-T-001 | New configuration ID format | 32 lowercase hexadecimal characters | Automated |
| FL-M2-06-T-002 | Two new configurations | Different IDs | Automated |
| FL-M2-06-T-003 | Repeated ID reads | Same value | Automated |
| FL-M2-06-T-004 | New schema version | Equals `CurrentSchemaVersion` | Automated |
| FL-M2-06-T-005 | Generated identity validity | `HasValidIdentity` is true | Automated |
| FL-M2-06-T-006 | Generated schema support | `HasSupportedSchema` is true | Automated |
| FL-M2-06-T-007 | Malformed identity | `HasValidIdentity` is false without repair | Automated |
| FL-M2-06-T-008 | Unsupported schema | `HasSupportedSchema` is false without rewrite | Automated |
| FL-M2-06-T-009 | Authority with assignment | Exposes assigned configuration | Automated |
| FL-M2-06-T-010 | Authority without assignment | Exposes `null` | Automated |
| FL-M2-06-T-011 | Duplicate with assignment | Exposes `null` | Automated |
| FL-M2-06-T-012 | Duplicate creation | Does not replace authority configuration | Automated |
| FL-M2-06-T-013 | Authority reset | Former authority hides configuration | Automated |
| FL-M2-06-T-014 | Fresh root after reset | Exposes only its own configuration | Automated |
| FL-M2-06-T-015 | Root lifecycle | Does not mutate configuration ID or schema | Automated |
| FL-M2-06-T-016 | Full Runtime Play Mode suite | 117 passed, 0 failed, 0 ignored | Automated |
| FL-M2-06-T-017 | Unity Create menu | Asset can be created without scene side effects | Manual |
| FL-M2-06-T-018 | Git scope | Only authorized code, metadata, and closeout docs changed | Manual |

All tests must pass before closeout.

---

## 13. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Create menu entry missing | Attribute or compilation failure | Correct `CreateAssetMenu` and compile errors |
| ID is empty | Field initializer missing or overwritten | Correct initial creation logic; do not add runtime repair |
| ID contains uppercase or hyphens | Wrong GUID format | Use `ToString("N")` and canonical validation |
| Duplicate root exposes its asset | Property ignores authority | Gate the property with `IsAuthoritative` |
| Root state changes after assignment | Binding accidentally starts validation | Remove execution/preflight behavior |
| Configuration values change after Play Mode | Runtime writes to ScriptableObject | Remove every runtime write; keep state in session |
| Existing notification tests fail | Root lifecycle changed unnecessarily | Restore FL-M2-05 behavior and add only passive binding |
| Tests require `UnityEditor` | Test design crossed runtime boundary | Use transient ScriptableObjects and reflection |
| Unity creates extra files | Temporary asset or unauthorized content retained | Remove only checkpoint-owned accidental files |

Do not fix a failure by adding preflight, migration, inspectors, setup tools, or sequence execution.

---

## 14. Rollback and Recovery

To return to `c842cef` behavior:

1. Preserve unrelated working-tree changes.
2. Restore `EchoLaunchRoot.cs`.
3. Remove:
   - `Runtime/Configuration/`
   - `Runtime/Configuration.meta`
   - `LaunchConfigurationBindingTests.cs`
   - Its `.meta` file
4. Delete the temporary test asset if it exists.
5. Refresh Unity.
6. Confirm the original 102 Runtime Play Mode tests pass.
7. Revert only FL-M2-06 documentation if the checkpoint is abandoned.

No save data, scene, prefab, Build Profile, or project setting requires recovery.

---

## 15. Documentation Reconciliation at Closeout

Update only the established checkpoint family:

- Package `CHANGELOG.md`
- Package `README.md`
- Package documentation index
- Package architecture
- Package Current Notes
- Package FL-M2-06 checkpoint record
- Package FL-M2-06 test report
- Suite Current Notes
- Suite FL-M2-06 implementation completion record

Update the First Light specification or create an ADR only if implementation reveals a genuine contract conflict.

Do not create a separate public API document or duplicate the test report in a second location.

---

## 16. Commit and Push Plan

Preferred implementation commit:

```text
Add EchoLaunch configuration identity and root binding
```

Preferred adjacent documentation commit:

```text
Close FL-M2-06 launch configuration documentation
```

The implementation commit contains:

- `EchoLaunchConfiguration.cs`
- Modified `EchoLaunchRoot.cs`
- `LaunchConfigurationBindingTests.cs`
- Required `.meta` files

The documentation commit contains the established nine-file closeout pattern.

Do not claim commit or remote state before command evidence exists.

---

## 17. Completion Criteria

- [ ] Working tree and Unity compile were clean before implementation.
- [ ] `EchoLaunchConfiguration` exists in `Runtime/Configuration`.
- [ ] Create menu path works.
- [ ] New configurations receive canonical IDs.
- [ ] Schema version is serialized and equals `1`.
- [ ] Invalid identity is detected but not repaired.
- [ ] Unsupported schema is detected but not rewritten.
- [ ] Configuration contains no mutable launch-session state.
- [ ] Authoritative root exposes its assigned configuration.
- [ ] Duplicate and stale roots expose `null`.
- [ ] Root lifecycle does not mutate the configuration.
- [ ] Existing 102 tests remain green.
- [ ] Fifteen new tests pass.
- [ ] Full result is 117 passed, 0 failed, 0 ignored.
- [ ] No startup sequence or execution behavior was added.
- [ ] Git scope contains no unrelated changes.
- [ ] Implementation commit and push are confirmed.
- [ ] Documentation closeout commit and push are confirmed.
- [ ] Working tree is clean.
- [ ] Work stops before `StartupSequence`.

---

## 18. Next Recommended Checkpoint

**FL-M2-07 — Startup Sequence Definition and Ordered Entry Model**

Expected future outcome:

- One project-owned `StartupSequence`.
- Ordered immutable sequence entries.
- Stable sequence and entry identity.
- No executor or automatic run yet.

FL-M2-06 does not authorize FL-M2-07.

---

## 19. Handoff Record

| Field | Value |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` |
| Specification | v1.3.0 |
| Checkpoint | FL-M2-06 — Launch Configuration Identity and Root Binding |
| Starting commit | `c842cef` |
| Starting tests | 102 passed, 0 failed, 0 ignored |
| Planned tests | 117 passed, 0 failed, 0 ignored |
| Known blockers | None |
| Stop point | Before `StartupSequence` |
| Next checkpoint | FL-M2-07, not authorized |

---

## 20. Approval

**Decision:** Active and authorized
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 4, 2026
**Conditions:** Execute only the file manifest and behavior in this plan. Keep configuration immutable, preserve root authority filtering, prove all behavior through the complete Runtime Play Mode suite, and stop before sequence modeling or startup execution.
