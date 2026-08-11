# ESV-M4-R4 — Chronicle 100-Case Registry Evidence Matrix

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Checkpoint:** ESV-M4-R4
**Reconciliation baseline:** `81c53dd` — `Activate ESV-M4-R4 final registry reconciliation`
**Incoming focused Chronicle floor:** `660 / 660 passed, 0 failed`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.43.0 / ESV-D-036
**Reconciliation date:** 2026-08-11

## Outcome

- **61 rows — Complete from retained direct evidence.**
- **39 rows — Deferred to an explicitly later gate.**
- **0 rows — Blocked as an unresolved M4-applicable evidence gap.**

This matrix is an evidence classification, not a claim that the package is release-qualified. A `Deferred` row remains not complete until its named later gate produces direct evidence. The fresh R4 closing Chronicle Editor rerun passed **660 / 660**, with **0 failed**; Chronicle M4 is complete.

## Row-by-row reconciliation

| Test ID | Scenario | Pre-R4 status | M4 applicability / owner | Retained evidence | R4 disposition |
|---|---|---|---|---|---|
| ESV-T-001 | Install — Clean project install → Package compiles and setup opens | Planned | M7 / clean-project release qualification | No external clean-project installation/setup-opening run has been retained yet. | **Deferred** |
| ESV-T-002 | Install — Tarball install → Package compiles with stable GUIDs | Planned | M7 / Distribution Kit | Tarball installation and stable-GUID package distribution are release-route evidence. | **Deferred** |
| ESV-T-003 | Install — Remove sample → Runtime still compiles | Planned | M5 Laboratory / packaging | Exact sample-removal compile proof is LAB-030 / package-graduation work. | **Deferred** |
| ESV-T-004 | Install — No UnityEditor reference in runtime → Assembly audit passes | Planned | M7 / player-build & assembly audit | Runtime has remained package-compiled, but the registry asks for an explicit assembly audit/player-safe qualification that has not been retained. | **Deferred** |
| ESV-T-005 | Lifecycle — One root claims → Service initializes once | Planned | Applicable — retained M1-M4/R1-R3 core proof | M1-01 — `OneConfiguredRootClaimsAuthority` + `ValidConfigurationInitializesExactlyOnce`. | **Complete** |
| ESV-T-006 | Lifecycle — Duplicate before play → Duplicate has zero side effects | Planned | Applicable — retained M1-M4/R1-R3 core proof | M1-01 — `DuplicateRootIsRejectedBeforeServiceConstruction` / duplicate zero-side-effect lifecycle proof. | **Complete** |
| ESV-T-007 | Lifecycle — Duplicate during scene load → Original remains authority | Planned | M5 Laboratory / lifecycle | Duplicate-during-scene-load is a direct-scene/lifecycle scenario, not yet a retained M1-M4 proof. | **Deferred** |
| ESV-T-008 | Lifecycle — Shutdown and recreate → Authority clears cleanly | Planned | Applicable — retained M1-M4/R1-R3 core proof | M1-01 — `ShutdownReleasesAuthorityAndLaterRootMayClaim`. | **Complete** |
| ESV-T-009 | Lifecycle — Domain reload disabled → Static state resets correctly | Planned | M5 Laboratory / lifecycle | Domain-reload-disabled static reset qualification has not yet been retained. | **Deferred** |
| ESV-T-010 | Config — Missing configuration → Blocked result ESV-CFG-001 | Planned | Applicable — retained M1-M4/R1-R3 core proof | M1-01 — `MissingConfigurationBlocksWithoutInitializationSideEffect` returns `ESV-CFG-001` before backend creation. | **Complete** |
| ESV-T-011 | Config — Unsafe path → Operation refuses ESV-PATH-001 | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-01 — storage-root/path-safety tests reject traversal/unsafe configured roots before I/O. | **Complete** |
| ESV-T-012 | Config — Invalid limits → Validator blocks | Planned | Applicable — retained M1-M4/R1-R3 core proof | R2 — `SchemaTwoRejectsInvalidActiveCapacity` + `InvalidSchemaTwoPolicyBlocksBeforeStorageFactorySideEffects`. | **Complete** |
| ESV-T-013 | Config — Missing serializer → Initialization blocks | Planned | M5 CAP-020 Setup/Validator | Current runtime owns package defaults; configured missing-serializer initialization validation belongs to later authoring/setup qualification. | **Deferred** |
| ESV-T-014 | Config — Missing backend → Initialization blocks | Planned | M5 CAP-020 Setup/Validator | Configured missing-backend initialization validation belongs to later provider/setup qualification. | **Deferred** |
| ESV-T-015 | Slots — Create single slot → Stable ID and metadata created | Complete | Applicable — retained M1-M4/R1-R3 core proof | M4-02 + R2 — real initial slot publication and SingleSlot effective-capacity proof. | **Complete** |
| ESV-T-016 | Slots — Fixed slot capacity → Extra slot rejected | Complete | Applicable — retained M1-M4/R1-R3 core proof | M4-02 + R2 — fixed finite capacity rejection proof. | **Complete** |
| ESV-T-017 | Slots — Configurable capacity → Configured limit enforced | Complete | Applicable — retained M1-M4/R1-R3 core proof | R2 — configurable effective capacity enforced by service creation path. | **Complete** |
| ESV-T-018 | Slots — Unlimited policy safety cap → Platform/config cap enforced | Complete | Applicable — retained M1-M4/R1-R3 core proof | R2 — BoundedProfiles resolves to finite safety limit; over-capacity creation rejects. | **Complete** |
| ESV-T-019 | Slots — Rename slot → ID/path unchanged | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-09 — rename preserves canonical slot ID/path while publishing metadata generation. | **Complete** |
| ESV-T-020 | Slots — Duplicate slot → New ID and equivalent payload | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-09 — duplicate creates fresh slot/generation identity with equivalent verified state. | **Complete** |
| ESV-T-021 | Slots — Delete without plan → No mutation | Complete | Applicable — retained M1-M4/R1-R3 core proof | M4-10 — prepare-delete is read-only; no confirmation means no mutation. | **Complete** |
| ESV-T-022 | Slots — Expired delete plan → Rejected | Complete | Applicable — retained M1-M4/R1-R3 core proof | M4-10 — expired/invalid deletion plan rejects before durable mutation. | **Complete** |
| ESV-T-023 | Slots — Confirm delete → Trash/delete policy applied | Complete | Applicable — retained M1-M4/R1-R3 core proof | M4-10 — confirmed deletion publishes recoverable trash and reconciles live catalog. | **Complete** |
| ESV-T-024 | Catalog — List slots → Payload files unopened | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-01 — catalog refresh reconstructs head/current-manifest metadata with zero payload reads. | **Complete** |
| ESV-T-025 | Catalog — Corrupt cache → Rebuild succeeds | Planned | M5 persistent catalog cache | `catalog.cache.json` optimization is intentionally deferred; no corrupt-cache rebuild can be complete before the cache exists. | **Deferred** |
| ESV-T-026 | Catalog — Missing cache → Rebuild succeeds | Planned | M5 persistent catalog cache | Missing-cache rebuild is intentionally deferred with the persistent cache feature. | **Deferred** |
| ESV-T-027 | Participants — Register unique participants → Deterministic registry | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-01 — deterministic unique participant registration/snapshot proof. | **Complete** |
| ESV-T-028 | Participants — Duplicate participant ID → Later registration rejected | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-01 — duplicate canonical/alias ownership collisions reject. | **Complete** |
| ESV-T-029 | Participants — Required capture success → Payload entry written | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-02/M3-03 — successful required capture becomes validated published payload entry. | **Complete** |
| ESV-T-030 | Participants — Required capture failure → No generation published | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-02/M3-03 — capture-batch failure exposes no publishable partial batch and performs no generation mutation. | **Complete** |
| ESV-T-031 | Participants — Optional capture default failure → Save fails visibly | Planned | M5 Laboratory LAB-014 | Exact optional-capture default-failure scenario is explicitly reserved for LAB-014. | **Deferred** |
| ESV-T-032 | Participants — Missing required apply participant → Prepared load remains/reports | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-09 — missing required apply owner fails preflight before participant mutation. | **Complete** |
| ESV-T-033 | Participants — Missing payload initialize default → Participant default policy runs | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-09 — explicit `InitializeDefault` missing-payload action executes through optional capability. | **Complete** |
| ESV-T-034 | Participants — Apply failure → Detailed partial/rollback report | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-09/R1 — apply failure reports ordered partial truth after mutation begins. | **Complete** |
| ESV-T-035 | Participants — Out-of-order unregister → Registry remains correct | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-01 — ownership-token/idempotent unregister behavior preserves registry correctness. | **Complete** |
| ESV-T-036 | Unknown — Removed optional participant → Opaque payload preserved | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-04/M3-05 — unclaimed payload preserved opaquely and carried forward byte-for-byte. | **Complete** |
| ESV-T-037 | Unknown — Reinstalled participant → Preserved payload applies | Planned | M5 Laboratory / adoption fixture | Core unknown preservation/preparation mechanisms are proven, but the exact remove-then-reinstall end-to-end scenario has no retained direct fixture. | **Deferred** |
| ESV-T-038 | Unknown — Explicit prune plan → Only selected payload removed | Planned | M5 Laboratory LAB-016 | Explicit prune authority/UI is intentionally deferred. | **Deferred** |
| ESV-T-039 | Unknown — Oversized unknown payload → Rejected/quarantined | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-04 — bounded unknown count/aggregate-byte safeguards reject over-bound unknown state without replacing prior valid store. | **Complete** |
| ESV-T-040 | Save — Basic save → Generation verifies and head advances | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-03 — selected healthy slot save publishes verified participant generation and advances head last. | **Complete** |
| ESV-T-041 | Save — Second save → Prior generation retained | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-04/M3-03/M4-03 — subsequent publication preserves previous known-good generation/head lineage. | **Complete** |
| ESV-T-042 | Save — Rapid manual saves → Busy/reject policy enforced | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-04 — overlapping manual save is immediate Busy with no hidden queue. | **Complete** |
| ESV-T-043 | Autosave — Rapid autosaves → One pending latest request | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-05 — rapid autosave requests coalesce to at most one latest pending request. | **Complete** |
| ESV-T-044 | Autosave — Retention rotation → Bounds enforced after commit | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-06 — post-publication retention preserves current/predecessor and removes only oldest excess verified history. | **Complete** |
| ESV-T-045 | Save — Permission denied → No capture/write | Planned | M5 fault injection / provider qualification | Exact permission-denied public-save scenario with pre-capture/no-write proof remains a backend-fault qualification case. | **Deferred** |
| ESV-T-046 | Save — Cancel queued → No side effects | Planned | M5 Laboratory LAB-026 | Generic queued cancellation is not the current manual-save model; manual saves are Busy/no-queue and LAB-026 owns the later queue scenario. | **Deferred** |
| ESV-T-047 | Save — Cancel pre-publication → Head unchanged | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-04 — safe pre-publication cancellation leaves current head unchanged. | **Complete** |
| ESV-T-048 | Save — Cancel during head publication → Operation settles/TooLate | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-04 — cancellation after durable publication begins settles truthfully as TooLate/committed outcome. | **Complete** |
| ESV-T-049 | Save — File lock → Current generation survives | Planned | M5 Laboratory LAB-025 | Exact locked-backend operation is explicitly a Laboratory fault-injection scenario. | **Deferred** |
| ESV-T-050 | Save — Out of space simulation → Current generation survives | Planned | M5 fault injection | Exact out-of-space public-save simulation has not yet been retained. | **Deferred** |
| ESV-T-051 | Commit — Crash before generation complete → Incomplete ignored | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-04/M3-03 — injected pre-publication generation failures preserve prior current truth; incomplete candidate never becomes current. | **Complete** |
| ESV-T-052 | Commit — Crash after generation verify before head → Old head remains; orphan recoverable | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-04/M3-03 — verified generation followed by head-publication failure leaves prior head current and new generation orphaned/non-current. | **Complete** |
| ESV-T-053 | Commit — Crash during head fallback update → Recovery scan chooses valid generation | Planned | M5 fault injection | Recovery planning is proven, but the exact crash during local head fallback/update path is a later fault-injection scenario. | **Deferred** |
| ESV-T-054 | Commit — Crash after head publish before cache → Head authoritative; cache rebuilds | Planned | M5 persistent catalog cache | Head-authoritative/cache-rebuild behavior depends on the intentionally deferred persistent cache. | **Deferred** |
| ESV-T-055 | Load — Prepare valid generation → Handle created | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-08/R1 — valid prepared-load creation yields bounded live handle with exact source provenance. | **Complete** |
| ESV-T-056 | Load — Dispose prepared handle → No apply/disk mutation | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-08/R1 — disposing prepared handle invalidates memory authority without apply or disk mutation. | **Complete** |
| ESV-T-057 | Load — Prepared handle expiry → Apply rejected | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-08 — deterministic prepared-handle expiry rejects use after lifetime bound. | **Complete** |
| ESV-T-058 | Load — Load and apply same scene → State restored | Planned | Applicable — retained M1-M4/R1-R3 core proof | R1 — `ConvenienceLoadPreparesAndAppliesInCurrentScene` restores saved participant state. | **Complete** |
| ESV-T-059 | Load — Prepare then simulated scene change → Apply after participant registration | Planned | M6 Passage / scene-flow integration | Prepare-then-scene-change/apply is cross-scene coordination owned by later integration. | **Deferred** |
| ESV-T-060 | Load — Wrong slot/generation identity → Validation rejects | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-08/R1 — prepared handles are source/owner/session bound; foreign/stale identity rejects before mutation. | **Complete** |
| ESV-T-061 | Integrity — Truncated manifest → Corrupt status | Planned | M5 Laboratory corruption fixtures | Exact truncated-manifest fixture has not yet been retained as a completed Laboratory case. | **Deferred** |
| ESV-T-062 | Integrity — Truncated payload → Corrupt status | Planned | M5 Laboratory LAB-017 | Exact truncated-payload corruption simulation is reserved for the Laboratory. | **Deferred** |
| ESV-T-063 | Integrity — Checksum mismatch → Recovery plan | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-07 — checksum-invalid candidate is excluded and recovery planning preserves valid fallback truth. | **Complete** |
| ESV-T-064 | Integrity — Manifest/payload entry mismatch → Rejected | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-04/M4-07 — manifest/payload inventory disagreement rejects generation as invalid. | **Complete** |
| ESV-T-065 | Integrity — Oversized manifest → Rejected before large allocation | Planned | M5 Laboratory / bounded corruption fixture | Exact oversized-manifest pre-allocation fixture remains later qualification. | **Deferred** |
| ESV-T-066 | Integrity — Oversized payload → Rejected before apply | Planned | M5 Laboratory LAB-024 | Exact oversized-payload pre-apply fixture is explicitly reserved for LAB-024. | **Deferred** |
| ESV-T-067 | Migration — Current version → No migration | Complete | Applicable — retained M1-M4/R1-R3 core proof | R3 — exact-current package document bypasses migration. | **Complete** |
| ESV-T-068 | Migration — Contiguous document chain → Migrates in memory | Complete | Applicable — retained M1-M4/R1-R3 core proof | R3 — deterministic contiguous historical document chain migrates detached text in memory. | **Complete** |
| ESV-T-069 | Migration — Missing document step → Blocks source unchanged | Complete | Applicable — retained M1-M4/R1-R3 core proof | R3 — missing package-document edge fails closed with source unchanged. | **Complete** |
| ESV-T-070 | Migration — Participant chain → Payload reaches current version | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-07 — participant migration executes exact contiguous chain to current schema. | **Complete** |
| ESV-T-071 | Migration — Participant alias ID → Old ID maps safely | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-07 — participant prior-ID alias resolves safely to current canonical owner during migration. | **Complete** |
| ESV-T-072 | Migration — Migration throws/fails → Prepare fails source unchanged | Complete | Applicable — retained M1-M4/R1-R3 core proof | R3/M3-07 — migration failure/throw converts to bounded failure and never rewrites source generation. | **Complete** |
| ESV-T-073 | Migration — Newer major format → Refused preserved | Complete | Applicable — retained M1-M4/R1-R3 core proof | R3 — unsupported newer package-document version is refused and preserved. | **Complete** |
| ESV-T-074 | Recovery — Missing head valid generations → Plan selects newest valid | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-07 — missing/broken head with valid retained generations selects newest verified candidate. | **Complete** |
| ESV-T-075 | Recovery — Current corrupt prior valid → Prior candidate offered | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-07 — corrupt current generation exposes prior valid recovery candidate. | **Complete** |
| ESV-T-076 | Recovery — Multiple valid candidates → Deterministic order | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-07 — multiple verified candidates are ordered deterministically newest-valid first. | **Complete** |
| ESV-T-077 | Recovery — No candidate → Files preserved | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-07 — no valid recovery candidate preserves source evidence and performs zero mutation. | **Complete** |
| ESV-T-078 | Recovery — Execute plan → Head/catalog update atomically/fallback safely | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-08 — recovery execution revalidates plan/candidate then republishes head only and reconciles catalog truth. | **Complete** |
| ESV-T-079 | Recovery — Stale recovery plan → Rejected | Planned | Applicable — retained M1-M4/R1-R3 core proof | M4-08 — stale recovery plan/source provenance rejects before head mutation. | **Complete** |
| ESV-T-080 | Serializer — Unity JSON plain DTO → Round trip | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-02 — Unity JSON plain serializable DTO/envelope round-trip tests. | **Complete** |
| ESV-T-081 | Serializer — Unsupported DTO shape → Actionable failure | Planned | M5 CAP-020 Validator / serializer guidance | Default serializer limitations are documented, but the registry asks for an explicit unsupported-shape actionable validator failure. | **Deferred** |
| ESV-T-082 | Serializer — Custom provider → Provider selected by ID | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-02/M3-06 — registered alternate serializer provider is selected by stable provider ID. | **Complete** |
| ESV-T-083 | Serializer — Provider missing on load → Structured failure | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-06 — `MissingSerializerProviderFailsClosed` returns structured serializer-unavailable failure. | **Complete** |
| ESV-T-084 | Security — Path traversal ID → Rejected | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-01 — relative storage-key/path traversal attempts reject. | **Complete** |
| ESV-T-085 | Security — Absolute external path → Rejected by default | Planned | Applicable — retained M1-M4/R1-R3 core proof | M2-01 — configured storage root remains contained below approved persistent-data child; external/absolute escape rejects. | **Complete** |
| ESV-T-086 | Security — Unknown type name in file → No arbitrary type activation | Planned | Applicable — retained M1-M4/R1-R3 core proof | M3-02/M3-06 — saved data carries no CLR activation authority; runtime `Type` comes only from trusted live registration. | **Complete** |
| ESV-T-087 | Privacy — Redacted snapshot → No payload/full path/display name | Planned | M5 diagnostics/support tooling | Redacted support snapshot/export privacy proof belongs to later Observatory/support tooling qualification. | **Deferred** |
| ESV-T-088 | Performance — Idle root → No Update/allocations | Planned | M5/M7 performance qualification | Idle Update/allocation measurement has not been run as a retained performance gate. | **Deferred** |
| ESV-T-089 | Performance — 32-slot catalog → Manifest-only async refresh | Planned | M5/M7 performance qualification | Payload-free catalog behavior is proven, but the registry asks for measured 32-slot async performance qualification. | **Deferred** |
| ESV-T-090 | Performance — 50 participants 5MB → Budgets measured/reportable | Planned | M5/M7 performance qualification | 50-participant / 5 MB budget measurements remain performance work. | **Deferred** |
| ESV-T-091 | Stress — 100 sequential saves → Retention/disk bounded | Planned | M5/M7 stress qualification | 100 sequential-save endurance/retention/disk measurement has not been run. | **Deferred** |
| ESV-T-092 | Stress — Queue flood → Capacity enforced | Planned | M5/M7 stress qualification | Queue/admission flood stress measurement remains later qualification. | **Deferred** |
| ESV-T-093 | Stress — Prepared-load flood → Count/bytes cap enforced | Planned | M5/M7 stress qualification | Prepared-load count/byte caps are unit-proven, but flood/stress qualification remains later. | **Deferred** |
| ESV-T-094 | Direct scene — Development initializer → One sandbox root | Planned | M5 Standalone Save Laboratory | Development direct-scene initializer/sandbox authority is a Laboratory gate. | **Deferred** |
| ESV-T-095 | Direct scene — Production root already exists → No duplicate | Planned | M5 Standalone Save Laboratory | Production-root-already-exists direct-scene duplicate proof is a Laboratory gate. | **Deferred** |
| ESV-T-096 | Integration — First Light absent/present → Both paths work | Planned | M6 peer integration | First Light absent/present compatibility is later integration evidence. | **Deferred** |
| ESV-T-097 | Integration — Looking Glass bridge removed → Core compiles/operates | Planned | M6 peer integration | Looking Glass bridge absence/removal is later optional-integration evidence. | **Deferred** |
| ESV-T-098 | Integration — Passage coordination failure → Prepared handle retry/dispose | Planned | M6 Passage integration | Passage coordination failure/retry-dispose is later scene-flow integration evidence. | **Deferred** |
| ESV-T-099 | Migration adoption — Existing project parallel run → Old system remains rollback | Planned | M6/M7 migration adoption | Existing-project parallel-run/rollback is adoption qualification, not M4 runtime proof. | **Deferred** |
| ESV-T-100 | Release — External clean install and sample checklist → Pass | Planned | M7 release qualification | External clean install plus sample checklist is the release gate. | **Deferred** |

## Gate summary

The 39 Deferred rows are not hidden failures. They are deliberately owned by later package-graduation work: M5 Setup/Validator and Save Laboratory qualification, persistent catalog-cache work, fault-injection fixtures, M6 scene/peer integration and adoption, or M7 clean-project/distribution/performance/stress/release qualification.

No M4-applicable row was found without retained direct evidence. Therefore R4 does **not** open a repair checkpoint.

## Remaining R4 close condition

Final gate satisfied: the focused `EchoDevGames.EchoSave.Tests.Editor` suite passed **660 / 660**, with **0 failed**, from the reconciled documentation state. Chronicle M4 is complete. M5 is eligible for separate activation but is not automatically active.
