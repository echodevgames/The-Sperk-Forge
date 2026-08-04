# Checkpoint Handoff Snapshot Template

**Purpose:** Provide the smallest complete context a fresh collaborator or ChatGPT conversation needs to continue safely.

## Handoff Snapshot

**Repository/project:** `<REPOSITORY>`  
**Completed checkpoint:** `<ID AND TITLE>`  
**Result:** `<APPROVED / PASS / REVISE / BLOCKED>`  
**Current package/standard:** `<NAME>`  
**Current focus:** `<FOCUS>`  
**Active checkpoint:** `<NEXT ID AND TITLE>`  
**Relevant versions:** `<VERSIONS>`  
**Files changed:** `<LIST OR LINK>`  
**Tests/evidence:** `<EXECUTED STATES OR Not run>`  
**Known blockers:** `<BLOCKERS>`  
**Open questions:** `<QUESTIONS>`  
**Implementation authorization:** `<NONE OR EXACT BOUNDED PLAN>`  
**Commit/push:** `<STATUS/COMMIT IF KNOWN>`  
**Stop point:** `<WHAT MUST NOT BEGIN YET>`

## Required reading order

1. Repository README
2. Suite Graph Roadmap or package documentation index
3. SFGSS-000 when suite boundaries matter
4. Active package specification or standard
5. Applicable ADRs and integration specifications
6. Current Notes
7. Current checkpoint, tests, issues, and implementation

## Fresh-conversation prompt

> We are continuing `<PROJECT>`. Treat `<AUTHORITY>` as the source of truth for `<SCOPE>`. Current checkpoint: `<CHECKPOINT>`. Current status: `<STATUS>`. Known blockers: `<BLOCKERS>`. Evidence state: `<STATE>`. Before changing files, summarize the relevant boundaries, identify any conflict or missing decision, and continue only within the approved checkpoint. Do not infer missing architecture from old chat history.
