# The Sperk’s Forge — Data, IDs, Serialization, and Migration Standard

**Document ID:** SFGSS-003  
**Version:** 1.0.0  
**Status:** Approved architecture standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.11.0  
**Related authorities:** SFGSS-001, SFGSS-002, SFGSS-ADR-001, SFGSS-ADR-002, SFGSS-INT-FOUNDATION-001  
**Current development baseline:** Unity 6000.3.8f1  
**Minimum planned public Unity floor:** Unity 6000.0  
**Last updated:** August 4, 2026

> A durable record should remember the game, not the accident of where an asset happened to sit yesterday.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Data classification
6. Identity taxonomy
7. Stable domain identifier standard
8. Unity asset identity and GUID policy
9. Definition, configuration, and mutable-state separation
10. ScriptableObject safety
11. Runtime state associated with definitions
12. Serialization boundary taxonomy
13. Durable document envelope
14. Data-transfer object design
15. Serializer-provider contract
16. Unity JSON baseline and limitations
17. Schema versioning
18. Migration model
19. Aliases, renames, tombstones, and canonicalization
20. Unknown-data preservation
21. Transactions, publication, and rollback
22. Integrity, backups, recovery, and fingerprints
23. Import, export, and external-data validation
24. Persistence ownership and cross-package boundaries
25. Package removal, reinstallation, and optional-data survival
26. Editor authoring and migration tooling
27. Validation, testing, and release gates
28. Foundation application matrix
29. Reconciliation queue
30. Approval

---

## 1. Purpose and authority

SFGSS-003 defines the canonical rules for data classification, stable identity, Unity asset identity, serialization, schema versions, migrations, aliases, unknown-data preservation, transactions, integrity, and clean data survival across **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

SFGSS-000 establishes the suite-level separation between immutable definitions, mutable runtime state, durable game state, global preferences, diagnostics, and project-owned content. This standard turns that separation into implementation-facing contracts that package specifications, setup tools, save/settings formats, provider adapters, migration tools, tests, and releases must follow.

This document answers the questions that otherwise become expensive after data has shipped:

- Which identity survives an asset rename?
- Which identity exists in a Player build?
- When is a Unity asset GUID appropriate, and when is it not?
- What belongs in a ScriptableObject, a runtime object, or a durable DTO?
- How are package documents and participant/section payloads versioned independently?
- What happens when an optional package is removed and later reinstalled?
- How are unknown records preserved without executing them?
- When may a migration rewrite project-owned data?
- What does “transactional” mean for settings, saves, generation, inventory, and crafting?
- Which evidence is required before a format is considered migration-safe?

### 1.1 Authority order

When data documents disagree, use this order:

1. SFGSS-000 suite ownership and project-boundary rules.
2. The approved package specification that owns the data.
3. This standard.
4. An accepted ADR, integration specification, provider specification, or format specification that explicitly refines the package.
5. SFGSS-002 for dependency, bridge, provider, removal, and assembly behavior.
6. Checkpoint plans, setup/migration guides, test reports, release records, and Current Notes.

A lower document may define a package-specific identifier format, serializer, envelope, storage backend, or migration algorithm. It must not silently:

- turn project content into package-owned content;
- store live mutable state in shared definition assets;
- use an Editor-only identifier as a runtime contract;
- discard unknown durable data owned by an absent optional package;
- bypass schema/version checks;
- overwrite newer or unsupported data;
- weaken backup, preview, validation, transaction, or rollback requirements.

### 1.2 Requirement language

- **Must** means release-blocking.
- **Must not** means prohibited unless a higher authority or accepted ADR grants a narrow exception.
- **Should** means the default choice; deviation requires a documented reason.
- **May** means optional.

---

## 2. Scope and non-goals

### 2.1 In scope

This standard governs:

- Shared definitions, catalogs, configuration assets, runtime state, preferences, save data, generated project records, diagnostics, and support exports.
- Stable IDs for package definitions, project definitions, slots, participants, settings sections, scenes, routes, screens, providers, bridges, operations, and documents.
- Unity `.meta` GUID preservation and Editor-only AssetDatabase identity.
- Durable document envelopes and detached DTOs.
- Serializer-provider contracts and Unity JSON usage.
- Document, section, participant, definition, and generated-record schema versions.
- Contiguous migrations, aliases, tombstones, and unsupported-version behavior.
- Unknown optional-package record preservation.
- Transactions, staging, validation, publication, rollback, backup, recovery, and integrity checks.
- Import/export boundaries and untrusted external data.
- Data behavior during package removal, replacement, reinstallation, and downgrade attempts.
- Validation and test evidence required before a format is released.

### 2.2 Not in scope

This standard does not define:

- One mandatory serializer for every package.
- One mandatory storage backend or filesystem algorithm.
- The detailed save-slot implementation owned by The Chronicle.
- The detailed settings transaction owned by The Accord.
- Cloud synchronization, merge/conflict resolution, account identity, or encryption-provider implementation.
- Network replication formats or authoritative multiplayer provider selection.
- Database schemas for project-specific content.
- Final RPG data models for `EchoRPG.Foundation`.
- Repository tag/version rules except where a data-format change affects compatibility; those belong to SFGSS-009.
- Test evidence taxonomy beyond data-facing requirements; the full standard belongs to SFGSS-004.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Definition** | Reusable authored truth such as a music track, scene definition, item definition, input context, or crafting recipe. |
| **Configuration** | Project-selected policy and references controlling how a package initializes or behaves. |
| **Mutable runtime state** | Changing in-memory state for the current session, operation, scene, voice, draft, queue, cooldown, or authority. |
| **Durable state** | Data intentionally written so it can survive process termination, such as preferences, save progress, generated manifests, or migration records. |
| **DTO** | A detached data-transfer object containing serializable data but no live service, scene, native handle, or Unity object behavior. |
| **Domain stable ID** | A serialized identity owned by a package or project domain and intended to survive rename, move, save/load, export, or network transport. |
| **Unity asset GUID** | The GUID stored in an asset’s `.meta` file and resolved through Editor AssetDatabase APIs. |
| **Runtime instance ID** | A temporary identity for one in-memory object, request, lease, handle generation, transition, or operation. |
| **Display name** | Human-facing text that may change without changing identity. |
| **Schema version** | The declared structure version for one document, payload, definition, record, or section. |
| **Format ID** | Stable identifier naming the kind of durable document or payload. |
| **Migration** | A deterministic conversion from one supported schema version to a newer supported version. |
| **Alias** | A prior stable ID that resolves to one current canonical ID. |
| **Tombstone** | An explicit record that an identity was intentionally retired and must not be silently reused. |
| **Unknown record** | Valid bounded durable data whose owning definition, participant, section, package, or provider is currently unavailable. |
| **Opaque preservation** | Retaining unknown payload bytes/text and metadata without interpreting or executing them. |
| **Transaction** | A bounded operation that validates and stages changes before publishing authoritative results, with rollback or safe failure behavior. |
| **Publication** | The moment staged data becomes the authoritative current record. |
| **Fingerprint** | A non-security comparison signal used to detect change or compatibility drift. |
| **Integrity hash** | A value used to detect accidental corruption or incomplete transfer; it is not authentication unless a cryptographic trust design says otherwise. |
| **Canonicalization** | Converting accepted equivalent forms into one stable stored or in-memory representation. |

---

## 4. Governing principles

### 4.1 Identity is not presentation

Names, labels, paths, scene hierarchy positions, array indexes, registration order, and localized text are not stable identity.

### 4.2 Editor identity is not automatically runtime identity

A Unity asset GUID is excellent for preserving Unity asset references in the Editor and serialized assets. It is not automatically available or resolvable through `AssetDatabase` in a Player build. A runtime/save/network contract therefore needs an explicit serialized domain ID or another approved runtime-safe reference mechanism.

### 4.3 Definitions are immutable inputs

Shared ScriptableObjects and definition assets describe rules and content. Active indexes, cooldowns, draft values, selected slots, current progress, runtime owners, scene objects, and handles belong to runtime state.

### 4.4 Durable formats are explicit contracts

A file is not safe merely because it is valid JSON. Durable data declares its format, schema version, identity, ownership, and unsupported-version behavior.

### 4.5 Unknown optional data survives absence

Removing an optional package, bridge, provider, settings section, or save participant must not silently erase its valid durable records when preservation is part of the owning format contract.

### 4.6 Migration is forward, bounded, and observable

Supported migrations move from an older known version to the current version through explicit steps. They produce reports, preserve source/recovery records, and never promise silent downgrade.

### 4.7 Publish last

A durable record becomes current only after required validation, serialization, write, integrity, and verification steps succeed according to the owning package’s policy.

### 4.8 Failure must preserve evidence

Corrupt, incomplete, unsupported-newer, or failed-migration records are retained or quarantined where practical. Recovery/default behavior must not overwrite the only evidence of the failure.

### 4.9 Transaction scope must be honest

No package may claim an atomic operation across arbitrary external systems unless it owns and tests that guarantee. When true atomicity is impossible, the contract must state staged, compensating, partial, or best-effort behavior.

### 4.10 Data ownership follows authority

The package that owns the truth owns the schema and migration contract. A bridge translates data but does not steal schema authority from either peer.

---

## 5. Data classification

Every package specification must classify each significant data type before implementation.

| Class | Typical representation | Runtime mutable? | Durable? | Authority |
|---|---|---:|---:|---|
| Package definition template | Package ScriptableObject/sample asset | No | Package asset | Package |
| Project definition | Project ScriptableObject/catalog | No during play | Project asset | Project/package domain |
| Project configuration | Project ScriptableObject/asset | No during play | Project asset | Project with package schema |
| Runtime authority state | Class/struct/service-owned model | Yes | Usually no | Runtime authority |
| Runtime operation state | Request, handle, transaction, queue entry | Yes | No, except bounded journal when explicitly required | Operation owner |
| Global preference | Detached section DTO/document | Through draft/effective copies | Yes | The Accord |
| Save payload | Detached participant DTO/opaque entry | Captured/applied through participant | Yes | Participant plus Chronicle transport |
| Generated-project record | Manifest/receipt/journal | During Editor transaction | Yes | The Workshop/project |
| Diagnostic snapshot | Immutable bounded record | Replaced | Optional export only | Observed package/Observatory |
| Support export | Redacted structured artifact | No after creation | Optional | Exporting package/project |
| External import | Untrusted detached input | Staged only | Maybe after validation | Import owner |
| Cache/index | Derived data | Yes | Optional | Rebuildable, never sole truth unless specified |

### 5.1 Required classification questions

Before a type is approved, answer:

1. Who owns the truth?
2. Is the value authored, runtime, durable, derived, or imported?
3. Does it need stable identity?
4. Can it contain Unity object references?
5. Can it survive package removal?
6. Does it require a schema version?
7. What resets it?
8. What migration or recovery behavior applies?
9. Is it safe to inspect/export?
10. What test proves it does not contaminate definition assets?

---

## 6. Identity taxonomy

The suite recognizes distinct identity domains. They must not be substituted merely because they are all strings or integers.

| Identity | Lifetime | Durable? | Typical generator | Valid use |
|---|---|---:|---|---|
| Unity asset GUID | Asset `.meta` lifetime | Yes in project source | Unity Editor | Asset reference preservation, Editor lookup, generated manifest linkage |
| Domain stable ID | Definition/data lifetime | Yes | Package/project Editor tool or explicit authoring | Saves, exports, catalogs, reports, provider/section/participant identity |
| Runtime instance ID | One session/object/operation | No unless copied into report as non-resolvable history | Runtime authority | Handles, transitions, voices, leases, requests |
| Document ID | One durable document lineage | Yes | Owning package | Support correlation, revision history, import/export |
| Slot/profile ID | User-created durable container | Yes | Owning package | Save slots, profiles, generated projects |
| Schema/format ID | Format lifetime | Yes | Package specification | Codec and migration dispatch |
| External provider ID | Provider contract lifetime | Yes | Provider adapter | Provider selection and durable provider records |
| Display name | Presentation lifetime | Maybe | User/project | UI only |
| File path | Location lifetime | Maybe | Storage backend | Location, never sole identity |
| Registration index/order | Current registry lifetime | No | Runtime | Ordering only, never identity |

### 6.1 Prohibited durable identities

The following must not be used as the sole durable identity:

- `GetInstanceID()` or a Unity runtime instance ID.
- Scene hierarchy path.
- Transform sibling index.
- List or array index.
- Registration order.
- Asset filename or display name.
- Localized text.
- Type `AssemblyQualifiedName`.
- Raw enum ordinal unless the enum defines explicit frozen compatibility codes.
- Current file path.
- Build index alone.
- Memory address, hash code, or object reference.
- Timestamp alone.

---

## 7. Stable domain identifier standard

### 7.1 Approved forms

Each registry chooses one form and documents it.

#### Opaque generated ID

- Canonical lowercase 32 hexadecimal characters.
- Generated once by package-owned or project-owned Editor tooling.
- Example: `6e36d5c9e2a1497aa1131dfeb4cc8a70`.
- Recommended for definitions whose identity has no useful human semantic.

#### Namespaced semantic ID

- Lowercase ASCII.
- Reverse-domain or package-qualified owner prefix.
- Slash or dot-delimited category and stable slug.
- Examples:
  - `com.echodevgames.echo-settings.audio`
  - `com.mygame.player-progress`
  - `com.echodevgames.jukebot/cue/menu-confirm`
- Recommended for sections, participants, providers, bridges, diagnostic sources, and intentionally authored contracts.

A package may expose validated wrapper types such as `SceneId`, `SaveParticipantId`, or `SettingsSectionId`. Public APIs should prefer wrappers over undifferentiated strings when IDs from multiple domains could be confused.

### 7.2 Normalization

An ID contract must state:

- case sensitivity;
- whitespace behavior;
- maximum length;
- allowed characters;
- whether Unicode is prohibited or normalized;
- path-separator and traversal restrictions;
- reserved prefixes/values;
- empty/default behavior.

Default suite policy:

- IDs are case-sensitive after canonical lowercase validation.
- Leading/trailing whitespace is invalid, not silently trimmed during authoritative registration.
- Control characters, path separators where unsafe, `..`, and platform-reserved file names are rejected for storage-facing IDs.
- Display labels may use Unicode; stable IDs should remain portable ASCII unless a package specification proves another requirement.
- User-facing names are never rewritten into stable IDs after initial creation.

### 7.3 Generation and duplication

- New IDs are generated in Editor tooling, setup tools, or explicit project authoring flows.
- Runtime code must not regenerate a released definition ID because validation failed.
- Duplicating a Unity asset may duplicate its serialized domain ID. Validation must detect this.
- Repair tooling offers explicit choices such as regenerate the selected duplicate, preserve the original, update aliases/references when safe, or cancel.
- Automatic duplicate repair must not guess which asset owns the released identity.
- Package template IDs and project-cloned IDs follow the owning package’s clone policy.

### 7.4 Registry scope

Uniqueness is enforced in the declared domain:

- package-wide;
- project-wide;
- catalog-wide;
- document-wide;
- slot-wide;
- provider-wide;
- participant-wide.

An ID may be locally unique only when every reference also carries the parent domain identity. Cross-package reports qualify local IDs with package ID and category.

### 7.5 Rename and move behavior

- Display-name, filename, folder, and asset-path changes do not change the domain ID.
- Moving an asset must preserve its `.meta` GUID when it remains the same asset.
- A domain ID changes only through an explicit migration/identity-replacement operation.
- An identity replacement evaluates aliases, tombstones, saved references, exported references, and provider records before approval.

---

## 8. Unity asset identity and GUID policy

### 8.1 What a Unity asset GUID owns

A Unity asset GUID identifies a project/package asset through its `.meta` file. It is appropriate for:

- serialized Unity object references;
- Editor tooling that resolves an asset path;
- Workshop generation manifests;
- preserving public package asset identity across moves/renames;
- detecting missing or replaced assets;
- referencing `.asmdef` assets by GUID where SFGSS-002 applies.

### 8.2 What it does not own

A Unity asset GUID must not be treated automatically as:

- a Player-runtime lookup service;
- a save-game definition ID;
- a network ID;
- a provider ID;
- a settings section ID;
- a participant ID;
- a user profile ID;
- a durable ID for non-asset records.

`AssetDatabase` APIs are Editor-only. If an asset’s GUID is mirrored into a runtime-safe serialized field or build-time catalog, that mirrored value becomes an explicit domain/build record with its own validation and migration policy.

### 8.3 `.meta` preservation

Public package assets, scripts, asmdefs, templates, samples, prefabs, and configuration assets keep committed `.meta` files.

- Move/rename operations preserve the existing `.meta` file.
- Delete/recreate is not equivalent to move.
- Copying an asset creates a new Unity asset identity and may duplicate its serialized domain ID.
- GUID changes for released public assets are compatibility events and require migration/relink guidance.
- Setup tools must not replace existing project assets merely to make their GUID match a template.

### 8.4 Field and type rename compatibility

For Unity-serialized assets:

- Renaming a serialized field requires an approved compatibility path such as Unity’s field-rename attribute where applicable.
- Public serialized enum values use explicit stable numeric values and are not reordered or reused.
- Type/namespace/assembly moves require Unity-compatible migration attributes or explicit asset migration where applicable.
- Compatibility attributes assist Unity asset deserialization; they do not replace durable document schema migration.
- Removed fields remain documented through the deprecation window before destructive cleanup.

### 8.5 Direct asset references versus domain IDs

Use direct Unity object references inside project configuration when:

- the reference remains inside Unity-authored assets/scenes;
- Player builds can load the referenced asset through normal serialized references;
- no durable external reference must survive independently.

Add a domain ID when:

- saves, exports, network messages, generated reports, or external tools reference the definition;
- content must be resolved through a catalog;
- aliases or cross-project migration are required;
- diagnostic identity must survive display/path changes.

Many definition assets legitimately carry both a Unity asset GUID and a domain stable ID. They serve different contracts.

---

## 9. Definition, configuration, and mutable-state separation

### 9.1 Definitions

Definitions may contain:

- stable domain ID;
- display metadata;
- immutable tuning values;
- references to other immutable definitions;
- semantic tags;
- authoring-time validation metadata;
- schema version;
- project-owned presentation references.

Definitions must not contain current session state.

### 9.2 Configuration

Configuration may contain:

- project-selected policies;
- root prefab/scene references;
- limits and defaults;
- provider/serializer IDs;
- startup selections;
- safe default templates;
- required/optional references;
- schema version.

Configuration is copied or resolved into runtime policy. Runtime services do not write active values back into the asset.

### 9.3 Runtime state

Runtime state contains:

- active indexes, cooldowns, queues, histories, current progress, elapsed time;
- current scene/voice/screen/request references;
- drafts and provisional settings;
- selected save slot;
- resolved providers;
- active handles, leases, registrations, transactions;
- runtime caches and derived snapshots.

Runtime state belongs to an authority/service/session object and declares reset/shutdown behavior.

### 9.4 Durable state

Durable state is captured into detached DTOs or opaque payloads. It must not be the same live object graph used by the runtime authority.

### 9.5 Presentation state

Transient selection, animation, focus, tooltip, modal, and visual state belongs to presentation/runtime models unless the package specification explicitly defines a durable user preference or save requirement.

---

## 10. ScriptableObject safety

### 10.1 Default immutability rule

ScriptableObject definitions and project configuration assets are immutable inputs during Play Mode and Player runtime.

The following are prohibited in shared assets by default:

- current health, progress, selection, active state, or current save values;
- sequential indexes, shuffle bags, cooldown timestamps, active counts;
- current scene objects, transforms, GameObjects, AudioSources, EventSystems, or native operation handles;
- runtime subscribers, registration handles, leases, tasks, cancellation tokens, coroutines, or service references;
- current user input overrides, draft settings, queue contents, or migration progress;
- runtime-generated IDs that are expected to reset with the session.

### 10.2 Runtime copies

A package may create an immutable snapshot or mutable runtime copy from a ScriptableObject. The runtime copy:

- has explicit ownership and lifetime;
- does not retain unsafe Editor-only state;
- does not write changes back automatically;
- is independently constructible for tests;
- reports its configuration source identity diagnostically when safe.

### 10.3 Intentional runtime mutation exception

Runtime mutation of an asset requires an accepted package-level exception that documents:

- why a detached runtime model is unsuitable;
- how Play Mode contamination is prevented;
- behavior with domain reload disabled;
- behavior with multiple consumers and tests;
- reset and teardown;
- build behavior;
- dirty-asset prevention;
- automated proof.

No Foundation package currently requires this exception.

### 10.4 Authoring callbacks

`OnValidate`, serialization callbacks, custom inspectors, and migration tools may normalize authored data, but they must:

- avoid destructive surprise changes;
- support Undo where appropriate;
- mark assets dirty only for intentional authoring changes;
- avoid generating new released IDs merely because scripts recompiled;
- keep expensive catalog-wide validation out of per-frame/runtime paths;
- produce actionable validation reports.

---

## 11. Runtime state associated with definitions

When behavior needs mutable state per definition, the authority owns a runtime record keyed by:

```text
Definition domain ID
+ optional runtime owner/session ID
+ optional allocation/generation ID
```

Examples:

- Jukebot cue ID to sequential index, cooldown, and active voice count.
- Scene route ID to current operation data.
- UI screen ID to active view instance and history entry.
- Input context ID to active lease counts.
- Save participant ID to prepared payload/apply status.

Rules:

- A definition reference may be used as an in-memory key when lifetime is controlled, but diagnostics and durable references use the stable ID.
- Pooled/reused runtime objects use generation counters so stale handles cannot control a new allocation.
- Runtime state is cleared by its owning authority on reset/shutdown.
- A catalog reload or definition replacement invalidates/reconciles runtime state explicitly.
- Missing definitions produce an unavailable/orphan result, not accidental fallback by display name.
- Runtime state dictionaries are bounded or clearable and are never serialized back into the definition asset.

---

## 12. Serialization boundary taxonomy

The suite distinguishes four serialization systems.

### 12.1 Unity asset/scene serialization

Used for:

- ScriptableObjects;
- MonoBehaviours;
- scenes and prefabs;
- project configuration;
- package templates.

Rules:

- follows Unity serialization constraints;
- preserves `.meta` GUIDs;
- may use direct Unity object references;
- uses Editor migration/compatibility mechanisms;
- must not be confused with a portable save/export format.

### 12.2 Durable package documents

Used for:

- settings;
- save envelopes/manifests/payloads;
- binding override documents;
- Workshop manifests/journals/receipts;
- support snapshots;
- future progression/localization/import records.

Rules:

- detached DTOs or opaque bounded payloads;
- explicit format ID and schema version;
- no live `UnityEngine.Object` references;
- validated before application;
- migration and unsupported-version behavior defined.

### 12.3 Runtime snapshots/reports

Used for:

- diagnostics;
- launch reports;
- transition results;
- operation histories.

Rules:

- immutable and bounded;
- may be exported through a versioned document;
- do not become gameplay truth merely because they are serializable.

### 12.4 Provider/external formats

Used for:

- platform services;
- cloud backends;
- localization tables;
- build/release manifests;
- networking providers.

Rules:

- provider adapter owns translation;
- external data is untrusted;
- provider format/version is recorded separately from the suite DTO;
- provider removal does not silently erase neutral/core-owned durable data.

---

## 13. Durable document envelope

A durable document must define an envelope or equivalent top-level contract.

### 13.1 Minimum fields

| Field | Purpose | Required? |
|---|---|---:|
| `formatId` | Stable document kind | Yes |
| `schemaVersion` | Current structure version | Yes |
| `documentId` | Stable document lineage/instance ID when meaningful | Usually |
| `producerPackageId` | Package/project that wrote the document | Yes |
| `producerVersion` | Writer package/version for support context | Yes |
| `projectIdentity` | Project-safe identity when cross-project confusion matters | As applicable |
| `revision` | Monotonic logical revision within the document lineage | As applicable |
| `createdUtc` | Creation timestamp | As applicable |
| `modifiedUtc` | Last successful publication timestamp | As applicable |
| `payload` or `entries` | Owned data | Yes |
| `extensionData` / opaque records | Forward/optional preservation seam | When required |
| `integrity` | Algorithm and value | When package promises integrity checking |

### 13.2 Timestamp policy

- Durable timestamps use UTC.
- Ordering must not rely on wall-clock time alone where uniqueness matters.
- Human display converts from stored UTC through project/UI policy.
- Monotonic runtime timing is not serialized as wall-clock truth.
- Packages document whether timestamps are ISO 8601 strings, integer epoch units, or provider-specific DTOs.

### 13.3 Revisions

- Revision increments only after successful authoritative publication.
- Draft, preview, failed, and rolled-back writes do not consume an authoritative revision unless the owning package needs a separate attempt sequence.
- Revision is not identity.
- Concurrent/stale edits compare expected revision where conflict prevention is required.

### 13.4 Format dispatch

- `formatId` selects the owning reader/migration chain.
- Unknown format IDs remain unavailable and preserved where practical.
- A type name or assembly-qualified name is not a format ID.
- A serializer ID and a format ID are distinct. The same document format may support more than one serializer through explicit providers.

---

## 14. Data-transfer object design

### 14.1 Detached data only

Durable DTOs must not contain:

- `MonoBehaviour`, `ScriptableObject`, `GameObject`, `Transform`, `Scene`, `AsyncOperation`, `InputAction`, `AudioSource`, or other live Unity/native objects;
- delegates, events, tasks, coroutines, cancellation tokens, services, registries, or handles;
- direct file streams or provider SDK objects;
- arbitrary `System.Type` or assembly-qualified type names;
- unbounded recursive graphs without explicit limits.

References to definitions use stable domain IDs, project-approved addresses, or provider-neutral descriptors.

### 14.2 Collections

- Collection count limits are explicit.
- Ordering is deterministic where order has meaning.
- Unordered maps serialize through a documented canonical ordering when diffing, hashing, or deterministic output matters.
- For Unity JSON, maps/dictionaries use entry arrays or another approved representation.
- Top-level arrays/lists use a wrapper document when required by the serializer.
- Duplicate keys/IDs are rejected or resolved by an explicit policy, never last-write-wins by accident.

### 14.3 Polymorphism

Durable polymorphism uses:

- an explicit stable discriminator token;
- a bounded union of DTO shapes;
- or a serializer provider with a documented safe type registry.

It must not use CLR/assembly type names as the long-term public discriminator. Renaming a namespace or assembly must not orphan durable data.

### 14.4 Enums and tokens

- Public durable enum codes use explicit frozen values or stable string tokens.
- Existing numeric codes are not reordered or reused.
- Removed values become deprecated/tombstoned.
- Unknown values produce an explicit `Unknown`/unsupported path and preserve the raw value where required.
- Unity-serialized Inspector enums follow the same no-reorder/no-reuse compatibility rule.

### 14.5 Numbers

- Numeric range and unit are documented.
- NaN and infinity are prohibited in JSON-facing durable DTOs unless a custom serializer explicitly defines them.
- Floating-point values used for fingerprints/hashes require canonical representation.
- Currency, counts, sequence numbers, and IDs use integer types where practical.
- Time durations state units explicitly.

### 14.6 Null, missing, and default

A format distinguishes:

- field absent;
- field present with null;
- field present with default/empty value;
- field removed/deprecated;
- unknown field.

A serializer’s default-value behavior must not become an accidental migration policy.

### 14.7 Sensitive data

DTOs record only data required by the owning feature. Support/diagnostic exports redact paths, credentials, account IDs, typed text, save payload contents, and user-authored strings unless explicitly approved.

---

## 15. Serializer-provider contract

A serializer provider declares:

- stable serializer ID;
- provider/package version;
- supported DTO/data shapes;
- text or binary output;
- determinism/canonicalization behavior;
- maximum input/output size;
- unknown-field behavior;
- polymorphism behavior;
- numeric/enum behavior;
- thread-safety and Unity-main-thread requirements;
- error model;
- compatibility and migration responsibilities;
- security/trust assumptions.

### 15.1 Required operations

At minimum:

```text
Serialize<T>(detached DTO) -> bytes/text + result
Deserialize<T>(bytes/text) -> detached DTO + result
ValidateEnvelope(bytes/text) -> format/version/basic bounds result
```

The exact API belongs to the package specification. Results include stable error codes and never rely only on exceptions/logging.

### 15.2 Provider ownership

- The core package owns its neutral document/schema.
- The serializer provider owns encoding/decoding behavior.
- A provider must not change the semantic meaning of fields.
- Switching serializer requires an explicit conversion/migration plan when existing durable data must remain readable.
- Provider IDs are durable and survive display-name changes.
- Removing a serializer provider leaves records unavailable but preserved; it does not cause silent fallback decoding.

### 15.3 Determinism

A package states whether deterministic output is required for:

- diffing;
- fingerprints;
- hashes;
- network/replay comparison;
- support exports;
- tests.

When deterministic output is not promised, integrity checks operate on the exact published bytes rather than reserialization.

---

## 16. Unity JSON baseline and limitations

### 16.1 Approved baseline use

Unity `JsonUtility` may be the default serializer for simple package envelopes and plain serializable DTOs when its limits match the format.

Appropriate uses include:

- bounded plain classes/structs;
- explicit fields;
- simple lists/arrays inside wrapper documents;
- package-owned DTOs without dynamic polymorphism or dictionaries;
- detached data with explicit migration.

### 16.2 Required limitations

A package using Unity JSON must document and test:

- Unity serialization field rules;
- top-level wrapper requirement for collections;
- lack of native dictionary support;
- polymorphism limitations;
- unsupported/unknown field behavior;
- defaulting of missing fields;
- behavior of private/public serialized fields;
- thread and Unity-object constraints for the chosen API;
- deterministic output requirements.

### 16.3 Unknown-field warning

Deserializing an unknown-rich document into a known DTO and reserializing it does not preserve unknown fields automatically. Packages that promise unknown-data round trips must preserve the unknown record/payload opaquely or use a serializer/provider with explicit extension-data support.

### 16.4 Overwrite warning

Overwrite-style deserialization is useful for controlled Unity object/editor workflows, but it is not a migration system. Missing fields, stale fields, and partial updates still require explicit version and validation policy.

### 16.5 Custom Unity serialization

`ISerializationCallbackReceiver` may transform unsupported authoring structures such as dictionaries into Unity-serializable lists. The callback representation:

- remains an asset-authoring concern;
- must be deterministic and bounded;
- must not perform scene/service work;
- must not become the durable external schema unless explicitly specified.

### 16.6 Managed references

`[SerializeReference]` may support polymorphic authoring data when appropriate. Managed-reference type metadata and host-local managed-reference IDs are not suite domain IDs and must not be copied into saves/network messages as stable public identity.

---

## 17. Schema versioning

### 17.1 Every durable/upgradeable format declares a version

Versioned artifacts include as applicable:

- ScriptableObject definition/configuration schema;
- durable document;
- section record;
- participant payload;
- provider payload;
- generated manifest/journal/receipt;
- support export;
- imported table/catalog;
- report schema.

### 17.2 Version representation

Default policy:

- positive integer schema version;
- `1` is the first released schema;
- `0` is reserved for unset/legacy detection when needed;
- schema version is independent from package SemVer;
- package version is support context, not the migration dispatch key.

### 17.3 Layered versions

A document may contain independent versions:

```text
Document schema version
Section/entry/participant schema version
Serializer/provider format version
Referenced definition/content version
```

A package must not increment every layer merely because one payload changed.

### 17.4 Compatibility states

Readers return one of:

- Current.
- Supported older, migration available.
- Supported older, readable without migration.
- Unsupported older.
- Unsupported newer.
- Unknown format.
- Corrupt/invalid.
- Missing required version.

Unsupported newer data is preserved and opened read-only/unavailable. It is not rewritten to the current version.

### 17.5 Breaking changes

Schema-breaking changes include:

- removing/reinterpreting a field;
- changing units or numeric meaning;
- reusing an enum/token code;
- changing ID semantics;
- changing requiredness/default behavior;
- changing payload ownership;
- changing serializer without conversion;
- changing polymorphic discriminator meaning.

Field addition may still be breaking if older readers cannot preserve or ignore it safely.

---

## 18. Migration model

### 18.1 Contiguous forward chain

Preferred migration graph:

```text
v1 -> v2 -> v3 -> current
```

Each step:

- accepts one declared source version;
- produces one declared target version;
- is deterministic for the same input and context;
- validates output;
- reports warnings and changes;
- does not mutate the only source record;
- has automated fixtures.

Skipping versions requires a documented direct migration with equivalent evidence.

### 18.2 Migration phases

1. Identify format and stored version.
2. Enforce size/count/security bounds before expensive parsing.
3. Preserve source bytes/text and metadata.
4. Parse into detached source representation.
5. Run package-document migrations.
6. Run section/participant/provider migrations owned by their registries.
7. Resolve aliases/canonical IDs in detached data.
8. Validate current schema and cross-record constraints.
9. Produce migration report and staged current model.
10. Apply/publish only through the owning package transaction.
11. Preserve backup/source according to policy.

### 18.3 Ownership

- Package document migration belongs to the package that owns the envelope.
- Settings section migration belongs to the section owner.
- Save participant migration belongs to the participant/project.
- Provider payload migration belongs to the provider adapter.
- Definition asset migration belongs to the owning package’s Editor tooling.
- A bridge may translate between current peer models; it does not migrate either peer’s core document unless explicitly delegated.

### 18.4 Registration

Migration steps are registered explicitly through package-owned registries, generated tables, or exact allowlisted facades. Broad reflection scans are prohibited by SFGSS-002.

### 18.5 Source preservation

- Migration first occurs in memory or a staging location.
- The original remains available until the new record is verified and published.
- Successful read/migration does not automatically rewrite a save/settings document unless the package policy explicitly chooses a safe upgrade write.
- The next normal save/commit may publish the current schema.
- Destructive project-asset migrations require preview, backup, Undo or recovery where practical, and explicit approval.

### 18.6 Downgrade

Downgrade is not promised. A newer document encountered by older code remains preserved and unavailable/read-only. Exporting to an older format requires an explicit separate feature and data-loss report.

### 18.7 Migration failure

A failure result includes:

- format and document/asset identity;
- source and target versions;
- failing step ID;
- validation issues;
- whether source was preserved;
- whether staged output was discarded/quarantined;
- recovery/manual action;
- privacy-safe diagnostic context.

---

## 19. Aliases, renames, tombstones, and canonicalization

### 19.1 Alias rules

An alias map is directional:

```text
old ID -> current canonical ID
```

Rules:

- aliases are package/project-owned data;
- aliases do not form cycles;
- one old ID maps to one canonical ID;
- canonical IDs do not map back to aliases;
- alias chains should be flattened during validation;
- alias IDs remain reserved and are not reused for new definitions;
- collisions block release;
- resolution is bounded.

### 19.2 When aliases are appropriate

Use aliases when:

- a released definition ID must change;
- a participant/section/provider ID was renamed;
- project content was consolidated;
- imported legacy IDs need stable mapping.

Do not use aliases merely to hide duplicate-ID mistakes before release. Repair the duplicate and update references while no durable external dependency exists.

### 19.3 Tombstones

A tombstone records that an ID was intentionally removed.

It may include:

- retired ID;
- retirement version/date;
- replacement ID if any;
- removal reason;
- migration/default behavior;
- whether unknown payload remains preserved.

Tombstones prevent accidental reuse and distinguish “removed intentionally” from “missing package/content.”

### 19.4 Canonicalization timing

- Read/import accepts documented aliases.
- In-memory models use canonical IDs.
- Source records are not destructively rewritten merely because an alias resolved.
- The next successful authoritative transaction may write canonical IDs while preserving migration provenance.
- Diagnostics may report both stored and canonical IDs.

### 19.5 Display rename

Changing display text never requires a data migration unless display text was incorrectly used as identity in a legacy format. Legacy name-to-ID migration must handle ambiguity explicitly.

---

## 20. Unknown-data preservation

### 20.1 Preservation requirement

Unknown records are preserved when:

- the format advertises optional extensibility;
- an optional package/bridge/provider may be absent;
- settings sections or save participants are independently registered;
- a newer writer added fields/entries that the current reader can safely retain opaquely;
- SFGSS-002 clean-removal rules require data survival.

### 20.2 Opaque record minimum metadata

An unknown record preserves:

- owner/section/participant/provider ID;
- schema version;
- serializer ID;
- payload bytes/text;
- integrity/length metadata when present;
- required/optional flags when relevant;
- provenance/recovery state.

### 20.3 Safety

Unknown data:

- is never executed;
- is not deserialized into arbitrary runtime types;
- is bounded by count and size;
- is integrity-checked where the envelope provides checks;
- is not displayed/exported in full diagnostics by default;
- cannot claim a registered identity already owned by active known data.

### 20.4 Round-trip behavior

When a known document is committed after loading unknown records:

- known sections/participants are serialized from current authoritative state;
- unknown records are copied from their preserved opaque representation;
- ordering is deterministic where needed;
- payload bytes are retained exactly when byte-for-byte preservation is promised;
- normalization is allowed only when the format explicitly promises semantic rather than byte preservation.

### 20.5 Pruning

Unknown data is removed only by:

- an explicit migration that owns the retired format;
- an explicit user-approved prune plan;
- a retention limit whose destructive consequence is visible and documented;
- project reset/delete behavior that clearly includes the whole durable container.

Prune plans show IDs, sizes, source, recoverability, and backup behavior.

### 20.6 Reinstallation

When the owning package/provider returns:

- it registers the same stable owner ID;
- compatibility/migration is evaluated;
- preserved records become claimable only after successful validation;
- newer incompatible records remain preserved;
- ownership conflicts block activation.

---

## 21. Transactions, publication, and rollback

### 21.1 Standard transaction phases

```text
Inspect -> Validate -> Plan -> Stage -> Apply provisional changes
-> Verify -> Publish -> Notify -> Cleanup
```

Not every operation uses every phase, but the owning specification must state equivalents.

### 21.2 Validate before mutation

Validation covers:

- IDs and collisions;
- versions and migration availability;
- required references/providers;
- capacity/size bounds;
- permissions/path safety;
- required participants/appliers;
- output grant/capacity for gameplay transactions;
- stale revision/conflict detection.

### 21.3 Publication point

The package defines one authoritative publication point.

Examples:

- Accord replaces the committed settings snapshot/revision.
- Chronicle updates the verified generation head.
- Workshop records a completed operation receipt/manifest state.
- Inventory/crafting commits all container deltas after output validation.

Before publication, staged data is not current truth.

### 21.4 Rollback classes

| Class | Guarantee |
|---|---|
| Full rollback | All provisional changes restore prior authoritative state |
| Compensating rollback | External effects are reversed through explicit compensating actions |
| Publish-last safety | No current pointer/revision changes until complete verified output exists |
| Partial apply with report | Some participants may have applied; result identifies exact partial state |
| Irreversible explicit | Operation requires destructive confirmation and backup/recovery guidance |

A package must not label a partial apply “atomic.”

### 21.5 Multi-package operations

A bridge may coordinate requests, but it does not create a distributed transaction unless an integration specification defines:

- owner/coordinator;
- prepare/commit/abort contracts;
- timeout/failure behavior;
- rollback limits;
- durable recovery journal;
- tests for every interruption point.

Default suite behavior is authority-local transactions plus explicit failure results.

### 21.6 Events

Authoritative change events fire after publication. Preview/provisional events are separately named and must not be mistaken for committed truth.

### 21.7 Cancellation

Cancellation is honored only through documented safe boundaries. Once publication begins, the operation completes or recovers according to its transaction policy rather than abandoning a half-published record.

---

## 22. Integrity, backups, recovery, and fingerprints

### 22.1 Integrity

Integrity checks detect corruption/incomplete data. They do not establish trust or authorship unless a cryptographic security design explicitly adds authentication.

The owning format documents:

- algorithm ID;
- exact bytes covered;
- encoding/canonicalization;
- verification timing;
- failure behavior;
- upgrade behavior.

### 22.2 Backups

Backups are required before destructive migrations or replacement when practical.

A backup policy states:

- location;
- retention count/age/size;
- naming/identity;
- cleanup;
- privacy;
- restore flow;
- behavior when backup creation fails.

Failure to create a required backup blocks the destructive operation.

### 22.3 Recovery

Recovery is deterministic and bounded.

Possible candidates:

- prior immutable generation;
- replacement backup;
- temporary/staging file;
- source legacy file;
- catalog rebuild;
- defaults plus preserved corrupt artifact.

Recovery never silently overwrites the only corrupt/unsupported evidence.

### 22.4 Fingerprints

Fingerprints may detect:

- generated file modification;
- source asset/catalog drift;
- input action asset compatibility;
- plan snapshot changes;
- definition set changes.

Fingerprints:

- are not authorization to overwrite;
- record algorithm/version;
- use canonical input;
- tolerate documented irrelevant differences;
- are recalculated only by the owner;
- are not substituted for stable IDs.

### 22.5 File replacement

The standard does not promise filesystem atomicity on every platform. Packages choose a tested backend strategy such as immutable generations, temporary-write plus replacement, or provider transaction. The publication guarantee and fallback behavior are documented per platform.

---

## 23. Import, export, and external-data validation

### 23.1 External data is untrusted

Imports enforce:

- maximum bytes, records, depth, string length, and collection count;
- format/schema/serializer allowlists;
- ID validation and collision policy;
- path traversal and reserved-name protection;
- numeric ranges;
- duplicate key handling;
- migration bounds;
- timeout/cancellation where applicable;
- no arbitrary type activation;
- no code execution from payload data.

### 23.2 Import phases

1. Read into bounded staging memory/location.
2. Validate envelope.
3. Preserve raw source.
4. Parse with approved serializer.
5. Migrate detached data.
6. Resolve aliases.
7. Compare conflicts/ownership.
8. Preview changes.
9. Apply through an explicit transaction.
10. Produce receipt/report.

### 23.3 Export

Exports declare:

- format/schema version;
- included and redacted fields;
- stable IDs versus display labels;
- whether deterministic output is promised;
- compatibility target;
- privacy warning;
- whether reimport is supported.

Support exports default to metadata and diagnostics, not full save/settings payloads.

### 23.4 Human-editable files

When a format is intentionally human-editable:

- comments/unknown fields behavior is documented;
- validation errors report line/field when possible;
- normalization is explicit;
- user formatting preservation is not promised unless supported;
- secrets/credentials are excluded;
- source-control diff friendliness is considered.

### 23.5 Table imports

Future localization/content table imports must define stable row/key identity independent from row order and display text. Spreadsheet row numbers are not durable IDs.

---

## 24. Persistence ownership and cross-package boundaries

### 24.1 The Accord

Owns:

- global preference document;
- committed revision;
- section records;
- settings migrations;
- unknown settings section preservation;
- storage policy for preferences.

Does not own:

- per-save progress;
- peer runtime behavior;
- audio/input/UI implementations.

Section owners define their section schema/applier/migration through explicit registration/bridge contracts.

### 24.2 The Chronicle

Owns:

- save slots/generations;
- manifests/envelopes;
- participant transport;
- integrity/recovery;
- package document migrations;
- unknown participant payload preservation.

Participants own:

- project-specific DTO schema;
- definition references;
- participant migrations;
- capture/apply behavior.

### 24.3 The Workshop

Owns Editor transaction journals and generation-record schemas while creating them. Generated assets/reports/manifests become project-owned according to the Workshop classification model.

### 24.4 Other packages

Foundation peers keep session-only state unless their specification declares an optional settings/save contribution. A peer does not create a second preference/save backend merely for convenience.

### 24.5 Bridges

A settings/save bridge:

- registers a stable section/participant ID;
- owns registration lifetime;
- owns its payload/schema/migration when delegated;
- preserves data when absent through Accord/Chronicle unknown-record rules;
- does not cause the peer core to depend on Accord/Chronicle;
- is removed before peer packages under SFGSS-002.

### 24.6 Project data

Game-specific stats, quests, inventory, character state, world state, dialogue state, and content databases remain project-owned or belong to approved expansion/genre packages. General packages transport/reference them through narrow contracts.

---

## 25. Package removal, reinstallation, and optional-data survival

### 25.1 Removal classification

Before removal, data is classified:

| Class | Default removal behavior |
|---|---|
| Package-owned immutable source | Removed with package |
| Project-owned configuration/content | Preserved |
| Generated-managed project asset | Preserved unless explicit removal plan |
| Adopted/modified asset | Preserved |
| Global preference section | Preserved as unknown record when package/bridge absent |
| Save participant payload | Preserved as unknown opaque payload |
| Provider-specific record | Preserved/unavailable or exported per provider policy |
| Runtime/session state | Ends with authority/package |
| Cache/index | May be deleted if rebuildable |
| Diagnostic/support export | Preserved as user/project artifact |
| Migration backup/quarantine | Preserved according to retention policy |

### 25.2 Reinstallation

Reinstallation must:

- reuse the same package/section/participant/provider IDs;
- validate preserved data before claiming it;
- run supported migrations;
- leave unsupported newer data preserved;
- avoid resetting to defaults merely because data is temporarily unavailable;
- report aliases/tombstones/conflicts.

### 25.3 Replacement

Replacing a package/provider follows preserve-until-parity:

- export/document owned data;
- install replacement beside old system where possible;
- map identities through explicit migration;
- verify all records;
- retain rollback;
- remove old package only after parity.

### 25.4 Clean removal with SFGSS-002

Bridge-first removal occurs before peer removal. Removing code does not imply deleting durable project data. Setup/removal guides list exact files/records and manual choices.

### 25.5 Downgrade and rollback

Rolling package code back to an older version may make current documents unsupported-newer. The older package must preserve them and avoid overwrite. Code rollback is not data downgrade.

---

## 26. Editor authoring and migration tooling

### 26.1 Stable ID tooling

Package Editor tools provide as applicable:

- generate missing ID;
- scan duplicates;
- regenerate selected duplicate;
- show Unity asset GUID and domain ID separately;
- show references/aliases/tombstones;
- validate format/namespace;
- export collision report.

### 26.2 Migration tooling

A migration tool provides:

1. discovery and compatibility report;
2. target list;
3. source/target schema versions;
4. dry run/preview;
5. backup plan;
6. estimated destructive changes;
7. explicit confirmation;
8. progress and cancellation boundaries;
9. post-migration validation;
10. receipt/report;
11. rollback/recovery guidance.

### 26.3 Asset migration

- Uses Unity serialized workflows.
- Preserves `.meta` GUIDs.
- Supports Undo where practical.
- Does not bulk rewrite unrelated assets.
- Unknown newer assets remain unchanged.
- Field/type rename compatibility is tested in fixture assets.
- Force-reserialization is used only with a reviewed target set and source-control diff.

### 26.4 Durable document migration tools

- Operate on copies/staging.
- Never parse human-readable Markdown as authority when structured data exists.
- Keep failed inputs.
- Redact payload details in logs.
- Support batch limits and resumable reports where volume requires.

### 26.5 Workshop setup facade

SFGSS-ADR-001 setup facades report created/modified assets, schema versions, IDs, and migration requirements through versioned receipts. The Workshop does not invent or bypass a package’s data migration policy.

---

## 27. Validation, testing, and release gates

### 27.1 EditMode validation

Required as applicable:

- missing/duplicate/malformed IDs;
- alias cycles/collisions;
- tombstone reuse;
- definition asset mutation checks;
- schema version missing/invalid;
- DTO bounds;
- deterministic ordering;
- serializer round trip;
- unknown-record preservation;
- migration chain gaps;
- enum/token compatibility;
- Unity field/type rename fixture;
- `.meta` GUID preservation.

### 27.2 PlayMode/runtime validation

Required as applicable:

- definition assets remain unchanged after use/stress;
- runtime state resets on shutdown/domain configuration;
- stale handles cannot control reused instances;
- missing definitions produce explicit unavailable results;
- settings/save transactions publish only after success;
- failed migrations do not apply;
- unsupported newer data is preserved;
- unknown records survive load/commit/reload;
- cancellation at every safe boundary;
- partial apply results are honest.

### 27.3 Migration fixtures

Every released durable schema keeps fixtures for:

- each supported historical version;
- corrupt/truncated records;
- unknown fields/sections/participants;
- removed IDs and aliases;
- duplicate IDs;
- newer unsupported versions;
- maximum supported sizes/counts;
- migration interruption/retry;
- provider absent/present.

Fixtures are sanitized and redistributable.

### 27.4 Clean-project and upgrade tests

- Create data with version N.
- Upgrade package to N+1.
- Validate/read/migrate.
- Remove optional owner.
- Save/commit while owner absent.
- Reinstall owner.
- Reclaim and validate data.
- Attempt older code against newer data without overwrite.
- Remove package and confirm project-owned data remains.

### 27.5 Release gate

A data-bearing feature is not release-ready until:

- stable ID domain and format are documented;
- duplicate/missing validation exists;
- definitions versus state are proven;
- schema versions and compatibility states are explicit;
- migrations have fixtures/tests;
- unknown-data behavior is tested;
- destructive actions preview and back up;
- transaction/publication point is defined;
- recovery behavior is tested;
- migration/upgrade guide matches the release;
- no empirical claim is marked passed without evidence.

SFGSS-004 will define the complete evidence-state and release-report taxonomy.

---

## 28. Foundation application matrix

| Package | Stable identity | Durable data | Unknown-data rule | Migration/transaction note |
|---|---|---|---|---|
| First Light | Configuration, sequence, step, destination IDs | Project assets; optional launch report export | Unknown newer asset/report schema preserved/unavailable | Editor asset migrations; launch runtime state never persisted |
| Observatory | Provider, panel, validation-rule, snapshot schema IDs | Optional support snapshot/export | Unknown provider data is unavailable; export version guarded | Diagnostics failure never mutates observed truth |
| Accord | Package-qualified section IDs and document ID/revision | Global preference document | Unknown section records preserved across commits | Document + section migrations; revisioned apply transaction |
| Passage | Scene, route, transition profile IDs | Project assets; optional current-scene reference for consumers | Unknown newer asset schemas untouched | Editor asset migrations; runtime operations not persisted |
| Pulse | State/policy/scope-definition IDs | Session-only by default | Not applicable to core runtime state | No live pause/scope save; definition asset migration only |
| Resonance | Track, playlist, cue, profile, routing IDs | Project assets; preferences owned by Accord | Removed profile/schema assets never delete project mappings | Live playback state remains runtime-owned |
| Will | Input action/map/binding GUIDs plus package document IDs | Binding override document through project/Accord storage | Orphan entries and unknown extension data preserved | Fingerprint compatibility, transactional rebind rollback |
| Looking Glass | Screen, modal, HUD region, theme, notification IDs | Project assets; UI state session-only by default | Missing optional presenter/bridge becomes unavailable | View/theme asset migrations; domain state remains external |
| Chronicle | Slot, generation, participant, serializer, backend IDs | Save heads, manifests, payload generations | Unknown participant payloads preserved opaquely | Immutable generation publication; document + participant migrations |
| Workshop | Preset, package, bridge, adapter, plan, operation IDs; Unity GUIDs for outputs | Journals, manifests, reports, receipts | Missing adapter/package receipts preserved | Plan hash/reapproval, versioned journal/manifest migration |

### 28.1 Foundation consistency result

The Foundation specifications already align with the core direction of this standard:

- immutable definitions;
- service-owned runtime state;
- stable package/project IDs;
- independent document and payload versions;
- unknown optional-data preservation in Accord, Will, Chronicle, and Workshop;
- explicit transactions in Accord, Chronicle, Workshop, and input rebinding;
- project-owned content preservation.

The reconciliation queue below records wording/API refinements rather than authority blockers.

---

## 29. Reconciliation queue

The following items are approved for correction during **SUITE-DOC-10 Standards Consistency Review** before implementation:

1. **Accord configuration identity:** `EchoSettingsConfiguration` and `SettingsDefaultsProfile` currently list “Asset GUID” as stable identity. Clarify that the Unity GUID is Editor/project asset identity; add a domain/configuration ID only where runtime reports, exports, or external references require one.
2. **Chronicle configuration identity:** `EchoSaveConfiguration` currently lists “Asset GUID only.” Apply the same Unity-GUID versus domain-ID distinction.
3. **Passage scene authoring:** Confirm every `SceneDefinition` stores a runtime-safe `SceneId` separate from Editor source GUID/path metadata. AssetDatabase GUID resolution remains Editor/build-time tooling.
4. **First Light naming:** Reconcile `StartupStep` versus `StartupStepDefinition` terminology while preserving the approved definition/executor separation.
5. **Unknown JSON fields:** Accord’s “byte-for-byte or semantically equivalent” rule must select an explicit opaque-record/provider strategy. JsonUtility DTO round trips alone cannot satisfy unknown-field preservation.
6. **Will binding document:** Specify whether `extensionData` uses opaque raw records or a serializer provider with extension-data support.
7. **Workshop fingerprints:** Record canonicalization and algorithm version for every fingerprint used to compare generated project outputs.
8. **Serialized enums:** Review all Foundation public asset/document enums for explicit stable numeric/token values and no-reorder/no-reuse rules.
9. **Schema field naming:** Standardize `schemaVersion`, `formatId`, serializer/provider IDs, and compatibility-state terminology where package-specific reasons do not require variation.
10. **Migration evidence:** Keep every migration/test result `Not run` until fixtures and implementation exist.

No item changes package authority or unlocks code.

---

## 30. Approval

### 30.1 Approval checklist

- [x] Data classes and ownership are defined.
- [x] Unity asset GUID and domain stable ID are distinguished.
- [x] Generated and semantic ID forms are approved.
- [x] Definition/configuration/runtime/durable separation is explicit.
- [x] ScriptableObject safety rules are explicit.
- [x] Durable envelope and DTO rules are defined.
- [x] Serializer-provider requirements are defined.
- [x] Unity JSON limitations are recorded.
- [x] Schema version and migration rules are defined.
- [x] Aliases, tombstones, and canonicalization are defined.
- [x] Unknown optional data survives clean removal.
- [x] Transaction, publication, rollback, backup, and recovery rules are defined.
- [x] Import/export and untrusted-data rules are defined.
- [x] Foundation package data contracts were reconciled.
- [x] Implementation remains locked by ADR-002 and the full-suite roadmap.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames through the approved documentation-first delegation to select the most effective long-term architecture  
**Date:** August 4, 2026  
**Conditions:** Reconcile the Section 29 wording/API items during SUITE-DOC-10. Empirical serializer, platform, migration, compatibility, and performance claims remain `Not run` until implementation evidence exists.

---

## External technical basis

This standard is designed around Unity 6 behavior documented by Unity:

- Unity script serialization and custom serialization callbacks.
- Unity JSON serialization and `JsonUtility` limitations.
- Unity serialized field rename compatibility.
- Unity AssetDatabase GUID lookup as an Editor API.
- Managed-reference type/host behavior for `[SerializeReference]`.

These references inform technical constraints. The approved suite ownership, format, migration, preservation, and transaction rules remain defined by SFGSS-000, the package specifications, and this standard.


---

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
