# GEMINI_AGENT_BRIEF.md
Single Entry Point for AI Build Review & Coordination

Generated: 2025-12-25


================================================================
PURPOSE (AUTHORITATIVE)
================================================================

This file is the ONLY instruction Gemini should follow when
working in this repository.

When instructed to follow this brief, Gemini MUST:

1. Read the Sources of Truth (in order).
2. Scan the repository filesystem and code.
3. Compare what EXISTS vs what is REQUIRED.
4. Determine the next concrete steps toward the current milestone.
5. Update STATUS.md with the current factual state.

No other behavior is permitted.


================================================================
REPOSITORY ROOT (AUTHORITATIVE)
================================================================

Windows path:

C:\Users\trulu\Field_OPs

All paths referenced below are RELATIVE to this root.


================================================================
SOURCES OF TRUTH (READ FIRST, IN ORDER)
================================================================

These define architecture and rules. Do NOT contradict them.

1. control/sovereign_state_control.yaml
2. control/SovereignState_Build_Reference.md
3. control/SovereignState_Unity_Placement_Reference.md
4. control/SovereignState_Dev_Onboarding_Checklist.md
5. control/SovereignState_First_Vertical_Replication_Guide.md
6. README.md (repo root)


================================================================
OPERATING RULES (NON-NEGOTIABLE)
================================================================

1. NO INVENTION
- Do NOT invent files, folders, systems, or architecture.
- If a required item is missing, mark it as MISSING and propose:
  - exact file path
  - minimal contents
  - reason it is required

2. STRICT LAYER SEPARATION
- Unity (Assets/**):
  - Presentation only
  - NO simulation logic
  - NO ledger, pricing, or tick math
- Engine (engine/**):
  - Headless only
  - NO UnityEngine or UnityEditor references
- Control (control/**):
  - Design truth, schemas, guardrails, documentation

3. SECURITY & SECRETS
- Never print, request, or move secret values
- Do NOT touch:
  - .github/**
  - Assets/_Scripts/SupabaseManager.cs
  - Assets/_Scripts/ConstructionMaterial.cs
- Do NOT delete or move Secrets.* files
- Secrets must remain git-ignored


================================================================
FILESYSTEM ENFORCEMENT (MANDATORY)
================================================================

This section is STRICTLY ENFORCED.

HARD RULES:
- Every file MUST be created in its correct directory
- Repo root is NOT a default dumping ground
- If a directory does not exist, CREATE IT FIRST
- If unsure of the correct path, STOP AND ASK

------------------------------------------------
CANONICAL PATHS (AUTHORITATIVE)
------------------------------------------------

ROOT-ONLY FILES (ONLY THESE MAY LIVE AT ROOT):
- README.md
- STATUS.md
- run_guardrails.ps1

CONTROL LAYER:
control/
  sovereign_state_control.yaml
  tools/
    validate_control.py
    validate_repo_guardrails.py
  *.md   (design, onboarding, references)

ENGINE LAYER:
engine/
  src/
  scenarios/
    PowerAcceptance.cs
  tests/

UNITY LAYER:
Assets/
  Scripts/
    SimBridge/
    DevTools/
  Scenes/
  Settings/

If Gemini creates a file:
- It MUST announce the full path first
- It MUST match one of the above categories


================================================================
REQUIRED REPOSITORY SCAN TASKS
================================================================

Gemini MUST verify the following:

1. GUARDRAILS
- control/tools/validate_control.py
- control/tools/validate_repo_guardrails.py
- run_guardrails.ps1

2. UNITY DEV CONSOLE
- Assets/Scripts/SimBridge/ISimDebugProvider.cs
- Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs
- Determine whether the provider is wired into a runner or scene

3. ENGINE SCENARIOS
- engine/**/PowerAcceptance.cs
- Determine whether it is scaffold-only or implemented

4. CONTROL ALIGNMENT
- control/sovereign_state_control.yaml matches documentation


================================================================
OUTPUT REQUIREMENTS (SINGLE RESPONSE)
================================================================

Gemini MUST output, IN THIS ORDER:

A) FINDINGS (FACTUAL INVENTORY)
- Grouped by:
  - Control layer
  - Engine layer
  - Unity layer
  - CI / Guardrails
- State only: EXISTS / MISSING / PARTIAL

B) NEXT STEPS (PRIORITIZED)
- Provide 5–12 steps
- Each step MUST include:
  - exact file path(s)
  - what to implement or change
  - acceptance condition (“done when …”)

C) STATUS.md UPDATE
- Create STATUS.md if missing
- Otherwise, update it
- Keep changes factual and concise
- Do NOT rewrite history

IN THIS RUN, GEMINI MAY ONLY EDIT:
- STATUS.md

All other changes must be PROPOSED, not applied.

ABSOLUTE PROHIBITION:
- No .cs files may exist at repo root.
- Any engine-domain class MUST live under engine/src/**.
- If Gemini creates a .cs file at root, that is a failure.

================================================================
END CONDITION
================================================================

The task is complete ONLY when:
- Findings are delivered
- Next steps are listed
- STATUS.md is updated accurately

If any rule conflicts with speed or convenience:
THE RULE WINS.
