# <Package Public Title> Complete User Handout

**Technical package:** `<Technical Identifier>`
**Package ID:** `<com.echodevgames.package>`
**Package version:** `<VERSION>`
**Unity baseline:** `<VERSION>`
**Minimum declared Unity version:** `<VERSION>`
**Handout status:** `<STATUS>`
**Distribution status:** `<ARTIFACT / QUALIFICATION STATE>`

---

## 1. What This Package Is

- One-sentence ownership contract.
- Who should use it.
- Normal happy path.

## 2. What It Owns

List the package's actual authorities and responsibilities.

## 3. What It Does Not Own

List neighboring/project concerns that remain outside the package.

## 4. Current Product and Qualification Status

Record implemented scope and exact retained evidence.

Separate:

- artifact availability;
- clean-project proof;
- supported install routes;
- player/performance evidence;
- beta/RC/stable status.

## 5. Distribution Kit

Document:

- repository kit path;
- artifact filename;
- SHA-256 verification;
- manifest/build record;
- what is and is not bundled.

## 6. Installation

For each intended route:

- exact steps;
- prerequisites;
- current evidence state;
- removal/reinstall behavior.

Never describe an untested route as Supported.

## 7. Five-Minute Quick Start

Give the shortest successful consumer path.

## 8. Setup and Configuration

Document every user-facing setup field, mode, generated asset, safety rule, preview/apply/repair boundary, and repeatability expectation.

## 9. Runtime Model

Explain:

- authority;
- lifecycle;
- data definitions;
- runtime state;
- success/failure/cancellation;
- reports/events;
- performance-relevant behavior.

## 10. Presentation / User Interaction

Document package-owned presentation and project-owned replaceable content.

## 11. Editor Tools

For each tool:

- menu path;
- purpose;
- side effects;
- explicit non-authority;
- copy/report evidence.

## 12. Standalone Test Lab / Samples

Explain import, scenarios, and isolation boundary.

## 13. Reference Showcase / Gallery

Explain repository-only consumer examples and what they prove.

## 14. Developer Extension Points

Document public interfaces, request/result contracts, events, providers/adapters, and common extension recipes.

## 15. Schemas / Stable Identity / Migration

Record current schema versions and compatibility rules.

## 16. Diagnostics

Provide diagnostic-family and important-code references.

## 17. Common Workflows

Include recipes for the package's most common real uses.

## 18. Troubleshooting

Use symptom -> likely cause -> evidence -> safe action.

## 19. Known Limitations / Deferred Capabilities

Be explicit. Do not hide deferred scope.

## 20. Removal / Reinstall / Upgrade

Describe project-owned data preservation and safe upgrade behavior.

## 21. Evidence and Qualification Boundary

State what is proven and what remains Not run / Planned / Unknown.

## 22. Support and Bug Reporting

List the environment, package/artifact identity, checksum, report/log, reproduction, and privacy-safe evidence needed for a useful report.

## 23. Quick Reference

Menus, paths, defaults, golden rules.

## 24. Glossary

Define package-specific terminology.

---

## Completion Rule

The handout is complete when a new recipient can:

1. identify and verify the artifact;
2. install it through the intended evaluation/supported route;
3. achieve the normal happy path;
4. understand every advertised capability;
5. know what the package refuses to own;
6. diagnose ordinary failures;
7. find the Laboratory/Showcase;
8. remove/reinstall safely;
9. distinguish artifact availability from qualified support;
10. file a useful issue without consulting the historical development conversation.
