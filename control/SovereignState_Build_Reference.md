# Sovereign State — Build Reference (Hierarchy + Logic Tree + Control File)

**Purpose**
- Single in-repo reference for builders and for AI agents assisting the build.
- Captures: vision, pillars, systems, tick loop, architecture, MVP (Power), milestones, invariants, tests, and work rules.

**Scope**
- This file summarizes the full plan discussed in this chat.
- MVP vertical: **Power**. The architecture is designed to scale to all resources via data + vertical replication.

---

## 1) Hierarchy (What the Game Is)

### 1.1 Project Identity
- **Project:** Sovereign State (Working Title)
- **Genre:** Asynchronous MMO / Hardcore Economic Simulation / Region Builder
- **Core premise:** Data-driven region builder simulating real-world economic friction via physicalized resources and market displacement.
- **Primary differentiator:** **Meta-Universe** structure (isolated universes, shared market).

### 1.2 The Core Vision
- Resources are not “stats” painted onto a map; they are **physical commodities**:
  - origin points
  - logistical cost / loss
  - market value
  - traceable flow end-to-end
- City visuals are an *output* of economic performance:
  - bad logic → blackout/shortages → decay
  - good logic → stable supply chains → export dominance

### 1.3 Meta-Universe Structure
- Each player’s region exists in a private **Universe**.
- Players do not see other players’ cities.
- Players must trade with others to survive:
  - The multiplayer surface is **price signals + offers** (Global Exchange), not visibility or direct interaction.

---

## 2) Pillars (Non-Negotiables)

### 2.1 Physicalized Resources
- Every unit is traceable in the **Ledger**.
- If it cannot be reconciled via the ledger, it does not exist.

### 2.2 AI as a Temporary Monopoly (AI Director)
- AI is the initial sole supplier (“Global Market”).
- AI supply is:
  - always available
  - expensive
  - predictable
  - intentionally beatable
- The player’s long-term goal is market displacement:
  - “retire the AI” from segments by undercutting it.

### 2.3 Isolation + Interdependence
- Universes are visually isolated; only the market crosses boundaries.
- No spying, no griefing, no direct coordination required.

### 2.4 Deterministic Tick Simulation
- The sim runs on a strict tick loop (not real-time physics).
- Two identical states + identical inputs → identical outcomes (within determinism constraints).

### 2.5 Failure is Structural
- Failure comes from economics/logistics, not arbitrary punishment.
- Decay is feedback:
  - Active → Slum → Abandoned if chronic shortages

### 2.6 Mod-Native “Physics”
- Government style and other “physics” are loaded from external data (JSON).
- Community/AI can balance without code changes.

---

## 3) Progression Arc (AI-to-Player)

### 3.1 Phase 1 — Dependency Era
- World starts empty.
- Player must import all key inputs.
- AI Director provides synthetic supply at high price.

### 3.2 Phase 2 — Import Substitution
- Player invests UBI into means of production.
- Player replaces imports with local production.

### 3.3 Phase 3 — Export Tycoon
- Player produces surplus.
- Player lists on Global Exchange.
- New universes see player offers cheaper than AI and buy them.
- Player displaces AI from that commodity segment.

---

## 4) Gameplay Loop (Tick System)

### 4.1 Input (Setup)
- Player selects:
  - Government Style (Democracy/Technocracy/Dictatorship) from external JSON
  - Architecture Pack (e.g., Cyberpunk/Victorian) from external JSON
- These define economy “physics”:
  - taxes, tariffs
  - UBI distribution
  - build cost/time multipliers
  - growth modifiers

### 4.2 Action (Build)
- Player spends UBI to zone/build on plots.

### 4.3 Simulation (Logistics)
- Plots request resources (Power/Water/Materials/etc).
- Resolver checks the Global Ledger and Market:
  - local supply?
  - exchange supply?
  - AI supply?

### 4.4 Result (State Change)
- If delivered:
  - plot progresses (Ghost/Construction/Active…)
- If failed (shortage/blackout):
  - plot decays (Slum/Abandoned)

---

## 5) Technical Architecture (Headless Engine First)

### 5.1 Scaffold-First Approach
- Ignore roads/traffic/pedestrians for MVP.
- Focus on **Volume & Flow**.

### 5.2 Core Systems
- **Ledger:** records every resource and monetary movement
- **State Machine:** plot lifecycle transitions
- **Market Layer:** AI Global Market + Global Exchange
- **External Mods:** JSON-driven balance & content

### 5.3 Unity Approach
- Unity is the client/renderer and input layer.
- Engine runs as a pure C# library (runnable without Unity).

---

## 6) Current Focus: Power Vertical MVP

### 6.1 Power MVP Goal
- Prove end-to-end commodity flow:
  - plant produces power
  - power transmitted via “grid math” (loss + transport fee)
  - consumer consumes power
  - settlement pays supplier

### 6.2 Canonical MVP Example
- Nuclear Plant in Universe A generates 100MW.
- It travels through grid abstraction (loss calculation).
- A House consumes it and pays the plant.
- AI supply fills gaps at premium price.
- Surplus can be exported via Global Exchange.

### 6.3 Power MVP Success Criterion
- Player can harm themselves economically through bad decisions (e.g., pricing/overbuild), meaning the model is real.

---

## 7) Non-Negotiable Technical Decisions (Lock Now)

### 7.1 Engine-first, Unity-second
- Economic engine must compile/run without Unity.
- Unity must not contain business logic for ledger, pricing, or settlement.

### 7.2 Determinism Baseline
- Prefer integer math:
  - Money in cents (int64)
  - Power in Wh (int64)
  - Distance in meters (int32)
- Tick length default: 1 hour (configurable)

### 7.3 Mod Pipeline
- Government styles, building archetypes, and physics are JSON.
- Engine consumes validated DTOs; Unity only loads/presents.

---

## 8) Repo & Project Layout (Mono-repo)

```
SovereignState/
  README.md
  control/
    sovereign_state_control.yaml
    schemas/
      government.schema.json
      building.schema.json
      resource.schema.json
  engine/
    Sovereign.Engine.sln
    src/
      Sovereign.Core/
      Sovereign.Sim/
      Sovereign.Economy/
      Sovereign.Mods/
      Sovereign.Serialization/
      Sovereign.Tests/
  unity/
    SovereignState.Unity/
      Assets/
      Packages/
      ProjectSettings/
```

---

## 9) Simulation Model (Canonical Entities)

### 9.1 Universe
- Container for:
  - plots
  - local producers/consumers
  - local inventory buffers (optional)
  - local orders

### 9.2 Tick
- Discrete step:
  - produce
  - request
  - resolve
  - deliver
  - settle
  - update states
  - publish offers

### 9.3 Ledger
- Source of truth for:
  - resource units
  - money transfers
  - audit & reconciliation

### 9.4 Resource
- A commodity type (Power for MVP).
- Future resources plug into the same pipeline via data.

### 9.5 Plot (State Machine)
- Vacant → Surveyed → Serviced → Construction → Active
- Failure:
  - Active → Slum → Abandoned

### 9.6 Market
- AI Global Market (premium synthetic supply)
- Global Exchange (player offers)

---

## 10) Tick Pipeline (Deterministic Phases)

1. TickStart
2. UpdateProducers
3. CollectRequests
4. ResolveSupply (Local → Exchange → AI)
5. ApplyDeliveries (loss + fees)
6. SettlePayments
7. UpdatePlotStateMachines
8. PublishExchangeOffers
9. TickEnd

---

## 11) Unity Architecture (Client)

### 11.1 Unity Responsibilities
- Map rendering + plot UI
- dashboards & charts (why things failed)
- input to engine via commands
- mod discovery (StreamingAssets)
- save/load integration (via engine serialization)

### 11.2 Engine Boundaries
- Unity does not:
  - calculate prices
  - settle the ledger
  - mutate internal sim state except by commands

### 11.3 Recommended MVP Execution
- Single-player local sim (engine in-process).
- Exchange is a local stub service, later replaced with server.

---

## 12) Power MVP Details (Implementation Targets)

### 12.1 Entities
- Producer: NuclearPlant
- Consumer: House
- Grid abstraction:
  - distance scalar
  - loss factor by distance
  - transport fee by distance

### 12.2 Matching & Settlement Rules
- Resolve cheapest delivered total cost:
  - unit price + transport + loss
- AI offer always available at premium price.
- Payment flows:
  - consumer pays supplier
  - optional exchange fee later

---

## 13) Resource Coverage Clarification (Important)

- The plan defines a **generic resource architecture** and import/export pipeline.
- It does **not** enumerate every resource type and its specialized variables yet—intentionally.
- After Power proves the pattern, expand via a **Resource Specification Matrix** (data-only), reusing the same systems.

---

## 14) Testing Strategy (Mandatory)

### 14.1 Unit Tests
- Ledger_Reconciles_PerTick
- Resolver_Selects_Cheapest_Offer
- Plot_Decay_On_Unserved_Demand
- Exchange_Matching_Respects_Qty_And_Price
- Government_Modifiers_Apply_Correctly

### 14.2 Scenario Tests
- DependencyEra_AIOnly
- ImportSubstitution_LocalPlant
- ExportTycoon_Displacement

---

## 15) Work Rules (Process)

- No feature enters Unity unless engine tests exist for its logic.
- Every new resource vertical must reuse:
  - ledger, orders, resolver, delivery, settlement
- If UI cannot explain a failure, feature is not complete.

---

# 16) Logic Tree (Cause → Effect)

## 16.1 Player Decision Tree
Player chooses:
- Government Style + Architecture Pack
  → economy parameters set (tax/UBI/build costs/growth)

Player action:
- Zone/build plot
  → plot requests resources each tick

Resolver checks supply:
- Local supply exists?
  - Yes → deliver locally
  - No → check exchange
    - Exchange offer exists?
      - Yes → import from cheapest offer
      - No → AI supplies at premium

Delivery outcome:
- Delivered meets demand
  → plot stability increases → progresses toward Active
- Delivered fails
  → plot stability decreases → Slum → Abandoned

Player progression:
- Imports expensive AI inputs
  → invests in production
  → produces surplus
  → lists offers
  → displaces AI for other players

## 16.2 Market Displacement Loop
AI sets high baseline price
→ player produces cheaper
→ exchange lists player price
→ new universes import cheaper
→ AI share declines
→ player becomes “tycoon” supplier

---

# 17) Control File (Copy into /control/sovereign_state_control.yaml)

(See the companion YAML file generated alongside this document.)

