---
tags:
  - sfgss/adr
  - sfgss/decision-log
  - sfgss/navigation
status: active
updated: 2026-08-04
---

# The Sperk’s Forge — Suite Architecture Decision Log

**Document role:** Navigation and status index  
**Authority:** Index only; each ADR and its affected higher-authority documents remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Standard:** [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007]]  
**Next available suite ADR:** `SFGSS-ADR-004`

> The log tells us which doors were chosen. The ADRs explain why, and the specifications define what lies beyond them.

## Accepted suite ADRs

| ID | Decision | Version | Evidence maturity | Scope | Review trigger |
|---|---|---:|---|---|---|
| [[SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|SFGSS-ADR-001]] | Suite Package Editor Setup Facade Protocol | 1.2.0 | Design approved; evidence pending | Workshop/package setup integration | Three real facades reveal the need for compiled shared contracts, or protocol/version/removal tests fail |
| [[SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation|SFGSS-ADR-002]] | Full Suite Documentation Gate and Learning-Oriented Implementation | 1.0.0 | Not applicable to runtime evidence | Suite governance and implementation workflow | Final readiness gate, owner workflow change, or learning reviews change implementation order |
| [[SFGSS-ADR-003_Graph_Roadmap_and_Pre-Implementation_Learning_Review|SFGSS-ADR-003]] | Graph Roadmap and Pre-Implementation Package Learning Review | 1.0.0 | Not applicable to runtime evidence | Documentation navigation and learning | Graph becomes unmanageable, package grouping changes, or reviews expose a better order |

## Proposed suite ADRs

None.

## Rejected suite ADRs

None.

## Withdrawn suite ADRs

None.

## Superseded suite ADRs

None.

## Linked package and integration ADRs

No package-local or integration-local ADR has been created yet. Their future logs remain owned by their repositories and are linked here when they affect suite compatibility or guided pathways.

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
