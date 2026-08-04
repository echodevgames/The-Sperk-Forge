# SFGSS-ADR-001 — Suite Package Editor Setup Facade Protocol

**Status:** Accepted  
**Decision date:** August 3, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Scope:** The Workshop (`EchoGameStarter`) and every approved package that advertises automated Workshop setup  
**Parent authorities:** SFGSS-000, approved package specifications, SFGSS-001, and SFGSS-002  
**Supersedes:** No prior ADR  
**Protocol version:** 1  
**ADR revision:** 1.1.0  

### Revision history

| Version | Date | Status | Summary |
|---|---|---|---|
| 1.0.0 | 2026-08-03 | Accepted | Foundation package facade protocol |
| 1.1.0 | 2026-08-04 | Accepted | Extended the exact facade registry and minimum setup domains through all thirteen Expansion packages |

> Package setup remains with the package that understands its own assets. The Workshop may coordinate the work, but it may not counterfeit another package’s hands.

---

## 1. Context

The Workshop is an Editor-only composer. Its approved specification requires it to install selected packages, generate generic project structure, and delegate package-specific configuration, prefab, scene, profile, validation, repair, and removal guidance to the package that owns those types.

Package specifications define setup and validation tooling, but The Workshop requires one exact invocation model across independently distributed packages. None of them yet promises a stable, versioned endpoint that The Workshop can invoke without directly referencing every peer Editor assembly.

A cross-package contract is therefore required before automated Workshop composition can be implemented. The contract must preserve all of the following:

- No runtime or core package dependency on The Workshop.
- No broad assembly scan or convention-based plugin discovery.
- No copy of package-specific setup logic inside The Workshop.
- Package ownership of its asset schemas, validation, idempotency, migration, and repair behavior.
- Exact version checks, deterministic dry runs, explicit user approval, and resumable transaction evidence.
- A manual setup path when a package has not yet implemented its facade.

---

## 2. Decision

The suite adopts a **package-owned, exact, allowlisted Editor setup facade protocol**.

Each runtime package may expose one public static facade in its existing Editor assembly. The facade does not reference The Workshop. The Workshop stores the package ID, compatible package range, exact assembly-qualified type name, protocol version, facade schema version, and supported method set in a reviewed adapter descriptor.

The Workshop may use narrow reflection only after it verifies that exact descriptor. It must not scan arbitrary assemblies, search for interfaces by convention, or invoke an endpoint absent from the approved catalog.

A shared Editor contracts package is **not** introduced at this stage. The boundary uses bounded detached JSON request and response records so every peer remains independently installable. A shared Editor-only contracts package may be reconsidered only through a later ADR after at least three real facade implementations demonstrate that shared compiled types materially improve safety or maintenance.

---

## 3. Exact facade identity

The default type pattern is:

```text
EchoDevGames.<TechnicalPackage>.Editor.Workshop.<TechnicalPackage>WorkshopSetupFacade
```

The exact assembly-qualified type remains recorded in The Workshop’s reviewed package adapter descriptor. The pattern is a naming standard, not permission to discover types by scanning.

| Package | Exact facade type |
|---|---|
| First Light (`EchoLaunch`) | `EchoDevGames.EchoLaunch.Editor.Workshop.EchoLaunchWorkshopSetupFacade` |
| The Observatory (`EchoDiagnostics`) | `EchoDevGames.EchoDiagnostics.Editor.Workshop.EchoDiagnosticsWorkshopSetupFacade` |
| The Accord (`EchoSettings`) | `EchoDevGames.EchoSettings.Editor.Workshop.EchoSettingsWorkshopSetupFacade` |
| The Passage (`EchoSceneFlow`) | `EchoDevGames.EchoSceneFlow.Editor.Workshop.EchoSceneFlowWorkshopSetupFacade` |
| The Pulse (`EchoGameState`) | `EchoDevGames.EchoGameState.Editor.Workshop.EchoGameStateWorkshopSetupFacade` |
| Resonance (`Jukebot`) | `EchoDevGames.Jukebot.Editor.Workshop.JukebotWorkshopSetupFacade` |
| The Will (`EchoInput`) | `EchoDevGames.EchoInput.Editor.Workshop.EchoInputWorkshopSetupFacade` |
| The Looking Glass (`EchoUI`) | `EchoDevGames.EchoUI.Editor.Workshop.EchoUIWorkshopSetupFacade` |
| The Chronicle (`EchoSave`) | `EchoDevGames.EchoSave.Editor.Workshop.EchoSaveWorkshopSetupFacade` |
| Impact (`EchoFeedback`) | `EchoDevGames.EchoFeedback.Editor.Workshop.EchoFeedbackWorkshopSetupFacade` |
| The Wellspring (`EchoPool`) | `EchoDevGames.EchoPool.Editor.Workshop.EchoPoolWorkshopSetupFacade` |
| The Ascent (`EchoProgression`) | `EchoDevGames.EchoProgression.Editor.Workshop.EchoProgressionWorkshopSetupFacade` |
| The Foundry (`EchoBuildTools`) | `EchoDevGames.EchoBuildTools.Editor.Workshop.EchoBuildToolsWorkshopSetupFacade` |
| Many Tongues (`EchoLocalization`) | `EchoDevGames.EchoLocalization.Editor.Workshop.EchoLocalizationWorkshopSetupFacade` |
| Voices (`EchoDialogue`) | `EchoDevGames.EchoDialogue.Editor.Workshop.EchoDialogueWorkshopSetupFacade` |
| The Path (`EchoObjectives`) | `EchoDevGames.EchoObjectives.Editor.Workshop.EchoObjectivesWorkshopSetupFacade` |
| The Vault (`EchoInventory`) | `EchoDevGames.EchoInventory.Editor.Workshop.EchoInventoryWorkshopSetupFacade` |
| The Hand (`EchoInteraction`) | `EchoDevGames.EchoInteraction.Editor.Workshop.EchoInteractionWorkshopSetupFacade` |
| The Eye (`EchoCamera`) | `EchoDevGames.EchoCamera.Editor.Workshop.EchoCameraWorkshopSetupFacade` |
| The Fellowship (`EchoCharacters`) | `EchoDevGames.EchoCharacters.Editor.Workshop.EchoCharactersWorkshopSetupFacade` |
| The Vessel (`EchoControllers`) | `EchoDevGames.EchoControllers.Editor.Workshop.EchoControllersWorkshopSetupFacade` |
| The Crucible (`EchoCrafting`) | `EchoDevGames.EchoCrafting.Editor.Workshop.EchoCraftingWorkshopSetupFacade` |

The facade belongs to the package Editor assembly. It must not be placed in the runtime assembly, sample assembly, project code, or The Workshop package.

---

## 4. Exact method surface

Protocol version 1 uses six public static methods. Every implemented method has exactly this compiled signature:

```csharp
public static string MethodName(string requestJson)
```

The approved methods are:

| Method | Responsibility | Mutation allowed? |
|---|---|---:|
| `Describe` | Return supported setup schema, options, capabilities, limits, documentation routes, and facade versions | No |
| `Plan` | Convert an approved package-selection request and project snapshot into package-owned proposed operations | No |
| `Apply` | Apply only the package-owned operations approved from a matching plan | Yes, bounded and journaled |
| `Validate` | Inspect package-owned output and return findings | No unless a separately approved repair operation is invoked through `Apply` |
| `Compare` | Compare a prior receipt/manifest record with current package-owned output for repeat, repair, or upgrade planning | No |
| `RemovalGuidance` | Return dependency-aware, non-destructive removal guidance and ownership classifications | No |

No overloads are part of protocol version 1. Optional capabilities are reported by `Describe`; an unimplemented optional method returns a structured `Unsupported` result rather than disappearing or throwing during signature verification.

---

## 5. Common request envelope

Every request is bounded JSON and includes at least:

| Field | Purpose |
|---|---|
| `protocolId` | Must equal `sfgss.workshop.setup-facade` |
| `protocolVersion` | Suite protocol version, initially `1` |
| `requestId` | Unique correlation ID generated by The Workshop |
| `operation` | `Describe`, `Plan`, `Apply`, `Validate`, `Compare`, or `RemovalGuidance` |
| `packageId` | Expected installed package ID |
| `packageVersion` | Resolved installed version observed by The Workshop |
| `facadeSchemaVersion` | Expected facade contract version |
| `setupSchemaVersion` | Package-specific setup schema requested by the adapter |
| `projectRoot` | Normalized project-relative generated root, never an unrestricted arbitrary filesystem target |
| `transactionId` | Workshop transaction identity when applicable |
| `planHash` | Approved plan hash for `Apply`, `Compare`, and repair/upgrade work |
| `approvedOperationIds` | Exact package-owned operations authorized by the user |
| `payload` | Operation-specific detached data |

The Workshop provides only the minimum project snapshot needed by the package facade. Requests must not contain credentials, source-control tokens, arbitrary personal data, raw save payloads, or unrestricted absolute paths.

---

## 6. Common result envelope

Every method returns bounded JSON containing at least:

| Field | Purpose |
|---|---|
| `protocolId` / `protocolVersion` | Confirms the understood protocol |
| `requestId` | Correlates the response |
| `packageId` / `packageVersion` | Identifies the responding package |
| `facadeSchemaVersion` / `setupSchemaVersion` | Records compatibility |
| `status` | `Succeeded`, `SucceededWithWarnings`, `Blocked`, `Failed`, `Unsupported`, `Cancelled`, `TooLate`, or `RequiresReload` |
| `diagnostics` | Stable package-owned codes, severity, concise message, and safe remediation data |
| `operations` | Package-owned planned or settled operations with stable IDs |
| `receipts` | Settled operation evidence suitable for the Workshop manifest |
| `requiresReload` | Whether the transaction must pause and reconcile after a domain/asset reload |
| `manualActions` | Explicit remaining user steps |
| `payload` | Operation-specific detached data |

Facade diagnostics retain the package’s own unique diagnostic namespace. The Workshop wraps invocation failures with its `EGS-ADP-*` codes while preserving the package code as nested evidence.

---

## 7. Plan and apply handshake

`Apply` is valid only when all of the following are true:

1. The Workshop has already written its transaction journal.
2. The installed package and exact facade still match the reviewed adapter descriptor.
3. `Plan` succeeded against a project snapshot whose relevant fingerprints still match.
4. The user approved the plan and its destructive classifications.
5. `Apply` receives the matching `planHash` and exact approved operation IDs.
6. The facade independently revalidates paths, ownership, preconditions, and setup schema.

The facade must refuse unplanned operations, added targets, stale fingerprints, a mismatched package/version, or a changed setup schema. It may settle a bounded subset and return exact receipts before a later operation fails, but it must never report an unsettled operation as complete.

Package add/remove requests remain The Workshop’s responsibility through Unity Package Manager. A package setup facade must not install or remove UPM packages behind the approved package phase.

---

## 8. Idempotency, ownership, and destructive behavior

- Create-only behavior is the default.
- Existing compatible assets may be adopted only after package validation and explicit user approval.
- A prior Workshop/package receipt does not grant permanent overwrite authority.
- Fingerprint drift removes automatic overwrite eligibility.
- Repair may recreate a missing generated asset only when the package can prove the expected recipe/schema and the target is still eligible.
- Destructive migration must be named, previewed, backed up where practical, and explicitly approved.
- Package-owned plans must distinguish created, adopted, modified, missing, blocked, skipped, and manual outputs.
- Project-authored content must never be silently replaced.

---

## 9. Reload, cancellation, and errors

- The Workshop writes its journal before invoking `Apply`.
- A facade may return `RequiresReload`; The Workshop then pauses, reloads/reconciles, and revalidates before continuing.
- The facade must not silently resume mutation after an Editor restart.
- Cancellation is cooperative before a bounded atomic operation starts.
- Once an operation reports `TooLate`, it settles to a truthful terminal result and returns receipts.
- Exceptions are caught at the invocation boundary, sanitized, and reported with package/facade/method/operation identity. Payloads and private paths are not dumped into logs.
- A missing or incompatible facade blocks only automated setup for that package. Manual package setup remains available.

---

## 10. Package-specific setup schemas

Each package owns and versions its own setup schema. Protocol version 1 requires the following minimum planning domains:

| Package | Minimum setup domains |
|---|---|
| EchoLaunch | Configuration, sequence, presenter choice, Boot scene, destination, root prefab, direct-scene option |
| EchoDiagnostics | Configuration, root, overlay preset, validation profile, export/privacy policy |
| EchoSettings | Configuration, defaults, built-in sections, storage policy, root prefab, display adapter choice |
| EchoSceneFlow | Configuration, scene catalog, route definitions, root prefab, transition/recovery policy |
| EchoGameState | State definitions, policies, initial state, root prefab, time/cursor adapter settings |
| Jukebot | Configuration, mixer/routing, root prefab, music/SFX/ambience assets, profile templates |
| EchoInput | Action asset/template, contexts, locks, glyph library, rebinding policy, root prefab |
| EchoUI | Root/layers, EventSystem policy, theme, screen/HUD/modal templates, accessibility defaults |
| EchoSave | Configuration, slot model, root prefab, storage sandbox, participant/sample choices |
| EchoFeedback | Configuration, root, recipe catalogs, channel/provider selections, accessibility limits, Laboratory |
| EchoPool | Configuration, root, pool catalogs, prefab/capacity policies, prewarm choices, Laboratory |
| EchoProgression | Configuration, root, progression catalogs, prerequisites, checkpoints, metrics/ranks, password schemes, Laboratory |
| EchoBuildTools | Build recipes, Build Profile bindings, validators, version/output policy, release checklists, Editor Laboratory |
| EchoLocalization | Configuration, root, locales, fallbacks, table references, font/script profiles, pseudo locales, Laboratory |
| EchoDialogue | Configuration, root, speaker/conversation catalogs, provider schemas, authoring defaults, Laboratory |
| EchoObjectives | Configuration, root, objective catalogs, provider/reward registries, tracking policy, Laboratory |
| EchoInventory | Configuration, root, item/container catalogs, equipment schemas, persistence policy, Laboratory |
| EchoInteraction | Configuration, root, action catalogs, detector adapters, concurrency policy, 2D/3D Laboratories |
| EchoCamera | Configuration, root, channels, backend choice, modes, blends, bounds/zones, 2D/3D Laboratories |
| EchoCharacters | Configuration, root, character catalogs, roster defaults, spawn provider, control policy, Laboratory |
| EchoControllers | Controller presets, host/config assets, intent adapters, family-specific Laboratories; no persistent root |
| EchoCrafting | Configuration, root, recipe catalogs, provider/station schemas, knowledge policy, Laboratory |

A package may expose fewer automated domains in its first release. Unsupported domains must appear as manual actions, not fabricated generic operations.

---

## 11. Compatibility and release gates

- A peer package may release and remain fully usable without implementing this facade.
- The package may not advertise **automated Workshop setup compatibility** until its facade and Workshop adapter tests pass.
- The Workshop may list a manual-only package selection when no facade exists, provided the dry run clearly distinguishes installation from automated setup.
- Breaking the facade type, signature, protocol, setup schema, operation identity, or receipt semantics requires versioned compatibility overlap or a documented manual migration.
- The facade and adapter receive clean-project, repeat-run, drift, missing-facade, wrong-signature, unsupported-schema, partial-failure, reload, removal, and Workshop-removal tests.

---

## 12. Consequences

### Positive

- Package setup logic stays with the package that owns the assets.
- The Workshop remains compile-time independent from every peer.
- The integration is exact, reviewable, fail-closed, and versioned.
- Generated projects have durable operation receipts and removal guidance.
- Packages without facades remain usable through their own setup tools.

### Costs

- Every automated peer integration needs a package facade and Workshop adapter descriptor.
- Detached JSON envelopes require schema tests and careful validation.
- Reflection invocation is still present at one narrow boundary and therefore needs explicit security and compatibility tests.
- No shared compiled types means some envelope validation code may initially be duplicated.

### Rejected alternatives

| Alternative | Reason rejected |
|---|---|
| The Workshop references every peer Editor assembly | Creates hard compile-time coupling and brittle package removal |
| Copy peer setup logic into The Workshop | Violates authority and guarantees drift |
| Broad reflection/interface discovery | Non-deterministic, difficult to secure, and incompatible with the approved catalog model |
| Mandatory shared contracts package now | Introduces a new dependency before implementation evidence proves it is necessary |
| Generate package assets generically from serialized guesses | The Workshop does not own peer asset schemas or migration rules |

---

## 13. Follow-up

1. Peer package implementation plans include facade work in their Editor tooling milestone, not necessarily their runtime skeleton milestone.
2. The Workshop M5 facade-protocol milestone implements fake facades first, followed by First Light and Observatory reference facades.
3. The Foundation contract matrix records every peer as `Manual until facade implemented`, then updates compatibility as each adapter passes.
4. A later ADR may introduce a tiny Editor-only shared contract only after measured implementation evidence.


---

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
