# SFGSS-ADR-005 — Package Reference Showcases and the Suite Showcase Hub
**Document ID:** `SFGSS-ADR-005`
**ADR version:** `1.0.0`
**Status:** Accepted
**Decision date:** `2026-08-08`
**Last reviewed:** `2026-08-08`
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Decision scope:** Suite
**Evidence maturity:** Design approved; first implementation pending
**Parent authorities:** SFGSS-000, SFGSS-001, SFGSS-004
**Affected documents:** SFGSS-000, SFGSS-001, SFGSS-004, future/active package specifications, Suite Graph Roadmap, Current Notes
**Supersedes:** None
**Superseded by:** None
**Review triggers:** Three package showcases reveal a repeated structure that should be standardized further; the Suite Showcase Hub begins implementation; external beta workflow proves the rule too heavy or too weak; the suite cosmology resolves the hub’s final lore-facing name
**Related evidence:** First Light FL-M5-07 completed `809 / 809` automated and `12 / 12` manual Laboratory acceptance; production-style consumer demonstration not yet implemented
> Every package gets a clean in-house display case after its engineering proof: a project-owned Reference Showcase built through the same public surfaces available to a real consumer.

---

## 1. Context and problem

The suite already separates Standalone Laboratories, Integration Laboratories, and optional Showcases. That architecture proved excellent for engineering evidence, but First Light FL-M5-07 exposed a usability gap: the package could prove its internals exhaustively while the development repository still lacked a clean example of what normal production use should actually look like.

The Standalone Test Lab is intentionally diagnostic. It answers questions such as duplicate authority, invalid configuration, warning/failure policy, direct-scene behavior, and timing. It should not be forced to double as the polished example a developer, collaborator, portfolio viewer, or future maintainer uses to understand the package at a glance.

### 1.1 Known facts

- First Light FL-M5-07 passed its complete Laboratory acceptance matrix and final regression suite.
- The existing suite standards treated combined Showcases as optional presentation evidence.
- Package samples are intentionally removable and cannot become production dependencies.
- Project-owned content is the correct home for actual game/studio presentation and configuration.
- An external consumer should be able to reproduce normal package usage without repository-only privileges.

### 1.2 Assumptions and evidence gaps

- The exact reusable folder/prefab conventions for Reference Showcases may evolve after several packages use the pattern.
- The final visual design and navigation of the Suite Showcase Hub are not yet implemented.
- First Light will be the first executed proof of this convention.

---

## 2. Decision drivers and constraints

- Preserve package independence and Test Lab isolation.
- Make every completed package understandable as a real usable product, not only as a diagnostic system.
- Keep game/studio art, scenes, and filled configuration project-owned.
- Exercise the same documented public setup and API surfaces an outside user receives.
- Avoid shipping test-only or repository-only dependencies into package Runtime.
- Create a consistent portfolio/onboarding surface as the suite grows.
- Keep one package showcase small enough that it does not become a second game project.

---

## 3. Options considered

### Option A — Keep Showcases optional and combined only

**Description:** Retain the prior model where Laboratories are required and combined Showcase scenes are optional.

**Advantages:**
- Least additional work.
- No new documentation requirement.

**Costs/risks:**
- A package can be technically proven while still lacking a clean example of actual consumer usage.
- Combined scenes can hide which public setup belongs to which package.
- The suite loses an obvious in-house portfolio/onboarding surface.

### Option B — Require one project-owned Package Reference Showcase per package

**Description:** After isolated proof, create the smallest production-style in-house scene or scene set that consumes the package through documented public surfaces. Later link these from a suite hub.

**Advantages:**
- Separates engineering proof from correct-use presentation.
- Exercises real consumer ergonomics continuously.
- Gives each package a maintainable display case.
- Builds a natural portfolio and collaborator-onboarding collection.
- Makes consumer pain visible before external beta.

**Costs/risks:**
- Adds a required in-house maintenance surface.
- Requires discipline to keep the showcase consumer-like rather than privileged.
- Some Editor-only packages need an equivalent non-scene demonstration.

### Option C — Ship the Reference Showcase inside every package

**Description:** Make each Reference Showcase a required UPM sample.

**Advantages:**
- Consumers receive the exact same showcase.

**Costs/risks:**
- Bloats package distribution.
- Confuses internal branded presentation with required package content.
- Increases licensing/removal/versioning burden.
- Risks coupling the package to a large demonstration surface.

---

## 4. Decision

Option B is accepted.

1. Every scene-meaningful package must receive an in-house **Package Reference Showcase** after its Standalone Test Lab passes and before that package is presented as an external beta candidate.
2. The Showcase is project-owned, normally under `Assets/EchoDevGames/SuiteShowcase/<Package>/`, and is not immutable package source.
3. The Showcase uses only documented public package setup, configuration, prefabs, APIs, and extension seams. Test-only APIs, hidden internals, privileged repository state, and accidental unrelated-package dependencies are prohibited.
4. The Showcase demonstrates the normal front-facing happy path. Diagnostics may be available but are secondary and should not dominate the default presentation.
5. One scene is not mandatory. The smallest scene set that represents the actual workflow is correct.
6. Editor-only/non-scene packages provide the equivalent clean in-house reference workspace/window demonstration.
7. Distributed Showcase samples remain optional and require explicit package-specification approval.
8. A project-owned suite hub, working title **Suite Showcase Hub**, may later link or launch package Reference Showcases and integrated demonstrations.
9. the Suite Showcase Hub is not a new runtime package, authority, or substitute for Standalone/Integration evidence.
10. Clean-project beta proof should reproduce the Reference Showcase’s smallest normal consumer workflow.

---

### 4.1 Hub naming status

The architecture of the project-owned Suite Showcase Hub is approved, but its final lore-facing name is intentionally unresolved.

**Suite Showcase Hub** is only the current functional label. A later naming decision may draw from the Hackulos / Sperk-galaxy large computer-brain cosmology without changing the architectural decision recorded here.

## 5. Rationale

The suite now has its first completed piece: First Light. FL-M5-07 proved that exhaustive engineering evidence and understandable production usage are different jobs.

The Reference Showcase gives each package a display case without weakening the Laboratory. It also turns the in-house development workspace into an early consumer of every package’s public workflow. When that workflow is awkward, the suite discovers the usability problem before asking an outside developer to absorb it.

Keeping the Showcase project-owned preserves the existing rule that package source supplies reusable types, templates, and tools while concrete scenes, branding, art, and filled configuration belong to the consuming project.

---

## 6. Consequences

### 6.1 Positive

- Every package gains a clear correct-use demonstration.
- The suite accumulates a coherent internal portfolio as implementation progresses.
- Consumer ergonomics become testable before beta.
- Labs remain honest engineering tools instead of being polished beyond their purpose.
- the Suite Showcase Hub can later present the suite as a collection without changing package authority.

### 6.2 Costs and risks

- Each package has one additional project-owned surface to maintain.
- Reference Showcases must be checked after material public setup/API changes.
- the Suite Showcase Hub could become an accidental monolith if it starts owning package behavior rather than navigation/presentation.

### 6.3 Deferred consequences

- Exact Gallery navigation, art direction, and scene-loading architecture.
- Whether any individual package later distributes a Showcase sample.
- Whether repeated Showcase scaffolding should be generated by The Workshop after several implementations provide evidence.

---

## 7. Authority and document impact

| Document/artifact | Required action | Status |
|---|---|---|
| SFGSS-000 | Promote Reference Showcase and Gallery rule | Updated in this authority change |
| SFGSS-001 | Require package specs to define the Reference Showcase | Updated in this authority change |
| SFGSS-004 | Define evidence and beta gate | Updated in this authority change |
| Package specifications | Reconcile when package enters/continues implementation | First Light next |
| Suite Graph Roadmap | Add ADR and current display-case work | Updated in this authority change |
| Current Notes | Reconcile decision and next action | Updated in this authority change |
| Tests/research | First executed proof in First Light M6 | Pending |

---

## 8. Implementation and migration impact

- **Implementation state:** Not started as a suite-wide showcase convention
- **Public API impact:** None
- **Serialized data impact:** None
- **Migration/upgrade impact:** None
- **Removal/reinstall impact:** Showcase content is project-owned and removable independently of package Runtime
- **Workshop/setup impact:** No new Workshop automation is authorized. A later generator/preset requires separate evidence and authority.

---

## 9. Evidence and validation plan

| Evidence | Required result | Current status |
|---|---|---|
| Research | Existing suite boundaries support project-owned consumer proof | Complete |
| Prototype | First Light Reference Showcase works without test-only/hidden APIs | Not run |
| Automated tests | Existing package regression remains green after showcase work | Not run |
| Laboratory | Existing Standalone Lab remains independent and unchanged in purpose | First Light FL-M5-07 passed; post-showcase regression pending |
| Real-project integration | Clean consumer project reproduces the Reference Showcase happy path | Not run |

---

## 10. Security, privacy, licensing, cost, and provider impact

No new security, privacy, provider, or service dependency is introduced. Showcase art and media must have redistribution/use rights appropriate to the workspace and any later public distribution. Internal-only content must not be accidentally promoted into package samples.

---

## 11. Removal, reversal, and supersession plan

Reference Showcase content can be removed from the integration/development workspace without removing the package. Reversing the suite-wide requirement requires a new superseding suite ADR and corresponding SFGSS-000/SFGSS-001/SFGSS-004 updates.

---

## 12. Review triggers

- Three implemented package Reference Showcases reveal a better common anatomy.
- the Suite Showcase Hub begins implementation.
- A package cannot create a meaningful consumer-style reference surface without violating independence.
- External beta feedback shows the showcase workflow does not match real consumer usage.
- The Workshop proposes automation for Reference Showcase scaffolding.

---

## 13. Approval record

**Decision:** ACCEPTED
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** 2026-08-08
**Conditions:** First Light is the first implementation proof. Do not create Gallery runtime authority or Showcase-generation tooling without a later checkpoint/decision.

---

## 14. Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001]]
- [[SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard|SFGSS-004]]
- [[Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification|First Light]]
- [[Current Notes]]
