# Sovereign State: Project Summary & Alignment Review

**Generated:** December 28, 2025
**Version:** Prototype 0.1 (Power Vertical)

---

## 1. What is "Sovereign State"?

**Sovereign State** is a deterministic, tick-based economic simulation game where you play as the central planner (The State) of a developing region ("Universe").

### Core Gameplay Loop
1.  **Build**: You place infrastructure (Power Plants, Water Pumps, Mines) and Zones (Houses, Farms, Factories) on a grid.
2.  **Simulate**: The engine ticks forward. 
    -   **Consumers** (Houses/Factories) demand resources (Power, Water, Food, Iron).
    -   **Producers** (Plants/Mines) generate resources.
3.  **The Economy**:
    -   **Procurement**: The State (Treasury) automatically buys all local production to support the industry.
    -   **Distribution**: The State sells resources to Consumers.
    -   **Import/Export**: If local supply is insufficient, the State automatically imports from the high-priced **AI Global Market**. If there is a surplus, the State lists it on the Global Exchange to undercut the AI.
4.  **The Goal**: Move from **Dependency** (bleeding money on imports) to **Sovereignty** (self-sufficiency) and finally to **Dominance** (exporting for profit).

---

## 2. Technical Architecture (What it Does)

The project is strictly separated into three layers, ensuring the simulation can run headless (on a server) or visualized (in Unity).

### A. The Engine (`engine/src`)
-   **Pure C#**: Zero dependency on Unity. Runs in .NET Console or Test Runner.
-   **Deterministic**: A save file loaded on any machine will produce the exact same future ticks.
-   **Ledger-Based**: Every resource unit (Wh of Power, kg of Food) and every cent of Money is tracked in a double-entry style Ledger. Resources do not disappear; they are consumed or decayed.
-   **Data-Driven**: Buildings and rules are defined in code but structured to be moddable.

### B. The Visualizer (`Assets/Scripts`)
-   **Unity Frontend**: Acts as a "dumb terminal" for the Engine.
-   **16x16 Grid**: Renders the simulation state as color-coded cubes (Magenta=Power, Green=House, etc.).
-   **Dev Console**: Provides real-time metrics (Treasury Balance, Supply/Demand charts).
-   **Input**: WASD Camera control and Point-and-Click building system.

### C. The Verification Suite (`engine/tests`)
-   **13 Scenarios**: Automated tests prove that the economy works as designed before Unity is even opened. We verify that "Building a power plant stops the treasury from draining" mathematically.

---

## 3. Alignment with Initial Vision

| Feature | Initial Vision | Current State | Alignment |
| :--- | :--- | :--- | :--- |
| **Engine** | Headless, tick-based, deterministic | **DONE**. Runs in .NET, verified by tests. | ✅ 100% |
| **Economy** | Ledger-based, physicalized resources | **DONE**. Ledger tracks every cent and unit. | ✅ 100% |
| **Market** | Local -> Exchange -> AI Fallback | **DONE**. Logic handles Imports/Exports dynamically. | ✅ 100% |
| **Visuals** | 3D Isometric View | **PARTIAL**. We have a 3D grid of cubes. Art is placeholder. | ⚠️ 50% (Scope) |
| **Persistence** | Save/Load full state | **DONE**. JSON serialization works. | ✅ 100% |
| **Verticals** | Power, Water, Industrial | **DONE**. All three verticals are implemented in logic. | ✅ 100% |

### Deviations / Discoveries
1.  **State Procurement Model**: We discovered that for the "Treasury" to track game progress, it acts as the middleman. It buys from producers and sells to consumers. This creates a "State Capitalism" vibe initially, which fits the "Sovereign" theme perfectly.
2.  **Consumer Debt**: To bootstrap the economy, we allow Consumers (Houses) to go into debt (`ForceDebit`) so they can buy food/power even if they haven't earned wages yet. This was a necessary logic fix to close the loop.

---

## 4. Conclusion

**Sovereign State** is currently a **Functional Grey-Box Prototype**. 
The "Backend" is production-ready. The "Frontend" is a developer visualization.
You have successfully built a simulation that is **verified correct** by automated tests, a rarity in game development.

**Status**: Ready for Art, UI Polish, and Gameplay Balancing.
