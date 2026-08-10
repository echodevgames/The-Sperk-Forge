# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 9, 2026
**Current focus:** ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation
**Current checkpoint:** ESV-M3-05 — The Chronicle (`EchoSave`) — **Active / authorized**

> Durable First Light decisions live in SFGSS-PKG-ECHOLAUNCH-001 v1.16.0 and the committed checkpoint/amendment records. Git history preserves the longer working trail.

---

## Current State

- First Light package version remains `0.1.0`.
- First Light package specification remains **SFGSS-PKG-ECHOLAUNCH-001 v1.16.0**.
- FL-M5-07 Standalone Test Laboratory is complete at `710aec3` with retained automated evidence `809 / 809` and manual Laboratory evidence `12 / 12`.
- FL-M6-01 Production Reference Showcase implementation and in-repository consumer proof are complete.
- A1 splash presentation/authoring is committed across `1b7ab84`, `d36b5cc`, `90e038c`, `9b24121`, and `4bdc264`.
- Slice E Setup creation-time splash authoring is committed at `9e6df00`.
- A1-E1 project-owned foundation resolution is authorized at `a70e478` and implemented at `e66b9fd`.
- The permanent First Light Gallery is committed at `ccb1d59`; obsolete pre-gallery folder metadata is removed at `ad12b27`.
- The final `EchoLaunchSetup` filtered EditMode gate passed **224 / 224**.
- No post-A1 full-suite aggregate is claimed by this closeout. The retained FL-M5-07 full-suite baseline remains historical evidence and should be rerun at the next release-qualification gate.
- SUITE-DIST-001 is complete at `c18eff6`; First Light `0.1.0` has a repository-owned Distribution Kit and Complete User Handout.
- First Light remains frozen for this pass.
- Jesse selected The Chronicle (`EchoSave`) as the next active package learning review.
- The next multi-package initiative is the **Game Shell / Front Door**: Chronicle → Accord → Resonance → Looking Glass, composed later with First Light while preserving standalone package ownership.

## Active Suite Architecture — Persistence and Lifetime Separation

Jesse approved SFGSS-ADR-006 and the suite-wide rule:

> Durable persistence, runtime state, and Unity object lifetime are separate concerns. Packages may expose persistence-capable state without depending directly on EchoSave. Cross-package persistence integration belongs in optional bridges/adapters. Long-lived Unity service composition is project-owned and must not turn First Light, The Chronicle, or another package into a universal service locator.

Immediate consequences:

- Chronicle owns durable **game-save transport**, slots/generations, migration/recovery, and save orchestration.
- Accord remains the authority for global preferences; Chronicle does not absorb graphics/audio settings merely because those values persist.
- Inventory, Progression, Objectives, Characters, World, and future peers retain their own live runtime truth and durable payload meaning.
- A Chronicle load restores participant-owned live state; Chronicle does not become that state after load.
- `DontDestroyOnLoad` / scene-surviving object lifetime is not durable persistence.
- Consumer projects may compose long-lived package services beneath a project-owned runtime root. That root does not become a new authority domain or service locator.
- First Light may initialize/discover long-lived services, then hands off.
- Chronicle may have a duplicate-safe package-local root, but never becomes the parent/locator for unrelated services.

SFGSS-000 v0.26.0, SFGSS-001 v1.5.0, SFGSS-ADR-006, SFGSS-INT-SUITE-001 v1.1.0, and Chronicle specification v1.2.0 carry this rule.

## Chronicle Learning Gate

- Review: `PKG-LEARN-009_EchoSave_Learning_Review.md`
- Status: **Complete**
- Implementation authorization: **ESV-M1-01 active / authorized**
- First implementation checkpoint scaffold: `ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim`
- Jesse completed the Chronicle teach-back and explicitly activated implementation on 2026-08-09.
- The completed teach-back covered runtime truth versus durable snapshots, scopes/policies, participant ownership, migrations, transactional candidate safety, required/optional participation, known-good generations, duplicate-safe authority, shutdown, operation gating, coherent capture, coordinated restore/rollback, dirty state versus save policy, and separation of save model/serialization/storage.
- Serializer/file-format implementation choices remain intentionally deferred because ESV-M1-01 proves lifecycle and duplicate safety without real durable storage.

## Chronicle ESV-M1-01 Closeout

- Learning/activation commit: `5b05d9d`.
- Chronicle package implementation commit: `ecfa922`.
- Embedded Package Manager resolution commit: `2c70b1d`.
- Package ID/version: `com.echodevgames.echo-save` `0.1.0`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate: **all green**; exact numeric count not captured, so no count is claimed.
- Apply-time M1 guards passed: no `System.IO` implementation, no `Application.persistentDataPath` resolution, no Chronicle-owned `DontDestroyOnLoad`, and no peer Echo runtime reference.
- ESV-M1-01 proved package-local authority, duplicate rejection before service construction/initialization, explicit initialize/shutdown, re-claim after shutdown, configuration blocking, stable provider IDs, and zero durable-storage side effects.

## Chronicle ESV-M2-01 Closeout

- Implementation commit: `e4ef76c`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor`: **40 / 40 passed, 0 failed**.
- First focused run was `29 / 40`; all 11 failures were isolated to EditMode `EchoSaveRoot` activation assumptions. The storage/path/backend tests were already green.
- A narrow internal `EnsureAuthorityClaimedForTesting()` seam was added so direct EditMode component construction deterministically exercises the exact production `Awake()` authority path when Unity has not invoked it.
- Final gate proves safe storage keys, traversal/root rejection, sandbox root resolution, default local backend initialization, exact-byte round trips, create-only conflict preservation, structured not-found/failure behavior, duplicate-before-storage behavior, and retained M1 lifecycle rules.
- No save document, serializer payload, slot, immutable generation publication, participant, recovery/autosave, peer bridge, or Chronicle-owned DDOL behavior was introduced.

## Chronicle ESV-M3-04 Closeout

- Implementation commit: `aa78e07`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor`: **218 / 218 passed, 0 failed**.
- The complete prior **197 / 197** Chronicle regression floor remained green.
- M3-04 added a read-only current-generation reader that resolves `head.json`, reads the selected immutable manifest/payload, and validates document identity/integrity before classification.
- Whole payload integrity and per-entry inline UTF-8 byte length/checksum validation remain mandatory.
- Canonical participant IDs and approved aliases are recognized against the live participant registry.
- Unclaimed entries are preserved as inert opaque session data in package-owned `SaveUnknownPayloadStore`.
- Unknown entry fields and serialized payload text are preserved without serializer resolution, CLR type activation, migration, capture, or apply.
- Unknown-store snapshots are defensive copies and deterministic.
- Unknown entry count and aggregate-byte safeguards are enforced.
- Failed reads/classification preserve the prior valid unknown store atomically.
- Current-generation inspection performs zero storage mutation.

## Active Chronicle M3 Slice

`ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation` is active / authorized.

Authorized next:
- bind unknown-payload snapshots to the exact source slot/generation that produced them;
- atomically preserve/clear source provenance with the unknown store;
- require the target slot's current head to still equal the snapshot source generation before any carry-forward mutation;
- fail closed on stale source snapshots;
- accept one successful fresh known participant capture batch plus one opaque unknown snapshot;
- re-resolve preserved unknown IDs against the current participant registry at merge time;
- treat canonical or alias ownership acquired since the read as a stale ownership collision;
- use **no implicit winner** for fresh-known versus preserved-unknown identity collisions;
- preserve unknown participant serialized payload UTF-8 bytes and transport metadata exactly;
- keep unknown payloads opaque: no serializer resolution, deserialization, CLR activation, migration, or participant invocation;
- reconstruct matching unknown inventory metadata only from preserved transport fields;
- produce one deterministic merged transport batch ordered by persisted participant ID;
- publish the complete merged batch through the established candidate/read-back/immutable-generation/published-reverify/`head.json` LAST transaction;
- preserve M3-03 failure semantics and the existing M2/M3 publication paths;
- leave the old unknown snapshot intentionally stale after a successful head advance until a fresh M3-04 read/classification occurs;
- preserve all prior **218 / 218** Chronicle regressions.

Still deferred:
- silent drop/prune policy;
- automatic fresh-wins or old-wins ownership resolution;
- participant deserialization/migration/apply;
- prepared/convenience loads;
- production `SaveAsync` admission/permission/busy/coalescing/cancellation;
- concurrent save-operation ownership;
- slot catalog/policy and active-slot service;
- recovery/retention/autosave;
- peer bridges and project-wide DDOL composition.

### Carry-forward safety rule

Chronicle must never delete opaque durable data merely because the original package is absent, and it must never guess when ownership becomes ambiguous.

If the preserved source generation is stale or an unknown identity is now claimed by a live participant/alias, carry-forward aborts before publication mutation and reports the conflict.

A successful carry-forward preserves the unknown participant payload body's UTF-8 bytes and transport metadata exactly; the surrounding generation document is new and therefore receives a new generation identity and whole-document checksum.


## Suite Distribution Kit Standard

Jesse approved one new suite-wide graduation rule after First Light FL-M6-01 closeout:

> Every independently distributed Sperk's Forge package gets a repository-owned, versioned Distribution Kit containing the exact package artifact plus a complete user handout, distribution manifest, SHA-256 integrity record, and build record.

The preferred repository layout is:

```text
Distributions/
├── README.md
├── _Template/
│   ├── COMPLETE_USER_HANDOUT_TEMPLATE.md
│   └── DISTRIBUTION_MANIFEST_TEMPLATE.md
└── <Public Title>/
    └── <Package Version>/
        ├── README.md
        ├── <package-artifact>.tgz
        ├── <COMPLETE_USER_HANDOUT>.md
        ├── DISTRIBUTION_MANIFEST.md
        ├── DISTRIBUTION_BUILD_RECORD.txt
        └── SHA256SUMS.txt
```

Authority is promoted into SFGSS-000 v0.25.0, SFGSS-001 v1.4.0, and SFGSS-004 v1.4.0.

**Honesty rule:** creating a tarball and kit does not prove the tarball route. The artifact is now available for distribution/evaluation, while external clean-project tarball installation, removal/reinstall, player builds, performance, tags/catalog, and release/private-beta qualification remain future evidence gates.

First Light is the first package to receive the standard kit at:

```text
Distributions/First Light/0.1.0/
```

The complete First Light handout is also included in package documentation at:

```text
Packages/com.echodevgames.echo-launch/Documentation~/User/Complete User Handout.md
```

## Permanent First Light Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
│   └── First Light Splashs/
│       ├── Art/
│       ├── Audio/
│       ├── Configuration/
│       ├── Prefabs/
│       └── Scenes/
└── UMBRA Example/
    └── UMBRA Splashs/
        ├── Art/
        ├── Audio/
        ├── Configuration/
        ├── Prefabs/
        └── Scenes/
```

The Gallery is project-owned consumer/showcase content. It is not package content, a UPM sample, a required dependency, or evidence that The Sperk’s Forge is an Isekai Studios product.

### First Light Example

The canonical in-house example demonstrates the normal First Light happy path:

```text
FirstLight_Showcase_Boot
→ EchoDevGames splash
→ First Light splash
→ startup settles
→ destination validates and loads
→ FirstLight_Showcase_MainMenu
```

It uses normal public Setup, project-owned configuration, normal Inspector authoring, stable splash identities, optional stored audio intent, and the public uGUI presentation path.

### UMBRA Example

The second Gallery example is the former Slice E proof promoted into a permanent consumer example. It proves the package is not secretly tied to the canonical First Light branding or foundation.

Observed proof:

- `Foundation > Asset Resolution = Create Project-Owned Setup`;
- fresh requested Configuration, LaunchDestination, SplashSequence, StartupSequence, RootPrefab, and Boot scene planned as `Create`;
- explicitly selected existing destination scene remained `Reuse`;
- first Apply created the requested project-owned foundation;
- generated SplashSequence retained **3** authored entries: `The Sperk`, `Isekai Studios`, and `UMBRA`;
- each generated entry received a non-empty stable ID;
- project-owned images, optional `PreferredAudioClip` intent, timings, motion, and advancement settings serialized correctly;
- presentation used Splash Only, black background, and advancement enabled;
- the Isekai entry retained Pulse (`1.05` maximum scale, `2.5` second cycle);
- the generated Boot experience played successfully and looked correct;
- identical second Preview reused the requested local targets;
- identical second Apply returned **`NoChanges`** with **Created paths: None** and unchanged Build Settings evidence.

Audio remains intent metadata only. First Light does not own audio playback.

## FL-M6-01 Defects Resolved by the Real Consumer Workflow

### H1 — Splash Entry Authoring Identity

Normal Inspector authoring originally left hidden blank `SplashEntry.entryId` values and runtime correctly blocked with `ELAUNCH-SPLASH-001`.

Resolved result:

- Editor-only blank-ID generation;
- existing non-empty IDs preserved;
- no Runtime rewrite;
- no schema bump;
- focused gate **5 / 5**;
- public Inspector workflow advanced beyond `ELAUNCH-SPLASH-001`.

### H2 — Destination Build Settings Conformance

Setup originally reported success while the configured destination was absent from Build Settings and runtime correctly blocked with `ELAUNCH-DEST-001`.

Resolved result:

- Setup ensures Boot and configured destination are enabled exactly once;
- unrelated Build Settings order remains preserved;
- repeat Apply settles `NoChanges`;
- focused H2 gate **35 / 35**;
- public Boot → splashes → destination path succeeded without manual Build Settings editing.

### A1 / A1-E1 — Presentation and Independent Setup Authoring

The Showcase then justified and proved:

- Splash Only / Splash + Status;
- project-owned background color;
- Allow Advancement;
- None / Pulse motion;
- Automatic / Skippable After Minimum / Wait For Input After Minimum;
- normal Inspector authoring;
- Setup creation-time authoring for newly-created sequences;
- backward-compatible `Reuse Compatible Assets`;
- explicit `Create Project-Owned Setup` for an independent requested foundation;
- request/plan freshness participation and repeat-safe convergence.

Schemas remain unchanged: SplashSequence schema `1`, EchoLaunchConfiguration schema `4`.

## Acceptance

FL-M6-01 acceptance is complete for the in-repository Package Reference Showcase stage.

```text
Learning / authority
→ implementation
→ Standalone Test Lab                PASS
→ Package Reference Showcase         PASS
→ clean-project reproduction         NOT RUN in this closeout
→ release qualification              NOT RUN
→ private beta / external adoption   NOT RUN
```

The permanent Gallery now supplies two consumer examples that can be extended later without widening First Light package authority.

## Evidence Boundary

This closeout does **not** claim:

- a post-A1 complete EditMode aggregate;
- a post-A1 complete Runtime Play Mode aggregate;
- clean-project reproduction;
- Git URL, tag, registry, or public-package installation support;
- clean-project **tarball installation support** (the versioned `0.1.0` tarball artifact is now prepared, but the route is not yet qualified);
- player-build qualification;
- performance qualification;
- release tag/catalog readiness;
- private beta or external adoption;
- First Light-owned audio playback.

Those remain future release-qualification work if/when First Light returns to the release queue.

## Stop Point

**First Light implementation and in-repository Gallery work are frozen for this pass.**

Do not begin FL-M6-02 automatically. Do not add more First Light features merely because the Gallery can host more examples.

## Next Action

1. Rehydrate the exact repository/Unity baseline after the ESV-M3-04 closeout.
2. Implement only `ESV-M3-05`: source-provenance/freshness, collision-safe fresh+unknown merge, and opaque unknown carry-forward through the existing immutable-generation/head-last transaction.
3. Extend the unknown store/snapshot so successful M3-04 classification records the exact source `SaveSlotId` and `SaveGenerationId`; failed replacement preserves entries and provenance together.
4. Before any publication mutation, require the target current head to still equal the unknown snapshot's source generation.
5. Re-resolve preserved unknown IDs against the live registry; canonical/alias ownership changes fail closed rather than silently overwriting/dropping data.
6. Preserve unknown `serializedPayload` UTF-8 bytes and all entry metadata exactly.
7. Keep unknown payloads opaque: no serializer resolution, deserialization, CLR type activation, migration, capture, or apply.
8. Publish only a completely validated deterministic merged batch; preserve candidate/final verification and `head.json` LAST.
9. After successful head advance, treat the old unknown snapshot as stale until a new current-generation read/classification refreshes it.
10. Do **not** activate prune policy, prepared loads, participant apply, production save admission/coalescing/cancellation, concurrent save ownership, slots, recovery/retention/autosave, peer bridges, or project-wide DDOL.
11. Preserve the complete `218 / 218` Chronicle regression floor.
