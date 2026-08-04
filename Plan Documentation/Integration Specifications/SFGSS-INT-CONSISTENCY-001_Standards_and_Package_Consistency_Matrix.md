# The Sperk’s Forge – Standards and Package Consistency Matrix

**Document ID:** SFGSS-INT-CONSISTENCY-001  
**Version:** 1.0.0  
**Status:** Approved  
**Date:** August 4, 2026  
**Implementation authorization:** None

## 1. Scope

This matrix reconciles SFGSS-001 through SFGSS-010, all twenty-eight package specifications/foundations, ADR-001 through ADR-003, the three wave-level integration matrices, naming and repository registries, Current Notes, Graph Roadmap, health check, and the active documentation roadmap.

## 2. Cross-suite results

| Review surface | Result | Resolution |
|---|---|---|
| Package ownership | Pass | No competing authority found |
| Core dependency direction | Pass | No circular core package dependency |
| Formal public titles | Pass after repair | Canonical SFGSS-008 titles applied to package metadata |
| Package/document IDs | Pass | Registered IDs preserved; six grandfathered IDs explicitly retained |
| Diagnostic/test prefixes | Pass | 28 unique package-qualified families |
| Workshop setup facades | Pass | 28 exact facades remain registered through ADR-001 |
| Repository planning | Pass | SFGSS-008 registry synchronized with SFGSS-009 planned repositories; actual remotes remain unverified |
| Assembly policy | Pass after repair | First Light presenter split; Foundation Editor assemblies set to `autoReferenced: false` |
| Asset GUID/domain ID boundary | Pass after repair | Accord, Chronicle, Passage, and Will clarified |
| Unknown-field preservation | Pass after repair | Accord and Will require opaque/extension-capable preservation |
| Test/evidence language | Pass after repair | Package addenda separate planned definitions, automation, status, evidence, and issues |
| Platform claims | Pass after interpretation rule | Older `Yes` cells mean Planned until evidence exists |
| Current Notes discipline | Pass after compaction | Historical closeouts removed from active page; Git remains archive |
| Crafting decision status | Pass after repair | Stale Bible open decision removed; Crucible authority remains approved |
| Implementation evidence | Honest | All code, Laboratory, provider, compatibility, migration, performance, and release results remain `Not run` |

## 3. Package version reconciliation

| Package | Previous spec | Reconciled spec | Canonical public title | Repair class |
|---|---:|---:|---|---|
| EchoAbilities | 1.0.0 | 1.0.1 | Arcana – Ability Activation and Effect Orchestration | Registry/title/test-evidence normalization |
| EchoCombat | 1.0.0 | 1.0.1 | Clash – Combat Messages, Targets, and Resolution | Registry/title/test-evidence normalization |
| EchoLaunch | 1.1.0 | 1.2.0 | First Light – Startup and Launch | Separated the default uGUI presenter from the neutral Runtime assembly.; Set the Editor assembly to `autoReferenced: false`.; Canonicalized immutable `StartupStepDefinition` versus runtime executor terminology. |
| EchoFeedback | 1.0.0 | 1.0.1 | Impact – Coordinated Feedback | Registry/title/test-evidence normalization |
| EchoAI | 1.0.0 | 1.0.1 | Instinct – AI Perception, Decisions, and Behavior | Registry/title/test-evidence normalization |
| EchoLocalization | 1.0.0 | 1.0.1 | Many Tongues – Localization, Locale, and Regional Content | Registry/title/test-evidence normalization |
| Jukebot | 1.0.0 | 1.0.1 | Resonance – Audio Runtime | Registry/title/test-evidence normalization |
| EchoSettings | 1.0.0 | 1.1.0 | The Accord – Global Preferences | Clarified Unity asset GUID versus optional runtime/export configuration IDs.; Required opaque or extension-capable preservation for unknown fields inside known settings sections.; Set the Editor assembly to `autoReferenced: false`. |
| EchoProgression | 1.1.0 | 1.1.1 | The Ascent – Progression, Unlocks, Passwords, and Checkpoints | Registry/title/test-evidence normalization |
| EchoWorld | 1.0.0 | 1.0.1 | The Atlas – World Identity, Topology, and Travel Metadata | Registry/title/test-evidence normalization |
| EchoSave | 1.0.0 | 1.1.0 | The Chronicle – Save Infrastructure | Clarified Unity asset GUID versus optional runtime/export save-configuration identity. |
| EchoMultiplayer | 1.0.0 | 1.0.1 | The Convergence – Multiplayer Sessions and Authority | Registry/title/test-evidence normalization |
| EchoCrafting | 1.0.0 | 1.0.1 | The Crucible – Recipe Transformation and Production | Registry/title/test-evidence normalization |
| EchoCamera | 1.0.0 | 1.0.1 | The Eye – Camera Direction | Registry/title/test-evidence normalization |
| EchoCharacters | 1.0.0 | 1.0.1 | The Fellowship – Character Identity and Roster | Registry/title/test-evidence normalization |
| EchoBuildTools | 1.0.0 | 1.0.1 | The Foundry – Build Preparation, Validation, and Release Output | Registry/title/test-evidence normalization |
| EchoInteraction | 1.0.0 | 1.0.1 | The Hand – World Interaction | Registry/title/test-evidence normalization |
| EchoUI | 1.0.0 | 1.0.1 | The Looking Glass – UI Framework | Registry/title/test-evidence normalization |
| EchoDiagnostics | 1.0.0 | 1.0.1 | The Observatory – Diagnostics and Runtime Inspection | Registry/title/test-evidence normalization |
| EchoSceneFlow | 1.0.0 | 1.1.0 | The Passage – Scene Flow | Confirmed `SceneId` as the durable runtime identity, separate from Editor source GUID/path metadata. |
| EchoObjectives | 1.0.0 | 1.0.1 | The Path – Objectives, Quests, and Tasks | Registry/title/test-evidence normalization |
| EchoGameState | 1.1.0 | 1.1.1 | The Pulse – Runtime State | Registry/title/test-evidence normalization |
| EchoInventory | 1.0.0 | 1.0.1 | The Vault – Inventory and Item Containers | Registry/title/test-evidence normalization |
| EchoControllers | 1.0.0 | 1.0.1 | The Vessel – Player Controller Foundations | Registry/title/test-evidence normalization |
| EchoPool | 1.0.0 | 1.0.1 | The Wellspring – Runtime Object Pooling | Registry/title/test-evidence normalization |
| EchoInput | 1.0.0 | 1.1.0 | The Will – Input Infrastructure | Clarified Unity asset GUID, Input System GUID, and project-authored domain identity roles.; Made unknown extension-data preservation an explicit serializer/opaque-record requirement. |
| EchoGameStarter | 1.1.0 | 1.2.0 | The Workshop – Project Starter | Set the Editor-only Workshop assembly to `autoReferenced: false`. |
| EchoDialogue | 1.0.0 | 1.0.1 | Voices – Dialogue and Conversation Flow | Registry/title/test-evidence normalization |

## 4. Current governing interpretation

1. Historical parent-authority headers record the checkpoint in which a package was approved.
2. The package’s SUITE-DOC-30 addendum names the standards governing implementation now.
3. Package specifications remain Level 2 authorities for package behavior.
4. Standards govern shared policy and vocabulary; conflicts require an explicit package revision or ADR rather than silent precedence.
5. Navigation hubs summarize and link but never override authority.
6. All pre-code test registries define intended evidence only.

## 5. Gate decision

The standards and package authorities are consistent enough to proceed to SUITE-DOC-31. Package implementation remains locked.

## Graph Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
