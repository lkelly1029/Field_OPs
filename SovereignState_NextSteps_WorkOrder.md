# Sovereign State — Next Steps Work Order (Post-Scaffold Build Plan)
Generated: 2025-12-27 01:51

## Context
Guardrails, repo structure, and the initial headless-engine + Unity harness exist. This work order shifts the team from
**scaffolding** to **productizing the loop** (player-facing feedback + deterministic economics + frictionless vertical replication).

## Non‑Negotiables (Carry Forward)
1) **No `.cs` files at repo root** (must pass):
   - `python control/tools/validate_no_root_cs.py`
2) Engine (`engine/**`) must remain **headless**:
   - No `UnityEngine` / `UnityEditor` references.
3) Unity (`Assets/**`) remains **presentation/orchestration only**:
   - No pricing, ledger math, tick progression rules in Unity scripts.
4) Secrets stay protected:
   - Do not move/print secrets; keep `.github/**` intact.

---

# Milestone A — Stop File Drift Forever (CI Guardrails)
## A1) Ensure guardrails run in CI
Add a CI workflow that runs **guardrails on every push/PR**:
- Run repo checks (structure + no-root-cs + control validation).
- Fail fast on violations.

**Acceptance condition**
- A PR that introduces a root-level `.cs` file fails CI.

## A2) Add “engine must not reference Unity” guardrail (if missing)
Create a scanner under `control/tools/` that fails if any file under `engine/**` contains:
- `UnityEngine`
- `UnityEditor`

Wire into `run_guardrails.ps1`.

**Acceptance condition**
- Any Unity reference under `engine/**` fails guardrails.

---

# Milestone B — Market UI + MarketSnapshot (Make the USP Visible)
This is the highest-leverage player-facing deliverable: show the AI being displaced by player listings.

## B1) Engine: Emit `MarketSnapshot` per tick
Add/extend an engine DTO (example):
- `BestDeliveredPriceByResource`
- `OfferCountByResource`
- `TopOffersByDeliveredCost` (limit 5–10)
- `AiMarketSharePct` over last N ticks (per resource)
- `PlayerMarketSharePct` over last N ticks

**Acceptance condition**
- A headless scenario can print a `MarketSnapshot` showing AI vs Player best price for Power.

## B2) Unity: Display market data in Dev Console (or a simple panel)
Add a panel/tab:
- Cheapest source (AI vs Player)
- AI share %
- Offer count
- Top offers (price + delivered cost)

**Acceptance condition**
- In Play Mode, you can watch AI share % decrease as player exports undercut AI.

---

# Milestone C — Close the Economic Loop (Taxes + UBI + Stability Feedback)
## C1) Implement clear UBI issuance + policy modifiers
Define:
- Tick frequency of UBI
- Policy parameters that modify UBI (gov style)

**Acceptance condition**
- Treasury changes are deterministic and logged in ledger per tick.

## C2) Implement taxation / fees (minimal viable)
Add one of:
- Household tax
- Service fee
- Production tax

**Acceptance condition**
- Households/plots remit taxes; ledger shows the flow and the treasury receives it.

## C3) Surface stability thresholds in UI
For blackout/shortage-driven decay:
- Define thresholds (e.g., shortage ratio > X for Y ticks)
- Surface them in tooltips / UI warnings

**Acceptance condition**
- Player can predict “slum/abandoned” transitions from visible metrics.

---

# Milestone D — Vertical Replication Audit (Power → Water must be frictionless)
Even if Water “exists,” we must ensure it uses the same primitives and has no special-case hacks.

## D1) Audit Water pipeline uses identical primitives
Confirm Water uses the same pattern:
Requests → Resolve (Local / Exchange / AI) → Delivery → Settlement → Ledger reconciliation

**Acceptance condition**
- Water can be enabled/disabled without any Unity special-casing beyond displaying a different resource.

## D2) Add Water acceptance scenario mirroring Power
Create `WaterAcceptance` scenario with the same structure as Power’s:
- Dependency era (AI import expensive)
- Import substitution (local production reduces imports)
- Export displacement (player listing undercuts AI)

**Acceptance condition**
- Scenario passes headless and prints a concise PASS summary.

## D3) Ensure Dev Console switches between resources without hacks
Implement a resource selector in Dev Console:
- Power / Water (and later others)

**Acceptance condition**
- One UI path renders snapshots for any resource type.

---

# Milestone E — Player-Facing HUD (Beyond the Dev Console)
Keep the Dev Console for internal validation, but add one minimal player HUD.

## E1) Implement a minimal HUD panel
Show:
- Treasury
- Tick
- Power balance (produced/consumed/imported/exported)
- Water balance (same fields)
- Alert banner: Blackout / Water Shortage

**Acceptance condition**
- A non-developer can interpret what’s happening without opening the dev console.

---

# Milestone F — Deterministic Save/Load + Replay (Async MMO Foundation)
## F1) Add Save/Load determinism test
Test:
Save at tick T → Load → Run N ticks → Snapshot hash equals baseline

**Acceptance condition**
- Test is repeatable and deterministic locally + in CI.

## F2) Add command-log replay harness
Record player actions (zone/build/list offers) as commands and replay them.

**Acceptance condition**
- Replay produces identical snapshots and ledger outputs.

---

# Execution Order (Do In This Sequence)
1) Milestone A (CI guardrails) — stop drift permanently
2) Milestone B (MarketSnapshot + UI) — make USP visible
3) Milestone C (Taxes/UBI/Stability) — close the loop
4) Milestone D (Water audit + acceptance) — prove replication
5) Milestone E (HUD) — move toward a playable prototype
6) Milestone F (Save/Load + Replay) — async MMO readiness

---

## Status Update Instructions
After completing any milestone:
- Update `STATUS.md` with:
  - Guardrails/CI state
  - What changed (paths + features)
  - What is next (top 3 tasks)
