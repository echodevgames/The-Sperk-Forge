# The Sperk’s Forge – Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 4, 2026  
**Current focus:** First Light package skeleton  
**Current checkpoint:** FL-M1-01

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

## How to Use This Page

- Record only active facts, questions, risks, tests, and handoff details.
- Prefix durable entries with the approved SFGSS-010 labels.
- Promote lasting truth into the owning authority before checkpoint closeout.
- Keep one current Handoff Snapshot.

## Current Focus

### Goal

Execute **FL-M1-01 – First Light Package Skeleton** exactly as authorized. Begin with the live Unity project, Git, package-path, and uGUI-version checks. Stop before any C# file or launch behavior.

### Starting state

- SUITE-DOC-33 passed with advisory.
- The implementation program is activated under checkpoint control.
- PKG-LEARN-001 First Light is complete.
- FL-M1-01 v1.3.0 is active and authorized.
- First Light implementation has not started.
- PKG-LEARN-002 Observatory is paused until EchoDiagnostics implementation approaches.
- Every package other than First Light remains locally locked.

## Active Notes

### August 4, 2026 – Initial implementation readiness

- `[DECISION]` SUITE-DOC-33 passed with advisory.
- `[DECISION]` Only FL-M1-01 is active.
- `[NOTE]` The gate validates documentation and learning readiness, not the live Unity project.
- `[TEST]` Unity compile, Git state, package-path inspection, and exact uGUI version remain `Not run`.
- `[HANDOFF]` Start FL-M1-01 at Section 2 starting conditions. Do not create files until those checks pass.

**Promoted to:** SUITE-DOC-33 report; SFGSS-000 v0.23.0; First Light specification v1.3.0; FL-M1-01 plan v1.3.0; roadmap; README; graph; health check; learning tracker.

## Open Questions

- `[QUESTION]` What exact `com.unity.ugui` version is resolved by the Unity 6000.3.8f1 project?
- `[QUESTION]` Is `Packages/com.echodevgames.echo-launch/` absent, or does it contain work requiring review?
- `[QUESTION]` Multiplayer provider selection remains intentionally unresolved until disposable prototypes execute.

## Promotion Queue

| Entry | Destination | State |
|---|---|---|
| SUITE-DOC-33 decision | Readiness report, Bible, First Light spec, FL-M1-01 plan | Promoted |
| Live Unity and Git checks | FL-M1-01 test report | Pending execution |
| Exact uGUI version | `package.json` and FL-M1-01 evidence | Pending execution |

## Latest Validation Snapshot

| Area | Result |
|---|---|
| Documentation and authority gate | Pass |
| First Light learning gate | Pass |
| FL-M1-01 scope review | Pass |
| Package implementation | Authorized, not started |
| Unity compile | Not run |
| Git working tree review | Not run |
| Exact uGUI version | Not run |
| Other packages | Locked |

## Checkpoint Closeout Checklist

- [ ] Verify FL-M1-01 starting conditions.
- [ ] Show and explain every authorized file before creation.
- [ ] Create only the approved skeleton.
- [ ] Run FL-M1-T-001 through FL-M1-T-012.
- [ ] Reconcile package and suite documentation.
- [ ] Commit and push the completed FL-M1-01 checkpoint.

## Handoff Snapshot

**Completed checkpoint:** SUITE-DOC-33 – Initial Implementation Readiness Gate  
**Active checkpoint:** FL-M1-01 – First Light Package Skeleton  
**First Light learning:** PKG-LEARN-001 complete  
**First Light implementation:** Authorized, not started  
**Package learning reviews:** 1 of 28 complete  
**Other package implementations:** Locked  
**Known blockers:** None recorded; four live-project starting conditions remain unverified  
**Evidence state:** Implementation evidence remains `Not run`


## Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Test Reports/SUITE-DOC-33_Initial_Implementation_Readiness_Gate_Report|SUITE-DOC-33 Readiness Report]]
- [[Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan|FL-M1-01 Checkpoint Build Plan]]
- [[Learning Reviews/PKG-LEARN-001_EchoLaunch_Learning_Review|First Light Learning Review]]

## FL-M1-01 First Light Package Skeleton

- Status: Complete, pending commit and push
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Unity restart: Pass
- Removal and reinstallation: Pass
- Stable asmdef GUID preservation: Pass
- C# implementation files: 0
- Runtime implementation remains locked.

## FL-M2-01 First Light Authority Claim and Static Reset Core

- Status: Complete, pending commit and push
- Package version: `0.1.0`
- Runtime tests: 7 passed, 0 failed, 0 ignored
- Duplicate diagnostic `ELAUNCH-ROOT-001`: Expected and verified
- Next runtime work remains locked pending approval.

## FL-M2-02 First Light Neutral Launch-State Vocabulary

- Status: Complete, pending commit and push
- Package version: `0.1.0`
- Vocabulary tests: 39 passed, 0 failed, 0 ignored
- Full Runtime Play Mode suite: 46 passed, 0 failed, 0 ignored
- Next runtime work remains locked pending approval.
