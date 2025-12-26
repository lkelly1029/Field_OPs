# Sovereign State — Developer Onboarding Checklist

This checklist ensures every developer understands the architecture before writing code.

---

## Day 1 — Orientation & Mental Model

### Read (Required)
- [ ] SovereignState_Build_Reference.md
- [ ] sovereign_state_control.yaml
- [ ] SovereignState_Unity_Placement_Reference.md

### Understand These Concepts
- [ ] Headless engine vs Unity client
- [ ] Tick-based deterministic simulation
- [ ] Ledger as source of truth
- [ ] AI Global Market vs Global Exchange
- [ ] Power as the canonical vertical

### Setup
- [ ] Clone repo
- [ ] Open `/engine/Sovereign.Engine.sln`
- [ ] Build engine projects
- [ ] Run unit tests
- [ ] Open Unity project (do not edit Assets yet)

---

## Day 1 — Rules You Must Not Break

- [ ] Do NOT add simulation logic to Unity
- [ ] Do NOT bypass the ledger
- [ ] Do NOT introduce real-time mechanics
- [ ] Do NOT hardcode economy values
- [ ] Do NOT invent systems not in `/control`

---

## Week 1 — Productive Contribution

### Engine
- [ ] Trace the tick pipeline end-to-end
- [ ] Step through a Power tick in debugger
- [ ] Understand producer → resolver → delivery → settlement

### Unity
- [ ] Identify SimBridge boundary
- [ ] Observe snapshot-driven UI updates
- [ ] Confirm Unity never mutates sim state directly

### Tests
- [ ] Read at least one unit test and one scenario test
- [ ] Add or modify a test before touching gameplay code

---

## Graduation Criteria (Week 1 Complete)

A developer is considered onboarded when they can:
- Explain the full Power flow without notes
- Identify where a feature belongs (engine vs Unity)
- Say “no” to a feature that violates `/control`

---

**If you are unsure, re-read `/control`.**
