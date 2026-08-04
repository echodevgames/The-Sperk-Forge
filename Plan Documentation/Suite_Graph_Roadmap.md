---
tags:
  - sfgss/navigation
  - sfgss/roadmap
  - sfgss/graph
status: active
updated: 2026-08-04
---

# The Sperk’s Forge — Suite Graph Roadmap

**Document role:** Obsidian navigation hub and visual roadmap  
**Authority:** Navigation only; it does not override SFGSS-000, package specifications, ADRs, standards, or integration specifications  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Current checkpoint:** SUITE-DOC-24 — Advanced Cross-Package and Research Review

> This note is the map room. The linked documents remain the territory.

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
    F --> I[Advanced Collision and Research Review]
    G --> J[Remaining Standards SFGSS-006 to 010]
    H --> J
    I --> J
    J --> K[Full Suite Matrix and Handoff Audit]
    K --> L[28 Package Learning Reviews]
    L --> M[SUITE-DOC-33 Final Readiness Gate]
    M --> N[First Light FL-M1-01 Package Skeleton]
```

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
- [[Full_Suite_Documentation_Program_Roadmap|Full Suite Documentation Program Roadmap]]
- [[Current Notes|Current Notes]]

## Integration and decision hubs

- [[Integration Specifications/Foundation_Cross-Package_Contract_Matrix|Foundation Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Cross-Package Contract Matrix]]
- [[Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|ADR-001 — Setup Facade Protocol]]
- [[Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation|ADR-002 — Documentation Gate and Learning Implementation]]
- [[Architecture Decision Records/SFGSS-ADR-003_Graph_Roadmap_and_Pre-Implementation_Learning_Review|ADR-003 — Graph Roadmap and Package Learning Review]]

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

- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
- [[Suite_Health_Check_and_Remaining_Documentation|Suite Health Check and Remaining Documentation]]

## Maintenance rule

At each checkpoint closeout:

1. Add newly approved or superseded documents.
2. Update the active-checkpoint node and status summary.
3. Confirm every current package specification links back to this note.
4. Remove or redirect superseded duplicate nodes from the active vault.
5. Keep empirical work marked `Not run` until evidence exists.
