# Project-Owned Launch Destination and Configuration Schema 3

**Document ID:** EchoLaunch-ADR-001
**ADR version:** 1.0.0
**Status:** Accepted
**Decision date:** 2026-08-05
**Last reviewed:** 2026-08-05
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Decision scope:** Package
**Evidence maturity:** Implemented and automated evidence complete; Standalone Laboratory and migration tooling pending
**Parent authorities:** SFGSS-000; SFGSS-PKG-ECHOLAUNCH-001 v1.4.0; SFGSS-003; SFGSS-005; SFGSS-007
**Affected documents:** First Light package specification; FL-M3-08 Checkpoint Build Plan; package and suite Current Notes; package documentation index
**Supersedes:** None
**Superseded by:** None
**Review triggers:** Destination-provider adoption; public scene-reference standard; configuration migration implementation; EchoSceneFlow handoff bridge; evidence that one reusable destination asset is harmful
**Related evidence:** Authority commit `eb9cc49`; implementation commit `114ac91`; 380 passing Runtime Play Mode tests; 0 compile errors; 0 compiler warnings; Standalone Laboratory activation pending

> First Light will reference one reusable project-owned destination asset, and the configuration schema advances to version 3 so existing schema-2 assets cannot masquerade as complete.

---

## 1. Context and problem

The approved First Light specification required one validated final destination but left the destination representation unresolved between an embedded serializable value and a standalone `ScriptableObject`.

During FL-M3-08 planning, the live repository showed that `EchoLaunchConfiguration.CurrentSchemaVersion` was already `2`. Schema 2 represented the configuration shape containing configuration identity and startup-sequence reference, but no destination reference.

Adding a required destination field while retaining schema 2 would make old and new serialized structures share the same version. Existing schema-2 assets could appear current while missing a newly required reference. That would violate the suite’s explicit migration and compatibility standards.

### 1.1 Known facts

- `EchoLaunchConfiguration` is project-owned and immutable at runtime.
- The current live configuration schema before this decision is version 2.
- The package specification requires one validated initial destination.
- Runtime must detect unsupported schema but must not silently rewrite project assets.
- Destination identity must not depend only on a display label.
- The neutral Runtime assembly must not depend on `UnityEditor`.
- Conditional, save-aware, and normal mid-game destination policy are outside the MVP.

### 1.2 Assumptions and evidence gaps

- The exact Editor authoring experience for selecting and synchronizing a scene asset is not implemented.
- Real Boot-to-destination Laboratory proof is not yet run.
- Migration tooling from schema 2 to 3 is not implemented.
- The decision is based on bounded compatibility and authoring reasoning, not performance measurement.

---

## 2. Decision drivers and constraints

- Preserve honest serialized schema identity.
- Keep authored destination data reusable and inspectable.
- Preserve package independence and neutral Runtime code.
- Support stable destination diagnostics and future bridges.
- Avoid hard-coded root scene strings.
- Keep runtime assets immutable.
- Prevent runtime auto-migration.
- Permit deterministic injected loader tests.
- Keep normal mid-game scene travel outside EchoLaunch.

---

## 3. Options considered

### Option A — Embedded destination data in `EchoLaunchConfiguration`

**Advantages**

- One fewer project asset.
- Fewer clicks for a tiny project.
- Direct serialization beside the startup sequence.

**Costs and risks**

- Destination identity and metadata cannot be reused cleanly across configurations.
- Future direct-scene and setup tooling must duplicate or copy embedded data.
- Report and migration diagnostics have a less distinct asset identity.
- The configuration becomes a wider mixed-responsibility asset.

### Option B — Standalone project-owned `LaunchDestination` ScriptableObject

**Advantages**

- Stable reusable destination identity.
- Clear separation between launch composition and scene target definition.
- Better future Editor validation, repair, and selection support.
- Cleaner report identity.
- Compatible with later provider and EchoSceneFlow bridge seams.
- Follows the package’s existing project-owned definition pattern.

**Costs and risks**

- Adds one project asset.
- Setup tooling must create and bind the asset.
- Users can temporarily create an unassigned or invalid reference.
- Asset migration and validation must be documented.

### Option C — Add destination reference but keep configuration schema 2

**Advantages**

- Avoids changing the schema constant.

**Costs and risks**

- Old schema-2 assets appear compatible while lacking required structure.
- Runtime cannot distinguish the historical shape from the new shape.
- Violates explicit serialization and migration standards.
- Creates hidden upgrade ambiguity.

### Option D — Do nothing

First Light would remain unable to complete its standalone handoff, successful report, or `LaunchCompleted` contract.

---

## 4. Decision

First Light adopts **Option B** with an explicit schema correction:

1. `LaunchDestination` is a standalone project-owned `ScriptableObject`.
2. `LaunchDestination.CurrentSchemaVersion` begins at `1`.
3. The asset owns:
   - Stable destination ID.
   - Serialized schema version.
   - User-facing display name.
   - Runtime-safe initial scene metadata.
4. `EchoLaunchConfiguration.CurrentSchemaVersion` advances from `2` to `3`.
5. Configuration schema 2 remains the historical startup-sequence-only shape.
6. Configuration schema 3 adds one serialized initial `LaunchDestination` reference.
7. Runtime blocks schema-2 and unknown configuration assets through the approved configuration compatibility diagnostic.
8. Runtime never silently rewrites or upgrades configuration or destination assets.
9. Editor migration from schema 2 to 3 is later tooling work.
10. Normal mid-game scene travel and conditional destination selection remain outside EchoLaunch MVP core.

---

## 5. Rationale

The standalone asset model gives the destination an identity and lifecycle equal to its diagnostic importance. It can be referenced, validated, reported, replaced, and later adapted without widening the startup configuration into a scene-policy container.

Advancing to schema 3 is required because schema 2 already exists in committed serialized history. Reusing schema 2 for a new required field would make compatibility checks untrustworthy.

---

## 6. Consequences

### 6.1 Positive

- Serialized compatibility remains explicit.
- Destination identity is stable and reusable.
- Configuration and destination responsibilities remain separated.
- Reports can record destination identity without exposing live scene objects.
- Future setup and validation tools gain a clear asset boundary.
- Loader injection remains package-local and testable.
- Future EchoSceneFlow integration can adapt the destination contract without changing authored startup steps.

### 6.2 Costs and risks

- Existing schema-2 assets become unsupported until explicitly migrated.
- Projects need one additional asset and reference.
- Runtime handoff tests require a destination and loader.
- Editor migration and scene-selection ergonomics remain required before release.
- Renaming or moving scenes can invalidate runtime scene metadata until Editor validation repairs it.

### 6.3 Deferred consequences

- Schema 2 to 3 migration UI.
- SceneAsset-backed custom inspector.
- Build Settings validation and repair.
- Conditional destination providers.
- EchoSceneFlow bridge.
- Standalone Test Lab Boot and destination scenes.

---

## 7. Authority and document impact

| Document/artifact | Required action | Status |
|---|---|---|
| SFGSS-000 | No suite authority change | Not applicable |
| First Light package specification | Select ScriptableObject model and schema 3 | Updated in v1.4.0 |
| Integration specification | No bridge contract selected | Not applicable |
| SFGSS-003 | Follow existing serialization rules | Compliant |
| Package Current Notes | Record accepted decision and active checkpoint | Updated |
| Suite Current Notes | Record accepted decision and handoff | Updated |
| FL-M3-08 plan | Bound implementation and evidence | Created |
| Automated tests | Prove schema, asset immutability, validation, load result, report, and completion | Complete: 380 passed, 0 failed, 0 ignored |
| Standalone Laboratory | Prove real Boot-to-destination load | Later checkpoint |

---

## 8. Implementation and migration impact

- **Implementation state:** Implemented and pushed in commit `114ac91`.
- **Public API impact:** Adds public `LaunchDestination`, destination loader contract, load result, completed report data, and `LaunchCompleted`.
- **Serialized data impact:** Configuration schema 3 adds a serialized destination reference. Destination schema 1 introduces a new project-owned asset type.
- **Migration impact:** Schema-2 configurations are blocked at runtime until an explicit Editor migration assigns a destination and writes schema 3.
- **Downgrade impact:** A package version supporting only schema 2 cannot safely consume schema-3 assets.
- **Removal/reinstall impact:** Project-owned destination assets remain in the consuming project when the package is removed.
- **Workshop/setup impact:** Later setup must create or select one destination asset and bind it to the launch configuration.

---

## 9. Evidence and validation plan

| Evidence | Required result | Current status |
|---|---|---|
| Source review | Confirm schema 2 already exists without destination | Complete |
| Compile | 0 errors and 0 compiler warnings | Complete |
| Runtime Play Mode | Full retained suite plus FL-M3-08 tests passes | Complete: 380 passed |
| Serialization tests | New config defaults to 3; schema 2 rejected; no runtime mutation | Complete |
| Destination tests | Identity/schema/scene metadata validation | Complete |
| Loader tests | Success, failure, cancellation, progress, and exactly-once invocation | Complete through injected/default-loader contract tests |
| Root handoff tests | `Transitioning -> Completed`, final report, `LaunchCompleted` | Complete |
| Standalone Laboratory | Real asynchronous scene activation | Not run |
| Real-project integration | External project adoption | Not run |

---

## 10. Security, privacy, licensing, cost, and provider impact

- **Security:** Scene metadata is project-authored. Runtime does not execute arbitrary code from the asset.
- **Privacy:** No personal or user-generated information is stored.
- **Licensing:** No third-party dependency is added.
- **Cost:** No provider or hosted service cost.
- **Platform:** Uses Unity runtime scene management already permitted by the package specification.
- **Provider impact:** None. EchoSceneFlow remains optional and separate.

---

## 11. Removal, reversal, and supersession plan

Reversing to embedded data or adopting a provider-owned destination requires a new package ADR because it changes public asset identity and serialized structure.

Schema 2 remains historical and is never redefined.

If a later ADR replaces the standalone asset, migration must preserve stable destination identity and provide an explicit path for schema-3 project assets.

---

## 12. Review triggers

- Three or more consuming projects prove the extra asset causes material authoring harm.
- A suite-wide scene-reference standard becomes authoritative.
- EchoSceneFlow provides an approved initial-handoff bridge.
- Conditional destination providers enter MVP scope.
- Unity changes build-scene runtime validation or loading semantics.
- A breaking package release revisits configuration serialization.

---

## 13. Approval record

**Decision:** Accepted
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** 2026-08-05
**Conditions:** Use configuration schema 3, preserve schema 2 as historical, keep runtime migration prohibited, and retain package independence.

---

## 14. Graph Navigation

- [[../../../../../Plan Documentation/Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification|First Light Package Specification]]
- [[../Current Notes|First Light Current Notes]]
- [[../../../../../Plan Documentation/Checkpoint Build Plans/FL-M3-08_Initial_Destination_Contract_Load_Result_and_Completed_Handoff_Checkpoint_Build_Plan|FL-M3-08 Checkpoint Build Plan]]
- [[../../../../../Plan Documentation/Current Notes|Suite Current Notes]]
- [[../../../../../Plan Documentation/SFGSS-003_Data_IDs_Serialization_and_Migration_Standard|SFGSS-003]]
- [[../../../../../Plan Documentation/SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007]]
