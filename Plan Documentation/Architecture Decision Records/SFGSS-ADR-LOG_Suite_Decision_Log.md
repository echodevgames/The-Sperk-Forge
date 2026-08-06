---
tags:
  - sfgss/adr
  - sfgss/decision-log
  - sfgss/navigation
status: active
updated: 2026-08-06
---

# The Sperk’s Forge — Suite Architecture Decision Log

**Document role:** Navigation and status index
**Authority:** Index only; each ADR and its affected higher-authority documents remain authoritative
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Standard:** [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007]]
**Next available suite ADR:** `SFGSS-ADR-005`

> The log tells us which doors were chosen. The ADRs explain why, and the specifications define what lies beyond them.

## Accepted suite ADRs

| ID | Decision | Version | Evidence maturity | Scope | Review trigger |
|---|---|---:|---|---|---|
| [[SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|SFGSS-ADR-001]] | Suite Package Editor Setup Facade Protocol | 1.2.0 | Design approved; evidence pending | Workshop/package setup integration | Three real facades reveal the need for compiled shared contracts, or protocol/version/removal tests fail |
| [[SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation|SFGSS-ADR-002]] | Full Suite Documentation Gate and Learning-Oriented Implementation | 1.0.0 | Not applicable to runtime evidence | Suite governance and implementation workflow | Final readiness gate, owner workflow change, or learning reviews change implementation order |
| [[SFGSS-ADR-003_Graph_Roadmap_and_Pre-Implementation_Learning_Review|SFGSS-ADR-003]] | Graph Roadmap and Package Learning Review | 1.0.0 | Not applicable to runtime evidence | Documentation navigation and learning; sequencing partially superseded by ADR-004 | Graph becomes unmanageable, package grouping changes, or reviews expose a better order |
| [[SFGSS-ADR-004_Just-in-Time_Package_Learning_Gate|SFGSS-ADR-004]] | Just-in-Time Package Learning Gate | 1.0.0 | Not applicable to runtime evidence | Package-local learning and implementation authorization | Package learning cadence changes or multi-package implementation makes the local gate impractical |

## Proposed suite ADRs

None.

## Rejected suite ADRs

None.

## Withdrawn suite ADRs

None.

## Superseded suite ADRs

None.

## Linked package and integration ADRs

Package-local ADRs remain owned by their package documentation and are linked here when they affect suite compatibility, authority boundaries, or guided setup pathways.

### First Light (`EchoLaunch`)

| ID | Decision | Status | Evidence maturity |
|---|---|---|---|
| [EchoLaunch-ADR-001](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-001_Project-Owned_Launch_Destination_and_Configuration_Schema_3.md) | Project-Owned Launch Destination and Configuration Schema 3 | Accepted | Implemented and retained |
| [EchoLaunch-ADR-002](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-002_Splash_Configuration_Schema_4_and_Root_Playback_Order.md) | Splash Configuration Schema 4 and Root Playback Order | Accepted | Implemented and tested |
| [EchoLaunch-ADR-003](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-003_Neutral_Startup_Prefab_Templates_and_Canvas_Assembly.md) | Neutral Startup Prefab Templates and Canvas Assembly | Accepted | Implemented and tested |
| [EchoLaunch-ADR-004](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-004_Read-Only_Project_Snapshot_and_Non-Destructive_Setup_Plan.md) | Read-Only Project Snapshot and Non-Destructive Setup Plan | Accepted | Implemented and tested |
| [EchoLaunch-ADR-005](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-005_Approved_Setup_Apply_Engine_and_Repeat-Safe_Asset_Creation.md) | Approved Setup Apply Engine and Repeat-Safe Asset Creation | Accepted | Implemented, automated-tested, and manually accepted in FL-M5-02 |
| [EchoLaunch-ADR-006](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-006_Explicit_Setup_Repair_and_Existing-Asset_Reconciliation.md) | Explicit Setup Repair and Existing-Asset Reconciliation | Accepted | Implemented, automated-tested, and manually accepted in FL-M5-03 |
| [EchoLaunch-ADR-007](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-007_Read-Only_Validator_and_Deterministic_Project_Health_Report.md) | Read-Only Validator and Deterministic Project Health Report | Accepted | Implemented, automated-tested, and manually accepted in FL-M5-04 |
| [EchoLaunch-ADR-008](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-008_Direct_Scene_Development_Initializer_and_Release-Safe_Runtime_Gate.md) | Direct Scene Development Initializer and Release-Safe Runtime Gate | Accepted | Implemented, automated-tested, and manually accepted in FL-M5-05 |
| [EchoLaunch-ADR-009](../../Packages/com.echodevgames.echo-launch/Documentation~/Developer/ADR/EchoLaunch-ADR-009_Editor-Only_Launch_Simulator_and_Deterministic_Failure_Injection.md) | Editor-Only Launch Simulator and Deterministic Failure Injection | Accepted | Implemented, automated-tested, manually accepted, and cancellation-determinism-corrected in FL-M5-06 |

## Candidate decisions without allocated IDs

| Candidate | Required trigger/evidence | Current state |
|---|---|---|
| First EchoMultiplayer production provider | Two comparable disposable prototypes plus license, cost, security, hosting, platform, and migration review | Provider-neutral foundation approved; prototypes `Not run` |
| EchoControllers package-family split | A third family/backend proves separate dependencies or release cadence | One modular package approved for MVP |
| AI navigation/behavior provider adoption | Adapter prototype, licensing, compatibility, removal, and Laboratory evidence | Candidates only |
| Observatory native hardware-sensor provider | Platform research and privacy/security boundary | Deferred |
| Suite source and sample licensing model | Public distribution policy before stable release | Open |
| Public Unity support-floor change | Compatibility matrix and migration consequences | Unity 6000.0 remains planned floor |
| Shared contracts package | At least three packages prove a repeated neutral contract that cannot remain locally owned | No mandatory shared core |

## Maintenance checklist

- [ ] Add every Proposed ADR immediately.
- [ ] Preserve rejected, withdrawn, and superseded IDs.
- [ ] Update version, status, evidence maturity, and review trigger with every ADR change.
- [ ] Link affected package specifications and integration matrices.
- [ ] Update `Suite_Graph_Roadmap.md`.
- [ ] Reconcile `Current Notes.md`.
- [ ] Never present the log itself as the architectural authority.

## Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007 ADR Standard]]
- [[SFGSS-ADR-TEMPLATE|Reusable ADR Template]]
- [[Current Notes]]
