# SUITE-DOC-03 — Data, IDs, Serialization, and Migration Audit Report

**Checkpoint:** SUITE-DOC-03  
**Deliverable:** SFGSS-003 v1.0.0  
**Result:** Passed with non-blocking reconciliation items  
**Date:** August 4, 2026  
**Implementation state:** Locked; not started

---

## 1. Audit scope

The audit reviewed:

- SFGSS-000 v0.10.0.
- SFGSS-001 v1.1.0.
- SFGSS-002 v1.0.0.
- SFGSS-ADR-001 and SFGSS-ADR-002.
- SFGSS-INT-FOUNDATION-001.
- All ten approved Foundation package specifications.
- The active full-suite roadmap, README, and Current Notes.

The review compared:

- definition/configuration/runtime-state separation;
- Unity asset GUID usage;
- package/project stable IDs;
- serializer and DTO assumptions;
- document and payload schema versions;
- aliases and rename behavior;
- unknown optional-data preservation;
- migration ownership;
- transaction and rollback claims;
- removal/reinstallation survival.

## 2. Result summary

| Area | Result |
|---|---|
| One owner per durable truth | Pass |
| Definitions separated from mutable runtime state | Pass |
| Stable IDs independent from display/path | Pass with wording advisories |
| Unity GUID versus runtime domain ID | Standard added; two specs need clarification |
| Document/payload version layering | Pass |
| Unknown optional-data preservation | Pass with serializer-strategy advisory |
| Migration ownership and no silent downgrade | Pass |
| Transaction/publication semantics | Pass |
| Clean removal and reinstallation | Pass |
| Implementation evidence | Correctly remains Not run |
| Release-blocking collision | None |

## 3. Approved decisions

1. Unity asset GUIDs, domain stable IDs, and runtime instance IDs are separate identity domains.
2. Domain IDs use either an opaque lowercase 32-hex form or an approved namespaced semantic form.
3. Shared definitions/configuration assets remain immutable during runtime.
4. Durable data uses detached DTOs or bounded opaque records, not live Unity object graphs.
5. Durable documents declare format and schema versions independently from package SemVer.
6. Migrations are explicit, contiguous, forward, staged, reported, and source-preserving.
7. Aliases resolve old IDs to canonical IDs; tombstones prevent accidental reuse.
8. Unknown optional settings/save/provider records are preserved and never executed.
9. Authoritative publication occurs only after required validation/staging/verification succeeds.
10. Removal of code does not imply deletion of project-owned durable data.

## 4. Foundation reconciliation findings

### D-003-001: Asset GUID language in Accord

**Severity:** Advisory  
**Finding:** `EchoSettingsConfiguration` and `SettingsDefaultsProfile` list Asset GUID as stable identity.  
**Resolution:** SFGSS-003 distinguishes asset identity from runtime/durable domain identity. Reword during SUITE-DOC-10; add domain ID only if a runtime/report/export contract needs it.

### D-003-002: Asset GUID language in Chronicle

**Severity:** Advisory  
**Finding:** `EchoSaveConfiguration` lists “Asset GUID only.”  
**Resolution:** Same as D-003-001.

### D-003-003: Passage scene source identity

**Severity:** Advisory  
**Finding:** Passage correctly defines `SceneId`, but its Editor source GUID/path metadata must not become a Player AssetDatabase dependency.  
**Resolution:** Confirm runtime catalog/build record during SUITE-DOC-10 and implementation planning.

### D-003-004: Unknown JSON preservation

**Severity:** Advisory  
**Finding:** Accord and Will promise unknown/extension preservation. Unity JSON DTO round trips do not inherently preserve unknown fields.  
**Resolution:** Choose opaque raw records or an extension-capable serializer provider before implementation.

### D-003-005: Enum compatibility

**Severity:** Advisory  
**Finding:** Public asset/document enums are specified semantically but do not all state explicit no-reorder/no-reuse numeric/token policy.  
**Resolution:** Reconcile Foundation specifications at SUITE-DOC-10.

### D-003-006: Fingerprint canonicalization

**Severity:** Advisory  
**Finding:** Will and Workshop use fingerprints, but every fingerprint algorithm/canonical input still needs implementation-level versioning.  
**Resolution:** Preserve as an implementation requirement; evidence remains Not run.

## 5. No-blocker conclusion

No Foundation package stores live mutable state in shared definition assets by design. Accord and Chronicle have distinct persistence authority. Unknown optional data survives clean removal. No package claims downgrade support or fabricated migration evidence.

SFGSS-003 can therefore be approved without revising package authority.

## 6. Validation performed

- Confirmed SFGSS-003 contains all thirty planned sections.
- Confirmed the Foundation matrix covers all ten package specifications.
- Confirmed SFGSS-002 clean-removal behavior is preserved.
- Confirmed SFGSS-000 is advanced to v0.11.0.
- Confirmed roadmap and README advance to SUITE-DOC-04.
- Confirmed implementation remains locked.
- Confirmed no package manifests, asmdefs, C# files, scenes, prefabs, ScriptableObjects, setup tools, bridges, or providers were created.

## 7. Next checkpoint

**SUITE-DOC-04:** SFGSS-004 — Testing, Validation, Test Labs, and Release Standard.
