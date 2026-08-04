# Many Tongues - Localization, Locale, and Regional Content Package Specification

**Working document ID:** SFGSS-PKG-ECHOLOCALIZATION-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoLocalization  
**Public title:** Many Tongues - Localization, Locale, and Regional Content  
**Package ID:** `com.echodevgames.echo-localization`  
**Runtime namespace:** `EchoDevGames.EchoLocalization`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoLocalization`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Planned Unity Localization dependency:** `com.unity.localization` 1.5.12, compatibility status `Planned` until clean-project implementation evidence exists  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Let every voice arrive intact, every symbol find a home, and every player understand the world before them.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoLocalization. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and the approved Foundation, Impact, Wellspring, Ascent, and Foundry authorities | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved Unity Localization backend, locale authority, fallback, string/asset reference, formatting, font/script, pseudo-localization, validation, integration, Laboratory, and release contracts | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Many Tongues - Localization, Locale, and Regional Content  
**Technical identifier:** EchoLocalization  
**Flavor line:** Let every voice arrive intact, every symbol find a home, and every player understand the world before them.  
**Plain-language subtitle:** A Unity package that coordinates locale selection, localized string and asset access, fallback policy, regional formatting, font/script metadata, pseudo-localization, validation, and optional suite integrations on top of Unity's official Localization package.

**One-sentence ownership contract:**

> EchoLocalization owns the project's runtime locale authority, deterministic locale-selection and change lifecycle, suite-facing localized-reference and result contracts, fallback and missing-content policy, regional formatting facade, font/script/direction profiles, localization-specific diagnostics, setup, validation, pseudo-localization workflow, and optional bridge seams; it does not own translation authorship, production UI layout, dialogue flow, audio playback, global preference storage, save files, currency conversion, legal or cultural approval, machine translation, remote translation services, bidirectional text shaping, or Unity's underlying table and Addressables implementation.

### 1.1 Elevator summary

Many Tongues is not a second localization engine. The package deliberately builds on Unity's official Localization package, which already provides Locales, String Tables, Asset Tables, Smart Strings, localized references, pseudo-localization, import/export tools, and Addressables-backed loading. EchoLocalization adds the suite-level authority and contracts that Unity projects repeatedly need around that backend: one deterministic effective locale, explicit startup and request priority, bounded fallback policy, safe locale changes, structured results, project-owned font and direction profiles, diagnostics, setup, completeness reports, and clean integration with The Accord, The Looking Glass, Voices, Resonance, First Light, The Foundry, The Workshop, and The Observatory.

A project continues to author concrete translations and localized assets in Unity Localization tables. Table display names and entry display keys remain authoring conveniences; durable references use the backend's stable table-collection identity and entry identity. EchoLocalization converts those backend identities into clear package-facing value objects and results without pretending a generic Unity AssetDatabase GUID is a save or runtime domain identifier.

The runtime root owns locale lifecycle, not every localized component. Native Unity `LocalizedString`, localized asset references, component localizers, and Unity 6 UI Toolkit bindings may continue to update their surfaces directly. EchoLocalization coordinates the selected Locale, critical preloads, policy, versioned invalidation, and suite events. This keeps the package useful to programmers and designers without forcing every label, sprite, audio clip, or prefab through a custom wrapper.

### 1.2 Why this belongs in The Sperk's Forge

Localization tends to arrive late, when hard-coded strings, fixed-width layouts, source-language fonts, string concatenation, and scene-specific locale selectors have already fossilized into the project. A reusable package is justified because the repeated engineering problem is not translating individual sentences. It is building a reliable localization lifecycle and making missing content, font gaps, fallbacks, formatting, and platform readiness visible before release.

| Source project or authority | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Rescuers2D and game-jam projects | Menus, passwords, role prompts, tutorials, win/lose copy, and credits are authored directly in UI | Fast content iteration | Stable references, source tables, pseudo-localization, and no hidden layout assumptions |
| Hackulos and RPG work | Dialogue, item names, spells, objectives, lore, and locale-specific voice/assets will be extensive | Data-driven project content | Keep translation tables project-owned and separate from dialogue, inventory, and RPG rules |
| The Looking Glass | UI templates need replaceable text, fonts, direction, and readable fallback states | Presentation remains modular | UI listens to localization without becoming locale/storage authority |
| Voices | Conversation lines and optional voice assets need localization | Dialogue flow stays independent | Dialogue stores localization references, not production text authority |
| The Accord | Language is a global player preference | One preference authority | Accord stores the choice; EchoLocalization validates and applies it |
| Resonance | Localized voice or announcer assets may differ by locale | Audio playback remains Jukebot's job | Resolve the asset without starting or mixing audio |
| The Foundry | Shipping locales, platform metadata, tables, fonts, and pseudo-locale exclusion need preflight | Build evidence discipline | Package-owned validator bridge reports localization truth |
| Unity Localization 1.5.x | Mature tables, Smart Strings, assets, pseudo-locales, import/export, UI bindings, and fallbacks | Use supported backend | Add suite authority, policy, diagnostics, and boundaries instead of reimplementing it |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---|---|
| Public title and documentation | Yes | “Many Tongues” always appears beside the localization responsibility |
| Setup headings and tooltips | Yes | Flavor may decorate Locale, Tables, Fonts, and Validation language |
| Laboratory content | Optional | Sample phrases and icons remain redistributable, replaceable, and removable |
| Runtime API/type names | No lore-only names | Use `LocaleId`, `LocalizedTextReference`, `LocaleChangeResult`, and direct technical names |
| Project translations and assets | No required Verse content | Consumer projects own every source string, translation, localized asset, font, and cultural decision |

---

## 2. Problem Statement

### 2.1 Current problem

Without a declared localization authority and workflow, projects repeatedly accumulate:

- hard-coded production text in scenes, prefabs, scripts, animation events, and editor-generated content;
- localized strings addressed by mutable names rather than stable table and entry identities;
- language selectors that write directly to unrelated settings storage;
- multiple scripts competing to change the selected Locale;
- source-language UI layouts that have never been tested for expansion or right-to-left direction;
- translation tables that ship with missing or stale entries;
- fallback graphs that are implicit, cyclic, too deep, or inconsistent between strings and assets;
- localized audio and fonts loaded without release ownership;
- formatted numbers, dates, percentages, plurals, and currency assembled with source-locale assumptions;
- font assets that cannot render translated glyphs;
- Player logs containing resolved private/user text or formatting arguments;
- remote spreadsheet credentials or network assumptions embedded into runtime code;
- build pipelines that include pseudo locales, omit platform app metadata, or claim language support without evidence.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| SFGSS-000 | EchoLocalization owns locale tables, localized references, fonts, and formatting | One localization authority | Define exact boundary with Unity Localization, UI, Dialogue, Jukebot, Settings, and BuildTools |
| Unity Localization | Supports strings, assets, Smart Strings, fallbacks, pseudo-locales, import/export, and UI bindings | Use the official backend | Add deterministic package lifecycle, suite diagnostics, and policy |
| SFGSS-002 | Dependencies and bridges must be visible | Hard platform dependency is honest | Keep UI, Dialogue, Audio, Settings, and TMS integrations removable |
| SFGSS-003 | Stable IDs, definitions/state separation, migrations, and unknown-data preservation | Durable references | Distinguish backend table IDs from raw AssetDatabase identity |
| SFGSS-004 | Planned tests are not evidence | Honest `Not run` status | Define completeness, font, pseudo, platform, and fallback evidence |
| The Accord | Global preferences use draft/apply/persistence contracts | Persist language globally | EchoLocalization owns validity and application, not storage |
| The Looking Glass | Presentation has focus, screen, theme, and accessibility authority | UI remains project-skinnable | Consume localized values and direction without owning translations |

### 2.3 Consequences of doing nothing

- Renaming a table or entry breaks production references.
- A language choice appears saved but is invalid or unavailable on next launch.
- Locale changes race and leave mixed-language surfaces.
- Players see missing keys, source-language fragments, broken glyphs, or stale assets.
- Regional formatting changes game meaning or displays misleading values.
- Right-to-left locales receive labels without usable layout direction.
- Build claims list languages that were never validated.
- Dialogue, UI, and audio become directly coupled to one table layout or storage backend.
- Translation work cannot be reviewed, migrated, or tested independently from gameplay code.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one deterministic runtime authority for the effective Locale.
- Use Unity Localization 1.5.x as the approved backend rather than rebuilding tables, Smart Strings, asset localization, pseudo-locales, or import/export.
- Keep concrete translations, localized assets, fonts, and locale choices project-owned.
- Use stable table-collection and entry identities for durable references.
- Separate locale configuration, runtime state, localized content, presentation, preference persistence, and dialogue/audio consumption.
- Make initial selection and runtime locale changes explicit, asynchronous, cancellable before commit, and diagnosable.
- Provide bounded fallback, missing-content, formatting, asset-lease, font, and direction policies.
- Provide a five-minute setup path plus an advanced programmer API.
- Detect missing translations, asset mismatches, font gaps, pseudo-locale release leaks, and unsupported backend versions before release.
- Support standalone operation and removable bridges.
- Preserve honest evidence states until implementation executes tests.

### 3.2 Non-goals

- Authoring or purchasing translations.
- Machine translation, translation memory, terminology management, translator accounts, or remote TMS credentials.
- Dialogue graphs, subtitle timing, quest text flow, or narrative state.
- UI layout, focus, responsive design, or automatic mirroring of every view.
- Audio playback, mixer routing, voice queues, or lip synchronization.
- Global preference storage, save slots, cloud sync, or player accounts.
- Currency exchange, unit conversion, taxation, regional pricing, legal review, or cultural certification.
- A custom bidirectional text shaping engine.
- Automatic runtime font atlas generation in the MVP.
- Runtime user-generated translations or mod localization.
- Replacing Unity Localization's tables, Addressables integration, LocalizedString, component localizers, or UI Toolkit bindings.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity 6 project | Create/link Localization Settings, source locale, profile, pseudo locale, and a sample table without hidden dependencies |
| Programmer | Needs locale-aware systems | Request locale changes and resolve text/assets through structured APIs and results |
| Designer/content author | Owns strings and localized assets | Work in Unity's tables with stable references, validation, completeness, and pseudo previews |
| UI developer | Uses uGUI, TMP, or UI Toolkit | Bind localized values while EchoUI remains presentation authority |
| Narrative designer | Uses Voices | Store localized references and arguments without embedding translation logic in dialogue flow |
| Audio developer | Uses Resonance | Resolve locale-specific voice/audio assets while Jukebot retains playback authority |
| Tester | Needs reproducible language checks | Switch locale/pseudo locale, inspect fallbacks, fonts, missing content, and export a redacted report |
| Release maintainer | Uses The Foundry | Block shipping when approved locale, table, font, or pseudo-locale gates fail |

### 3.4 Measurable success criteria

- Package installs into a clean supported Unity project with the approved Unity Localization dependency and zero compile errors.
- Runtime core compiles without any peer Echo package.
- One authoritative locale initializes deterministically and survives scene changes.
- Stable references survive table and entry display-name changes.
- Locale-change failures retain or report a known effective locale.
- Missing content and formatting failures return structured results without throwing on normal absence.
- Shipping locales pass table, fallback, asset-type, font, and pseudo-locale validation.
- The Standalone Laboratory proves locale switching, fallbacks, Smart Strings, assets, pseudo-localization, fonts, direction, and diagnostics.
- Optional bridges can be removed without breaking either core.
- Samples can be deleted without breaking the package.
- Documentation and claims remain aligned with executed evidence.


## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers preparing projects for more than one language or regional variant.
- Designers and writers authoring source strings and translated tables.
- Programmers integrating UI, dialogue, audio, settings, build, and content systems.
- QA testers validating missing content, formatting, fonts, direction, and locale changes.
- Package maintainers shipping reusable components that must expose localized labels without owning localization state.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ELOC-UC-001 | Initialize the effective locale | Runtime | Valid configuration and backend | One deterministic Locale reaches Ready with structured selection evidence | MVP |
| ELOC-UC-002 | Change player language | Player/UI/project code | Requested locale is enabled | Critical content prepares, locale publishes once, and persistence is requested separately | MVP |
| ELOC-UC-003 | Resolve localized text | Runtime consumer | Stable text reference | Text or structured missing/fallback result returns | MVP |
| ELOC-UC-004 | Format localized text | Runtime consumer | Smart String and safe arguments | Locale-correct plural/number/date/list output returns | MVP |
| ELOC-UC-005 | Resolve localized asset | Runtime consumer | Stable asset reference and expected type | Disposable typed lease returns or fails safely | MVP |
| ELOC-UC-006 | Validate translation coverage | Content author/tester | Source and target tables exist | Completeness/fallback report identifies missing or fallback-only entries | MVP |
| ELOC-UC-007 | Validate font coverage | UI/content author | Locale font profile and character fixture | Missing glyphs and fallback coverage are reported | MVP |
| ELOC-UC-008 | Run pseudo-localization | UI/tester | Development build/Laboratory | Expansion, accents, hard-coded strings, and direction risks become visible | MVP |
| ELOC-UC-009 | Persist locale preference | Accord bridge | EchoSettings installed | Valid explicit choice is stored globally and restored next launch | Bridge |
| ELOC-UC-010 | Localize dialogue | Voices bridge/project code | Dialogue stores localization references | Text/voice assets resolve without transferring dialogue authority | Bridge |
| ELOC-UC-011 | Localize audio asset | Resonance bridge/project code | Audio reference exists | Localized asset resolves; Jukebot decides playback | Bridge |
| ELOC-UC-012 | Build preflight | Foundry bridge | Build recipe/profile selected | Shipping locale/package/table/font/pseudo gates report into Foundry | Bridge |
| ELOC-UC-013 | Generate project setup | Workshop | ADR-001 facade available | Workshop previews and invokes package-owned setup safely | Bridge |
| ELOC-UC-014 | Export support snapshot | Tester/support | Runtime initialized or failed | Redacted locale/backend/lookup health report is generated | MVP |

### 4.3 Explicitly unsupported use cases

- Treating a locale code as authentication, entitlement, or legal jurisdiction proof.
- Automatically translating production text with an AI or online service.
- Converting currencies or gameplay units.
- Mirroring every UI hierarchy without an explicit presentation adapter.
- Using resolved display text as a durable save ID, objective ID, item ID, or dialogue branch ID.
- Loading arbitrary untrusted table files from players at runtime.
- Changing Locale from multiple uncoordinated scripts by writing directly to backend globals.
- Shipping a pseudo locale or incomplete language because fallback happened to hide missing entries.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- The EchoLocalization runtime authority, configuration, state, locale request lifecycle, and package diagnostics.
- Initial locale-selection precedence and runtime locale-change admission policy.
- Suite-facing stable localized text/asset reference values and structured result types.
- Package policy for fallbacks, missing content, critical preloads, locale visibility, and release eligibility.
- Locale descriptors, text direction metadata, project font-profile selection, and font coverage validation.
- Package setup, repair, validation, completeness reports, pseudo-localization workflow, Laboratory, and support exports.
- Explicit integration seams and package-owned ADR-001 Editor setup facade.

### 5.2 The package does not own

- Concrete source strings, translations, localized assets, fonts, icons, voice clips, or cultural approvals.
- Unity Localization's internal table database, Addressables implementation, Smart Format parser, or component localizers.
- Global preference storage or settings UI.
- Dialogue sequencing, subtitle timing, audio playback, UI layout, focus, or animation.
- Save files, profiles, inventory, objectives, progression, characters, or gameplay state.
- Remote spreadsheets, translator accounts, machine translation, version-control operations, or deployment.
- Bidirectional shaping or automatic presentation mirroring in the neutral core.
- Platform language support claims without evidence.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoLocalization interacts |
|---|---|---|
| Locale/String/Asset tables and backend loading | Unity Localization package | Hard platform dependency and explicit backend adapter |
| Global language preference | The Accord or project provider | Optional `ILocalePreferenceProvider`; validation/application remains EchoLocalization |
| Production screens/layout/focus | The Looking Glass/project UI | Locale, text, direction, font, and invalidation events through a bridge |
| Dialogue sequence and choices | Voices | Dialogue stores/retrieves localization references; optional voice asset bridge |
| Audio playback/mix | Resonance | Supplies localized asset/cue reference; Jukebot owns playback |
| Startup ordering | First Light | Optional startup-step bridge initializes localization before dependent presentation |
| Build validation | The Foundry | Package-owned validator bridge reports localization readiness |
| Project generation | The Workshop | ADR-001 setup facade; Workshop never edits tables directly |
| Diagnostics dashboard | The Observatory | Optional snapshot/provider bridge |
| Platform app name/metadata | Unity Localization platform metadata + Foundry | Echo validates configured locale coverage; Foundry remains build authority |
| Text layout/shaping | UI technology/project provider | Echo exposes direction/script metadata and optional provider seam |

### 5.4 Boundary tests

1. Is this feature selecting/resolving localized content, or is it presenting/playing/advancing that content?
2. Does the feature author translations, or validate and transport project-authored translations?
3. Is a value a durable domain ID, a Unity Localization table/entry provider ID, or only display text?
4. Does a locale choice belong to runtime validity/application or global preference storage?
5. Does the proposed integration remain removable and explicitly versioned?
6. Would the package still work if UI, Dialogue, Audio, Settings, Save, and Diagnostics packages were absent?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoLocalization must:

- Compile and run with only Unity, the declared Unity Localization dependency, and its transitive platform dependencies.
- Initialize without First Light, EchoUI, EchoSettings, EchoSave, Jukebot, EchoDialogue, EchoDiagnostics, or EchoBuildTools.
- Keep project strings, tables, assets, fonts, and Locale assets outside immutable package source.
- Use a direct setup path and a direct-scene development initializer.
- Remain diagnosable without The Observatory.
- Treat all optional peer connections as bridges or project adapters.
- Avoid direct references to Addressables APIs in the neutral package surface unless a later revision explicitly approves them.
- Fail safely when preference, presentation, dialogue, audio, build, or diagnostics collaborators are absent.
- Preserve project-owned Unity Localization data when samples or EchoLocalization are removed.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Locale selection, text/asset lookup, formatting, fonts, pseudo, validation, and Laboratory work | Clean-project and Laboratory tests |
| Enter Standalone Laboratory directly | Development root initializes only if absent and enabled | Direct-scene tests |
| Accord absent | Locale is session-only/system/configuration selected | Provider absence tests |
| EchoUI absent | Native or project presentation can consume service/results | Core compile and sample test |
| Dialogue/Jukebot absent | Text and asset APIs remain functional | Bridge absence tests |
| Observatory absent | Structured diagnostics and support snapshot still exist | Snapshot tests |
| Samples deleted | Runtime and Editor compile | Sample removal test |
| Package removed after bridge-first removal | Unity Localization tables/settings remain project-owned | Removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum/version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine | Platform | Yes | Unity 6000.0 | Runtime lifecycle, objects, assets, UI Toolkit sample capability | Package cannot run without Unity |
| `com.unity.localization` | Platform package | Yes | Planned 1.5.12 | Locales, tables, Smart Strings, localized assets, pseudo-locales, import/export, backend selection | EchoLocalization cannot compile; project tables remain |
| Addressables via Unity Localization | Transitive platform | Indirect | Backend-resolved | Unity Localization asset loading | Core does not directly own provider/version |
| Optional Echo bridge packages | Bridge | No | Per bridge spec | Settings/UI/dialogue/audio/build/diagnostics integrations | Core remains functional |

The exact package dependency is approved as a planned implementation baseline, not a tested compatibility claim. A clean Unity 6000.3.8f1 install must verify availability before the manifest is released.

### 6.4 Forbidden dependencies

- Any peer Echo runtime package in the core Runtime assembly.
- uGUI or TextMeshPro in the neutral Runtime assembly.
- Project gameplay assemblies.
- Samples or test assemblies.
- Translator credentials, Google account secrets, remote TMS SDKs, or network libraries in the core.
- Raw `AssetDatabase` APIs at runtime.
- Reflection-based discovery of arbitrary consumers/providers.
- Hard-coded scene names, UI prefabs, table names, entry names, locale names, Resources paths, tags, or layers.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---|---|
| ELOC-CAP-001 | Duplicate-safe runtime authority | One application-session localization root claims authority before backend initialization or subscriptions. | Approved | Yes | Runtime |
| ELOC-CAP-002 | Unity Localization backend | Use the official `com.unity.localization` package as the primary runtime and authoring backend. | Approved | Yes | Runtime/Editor |
| ELOC-CAP-003 | Deterministic initial locale selection | Resolve development override, persisted preference, system locale, configured fallback, and source locale in explicit order. | Approved | Yes | Runtime |
| ELOC-CAP-004 | Transactional locale changes | Serialize locale changes, preload critical content, retain prior locale on pre-commit failure, and publish structured results. | Approved | Yes | Runtime |
| ELOC-CAP-005 | Locale catalog | Expose enabled, hidden, pseudo, source, regional, and direction metadata without duplicating Unity Locale assets. | Approved | Yes | Runtime/Data |
| ELOC-CAP-006 | Fallback policy | Validate and report deterministic locale fallback graphs, maximum depth, and source-locale policy. | Approved | Yes | Runtime/Editor |
| ELOC-CAP-007 | Localized text references | Resolve text by stable table-collection and entry identity rather than display names. | Approved | Yes | Runtime/Data |
| ELOC-CAP-008 | Smart String arguments | Provide bounded, explicit argument sets for plural, select, list, date, time, number, percent, and currency formatting. | Approved | Yes | Runtime |
| ELOC-CAP-009 | Localized asset resolution | Resolve locale-specific Unity objects with type-safe, disposable, generational leases. | Approved | Yes | Runtime |
| ELOC-CAP-010 | Font and script profiles | Map locales or script groups to project-owned font assets, fallback chains, direction metadata, and validation fixtures. | Approved | Yes | Runtime/Data/Editor |
| ELOC-CAP-011 | Pseudo-localization | Use Unity Pseudo Locales for accented, expanded, encapsulated, and direction-stress development checks. | Approved | Yes | Runtime/Editor/Sample |
| ELOC-CAP-012 | Missing-content policy | Return structured results and configurable development/release presentation without throwing on normal absence. | Approved | Yes | Runtime |
| ELOC-CAP-013 | Critical content preload groups | Preload project-selected startup tables/assets before locale publication completes. | Approved | Yes | Runtime/Data |
| ELOC-CAP-014 | Locale-change events | Publish semantic lifecycle events only after authoritative state changes. | Approved | Yes | Runtime |
| ELOC-CAP-015 | Standalone diagnostics | Expose state, locale, fallback, lookup, asset, formatting, and content-health information without Observatory. | Approved | Yes | Runtime/Editor |
| ELOC-CAP-016 | Setup and repair | Create/link settings, source locale, profile, pseudo locales, and templates through dry-run, repeat-safe tooling. | Approved | Yes | Editor |
| ELOC-CAP-017 | Completeness validation | Measure source and shipping-locale coverage, fallbacks, asset types, font glyphs, and pseudo-locale release exclusion. | Approved | Yes | Editor |
| ELOC-CAP-018 | Import/export workflow guidance | Validate and report official Unity CSV/XLIFF workflows without owning translator credentials or a remote TMS. | Approved | Yes | Editor |
| ELOC-CAP-019 | Direct-scene development initialization | Create only the minimum localization authority when absent and explicitly enabled. | Approved | Yes | Runtime/Sample |
| ELOC-CAP-020 | Accord preference bridge | Persist player locale choice as a global preference without making Accord mandatory. | Approved | No | Bridge |
| ELOC-CAP-021 | Looking Glass presentation bridge | Provide locale, direction, font, and invalidation signals without owning UI layout. | Approved | No | Bridge |
| ELOC-CAP-022 | Voices bridge | Resolve localized dialogue text and optional voice assets without owning conversation flow. | Approved | No | Bridge |
| ELOC-CAP-023 | Resonance bridge | Provide localized audio assets/cues while Jukebot retains playback authority. | Approved | No | Bridge |
| ELOC-CAP-024 | Foundry validation bridge | Validate shipping locales, platform metadata, completeness, and pseudo exclusion during build preflight. | Approved | No | Bridge |
| ELOC-CAP-025 | Workshop setup facade | Expose ADR-001 Editor planning/apply/validate/report operations. | Approved | No | Editor |
| ELOC-CAP-026 | UI Toolkit sample binding | Demonstrate official Unity 6 localization bindings without making UI Toolkit presentation authoritative. | Approved | No | Sample |
| ELOC-CAP-027 | Translation status metadata | Optional Draft/Reviewed/Approved metadata and review reports. | Deferred | No | Editor/Data |
| ELOC-CAP-028 | Remote TMS providers | Vendor-specific translation-management synchronization. | Deferred | No | Provider |
| ELOC-CAP-029 | Advanced RTL shaping/layout provider | Explicit provider for bidirectional shaping and layout mirroring beyond neutral metadata. | Deferred | No | Provider/Bridge |
| ELOC-CAP-030 | Runtime content updates | Remote/Addressables-delivered localization revisions with signed compatibility policy. | Deferred | No | Provider |

### 7.2 MVP capability set

The smallest complete first release includes:

1. One duplicate-safe persistent localization authority.
2. One project-owned configuration linked to Unity Localization Settings.
3. Deterministic initial locale selection and runtime locale changes.
4. Source, shipping, hidden, and pseudo locale descriptors.
5. Stable localized text and asset references.
6. Smart String argument and regional formatting facade.
7. Bounded fallback and missing-content policy.
8. Typed localized asset leases and release ownership.
9. Locale font/direction profiles and Editor glyph coverage validation.
10. Critical preload groups for startup and locale publication.
11. Structured diagnostics and redacted support snapshots.
12. Repeat-safe setup/repair and completeness validation.
13. A Standalone Localization Laboratory.
14. ADR-001 setup facade.
15. Optional bridges remain separate deliverables.

### 7.3 Later capability set

- Translation status and review metadata.
- Source-text revision/staleness detection.
- Advanced asset preload catalogs and memory budgets.
- Remote translation-management providers.
- Runtime localization-content update providers.
- Advanced bidirectional shaping and automated layout-mirroring providers.
- Localized subtitle timing and voice-lip metadata bridges.
- Addressables remote-catalog verification and signed content compatibility.
- Translator-facing context screenshot and reference tooling.
- Locale-specific controller glyph wording and platform storefront metadata expansion.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Custom replacement for Unity Localization | Rejected | Duplicates a mature supported backend and increases migration risk | Only if Unity backend becomes unusable for approved targets |
| Machine translation in core | Rejected | Credentials, cost, quality, privacy, and provider lock-in | Separate researched provider only |
| Remote Google Sheets credentials in core | Rejected | Network and secret ownership violate neutral package boundary | Explicit provider/project tooling |
| Automatic full RTL shaping/mirroring | Deferred | UI/backend-specific and too broad for neutral MVP | Approved presentation provider and tested target languages |
| Runtime font atlas mutation | Deferred | Platform, memory, licensing, and determinism risk | Dedicated font-generation design |
| Currency conversion | Rejected | Formatting is not economic conversion | Separate commerce/economy authority |
| Localized save IDs | Rejected | Display values are not stable identity | Never |
| User-authored runtime translation mods | Deferred | Security, schema, distribution, and moderation concerns | Dedicated mod/content-provider specification |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Echo configuration, locale profiles, font profiles, preload groups, stable references, missing/fallback policy | Current effective locale, active requests, loaded handles, caches, or UI instances |
| Runtime state/behavior | Root, service, Unity backend adapter, locale transaction, lookup/formatting service, asset leases, diagnostics | Editor logic, translation authorship, UI layout, dialogue flow, audio playback, storage implementation |
| Presentation/feedback | Native Unity localizers, UI Toolkit bindings, optional EchoUI/Dialogue/Jukebot bridges, Laboratory views | Locale truth, table mutation, preference storage, or gameplay rules |

### 8.2 Component topology

```text
Project-owned Unity Localization assets
├── Localization Settings
├── Locale assets and fallback metadata
├── String/Asset Table Collections
└── localized content and fonts
            |
            v
EchoLocalizationConfiguration
├── source/startup policy
├── enabled/hidden/pseudo locale profile
├── critical preload groups
├── missing/fallback policy
├── font/script profiles
└── limits/diagnostics
            |
            v
EchoLocalizationRoot
├── authority claim
├── EchoLocalizationService
│   ├── locale transaction coordinator
│   ├── lookup/format facade
│   ├── asset lease registry
│   ├── cache/version state
│   └── diagnostic snapshot
└── UnityLocalizationBackend
            |
            +--> native Unity localizers and project consumers
            +--> optional Accord / UI / Dialogue / Jukebot / Foundry bridges
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes |
| Root type | `EchoLocalizationRoot` |
| Duplicate behavior | First valid root claims authority in `Awake`; duplicates reject themselves before backend initialization, subscriptions, caches, or locale changes |
| Initialization trigger | Explicit `InitializeAsync` called by root/First Light step; optional auto-start for standalone setup |
| Default lifetime | Application session |
| Shutdown behavior | Stop admitting requests, settle/cancel pre-commit work, unsubscribe, release owned leases, clear static access |
| Direct-scene behavior | Development initializer creates configured root only when absent and explicitly enabled |
| Test injection seam | `ILocalizationBackend`, `ILocalePreferenceProvider`, `ILocalizationClock`, and service constructor/factory |

### 8.4 Lifecycle sequence

1. **Claim authority** before side effects.
2. **Validate configuration** and backend references.
3. **Initialize backend** and wait within a bounded timeout.
4. **Build locale catalog** from project configuration and backend Locales.
5. **Resolve initial locale** through the approved precedence chain.
6. **Prepare critical content** for that locale.
7. **Publish effective locale** and runtime version.
8. **Enter Ready** and admit lookup/change requests.
9. **Serialize locale changes** through validate, prepare, publish, notify, and settle phases.
10. **Shutdown** by stopping admission, releasing leases, unsubscribing, and clearing static state.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Authority claim in `Awake` | Duplicate destroys/disables itself and reports `ELOC-AUTH-001`. | Existing authority remains unchanged. | ELOC-AUTH-001 |
| Missing Echo configuration | Preflight | Blocking setup panel/report. | No backend call or locale mutation. | ELOC-INIT-001 |
| Missing Unity Localization Settings | Preflight | Blocking setup panel/report. | No backend initialization. | ELOC-INIT-002 |
| Backend initialization timeout | Initialization | Failed state with retry guidance. | No partial Ready claim; release owned operations. | ELOC-INIT-003 |
| No valid initial locale | Selection | Blocking startup status. | Remain Failed; do not invent a locale. | ELOC-LOC-001 |
| Unsupported locale request | Request validation | Structured denial. | Keep current locale. | ELOC-LOC-002 |
| Locale preparation failure | Before commit | Structured failure. | Keep previous locale. | ELOC-LOC-004 |
| Locale publication failure | Commit | Structured failure and rollback result. | Restore previous locale when possible; otherwise expose actual effective locale and fault state. | ELOC-LOC-004 |
| Fallback cycle | Editor validation/runtime guard | Blocker report. | Stop fallback traversal. | ELOC-FALLBACK-001 |
| Missing text entry | Lookup | Configured safe marker/fallback result. | Return `Missing`, never throw for normal absence. | ELOC-ENTRY-001 |
| Formatting failure | Lookup | Safe fallback text or marker and actionable code. | Return failure result; no argument data in logs. | ELOC-FMT-001 |
| Localized asset type mismatch | Asset load | Structured error. | Release backend handle and return no lease. | ELOC-ASSET-002 |
| Stale asset lease | Dispose/access | Development warning/result. | Do not release a recycled operation twice. | ELOC-ASSET-003 |
| Missing font glyph | Validation/runtime advisory | Coverage report or optional marker. | Use configured fallback chain if available. | ELOC-FONT-002 |
| Preference provider failure | Initialization/change completion | Warning separated from locale success. | Continue session-only. | ELOC-PERSIST-001 |
| Optional bridge failure | Bridge callback | Bridge-specific warning. | Core locale change completes independently. | Bridge-owned code |

### 8.6 Initial locale selection precedence

The default order is:

1. Explicit development-only override, when enabled.
2. Valid persisted player preference from a registered provider.
3. Valid system locale match.
4. Configured startup fallback.
5. Configured source locale.
6. Blocking failure when none is valid.

The result records which source won and why every earlier candidate was rejected. An explicit runtime player request outranks the automatic selection chain after initialization.

### 8.7 Locale transaction phases

```text
Requested
 -> Validating
 -> Preparing backend/critical content
 -> Commit ready
 -> Publishing effective Locale
 -> Invalidating locale-versioned caches
 -> Notifying semantic listeners
 -> Persisting explicit preference through provider
 -> Completed
```

Preference persistence occurs after runtime publication and is reported separately. A storage failure does not roll back a valid locale change.

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoLocalizationConfiguration` | Root/backend/policy/limits | Configuration domain ID | No | Yes |
| Unity `LocalizationSettings` | Backend Locales/tables/selectors/database | Backend asset identity | Backend-controlled | Yes |
| `LocalizationProfile` | Player-visible locale catalog and package metadata | Locale codes + profile ID | No | Yes |
| `LocalizedFontProfile` | Font assets, fallback chain, fixtures, direction/script notes | Profile ID | No | Yes |
| `LocalizationPreloadGroup` | Critical tables/assets by stable provider identity | Group ID | No | Yes |
| `LocalizedTextReference` | Table collection + entry stable identity | Provider IDs | No | Embedded/project-owned |
| `LocalizedAssetReference` | Table collection + entry stable identity | Provider IDs | No | Embedded/project-owned |
| Unity String/Asset Table Collections | Concrete localized content | Unity Localization stable table/entry IDs | Authoring only | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| Effective locale | Service | Application session | Re-selected at initialization | Preference provider may store `LocaleId` |
| Locale transaction | Service | One request | Dispose after settlement/history copy | Never serialized |
| Locale version | Service | Session monotonic value | Reset on root initialization | Never serialized |
| Lookup/format caches | Service/backend | Session/configured bounds | Invalidate on locale/version/content change | Never serialized |
| Asset lease registry | Service/backend | Until lease disposal/shutdown | Clear on shutdown | Never serialized |
| Diagnostic history | Root/service | Bounded session | Clear on root initialization | Optional redacted export only |
| Completeness report | Editor tool | One scan/report file | Recompute | Versioned report, not runtime state |

### 9.3 Stable identifiers

EchoLocalization distinguishes four identity domains:

1. **Locale identity** - canonical `LocaleId` strings, normally backend locale codes such as BCP 47-style language/region codes. Custom/pseudo codes must be explicitly declared.
2. **Unity Localization table identity** - the backend's stable table-collection identity, not a table display name.
3. **Unity Localization entry identity** - the backend's stable entry ID, not an entry display key.
4. **Echo configuration/profile identity** - SFGSS-003 domain IDs generated and validated by Echo tooling.

A generic Unity AssetDatabase GUID must not be described as the runtime locale or content identity merely because the authoring asset has one. The Unity Localization provider's own runtime table-collection GUID is treated as a provider-defined stable ID because the backend serializes and resolves it in Player builds.

### 9.4 ScriptableObject safety

Project-owned profiles and references remain immutable during play. The package must not write:

- selected locale,
- fallback traversal state,
- cache contents,
- loaded handles,
- missing-entry counters,
- current font,
- or preference state

back into shared assets. Runtime snapshots are copied into service-owned models.

### 9.5 Serialization and migration

- Configuration and profile assets carry explicit schema versions.
- Stable content references preserve provider ID, table collection identity, entry identity, and expected content kind.
- Unsupported newer schema assets block mutation and remain unchanged.
- Migrations are contiguous, test-fixtured, non-destructive, and preserve source assets or backups.
- Locale preference persistence stores only the canonical `LocaleId` plus provider schema metadata.
- Unknown configuration extension records are preserved where the serializer format supports them.
- Concrete translation tables use Unity Localization's supported migration path; Echo tools validate but do not rewrite them silently.

### 9.6 Missing-content state

A lookup result separates:

- requested locale,
- effective locale,
- locale that supplied the result,
- fallback chain attempted,
- stable table/entry identities,
- status,
- and diagnostic code.

The resolved string or asset is data, not identity. Missing content is expected operational input and must not throw by default.

### 9.7 Locale/font metadata

`LocaleDescriptor` may include:

- canonical locale code;
- English display name and project-selected native display name;
- source/shipping/hidden/development/pseudo flags;
- text direction;
- regional parent;
- fallback summary;
- font profile ID;
- optional flag/icon reference for presentation;
- translation readiness state;
- formatting culture identity.

Flags/icons are presentation metadata, not locale identity and not a substitute for readable names.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| EchoLocalizationRoot | sealed MonoBehaviour | Duplicate-safe runtime authority and lifecycle owner. | Scene/prefab; one application-session authority. |
| EchoLocalizationConfiguration | ScriptableObject | Project-owned backend reference, source locale, policies, preload groups, limits, and diagnostics configuration. | Created by setup tool; project-owned. |
| LocalizationProfile | ScriptableObject | Locale catalog presentation, shipping/pseudo visibility, direction, font profile, and project labels. | Project-owned; references backend Locale assets. |
| LocalizedFontProfile | ScriptableObject | Primary font, fallback chain, required-character fixtures, and script/direction policy. | Project-owned. |
| ILocalizationService | interface | Locale selection, lookup, formatting, asset loading, status, and events. | Implemented by root-owned service. |
| ILocalizationBackend | interface | Adapter over Unity Localization initialization, locale selection, text, asset, and table operations. | Injected; default Unity backend. |
| LocaleId | readonly value type | Canonical locale identifier independent from display name and raw AssetDatabase GUID. | Constructed from validated backend locale code. |
| LocaleDescriptor | immutable DTO | Display/native names, source/pseudo/hidden flags, direction, region, and availability. | Created from configuration and backend metadata. |
| LocalizationRuntimeState | enum | Uninitialized, Initializing, Ready, ChangingLocale, Failed, ShuttingDown. | Root-owned. |
| LocaleSelectionSource | enum | DevelopmentOverride, PersistedPreference, SystemLocale, ConfiguredFallback, SourceLocale, ExplicitRequest. | Result metadata. |
| LocaleChangeRequest | immutable request | Requested locale, reason, admission policy, persistence intent, and cancellation. | Caller-created. |
| LocaleChangeResult | immutable result | Status, previous/effective locale, phase, fallback/preload facts, diagnostics, and persistence result. | Returned by service. |
| LocaleChangeHandle | generational handle | Optional observation/cancellation handle for an accepted locale transaction. | Root-owned lease. |
| LocalizedTextReference | serializable value | Stable Unity Localization table-collection and entry identity plus optional fallback/missing policy override. | Project-owned field. |
| LocalizationArguments | bounded value collection | Named primitive/date/list/provider-safe values for Smart String formatting. | Caller-created; copied/detached. |
| LocalizedTextResult | immutable result | Text, requested/effective/source locale, fallback path, status, and diagnostic code. | Returned by lookup. |
| LocalizedAssetReference | serializable value | Stable table/entry identity for localized Unity objects. | Project-owned field. |
| LocalizedAssetLease<T> | disposable generational lease | Loaded localized asset plus source locale and release ownership. | Root/backend-owned. |
| LocalizedAssetResult<T> | immutable result | Status and optional lease, fallback path, type information, and diagnostic code. | Returned by asset lookup. |
| LocalizationPreloadGroup | ScriptableObject | Critical tables/assets required before startup or locale publication completes. | Project-owned. |
| LocalizationDiagnosticSnapshot | immutable DTO | Runtime state, versions, locale, counts, last results, and redacted health data. | Produced on request. |
| ILocalePreferenceProvider | interface | Load and save the preferred locale without embedding storage in the core. | Optional Accord/project provider. |
| ILocalizationContentConsumer | interface | Optional explicit invalidation/refresh seam for project systems that do not use native bindings. | Project/bridge implementation. |
| ILocalizationClock | interface | Unscaled timeout and diagnostic timing. | Injected for tests. |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `Awaitable<LocalizationInitializeResult> InitializeAsync(CancellationToken)` | Initialize backend and initial locale | Claimed root, valid config | Ready or structured failure | Main-thread entry/completion |
| `Awaitable<LocaleChangeResult> RequestLocaleAsync(LocaleChangeRequest, CancellationToken)` | Change effective locale | Ready and enabled locale | Success, denied, cancelled, too late, failed, or rolled back | Main-thread authority; backend may await async loads |
| `IReadOnlyList<LocaleDescriptor> AvailableLocales` | Player/project locale catalog | Initialized | Immutable snapshot | Main thread |
| `LocaleId EffectiveLocale` | Current authoritative locale | Initialized | Invalid/default before Ready | Main thread |
| `LocalizedTextResult ResolveText(LocalizedTextReference, LocalizationArguments)` | Synchronous cached/available lookup | Ready; backend data available | Structured text/fallback/missing/format result | Main thread unless backend proves safe |
| `Awaitable<LocalizedTextResult> ResolveTextAsync(...)` | Async text lookup/preload path | Ready | Structured result | Main-thread completion |
| `Awaitable<LocalizedAssetResult<T>> LoadAssetAsync<T>(LocalizedAssetReference, CancellationToken)` | Load localized asset | Ready and supported Unity Object type | Disposable lease or structured failure | Main-thread completion; backend manages async provider |
| `string FormatNumber(...)`, `FormatDate(...)`, `FormatCurrency(...)` | Culture-aware formatting facade | Valid effective/specified locale | Structured fallback or exception-free failure result in advanced overloads | Pure/controlled; culture snapshot |
| `LocalizationDiagnosticSnapshot CaptureSnapshot()` | Redacted package health | Any root state | Immutable snapshot | Main thread |
| `void RegisterPreferenceProvider(ILocalePreferenceProvider)` | Optional durable preference seam | Before or after Ready | Exact registration handle/result | Main thread |
| `IDisposable RegisterConsumer(ILocalizationContentConsumer)` | Explicit refresh/invalidation consumer | Ready/initializing | Disposable registration | Main thread |
| `Awaitable ShutdownAsync()` | Stop requests and release ownership | Root exists | Idempotent completion | Main thread |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `InitializationStateChanged` | Root/service | After state changes | Previous/current state and code | Diagnostics/presentation only |
| `LocaleChangeStarted` | Service | After request admission | Request ID, previous/requested locale, reason | Must not mutate service recursively |
| `EffectiveLocaleChanged` | Service | After backend publication and state update | Previous/effective locale, source, locale version | May invalidate presentation/content caches |
| `LocaleChangeCompleted` | Service | Exactly once after transaction settles | Full result including persistence outcome | Listener exceptions isolated |
| `LocalizedContentInvalidated` | Service | After successful locale/content version change | Locale version and scope | Consumers refresh as needed |
| `LocalizationWarningRaised` | Service | After bounded diagnostic record is created | Stable code and redacted context | No player text/argument data |

Events occur after authoritative state changes. No listener is required for locale selection or lookup to complete.

### 10.4 Async and cancellation policy

- Public asynchronous operations return fresh Unity `Awaitable<T>` instances.
- Entry and completion occur on the Unity main thread.
- Backend initialization and localized asset operations may await backend async handles through the adapter.
- Cancellation is honored before the locale publication commit point.
- After publication begins, cancellation returns `TooLate`; the transaction settles and reports the actual effective locale.
- One locale change runs at a time. Default admission keeps one latest pending request. A configuration may choose reject-while-busy.
- Scene destruction of a caller does not implicitly cancel a root-owned transaction unless the caller supplied a cancellation token.
- Shutdown stops new requests, cancels pre-commit work where safe, and waits within a bounded timeout before forced cleanup.

### 10.5 API ergonomics

**Novice path**

1. Run Setup.
2. Link/create Unity Localization Settings.
3. Choose source and startup locales.
4. Create a profile and sample table.
5. Add the root prefab or First Light step.
6. Use native Unity Localize components or sample bindings.
7. Open the Localization Laboratory.

**Programmer path**

- Inject/use `ILocalizationService`.
- Store stable `LocalizedTextReference` / `LocalizedAssetReference`.
- Request locale changes through structured requests.
- Consume results and semantic events.
- Register optional preference/presentation/content providers explicitly.
- Test through fake backend, clock, and preference provider.


## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoLocalization and the approved Unity Localization package version.
2. Open **Tools > EchoDevGames > Many Tongues > Setup**.
3. Scan the project for existing Localization Settings, Locales, tables, profile assets, and roots.
4. Choose **Adopt Existing** or **Create Missing** behavior.
5. Select the source locale, startup fallback, player-visible locales, pseudo locales, font profiles, and root/direct-scene policy.
6. Preview a dry-run plan showing every created or modified asset.
7. Apply only the approved plan.
8. Open the Standalone Localization Laboratory.
9. Run validation and export the initial readiness report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create Echo configuration | Configuration asset | Nothing existing | Yes | Unity Undo/create receipt | Setup receipt |
| Link existing Localization Settings | Reference in Echo configuration | Echo config only | Yes | Unity Undo | Link report |
| Create Localization Settings | Project-owned backend asset | Project Settings reference | Yes, when absent | Unity Undo/receipt | Setup receipt |
| Create Locale assets | Selected project Locales | Available Locales list | Yes by canonical ID | Unity Undo | Locale report |
| Create pseudo locales | Development pseudo assets | Available Locales/profile | Yes by stable ID | Unity Undo | Pseudo report |
| Create table templates | Empty String/Asset collections | Nothing existing | Yes by stable collection ID/name conflict rules | Unity Undo | Table report |
| Create font profiles | Empty project-owned profiles | Echo config/profile references | Yes | Unity Undo | Font report |
| Create root prefab/Boot instance | Package root prefab or scene instance | Approved Boot scene only | Yes, duplicate-aware | Scene backup/Undo | Scene receipt |
| Repair missing references | Eligible missing Echo-owned references | Echo assets/scenes only | Yes | Dry-run + Undo | Repair report |
| Generate validation report | Report asset/file | Nothing | Yes | Not applicable | Versioned report |

No setup operation silently translates text, overwrites a table entry, changes a font asset, removes a Locale, changes Addressables groups, adds a shipping language, or edits production UI.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Many Tongues Setup | Installer | Create/adopt configuration and project assets | No |
| Localization Profile Inspector | Designer | Locale visibility, direction, source, font, fallback summary | No |
| Content Completeness Window | Writer/tester | Source vs target entry/asset coverage and fallback-only content | No |
| Font Coverage Inspector | UI/localization | Required glyph and translated-content coverage | No |
| Fallback Graph Viewer | Maintainer | Visualize order, cycles, repeated nodes, and depth | No |
| Pseudo Preview Launcher | UI/tester | Enter Edit/Play preview with approved pseudo locale | No |
| Reference Inspector | Programmer/designer | Show stable table/entry identity and rename safety | No |
| Support Snapshot Exporter | Tester/support | Export redacted backend/runtime/content health | No |
| Migration/Removal Planner | Maintainer | Preview schema migration or Echo-specific metadata conversion | No |
| Localization Laboratory Controller | Tester | Inject delays, failures, missing entries, fallback, fonts, and providers | Sample only |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| ELOC-VAL-001 | Echo configuration missing | Blocker | Yes | Create only |
| ELOC-VAL-002 | Unity Localization Settings missing/unlinked | Blocker | Yes | Create/link with approval |
| ELOC-VAL-003 | Source locale missing or disabled | Blocker | Yes | Manual selection |
| ELOC-VAL-004 | Duplicate canonical locale code | Blocker | No | No |
| ELOC-VAL-005 | Fallback cycle | Blocker | No | No |
| ELOC-VAL-006 | Fallback depth exceeds policy | Error | No | No |
| ELOC-VAL-007 | Required source entry missing | Blocker | No | No |
| ELOC-VAL-008 | Shipping translation missing without allowed fallback | Error | No | No |
| ELOC-VAL-009 | Shipping entry resolves only through fallback | Warning/Error by policy | No | No |
| ELOC-VAL-010 | Localized asset type mismatch | Error | No | No |
| ELOC-VAL-011 | Shipping locale has no font profile | Error | Yes | Assign only |
| ELOC-VAL-012 | Required glyph missing | Warning/Error by policy | No | No |
| ELOC-VAL-013 | RTL locale has no declared presentation strategy | Warning | No | No |
| ELOC-VAL-014 | Pseudo locale included in release list | Blocker | Yes | Remove after approval |
| ELOC-VAL-015 | Hard-coded registered sample/UI string | Warning | No | No |
| ELOC-VAL-016 | Unsupported Unity Localization version | Blocker | No | No |
| ELOC-VAL-017 | Root missing from canonical startup | Error/Warning by setup policy | Yes | Add with approval |
| ELOC-VAL-018 | Duplicate roots in scenes/prefabs | Blocker | Yes | Manual review |

### 11.5 Import and export policy

Unity Localization's supported CSV and XLIFF workflows are the approved baseline. EchoLocalization may:

- launch or document official import/export tools;
- validate stable table/entry identity before and after import;
- compare source and target completeness;
- detect new, removed, empty, or fallback-only entries;
- write a versioned import/export report;
- preserve backups before an approved destructive import.

The core does not store Google credentials, call a translation service, or declare translation quality. Google Sheets or a translation-management platform requires an explicit project/provider integration and separate security review.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned support:

- Unity Package Manager Git URL.
- Local path during development.
- Embedded package development.
- Tarball distribution.
- The Workshop selection after implementation.
- Registry distribution only after an approved repository/versioning standard.

The planned package manifest declares an exact Unity Localization dependency. No compatibility status becomes `Tested` or `Supported` until SFGSS-004 evidence exists.

### 12.2 Minimal scene setup

A minimal standalone runtime requires:

1. One project-owned Unity Localization Settings asset.
2. At least one source Locale.
3. At least one String Table Collection and entry for meaningful proof.
4. One `EchoLocalizationConfiguration`.
5. One `EchoLocalizationRoot` with the configuration assigned.
6. Optional native localizer/binding or project script consuming `ILocalizationService`.

No scene name, build index, tag, layer, Resources path, EventSystem, uGUI Canvas, or Echo package is assumed.

### 12.3 Boot-scene setup

Normal production setup places the duplicate-safe root in the canonical Boot/preload scene or initializes it through the First Light bridge. Localization should normally become Ready before menus, legal text, accessibility prompts, or locale-sensitive startup content appears.

The configuration chooses whether source-language startup status may appear while localization initializes. First Light retains startup sequencing authority.

### 12.4 Direct-scene setup

`EchoLocalizationDirectSceneInitializer` is development-only by default:

- detect an existing valid authority;
- adopt it when present;
- create the configured root only when absent;
- mark diagnostics as development initialization;
- never create peer package roots;
- never run in release builds unless explicitly approved.

### 12.5 Scene isolation rule

The Standalone Laboratory contains only EchoLocalization, its declared Unity dependencies, and redistributable sample content. It must not require First Light, The Looking Glass, The Accord, Voices, Resonance, The Observatory, or project code. Simulated providers demonstrate bridge contracts without importing peer packages.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Many Tongues Localization Laboratory** proves the complete standalone loop:

```text
Initialize
 -> inspect locale catalog
 -> resolve/format strings
 -> load/release assets
 -> switch locale
 -> evaluate fallback
 -> run pseudo locale
 -> inspect font/direction
 -> inject failures
 -> export diagnostics
```

The Laboratory uses a small project-owned sample set with a source locale, two translated/regional locales, one RTL descriptor, multiple pseudo locales, one String Table Collection, one Asset Table Collection, font fixtures, and simulated preference/UI/dialogue/audio providers.

### 13.2 Required Test Lab contents

- In-scene or sample README instructions.
- Runtime UI Toolkit or simple dependency-free debug controls.
- Source, complete, partial, regional, RTL, and pseudo locale fixtures.
- Plain and Smart String examples.
- Localized Sprite and AudioClip fixtures with redistributable content.
- Font profile and missing-glyph fixture.
- Fallback graph and missing-entry controls.
- Locale request delay, timeout, cancellation, and publication-failure simulation.
- Asset lease count/stale handle controls.
- Simulated Accord, UI, Dialogue, Jukebot, and Observatory consumers.
- Reset-to-known-state control.
- Redacted snapshot export.
- No production/project or restricted content.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Automation | Status |
|---|---|---|---|---|
| ELOC-LAB-001 | Initialize from the Laboratory scene with the configured source locale. | Root becomes Ready; source locale and selection source are visible. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-002 | Add a duplicate root before Play Mode. | Duplicate is rejected before backend subscription or locale change. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-003 | Start the Laboratory directly with no runtime root. | Development initializer creates only the configured EchoLocalization authority. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-004 | Disable development initialization and enter directly. | Missing authority is reported; no hidden root is created. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-005 | Select a fully translated locale. | Critical content preloads, effective locale changes, and registered surfaces update. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-006 | Select the currently active locale. | Request returns an idempotent AlreadyEffective result. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-007 | Select an unsupported locale. | Request fails visibly and the current locale remains active. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-008 | Issue three rapid locale requests. | Default coalescing preserves the active transaction and keeps only the latest pending request. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-009 | Cancel before locale preparation begins. | Request cancels with no authoritative change. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-010 | Cancel after publication starts. | Result reports TooLate and settles to a known effective locale. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-011 | Remove an entry from the requested locale but keep it in the first fallback. | Fallback text resolves and the attempted chain is visible. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-012 | Remove the entry from all allowed fallbacks. | Configured development missing marker appears with a structured diagnostic. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-013 | Create a direct fallback cycle. | Validation blocks the configuration. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-014 | Create a fallback chain deeper than the configured maximum. | Validation reports the excessive chain and runtime does not recurse without bound. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-015 | Rename a table collection display name. | Stable reference continues to resolve. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-016 | Rename an entry display key. | Stable entry identity continues to resolve. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-017 | Format a plural Smart String with 1 and then 2. | Locale-correct plural branches display. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-018 | Format a date, time, percentage, number, and currency. | Values follow the effective locale culture without changing source values. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-019 | Omit a required Smart String variable. | Formatting failure is visible without throwing or logging private argument data. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-020 | Load a localized Sprite. | Correct locale asset loads and the lease can be disposed. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-021 | Load an asset through fallback. | Fallback source is shown in the result. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-022 | Request the wrong localized asset type. | TypeMismatch is returned and no invalid cast escapes. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-023 | Dispose the same localized asset lease twice. | Second disposal is harmless and diagnosed only at development verbosity. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-024 | Force an old lease token after a slot is recycled. | Stale generational lease is rejected. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-025 | Switch locale while localized assets are leased. | Existing lease policy and new-locale resolution behave as configured. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-026 | Select an accented pseudo locale. | All registered text surfaces show accented pseudo content. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-027 | Select an expanded pseudo locale. | Longer text exposes wrapping and truncation issues. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-028 | Select an encapsulated pseudo locale. | Hard-coded and localized text are visually distinguishable. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-029 | Use a right-to-left locale descriptor. | Direction metadata changes and the sample presenter mirrors only through its explicit adapter. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-030 | Use a locale whose primary font lacks required glyphs. | Font coverage warning lists the missing code points. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-031 | Use a valid fallback font chain. | Required sample glyphs render through the configured fallback. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-032 | Remove the font profile for a shipping locale. | Validation blocks release readiness. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-033 | Load a persisted locale through a simulated Accord provider. | Initial selection identifies the persisted preference source. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-034 | Make the simulated preference provider fail on save. | Locale changes for the session and persistence failure is reported separately. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-035 | Unregister the preference provider at runtime. | Session localization continues without durable storage. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-036 | Register a simulated EchoUI bridge. | Locale and text-direction events update the sample view without transferring UI authority. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-037 | Register a simulated Dialogue consumer. | Localized text resolves without EchoLocalization owning dialogue progression. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-038 | Register a simulated Jukebot consumer. | Localized audio asset resolves without EchoLocalization starting playback. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-039 | Trigger a backend initialization failure. | Root enters Failed with actionable diagnostics and no partial authority. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-040 | Trigger a critical-table preload timeout. | Previous or startup-safe locale remains effective according to phase. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-041 | Open the validation window and scan incomplete tables. | Completeness report lists locale, table, entry, fallback, and severity. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-042 | Generate a support snapshot. | Snapshot contains IDs, states, counts, and codes but not resolved text or arguments. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-043 | Delete all imported sample assets. | Package Runtime and Editor assemblies continue to compile. | Manual with supporting PlayMode tests | Not run |
| ELOC-LAB-044 | Remove a simulated optional bridge. | EchoLocalization and the peer remain independently functional. | Manual with supporting PlayMode tests | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Localized Settings Screen | EchoLocalization + Accord + Looking Glass | Persist locale and update a menu | Depends on two peer authorities |
| Localized Dialogue and Voice | EchoLocalization + Voices + Resonance | Resolve line, subtitle, and voice asset | Does not prove any core alone |
| First Light Localized Startup | EchoLocalization + First Light | Initialize locale before localized splash/legal content | Bridge lifecycle evidence only |
| Foundry Localization Preflight | EchoLocalization + Foundry | Block incomplete shipping language | Editor bridge evidence only |
| Full Suite Showcase | Many packages | Demonstrate composition | Showcase cannot replace Labs |

Samples must be separately importable and removable.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoLocalization is nonvisual at its core. It supplies localized values, locale metadata, direction, font-profile references, invalidation events, and structured missing/error states. Native Unity localizers, UI Toolkit bindings, project presenters, and the Looking Glass bridge own actual presentation.

### 14.2 Required states

Consumers must be able to represent:

- Uninitialized.
- Initializing.
- Ready.
- Locale changing.
- Content loading.
- Empty/missing.
- Fallback used.
- Formatting failure.
- Unsupported/disabled locale.
- Warning.
- Blocking failure.
- Pseudo/development locale.
- Right-to-left direction declared.
- Font/glyph advisory.

### 14.3 Accessibility requirements

- Locale controls must expose readable language names, preferably native and project/source names.
- Language choice must not rely on flag icons alone.
- Locale menus must support keyboard/controller navigation when presented through a compatible UI.
- Text expansion must be tested through pseudo-localization.
- Important meaning must not depend on localized color or audio alone.
- Font profiles must support required glyphs or report gaps.
- Right-to-left direction must be exposed even when advanced shaping/mirroring is not installed.
- Locale changes must not trap focus or silently dismiss player input.
- Timed locale-change messaging uses unscaled time and project-configurable duration.
- Resolved production text is project content and must not be copied into support diagnostics by default.

### 14.4 Visual customization

All player-facing locale selectors, labels, icons, fonts, layout, transitions, missing markers, and error panels are project-owned or supplied by optional presentation samples/bridges. Runtime code must not require a Verse style, one Canvas, or one UI technology.

### 14.5 Right-to-left boundary

The MVP:

- identifies locale direction;
- publishes direction changes;
- validates that a project has declared an RTL strategy;
- supports font profiles and pseudo stress testing;
- permits explicit provider/bridge registration.

It does not promise language-correct bidirectional shaping, line breaking, cursor movement, number embedding, or automatic mirroring for every UI backend. Those claims require target-language and provider evidence.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Initialization/backend state | API/Inspector/report | Development and safe release summary | Constant |
| Effective/source/requested locale | API/Inspector | Development and release | Constant |
| Locale selection source | Snapshot | Development/support | Constant |
| Pending/active locale transaction | Snapshot | Development | Constant |
| Fallback and missing counters | Bounded snapshot | Development/support | Bounded |
| Last lookup/format/asset codes | Bounded history | Development | Bounded |
| Outstanding asset leases | Snapshot | Development | Constant/bounded |
| Font/profile health | Editor validation | Editor | Scan-time |
| Table completeness | Editor validation | Editor/build preflight | Scan-time |
| Backend/package version | Snapshot | Development/support | Constant |
| Redacted support export | JSON/Markdown report | Explicit action | Bounded |

### 15.2 Structured status

Snapshot fields include:

- package/configuration/backend version;
- root identity and initialization source;
- runtime state;
- source and effective locale;
- locale selection source;
- enabled/hidden/pseudo locale counts;
- current locale transaction phase;
- critical preload state;
- fallback depth/last chain IDs;
- missing text/asset and formatting-failure counts;
- active asset lease count;
- registered optional provider/consumer IDs;
- last stable diagnostic codes;
- whether the session used direct-scene development initialization.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ELOC-AUTH-001 | Blocker | A duplicate localization authority attempted to initialize. | Remove or disable the duplicate root. |
| ELOC-INIT-001 | Blocker | Localization configuration is missing. | Assign or generate `EchoLocalizationConfiguration`. |
| ELOC-INIT-002 | Blocker | Unity Localization Settings/backend reference is missing. | Create or link the project Localization Settings asset. |
| ELOC-INIT-003 | Error | Backend initialization failed or timed out. | Review backend/package configuration and generated report. |
| ELOC-LOC-001 | Error | No valid initial locale could be selected. | Enable a source or startup fallback locale. |
| ELOC-LOC-002 | Warning | Requested locale is unsupported, hidden, or disabled. | Select an enabled runtime locale. |
| ELOC-LOC-003 | Warning | A locale request was rejected while another request was active. | Retry or use the configured coalescing policy. |
| ELOC-LOC-004 | Error | Locale publication failed and rollback was attempted. | Review backend state and the locale change receipt. |
| ELOC-FALLBACK-001 | Blocker | Locale fallback cycle detected. | Remove the cycle in Locale metadata. |
| ELOC-FALLBACK-002 | Error | Fallback graph exceeds the configured maximum depth. | Simplify or explicitly raise the bounded depth. |
| ELOC-TABLE-001 | Error | Required table collection is unavailable. | Add it to the project Localization Settings/build. |
| ELOC-ENTRY-001 | Warning | Localized entry is missing after allowed fallback evaluation. | Translate it or approve a fallback/missing policy. |
| ELOC-FMT-001 | Error | Smart String formatting failed. | Correct the source format or supplied argument contract. |
| ELOC-ASSET-001 | Error | Localized asset is missing or failed to load. | Add the asset/fallback or inspect Addressables/backend setup. |
| ELOC-ASSET-002 | Error | Localized asset type does not match the requested type. | Correct the table entry or request type. |
| ELOC-ASSET-003 | Warning | Localized asset lease limit was reached or a stale lease was used. | Dispose leases and inspect ownership. |
| ELOC-FONT-001 | Error | A shipping locale has no approved font profile. | Assign a font profile. |
| ELOC-FONT-002 | Warning | Required glyphs are missing from the configured font chain. | Add glyphs or a fallback font. |
| ELOC-DIR-001 | Advisory | Right-to-left direction is declared but no layout/shaping adapter is registered. | Install/configure an approved presentation provider if required. |
| ELOC-PSEUDO-001 | Blocker | A pseudo locale is included in a release locale set. | Remove it from release selection/build metadata. |
| ELOC-PERSIST-001 | Warning | Locale preference load or save failed. | Continue session-only and inspect the preference provider. |
| ELOC-PRIV-001 | Error | A diagnostic/export path attempted to include resolved text or argument values. | Redact content and regenerate the report. |
| ELOC-VER-001 | Blocker | Unity Localization package version is unsupported or unverified for the selected release claim. | Use an approved version or revise the compatibility record. |

### 15.4 Observatory bridge

A separate bridge may publish:

- localization state and effective locale;
- backend initialization and version;
- shipping/source/pseudo locale counts;
- current transaction phase/timing;
- missing/fallback/format counts;
- outstanding asset leases;
- font/table health summary;
- recent redacted diagnostic codes.

EchoLocalization never requires The Observatory.

### 15.5 Logging policy

- Use the globally unique `ELOC-*` namespace.
- Avoid per-frame logging and lookup spam.
- Do not log resolved player text, Smart String argument values, dialogue text, player names, typed input, or full table contents.
- Development logs may include stable table/entry IDs and locale codes.
- Absolute project paths are redacted in support exports.
- One repeated missing item may be rate-limited and counted rather than logged every lookup.
- Release logging is configuration-controlled and defaults to actionable warnings/errors only.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Effective locale | Session | EchoLocalization | Re-selected each launch | Runtime |
| Explicit player locale preference | Global preference | Accord/project provider | Optional Yes | Provider |
| Source/enabled locale configuration | Project | Project/Echo configuration | Yes as assets | Unity assets |
| Translation tables/assets | Project content | Project/Unity Localization | Yes as assets/content | Unity Localization |
| Asset leases/caches/requests | Session | EchoLocalization | No | Runtime |
| Content QA reports | Development record | Editor tooling/project | Optional | Repository report |

### 16.2 Standalone behavior

Without EchoSettings:

- configuration/system locale selects the initial Locale;
- explicit choices apply for the current session;
- a project may register its own preference provider;
- the core never silently writes PlayerPrefs or a JSON file;
- next launch re-evaluates the selection chain.

EchoSave is not used for global locale preference. A save slot may contain locale-sensitive gameplay data only when the project owns that separate domain, not the UI language selection itself.

### 16.3 Optional participant/provider contract

`ILocalePreferenceProvider` is narrow and versioned:

- `TryLoadPreferredLocaleAsync`
- `TrySavePreferredLocaleAsync`
- provider stable ID/version
- availability and failure result
- no direct access to EchoLocalization private state
- no right to force an invalid locale

The Accord bridge maps the preference into its normal committed/effective settings transaction. EchoLocalization validates and applies; Accord stores.

### 16.4 Failure and recovery

- Missing preference: continue with normal selection.
- Unsupported preference: ignore with diagnostic and preserve storage until user chooses another valid locale.
- Provider unavailable: continue session-only.
- Save failure: locale change succeeds for session; persistence result warns.
- Corrupt provider data: provider reports failure; EchoLocalization does not repair private storage.
- Newer provider schema: preserve/ignore according to provider authority.
- Package removal: project-owned Unity tables remain; Echo-specific configuration requires explicit removal/migration guidance.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Localization is consumed broadly, so dependency direction must remain especially strict. Peer packages store stable localization references or consume service/events through bridges; EchoLocalization core never imports peer runtime assemblies. Installing a peer package does not silently alter locale policy.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| First Light | Separate bridge | Integration package | First Light -> EchoLocalization | Startup step, progress, result, diagnostics | No |
| The Accord | Separate bridge | Integration package | Bidirectional | Preferred `LocaleId`, apply result | No |
| The Looking Glass | Separate bridge | Integration package | EchoLocalization -> UI | Locale, direction, font profile, invalidation, localized refs | No |
| Resonance | Separate bridge/project adapter | Integration/project | Localization -> Jukebot | Localized AudioClip/cue asset lease/reference | No |
| Voices | Separate bridge | Integration package | Dialogue -> Localization | Text/asset references, arguments, results | No |
| The Will | Project/bridge | Project/owner | Input -> Localization | Localized binding labels and accessibility text references | No |
| The Observatory | Separate bridge | Integration package | Localization -> Diagnostics | Redacted snapshot/provider | No |
| The Foundry | Separate Editor bridge | Integration package | Foundry -> Localization | Validation request/results, locale/platform metadata | No |
| The Workshop | ADR-001 Editor facade | EchoLocalization Editor assembly | Workshop -> setup | Plan/apply/validate/report JSON | No |
| EchoSave | None normally | N/A | N/A | Global locale is not save-slot data | No |

### 17.3 Bridge placement decision

- Two-package Echo integrations ship separately when they reference both APIs.
- The ADR-001 setup facade lives in EchoLocalization's Editor assembly because it is package-owned and compile-safe.
- Native Unity Localization backend code lives inside EchoLocalization because the backend is a declared hard platform dependency.
- Vendor TMS, shaping, remote update, or platform metadata SDKs ship as providers.
- Game-specific text/context translation remains project adapter code.

### 17.4 Integration failure behavior

- Missing peer: core remains functional.
- Missing bridge: no behavior changes silently.
- Version mismatch: bridge fails to register and reports its own diagnostic.
- Peer initializes later: bridge may register explicitly and receive the current immutable snapshot.
- Peer shuts down first: bridge unregisters before peer removal.
- EchoLocalization shuts down first: no new callbacks; registrations become invalid safely.
- Bridge exception: isolate it from locale transaction completion.
- Removal order: remove bridge/provider before either peer.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

All targets are planned and remain `Not run`.

| Metric | Planned target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Idle package update work | No per-frame polling in Ready state | Profiler/Laboratory | No package Update allocation/work without active operation |
| Initial locale selection | Bounded by configured critical preloads | Laboratory + Profiler | Complete within project budget or report timeout |
| Locale change | No mixed authoritative state; bounded transaction/history | Laboratory | Settle within configured timeout |
| Cached text lookup | Avoid repeated provider work for unchanged stable lookup where backend permits | PlayMode microfixture | Project-defined budget |
| Locale invalidation | Linear in registered explicit consumers, bounded count | Stress fixture | No unbounded listener list |
| Asset leases | Enforce configured maximum and clean release | Asset stress fixture | No leaked backend handles |
| Completeness scan | Stream/batch large tables with progress/cancellation | Editor fixture | No unbounded memory growth |
| Diagnostic history | Fixed configured capacity | Runtime stress | Never grows without bound |

### 18.2 Allocation policy

- No LINQ in hot runtime lookup/locale change paths unless profiling approves it.
- Copy and bound argument sets before asynchronous use.
- Do not build strings for diagnostics unless the diagnostic will be recorded.
- Reuse internal buffers where safe but never expose mutable pooled state as public results.
- Cache provider conversions and locale descriptors by locale version.
- Formatted strings with dynamic arguments are not globally cached by default.
- Asset leases own backend release state and must not rely on finalizers.
- Editor scans may allocate but must expose progress/cancellation for large projects.

### 18.3 Scene and domain reload behavior

- Unsubscribe backend callbacks on shutdown/domain reload.
- Reset static access under both normal and disabled-domain-reload Play Mode.
- Reject duplicate roots before subscribing.
- Clear pending locale requests, cache version, asset lease registry, and diagnostic history on fresh root initialization.
- Preserve project-owned tables/assets; never write runtime state into them.
- Direct-scene helper marks and cleans only its own created authority.

### 18.4 Scalability limits

Configuration must bound:

- enabled locale count;
- fallback depth;
- simultaneous pending locale requests;
- registered consumers/providers;
- critical preload groups and entries;
- outstanding localized asset leases;
- diagnostic history and per-code rate limits;
- formatting argument count and nesting;
- completeness scan batch size;
- generated report size.

Advertised limits are finalized only after implementation stress evidence.


## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Locale preference is generally low-sensitivity configuration, but localization content can contain:

- player names or generated arguments;
- private dialogue or unreleased story text;
- platform/legal descriptions;
- translator comments;
- proprietary asset names;
- absolute project paths in Editor reports.

Ordinary diagnostics store stable IDs, locale codes, counts, statuses, and codes, not resolved production text or argument values.

### 19.2 Trust boundaries

- Runtime table assets are trusted project/build content, not arbitrary player files.
- Import files are Editor-only, size-limited, encoding-validated, backed up, and previewed before mutation.
- Remote TMS and runtime update providers are absent from the core.
- Smart String arguments are bounded and passed through explicit safe types/providers.
- Locale codes are normalized and validated before lookup.
- File exports use approved project paths and redact absolute machine paths.
- Locale selection is not proof of physical location, citizenship, entitlement, age, or legal jurisdiction.
- Translation completeness does not certify cultural, legal, or accessibility quality.

### 19.3 Platform behavior

| Platform | Compatibility status | Planned behavior | Required evidence |
|---|---|---|---|
| Windows | Planned | Locales, tables, formatting, assets, fonts, pseudo, persistence provider | Clean install, Player build, locale switch, font/asset tests |
| macOS | Planned | Same core behavior; platform metadata validation | Clean install/build and content tests |
| Linux | Planned | Same core behavior; font availability is project-owned | Clean install/build and content tests |
| WebGL | Planned | Async asset behavior and storage provider constraints | Browser build, memory, async, locale switch tests |
| Android | Planned | System locale, BCP 47/resource metadata, app-info validation | Device/build tests |
| iOS | Planned | System locale and app-info metadata validation | Device/Xcode build tests |
| Consoles | Unknown | Provider/platform restrictions to be researched | Platform access and certification evidence |

No platform moves to `Supported` before SFGSS-004 evidence.

### 19.4 External technical basis

The approved design relies on official Unity capabilities available in the current Localization 1.5 line:

- strings and Smart Strings;
- localized assets;
- Locale fallback metadata and ordered fallback evaluation;
- pseudo-localization;
- CSV/XLIFF/Google Sheets tooling;
- UI Toolkit localization bindings in Unity 6;
- localized font and TMP/TextCore font assets;
- backend Localization Settings and available Locale selection.

EchoLocalization wraps these capabilities with package policy and lifecycle rather than reproducing them.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-localization/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
│       ├── Architecture.md
│       ├── Data-and-IDs.md
│       ├── Localization-Backend.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
├── Editor/
├── Samples~/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoLocalizationRoot.cs
│   ├── EchoLocalizationService.cs
│   ├── ILocalizationService.cs
│   ├── LocalizationRuntimeState.cs
│   └── LocalizationResults.cs
├── Configuration/
│   ├── EchoLocalizationConfiguration.cs
│   ├── LocalizationProfile.cs
│   ├── LocalizedFontProfile.cs
│   └── LocalizationPreloadGroup.cs
├── Data/
│   ├── LocaleId.cs
│   ├── LocaleDescriptor.cs
│   ├── LocalizedTextReference.cs
│   ├── LocalizedAssetReference.cs
│   └── LocalizationArguments.cs
├── Backend/
│   ├── ILocalizationBackend.cs
│   └── UnityLocalizationBackend.cs
├── Locale/
│   ├── LocaleSelectionPolicy.cs
│   ├── LocaleChangeCoordinator.cs
│   └── FallbackPolicy.cs
├── Content/
│   ├── LocalizedTextResolver.cs
│   ├── LocalizedAssetLease.cs
│   └── LocalizationFormattingService.cs
├── Fonts/
│   └── LocaleFontResolver.cs
├── Persistence/
│   └── ILocalePreferenceProvider.cs
├── Diagnostics/
│   ├── LocalizationDiagnosticSnapshot.cs
│   └── LocalizationDiagnosticCodes.cs
├── Development/
│   └── EchoLocalizationDirectSceneInitializer.cs
└── EchoDevGames.EchoLocalization.Runtime.asmdef

Editor/
├── Setup/
│   ├── ManyTonguesSetupWindow.cs
│   └── EchoLocalizationSetupFacade.cs
├── Validation/
│   ├── LocalizationValidator.cs
│   ├── CompletenessValidator.cs
│   ├── FallbackGraphValidator.cs
│   └── FontCoverageValidator.cs
├── Authoring/
│   ├── LocalizationProfileInspector.cs
│   ├── ReferenceInspector.cs
│   └── PseudoPreviewLauncher.cs
├── Migration/
├── Reports/
├── Laboratory/
└── EchoDevGames.EchoLocalization.Editor.asmdef

Tests/
├── Editor/
│   └── EchoDevGames.EchoLocalization.Tests.Editor.asmdef
└── Runtime/
    └── EchoDevGames.EchoLocalization.Tests.Runtime.asmdef

Samples~/
└── Standalone Labs/
    └── Many Tongues Localization Laboratory/
```

Optional bridges use separate packages/assemblies under SFGSS-002.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoLocalization.Runtime` | Runtime | Unity core, Unity Localization runtime | Yes | Core authority, backend adapter, data, lookup, formatting, assets, diagnostics |
| `EchoDevGames.EchoLocalization.Editor` | Editor | Runtime, Unity Localization Editor, UnityEditor | False | Setup, validation, reports, migration, ADR-001 facade |
| `EchoDevGames.EchoLocalization.Tests.Runtime` | Test | Runtime, Unity Test Framework | False | Edit/Play runtime tests |
| `EchoDevGames.EchoLocalization.Tests.Editor` | Editor test | Runtime, Editor, Unity Test Framework | False | Setup/validation/migration tests |
| Optional bridge assemblies | Mixed | Both peer package assemblies | False | Explicit removable integrations |

If direct Runtime references to UI Toolkit or specific font packages become necessary, they must be isolated into visible assemblies rather than expanding the neutral core silently.

### 20.4 Repository files

- Concise README with five-minute setup.
- Complete package specification and architecture summary.
- Unity Localization backend/version notes.
- Data/ID/reference guidance.
- Table and font authoring guide.
- Pseudo-localization guide.
- Missing-content and diagnostic-code reference.
- Import/export safety guide.
- Bridge index.
- Migration/removal guide.
- Changelog, license, notices, support/security guidance.
- Release checklist and evidence links.
- Stable `.meta` files and GUIDs.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Planned/tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | Baseline 6000.3.8f1; evidence Not run | Final public floor verified during implementation |
| `com.unity.localization` | Planned 1.5.12 | Planned, not tested | Current official 1.5-line version at specification approval; exact availability must be verified |
| Unity Test Framework | Unity-provided | Not run | Test assemblies only |
| Optional peers | Per bridge | Not run | Bridge package declares exact compatible ranges/releases |

### 21.2 Semantic versioning policy

**Patch**

- diagnostics wording/codes without changed meaning;
- validation fixes;
- internal performance fixes;
- documentation corrections;
- backend adapter bug fixes that preserve behavior.

**Minor**

- new optional locale metadata;
- new formatter or validator;
- new localized asset type support;
- new optional bridge;
- backward-compatible configuration fields/migrations;
- new Laboratory scenarios.

**Major**

- locale-selection precedence changes;
- table/entry reference schema changes;
- public API removal/rename;
- configuration/profile schema incompatibility;
- fallback or missing-content semantic changes;
- provider contract break;
- removal of supported Unity/Localization versions.

### 21.3 Deprecation policy

- Mark APIs/assets obsolete with replacement guidance.
- Keep supported migration for at least one documented minor/major window when practical.
- Preserve serialized type names or use Unity migration attributes where safe.
- Never reuse removed diagnostic, test, capability, or stable IDs.
- Publish removal timing and migration examples.
- Backend deprecations follow Unity upgrade guidance plus Echo-specific migration tests.

### 21.4 GUID and asset compatibility

Public scripts, configuration types, profiles, prefabs, table templates, reports, and samples preserve committed `.meta` files. Moving or renaming an asset retains its GUID when it remains the same public artifact.

Unity Localization table collection and entry identities must remain stable across display-name changes. Echo setup/repair tools must not recreate public assets simply to rename them.

### 21.5 Backend upgrade policy

Before upgrading the Unity Localization dependency:

1. Review official changelog and upgrade guide.
2. Run clean install and upgrade fixtures.
3. Validate table/entry identity, fallback, Smart Strings, localized assets, fonts, UI bindings, pseudo-locales, and platform metadata.
4. Update compatibility records and known limitations.
5. Record an ADR if the package boundary or public behavior changes.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview, backend boundary, and non-goals.
- Installation and exact Unity Localization dependency.
- Five-minute quick start.
- Source and target locale setup.
- Table and stable-reference authoring.
- Smart String and formatting examples.
- Localized asset and lease use.
- Font/script/direction profiles.
- Pseudo-localization and completeness testing.
- Standalone Laboratory guide.
- Missing-content and fallback policy.
- Troubleshooting/diagnostic codes.
- Import/export safety.
- Optional integration index.
- Migration/removal guide.
- Known limitations and evidence status.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Authority and lifecycle.
- Unity backend adapter.
- Data and stable ID model.
- Locale selection and transaction state machine.
- Async/cancellation/commit point.
- Asset lease ownership.
- Formatting argument safety.
- Diagnostics/privacy policy.
- Extension/provider contracts.
- Test strategy and fixture authoring.
- Release workflow.
- Current Notes, ADRs, checkpoints, and status record.

### 22.3 Documentation truth rule

- All examples must compile against the documented release.
- Menu paths and screenshots must match the tested Unity/backend version.
- Translation completeness percentages must state scan scope and fallback policy.
- Platform language claims require execution evidence.
- The package must not describe Unity Localization 1.5.12 as tested until the implementation fixture passes.
- Missing, pseudo, RTL, and font limitations must remain visible.

### 22.4 Living repository and Obsidian workflow

Follow SFGSS-000 and SFGSS-005:

1. Capture discoveries in Current Notes.
2. Label proposals, decisions, tests, risks, and bugs.
3. Promote durable behavior into this specification or an ADR.
4. Move execution evidence into test reports.
5. Update migration, setup, diagnostic, and known-limitations docs when behavior changes.
6. Commit documentation with or adjacent to implementation.
7. Use Git history instead of retaining endless resolved notes.

### 22.5 Repository scan and handoff order

1. README.
2. SFGSS-000.
3. SFGSS-002, SFGSS-003, and SFGSS-004.
4. This package specification.
5. Applicable backend/bridge ADRs.
6. Current Notes.
7. Active checkpoint and test reports.
8. Relevant implementation and tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, selection, fallback graph, format policy, validation, migrations | Pure policy and fixture tests | Yes |
| PlayMode unit/integration | Root, backend adapter, locale changes, lookups, asset leases, events | Fake and Unity backend fixtures | Yes |
| Standalone Laboratory | User-visible isolated localization loop | 44 scenarios in this specification | Yes |
| Bridge Integration Lab | Accord/UI/Dialogue/Audio/Foundry connections | Separate bridge evidence | When bridge ships |
| Showcase | Combined localized game shell | Demonstration only | No |
| Clean-project install | Manifest/dependency/compile/setup | Git/local/tarball | Yes |
| Existing-project migration | Adopt Unity Localization project safely | Tables/Locales/references preserved | Before migration claim |
| Platform build | Runtime/backend/font/asset behavior | Windows/macOS/Linux/WebGL/mobile | Before support claim |

### 23.2 Required test categories

- Installation and assembly independence.
- Authority, duplicates, lifecycle, direct-scene, domain reload.
- Initial locale selection and runtime changes.
- Cancellation, timeout, rollback, repeated/rapid requests.
- Fallback graph, missing content, source policy.
- Stable table/entry identity and rename behavior.
- Smart String arguments and regional formatting.
- Localized asset loading, type safety, leases, release, limits.
- Font profiles, glyph coverage, scripts, text direction.
- Pseudo-localization and hard-coded content detection.
- Setup, repair, import/export, migration, removal.
- Preference and optional bridge absence/presence.
- Diagnostics, privacy, support exports.
- Performance, allocations, scale, platform builds.

### 23.3 Test case registry

| Test ID | Category | Requirement/action | Status |
|---|---|---|---|
| ELOC-T-001 | Installation and assembly | Install from a Git URL into a clean Unity 6000.3 project with the approved Unity Localization dependency. | Not run |
| ELOC-T-002 | Installation and assembly | Install from a local package path into a clean project. | Not run |
| ELOC-T-003 | Installation and assembly | Install from a tarball into a clean project. | Not run |
| ELOC-T-004 | Installation and assembly | Embed the package for package development. | Not run |
| ELOC-T-005 | Installation and assembly | Compile Runtime without any peer Echo package installed. | Not run |
| ELOC-T-006 | Installation and assembly | Compile Editor without any peer Echo package installed. | Not run |
| ELOC-T-007 | Installation and assembly | Compile Runtime with no UnityEditor reference. | Not run |
| ELOC-T-008 | Installation and assembly | Verify the core Runtime assembly does not reference uGUI, TextMeshPro, EchoUI, EchoSettings, EchoSave, Jukebot, or Dialogue. | Not run |
| ELOC-T-009 | Installation and assembly | Verify the package manifest declares the exact approved Unity Localization dependency. | Not run |
| ELOC-T-010 | Installation and assembly | Remove samples without breaking Runtime or Editor assemblies. | Not run |
| ELOC-T-011 | Installation and assembly | Remove EchoLocalization after removing its bridges and confirm the project compiles. | Not run |
| ELOC-T-012 | Installation and assembly | Reinstall EchoLocalization and reopen supported project-owned configuration assets. | Not run |
| ELOC-T-013 | Authority and lifecycle | Create one configured root and initialize successfully. | Not run |
| ELOC-T-014 | Authority and lifecycle | Create two roots in one scene and reject the duplicate before backend subscription. | Not run |
| ELOC-T-015 | Authority and lifecycle | Introduce a duplicate root during scene load and reject it before side effects. | Not run |
| ELOC-T-016 | Authority and lifecycle | Start in the canonical Boot scene and persist the authority across a scene transition. | Not run |
| ELOC-T-017 | Authority and lifecycle | Enter the Standalone Laboratory directly with no root and create only the configured development authority. | Not run |
| ELOC-T-018 | Authority and lifecycle | Enter the Laboratory when a valid root already exists and adopt it without duplication. | Not run |
| ELOC-T-019 | Authority and lifecycle | Disable direct-scene initialization and confirm missing authority is reported instead of created. | Not run |
| ELOC-T-020 | Authority and lifecycle | Initialize with a missing configuration asset and return a blocking result. | Not run |
| ELOC-T-021 | Authority and lifecycle | Initialize with a missing backend settings reference and return a blocking result. | Not run |
| ELOC-T-022 | Authority and lifecycle | Initialize with an empty supported-locale set and return a blocking result. | Not run |
| ELOC-T-023 | Authority and lifecycle | Shutdown releases backend subscriptions and owned asset leases. | Not run |
| ELOC-T-024 | Authority and lifecycle | Domain reload resets static access and does not retain stale authority. | Not run |
| ELOC-T-025 | Authority and lifecycle | Enter Play Mode without domain reload and reset all package static state deterministically. | Not run |
| ELOC-T-026 | Authority and lifecycle | Application quit suppresses unsafe asynchronous callbacks. | Not run |
| ELOC-T-027 | Authority and lifecycle | Backend initialization timeout produces a bounded failure. | Not run |
| ELOC-T-028 | Authority and lifecycle | Backend initialization exception is converted into a structured result and diagnostic. | Not run |
| ELOC-T-029 | Initial locale selection | Select an explicit development override when development overrides are enabled. | Not run |
| ELOC-T-030 | Initial locale selection | Ignore a development override in a release configuration. | Not run |
| ELOC-T-031 | Initial locale selection | Select a valid persisted preference supplied by the Accord bridge. | Not run |
| ELOC-T-032 | Initial locale selection | Reject a persisted locale that is not enabled for the current build. | Not run |
| ELOC-T-033 | Initial locale selection | Select the matching system locale when no persisted preference exists. | Not run |
| ELOC-T-034 | Initial locale selection | Select the closest configured regional parent when exact system locale is unavailable. | Not run |
| ELOC-T-035 | Initial locale selection | Select the configured startup fallback when no system locale matches. | Not run |
| ELOC-T-036 | Initial locale selection | Select the source locale as the final allowed fallback. | Not run |
| ELOC-T-037 | Initial locale selection | Block initialization when no valid initial locale can be selected. | Not run |
| ELOC-T-038 | Initial locale selection | Report the complete selection-source chain in diagnostics. | Not run |
| ELOC-T-039 | Initial locale selection | Do not write a preference when initialization merely follows the system locale. | Not run |
| ELOC-T-040 | Initial locale selection | Persist an explicit player locale choice only through a registered preference provider. | Not run |
| ELOC-T-041 | Initial locale selection | Continue session-only when the preference provider is absent. | Not run |
| ELOC-T-042 | Initial locale selection | Continue with a warning when preference persistence fails after a successful locale change. | Not run |
| ELOC-T-043 | Initial locale selection | Validate locale codes case-insensitively while preserving canonical display form. | Not run |
| ELOC-T-044 | Initial locale selection | Support an explicitly configured custom locale identifier. | Not run |
| ELOC-T-045 | Initial locale selection | Distinguish pseudo locales from shipping locales. | Not run |
| ELOC-T-046 | Initial locale selection | Hide disabled or development-only locales from the player-facing locale list. | Not run |
| ELOC-T-047 | Locale change transactions | Change from the source locale to a fully available locale. | Not run |
| ELOC-T-048 | Locale change transactions | Request the already-effective locale and return an idempotent result. | Not run |
| ELOC-T-049 | Locale change transactions | Reject a locale not present in the enabled locale catalog. | Not run |
| ELOC-T-050 | Locale change transactions | Reject a pseudo locale in a release configuration. | Not run |
| ELOC-T-051 | Locale change transactions | Coalesce multiple pending requests to the latest request under the default policy. | Not run |
| ELOC-T-052 | Locale change transactions | Return Busy when the configuration selects reject-while-changing. | Not run |
| ELOC-T-053 | Locale change transactions | Cancel a request before backend loading begins. | Not run |
| ELOC-T-054 | Locale change transactions | Cancel a request during preloading before the commit point. | Not run |
| ELOC-T-055 | Locale change transactions | Return TooLate when cancellation occurs after locale publication begins. | Not run |
| ELOC-T-056 | Locale change transactions | Retain the previous locale when preparation fails. | Not run |
| ELOC-T-057 | Locale change transactions | Rollback to the previous locale when publication fails and rollback succeeds. | Not run |
| ELOC-T-058 | Locale change transactions | Enter a faulted-but-diagnosable state when publication and rollback both fail. | Not run |
| ELOC-T-059 | Locale change transactions | Raise change-started after the request is accepted. | Not run |
| ELOC-T-060 | Locale change transactions | Raise effective-locale-changed only after authoritative state changes. | Not run |
| ELOC-T-061 | Locale change transactions | Raise change-completed exactly once. | Not run |
| ELOC-T-062 | Locale change transactions | Complete public Awaitable results on the main thread. | Not run |
| ELOC-T-063 | Locale change transactions | Invalidate locale-versioned caches after a successful change. | Not run |
| ELOC-T-064 | Locale change transactions | Keep noncritical on-demand content available after critical preloads complete. | Not run |
| ELOC-T-065 | Fallback graph and missing content | Resolve a missing entry through the first configured fallback. | Not run |
| ELOC-T-066 | Fallback graph and missing content | Resolve through a multi-level fallback chain. | Not run |
| ELOC-T-067 | Fallback graph and missing content | Honor deterministic breadth-first fallback order. | Not run |
| ELOC-T-068 | Fallback graph and missing content | Do not evaluate the same locale twice in a diamond fallback graph. | Not run |
| ELOC-T-069 | Fallback graph and missing content | Detect a direct fallback cycle. | Not run |
| ELOC-T-070 | Fallback graph and missing content | Detect an indirect fallback cycle. | Not run |
| ELOC-T-071 | Fallback graph and missing content | Enforce the configured maximum fallback depth. | Not run |
| ELOC-T-072 | Fallback graph and missing content | Use the source locale when policy allows it. | Not run |
| ELOC-T-073 | Fallback graph and missing content | Return Missing without throwing when no fallback contains the entry. | Not run |
| ELOC-T-074 | Fallback graph and missing content | Return TypeMismatch when an asset entry resolves to the wrong type. | Not run |
| ELOC-T-075 | Fallback graph and missing content | Report every attempted locale in a lookup result. | Not run |
| ELOC-T-076 | Fallback graph and missing content | Apply the development missing-content marker policy. | Not run |
| ELOC-T-077 | Fallback graph and missing content | Apply the release safe-placeholder policy without exposing internal keys. | Not run |
| ELOC-T-078 | Fallback graph and missing content | Treat fallback success as a warning or success according to project policy. | Not run |
| ELOC-T-079 | String lookup and Smart formatting | Resolve a plain localized string by stable table and entry identity. | Not run |
| ELOC-T-080 | String lookup and Smart formatting | Resolve after a table display name is renamed. | Not run |
| ELOC-T-081 | String lookup and Smart formatting | Resolve after an entry display key is renamed while its stable entry ID remains. | Not run |
| ELOC-T-082 | String lookup and Smart formatting | Reject an empty table identity. | Not run |
| ELOC-T-083 | String lookup and Smart formatting | Reject an empty entry identity. | Not run |
| ELOC-T-084 | String lookup and Smart formatting | Resolve an entry through a LocalizedTextReference serialized in a project asset. | Not run |
| ELOC-T-085 | String lookup and Smart formatting | Format a numeric argument with the effective locale culture. | Not run |
| ELOC-T-086 | String lookup and Smart formatting | Format a date argument with the effective locale culture. | Not run |
| ELOC-T-087 | String lookup and Smart formatting | Format a time argument with the effective locale culture. | Not run |
| ELOC-T-088 | String lookup and Smart formatting | Format a percentage argument with the effective locale culture. | Not run |
| ELOC-T-089 | String lookup and Smart formatting | Format a currency value without performing currency conversion. | Not run |
| ELOC-T-090 | String lookup and Smart formatting | Format plural branches using Smart Strings. | Not run |
| ELOC-T-091 | String lookup and Smart formatting | Format select/conditional branches using Smart Strings. | Not run |
| ELOC-T-092 | String lookup and Smart formatting | Format a list using the configured formatter. | Not run |
| ELOC-T-093 | String lookup and Smart formatting | Reject an argument name that violates the safe argument-name policy. | Not run |
| ELOC-T-094 | String lookup and Smart formatting | Return a structured formatting failure for a missing variable. | Not run |
| ELOC-T-095 | String lookup and Smart formatting | Return a structured formatting failure for an incompatible variable type. | Not run |
| ELOC-T-096 | String lookup and Smart formatting | Exclude argument values and resolved player text from ordinary diagnostics. | Not run |
| ELOC-T-097 | Localized asset loading and leases | Load a localized Sprite through the effective locale. | Not run |
| ELOC-T-098 | Localized asset loading and leases | Load a localized AudioClip through the effective locale. | Not run |
| ELOC-T-099 | Localized asset loading and leases | Load a localized Texture through the effective locale. | Not run |
| ELOC-T-100 | Localized asset loading and leases | Load a localized Material through the effective locale. | Not run |
| ELOC-T-101 | Localized asset loading and leases | Load a localized GameObject or prefab reference without instantiating it. | Not run |
| ELOC-T-102 | Localized asset loading and leases | Load a locale-specific font asset through the configured backend. | Not run |
| ELOC-T-103 | Localized asset loading and leases | Resolve an asset through fallback. | Not run |
| ELOC-T-104 | Localized asset loading and leases | Return Missing when no asset exists in the allowed chain. | Not run |
| ELOC-T-105 | Localized asset loading and leases | Return TypeMismatch for the wrong requested asset type. | Not run |
| ELOC-T-106 | Localized asset loading and leases | Dispose a localized asset lease exactly once. | Not run |
| ELOC-T-107 | Localized asset loading and leases | Ignore repeated disposal safely. | Not run |
| ELOC-T-108 | Localized asset loading and leases | Reject a stale generational asset lease after the slot is recycled. | Not run |
| ELOC-T-109 | Localized asset loading and leases | Release all root-owned leases during shutdown. | Not run |
| ELOC-T-110 | Localized asset loading and leases | Bound outstanding asset leases and reject or warn when the configured limit is reached. | Not run |
| ELOC-T-111 | Fonts, scripts, and text direction | Resolve the configured primary font profile for a Latin-script locale. | Not run |
| ELOC-T-112 | Fonts, scripts, and text direction | Resolve the configured primary font profile for a non-Latin locale. | Not run |
| ELOC-T-113 | Fonts, scripts, and text direction | Use a configured fallback font chain. | Not run |
| ELOC-T-114 | Fonts, scripts, and text direction | Report missing glyphs from the required-character fixture. | Not run |
| ELOC-T-115 | Fonts, scripts, and text direction | Report missing glyphs from translated table content. | Not run |
| ELOC-T-116 | Fonts, scripts, and text direction | Do not mutate shared font assets during runtime validation. | Not run |
| ELOC-T-117 | Fonts, scripts, and text direction | Expose left-to-right direction metadata. | Not run |
| ELOC-T-118 | Fonts, scripts, and text direction | Expose right-to-left direction metadata. | Not run |
| ELOC-T-119 | Fonts, scripts, and text direction | Notify presentation bridges when text direction changes. | Not run |
| ELOC-T-120 | Fonts, scripts, and text direction | Do not claim to perform bidirectional shaping in the neutral core. | Not run |
| ELOC-T-121 | Fonts, scripts, and text direction | Validate that every shipping locale has a font policy. | Not run |
| ELOC-T-122 | Fonts, scripts, and text direction | Allow an explicit project provider for advanced shaping or layout mirroring. | Not run |
| ELOC-T-123 | Pseudo-localization and content QA | Enable an accented pseudo locale in development. | Not run |
| ELOC-T-124 | Pseudo-localization and content QA | Enable an expanded-text pseudo locale in development. | Not run |
| ELOC-T-125 | Pseudo-localization and content QA | Enable an encapsulated pseudo locale in development. | Not run |
| ELOC-T-126 | Pseudo-localization and content QA | Detect hard-coded strings in registered Laboratory surfaces. | Not run |
| ELOC-T-127 | Pseudo-localization and content QA | Expose text expansion and truncation symptoms in the Laboratory. | Not run |
| ELOC-T-128 | Pseudo-localization and content QA | Expose missing-glyph symptoms in the Laboratory. | Not run |
| ELOC-T-129 | Pseudo-localization and content QA | Expose right-to-left layout advisories in the Laboratory. | Not run |
| ELOC-T-130 | Pseudo-localization and content QA | Block pseudo locales from release locale selection. | Not run |
| ELOC-T-131 | Pseudo-localization and content QA | Run table completeness validation against the source locale. | Not run |
| ELOC-T-132 | Pseudo-localization and content QA | Generate a content QA report without modifying project tables. | Not run |
| ELOC-T-133 | Editor setup, import/export, and validation | Create a new EchoLocalization configuration without overwriting existing Localization Settings. | Not run |
| ELOC-T-134 | Editor setup, import/export, and validation | Link an existing Unity Localization Settings asset. | Not run |
| ELOC-T-135 | Editor setup, import/export, and validation | Create a source Locale only after dry-run approval. | Not run |
| ELOC-T-136 | Editor setup, import/export, and validation | Create a pseudo Locale only after dry-run approval. | Not run |
| ELOC-T-137 | Editor setup, import/export, and validation | Create an empty String Table Collection template. | Not run |
| ELOC-T-138 | Editor setup, import/export, and validation | Create an empty Asset Table Collection template. | Not run |
| ELOC-T-139 | Editor setup, import/export, and validation | Repeat setup without duplicating settings, locales, or collections. | Not run |
| ELOC-T-140 | Editor setup, import/export, and validation | Repair a missing root prefab reference without replacing project-owned content. | Not run |
| ELOC-T-141 | Editor setup, import/export, and validation | Validate missing source locale. | Not run |
| ELOC-T-142 | Editor setup, import/export, and validation | Validate duplicate canonical locale identifiers. | Not run |
| ELOC-T-143 | Editor setup, import/export, and validation | Validate fallback cycles and depth. | Not run |
| ELOC-T-144 | Editor setup, import/export, and validation | Validate missing source-table entries. | Not run |
| ELOC-T-145 | Editor setup, import/export, and validation | Validate shipping-locale table completeness. | Not run |
| ELOC-T-146 | Editor setup, import/export, and validation | Validate localized asset type consistency. | Not run |
| ELOC-T-147 | Editor setup, import/export, and validation | Validate font profile coverage. | Not run |
| ELOC-T-148 | Editor setup, import/export, and validation | Validate pseudo locale release exclusion. | Not run |
| ELOC-T-149 | Editor setup, import/export, and validation | Validate unsupported or mismatched Unity Localization package version. | Not run |
| ELOC-T-150 | Editor setup, import/export, and validation | Export a validation report without transmitting data. | Not run |
| ELOC-T-151 | Persistence, bridges, and integration | Initialize and change locales without The Accord installed. | Not run |
| ELOC-T-152 | Persistence, bridges, and integration | Register the Accord preference bridge and load a persisted locale. | Not run |
| ELOC-T-153 | Persistence, bridges, and integration | Unregister the Accord bridge and retain session functionality. | Not run |
| ELOC-T-154 | Persistence, bridges, and integration | Publish locale and direction changes to the Looking Glass bridge. | Not run |
| ELOC-T-155 | Persistence, bridges, and integration | Resolve dialogue text and localized voice references through a Voices bridge without owning dialogue flow. | Not run |
| ELOC-T-156 | Persistence, bridges, and integration | Provide a localized audio asset to a Resonance bridge without playing it. | Not run |
| ELOC-T-157 | Persistence, bridges, and integration | Expose startup initialization through a First Light startup-step bridge. | Not run |
| ELOC-T-158 | Persistence, bridges, and integration | Expose localization health through an Observatory provider bridge. | Not run |
| ELOC-T-159 | Persistence, bridges, and integration | Expose locale content validation through a Foundry validator bridge. | Not run |
| ELOC-T-160 | Persistence, bridges, and integration | Expose setup planning through the Workshop ADR-001 facade. | Not run |
| ELOC-T-161 | Persistence, bridges, and integration | Remove an optional bridge and confirm both peer cores compile. | Not run |
| ELOC-T-162 | Persistence, bridges, and integration | Remove EchoLocalization after bridge-first removal and preserve project-owned Unity localization tables. | Not run |
| ELOC-T-163 | Diagnostics, privacy, and failure isolation | Produce a structured status snapshot while uninitialized. | Not run |
| ELOC-T-164 | Diagnostics, privacy, and failure isolation | Produce a structured status snapshot while ready. | Not run |
| ELOC-T-165 | Diagnostics, privacy, and failure isolation | Report effective locale, source locale, selection source, backend state, and pending request count. | Not run |
| ELOC-T-166 | Diagnostics, privacy, and failure isolation | Report missing-content counters without recording resolved player text. | Not run |
| ELOC-T-167 | Diagnostics, privacy, and failure isolation | Report formatting failures without recording argument values. | Not run |
| ELOC-T-168 | Diagnostics, privacy, and failure isolation | Report asset lease counts without exposing project hierarchy paths. | Not run |
| ELOC-T-169 | Diagnostics, privacy, and failure isolation | Redact absolute filesystem paths from support exports. | Not run |
| ELOC-T-170 | Diagnostics, privacy, and failure isolation | Bound diagnostic history. | Not run |
| ELOC-T-171 | Diagnostics, privacy, and failure isolation | Prevent a diagnostics listener exception from breaking locale change completion. | Not run |
| ELOC-T-172 | Diagnostics, privacy, and failure isolation | Generate a support snapshot with stable diagnostic codes. | Not run |
| ELOC-T-173 | Performance and scalability | Switch locale with the minimum supported table fixture and record timing. | Not run |
| ELOC-T-174 | Performance and scalability | Switch locale with the advertised table-count fixture and record timing. | Not run |
| ELOC-T-175 | Performance and scalability | Resolve repeated unchanged text references without per-frame polling. | Not run |
| ELOC-T-176 | Performance and scalability | Resolve concurrent text requests within configured bounds. | Not run |
| ELOC-T-177 | Performance and scalability | Load and dispose the advertised number of localized asset leases. | Not run |
| ELOC-T-178 | Performance and scalability | Validate a large table collection without unbounded memory growth. | Not run |
| ELOC-T-179 | Performance and scalability | Confirm idle runtime performs no package polling. | Not run |
| ELOC-T-180 | Performance and scalability | Profile cache invalidation and stale-entry cleanup after repeated locale changes. | Not run |
| ELOC-T-181 | Migration, removal, and platform claims | Migrate the package configuration from the immediately previous schema fixture. | Not run |
| ELOC-T-182 | Migration, removal, and platform claims | Migrate the locale-profile schema from the immediately previous schema fixture. | Not run |
| ELOC-T-183 | Migration, removal, and platform claims | Preserve unknown configuration extension records. | Not run |
| ELOC-T-184 | Migration, removal, and platform claims | Preserve project-owned Unity Localization tables during package upgrade. | Not run |
| ELOC-T-185 | Migration, removal, and platform claims | Preserve table and entry identities across display-name changes. | Not run |
| ELOC-T-186 | Migration, removal, and platform claims | Detect unsupported newer EchoLocalization configuration without destructive downgrade. | Not run |
| ELOC-T-187 | Migration, removal, and platform claims | Remove EchoLocalization-specific metadata through an explicit conversion/removal plan. | Not run |
| ELOC-T-188 | Migration, removal, and platform claims | Reinstall and reopen supported EchoLocalization configuration. | Not run |
| ELOC-T-189 | Migration, removal, and platform claims | Run Windows compatibility fixture. | Not run |
| ELOC-T-190 | Migration, removal, and platform claims | Run macOS compatibility fixture. | Not run |
| ELOC-T-191 | Migration, removal, and platform claims | Run Linux compatibility fixture. | Not run |
| ELOC-T-192 | Migration, removal, and platform claims | Run WebGL compatibility fixture. | Not run |
| ELOC-T-193 | Migration, removal, and platform claims | Run Android compatibility fixture. | Not run |
| ELOC-T-194 | Migration, removal, and platform claims | Run iOS compatibility fixture. | Not run |
| ELOC-T-195 | Migration, removal, and platform claims | Record console support as Unknown until platform-specific evidence exists. | Not run |
| ELOC-T-196 | Migration, removal, and platform claims | Verify release documentation does not claim unexecuted platform support. | Not run |

### 23.4 Evidence rules

- Every row above is a planned test definition, not a pass.
- Each execution receives its own environment, version, commit, result, evidence, and issue references.
- Retrying a flaky test does not erase the original failure.
- Fallback success does not automatically mean translation completeness passed.
- A table import completing does not prove translation quality.
- A Player build compiling does not prove font, RTL, or locale behavior.
- Platform support remains Planned/Unknown until the required execution set passes.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership/non-ownership approved.
- [x] Unity Localization backend boundary approved.
- [x] MVP/deferred scope separated.
- [x] Locale lifecycle, fallback, references, formatting, assets, fonts, diagnostics, and Labs defined.
- [x] Optional integrations separated.
- [x] Planned evidence remains Not run.
- [x] Jesse approved the specification through the documentation-first workflow.

### 24.2 Implementation gate

- [ ] Manifest installs the approved backend version.
- [ ] Runtime/Editor assemblies follow SFGSS-002.
- [ ] Root duplicate/lifecycle behavior passes.
- [ ] Locale transactions and rollback pass.
- [ ] Stable references and migrations pass.
- [ ] Asset leases release correctly.
- [ ] Setup/repair is repeatable and non-destructive.
- [ ] Public API matches this specification or authority is revised first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Package works without peer Echo packages.
- [ ] Standalone Laboratory passes all required scenarios.
- [ ] Samples remove safely.
- [ ] Direct-scene behavior matches documentation.
- [ ] Unity native localizers/bindings can coexist with the service.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual Laboratory passes.
- [ ] No blocker/critical defect remains.
- [ ] Fallback, missing, format, asset, font, pseudo, and privacy diagnostics are actionable.
- [ ] Performance and limits are measured.
- [ ] Documentation matches the tested build.
- [ ] Current Notes is reconciled.
- [ ] Licenses/notices are complete.

### 24.5 Distribution gate

**Beta**

- [ ] Manifest/package anatomy valid.
- [ ] Clean install and Laboratory proof pass.
- [ ] Known limitations and backend version stated.
- [ ] Table/font/pseudo validators work.
- [ ] Tarball/Git install tested.

**Release candidate**

- [ ] Upgrade/migration fixture passes.
- [ ] Advertised bridges pass Integration Labs.
- [ ] Target platform matrix has required evidence.
- [ ] No pseudo locale in release profile.
- [ ] Source/shipping locale completeness gates pass.

**Stable**

- [ ] Compatibility claims are supported by evidence.
- [ ] Public API/asset GUIDs stable.
- [ ] Removal/reinstall path documented/tested.
- [ ] Repository release/tag/changelog/notices complete.
- [ ] Suite compatibility catalog updated.


## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Hard-coded menu/tutorial/password/status text | Introduce source tables and profile; migrate one screen family at a time | Original and localized screen behavior match; pseudo test passes | Keep original prefabs/text until each screen passes |
| Don't Get Vince'd | Beat-em-up UI/dialogue/system text | Start with system/UI text and later dialogue/audio | Standalone package plus one project scene | Remove bindings and restore original text references |
| Echo Systems Lab | Portfolio/system descriptions and UI labels | Localize one isolated system page/demo | English source remains intact and table references resolve | Retain source text branch |
| Hackulos | Future large RPG text/asset volume | Establish IDs/table groups/font/Dialogue bridge before content scale | One quest/NPC/item/spell vertical slice | Preserve project data and disable bridge |
| The Workshop presets | New-project setup | Offer optional Localization selection/profile templates | Dry-run and clean generated project | Remove generated Echo config; preserve project tables |

### 25.2 Preserve-until-parity rule

- Existing text/assets remain intact until the localized replacement passes in isolation and the target project.
- Migrate one content domain at a time.
- Do not bulk-delete source text before stable references and fallbacks are validated.
- Keep translation table changes in reviewable commits.
- Remove old localization scripts only after all callers and scenes are scanned.
- Project-owned tables and fonts remain outside package source.

### 25.3 Migration tooling

Planned tooling:

1. Scan scenes/prefabs/assets for registered hard-coded text fields.
2. Generate a dry-run candidate list without changing content.
3. Create or select a target table collection.
4. Add source entries with stable IDs after approval.
5. Replace eligible references one controlled batch at a time.
6. Preserve backups and operation receipts.
7. Validate missing references and source parity.
8. Provide rollback mapping.
9. Never auto-translate or rewrite complex concatenated sentences silently.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ELOC-R-001 | Package duplicates Unity Localization instead of adding policy | Medium | High | Keep official backend authoritative; reject replacement-engine scope | Any custom table/parser proposal |
| ELOC-R-002 | Locale authority races with direct backend writes | Medium | High | Document single authority; validation; backend adapter; diagnostics | Mixed-language state |
| ELOC-R-003 | Stable reference confused with display name or AssetDatabase GUID | Medium | High | Provider table/entry IDs and SFGSS-003 rules | Rename break |
| ELOC-R-004 | Fallback hides incomplete shipping translations | High | High | Separate fallback success from completeness gate | Release report |
| ELOC-R-005 | Locale change leaves mixed content | Medium | High | Critical preload, commit point, locale version, semantic invalidation | Change failure |
| ELOC-R-006 | Font lacks target glyphs | High | High | Font profiles, content scan, required fixtures | Missing glyph report |
| ELOC-R-007 | RTL declared but not usable | Medium | High | Honest metadata boundary, required provider strategy, target-language tests | RTL locale enabled |
| ELOC-R-008 | Smart String arguments leak private text | Low | High | Redacted diagnostics and bounded safe argument model | Support export |
| ELOC-R-009 | Asset handles leak | Medium | High | Generational disposable leases, bounds, shutdown cleanup | Lease count growth |
| ELOC-R-010 | Backend package update breaks tables/references | Medium | High | Exact version, upgrade guide review, fixtures, ADR for behavior change | Dependency bump |
| ELOC-R-011 | Setup/import overwrites project content | Low | Critical | Dry-run, backups, create-only defaults, no silent table edits | Apply/import |
| ELOC-R-012 | Remote TMS credentials enter core | Low | High | Provider separation and no network core | Integration request |
| ELOC-R-013 | Scope grows into UI/dialogue/audio | Medium | High | Authority matrix and bridges | New feature review |
| ELOC-R-014 | Platform language claim exceeds evidence | Medium | High | SFGSS-004 statuses and Foundry validator | Release checklist |
| ELOC-R-015 | Pseudo locale ships publicly | Low | Major | Release validator blocker | Build preflight |
| ELOC-R-016 | Translation/cultural quality treated as automated pass | Medium | High | Reports state technical coverage only | QA signoff |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| ELOC-D-001 | Use Unity Localization as the required backend | Approved | Avoid duplicate engine; use mature tables/assets/Smart Strings | Exact dependency and upgrade tests required | No |
| ELOC-D-002 | One persistent Echo locale authority | Approved | Deterministic startup/change/persistence/diagnostics | Direct backend writes are unsupported integration behavior | No |
| ELOC-D-003 | Store provider table/entry identity, not display names | Approved | Rename safety | Conversion/migration tooling required | No |
| ELOC-D-004 | Locale changes are bounded transactions with a commit point | Approved | Prevent mixed/unknown state | Cancellation becomes TooLate after publication begins | No |
| ELOC-D-005 | Preference storage is optional provider/Accord authority | Approved | Keep core standalone and settings boundary clean | Session-only without provider | No |
| ELOC-D-006 | Native Unity localizers and bindings remain valid consumers | Approved | Do not force custom presentation wrappers | Echo events support non-native consumers |
| ELOC-D-007 | Localized asset loads return owned disposable leases | Approved | Make Addressables/backend ownership explicit | Callers must dispose |
| ELOC-D-008 | Font/script/direction policy is project-owned and validated | Approved | Typography varies by project/locale | Core does not generate fonts |
| ELOC-D-009 | RTL shaping/layout is not promised by neutral MVP | Approved | Requires backend/language evidence | Provider/bridge later |
| ELOC-D-010 | Pseudo-localization is mandatory MVP tooling | Approved | Finds expansion, hard-coded, glyph, and direction risks early | Shipping profiles exclude pseudo locales |
| ELOC-D-011 | Import/export uses official Unity workflows | Approved | Avoid credential/provider scope | Echo adds validation/reports only |
| ELOC-D-012 | Diagnostics redact resolved text and arguments | Approved | Privacy and unreleased-content safety | Stable IDs/codes used instead |
| ELOC-D-013 | Planned dependency is Localization 1.5.12 | Approved as planned | Current official 1.5 release includes Unity 6 UI Toolkit/font/dropdown fixes | Must be verified in baseline before manifest release | No |

### 27.2 Release-blocking questions

No architecture question blocks documentation approval. Before implementation distribution, the following evidence questions must be answered:

| Question | Why it blocks distribution | Owner | Due before |
|---|---|---|---|
| Is `com.unity.localization` 1.5.12 available and cleanly compatible with Unity 6000.3.8f1 in the package installation routes? | Exact manifest and support claim | Jesse/implementation checkpoint | M1/M2 |
| Which font assets and character fixtures prove the first supported locales? | Font/glyph release claim | Project/package test plan | Beta |
| Which RTL languages/providers, if any, are advertised? | Avoid false shaping/layout support | Package owner | RC |
| Which platform app-info metadata is required for each advertised target? | Build/store correctness | Foundry bridge/project | RC |

### 27.3 Non-blocking later questions

- Whether translation status metadata belongs in EchoLocalization or a separate content-production extension.
- Whether a shared suite localized-reference contract becomes necessary after Voices/Objectives/Inventory specifications.
- Whether runtime remote localization content is worth the security/versioning burden.
- Whether advanced font fallback and TextCore/TMP assets should be separate presentation/provider packages.
- Whether locale-specific audio selection uses direct localized assets or a Jukebot cue-profile bridge.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | This document | Approved v1.0.0 |
| M1 - Package skeleton | Installable anatomy | Manifest, asmdefs, docs shell | Clean compile/install |
| M2 - Backend and root | Deterministic initialization | Config, backend adapter, root, initial selection | Unit/PlayMode tests |
| M3 - Text/formatting | Stable text and Smart Strings | References, args, results, fallback | Automated tests |
| M4 - Assets/fonts | Asset leases and font policy | Typed load, release, profiles, validation | Stress/font fixtures |
| M5 - Locale transactions | Runtime changes | Preload, commit, cancellation, rollback, events | Laboratory tests |
| M6 - Editor tooling | Setup/validation/pseudo/reports | Repeat-safe tools and facade | Editor tests |
| M7 - Standalone Laboratory | Complete isolated proof | 44 scenarios | Manual/automated evidence |
| M8 - First bridges | Accord/UI/Foundry or selected integration | Separate packages | Integration Labs |
| M9 - Beta release | Distribution-ready beta | Docs, migration, tarball | SFGSS-004 beta gate |
| M10 - Stable release | Supported claims | Platforms, migration, removal | RC/stable gates |

### 28.2 Checkpoint rule

Every milestone is split into SFGSS-005 Checkpoint Build Plans. When code is authorized:

- show every complete compile-ready file;
- explain path, purpose, architecture, lifecycle, important sections, alternatives, and failure behavior;
- provide exact Editor setup;
- let Jesse enter the code by default;
- stop at compile/test boundaries;
- reconcile documentation and evidence before continuing.

### 28.3 First recommended checkpoint

After SUITE-DOC-33 authorizes implementation:

> **ELOC-M1-01 - Many Tongues Package Skeleton**

It creates only manifest, assemblies, documentation shell, test shells, and package metadata. It does not implement the root, backend, tables, configuration, or samples.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.

Treat SFGSS-000 as the suite authority; SFGSS-002 as dependency/assembly
authority; SFGSS-003 as data/ID/migration authority; SFGSS-004 as
test/evidence authority; and this Many Tongues specification as the Level 2
authority for EchoLocalization.

Package: EchoLocalization
Public title: Many Tongues - Localization, Locale, and Regional Content
Specification: v1.0.0 Approved
Backend: Unity Localization, planned 1.5.12, compatibility evidence Not run
Implementation status: Not started and locked until SUITE-DOC-33
Current documentation checkpoint: SUITE-DOC-10 - Voices (`EchoDialogue`)

Preserve these rules:
1. EchoLocalization is a policy/lifecycle layer over Unity Localization, not a
   replacement localization engine.
2. It owns effective locale, locale changes, fallback/missing policy, stable
   suite-facing references/results, formatting, font/direction profiles,
   diagnostics, setup, validation, and pseudo-localization.
3. It does not own translation authorship, UI layout, dialogue flow, audio
   playback, preference storage, save files, machine translation, or RTL
   shaping.
4. Keep optional integrations behind bridges/providers.
5. Keep project strings, tables, assets, fonts, and Locale assets outside
   package source.
6. Keep every unexecuted test and compatibility claim Not run.
7. When implementation begins, show complete code and explain every step so
   Jesse can enter and understand it himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification 1.0.0; implementation nonexistent |
| Completed checkpoint | SUITE-DOC-09 - EchoLocalization specification |
| Files/assets created | Documentation only |
| Tests passed | None; planned registry only |
| Tests failed | None; not run |
| Known issues | Exact backend/platform/font/RTL evidence pending |
| Decisions added | ELOC-D-001 through ELOC-D-013 |
| Next documentation checkpoint | SUITE-DOC-10 - Voices (`EchoDialogue`) |
| First future implementation checkpoint | ELOC-M1-01 after SUITE-DOC-33 |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and plain responsibility are clear.
- [x] Unity Localization backend boundary is explicit.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is useful without swallowing UI, Dialogue, Audio, Settings, or Save.
- [x] Stable references, locale lifecycle, fallback, formatting, asset leases, fonts, and direction are specified.
- [x] Setup, validation, direct-scene, and Laboratory workflows are defined.
- [x] Diagnostics exist without Observatory and protect content privacy.
- [x] Optional integrations are separate and removable.
- [x] Test/release gates are measurable and remain Not run.
- [x] No Isekai Studios identity or dependency was introduced.
- [x] Jesse approved the package-first documentation workflow.

### 30.2 Approval record

**Decision:** APPROVED  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 4, 2026  
**Conditions or notes:** Implementation remains locked until SUITE-DOC-33. Unity Localization 1.5.12, platform behavior, fonts, RTL strategy, performance, and all test results remain Planned/Not run until implementation evidence exists.

---

## Specification Completion Statement

A new collaborator can determine:

1. EchoLocalization owns locale lifecycle, policy, localized access/results, formatting, font/direction metadata, diagnostics, and tooling.
2. Unity Localization remains the backend authority for tables, Locales, Smart Strings, assets, pseudo-locales, and import/export.
3. Translations, assets, fonts, UI, dialogue, audio, preferences, saves, and cultural approval remain outside the core.
4. Stable references use backend table/entry identity rather than mutable display names.
5. Locale changes are serialized transactions with bounded cancellation and rollback behavior.
6. Missing content, fallback, formatting, and asset failures are structured and observable.
7. The package works alone and connects to peers through explicit bridges.
8. The Standalone Laboratory proves the core without unrelated Echo packages.
9. Every compatibility and execution claim remains honest until tested.
10. The next suite documentation checkpoint is Voices (`EchoDialogue`).

The specification is therefore complete and **Approved v1.0.0**.


---

## Graph Navigation

#sfgss/package #sfgss/wave/expansion #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
