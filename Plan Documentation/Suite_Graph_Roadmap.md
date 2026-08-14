---
tags:
  - sfgss/navigation
  - sfgss/roadmap
  - sfgss/graph
status: active
updated: 2026-08-13
---

# The Sperk’s Forge — Suite Graph Roadmap

**Document role:** Obsidian navigation hub and visual roadmap
**Authority:** Navigation only; it does not override SFGSS-000, package specifications, ADRs, standards, or integration specifications
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Current work item:** EUI-M1-02 – The Looking Glass (`EchoUI`) External UI Context, Ordered Surface Response Rules, and Input-Aware Selection Contract — ACTIVE / AUTHORIZED
**Completed collision reviews:** Foundation, Expansion, and Advanced

> This note is the map room. The linked documents remain the territory.


## Current frontier — August 13, 2026

- **Looking Glass:** EUI-M1-01 complete/finally reconciled at `57a4fa4`; EUI-M1-02 is **ACTIVE / AUTHORIZED** under package specification v1.2.0.
- **First Light:** FL-M5-R1 remains sealed/frozen for the current pass.
- **Chronicle:** M5 remains complete; M6 First Integration is not activated by the Looking Glass checkpoint.
- EUI-M1-02 is bounded to external active/inactive UI context, designer-ordered per-surface response, authored/local/runtime override resolution, and externally supplied input-aware selection. Motifs, Builder, preset tooling, MMO layout persistence, arbitrary context payloads, and peer bridges remain future work.


## How to use this in Obsidian

- Open this note and choose **Open local graph** to see the immediate suite structure.
- Use the global Graph View and filter with tags such as `#sfgss/wave/foundation`, `#sfgss/wave/expansion`, `#sfgss/wave/advanced`, `#sfgss/standard`, or `#sfgss/integration`.
- Each package specification contains a **Graph Navigation** block linking back here.
- Mermaid diagrams provide a readable roadmap, while the `[[wikilinks]]` below create the actual Obsidian graph edges.
- This file must be updated at every meaningful documentation checkpoint.

## Documentation journey

```mermaid
flowchart LR
    A[SFGSS-000 Suite Bible] --> B[SFGSS-001 Package Template]
    A --> C[SFGSS-002 to 005 Core Standards]
    B --> D[10 Foundation Specifications]
    B --> E[13 Expansion Specifications]
    B --> F[5 Advanced Foundations]
    D --> G[Foundation Collision Matrix]
    E --> H[Expansion Collision Matrix]
    F --> I[Advanced Collision and Research Matrix — Approved]
    G --> H[SFGSS-008 Glossary and Naming Registry]
    H --> J[All Standards SFGSS-001 to 010 Approved]
    H --> J
    I --> J
    J --> K[Full Suite Matrix — Approved]
    K --> Handoff[Documentation and Learning Handoff Audit]
    Handoff --> R1[PKG-LEARN-001 First Light Complete]
    R1 --> M[SUITE-DOC-33 Gate — Passed]
    M --> I1[FL-M1-01 First Light Skeleton — Active]
    I1 --> J[Just-in-Time Review Before Each Later Package]
```

## Package Reference Showcase path

```mermaid
flowchart LR
    Lab[Standalone Test Lab — engineering proof] --> Ref[Package Reference Showcase — consumer proof]
    Ref --> Clean[Clean-project reproduction]
    Ref --> Gallery[Suite Showcase Hub]
    Integration[Integration Labs] --> Gallery
    Gallery -. never substitutes for .-> Lab
```

- Every package Reference Showcase is project-owned and uses documented public consumer surfaces.
- Reference Showcases live outside immutable package source, normally under `Assets/EchoDevGames/SuiteShowcase/<Package>/`.
- the Suite Showcase Hub is a future project-owned navigation/presentation hub, not a runtime package authority.

## Package waves

```mermaid
flowchart TB
    subgraph F[Foundation Wave — application shell]
      FL[First Light] --> OBS[Observatory]
      FL --> ACC[Accord]
      FL --> PASS[Passage]
      FL --> PULSE[Pulse]
      FL --> RES[Resonance]
      FL --> WILL[Will]
      FL --> UI[Looking Glass]
      FL --> SAVE[Chronicle]
      WORK[Workshop] -. composes .-> FL
    end

    subgraph E[Expansion Wave — reusable gameplay infrastructure]
      IMP[Impact]
      POOL[Wellspring]
      PROG[Ascent]
      BUILD[Foundry]
      LOC[Many Tongues]
      DLG[Voices]
      OBJ[Path]
      INV[Vault]
      INT[Hand]
      CAM[Eye]
      CHAR[Fellowship]
      CTRL[Vessel]
      CRAFT[Crucible]
    end

    subgraph A[Advanced Wave — provider-neutral gameplay foundations]
      MP[Convergence]
      AI[Instinct]
      COMBAT[Clash]
      ABIL[Arcana]
      WORLD[Atlas]
    end

    UI -. presents .-> ACC
    UI -. presents .-> SAVE
    WILL -. intent .-> CTRL
    CHAR -. control ownership .-> CTRL
    CHAR -. target .-> CAM
    INT -. requests .-> DLG
    OBJ -. observes .-> DLG
    CRAFT -. consumes/grants .-> INV
    ABIL -. submits operations .-> COMBAT
    AI -. requests .-> CTRL
    WORLD -. travel plan .-> PASS
    MP -. authority adapters .-> CHAR
```

## Active Game Shell / Front Door initiative

This is the current **development sequence**, not a new hard dependency chain:

```mermaid
flowchart LR
    FL[First Light — complete] --> SAVE[Chronicle — learning active]
    SAVE --> ACC[Accord — planned next]
    ACC --> RES[Resonance — planned after Accord]
    RES --> UI[Looking Glass — planned after Resonance]
    UI --> SHOW[Combined Game Shell Showcase]
    FL -. startup/audio intent .-> RES
    FL -. startup presentation/handoff .-> UI
    ACC -. audio preference snapshot .-> RES
    ACC -. settings presentation .-> UI
    SAVE -. optional durable game-save transport .-> SHOW
```

Each package remains independently installable and useful. Chronicle does not become a dependency merely because another package has persistence-capable data. Optional integrations live in bridges/adapters, and long-lived Unity service composition remains project-owned under SFGSS-ADR-006.

## Practical game loop view

```mermaid
flowchart LR
    Launch[First Light] --> Menu[Looking Glass]
    Menu --> Settings[Accord]
    Menu --> Load[Chronicle]
    Menu --> Travel[Passage]
    Travel --> World[Atlas]
    World --> Character[Fellowship]
    Input[Will] --> Controller[Vessel]
    Character --> Controller
    Controller --> Interaction[Hand]
    Interaction --> Dialogue[Voices]
    Interaction --> Inventory[Vault]
    Dialogue --> Objectives[Path]
    Objectives --> Progression[Ascent]
    Inventory --> Crafting[Crucible]
    Ability[Arcana] --> Combat[Clash]
    AI[Instinct] --> Combat
    Combat --> Feedback[Impact]
    Feedback --> Audio[Resonance]
    Feedback --> Camera[Eye]
    Pool[Wellspring] --> Combat
    Diagnostics[Observatory] -. observes .-> Launch
    Diagnostics -. observes .-> Combat
    Build[Foundry] --> Release[Release Output]
    Workshop[Workshop] --> Launch
```

## Primary authorities and standards

- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 — Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 — Package Specification Template]]
- [[SFGSS-002_Dependency_Bridge_and_Assembly_Standard|SFGSS-002 — Dependencies, Bridges, and Assemblies]]
- [[SFGSS-003_Data_IDs_Serialization_and_Migration_Standard|SFGSS-003 — Data, IDs, Serialization, and Migration]]
- [[SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard|SFGSS-004 — Testing and Release Evidence]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 — Checkpoint and Learning Workflow]]
- [[SFGSS-006_New-Project_Guided_Pathways|SFGSS-006 — New-Project Guided Pathways]]
- [[SFGSS-007_Architecture_Decision_Record_Standard|SFGSS-007 – Architecture Decision Records]]
- [[SFGSS-008_Suite_Glossary_and_Naming_Registry|SFGSS-008 – Glossary and Naming Registry]]
- [[SFGSS-009_Repository_Versioning_and_Integration_Workspace_Standard|SFGSS-009 – Repository, Versioning, and Integration Workspace]]
- [[SFGSS-010_Living_Documentation_Current_Notes_and_Obsidian_Workflow_Standard|SFGSS-010 – Living Documentation, Current Notes, and Obsidian Workflow]]
- [[Full_Suite_Documentation_Program_Roadmap|Full Suite Documentation Program Roadmap]]
- [[Current Notes|Current Notes]]

- [[Full_Suite_Documentation_and_Learning_Handoff_Guide|Full Suite Documentation and Learning Handoff Guide]]
- [[Learning Reviews/README|Learning Reviews Index]]
- [[Learning Reviews/PKG-LEARN-TEMPLATE|Package Learning Review Template]]

## Integration and decision hubs

- [[Integration Specifications/Foundation_Cross-Package_Contract_Matrix|Foundation Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-ADVANCED-001_Advanced_Cross-Package_and_Research_Contract_Matrix|Advanced Cross-Package and Research Matrix]]
- [[Integration Specifications/SFGSS-INT-SUITE-001_Full_Suite_Authority_Dependency_Bridge_and_Persistence_Matrix|Full Suite Authority, Dependency, Bridge, and Persistence Matrix]]
- [[Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|ADR-001 — Setup Facade Protocol]]
- [[Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation|ADR-002 — Documentation Gate and Learning Implementation]]
- [[Architecture Decision Records/SFGSS-ADR-003_Graph_Roadmap_and_Pre-Implementation_Learning_Review|ADR-003 — Graph Roadmap and Package Learning Review]]
- [[Architecture Decision Records/SFGSS-ADR-004_Just-in-Time_Package_Learning_Gate|ADR-004 — Just-in-Time Package Learning Gate]]
- [[Architecture Decision Records/SFGSS-ADR-005_Package_Reference_Showcases_and_Suite_Showcase_Hub|ADR-005 — Package Reference Showcases and the Suite Showcase Hub]]
- [[Architecture Decision Records/SFGSS-ADR-006_Persistence_Runtime_State_and_Object_Lifetime_Separation|ADR-006 — Persistence, Runtime State, and Object Lifetime Separation]]
- [[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite ADR Decision Log]]
- [[Architecture Decision Records/SFGSS-ADR-TEMPLATE|Reusable ADR Template]]

## Package specification nodes

### Foundation Wave

- [[Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification|First Light (`EchoLaunch`)]]
- [[Package Specifications/SFGSS-The-Observatory-EchoDiagnostics-Package-Specification|The Observatory (`EchoDiagnostics`)]]
- [[Package Specifications/SFGSS-The-Accord-EchoSettings-Package-Specification|The Accord (`EchoSettings`)]]
- [[Package Specifications/SFGSS-The-Passage-EchoSceneFlow-Package-Specification|The Passage (`EchoSceneFlow`)]]
- [[Package Specifications/SFGSS-The-Pulse-EchoGameState-Package-Specification|The Pulse (`EchoGameState`)]]
- [[Package Specifications/SFGSS-Resonance-Jukebot-Package-Specification|Resonance (`Jukebot`)]]
- [[Package Specifications/SFGSS-The-Will-EchoInput-Package-Specification|The Will (`EchoInput`)]]
- [[Package Specifications/SFGSS-The-Looking-Glass-EchoUI-Package-Specification|The Looking Glass (`EchoUI`)]]
- [[Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification|The Chronicle (`EchoSave`)]]
- [[Package Specifications/SFGSS-The-Workshop-EchoGameStarter-Package-Specification|The Workshop (`EchoGameStarter`)]]
### Expansion Wave

- [[Package Specifications/SFGSS-Impact-EchoFeedback-Package-Specification|Impact (`EchoFeedback`)]]
- [[Package Specifications/SFGSS-The-Wellspring-EchoPool-Package-Specification|The Wellspring (`EchoPool`)]]
- [[Package Specifications/SFGSS-The-Ascent-EchoProgression-Package-Specification|The Ascent (`EchoProgression`)]]
- [[Package Specifications/SFGSS-The-Foundry-EchoBuildTools-Package-Specification|The Foundry (`EchoBuildTools`)]]
- [[Package Specifications/SFGSS-Many-Tongues-EchoLocalization-Package-Specification|Many Tongues (`EchoLocalization`)]]
- [[Package Specifications/SFGSS-Voices-EchoDialogue-Package-Specification|Voices (`EchoDialogue`)]]
- [[Package Specifications/SFGSS-The-Path-EchoObjectives-Package-Specification|The Path (`EchoObjectives`)]]
- [[Package Specifications/SFGSS-The-Vault-EchoInventory-Package-Specification|The Vault (`EchoInventory`)]]
- [[Package Specifications/SFGSS-The-Hand-EchoInteraction-Package-Specification|The Hand (`EchoInteraction`)]]
- [[Package Specifications/SFGSS-The-Eye-EchoCamera-Package-Specification|The Eye (`EchoCamera`)]]
- [[Package Specifications/SFGSS-The-Fellowship-EchoCharacters-Package-Specification|The Fellowship (`EchoCharacters`)]]
- [[Package Specifications/SFGSS-The-Vessel-EchoControllers-Package-Specification|The Vessel (`EchoControllers`)]]
- [[Package Specifications/SFGSS-The-Crucible-EchoCrafting-Package-Specification|The Crucible (`EchoCrafting`)]]
### Advanced Wave

- [[Package Specifications/SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation|The Convergence (`EchoMultiplayer`)]]
- [[Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation|Instinct (`EchoAI`)]]
- [[Package Specifications/SFGSS-Clash-EchoCombat-Package-Foundation|Clash (`EchoCombat`)]]
- [[Package Specifications/SFGSS-Arcana-EchoAbilities-Package-Foundation|Arcana (`EchoAbilities`)]]
- [[Package Specifications/SFGSS-The-Atlas-EchoWorld-Package-Foundation|The Atlas (`EchoWorld`)]]

## Learning and status nodes

- [[Full_Suite_Documentation_and_Learning_Handoff_Guide|Full Suite Handoff Guide]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
- [[Learning Reviews/README|Learning Reviews Index]]
- [[Learning Reviews/PKG-LEARN-TEMPLATE|Learning Review Template]]
- [[Suite_Health_Check_and_Remaining_Documentation|Suite Health Check and Remaining Documentation]]

## Maintenance rule

At each checkpoint closeout:

1. Add newly approved or superseded documents.
2. Update the active-checkpoint node and status summary.
3. Confirm every current package specification links back to this note.
4. Remove or redirect superseded duplicate nodes from the active vault.
5. Keep empirical work marked `Not run` until evidence exists.


## Guided pathway nodes

- [[SFGSS-006_New-Project_Guided_Pathways#17. PATH-000 - Blank Modular Starter|PATH-000 Blank Modular Starter]]
- [[SFGSS-006_New-Project_Guided_Pathways#18. PATH-001 - Package Laboratory and Portfolio System|PATH-001 Package Laboratory]]
- [[SFGSS-006_New-Project_Guided_Pathways#19. PATH-010 - Minimal Audiovisual Prototype|PATH-010 Minimal Audiovisual Prototype]]
- [[SFGSS-006_New-Project_Guided_Pathways#20. PATH-020 - Game Jam Quickstart|PATH-020 Game Jam Quickstart]]
- [[SFGSS-006_New-Project_Guided_Pathways#21. PATH-030 - Puzzle and Tabletop Game|PATH-030 Puzzle and Tabletop]]
- [[SFGSS-006_New-Project_Guided_Pathways#22. PATH-040 - Password-Based Puzzle Platformer|PATH-040 Password Platformer]]
- [[SFGSS-006_New-Project_Guided_Pathways#23. PATH-050 - Save-Based Adventure|PATH-050 Save-Based Adventure]]
- [[SFGSS-006_New-Project_Guided_Pathways#24. PATH-060 - Narrative Game|PATH-060 Narrative Game]]
- [[SFGSS-006_New-Project_Guided_Pathways#25. PATH-070 - Action Combat Prototype|PATH-070 Action Combat Prototype]]
- [[SFGSS-006_New-Project_Guided_Pathways#26. PATH-080 - RPG Foundation|PATH-080 RPG Foundation]]
- [[SFGSS-006_New-Project_Guided_Pathways#27. PATH-090 - Local Multiplayer Prototype|PATH-090 Local Multiplayer]]
- [[SFGSS-006_New-Project_Guided_Pathways#28. PATH-100 - Online Multiplayer Research Prototype|PATH-100 Online Multiplayer Research]]
- [[SFGSS-006_New-Project_Guided_Pathways#29. PATH-110 - Existing-Project Incremental Adoption|PATH-110 Existing-Project Adoption]]


## Repository and release topology

```mermaid
flowchart TB
    Central[Central suite documentation/catalog repository]
    Lab[Sperk's Forge Integration Lab]
    Central --> Lab
    Central --> Pkg[28 independent package repositories]
    Pkg --> Bridge[Bridge repositories]
    Pkg --> Provider[Provider adapter repositories]
    Pkg -. exact tags/commits .-> Lab
    Bridge -. exact versions .-> Lab
    Provider -. exact versions .-> Lab
    Lab --> Snapshot[compat-YYYY.MM.DD.N snapshot]
    Snapshot --> Catalog[Central compatibility catalog]
```

- [[SFGSS-009_Repository_Versioning_and_Integration_Workspace_Standard|Repository and versioning authority]]
- [Machine-readable repository registry](SFGSS-009_Repository_Registry.json)

- [[SFGSS-INT-CONSISTENCY-001_Standards_and_Package_Consistency_Matrix|Standards and Package Consistency Matrix]]


## Active implementation route

- [[Learning Reviews/PKG-LEARN-009_EchoSave_Learning_Review|PKG-LEARN-009 Chronicle Learning Review — In progress]]
- [[Checkpoint Build Plans/ESV-M1-01_Chronicle_Installable_Skeleton_and_Duplicate-Safe_Authority_Claim_Checkpoint_Build_Plan|ESV-M1-01 Chronicle Skeleton Plan — Scaffolded / Locked]]
- [[Architecture Decision Records/SFGSS-ADR-006_Persistence_Runtime_State_and_Object_Lifetime_Separation|ADR-006 Persistence/Lifetime Separation]]
- First Light remains complete/frozen at `c18eff6` distribution baseline; its release qualification is a separate future return gate.

## Learning review flow

```mermaid
flowchart LR
    H[Full Suite Handoff Guide] --> C[Package Learning Catalog]
    C --> R1[PKG-LEARN-001 First Light — Complete]
    R1 --> FL[First Light implementation/gallery/distribution — Complete]
    FL --> R9[PKG-LEARN-009 Chronicle — In progress]
    R9 --> TB[Chronicle teach-back]
    TB --> ESV[Explicit ESV-M1-01 activation]
    ESV --> ACC[Later PKG-LEARN-003 Accord]
    ACC --> RES[Later PKG-LEARN-006 Resonance]
    RES --> UI[Later PKG-LEARN-008 Looking Glass]
```

- Completed review: **PKG-LEARN-001 – First Light (`EchoLaunch`)**
- Paused review: **PKG-LEARN-002 – The Observatory (`EchoDiagnostics`)**
- Active review: **PKG-LEARN-009 – The Chronicle (`EchoSave`)**
- Tracker: `Learning Reviews/PKG-LEARN-TRACKER.json`
- Chronicle implementation remains locked until PKG-LEARN-009 passes and Jesse explicitly activates ESV-M1-01.
## 2026-08-13 Looking Glass activation

`PKG-LEARN-008` is Complete. `EUI-M1-01` is the active package checkpoint under SFGSS-005 Green Path from baseline `f57880a`. This activation intentionally pulls Looking Glass foundation work forward after Chronicle M5 so project UI composition can begin incrementally; it does not create a hard dependency chain or unlock another package.
