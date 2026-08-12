# ESV-M5-06 — Chronicle Minimal Direct-Scene Save Laboratory — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-06
**Status:** COMPLETE
**Planning baseline:** `868b17f` — `Close out ESV-M5-05 unknown prune and catalog cache prerequisites`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.55.0 / ESV-D-042
**Incoming focused Chronicle floor:** **753 / 753 passed, 0 failed**
**Unity baseline:** 6000.3.8f1

## 1. Objective

Build the smallest useful direct-scene human verification harness for Chronicle. The Laboratory must make real runtime behavior visible without becoming a save-menu product, fake game, or substitute for Looking Glass.

## 2. Core philosophy

The Laboratory is an engineering control panel: isolated, deterministic, visually obvious, lightly humorous, and disposable as presentation architecture. It is not a polished consumer save menu, reusable UI framework, Reference Showcase, or demo of every supported presentation style.

## 3. Minimal scene shape

Preferred implementation:
- one importable/direct-scene Chronicle Laboratory scene;
- one package-owned bootstrap/harness;
- one tiny persisted participant;
- one crude panel;
- one evidence console;
- deterministic fixture helpers only where needed.

Use the simplest Unity presentation mechanism practical. IMGUI/`OnGUI` is explicitly acceptable.

## 4. Sperk proof participant

Persist only primitive values that are obvious before/after Save/Load. Suggested fields:
- `Sperk Level`;
- `Galactic Rupees`;
- `Anvil Temperature`;
- `Has Forbidden Key`;
- `Reality Damage`.

Required visual proof: mutate values -> Save -> mutate without saving -> Load -> saved values visibly return.

## 5. Minimal controls

### Normal Operations
- Initialize/service status if useful;
- Create Slot;
- Select Slot;
- Save;
- Load;
- Duplicate Slot;
- Preview/Confirm Delete;
- Refresh Catalog.

### Sperk Test Subject
- primitive value fields/buttons;
- deterministic reset;
- mutate-without-saving controls.

### Controlled Evidence
- deterministic fixture preparation for specialized LAB cases;
- unknown-payload/prune/cache/recovery/inspection hooks only where LAB evidence requires them.

### Evidence Console
Show service state, active slot, generation, catalog count/health, relevant cache/unknown state, last operation status, and explicit evidence text.

## 6. Presentation boundary

Allowed: crude IMGUI, scroll views, buttons, primitive fields, simple evidence logs, restrained Systems Foundry/Sperk humor.

Not allowed: reusable production save-menu widgets, elaborate navigation/animation framework, custom UI subsystem inside EchoSave, Looking Glass dependency, or Resonance dependency.

Approved sample flavor may include `SUBJECT: SPERK-001`, `+100 GALACTIC RUPEES`, `DAMAGE REALITY`, `DO NOT PRESS UNLESS REALITY IS BACKED UP`, and `RESULT: THE CHRONICLE REMEMBERS.` Technical APIs remain neutral.

## 7. LAB-001 through LAB-032

The existing LAB matrix remains acceptance authority. Do not build 32 polished workflows. Group related cases, expose deterministic fixture buttons, log real Chronicle results, and record screenshots/manual evidence only where useful. A LAB row passes only when the harness verifies actual Chronicle state/result.

## 8. Production-data isolation

The Laboratory must use obvious Lab-owned state/root identity, refuse unsafe collisions, provide cleanup, avoid silent fallback to production saves, and remain bounded/repeatable.

## 9. Reference Showcase deferral

Do not build polished save-format examples in M5-06. Later Reference Showcase candidates include one-slot Continue, recognizable three-slot fantasy-adventure, configurable fixed slots, broad multi-slot browser, effectively-unbounded catalog, and manual/autosave/checkpoint UX variations. These belong after Looking Glass and preferably Resonance exist.

## 10. Tests

Add focused EditMode tests for Lab bootstrap assumptions, participant capture/apply round-trip, deterministic proof-state mutation/reset, Lab-root isolation, fixture ownership/cleanup, evidence-state calculations, and helper logic used for LAB cases. Do not test IMGUI cosmetics. Full focused Chronicle Editor suite must remain green at actual discovered total and not below **753 / 753**.

## 11. Manual proof

At minimum capture scene/service readiness, create/select slot, set Sperk proof values, Save, mutate unsaved values, Load, visibly restored values, create/duplicate/delete/catalog flows required by LAB matrix, specialized deterministic fixtures, real evidence-console output, and verified Lab cleanup.

## 12. Explicitly deferred

M5-06 does not implement Chronicle Reference Showcase, Looking Glass integration, Resonance integration, production save-menu templates, fake gameplay systems, production cleanup/quarantine, restore-from-trash public API, permanent erase, automatic recovery fallback, generic queueing, automatic autosave timing, or permission-provider production wiring.

## 13. Closeout

M5-06 closes only when implementation is committed, Unity compile is clean, focused Chronicle Editor suite is green >= 753, the direct-scene Lab is reproducible, visible Save -> mutate -> Load proof passes, LAB-001 through LAB-032 are truthfully reconciled, Lab ownership/cleanup proof passes, documentation matches committed behavior, and the repository is clean. M5 may close afterward only if milestone reconciliation finds no remaining M5 authority gap.

## 14. Closeout evidence

**Activation:** `d6f079a`
**Implementation:** `4bcfbf1`
**Adjacent project organization:** `b43e6bf`
**Final focused Chronicle Editor:** **761 / 761 passed, 0 failed**
**Net-new focused tests:** **8**
**Final Chronicle package scope:** **21 files / 2341 insertions / 0 deletions**

### Automated/package gate

The incoming `753 / 753` focused Chronicle floor advanced to **761 / 761** with eight new M5-06 tests. The final 761-test gate remained green after the Package Manager import correction and package-owned scene synchronization.

### Import correction

Initial Package Manager import surfaced a sample-only compile defect:
`ChronicleSaveLaboratoryHarness.cs(66,13): CS4008: Cannot await 'void'`.

The harness was correctly awaiting `InitializeLaboratoryAsync()`, but that sample-only method had been declared `async void`. It was corrected to `async Awaitable`, the imported sample was reimported, Unity compiled cleanly, and no Chronicle Runtime source/API change was required.

### Direct-scene proof

The Laboratory initialized directly with `Service: Ready`, one authoritative root, zero initial catalog slots, and isolated `EchoSave-M5-06-Laboratory` storage.

Retained manual evidence proves:
- LAB-003 create/select -> one healthy slot + initial immutable generation;
- Save -> verified generation/head publication;
- unsaved mutation -> visibly different live `SPERK-001` state while saved snapshot remained unchanged;
- Load & Apply -> exact saved values restored, `RESULT: THE CHRONICLE REMEMBERS.`;
- LAB-006 prepared load -> live bound handle, explicit wait boundary, successful apply, handle consumed;
- LAB-008 rename -> display metadata changed while technical slot identity/path remained stable;
- LAB-009 duplicate -> second healthy technical slot with distinct technical identity/generation;
- delete Preview -> source-bound/expiring zero-write plan with two live slots still present;
- LAB-011 Confirm -> duplicate removed from live catalog, active selection cleared, original slot preserved;
- LAB-031 reset -> Chronicle shutdown, ownership-marker verification, exact Laboratory-root removal, post-cleanup absence.

A simple scene-owned camera/background was added to remove Unity's `Display 1 / No cameras rendering` overlay from human Game-view evidence and synchronized back into the distributable `Samples~` scene.

### LAB-001 through LAB-032 reconciliation

The matrix closes from retained evidence rather than thirty-two polished UI flows:
- M4/M5 automated tests retain evidence for established authority, duplicate-root, multi-participant, failure, corruption, migration, cancellation, queueing, retention, backend, and packaging cases;
- M5-05 directly closed the previously missing LAB-016 unknown-prune and LAB-029 catalog-cache prerequisites;
- M5-06 adds the direct-scene human integration evidence listed above;
- the final focused Chronicle gate is **761 / 761**.

No PASS is inferred from presentation alone; evidence remains tied to real Chronicle result/state.

### Project organization

The adjacent `b43e6bf` project commit separated retained polished First Light/UMBRA showcase content from imported package sample content. This commit is not part of the 21-file Chronicle package implementation scope.

The Chronicle Laboratory remains a package-distributed engineering sample. A polished Chronicle Reference Showcase remains future work after Looking Glass and preferably Resonance. First Light's proper package sample restoration is a separate follow-up and is not silently replaced by UMBRA.

### Milestone result

ESV-M5-06 is **Complete**.

No remaining M5 authority gap was found. Chronicle **M5 — Tooling and Laboratory is Complete**.

M6 First Integration remains **not activated** and requires a separate authority/activation checkpoint.
