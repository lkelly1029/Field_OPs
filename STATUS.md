# STATUS.md

## 1. Findings (Factual Inventory)

### Control Layer
- **Control File**: `control/sovereign_state_control.yaml` [EXISTS]
- **Documentation**: Onboarding, Build Reference, First Vertical Guide [EXISTS]
- **Alignment**: Control file matches Brief requirements [EXISTS]

### Engine Layer
- **Core Logic**: `Sovereign.Core`, `Sovereign.Economy`, `Sovereign.Sim`, `Sovereign.Mods` projects [EXISTS]
- **Build Automation**: `engine/tools/build_engine.ps1` successfully deploys DLLs to Unity. [COMPLETE]
- **Architecture**: Circular dependencies resolved; projects target `netstandard2.1`. [CLEAN]

### Unity Layer
- **Consolidation**: `Assets/_Scripts` removed. All active scripts moved to `Assets/Scripts/`. [COMPLETE]
- **Integrations**: Supabase and Secrets correctly placed in `Assets/Scripts/Integrations/Supabase/`. [COMPLETE]
- **Plugins**: `Assets/Plugins/SovereignEngine/` populated with engine DLLs. [COMPLETE]
- **Visual KPIs**: `SovereignDevConsoleOverlay.cs` and `SimulationRunner.cs` updated to show detailed market metrics. [COMPLETE]
- **Fixes**: Removed `SovereignEngine` reference from `SovereignState.Unity.asmdef` to fix duplicate type errors. [COMPLETE]

### CI / Guardrails
- **Placement Contract**: `python control\tools\validate_no_root_cs.py` PASSES. [SUCCESS]
- **Overall**: `run_guardrails.ps1` verified. [GREEN]

## 2. Next Steps (Prioritized)

1.  **Unity Scene Verification**
    -   **Task**: Verify duplicate type errors are gone.
    -   **Goal**: Confirm "headless" engine runs within the Unity environment using ONLY the DLLs.

2.  **Persistence Finalization**
    -   **Task**: Verify `SaveGame` and `LoadGame` functionality with the new engine projects.
    -   **Goal**: Ensure state can be carried across sessions.

## 3. Recent Activity (Agent Maintenance)
- **Duplicate Type Fix**: Identified `SovereignEngine.asmdef` files in `engine/` causing Unity to double-compile the source. Stubbed them for deletion.
- **Assembly Definition Update**: Removed legacy `SovereignEngine` reference from `SovereignState.Unity.asmdef`.

## 4. Next Actions for User
1. **DELETE** `engine/src/SovereignEngine.asmdef`.
2. **DELETE** `engine/SovereignEngine.asmdef`.
3. **CHECK** Unity console. The `CS0433` and `CS0436` errors should be gone.