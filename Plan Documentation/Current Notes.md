# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 18, 2026
**Current focus:** No active Looking Glass checkpoint — EUI-M4-03 Runtime Motif Foundation is complete
**Current checkpoint:** None for The Looking Glass (`EchoUI`); Primitive Warehouse is named next direction only

> Durable authority lives in the package specifications, suite authorities, decisions, learning reviews, checkpoint plans, and Git history. This living page is intentionally compacted after completed work so current rehydration truth stays near the top.

---

## Looking Glass EUI-M4-03 final closeout — August 18, 2026

- Clean incoming baseline: `2f592513b6215b019ca9550fb302ab0cee6b65cc` — EUI-M4-02 documentation closeout.
- Suite authority remains **SFGSS-000 v0.27.0**; package authority remains **SFGSS-PKG-ECHOUI-001 v1.9.0**; workflow authority remains **SFGSS-005 v1.7.0**.
- Package remains `com.echodevgames.echo-ui` `0.1.0` on Unity **6000.3.8f1** with required `com.unity.ugui` **2.0.0**. M4-03 adds no hard peer Echo dependency and no mandatory TextMeshPro package dependency.
- **EUI-M4-03 is COMPLETE / CLOSED.** No successor Looking Glass checkpoint is active.

### Durable implementation chain

- activation `435fc66`;
- Motif contracts `d67550d`;
- catalog/fallback `172d230`;
- session service `43da17a`;
- registered targets `efbc503`;
- reusable bindings `e17d816`;
- root integration `ab5906c`;
- root EditMode teardown fixture correction `d291885`, with no Runtime source change;
- mirrored Motif Laboratory `b48eae68`;
- Check 3 proof assertion correction `7f9272bd`;
- Check 4 proof sequencing correction and accepted Laboratory head `8188b91c`.

### Final accepted automated evidence

`TestResults_20260818_060619.xml`:

- full Foundry EditMode **1445 / 1445 PASS**;
- EchoUI Editor **339 / 339 PASS**;
- all Motif fixtures **62 / 62 PASS**;
- `EchoUIMotifRootIntegrationTests` **12 / 12 PASS**;
- failed / skipped / inconclusive **0 / 0 / 0**.

The first full root-integration run was **1444 / 1445**. Its only failure, `RootDestructionShutsDownMotifRegistrations`, was an EditMode fixture mismatch: the GameObject was destroyed without explicitly invoking the private `EchoUIRoot.OnDestroy()` path used by established root tests. `d291885` corrects only that fixture. Runtime did not change. Final evidence above supersedes the intermediate run.

Intentional `motif-observer`, `motif-target`, and `root-motif-observer` exception logs remain accepted isolation proofs.

### Final manual Laboratory evidence

- Check 1 Default + Typed Tokens: **PASS**.
- Check 2 Switch + Keep Local: **PASS**.
- Check 3 Missing Token Safety: **PASS**.
- Check 4 Unknown ID Fallback: **PASS**.
- Check 5 Failure + Stale + Reset: **PASS**.
- Check 6 180-Frame Idle: **PASS**.
- Retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 representative smoke: **user-confirmed green**; exact per-tab observation strings were not separately supplied.
- Package/imported Motif proof source parity: **VERIFIED**.

Check 5 injects one deliberately broken target. The target throws once during immediate registration application and again during the following Motif switch. Both exceptions are caught and isolated by `UIMotifService`; **two target exception logs are intentional proof evidence**, not a failure.

### Laboratory-owned corrections

- `7f9272bd`: Check 3 originally expected `RegisteredWithApplyFailure`; authoritative `Partial` apply truth is successful, so registration correctly reports `Registered` while preserving partial/failure counts and safe prior presentation.
- `8188b91c`: Check 4 originally requested an unknown ID while its fallback was already effective, so Runtime correctly returned `Unchanged`. The proof now resets to the authored default before the request, forcing a real `FallbackApplied` transition while preserving the caller's requested unknown ID.
- Neither correction changed Motif Runtime behavior or authority.

### Publication-history note

A mistaken connector invocation created transient root file `__nope__` in intermediate commit `2b1e6cf`. The immediately following accepted `8188b91c` tree removed it through a non-forced fast-forward. The current tree contains no `__nope__`; the harmless intermediate commit remains in history because `main` was not rewritten.

### EUI-M4-03 completed boundary

Looking Glass now supplies immutable project-owned Motif definitions/tokens, detached snapshots, authored default/fallback resolution, one root-session effective Motif, explicit registered targets, `UseMotif`/`KeepLocal` bindings, generation-safe release and cleanup, safe missing-token behavior, target/listener isolation, root facade integration, structured truth, and bounded idle behavior.

It does **not** own durable preference selection, global settings/accessibility policy, layout/navigation/domain meaning, gameplay input, pause/time scale, scene travel, persistence, audio/localization, project lifetime composition, or final production art.

The first token families remain colors, uGUI Selectable state recipes, sprites, and small numeric/decorative values. Full accessibility policy, Motif Editor capture/apply/preview tools, final Warehouse-facing typography/provider schema, Primitive Warehouse/templates/catalog/utilities, Builder/Composer, prompts/tooltips, safe area, richer Window management, persistence, bridges, integration, clean-project reproduction, and release qualification remain separately gated.

### Exact Looking Glass resume phase

There is no active Looking Glass implementation checkpoint. Rehydrate from live `main`, confirm EUI-M4-03 closeout, then perform a fresh bounded JIT review/activation before successor work. **Primitive Warehouse is the named next program direction only; it is not active.**

---

## Retained Looking Glass completion chain

- **EUI-M1-01:** closeout `57a4fa4`; full EditMode **1113 / 1113**; Laboratory **5 / 5**.
- **EUI-M1-02:** closeout `c114ba2`; full **1130 / 1130**; EchoUI **24 / 24**; Laboratory **10 / 10**.
- **EUI-M2-01:** closeout `d5b9a73`; full **1153 / 1153**; EchoUI **47 / 47**; focused **23 / 23**; Laboratory **10 / 10**.
- **EUI-M2-02:** closeout `7f5ad40`; full **1181 / 1181**; EchoUI **75 / 75**; focused **28 / 28**; Laboratory **12 / 12**.
- **EUI-M3-01:** closeout `0c58240`; full **1205 / 1205**; EchoUI **99 / 99**; focused **24 / 24**; Laboratory **12 / 12**.
- **EUI-M3-02:** closeout `0affb7d`; full **1246 / 1246**; EchoUI **140 / 140**; Laboratory **14 / 14**.
- **EUI-M4-01:** closeout `5e7ad92`; manual HUD Laboratory **5 / 5**, retained smoke green; exact post-M4 aggregate was not captured, so the retained pre-M4 **1246 / 1246** floor is not relabeled.
- **EUI-M4-02:** closeout `2f59251`; final full **1383 / 1383**; EchoUI **277 / 277**; notifications **125 / 125**; presenter **17 / 17**; Laboratory **6 / 6**, retained smoke green.
- **EUI-M4-03:** final full **1445 / 1445**; EchoUI **339 / 339**; Motifs **62 / 62**; root **12 / 12**; Laboratory **6 / 6**; 180-frame idle PASS; retained smoke green.

---

## Other suite frontiers retained for rehydration

### First Light (`EchoLaunch`)

First Light remains package version `0.1.0` and frozen for the current pass after FL-M5-R1 / in-repository Reference Showcase work. Its package sample is `FirstLight_Boot_Splash_Laboratory`; UMBRA remains separate project-owned First Light showcase content. The repository-owned Distribution Kit standard applies. No additional First Light checkpoint is activated by this Looking Glass closeout.

### The Chronicle (`EchoSave`)

Chronicle M5 Tooling and Laboratory is complete through ESV-M5-06. The package declares/imports `Chronicle Save Laboratory`; direct-scene evidence proved the bounded save/load/slot-management path; final recorded focused Chronicle Editor gate is **761 / 761 passed**. M6 First Integration is not automatically active. Chronicle authority and detailed history remain in Chronicle-owned current notes/specification/checkpoint records.

### Suite architecture

Durable persistence, runtime state, and Unity object lifetime remain separate concerns. Packages expose their own runtime truth; persistence bridges are optional; project-owned lifetime composition must not turn First Light, Chronicle, Looking Glass, or another package into a universal service locator.

The suite package-development loop remains: Learn/authority → bounded implementation → standalone engineering proof → project-owned Reference Showcase → clean-project reproduction → release qualification/private beta. A completed checkpoint does not fabricate later-gate evidence.

---

## Stop point

**The Looking Glass EUI-M4-03 closeout is the stop point.** Do not begin Primitive Warehouse or any other Looking Glass successor merely because its direction is named. A successor requires an explicit bounded checkpoint activation.
