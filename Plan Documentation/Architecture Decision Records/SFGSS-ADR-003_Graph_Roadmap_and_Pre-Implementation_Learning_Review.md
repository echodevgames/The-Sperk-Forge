# SFGSS-ADR-003 — Graph Roadmap and Pre-Implementation Package Learning Review

**Status:** Accepted  
**Date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Decision scope:** Suite documentation navigation and implementation-readiness workflow  
**Related authorities:** SFGSS-000, SFGSS-005, SFGSS-ADR-002, Full Suite Documentation Program Roadmap

> **Supersession note:** SFGSS-ADR-004 supersedes only the requirement that all twenty-eight reviews finish before any implementation. Graph navigation, progressive teaching, package review existence, and teach-back requirements remain accepted.

## Context

The package-first documentation program has produced twenty-eight substantial package specifications or foundations plus standards, ADRs, matrices, research records, and test reports. The documents preserve architecture accurately, but their volume makes the suite difficult to understand as one connected system.

Jesse also wants to understand each package’s purpose, practical use, authority boundary, lifecycle, and relationships before entering implementation code himself. A documentation gate that checks only file completeness would not prove that the suite is navigable or understood.

## Decision

1. The vault maintains a central `Suite_Graph_Roadmap.md` note.
2. The graph roadmap uses Obsidian `[[wikilinks]]` to every current package specification and primary authority, creating a navigable Graph View.
3. Every current and future package specification contains a compact Graph Navigation block linking to the graph roadmap, program roadmap, SFGSS-000, and SFGSS-001.
4. README and Current Notes link to the graph roadmap as a primary navigation surface.
5. The graph roadmap is updated at every meaningful checkpoint.
6. Before code is authorized, every package receives an individual learning review using the format defined in `Package_Learning_Review_Catalog.md`.
7. The final readiness gate requires both documentation completeness and learning-review completion.
8. A learning review is not implementation authorization and does not create empirical evidence.

## Consequences

### Positive

- Obsidian Graph View becomes useful rather than decorative.
- Package relationships are visible without reading every specification first.
- New conversations and collaborators receive a map before entering details.
- Jesse can build a mental model of the suite before manually entering code.
- The final implementation decision becomes informed rather than merely procedural.

### Costs

- The graph roadmap and navigation blocks require ongoing maintenance.
- Twenty-eight learning reviews add time before implementation.
- Some package choices may be revisited after the plain-language reviews expose confusion or unnecessary complexity.

These costs are accepted because misunderstanding a reusable package authority is more expensive after code and serialized assets exist.

## Alternatives rejected

- **Rely only on Obsidian’s automatic global graph:** rejected because a graph without deliberate links becomes noisy and structurally weak.
- **Create only one static Mermaid diagram:** rejected because it does not create navigable backlinks.
- **Skip package reviews and learn during coding:** rejected because the owner explicitly wants architectural understanding before implementation.
- **Require memorization of every public type:** rejected because reviews focus first on purpose, boundaries, lifecycle, and practical use.

## Review trigger

Revisit this decision if the graph becomes too dense to navigate, package grouping materially changes, or the learning reviews reveal that the full suite should be implemented in a different order.

## Graph Navigation

- [[Suite_Graph_Roadmap]]
- [[Package_Learning_Review_Catalog]]
- [[Full_Suite_Documentation_Program_Roadmap]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules]]
