# STATUS.md

## 1. Findings (Factual Inventory)

### Control Layer
- **Control File**: `control/sovereign_state_control.yaml` [EXISTS]
- **Documentation**: Onboarding, Build Reference, First Vertical Guide [EXISTS]
- **Alignment**: Control file matches Brief requirements [EXISTS]

### Engine Layer
- **Core Logic**: `Sovereign.Core`, `Sovereign.Economy`, `Sovereign.Sim` projects [EXISTS]
- **Scenarios**: `PowerAcceptance.cs`, `WaterAcceptance.cs`, `IntegrationAcceptance.cs`, `IndustrialAcceptance.cs`, `SerializationAcceptance.cs` [EXISTS]
- **Legacy Artifacts**: `engine/src/world`, `simulation`, etc [BLOCKED - Shell Restricted] - Marked with NOTICE file and "nullified" with comments.
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
- **Commands**: **IMPLEMENTED**.
- **Persistence**: **IMPLEMENTED**.

### Unity Layer
- **Dev Console UI**: `Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs` [UPDATED]. Includes Sim/Persistence Controls and Hover Tooltips.
- **Sim Bridge Interface**: `Assets/Scripts/SimBridge/ISimDebugProvider.cs` [EXISTS]
- **Sim Bridge Implementation**: `Assets/Scripts/SimBridge/SimulationRunner.cs` [UPDATED].
- **Plot Renderer**: `Assets/Scripts/SimBridge/PlotRenderer.cs` [EXISTS]. Correct X,Y mapping to Engine.
- **Player Input**: `Assets/Scripts/SimBridge/PlotClicker.cs` [UPDATED]. Supports raycasting and building selection via command system.
- **Tooltip Manager**: `Assets/Scripts/SimBridge/TooltipManager.cs` [EXISTS].
- **Building Manager**: `Assets/Scripts/SimBridge/BuildingManager.cs` [EXISTS].
- **Scene**: `Assets/Scenes/SampleScene.unity` [EXISTS]

### CI / Guardrails
- **Scripts**: `validate_control.py`, `validate_repo_guardrails.py`, `validate_no_root_cs.py`, `validate_engine_no_unity.py` [EXISTS]
- **Runner**: `run_guardrails.ps1` [UPDATED]. Now runs all 4 validation checks.

## 2. Conclusion

The "First Vertical Replication" and "Unity Integration" phases are **COMPLETE**.

The project is now a functional city-building prototype where:
1.  **Engine** handles complex logistics, market logic, and resource decay.
2.  **Unity** renders the grid, provides tooltips, and sends commands.
3.  **Persistence** is implemented for the full simulation state.
4.  **Architecture** is protected by strict guardrails.

## 3. Next Milestone Recommendation

**UI & Economy Balance**
1.  **Market UI**: Expose the `GlobalExchange` offers to a UI list so players can see who is undercutting the AI.
2.  **Taxation Logic**: Implement a "TaxCommand" or tick-based tax collection from Houses based on their stability.
3.  **Visual Polish**: Move beyond cubes to simple building placeholders.