# Sovereign State — Repository & Unity Integration Reference

**Purpose**
This document explains *where* Sovereign State reference files live, *why* they live there,
and *how* builders and AI tools should use them during development.

This file complements:
- `SovereignState_Build_Reference.md`
- `sovereign_state_control.yaml`

It exists to prevent architectural drift, Unity overreach, and AI hallucinated systems.

---

## 1. Guiding Principle

> **If Unity needs it to RUN → it lives in `Assets/`**  
> **If humans or AI need it to THINK → it lives in `/control/`**

This rule is non‑negotiable.

---

## 2. Canonical Repository Layout

```
SovereignState/
├─ README.md
├─ control/
│  ├─ SovereignState_Build_Reference.md
│  ├─ SovereignState_Unity_Placement_Reference.md   ← (THIS FILE)
│  ├─ sovereign_state_control.yaml
│  └─ schemas/
│     ├─ government.schema.json
│     ├─ building.schema.json
│     └─ resource.schema.json
├─ engine/
│  ├─ Sovereign.Engine.sln
│  └─ src/
│     ├─ Sovereign.Core/
│     ├─ Sovereign.Sim/
│     ├─ Sovereign.Economy/
│     ├─ Sovereign.Mods/
│     ├─ Sovereign.Serialization/
│     └─ Sovereign.Tests/
└─ unity/
   └─ SovereignState.Unity/
      ├─ Assets/
      ├─ Packages/
      └─ ProjectSettings/
```

---

## 3. What Goes in `/control/` (Authoritative Thinking Layer)

### 3.1 Files in `/control/`
| File | Purpose |
|----|----|
| `SovereignState_Build_Reference.md` | Full system hierarchy, logic tree, vision |
| `sovereign_state_control.yaml` | Machine‑readable build contract |
| `schemas/*.json` | Validation schemas for mod data |
| `SovereignState_Unity_Placement_Reference.md` | Unity + repo usage rules |

### 3.2 What These Files Are
- **Design contracts**
- **Architectural guardrails**
- **AI grounding documents**
- **Onboarding references**

### 3.3 What These Files Are NOT
- Runtime assets
- Unity-imported data
- Gameplay configuration
- Player-facing content

Unity must never depend on these files to function.

---

## 4. Unity-Specific Rules

### 4.1 What Unity May Load
Unity may load ONLY:
- Runtime mod data
- Visual assets
- Presentation scripts
- Serialized save files

### 4.2 What Unity Must NEVER Load
Unity must never:
- Parse `sovereign_state_control.yaml`
- Interpret build reference markdown
- Contain economic logic duplicated from engine
- Make pricing, settlement, or ledger decisions

Unity is a **viewer and controller**, not a simulator.

---

## 5. Correct Unity Asset Structure

```
Assets/
├─ Scripts/
│  ├─ Presentation/
│  ├─ UI/
│  ├─ DevTools/        (optional, editor-only)
│  └─ SimBridge/
├─ StreamingAssets/
│  └─ Mods/
│     ├─ governments.json
│     ├─ buildings.json
│     └─ resources.json
├─ Scenes/
└─ Art/
```

### Key Distinction
| Layer | Lives Where | Responsibility |
|----|----|----|
| Design truth | `/control/` | Humans + AI |
| Simulation | `/engine/` | Deterministic logic |
| Runtime config | `StreamingAssets/Mods` | Data injected into engine |
| Presentation | `Assets/` | Visualization only |

---

## 6. How Builders Use These Files

### 6.1 During Development
- Start every feature by reading:
  - Build Reference
  - Control YAML
- Validate feature intent against:
  - Non‑negotiables
  - Tick pipeline
  - Milestones

### 6.2 During Code Review
Reject PRs that:
- Add simulation logic to Unity
- Bypass the ledger
- Add real-time mechanics
- Introduce resource logic outside the vertical pattern

---

## 7. How AI Tools Use These Files

### 7.1 AI Grounding Rule
Every AI interaction **must** be grounded with at least one of:
- `SovereignState_Build_Reference.md`
- `sovereign_state_control.yaml`

### 7.2 Recommended Prompt Template

```
You are assisting with the Sovereign State project.

Authoritative references:
- /control/SovereignState_Build_Reference.md
- /control/sovereign_state_control.yaml
- /control/SovereignState_Unity_Placement_Reference.md

Rules:
- Engine-first architecture
- Tick-based simulation only
- No Unity-side business logic
- No speculative systems outside the control file

Task:
[insert task]
```

---

## 8. Optional (Advanced) Tooling Use

Later phases MAY include:
- CI checks that:
  - Validate mod JSON against schemas
  - Flag code violating control invariants
- Unity Editor DevTools that:
  - Display markdown for reference
  - Show tick/ledger state visually (read-only)

These are **developer aids**, not gameplay systems.

---

## 9. Enforcement Rule (Final)

> **If a feature contradicts `/control`, the feature is wrong.**  
> **If Unity contains logic that belongs to the engine, it must be removed.**

This file exists to protect the simulation from accidental erosion.

---

**End of Reference**
