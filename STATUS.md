# STATUS.md

## 1. Findings (Factual Inventory)

### Control Layer
- **Control File**: `control/sovereign_state_control.yaml` [EXISTS]
- **Documentation**: Onboarding, Build Reference, First Vertical Guide [EXISTS]
- **Alignment**: Control file matches Brief requirements [EXISTS]

### Engine Layer
- **Core Logic**: `Sovereign.Core`, `Sovereign.Economy`, `Sovereign.Sim` projects [EXISTS]
- **Scenarios**: `engine/src/Sovereign.Scenarios/PowerAcceptance.cs` [EXISTS - Implemented]
- **Tests**: `Sovereign.Tests` project [EXISTS]
- **Legacy Artifacts**: `engine/src/` legacy folders cleanup script executed. [RESOLVED]
- **Universe.Tick**: Implements Money/Market (Macro) and Plot-level state updates. [COMPLETE]

### Unity Layer
- **Dev Console UI**: `Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs` [EXISTS]
- **Sim Bridge Interface**: `Assets/Scripts/SimBridge/ISimDebugProvider.cs` [EXISTS]
- **Sim Bridge Implementation**: `Assets/Scripts/SimBridge/SimulationRunner.cs` [EXISTS]
- **Scene**: `Assets/Scenes/SampleScene.unity` [EXISTS]

### CI / Guardrails
- **Scripts**: `validate_control.py`, `validate_repo_guardrails.py`, `run_guardrails.ps1` [EXISTS]

## 2. Next Steps (Prioritized)

1.  **Implement Save/Load in Unity**
    -   **Path**: `Assets/Scripts/SimBridge/SimulationRunner.cs`
    -   **Task**: Uncomment and finalize the `SaveGame` and `LoadGame` methods using `UniverseSerializer`.
    -   **Condition**: Can save state to JSON and reload it.
    -   **Status**: COMPLETE

2.  **Implement Government Modifiers (M4)**
    -   **Path**: `engine/src/Sovereign.Mods/GovernmentMod.cs` (New)
    -   **Task**: Create structure to load government policies (tax rates, UBI) from JSON.
    -   **Condition**: Engine can load a policy file.
    -   **Status**: COMPLETE

## 3. Recent Activity (Agent Maintenance)
- **Repo Hygiene**:
  - Moved `GovernmentMod.cs`, `ModLoader.cs`, `Sovereign.Mods.csproj` to `engine/src/Sovereign.Mods/`.
  - Moved `LedgerTests.cs`, `PlotDecayTests.cs` to `engine/src/Sovereign.Tests/`.
  - Moved `Assets/_Scripts/SupabaseManager.cs`, `Assets/_Scripts/ConstructionMaterial.cs` to `Assets/Scripts/Integrations/Supabase/`.
  - **BLOCKED**: Could not delete original files due to shell restrictions. Files have been stubbed with instructions to delete.
  - **BLOCKED**: Could not move `Assets/_Scripts/Secrets.cs` (read restricted). User must move manually.
  - **Guardrails**: Currently failing due to presence of stubbed root `.cs` files.

## 4. Next Actions for User
1. **DELETE** the following stubbed files from repo root:
   - `ResourceType.cs`
   - `MarketSnapshot.cs`
   - `GovernmentMod.cs`
   - `ModLoader.cs`
   - `LedgerTests.cs`
   - `PlotDecayTests.cs`
   - `Sovereign.Mods.csproj`
2. **DELETE** `Assets/_Scripts` folder after verifying `Secrets.cs` is moved to `Assets/Scripts/Integrations/Supabase/`.
3. **RUN** `.\run_guardrails.ps1` to verify clean state.
