# FL-M5-04 — Read-Only Validator and Project Health Report

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | FL-M5-04 |
| Version | 1.0.0 |
| Status | Approved |
| Package | First Light (`EchoLaunch`) |
| Package specification | SFGSS-PKG-ECHOLAUNCH-001 v1.10.0 |
| ADR | EchoLaunch-ADR-007 |
| Milestone | M5 — Tooling and Direct Scene |
| Repository | The-Sperk-Forge |
| Branch | `main` |
| Required baseline | `638e676` |
| Unity baseline | `6000.3.8f1` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Last updated | August 6, 2026 |
| Approved by | Jesse “Echo” Adams / EchoDevGames |

## 2. Purpose and Observable Outcome

First Light gains a dedicated read-only Validator window that inspects the
canonical installed foundation, enabled build scenes, configuration graph,
prefab/scene authority shape, startup and splash definitions, and Build
Settings, then returns one immutable deterministic project-health report.

Observable example:

```text
Healthy foundation
Health: Healthy
Information: 0
Warnings: 0
Errors: 0
Blockers: 0
Report fingerprint: <stable hash>

Broken foundation
Health: Blocked
- ELAUNCH-VAL-003: Root configuration is missing.
- ELAUNCH-VAL-008: Boot scene is disabled in Build Settings.
```

Running validation never applies, repairs, migrates, saves, creates, deletes, or
dirties project content.

## 3. Starting Conditions

- Branch: `main`
- HEAD: `638e676`
- Working tree: clean
- `main` equals `origin/main`
- Package version: `0.1.0`
- FL-M5-03 authority: `6615c8f`
- FL-M5-03 implementation: `dd15768`
- FL-M5-03 documentation: `638e676`
- Compilation baseline: `0` errors and `0` warnings
- EditMode baseline: `236` passed
- Runtime Play Mode baseline: `479` passed
- Total automated baseline: `715` passed
- No unresolved architecture blocker

## 4. Authority Set

1. SFGSS-000
2. SFGSS-PKG-ECHOLAUNCH-001 v1.10.0
3. EchoLaunch-ADR-004
4. EchoLaunch-ADR-005
5. EchoLaunch-ADR-006
6. EchoLaunch-ADR-007
7. SFGSS-004
8. SFGSS-005
9. Root and package Current Notes
10. FL-M5-01 through FL-M5-03 checkpoint/test/completion records
11. Existing Editor setup, apply, repair, snapshot, and test implementation

## 5. Checkpoint Learning Review

FL-M5-04 does not reopen runtime launch architecture or package independence.

The checkpoint-specific model is:

```text
Setup planner asks:
“What would create or repair the selected canonical foundation?”

Validator asks:
“What is true about the installed foundation right now?”

The first question can lead to a separately approved mutation.
The second question must remain an observation.
```

Implementation must preserve five ideas:

1. **Observation is not repair.**
2. **A healthy answer requires complete trustworthy evidence.**
3. **Scene inspection leaves the Editor exactly as it found it.**
4. **Stable codes make findings searchable and testable.**
5. **The same evidence produces the same report.**

## 6. Constraints

- Runtime and presentation assemblies remain unchanged.
- No project mutation is authorized.
- Validation is explicit, never automatic.
- Opening/repainting the window performs no validation.
- The default target root is `Assets/EchoDevGames/FirstLight`.
- Closed scenes may be opened additively only for read-only inspection.
- Open scene set, active scene, and dirty states must be preserved.
- No scene may be saved by validation.
- Build Settings must remain byte/semantic identical.
- Findings contain project-relative paths only.
- No wall-clock time, random ID, object instance ID, or scene handle enters
  deterministic fingerprints or copied reports.
- One validation run may be active at a time.
- Apply and Repair behavior and diagnostics remain unchanged.
- Direct-scene helper implementation remains deferred.
- `ELAUNCH-VAL-009` is reserved and not emitted in this checkpoint.

## 7. Scope

- Dedicated First Light Validator Editor window
- Explicit `Validate Project` action
- Editable project-root path with canonical default
- Immutable validation request, finding, and report contracts
- Validation report schema version 1
- Health and severity enums
- Stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`
- Deterministic request/evidence/report fingerprints
- Validation-specific read-only snapshot/evidence collector
- Ordered validation-rule catalog
- Canonical asset/type/schema/identity checks
- Startup-sequence entry, definition, ID, and policy checks
- Optional splash identity/reference/timing checks
- Root-prefab lineage/root/configuration/presentation checks
- Canonical Boot-scene root and lineage checks
- Enabled Build Settings scene inventory
- Destination and Boot uniqueness/enabled checks
- Duplicate-root scan across enabled build scenes
- Project-owned-versus-package-owned reference checks
- Sanitized copyable text report
- Validation re-entry containment
- Focused EditMode unit and real-asset/scene integration tests
- Full EditMode and Runtime Play Mode regression gates
- Manual healthy → invalid → healthy acceptance
- Documentation closeout after implementation

## 8. Explicit Exclusions

- Runtime code changes
- Presentation runtime changes
- Apply or Repair invocation
- Any auto-fix button
- Schema migration/downgrade
- Stable-ID regeneration
- Asset/prefab/scene/Build Settings writes
- Duplicate-root deletion
- Direct-scene initializer
- Build preprocess callbacks or automatic build blocking
- Automatic validation on import/reload/Play/window-open
- Runtime overlay or Observatory bridge
- JSON/support-bundle export
- Arbitrary gameplay-system validation
- Receipt, uninstall, reset, move, rename, or delete tools
- Package version change
- Player-build, clean-install, external-adoption, or performance claims

## 9. Project Health and Severity Model

| Finding severity | Effect |
|---|---|
| Information | Visible evidence; no health reduction |
| Warning | Health becomes `NeedsAttention` |
| Error | Health becomes `Invalid` |
| Blocker | Health becomes `Blocked` |

Health precedence:

```text
Blocked > Invalid > NeedsAttention > Healthy
```

An evidence-collection failure must not allow a false `Healthy` result.

## 10. Stable Validation Matrix

| Code | Proof | Default severity | Expected remediation boundary |
|---|---|---|---|
| ELAUNCH-VAL-001 | Canonical Boot scene exists and is a scene asset | Blocker | Setup Apply or explicit project choice |
| ELAUNCH-VAL-002 | Exactly one effective launch root across Boot/enabled scenes | Blocker | Manual resolution; no auto-delete |
| ELAUNCH-VAL-003 | Verified root binds the expected canonical configuration | Blocker | Setup Repair when eligible |
| ELAUNCH-VAL-004 | Configuration type, identity, and current schema are valid | Blocker | Repair only for authorized refs; migration otherwise |
| ELAUNCH-VAL-005 | Startup sequence and entries are complete and valid | Error | Explicit content edit |
| ELAUNCH-VAL-006 | Sequence/definitions have unique stable IDs | Blocker | Explicit ID/content correction |
| ELAUNCH-VAL-007 | Configured destination exists and is uniquely enabled | Blocker | Build Settings/project correction |
| ELAUNCH-VAL-008 | Canonical Boot entry is unique and enabled | Blocker | Setup Apply/Repair policy |
| ELAUNCH-VAL-009 | Direct helper release safety | Reserved | FL-M5-05 |
| ELAUNCH-VAL-010 | Configured visual presentation is available | Warning | Assign/repair appropriate project root presentation |
| ELAUNCH-VAL-011 | Splash identity, refs, and timing are valid | Error | Explicit splash edit |
| ELAUNCH-VAL-012 | Required step policy is coherent and safe | Error | Explicit policy edit |
| ELAUNCH-VAL-013 | Project-owned content does not live inside package source | Error | Later GUID-preserving move/manual correction |
| ELAUNCH-VAL-014 | Required evidence can be inspected safely | Blocker | Resolve scene/asset/import error |
| ELAUNCH-VAL-015 | Validation run is not re-entered | Warning | Wait for current run |

## 11. Files and Assets

### 11.1 Create

| Path | Responsibility |
|---|---|
| `Editor/Validation/EchoLaunchValidationEnums.cs` | Severity and project-health enums |
| `Editor/Validation/EchoLaunchValidationDiagnosticCodes.cs` | Stable `ELAUNCH-VAL-*` registry |
| `Editor/Validation/EchoLaunchValidationRequest.cs` | Immutable root/options request |
| `Editor/Validation/EchoLaunchValidationFinding.cs` | Immutable sanitized finding |
| `Editor/Validation/EchoLaunchValidationReport.cs` | Immutable schema-1 report and counts |
| `Editor/Validation/EchoLaunchValidationFingerprint.cs` | Deterministic request/evidence/report hashes |
| `Editor/Validation/EchoLaunchValidationEvidence.cs` | Immutable validation-specific evidence |
| `Editor/Validation/EchoLaunchValidationEvidenceCollector.cs` | Read-only asset/prefab/scene/Build Settings inspection |
| `Editor/Validation/EchoLaunchValidationRuleCatalog.cs` | Ordered package-owned rules |
| `Editor/Validation/EchoLaunchValidationService.cs` | Explicit run, re-entry gate, containment, report assembly |
| `Editor/Validation/EchoLaunchValidationTextFormatter.cs` | Deterministic copyable plain text |
| `Editor/Validation/EchoLaunchValidatorWindow.cs` | Dedicated read-only Editor window |
| `Tests/Editor/Validation/EchoLaunchValidationContractTests.cs` | Immutability, copies, counts, health |
| `Tests/Editor/Validation/EchoLaunchValidationFingerprintTests.cs` | Determinism and sensitivity |
| `Tests/Editor/Validation/EchoLaunchValidationRuleTests.cs` | Focused code/severity/rule outcomes |
| `Tests/Editor/Validation/EchoLaunchValidationServiceTests.cs` | Re-entry, exception containment, ordering |
| `Tests/Editor/Validation/EchoLaunchValidationTextFormatterTests.cs` | Stable sanitized report |
| `Tests/Editor/Validation/EchoLaunchValidationIntegrationTests.cs` | Real asset/prefab/scene/Build Settings proof |
| `Tests/Editor/Validation/EchoLaunchValidatorWindowTests.cs` | No auto-run, controls, copy/read-only behavior |

Unity creates and commits matching `.meta` files for every new folder/source/test
asset.

### 11.2 Modify only when necessary

| Path | Allowed change |
|---|---|
| `Editor/Setup/EchoLaunchProjectSnapshot.cs` | Expose existing immutable facts needed by validation without changing setup meaning |
| `Editor/Setup/EchoLaunchProjectSnapshotCollector.cs` | Factor/share strictly read-only helpers; setup evidence and fingerprints must remain unchanged |
| `Tests/Editor/Setup/*` | Regression expectations only when a shared helper is factored; no reduced coverage |
| `Documentation~/*` | Checkpoint closeout only after accepted implementation |

Do not modify Runtime, Presentation.UGUI, package prefabs, project assets, or
ProjectSettings for implementation.

### 11.3 Test-created assets

Tests may create only beneath unique roots:

```text
Assets/__EchoLaunch_FL_M5_04_Tests_<unique>
```

Every test must:

- Restore Editor Build Settings.
- Close scenes it opened.
- Restore the original active scene.
- Delete temporary assets and matching `.meta` files.
- Leave no dirty scene, prefab, asset, or package template.
- Leave no generated acceptance content in the implementation commit.

## 12. Implementation Sequence

### Phase 1 — Immutable contracts

1. Add severity and health enums.
2. Add immutable request/finding/report types.
3. Define report schema version 1.
4. Defensively copy all collections.
5. Exclude mutable Unity objects.
6. Prove health precedence and counts.

### Phase 2 — Evidence collection

1. Validate and normalize the requested project root.
2. Resolve canonical paths.
3. Read package-template and project-asset type/schema/identity facts.
4. Inspect configuration, sequence, definitions, splash, and destination.
5. Inspect root-prefab lineage, root count, configuration, and presentation.
6. Capture Build Settings scenes in stable order.
7. Inspect canonical Boot and enabled scenes without saving.
8. Restore open/active/dirty scene state.
9. Convert inspection failure into structured evidence, not exceptions.
10. Prove collection produces no dirty project state.

### Phase 3 — Rules

1. Implement rules in stable code order.
2. Emit path/entry-specific findings.
3. Derive health from highest severity.
4. Reserve but do not emit `ELAUNCH-VAL-009`.
5. Prevent a failed evidence source from producing false healthy findings.
6. Prove every implemented code with focused tests.

### Phase 4 — Fingerprints and formatting

1. Hash normalized request.
2. Hash immutable evidence in canonical order.
3. Hash report schema, health, counts, and findings.
4. Exclude timing, instance IDs, absolute paths, and random values.
5. Produce deterministic plain text.
6. Redact/sanitize exception and machine-path details.
7. Prove unchanged repeated runs match exactly.

### Phase 5 — Validation service

1. Add a single-active validation gate.
2. Reject re-entry with `ELAUNCH-VAL-015`.
3. Collect evidence once per explicit run.
4. Execute ordered rules with per-rule exception containment.
5. Return one immutable report even when blocked.
6. Never invoke setup mutation or refresh as a side effect.

### Phase 6 — Validator window

1. Add the dedicated Tools menu item.
2. Show default/editable project root.
3. Add explicit `Validate Project`.
4. Do not validate on window open/repaint/domain reload.
5. Show health, counts, fingerprints, and findings.
6. Add `Copy Report`.
7. Disable run controls only while the synchronous validation call is active.
8. Preserve the last immutable report in window memory only; no project receipt.

### Phase 7 — Automated validation

1. Run focused contract/fingerprint/rule/service/formatter/window tests.
2. Run real asset/prefab/scene/Build Settings integration tests.
3. Run complete EditMode suite.
4. Run complete Runtime Play Mode suite.
5. Confirm compilation and Console totals.
6. Inspect Git scope and remove test residue.

### Phase 8 — Manual acceptance

1. Create a temporary canonical First Light foundation.
2. Run Validator and expect `Healthy`.
3. Copy the report and record fingerprint.
4. Run again without changes and prove the same fingerprint/text.
5. Introduce approved temporary invalid states:
   - Clear root configuration.
   - Disable/remove canonical Boot Build Settings entry.
   - Add a second root to one enabled temporary scene or Boot acceptance copy.
6. Run Validator and expect `Blocked` with codes `003`, `008`, and `002`.
7. Confirm Validator changed nothing beyond the deliberately authored drift.
8. Restore through explicit Repair/manual acceptance cleanup.
9. Run Validator and expect the original `Healthy` fingerprint.
10. Remove generated acceptance assets and restore Build Settings before staging.

## 13. Automated Test Matrix

| ID | Proof | Expected |
|---|---|---|
| FL-M5-04-T01 | Window open/repaint | No validation, no dirty state |
| FL-M5-04-T02 | Contract immutability | Defensive copies; no Unity objects retained |
| FL-M5-04-T03 | Health precedence | Blocker > Error > Warning > Healthy |
| FL-M5-04-T04 | Request fingerprint | Root/options changes alter hash; order stable |
| FL-M5-04-T05 | Evidence fingerprint | Same evidence stable; relevant drift changes hash |
| FL-M5-04-T06 | Report fingerprint/text | Repeated unchanged run identical |
| FL-M5-04-T07 | Canonical healthy foundation | Healthy; no error/blocker |
| FL-M5-04-T08 | Missing Boot | `ELAUNCH-VAL-001` |
| FL-M5-04-T09 | Duplicate roots | `ELAUNCH-VAL-002` across enabled scenes |
| FL-M5-04-T10 | Root configuration mismatch | `ELAUNCH-VAL-003` |
| FL-M5-04-T11 | Unsupported configuration | `ELAUNCH-VAL-004`; no migration/write |
| FL-M5-04-T12 | Invalid/null sequence content | `ELAUNCH-VAL-005` |
| FL-M5-04-T13 | Duplicate IDs | `ELAUNCH-VAL-006` |
| FL-M5-04-T14 | Destination not uniquely enabled | `ELAUNCH-VAL-007` |
| FL-M5-04-T15 | Boot not uniquely enabled | `ELAUNCH-VAL-008` |
| FL-M5-04-T16 | Reserved direct-helper rule | `009` is not emitted in FL-M5-04 |
| FL-M5-04-T17 | Visual presentation unavailable | `ELAUNCH-VAL-010` warning |
| FL-M5-04-T18 | Invalid splash | `ELAUNCH-VAL-011` |
| FL-M5-04-T19 | Unsafe required-step policy | `ELAUNCH-VAL-012` |
| FL-M5-04-T20 | Project-owned asset in package | `ELAUNCH-VAL-013` |
| FL-M5-04-T21 | Scene/evidence inspection failure | `ELAUNCH-VAL-014`; health Blocked |
| FL-M5-04-T22 | Re-entry | `ELAUNCH-VAL-015`; one scan only |
| FL-M5-04-T23 | Scene state preservation | Open/active/dirty state unchanged |
| FL-M5-04-T24 | Byte/semantic preservation | Assets/scenes/settings unchanged |
| FL-M5-04-T25 | Rule exception containment | Sanitized 014; prior findings preserved |
| FL-M5-04-T26 | Apply/Repair regression | Existing setup tests unchanged and green |
| FL-M5-04-T27 | Full regression | Complete EditMode and Runtime Play Mode green |

## 14. Expected Automated Baseline

Starting baseline:

```text
Compilation:       0 errors, 0 warnings
EditMode:          236 passed
Runtime Play Mode: 479 passed
```

The new EditMode discovery count is not predetermined. Record Unity's actual
discovery after implementation.

Runtime code is unchanged, so the expected complete Runtime Play Mode gate
remains:

```text
Passed:   479
Failed:     0
Ignored:    0
Errors:     0
Warnings:   0
```

## 15. Manual Acceptance Checklist

- [ ] Temporary canonical foundation created.
- [ ] Validator opens without auto-running.
- [ ] First explicit validation reports `Healthy`.
- [ ] Second unchanged validation has identical fingerprint and copied text.
- [ ] Root-configuration drift emits `ELAUNCH-VAL-003`.
- [ ] Boot Build Settings drift emits `ELAUNCH-VAL-008`.
- [ ] Duplicate enabled-scene root emits `ELAUNCH-VAL-002`.
- [ ] Invalid report health is `Blocked`.
- [ ] Findings are actionable and project-relative.
- [ ] No absolute user path appears in copied report.
- [ ] Validator does not save or dirty scenes/assets/prefabs/settings.
- [ ] Repair/manual restoration returns project to healthy.
- [ ] Restored healthy fingerprint matches original.
- [ ] Temporary project content and Build Settings drift removed.
- [ ] Console/compiler totals are clean.
- [ ] Git scope contains only Editor validation implementation/tests/metadata.

## 16. Failure Symptoms and Responses

| Symptom | Response |
|---|---|
| Validator window runs immediately on open | Stop; remove automatic call and add regression test |
| Healthy project becomes dirty | Stop; identify the inspection API/write and restore exact state |
| Scene set or active scene changes | Stop; repair scene-state lease before further testing |
| Report fingerprint changes without evidence change | Find nondeterministic field/order and exclude it |
| Absolute machine path appears | Sanitize to project-relative path and test |
| One rule exception aborts the whole run | Convert to 014 and preserve accepted findings |
| Setup Apply/Repair tests regress | Stop; validation must not weaken mutation boundaries |
| Direct-helper code appears | Remove; FL-M5-05 owns that implementation |
| Validation “fixes” an issue | Remove mutation path; report only |

## 17. Completion Criteria

FL-M5-04 is complete only when:

- Specification v1.10.0 and ADR-007 remain satisfied.
- Dedicated Validator window is explicit and read-only.
- Immutable report schema 1 is implemented.
- Health/severity derivation is proven.
- Implemented stable validation codes are tested.
- Scene inspection preserves open/active/dirty state.
- No validation run dirties or changes project content.
- Deterministic repeated reports match.
- Manual healthy → blocked → healthy acceptance passes.
- Full EditMode and Runtime Play Mode gates pass.
- Generated acceptance residue is removed.
- Documentation, changelog, Current Notes, test report, and completion record are
  reconciled and committed adjacent to implementation.

## 18. Safe Rollback

Before implementation commit:

- Restore only FL-M5-04 Editor validation and test files.
- Remove only new validation files and their `.meta` files.
- Restore any shared setup helper changes.
- Remove temporary test/acceptance assets.
- Restore Editor Build Settings.
- Verify the FL-M5-03 baseline tests remain green.

Do not use broad `git clean` while untracked implementation files exist.

## 19. Stop Point

Stop FL-M5-04 after the read-only Validator and deterministic project-health
report are implemented, accepted, documented, committed, and pushed.

Do not continue into:

- Direct-scene initializer.
- Release-build helper policy implementation.
- Simulator.
- Laboratory.
- Build hooks.
- Migration.
- Distribution or external adoption.

The next checkpoint requires its own just-in-time learning and committed
authority.
