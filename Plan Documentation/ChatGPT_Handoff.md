# The Sperk’s Forge — ChatGPT Continuation and Repository Rehydration Protocol

**Document ID:** SFGSS-HANDOFF-001
**Version:** 1.0.0
**Status:** Approved working protocol
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository:** The Sperk’s Forge
**Current Unity baseline:** Unity 6000.3.8f1
**Last updated:** August 5, 2026

> This file tells a fresh ChatGPT conversation how to rebuild the working context from the repository without depending on an old transcript.

---

## 1. Purpose

Long development conversations eventually become difficult to navigate, expensive to reload, or vulnerable to missing context.

This protocol provides one permanent, repository-owned starting point for a new ChatGPT conversation.

It is intentionally **evergreen**:

- It defines where current truth lives.
- It defines the required reading order.
- It defines how to identify the active package and checkpoint.
- It defines the expected workflow and response format.
- It does not hard-code a current commit, test count, or checkpoint that will become stale.

Current implementation state belongs in:

- `Plan Documentation/Current Notes.md`
- The active package’s `Documentation~/Developer/Current Notes.md`
- The latest checkpoint completion record
- Git history and user-provided Git evidence

---

## 2. Authority Order

When documents disagree, use this order:

1. `Echo_Game_Systems_Suite_Bible.md` / SFGSS-000
2. The approved active package specification
3. Accepted ADRs and integration specifications
4. The active Checkpoint Build Plan
5. Package architecture and completed checkpoint records
6. Test reports, issue records, setup guides, changelog, and release records
7. Root and package `Current Notes.md`
8. Chat history

Rules:

- `Current Notes.md` contains the freshest working context but does not silently override approved architecture.
- Chat history is supporting context only.
- Do not invent a reconciliation when authoritative files disagree.
- Identify the conflict and pause only when it materially changes authority, architecture, public API, or irreversible project direction.

---

## 3. Required Reading Order

A fresh conversation must read in this order before writing code:

1. `Plan Documentation/README.md`
2. `Plan Documentation/ChatGPT_Handoff.md`
3. `Plan Documentation/Echo_Game_Systems_Suite_Bible.md`
4. `Plan Documentation/Current Notes.md`
5. The active package specification
6. `SFGSS-005` — Checkpoint Build Workflow and ChatGPT Collaboration Rules
7. Applicable ADRs or integration specifications
8. The active package’s:
   - `README.md`
   - `Documentation~/Index.md`
   - `Documentation~/Developer/Architecture.md`
   - `Documentation~/Developer/Current Notes.md`
9. The latest completed checkpoint record
10. The active Checkpoint Build Plan, when one exists
11. Relevant runtime, editor, and test files
12. Recent Git evidence supplied by Jesse

Do not read every package specification unless multiple packages are genuinely in scope.

---

## 4. Minimum Files for a New Conversation

### 4.1 When repository browsing is available

Provide this file or its repository path and ask ChatGPT to read the repository documents in the required order.

### 4.2 When repository browsing is unavailable

Upload at minimum:

1. `Plan Documentation/ChatGPT_Handoff.md`
2. `Plan Documentation/Echo_Game_Systems_Suite_Bible.md`
3. `Plan Documentation/Current Notes.md`
4. The active package specification
5. `SFGSS-005`
6. The active package architecture
7. The active package `Current Notes.md`
8. The latest completed checkpoint record
9. The active Checkpoint Build Plan, when one exists

For implementation or debugging, also upload the directly affected source and test files.

### 4.3 When the context is uncertain

A fresh conversation must not guess.

It should name the missing file or evidence needed and wait for Jesse to provide it.

---

## 5. Mandatory First Response

Before writing code, generating bundles, changing documents, or proposing a new checkpoint, the fresh conversation must report:

```text
Authorities read:
Active package:
Active package specification/version:
Current milestone/checkpoint:
Last completed checkpoint:
Latest implementation commit:
Latest documentation commit:
Compilation result:
Automated test result:
Known blockers:
Next authorized action:
Document conflicts found:
Missing evidence:
```

The response must distinguish:

- Repository-supported facts
- User-provided Git/test evidence
- Inference
- Missing information

If no conflict or missing evidence exists, say so plainly.

---

## 6. Fresh Conversation Opening Prompt

Copy this prompt into a new ChatGPT conversation after supplying the required files:

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Read every supplied repository document completely before writing code.

Use this authority order:
1. SFGSS-000
2. The approved active package specification
3. Accepted ADRs and integration specifications
4. The active Checkpoint Build Plan
5. Package architecture and completed checkpoint records
6. Test, issue, setup, changelog, and release records
7. Root and package Current Notes
8. Chat history

Use Plan Documentation/ChatGPT_Handoff.md as the workflow and conversation-rehydration protocol.

Determine the active package, current checkpoint, last completed checkpoint, latest implementation state, test state, blockers, and next authorized action from the repository documents and the Git evidence I provide.

Before doing any work, report:
- Authorities read
- Active package and specification version
- Current milestone/checkpoint
- Last completed checkpoint
- Latest implementation commit
- Latest documentation commit
- Compilation result
- Automated test result
- Known blockers
- Next authorized action
- Any conflict or missing evidence

Preserve package independence, isolated Test Lab proof, neutral technical APIs, Verse-flavored user-facing names, and the repository/Obsidian documentation workflow.

Use the Checkpoint Build Plan format for implementation.

Treat normal bounded checkpoint details, test maintenance, documentation closeouts, and incremental architectural decisions as pre-approved. Ask me only when a decision is fundamental, irreversible, authority-changing, or materially contradicts an approved specification.

Keep code implementation visible and explained file by file. Batch-generate documentation scaffolding and checkpoint Markdown instead of making me create documentation files one at a time.

Use Command Prompt, Notepad++, Unity, and GitHub Desktop workflows. Do not use PowerShell unless the task truly cannot be completed another way and I explicitly agree.

Do not claim compilation, tests, commits, pushes, clean Git state, or Unity results without evidence.

When I say “go,” continue from the next authorized checkpoint or phase. Do not redesign the roadmap unless an authoritative conflict requires it.
```

---

## 7. Workflow Rules for ChatGPT

### 7.1 Read before acting

- Read the relevant documents completely.
- Reconstruct the active state from repository records.
- Do not restart a completed checkpoint.
- Do not repeat implementation already committed.
- Do not silently replace current architecture with a remembered older version.

### 7.2 Normal checkpoint decisions are pre-approved

Proceed without asking for repeated approval when the work is:

- Inside the active checkpoint’s authorized scope
- A bounded compile fix
- A bounded test-fixture correction
- Documentation reconciliation
- Checkpoint closeout
- Incremental implementation detail that preserves the approved architecture
- A safe file-layout or naming decision already implied by the specification
- A non-destructive automation improvement

Pause only when a choice is:

- Groundbreaking
- Fundamental
- Irreversible
- Authority-changing
- Public-API breaking
- Serialization-breaking
- Dependency-changing
- A material contradiction of SFGSS-000, the package specification, or an ADR

### 7.3 Package learning reviews

Perform each package learning review immediately before that package is implemented.

Do not front-load every package learning review before the related package work begins.

### 7.4 Visible implementation, streamlined documentation

For code:

- Show and explain complete implementation files.
- Keep file ownership and boundaries visible.
- Explain why each file exists.
- Preserve explicit stop points.
- Do not conceal runtime changes inside a documentation script.

For documentation:

- Generate complete Markdown files or a one-command documentation bundle.
- Avoid making Jesse create and verify every Markdown file manually.
- Provide a concise review and final validation summary.

### 7.5 CMD-first workflow

Preferred tools:

- Windows Command Prompt
- Notepad++
- Unity Editor
- GitHub Desktop
- Direct file downloads and ZIP bundles

Rules:

- Do not require PowerShell unless no reasonable alternative exists.
- Ask before using PowerShell.
- Quote paths containing spaces.
- Scripts must print what they changed.
- Scripts must stop on missing baselines or missing files.
- Scripts must not commit or push unless the user explicitly requested that behavior.

### 7.6 Download bundle standard

For multi-file phases, provide:

- One ZIP bundle
- One CMD apply script
- A short README
- Direct-file fallbacks for critical source files when practical

The apply script should:

1. Accept the repository root as its first argument.
2. Verify expected package or repository structure.
3. Verify the expected baseline commit when appropriate.
4. Copy only authorized files.
5. Print the changed files and scope.
6. Avoid staging, committing, or pushing unless explicitly intended.

### 7.7 Compile and test stop points

After a runtime phase:

1. Stop.
2. Return to Unity.
3. Wait for compilation.
4. Require the actual compiler result.
5. Fix the first error or warning before continuing.

After tests are installed:

1. Run the complete required suite.
2. Use actual totals.
3. Investigate the first failure.
4. Do not call a checkpoint complete while failures remain.

### 7.8 Git evidence

Use the user’s actual CMD output as the source of truth.

Required evidence may include:

```cmd
git status --short
git diff --cached --check
git diff --cached --stat
git status
git log -2 --oneline
```

Do not claim:

- A commit exists
- A push succeeded
- A branch is synchronized
- A working tree is clean

until the output proves it.

### 7.9 Documentation closeout

At every meaningful checkpoint:

1. Reconcile root and package `Current Notes.md`.
2. Promote durable decisions.
3. Update architecture, checkpoint, test, changelog, README, index, issue, setup, or release records as applicable.
4. Confirm documentation matches committed implementation.
5. Commit documentation with the code when practical or in an immediately adjacent documentation commit.
6. Confirm the repository is clean and synchronized.

---

## 8. User Shorthand

Interpret these phrases consistently:

### `go`

Continue to the next authorized phase or checkpoint.

Do not ask Jesse to repeat approval for normal bounded work.

### `Compiled with 0 errors`

Record compilation success for the current phase.

If warnings were not reported, ask for the warning count only when the checkpoint requires zero warnings.

### `Compiled with 0 errors, 0 warnings`

Record the compile gate as passed and proceed to the next authorized phase.

### Test totals or Test Runner screenshot

Evaluate the actual result.

- If all required tests pass, proceed to Git scope verification.
- If any test fails, diagnose the first failure.
- Do not skip directly to staging.

### `git status --short` output

Verify:

- Authorized files only
- Required `.meta` files
- No missing generated files
- No unrelated files

Then provide one exact staging command.

### Commit and push output

Verify:

- Commit hash
- Push destination
- Branch synchronization
- Clean working tree
- Latest two commits

Then either generate the adjacent documentation closeout or declare the checkpoint fully closed.

---

## 9. Evidence and Honesty Rules

Never invent:

- Test totals
- Compiler results
- Unity screenshots
- Git commits
- Git hashes
- Push success
- Package versions
- File contents
- Repository cleanliness
- User actions

When evidence is missing:

- State what is known.
- State what is inferred.
- State what must be supplied.
- Do not fill the gap with a plausible story.

When a source file is stale:

- Do not silently correct it from memory.
- Compare it against higher-authority and newer repository records.
- Record the reconciliation in the proper document.

---

## 10. Conversation Rotation Rule

Start a fresh conversation when any of these becomes true:

- The conversation has become difficult to navigate.
- Earlier responses are being truncated or malformed.
- Download links or tool output are repeatedly corrupted.
- The active checkpoint has closed and a clean handoff point exists.
- The next task enters a new package or materially different workstream.
- Context recovery is taking longer than reading the repository documents again.

Before rotating:

1. Finish or safely stop the active phase.
2. Update root and package `Current Notes.md`.
3. Commit and push the checkpoint or record the exact uncommitted state.
4. Capture current compilation and test evidence.
5. Capture `git status` and recent commits.
6. Start the new conversation with the prompt in Section 6.

Do not paste the entire old transcript into the repository.

Git history and checkpoint records are the archive.

---

## 11. Handoff Snapshot Template

Use this compact snapshot at the end of a conversation when the repository cannot yet be updated:

```text
Project:
Repository:
Unity version:
Active package:
Package specification/version:
Current checkpoint:
Last completed checkpoint:
Implementation commit:
Documentation commit:
Working tree:
Compilation:
Tests:
Files currently modified/untracked:
Known blockers:
Last action completed:
Next authorized action:
Required uploads for the new conversation:
```

Repository documentation should replace this temporary snapshot as soon as practical.

---

## 12. Maintenance Rule

This file owns the **process**, not the current project status.

Update this file only when the handoff or ChatGPT collaboration workflow changes.

Do not update it for every checkpoint.

Current package, checkpoint, commit, tests, blockers, and next action belong in `Current Notes.md`, checkpoint records, test reports, and Git history.
## Looking Glass / Green Path handoff — 2026-08-13

- Clean kickoff baseline: `f57880a`.
- PKG-LEARN-008 — The Looking Glass (`EchoUI`) is complete through Learn → Declare → Authorize.
- Looking Glass package authority: SFGSS-PKG-ECHOUI-001 v1.1.0.
- EUI-M1-01 is ACTIVE / AUTHORIZED.
- SFGSS-005 v1.6.0 and SFGSS-ADR-007 authorize Green Path self-validating checkpoint execution.
- First proof: one exclusive `frontend` screen scope (`main-menu -> settings -> Back`) plus independent `default-window` coexistence.
- Later authorized direction: layered external UI context, cascading visibility, input-aware default focus, Lego primitives, Motifs, Builder, stable Surface Registry, and project-authored Menu-for-Menus interfaces.
- Looking Glass does not own Pause/Cinematic/Loading truth, input mappings, domain data/actions, or project DDOL composition.
- Stop on unexpected compile/test/manual-proof/repository/rollback failure or any authority-changing discovery.
