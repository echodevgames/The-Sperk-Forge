# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 7, 2026
**Current focus:** First Light FL-M5-07 closeout
**Current checkpoint:** FL-M5-07 — Standalone Test Laboratory and Importable Package Sample

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Checkpoint State

- Branch: `main`
- Authority commit: `741b77d`
- Implementation commit: `583b91a`
- Documentation closeout commit: pending
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Status: implementation complete; documentation commit and push pending
- Compilation: `0` errors, `0` warnings
- Focused Laboratory EditMode: `7` passed
- Complete EditMode: `299` passed
- Runtime Play Mode: `503` passed
- Total automated: `802` passed, `0` failed, `0` ignored

## Delivered Outcome

- One UPM sample is declared as `First Light Standalone Test Lab`.
- Package Manager import is explicit and creates one editable project-owned copy.
- The sample contains Boot and Destination scenes, neutral presentation,
  pre-authored scenario configurations, public-API sample steps, a visible
  readout, a duplicate-root fixture, and Direct Scene proof.
- Import does not automatically change Build Settings, ProjectSettings,
  canonical setup assets, or scripting defines.
- Sample Runtime code references only First Light Runtime and approved Unity
  modules; core assemblies do not reference sample code or content.
- The imported sample is excluded from automatic Setup/Repair candidate
  discovery.
- Direct Scene sample generation reloads and verifies the persistent
  `DirectSceneConfiguration` before saving its scene reference.
- Sample removal preserves package compilation and tools; reimport restores
  one clean copy.

## Accepted Evidence

- `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012`: passed.
- Canonical Boot launch completed and handed off to Destination.
- Timed progress, warning continuation, recoverable-failure continuation, and
  blocking failure behaved according to authored policy.
- Missing configuration blocked with `ELAUNCH-CFG-001`.
- Duplicate authority was rejected with `ELAUNCH-ROOT-001` and no duplicate
  launch side effects.
- Invalid destination blocked with `ELAUNCH-DEST-001`.
- Direct Scene creation and existing-authority reuse completed without scene
  reload; the final imported Destination initializer retained its serialized
  `LaboratoryDirectSceneConfiguration` reference.
- Early splash skip remained bounded by the configured minimum duration.
- Import, removal, reimport, three-run Setup/Repair repeatability, canonical
  asset preservation, and repair-backup cleanup were accepted.
- Final implementation staging excluded imported `Assets/Samples`, temporary
  authoring content, Build Settings, ProjectSettings, and generated solution
  drift.

## Narrow Checkpoint-Owned Corrections

1. Setup/Repair candidate collection now ignores imported First Light sample
   definitions and root prefabs so sample content cannot become canonical
   project setup by discovery.
2. Laboratory authoring now saves, imports, reloads, and verifies the
   persistent Direct Scene configuration before the Destination scene is
   saved, preventing a `{fileID: 0}` serialized reference.

Both corrections are covered by focused regression tests and the complete
`299` EditMode plus `503` Runtime Play Mode suites.

## Deferred Beyond FL-M5-07

- M6 project adoption and optional bridges
- Git URL, tarball, and separate clean-project installation evidence
- player builds and automatic production-startup evidence
- historical configuration migration
- receipts, uninstall/reset implementation, and crash-persistent recovery
- automatic Direct Scene installation and build hooks
- persistent-root lifetime policy
- performance evidence and external adoption

## Next Action

Review and commit the documentation closeout:

```text
Close out FL-M5-07 standalone laboratory checkpoint
```

Then push `main`, confirm `main == origin/main`, and confirm a clean working
tree before selecting the next just-in-time learning review.
