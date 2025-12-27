# STATUS.md

## 1. Findings (Factual Inventory)

### Control Layer
- **Control File**: `control/sovereign_state_control.yaml` [EXISTS]
- **Documentation**: Onboarding, Build Reference, First Vertical Guide [EXISTS]
- **Alignment**: Control file matches Brief requirements [EXISTS]

### Engine Layer
- **Core Logic**: `Sovereign.Core`, `Sovereign.Economy`, `Sovereign.Sim` projects [EXISTS]
- **Scenarios**: `PowerAcceptance.cs`, `WaterAcceptance.cs`, `IntegrationAcceptance.cs`, `IndustrialAcceptance.cs` [EXISTS]
- **Legacy Artifacts**: `engine/src/world`, `simulation`, etc [BLOCKED - Shell Restricted] - Marked with NOTICE file.
- **Multi-Resource Support**: **IMPLEMENTED**.
- **Water Vertical**: **IMPLEMENTED & VERIFIED**.
- **Deep Integration**: **IMPLEMENTED**.
- **Metrics**: **IMPLEMENTED**.
- **Resource Matrix**: **EXPANDED**.
- **Storage Buffers**: **IMPLEMENTED**.
- **Storage Caps**: **IMPLEMENTED**.
- **Resource Decay**: **IMPLEMENTED**.
- **Steel Vertical**: **IMPLEMENTED**.
- **Iron Vertical**: **IMPLEMENTED**.
- **Refinery Logic**: **IMPLEMENTED & VERIFIED**.
- **Financial Dashboard**: **IMPLEMENTED**.
- **Industrial Chain**: **VERIFIED**.
- **Logistics**: **IMPLEMENTED**.

### Unity Layer
- **Dev Console UI**: `Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs` [EXISTS - Updated with Visual Bars]
- **Sim Bridge Interface**: `Assets/Scripts/SimBridge/ISimDebugProvider.cs` [EXISTS]
- **Sim Bridge Implementation**: `Assets/Scripts/SimBridge/SimulationRunner.cs` [EXISTS]
- **Plot Renderer**: `Assets/Scripts/SimBridge/PlotRenderer.cs` [UPDATED]. Now correctly links Engine `Plot` data to Unity `GameObject` visuals using X, Y coordinates.
- **Scene**: `Assets/Scenes/SampleScene.unity` [EXISTS]

### CI / Guardrails
- **Scripts**: `validate_control.py`, `validate_repo_guardrails.py`, `run_guardrails.ps1` [EXISTS]

## 2. Next Steps (Prioritized)

1.  **Player Input (Plot Interaction)**
    -   **Path**: `Assets/Scripts/SimBridge/PlotClicker.cs` (New)
    -   **Task**: Implement a raycasting script that detects clicks on plot cubes and toggles their state/building.

2.  **Save/Load**
    -   **Path**: `engine/src/Sovereign.Serialization/UniverseState.cs` (New)
    -   **Task**: Use `System.Text.Json` to serialize the `Universe` and its `Plots`.

3.  **Refine Unity UI**
    -   **Task**: Add resource-specific icons or better formatting to the console overlay.
