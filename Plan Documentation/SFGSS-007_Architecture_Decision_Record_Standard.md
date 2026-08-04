# SFGSS-007 — Architecture Decision Record Standard and Decision Register

**Document ID:** SFGSS-007  
**Version:** 1.0.0  
**Status:** Approved documentation and governance standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000  
**Related standards:** SFGSS-001 through SFGSS-006  
**Current development baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> A specification says what the system must be. An ADR preserves why a durable choice was made, what was rejected, and what would justify changing it later.

---

## 1. Purpose

This standard defines how **The Sperk’s Forge — EchoDevGames Game Systems Suite** records durable architectural decisions.

It provides:

- The test for deciding whether an ADR is required.
- ADR scopes and identifier rules.
- Required metadata and document structure.
- Proposal, approval, rejection, withdrawal, revision, and supersession lifecycles.
- The distinction between accepted design and executed evidence.
- Rules for coordinating ADRs with SFGSS-000, package specifications, integration specifications, standards, Current Notes, tests, and implementation checkpoints.
- A reusable ADR template.
- A current suite decision register.
- Obsidian graph and handoff requirements.

An ADR is a durable reasoning record. It is not a substitute for the higher-authority document that owns the approved architecture.

---

## 2. Authority and conflict rule

The documentation hierarchy remains:

1. SFGSS-000 — suite-wide authority.
2. Approved package specifications.
3. Accepted ADRs and integration specifications.
4. Standards, checkpoint plans, setup guides, test reports, and release records.
5. Current Notes.

An ADR may explain or propose a change to a Level 1 or Level 2 authority, but it does not silently overrule that authority.

When an accepted decision changes suite or package truth:

1. Update the affected higher-authority document in the same checkpoint.
2. Record the reasoning and alternatives in the ADR.
3. Update integration matrices, standards, tests, guides, or roadmaps affected by the choice.
4. Reconcile Current Notes.
5. Commit the decision and authority updates together when practical.

If an ADR and a higher-authority document disagree because the authority update was missed, stop and repair the documentation before implementation continues.

---

## 3. When an ADR is required

Create an ADR when an approved choice does one or more of the following:

- Changes a package’s authority or explicit non-goals.
- Adds, removes, or reverses a hard dependency.
- Introduces a shared contracts package, provider SDK, native plugin, cloud service, platform service, or mandatory backend.
- Selects a production provider after research or prototype evidence.
- Changes public API compatibility, serialized identity, durable file format, migration policy, or public asset identity.
- Changes persistent-root ownership, initialization order, shutdown policy, scene topology, or cross-package lifecycle.
- Creates or changes a cross-package bridge protocol used by more than one integration.
- Changes the suite’s repository, licensing, distribution, versioning, support, security, privacy, or compatibility policy.
- Changes the implementation gate, learning workflow, release gate, or evidence standard.
- Reverses or materially replaces an accepted ADR.
- Accepts a long-lived tradeoff whose consequences future maintainers must understand.
- Makes a decision that would be expensive to rediscover from code or Git history alone.

A decision may begin in Current Notes as a `[PROPOSAL]`. It becomes binding only after the owning authority and, when required, an accepted ADR are updated.

---

## 4. When an ADR is not required

Do not create an ADR solely for:

- A routine implementation detail already permitted by the approved specification.
- A bug fix that restores documented behavior without changing the contract.
- File lists, Editor steps, or test instructions for one checkpoint.
- A test result, defect reproduction, benchmark, or release note.
- Editorial cleanup, spelling correction, or link repair that changes no meaning.
- Project-owned content, tuning values, art, narrative, level layout, or one game’s local rule.
- A temporary experiment that has not been accepted as architecture.
- A proposal that is still being explored and has no approved consequence.
- A package-local refactor that preserves public API, data, lifecycle, dependencies, and evidence expectations.

Use the package specification, checkpoint plan, test report, issue record, research record, guide, changelog, or Current Notes instead.

---

## 5. ADR scopes

| Scope | Purpose | Primary location | Identifier pattern |
|---|---|---|---|
| Suite | Changes or explains suite-wide architecture, governance, workflow, or shared protocol | Central Sperk’s Forge documentation repository | `SFGSS-ADR-###` |
| Package | Records a durable choice owned by one package and not by the full suite | Package repository `Documentation~/Developer/ADR/` | `<TechnicalPackage>-ADR-###` |
| Integration | Records a durable choice for one bridge or multi-package workflow | Integration/bridge repository or central integration documentation | `SFGSS-INT-ADR-###` |
| Provider | Selects or constrains a backend, SDK, platform, hosting, or vendor adapter | Owning package/provider repository; centrally indexed when distributed | Package or integration ADR sequence |
| Project-local | Records a game-specific architecture choice that does not change a reusable package | Game repository | Project-defined ADR sequence |

Examples:

```text
SFGSS-ADR-004
EchoLaunch-ADR-001
SFGSS-INT-ADR-001
```

Exact package technical names and reserved terms are governed by SFGSS-008 once approved.

---

## 6. Identifier and filename rules

1. ADR IDs are permanent and never reused.
2. Suite IDs use three digits and one central ascending sequence.
3. A draft may remain unnumbered. Allocate the durable ID when the decision becomes **Proposed** and enters the register.
4. Rejected or withdrawn proposals retain their IDs and log entries.
5. A superseded ADR retains its original file, title, history, and links.
6. Filenames use:

```text
<ID>_<Short_Descriptive_Title>.md
```

Example:

```text
SFGSS-ADR-004_Networking_Provider_Selection.md
```

7. Renaming a title does not change the ADR ID.
8. The next available suite ADR ID after this checkpoint is **SFGSS-ADR-004**.

---

## 7. Decision status lifecycle

| Status | Meaning | Binding? |
|---|---|---:|
| Draft | Working text that has not entered formal review | No |
| Proposed | Numbered decision under review; appears in the decision register | No |
| Accepted | Approved decision; affected authorities must agree with it | Yes |
| Rejected | Considered and deliberately not selected | No |
| Withdrawn | Proposal removed by its owner before acceptance | No |
| Superseded | Formerly accepted decision replaced by a later accepted ADR | Historical only |

Status is different from feature lifecycle labels such as Proposed, Experimental, Implemented, or Deprecated. ADR status describes the decision record; package lifecycle labels describe product capabilities.

### 7.1 Allowed transitions

```text
Draft -> Proposed
Proposed -> Accepted
Proposed -> Rejected
Proposed -> Withdrawn
Accepted -> Superseded
```

An accepted ADR is not changed to Rejected. A later ADR supersedes it.

---

## 8. Evidence maturity

An accepted ADR may still describe a pre-code design. Every ADR therefore records evidence maturity separately from decision status.

| Evidence maturity | Meaning |
|---|---|
| Design approved; evidence pending | Accepted architecture with no executed implementation proof yet |
| Research supported | Supported by dated source research, but not by a working suite implementation |
| Prototype supported | Disposable or experimental prototype evidence exists |
| Implementation validated | Implemented and passed its defined package or integration tests |
| Production validated | Used successfully in a released or real-project integration with retained evidence |
| Not applicable | Decision concerns documentation/governance and does not require runtime proof |

Never translate **Accepted** into “tested,” “fast,” “compatible,” or “production ready” unless the evidence field supports that claim.

---

## 9. Required metadata

Every new ADR must state:

| Field | Requirement |
|---|---|
| Document ID | Permanent ADR ID |
| Title | Concise decision subject |
| Status | Draft, Proposed, Accepted, Rejected, Withdrawn, or Superseded |
| ADR version | Semantic document revision |
| Decision date | Date accepted, rejected, or withdrawn; blank while only Proposed |
| Last reviewed | Latest meaningful review date |
| Owner | Person responsible for the decision |
| Decision scope | Suite, package, integration, provider, or project |
| Evidence maturity | Separate from status |
| Parent authorities | Higher documents that constrain the choice |
| Affected documents | Authorities, matrices, standards, guides, or tests changed with the decision |
| Supersedes | Earlier ADR or “None” |
| Superseded by | Later ADR or “None” |
| Review triggers | Conditions that require reconsideration |
| Related evidence | Research, prototypes, tests, issues, or release records |

Jesse “Echo” Adams / EchoDevGames is the approval authority for suite ADRs. ChatGPT or another collaborator may research, draft, compare, and recommend; it does not silently approve a suite decision.

---

## 10. Required ADR structure

Every ADR must include these sections. A section may state **Not applicable** with a reason, but it must not disappear silently.

1. **Context and problem**
2. **Decision drivers and constraints**
3. **Options considered**
4. **Decision**
5. **Rationale**
6. **Consequences**
7. **Authority and document impact**
8. **Implementation and migration impact**
9. **Evidence and validation plan**
10. **Security, privacy, licensing, cost, and provider impact**
11. **Removal, reversal, and supersession plan**
12. **Review triggers**
13. **Approval record**
14. **Graph navigation**

The reusable file `Architecture Decision Records/SFGSS-ADR-TEMPLATE.md` supplies the full writing shell.

---

## 11. Context and problem rules

The context must explain:

- What has become difficult, ambiguous, risky, incompatible, or expensive.
- Which package or authority currently owns the concern.
- Why existing specifications, standards, or checkpoints are insufficient.
- Which facts are observed and which are assumptions.
- Which evidence is available and which remains `Not run`.

Do not write context as a sales pitch for the preferred answer. A future reader must understand the problem even if the decision is later reversed.

---

## 12. Options and decision drivers

List realistic alternatives, including “do nothing” when meaningful.

Evaluate options against named drivers such as:

- Package independence
- Authority clarity
- Compile dependency direction
- Removal behavior
- Data compatibility and migration
- Performance and allocation risk
- Unity/platform support
- Licensing and cost
- Security and privacy
- Testing and diagnostics
- Novice usability
- Advanced extensibility
- Repository/versioning burden
- Provider lock-in
- Learning and maintenance cost

Do not invent measured superiority. Pre-code comparisons must use bounded design reasoning or cited research and retain evidence-pending labels.

---

## 13. Decision statement rules

The decision must be explicit enough that a maintainer can identify:

- What is approved.
- What is prohibited.
- What remains optional.
- What remains unknown.
- Which authority owns the resulting behavior.
- Which package or document must change.
- Whether implementation is authorized or still gated.

Avoid vague conclusions such as “use the best approach,” “support modularity,” or “prefer clean code.”

---

## 14. Consequences

Record positive and negative consequences separately.

At minimum, consider:

- New dependencies or assemblies
- Public API effects
- Data and migration effects
- Setup and authoring effects
- Test and Laboratory work
- Removal order
- Platform/provider constraints
- Documentation maintenance
- Future flexibility lost or gained
- Operational cost or support burden

A tradeoff hidden from the consequences section is a decision debt waiting in a trench coat.

---

## 15. Authority and document impact

Each ADR includes a table like:

| Document/artifact | Required action | Status |
|---|---|---|
| SFGSS-000 | Update decision/boundary or Not applicable | `<STATUS>` |
| Package specification | Update public contract or Not applicable | `<STATUS>` |
| Integration matrix | Update bridge/workflow or Not applicable | `<STATUS>` |
| Standard | Update rule or Not applicable | `<STATUS>` |
| Current Notes | Reconcile proposal and handoff | `<STATUS>` |
| Tests/research | Add evidence plan or Not applicable | `<STATUS>` |

An ADR checkpoint is incomplete while a required higher-authority update remains merely promised.

---

## 16. Implementation and migration impact

State whether the decision affects:

- Existing code or serialized assets.
- Public types or assemblies.
- Stable IDs or file formats.
- Upgrade and downgrade behavior.
- Setup, repair, or migration tooling.
- Existing projects or generated Workshop receipts.
- Package removal or reinstall.

If implementation has not started, write **Not implemented; evidence pending** rather than inventing migration success.

---

## 17. Revision versus supersession

### 17.1 Revise the same ADR when

- Correcting spelling, links, or metadata.
- Clarifying wording without changing the decision.
- Adding new compatible registry entries under the same approved protocol.
- Recording new evidence that confirms the same decision.
- Expanding consequences or examples without changing ownership or prohibition.

Use semantic document versions:

- Patch: editorial correction or link repair.
- Minor: compatible clarification, registry extension, or added evidence.
- Major: use only when the record’s structure changes but the decision remains recognizably the same. Prefer a superseding ADR when the decision itself changes.

### 17.2 Create a new superseding ADR when

- Reversing the selected option.
- Changing authority ownership.
- Replacing a provider or mandatory backend.
- Removing a dependency previously approved as required.
- Changing a public protocol incompatibly.
- Accepting an alternative previously rejected.
- Changing the fundamental tradeoff or prohibited behavior.

The new ADR states `Supersedes: <ID>`. The old ADR becomes `Superseded` and links forward. History stays visible.

---

## 18. Review triggers

Every accepted ADR defines concrete triggers, such as:

- A third package needs the same shared contract.
- A Unity API or provider reaches end of support.
- Prototype evidence contradicts the design assumption.
- Licensing, pricing, or platform support changes.
- A migration or removal test fails.
- A package’s authority boundary changes.
- A learning review exposes persistent confusion.
- Performance exceeds a stated threshold.
- A provider adapter becomes mandatory for the core promise.

A review trigger does not automatically reverse the decision. It requires reassessment and, when necessary, a new ADR.

---

## 19. Decision register rules

`Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log.md` is the central navigation and status index.

The register must:

- List every suite ADR, including Proposed, Rejected, Withdrawn, and Superseded records.
- Record current version, status, evidence maturity, scope, and review trigger.
- Link package and integration ADRs that materially affect the suite.
- State the next available suite ADR number.
- List decision candidates without allocating IDs prematurely.
- Update in the same checkpoint as any ADR status or version change.

The register is an index, not a replacement for the ADR text.

---

## 20. Current suite decision register

| ID | Title | Status | Version | Scope | Evidence maturity | Decision date | Supersedes |
|---|---|---|---:|---|---|---|---|
| SFGSS-ADR-001 | Suite Package Editor Setup Facade Protocol | Accepted | 1.2.0 | Suite integration/workflow | Design approved; evidence pending | 2026-08-03 | None |
| SFGSS-ADR-002 | Full Suite Documentation Gate and Learning-Oriented Implementation | Accepted | 1.0.0 | Suite governance/workflow | Not applicable to runtime evidence | 2026-08-03 | None; supersedes immediate-activation language in SFGSS-000 decision 40 |
| SFGSS-ADR-003 | Graph Roadmap and Pre-Implementation Package Learning Review | Accepted | 1.0.0 | Suite documentation/learning | Not applicable to runtime evidence | 2026-08-04 | None |

**Next available suite ADR:** `SFGSS-ADR-004`

No suite ADR is currently Proposed, Rejected, Withdrawn, or Superseded.

---

## 21. Decision candidates without allocated IDs

The following subjects may require future ADRs. They are not approved decisions and do not reserve numbers.

| Candidate | Trigger before proposal | Current state |
|---|---|---|
| First EchoMultiplayer production provider | At least two disposable comparison prototypes and license/cost/security review | Research plan approved; prototypes `Not run` |
| EchoControllers package-family split | Third materially distinct backend/family proves separate dependencies or release cadence | One modular package remains approved for MVP |
| First production AI navigation or behavior provider | Adapter prototype and package/license/compatibility evidence | Candidates only |
| Observatory native hardware-sensor provider | Platform/provider research and privacy/security review | Deferred |
| Suite licensing model | Distribution and contribution policy decision before public stable releases | Open |
| Change to public Unity support floor | Compatibility evidence across candidate Unity versions | Unity 6000.0 remains planned floor |
| New shared contracts package | At least three independent packages demonstrate a truly neutral repeated contract | No mandatory shared core approved |

---

## 22. Package and integration ADR indexing

When package repositories exist:

- Each package maintains `Documentation~/Developer/ADR/` and a local decision log.
- Package ADRs link to the package specification, Current Notes, checkpoints, tests, and relevant suite standards.
- A package ADR that changes a suite boundary must also create or update a suite ADR and SFGSS-000.
- Integration ADRs live with the bridge/integration artifact and link both package specifications.
- The central suite log links externally distributed ADRs whose decisions affect compatibility, selection pathways, or integration work.
- Package or integration ADR numbers are never converted into suite ADR numbers.

---

## 23. Obsidian graph requirements

Every ADR must link to:

- `[[Suite_Graph_Roadmap]]`
- `[[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]` for suite-relevant decisions
- Its affected package specifications or integration matrices
- Relevant research, tests, and Current Notes

The decision log links back to every ADR. Superseded and superseding ADRs link directly to one another.

Use links to expose the decision path, not decorative tag confetti.

---

## 24. Review and approval workflow

1. Capture the question and evidence in Current Notes.
2. Decide whether an ADR is required using Sections 3 and 4.
3. Draft without a durable ID while the options are still shapeless.
4. Allocate the next ID when entering Proposed review.
5. Add the proposal to the decision log.
6. Review parent authorities, package specifications, matrices, standards, research, and evidence.
7. Record realistic alternatives and consequences.
8. Jesse accepts, rejects, or withdraws the decision.
9. Update affected higher-authority documents in the same checkpoint.
10. Update the decision log, graph roadmap, Current Notes, tests, and handoff state.
11. Commit and push the complete decision checkpoint.

An ADR must not be marked Accepted merely because a draft sounds convincing.

---

## 25. Completion checklist

- [ ] ADR requirement test completed.
- [ ] Correct scope and identifier used.
- [ ] Status and evidence maturity are separate.
- [ ] Context distinguishes facts, assumptions, and `Not run` evidence.
- [ ] Real alternatives are documented.
- [ ] Decision, prohibitions, unknowns, and owner are explicit.
- [ ] Positive and negative consequences are recorded.
- [ ] Higher-authority documents are updated where required.
- [ ] Migration, removal, and reversal effects are addressed.
- [ ] Review triggers are concrete.
- [ ] Supersession links are complete when applicable.
- [ ] Decision log and Obsidian links are updated.
- [ ] Current Notes is reconciled.
- [ ] Checkpoint is committed and pushed.

---

## 26. Graph Navigation

#sfgss/standard #sfgss/adr #sfgss/governance

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]
- [[Architecture Decision Records/SFGSS-ADR-TEMPLATE|Reusable ADR Template]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
