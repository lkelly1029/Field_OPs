# Sovereign State

**Sovereign State** is a headless, deterministic, tick-based economic simulation where cities emerge from supply chains and market logic—not from painted stats.

This repository is structured to protect the simulation core, enable mod-native economics, and allow Unity to remain a presentation layer only.

---

## 60-Second Orientation

**What this is**
- A **gamified supply-chain ERP**
- An **AI-to-player market displacement simulator**
- A **city builder where spreadsheets determine skylines**

**What this is not**
- A real-time traffic simulator
- A Unity-first game
- A cosmetic city painter

The player progresses by replacing an expensive AI monopoly with efficient player-run production, eventually exporting surplus to other isolated universes via a global exchange.

---

## Repository Structure (At a Glance)

```
SovereignState/
├─ control/     ← Design truth (humans + AI)
├─ engine/      ← Deterministic simulation (pure C#)
├─ unity/       ← Presentation & input (no business logic)
└─ README.md
```

---

## Authoritative Reference Files (READ THESE FIRST)

All design, rules, and guardrails live in `/control/`.

### 1. SovereignState_Build_Reference.md
**What we are building and why**
- Vision
- System hierarchy
- Logic trees
- Tick pipeline
- MVP scope (Power)
- Testing strategy

### 2. sovereign_state_control.yaml
**What is allowed**
- Non-negotiables
- Invariants
- Milestones
- Interfaces
- Acceptance criteria

This file is the **build contract**.

### 3. SovereignState_Unity_Placement_Reference.md
**Where things live**
- What Unity may and may not do
- Repo boundaries
- AI grounding rules

This file prevents Unity from becoming the architecture.

---

## Golden Rule

> **If Unity needs it to run → `Assets/`**  
> **If humans or AI need it to think → `/control/`**

---

## How to Work in This Repo

1. Read the three control files.
2. Build engine logic first.
3. Add tests.
4. Only then expose state to Unity.

If a feature contradicts `/control`, the feature is wrong.

---

**Start here:** `/control/SovereignState_Build_Reference.md`
