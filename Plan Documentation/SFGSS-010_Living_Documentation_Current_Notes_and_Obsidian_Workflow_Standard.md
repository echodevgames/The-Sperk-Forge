# The Sperk’s Forge – Living Documentation, Current Notes, and Obsidian Workflow Standard

**Document ID:** SFGSS-010  
**Version:** 1.1.0  
**Status:** Approved living-documentation and knowledge-navigation standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.21.0  
**Related authorities:** SFGSS-001 through SFGSS-009, SFGSS-ADR-001 through SFGSS-ADR-003, the Foundation, Expansion, and Advanced integration matrices, and approved package specifications  
**Current development baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> Keep the workbench clear, the map legible, and the durable truth in the document that owns it.

### Revision history

| Version | Date | Status | Summary |
|---|---|---|---|
| 1.0.0 | August 4, 2026 | Approved | Initial living-documentation, Current Notes, Obsidian, and handoff standard |
| 1.1.0 | August 4, 2026 | Approved | Added the canonical full-suite handoff guide and Learning Reviews folder, template, tracker, artifact naming, and review-handoff scan order |

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Approved documentation topology
6. Canonical document classes
7. Canonical entry points and scan order
8. Authority, status, and evidence labels
9. Canonical filenames and single-live-copy rule
10. Document metadata and visible headers
11. Markdown and cross-surface compatibility
12. Internal links and relationship types
13. Obsidian tags, backlinks, and Graph View
14. Navigation hubs and maps of content
15. Current Notes authority boundary
16. Required Current Notes structure
17. Current Notes entry labels
18. Current Notes entry anatomy
19. Active-note ordering and size discipline
20. Promotion lifecycle
21. Promotion routing matrix
22. Questions, proposals, and decisions
23. Tests, bugs, risks, and research findings
24. Checkpoint closeout workflow
25. Handoff snapshots and fresh-conversation recovery
26. README and repository index responsibilities
27. Central suite versus package documentation
28. User documentation versus development documentation
29. Obsidian configuration and shared settings
30. Images, diagrams, attachments, and exports
31. Git commits, pull requests, and documentation adjacency
32. Merge conflicts, reviews, and concurrent editing
33. Stale documents, broken links, and duplicate detection
34. Archival, compaction, and deletion
35. Security, privacy, and sensitive information
36. Templates and reusable forms
37. Validation and release gates
38. Reconciliation findings
39. Approval

---

## 1. Purpose and authority

SFGSS-010 is the canonical living-documentation, Current Notes, repository-vault, Obsidian-navigation, link, promotion, handoff, and documentation-closeout standard for **The Sperk’s Forge – EchoDevGames Game Systems Suite**.

The suite deliberately treats documentation as part of the product. That only works when the active notes remain distinguishable from approved truth, navigation does not become authority, Git history remains trustworthy, and a fresh collaborator can recover the current state without reconstructing it from a ChatGPT transcript.

This standard answers:

- Where does each kind of documentation live?
- Which documents are authoritative, working, navigational, historical, or evidentiary?
- What belongs in `Current Notes.md`?
- How does a note become an approved decision, test record, issue, guide, or release fact?
- How are Obsidian links, tags, graphs, and workspace settings handled without making Obsidian a runtime dependency?
- How are checkpoint handoffs kept accurate and compact?
- How are duplicate files, stale links, copied vaults, and version-suffixed authorities prevented?
- What must be committed before a documentation or implementation checkpoint can close?

### 1.1 Authority order

When documentation-workflow sources disagree, use this order:

1. SFGSS-000 for suite-wide authority and documentation-as-code principles.
2. Approved package specifications for package-local documentation obligations.
3. This standard for vault structure, Current Notes, links, navigation, promotion, handoff, and archival behavior.
4. SFGSS-005 for checkpoint execution and learning-oriented implementation.
5. SFGSS-007 for ADR creation, status, revision, and supersession.
6. SFGSS-009 for repository ownership, Git history, versions, tags, releases, and compatibility snapshots.
7. Approved integration specifications, test reports, research records, guides, roadmaps, and Current Notes.

A convenience in Obsidian, GitHub, ChatGPT, or a generated checkpoint archive must never override the document that owns the decision.

### 1.2 Requirement language

- **Must** is checkpoint or release blocking.
- **Must not** is prohibited unless a higher authority or accepted ADR grants an explicit exception.
- **Should** is the default; deviations require a recorded reason.
- **May** is optional.

---

## 2. Scope and non-goals

### 2.1 This standard governs

- The central `Plan Documentation/` vault.
- Package-repository development documentation and `Documentation~` boundaries.
- Canonical entry points, folders, filenames, links, tags, and navigation hubs.
- `Current Notes.md` structure, labels, ordering, promotion, compaction, and handoff.
- Document headers, status language, evidence language, and update dates.
- Obsidian Graph View, local graphs, backlinks, Mermaid diagrams, and shared configuration.
- Documentation attachments, generated exports, checkpoint archives, and duplicate handling.
- Documentation commits, review, merge conflict resolution, stale-document checks, and closeout gates.
- Fresh ChatGPT and collaborator handoff order.

### 2.2 This standard does not govern

- Package runtime APIs or behavior.
- Package source-code style.
- Git branches, tags, package releases, or repository protection, which belong to SFGSS-009.
- Test execution states and release evidence, which belong to SFGSS-004.
- ADR reasoning and supersession, which belong to SFGSS-007.
- A mandatory Obsidian plugin, theme, or personal workspace layout.
- Publishing private planning notes to a public repository without review.
- Treating ChatGPT history as an authoritative archive.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Living documentation** | Documentation updated alongside design, implementation, testing, and release work rather than reconstructed afterward. |
| **Vault** | The repository folder opened directly in Obsidian. The repository files, not an Obsidian copy, are authoritative. |
| **Current Notes** | The fast, non-authoritative capture page for active observations, questions, proposals, tests, bugs, risks, decisions awaiting promotion, and handoff context. |
| **Promotion** | Moving durable information from Current Notes into the authority or permanent record that owns it. |
| **Reconciliation** | Reviewing working notes, statuses, links, evidence, and authorities so they agree at checkpoint closeout. |
| **Navigation hub** | A README, Graph Roadmap, health check, catalog, decision log, or map-of-content note that helps readers find authoritative documents. |
| **Map of content (MOC)** | A curated link hub for one domain or workflow. It is navigation, not authority. |
| **Canonical live file** | The one current repository file representing an active authority or hub. Its version lives in the document header, not in a filename suffix. |
| **Checkpoint report** | A durable record of a checkpoint’s scope, findings, evidence state, repairs, and gate decision. |
| **Handoff snapshot** | A concise current-state record naming the completed checkpoint, active checkpoint, blockers, evidence state, and stop point. |
| **Graph edge** | An internal link that creates an Obsidian backlink and Graph View relationship. |
| **Workspace state** | Device-specific Obsidian UI state such as open panes, recent files, window layout, and personal appearance choices. |
| **Checkpoint archive** | A transport ZIP containing canonical repository paths for one documentation checkpoint. It is not a second source of truth. |
| **Compaction** | Removing or condensing already-promoted working notes while relying on Git history and permanent records for detail. |

---

## 4. Governing principles

### 4.1 One repository-backed vault

The repository documentation folder is opened directly in Obsidian. A second copied vault is prohibited because it creates competing edits and uncertain history.

### 4.2 One current authority file

Every active authority, standard, package specification, hub, and Current Notes page has one canonical live file. Git preserves earlier versions.

### 4.3 Current Notes is a workbench

Current Notes captures work quickly. It is not a permanent warehouse, decision register, issue tracker, test archive, changelog, or release record.

### 4.4 Promote durable truth

A durable architectural, behavioral, setup, testing, migration, compatibility, or release fact belongs in the document that owns it. Current Notes records where it was promoted.

### 4.5 Navigation is not authority

README files, Graph Roadmaps, catalogs, dashboards, and health checks may summarize or link truth. They must point to the owning document and must not invent a competing rule.

### 4.6 Git is the historical archive

Resolved working notes may be removed after promotion. Git history, checkpoint reports, ADRs, test reports, research records, changelogs, and release records preserve the durable history.

### 4.7 Cross-surface readability

The documentation should remain understandable in Obsidian, GitHub, a plain-text editor, and a fresh ChatGPT handoff. No essential rule may exist only in a plugin-rendered view.

### 4.8 Evidence remains honest

A planning note, checklist, graph, or generated report does not prove implementation. Unexecuted evidence remains `Not run` under SFGSS-004.

### 4.9 Small active surfaces

Active hubs and handoffs should remain compact enough to scan. Long historical detail belongs in permanent records or Git history.

### 4.10 Documentation and implementation close together

When behavior changes, its specification, guide, test record, issue status, and changelog change in the same commit when practical or in an immediately adjacent documentation commit.

---

## 5. Approved documentation topology

### 5.1 Central suite vault

```text
Plan Documentation/
├── README.md
├── Current Notes.md
├── Echo_Game_Systems_Suite_Bible.md
├── Full_Suite_Documentation_Program_Roadmap.md
├── Foundation_Wave_Specification_Roadmap.md
├── Suite_Graph_Roadmap.md
├── Suite_Health_Check_and_Remaining_Documentation.md
├── Full_Suite_Documentation_and_Learning_Handoff_Guide.md
├── Package_Learning_Review_Catalog.md
├── Learning Reviews/
│   ├── README.md
│   ├── PKG-LEARN-TEMPLATE.md
│   └── PKG-LEARN-TRACKER.json
├── SFGSS-001_Package_Specification_Template.md
├── SFGSS-002_Dependency_Bridge_and_Assembly_Standard.md
├── SFGSS-003_Data_IDs_Serialization_and_Migration_Standard.md
├── SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard.md
├── SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules.md
├── SFGSS-006_New-Project_Guided_Pathways.md
├── SFGSS-007_Architecture_Decision_Record_Standard.md
├── SFGSS-008_Suite_Glossary_and_Naming_Registry.md
├── SFGSS-009_Repository_Versioning_and_Integration_Workspace_Standard.md
├── SFGSS-010_Living_Documentation_Current_Notes_and_Obsidian_Workflow_Standard.md
├── Package Specifications/
├── Architecture Decision Records/
├── Integration Specifications/
├── Checkpoint Build Plans/
├── Research Records/
├── Test Reports/
├── Release Records/
└── Documentation Assets/        # create only when the first real asset exists
```

### 5.2 Package repository development documentation

A package repository should expose:

```text
README.md
Documentation~/
├── Index.md
├── User/
├── Developer/
│   ├── Architecture.md
│   ├── Current Notes.md
│   ├── ADR/
│   ├── Checkpoints/
│   ├── Tests/
│   └── Migration/
└── Images/                      # only when needed
```

The exact subfolders may be delayed until their first real document exists. Empty directories are not created for decoration.

### 5.3 Integration Lab documentation

The Integration Lab records exact package sources, compatibility snapshots, bridge/provider setup, pathway fixtures, test evidence, and removal results. It does not copy package specifications as independent authorities.

---

## 6. Canonical document classes

| Class | Purpose | Authority/evidence role | Normal location |
|---|---|---|---|
| Suite Bible | Suite-wide authority | Level 1 authority | Vault root |
| Standard | Cross-suite operating rule | Approved authority under Bible | Vault root |
| Package specification/foundation | Package-local design authority | Level 2 authority | `Package Specifications/` |
| ADR | Durable reasoning and decision history | Decision record; updates higher authority when needed | `Architecture Decision Records/` |
| Integration specification | Cross-package contract | Approved integration authority | `Integration Specifications/` |
| Checkpoint Build Plan | Bounded implementation plan | Approved execution plan | `Checkpoint Build Plans/` |
| Research record | Dated investigation | Research evidence; not implementation proof | `Research Records/` |
| Test report | Executed or static audit evidence | Evidence record | `Test Reports/` |
| Release record | Versioned distribution evidence | Release history | `Release Records/` |
| Guide | Setup, use, troubleshooting, migration | User/developer instruction | Package `Documentation~` or approved central guide |
| README/index | Entry point and routing | Navigation only | Repository/folder root |
| Graph Roadmap/MOC | Visual and linked navigation | Navigation only | Vault root or domain folder |
| Full Suite Handoff Guide | Fresh-collaborator recovery and learning-phase entry | Approved guidance; navigation only | Vault root |
| Package learning review | Educational understanding record and teach-back | Learning evidence; not implementation evidence | `Learning Reviews/` |
| Current Notes | Fast active capture and handoff | Working context only | Repository documentation root |
| Changelog | User-visible released changes | Release record | Package root |

A document must not claim two incompatible roles. A navigation hub may summarize an authority but must link to it.

---

## 7. Canonical entry points and scan order

### 7.1 Central suite repository

A new collaborator or ChatGPT conversation reads:

1. `README.md`
2. `Full_Suite_Documentation_and_Learning_Handoff_Guide.md`
3. `Suite_Graph_Roadmap.md`
4. `Suite_Health_Check_and_Remaining_Documentation.md`
5. `Echo_Game_Systems_Suite_Bible.md`
6. `Current Notes.md`
7. `Full_Suite_Documentation_Program_Roadmap.md`
8. Applicable standards and ADRs
9. Active package, integration, research, test, checkpoint, or learning-review documents

### 7.2 Package repository

1. Repository README
2. Package specification
3. `Documentation~/Index.md`
4. Package `Current Notes.md`
5. Active checkpoint
6. Applicable ADRs, tests, migration notes, and implementation

### 7.3 Integration Lab

1. README and current compatibility snapshot
2. Exact package manifest and lock file
3. Integration specification
4. Current Notes
5. Test plan and latest retained evidence
6. Known issues and removal instructions

A handoff prompt may name this order, but it must not require an old conversation transcript.

---

## 8. Authority, status, and evidence labels

### 8.1 Authority labels

Documents state their role explicitly:

- Authority
- Decision record
- Integration authority
- Execution plan
- Evidence record
- Navigation only
- Working context only
- User/developer guidance

### 8.2 Design lifecycle labels

Use the suite lifecycle labels from SFGSS-000 and SFGSS-001:

- Proposed
- Approved
- In Development
- Implemented
- Deferred
- Experimental
- Deprecated
- Removed

### 8.3 Evidence labels

Use SFGSS-004 states exactly:

- Not run
- Pass
- Pass with advisory
- Fail
- Blocked
- Not applicable

A document’s approval status and its evidence status are separate.

---

## 9. Canonical filenames and single-live-copy rule

### 9.1 Current authority filenames

The current repository filename remains stable while the version changes inside the document header.

Correct:

```text
Echo_Game_Systems_Suite_Bible.md
SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules.md
Package Specifications/SFGSS-The-Ascent-EchoProgression-Package-Specification.md
```

Incorrect inside the repository:

```text
Echo_Game_Systems_Suite_Bible_v0.19.0.md
Current Notes - Copy.md
SFGSS-005_FINAL_FINAL.md
EchoProgression-Spec-2026-08-04.md
```

### 9.2 Versioned reports and immutable records

Checkpoint reports, release records, evidence captures, and intentionally immutable research snapshots may include stable checkpoint IDs or dates because they are distinct records rather than replacement copies.

### 9.3 External checkpoint archives

A downloadable ZIP may include a checkpoint and date in its filename. Inside the ZIP, files use canonical repository paths and current filenames.

### 9.4 Duplicate handling

When duplicate current authorities appear:

1. Stop edits.
2. Identify the repository-tracked canonical path.
3. Compare content and Git history.
4. Merge supported changes into the canonical file.
5. Delete the duplicate.
6. Record the repair in Current Notes and the checkpoint report.

---

## 10. Document metadata and visible headers

### 10.1 Authority documents

Standards, specifications, ADRs, integration specifications, and formal reports expose visible metadata near the top:

- Document ID
- Version when versioned
- Status
- Owner
- Parent or related authority
- Project boundary when relevant
- Current baseline when relevant
- Last updated date

### 10.2 Navigation notes

Navigation hubs may use YAML frontmatter for Obsidian tags, status, and update date. Essential role and authority text must also remain visible in the document body.

### 10.3 Current Notes

Current Notes exposes:

- Document role
- Authority boundary
- Owner
- Last reconciled date
- Current focus
- Active checkpoint

### 10.4 No false precision

A date, version, status, commit, environment, or test result is recorded only when known. Unknown values are marked `Unknown`, `Planned`, or `Not run` rather than invented.

---

## 11. Markdown and cross-surface compatibility

### 11.1 Base format

Documentation uses ordinary UTF-8 Markdown. Essential content must remain readable without Obsidian plugins.

### 11.2 Relative Markdown links

New critical navigation links should use relative Markdown links when practical because both GitHub and Obsidian understand them:

```markdown
[Suite Bible](Echo_Game_Systems_Suite_Bible.md)
[First Light specification](Package%20Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification.md)
```

### 11.3 Wikilinks

Obsidian `[[wikilinks]]` remain approved for Graph View, aliases, and existing navigation blocks. They must target one canonical file. Critical repository entry points should remain understandable even when a renderer does not resolve wikilinks.

Existing wikilinks are not mass-rewritten during this checkpoint. SUITE-DOC-30 will normalize critical cross-surface links where useful.

### 11.4 Mermaid

Mermaid diagrams may visualize ownership, lifecycle, or roadmaps. The surrounding prose and tables must still state the authoritative rule.

### 11.5 Unsupported plugin syntax

An Obsidian plugin may enhance private authoring, but no mandatory rule, status, checklist, or API contract may exist only inside plugin-specific syntax.

---

## 12. Internal links and relationship types

Every meaningful document should link to the documents that explain its context.

Recommended relationship labels include:

- Parent authority
- Implements
- Refines
- Integrates with
- Supersedes
- Superseded by
- Evidence for
- Tested by
- Blocks
- Depends on
- Related research
- Current checkpoint
- Next checkpoint

Critical links should be bidirectional when the relationship matters for handoff. For example, an ADR links to the affected specification, and the specification links back to the ADR.

Absolute local filesystem paths and private connector URLs are prohibited in committed links.

---

## 13. Obsidian tags, backlinks, and Graph View

### 13.1 Tags are navigation metadata

Tags do not create authority. Use a small controlled vocabulary such as:

```text
#sfgss/authority
#sfgss/standard
#sfgss/package
#sfgss/integration
#sfgss/research
#sfgss/test
#sfgss/navigation
#sfgss/wave/foundation
#sfgss/wave/expansion
#sfgss/wave/advanced
#sfgss/status/approved
#sfgss/status/proposed
```

### 13.2 Tag discipline

- Use lowercase hierarchical tags.
- Prefer two to six useful tags over decorative tag clouds.
- Do not encode mutable implementation truth only in a tag.
- Register new suite-wide tag families in this standard or SFGSS-008.

### 13.3 Backlinks

Backlinks help discover relationships but do not replace explicit related-document sections in important authorities.

### 13.4 Graph filters

Graph View filters may be documented for convenience. Personal graph colors and visual groups are not authoritative and need not be committed.

---

## 14. Navigation hubs and maps of content

### 14.1 Required central hubs

- `README.md` – shortest entry point
- `Suite_Graph_Roadmap.md` – visual and link navigation
- `Suite_Health_Check_and_Remaining_Documentation.md` – current health and remaining gates
- `Full_Suite_Documentation_Program_Roadmap.md` – ordered checkpoint program
- `Package_Learning_Review_Catalog.md` – learning-review progress
- `SFGSS-ADR-LOG_Suite_Decision_Log.md` – ADR index

### 14.2 Hub update rule

A checkpoint updates only the hubs whose current status, link set, version, or next action changed. Hubs must not copy entire specifications.

### 14.3 MOC authority boundary

A MOC summarizes and links. When its summary conflicts with an authority, the authority wins and the MOC is repaired.

---

## 15. Current Notes authority boundary

`Current Notes.md` is the first capture surface for active work. It may contain:

- Observations
- Questions
- Proposals
- Decisions awaiting or confirming promotion
- Static or executed test findings
- Bugs and regressions awaiting permanent records
- Risks
- Handoff context
- Promotion tracking

It must not become the only home of:

- An approved architecture decision
- A package behavior contract
- A public API
- A migration rule
- A defect record that affects release
- Executed test evidence
- A setup guide
- A changelog or release note
- A compatibility claim

If a durable item has no owning document yet, create the appropriate document or mark the item as unresolved. Do not silently treat the note as approved authority.

---

## 16. Required Current Notes structure

Every active repository Current Notes page uses this minimum structure:

```text
# <Repository or Package> - Current Notes

Document metadata
How to Use This Page
Current Focus
Open Questions
Active Notes
Promotion Queue
Latest Validation Snapshot
Checkpoint Closeout Checklist
Handoff Snapshot
Graph Navigation
```

Sections with no entries remain present with `None recorded` rather than disappearing.

The canonical reusable form is `SFGSS-010_Current_Notes_Template.md`.

---

## 17. Current Notes entry labels

Use exactly these labels unless a later standard extends them:

| Label | Meaning | Required destination when durable |
|---|---|---|
| `[NOTE]` | Useful observation or context | Guide, specification, report, or remove after use |
| `[QUESTION]` | Unresolved question requiring research or approval | Open-decision table, research record, or ADR proposal |
| `[PROPOSAL]` | Suggested change not yet approved | Specification/ADR review or rejection note |
| `[DECISION]` | Approved decision awaiting/confirming promotion | Owning authority and ADR when required |
| `[TEST]` | Test result, static audit, reproduction, or evidence | Test report/evidence registry |
| `[BUG]` | Defect or regression | Issue/test record and release impact |
| `[RISK]` | Architecture, compatibility, schedule, security, or release risk | Risk register, specification, ADR, or report |
| `[HANDOFF]` | Context the next work session must see | Current Handoff Snapshot or checkpoint report |

Labels describe the entry’s present role. Promotion may change the final document class.

---

## 18. Current Notes entry anatomy

A material entry should contain:

- Date
- Label
- Concise statement
- Affected package/document/system
- Supporting evidence or source when available
- Status or next action
- Promotion destination when known

Example:

```markdown
### August 4, 2026 - Display rollback finding

- `[TEST]` The display preview timed out correctly in the Accord Laboratory fixture.
- Affected authority: The Accord specification section 8.5.
- Evidence: `ESET-T-042`, Unity 6000.3.8f1, Windows Player.
- Next action: attach execution result to the Accord test report.
```

Do not paste large raw logs, full ChatGPT transcripts, secrets, or duplicate source files into Current Notes. Link to the durable record instead.

---

## 19. Active-note ordering and size discipline

### 19.1 Ordering

- Keep the Current Focus and Handoff Snapshot current.
- Add new dated Active Notes newest-first.
- Keep Open Questions and Promotion Queue sorted by urgency or owner.
- Do not append a second current handoff while leaving an older handoff presented as current.

### 19.2 Compaction trigger

Current Notes should be reviewed for compaction when any of these occur:

- The file becomes difficult to scan in one work session.
- It exceeds roughly 750 lines or 75 KB.
- More than one completed checkpoint’s handoff remains in the active surface.
- Resolved material outnumbers active material.
- A fresh collaborator cannot identify the active checkpoint within two minutes.

These are review triggers, not automatic deletion commands.

### 19.3 Compaction method

1. Confirm entries were promoted.
2. Preserve unresolved questions and active risks.
3. Keep the current checkpoint and latest validation snapshot.
4. Remove or condense resolved entries.
5. Rely on Git history and permanent records for detail.
6. Record the compaction in the checkpoint report.

The current central `Current Notes.md` exceeds the recommended active-surface trigger. Its structural compaction is queued for SUITE-DOC-30 after this standard is approved, so the operation can be reviewed as a consistency repair rather than silently applied.

---

## 20. Promotion lifecycle

Every material working note follows:

```text
Capture
    -> classify
        -> investigate or approve
            -> identify owning document
                -> promote
                    -> verify links and status
                        -> mark destination
                            -> compact working note
```

### 20.1 Promotion states

- Pending
- In review
- Promoted
- Deferred
- Rejected
- Superseded

`Promoted` requires a concrete destination, not merely an intention.

### 20.2 Promotion timing

Promotion occurs before checkpoint closeout when the note changes:

- Authority
- Public API or behavior
- Setup or migration
- Test or compatibility evidence
- Release scope
- Known limitations
- User-facing behavior
- Security/privacy policy

Minor scratch observations may be removed when no longer useful.

---

## 21. Promotion routing matrix

| Working note content | Permanent destination |
|---|---|
| Suite-wide authority/boundary | SFGSS-000 and ADR when material |
| Package behavior/API/data/lifecycle | Package specification and ADR when material |
| Cross-package workflow | Integration specification and bridge/provider docs |
| Dependency/assembly rule | SFGSS-002 or package/bridge specification |
| Durable ID/serialization/migration | SFGSS-003 and owning package specification |
| Test result or validation evidence | Test report/execution registry under SFGSS-004 |
| Checkpoint execution detail | Checkpoint Build Plan/status record under SFGSS-005 |
| Package-selection guidance | SFGSS-006 or Workshop pathway/preset docs |
| Architectural reasoning | ADR under SFGSS-007 |
| Naming/terminology | SFGSS-008 registry |
| Repository/version/release rule | SFGSS-009 |
| Documentation/vault workflow | SFGSS-010 |
| Defect/regression | Issue/test record and affected release gate |
| Research/provider comparison | Research record and ADR only after decision |
| User setup/use/troubleshooting | User/developer guide |
| Released visible change | Changelog and release record |
| Current progress/next action | Current Notes and checkpoint status |

---

## 22. Questions, proposals, and decisions

### 22.1 Questions

A release-blocking question moves into the owning specification or ADR proposal. A non-blocking question may remain in Current Notes with an owner and review trigger.

### 22.2 Proposals

A proposal remains visibly unapproved. It must not be written into a guide, sample, setup tool, or API as though accepted.

### 22.3 Decisions

A decision becomes binding only after the owning authority is updated. An ADR is added when SFGSS-007 requires one.

### 22.4 Rejected ideas

Rejected proposals may be summarized in the specification’s deferred/rejected table or an ADR. Current Notes does not need to preserve every discarded brainstorming branch forever.

---

## 23. Tests, bugs, risks, and research findings

### 23.1 Tests

Current Notes may capture a quick result, but exact environment, test ID, execution state, evidence, and issue references belong in the permanent test report.

### 23.2 Bugs

A bug entry records symptom and impact. Release-relevant defects receive a stable issue/test reference and severity under SFGSS-004.

### 23.3 Risks

A risk identifies likelihood, impact, mitigation, owner, and trigger when material. Package risks belong in package specifications; suite risks belong in standards, ADRs, or review reports.

### 23.4 Research

Research records distinguish source facts, inference, recommendations, and unexecuted prototype claims. Current Notes links to the record rather than copying it.

---

## 24. Checkpoint closeout workflow

At every meaningful checkpoint:

1. Stop at the bounded checkpoint end.
2. Review every Current Notes entry added since the prior closeout.
3. Separate facts, evidence, proposals, decisions, and unresolved questions.
4. Promote durable information into its owning documents.
5. Update test, issue, setup, migration, changelog, release, and compatibility records as applicable.
6. Update README, Graph Roadmap, roadmap, health check, and decision log only where their current state changed.
7. Update Current Notes metadata, Current Focus, promotion queue, validation snapshot, and Handoff Snapshot.
8. Validate filenames, IDs, versions, statuses, links, evidence language, and single-live-copy rules.
9. Confirm documentation matches approved or observed truth.
10. Commit and push the checkpoint when practical before advancing.

A checkpoint is incomplete when its implementation or design changed but the handoff still points at the old state.

---

## 25. Handoff snapshots and fresh-conversation recovery

### 25.1 One current handoff

Current Notes contains one clearly marked current Handoff Snapshot. Older handoffs are checkpoint history and may be removed after promotion.

### 25.2 Required handoff fields

- Completed checkpoint
- Result/status
- Current focus
- Active checkpoint
- Relevant versions
- Completed package/standard counts
- Implementation authorization
- Evidence state
- Known blockers
- Commit/push state
- Stop point
- Next recommended document

### 25.3 Fresh ChatGPT handoff

A handoff names exact files and authority order. It does not ask a new conversation to infer architecture from raw transcript history.

### 25.4 Missing context

When a referenced document is absent, the collaborator states the gap and does not invent the missing decision.

The canonical reusable form is `SFGSS-010_Checkpoint_Handoff_Template.md`.

---

## 26. README and repository index responsibilities

A repository README must:

- Explain what the repository owns.
- Link to the canonical entry points.
- State authority order.
- Show current approved state and active checkpoint.
- State implementation/evidence gates.
- Link to Current Notes.
- Provide the current handoff prompt or route to it.
- Describe the repository structure.

A README stays concise. It routes to detail rather than becoming a shadow Bible.

---

## 27. Central suite versus package documentation

### 27.1 Central suite owns

- Suite Bible and standards
- Package catalog and pathways
- Cross-package matrices
- Suite ADR log
- Suite roadmap, graph, and health
- Central Current Notes
- Compatibility catalog links

### 27.2 Package repository owns

- Package specification copy appropriate to that repository
- User and developer documentation
- Package-local ADRs
- Package Current Notes
- Package checkpoints, tests, migration, changelog, and release records

### 27.3 No copied competing authorities

The central repository may catalog and link package releases. If an authoritative package specification is mirrored during the current documentation program, repository split work must define which copy becomes canonical before implementation. Two independently edited copies are prohibited.

---

## 28. User documentation versus development documentation

### 28.1 User-facing docs

Installation, quick start, configuration, common workflows, troubleshooting, known limitations, migration, license, and support guidance belong in package `Documentation~` and README routes.

### 28.2 Developer docs

Architecture, lifecycle, extension points, tests, ADRs, Current Notes, and checkpoints belong in developer sections or repository planning folders.

### 28.3 Private planning data

Private credentials, personal identifiers, unreleased partner information, or sensitive support data must not enter public documentation. Use redacted examples and approved private storage.

---

## 29. Obsidian configuration and shared settings

### 29.1 Approved use

Obsidian is an authoring and navigation surface over repository Markdown.

### 29.2 Shared configuration

Only settings deliberately adopted for the whole repository may be committed, such as an approved shared plugin list or link behavior. The repository currently does not require any Obsidian plugin.

### 29.3 Device-specific exclusions

Do not commit personal workspace state such as:

- Open panes and tabs
- Recent files
- Window layout
- Local hotkeys
- Personal themes
- Device paths
- Personal graph colors
- Cache files

Typical files such as `.obsidian/workspace.json` and device-specific variants should remain ignored unless an explicit ADR approves shared use.

### 29.4 Plugin safety

A plugin may assist editing but must not become necessary to interpret an authority document or build a package.

---

## 30. Images, diagrams, attachments, and exports

### 30.1 Documentation assets

Create `Documentation Assets/` or package `Documentation~/Images/` only when the first real asset exists.

### 30.2 Naming

Use descriptive stable filenames. Avoid `image1.png`, clipboard timestamps, and duplicate exports.

### 30.3 Source and rights

Record source, license, author, and editability where applicable. Do not commit assets without redistribution rights.

### 30.4 Diagrams

Prefer Mermaid for architecture that benefits from text-based review. Use image exports when visual fidelity requires them, but retain editable source when practical.

### 30.5 Large attachments

Large binaries follow SFGSS-009 and Git LFS policy. Raw logs and generated checkpoint ZIPs do not live inside the canonical vault unless explicitly adopted as release evidence.

---

## 31. Git commits, pull requests, and documentation adjacency

### 31.1 Documentation-only checkpoints

A documentation checkpoint commits all reconciled authorities, reports, navigation updates, Current Notes, and manifests together when practical.

### 31.2 Implementation checkpoints

Behavior and its documentation should share one commit. When that is impractical, use an immediately adjacent, clearly labeled documentation commit before the checkpoint closes.

### 31.3 Commit messages

Messages name the checkpoint or package and the actual outcome, for example:

```text
Docs: approve SFGSS-010 living documentation standard
First Light: add FL-M1-01 package skeleton
Docs: reconcile First Light FL-M1-01 closeout
```

### 31.4 Pull request review

A review checks both changed behavior and changed documentation. A code review that ignores stale setup, tests, migration, or known limitations is incomplete.

---

## 32. Merge conflicts, reviews, and concurrent editing

### 32.1 Current Notes conflicts

Current Notes is conflict-prone because it changes often. Resolve conflicts by preserving all still-active entries, removing duplicated promoted entries, and producing one current focus/handoff.

### 32.2 Authority conflicts

Do not choose “ours” or “theirs” mechanically for specifications, ADRs, registries, or roadmaps. Reconcile the actual authority and version history.

### 32.3 Generated companions

Machine-readable registries and manifests are regenerated from the approved source or reconciled deliberately. Do not hand-merge hashes or counts without validation.

### 32.4 Concurrent ownership

When multiple contributors edit one package, agree on the active checkpoint and document owner before starting. Avoid parallel edits to the same authority without coordination.

---

## 33. Stale documents, broken links, and duplicate detection

A documentation validation pass should check:

- Missing canonical entry points
- Duplicate current filenames
- Version-suffixed live authorities
- Broken relative links and unresolved wikilinks
- Missing backlinks for critical relationships
- Stale active-checkpoint text
- Inconsistent version/status/date metadata
- Current Notes decisions without promotion destinations
- Handoffs pointing at completed checkpoints
- Navigation summaries contradicting authorities
- `Not run` evidence accidentally promoted to Pass/Supported
- Missing Current Notes or README links
- Orphaned attachments
- Device-specific Obsidian state

Static documentation validation may pass before runtime implementation. It must not be described as runtime proof.

---

## 34. Archival, compaction, and deletion

### 34.1 Git history is the default archive

Do not create copied “archive” versions of every current file. Use Git history, tags, releases, reports, and immutable ADR/test/release records.

### 34.2 Deleting resolved Current Notes

Resolved notes may be deleted after their destination is verified. The closeout report records the compaction.

### 34.3 Superseded authorities

ADRs remain visible under SFGSS-007. Specifications and standards normally retain one current file with revision history; superseded releases remain available through Git tags/history.

### 34.4 Repository archive

Repository archival follows SFGSS-009 and preserves final status, supported versions, replacement, migration, and security contact information.

---

## 35. Security, privacy, and sensitive information

Documentation must not commit:

- Passwords, API keys, tokens, private certificates, or signing material
- Private player/support data
- Authentication tickets or multiplayer secrets
- Personal local paths when avoidable
- Unredacted crash dumps containing sensitive data
- Private legal or partner documents without approval

Use placeholders, redaction, secret stores, and private issue systems as appropriate. If sensitive data enters Git history, treat it as compromised and rotate it; deleting the visible line is not sufficient.

---

## 36. Templates and reusable forms

This checkpoint approves:

- `SFGSS-010_Current_Notes_Template.md`
- `SFGSS-010_Checkpoint_Handoff_Template.md`
- `SFGSS-010_Documentation_Registry.json`

Templates are starting points. Repositories may add package-specific sections without removing the authority boundary, promotion queue, current handoff, or closeout requirements.

---

## 37. Validation and release gates

### 37.1 Documentation checkpoint gate

- [ ] Canonical filenames are used.
- [ ] One live copy of each current authority exists.
- [ ] Metadata, status, version, and date are accurate.
- [ ] Current Notes is reconciled.
- [ ] Durable decisions and evidence are promoted.
- [ ] README and active navigation hubs agree.
- [ ] Graph links resolve or are deliberately deferred.
- [ ] Handoff points to the real next checkpoint.
- [ ] Unexecuted evidence remains `Not run`.
- [ ] No sensitive or device-specific data is committed.
- [ ] Artifact manifests match the archive when one is produced.

### 37.2 Package release gate

A package release additionally requires its user/developer documentation, changelog, migration guidance, licenses/notices, tests, and release record to match the shipped version under SFGSS-004 and SFGSS-009.

### 37.3 Fresh-handoff gate

A fresh collaborator must be able to identify:

- The authority order
- Current checkpoint
- Current package or standard
- Known blockers
- Evidence state
- Next action
- Stop point

without reading old ChatGPT transcripts.

---

## 38. Reconciliation findings

The SUITE-DOC-29 static audit found:

1. The central vault already follows the repository-first Obsidian model and contains all required document classes.
2. `Suite_Graph_Roadmap.md`, README, Current Notes, the health check, roadmap, package Graph Navigation blocks, and ADR graph requirements already provide a strong link foundation.
3. The central `Current Notes.md` has grown beyond the recommended active-surface trigger and contains multiple historical handoff snapshots. Structural compaction is queued for SUITE-DOC-30 after this standard is committed.
4. Existing documents mix wikilinks and relative Markdown links. Both are supported, but critical cross-surface navigation may be normalized during SUITE-DOC-30.
5. Existing package repositories are planned rather than created; package-local Current Notes and `Documentation~` evidence remain `Not run` until those repositories exist.
6. The stale Crafting open-decision wording, grandfathered document IDs, public-title punctuation variants, and missing repository fields remain queued for SUITE-DOC-30.
7. No runtime, package, repository, Obsidian-plugin, or empirical evidence was created by this standard.

---

## 38A. SUITE-DOC-32 reconciliation finding

The full-suite handoff audit approved one canonical handoff guide and a dedicated `Learning Reviews/` folder. Learning IDs are normalized to `PKG-LEARN-001` through `PKG-LEARN-028`. Review artifacts are created only when work begins, use one template and tracker, and remain educational evidence rather than implementation proof.

---

## 39. Approval

### 39.1 Approval checklist

- [x] Vault and repository boundaries are explicit.
- [x] Current Notes is separated from authority and permanent evidence.
- [x] Note labels, structure, promotion, and compaction are defined.
- [x] Obsidian links, tags, Graph View, and configuration boundaries are defined.
- [x] Canonical filenames and single-live-copy behavior are defined.
- [x] Handoff, README, checkpoint, and Git adjacency rules are defined.
- [x] Security, attachments, validation, and archival rules are defined.
- [x] Reconciliation findings are queued without fabricating completion.

### 39.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Implementation authorization:** None

---

## Graph Navigation

#sfgss/standard #sfgss/documentation #sfgss/obsidian #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 Checkpoint Workflow]]
- [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007 ADR Standard]]
- [[SFGSS-009_Repository_Versioning_and_Integration_Workspace_Standard|SFGSS-009 Repository Standard]]
- [[Current Notes]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
