# GEMINI_AGENT_BRIEF.md
Generated: 2025-12-26 03:03

## Purpose
This file is the single entry point for the Gemini VS Code assistant.

When you open this file and ask Gemini to follow it, Gemini must:
1) Read the **Sources of Truth** listed below (in order).
2) Scan the repository to compare **what exists** vs **what the docs require**.
3) Produce the **next steps** to reach the current milestone (Power Vertical MVP).
4) Update `STATUS.md` with the current factual state and next steps.

## Repository Root (Windows)
`C:\Users\trulu\Field_OPs`

## Sources of Truth (READ FIRST, IN THIS ORDER)
1. `control/sovereign_state_control.yaml`
2. `control/SovereignState_Build_Reference.md`
3. `control/SovereignState_Unity_Placement_Reference.md`
4. `control/SovereignState_Dev_Onboarding_Checklist.md`
5. `control/SovereignState_First_Vertical_Replication_Guide.md`
6. `README.md` (root)

## Operating Rules (NON-NEGOTIABLE)
- Do NOT invent files, folders, or systems that do not exist.
- If a required item is missing, explicitly mark it as **MISSING** and propose:
  - exact file path
  - minimal contents needed
  - why it is required
- Unity is **presentation only**:
  - Unity code may NOT contain simulation, pricing, ledger math, or tick progression logic.
- Engine is **headless**:
  - Engine code may NOT reference `UnityEngine` or `UnityEditor`.
- Protect secrets:
  - Never print or request secret values.
  - Do not move or delete `Secrets.*` files.
- Do NOT touch `.github/**` unless explicitly requested by the user.

## Output Requirements (Single Response)
Gemini must produce, in this order:

### A) Findings (Factual Inventory)
- List what exists vs missing, grouped by:
  - Control layer (`control/**`)
  - Engine layer (`engine/**`)
  - Unity layer (`Assets/**`)
  - CI/guardrails (`control/tools/**`, `.github/**`)

### B) Next Steps (Prioritized)
- Provide 5–12 steps.
- Each step must include:
  - the exact file(s) to edit/create
  - the intended change
  - acceptance condition (“done when …”)

### C) Update `STATUS.md`
- Create `STATUS.md` if missing, otherwise update it.
- Keep the update factual and concise.
- Do not rewrite history; just reflect current state and next actions.

## Required Repo Scan Tasks
Perform these checks while scanning:

1) Guardrails presence
- Confirm:
  - `control/tools/validate_control.py`
  - `control/tools/validate_repo_guardrails.py`
  - `run_guardrails.ps1`

2) Unity Dev Console presence
- Confirm:
  - `Assets/Scripts/SimBridge/ISimDebugProvider.cs`
  - `Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs`
- Determine whether the debug provider is wired in any scene/runner.

3) Engine Scenario presence
- Confirm:
  - `engine/**/PowerAcceptance.cs` (or equivalent)
- Determine if it is scaffold-only or fully implemented.

4) Control file alignment
- Confirm that `control/sovereign_state_control.yaml` matches the structure described in docs.

## Write Permissions
- In this run, Gemini may edit/create:
  - `STATUS.md`
- Gemini may propose other edits but must NOT apply them unless asked.

## End Condition
The run is complete only when:
- A, B, and C are delivered, and `STATUS.md` is updated.
