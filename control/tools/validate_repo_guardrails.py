import os
import sys
import glob

def check_structure():
    print("Checking repository structure guardrails...")
    repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
    
    violations = []
    
    required_files = [
        "GEMINI_AGENT_BRIEF.md",
        "README.md",
        "control/sovereign_state_control.yaml",
        "control/SovereignState_Build_Reference.md",
        "control/SovereignState_Unity_Placement_Reference.md",
        "control/SovereignState_Dev_Onboarding_Checklist.md",
        "control/SovereignState_First_Vertical_Replication_Guide.md",
        "control/tools/validate_control.py",
        "control/tools/validate_repo_guardrails.py",
        "run_guardrails.ps1",
        "Assets/Scripts/SimBridge/ISimDebugProvider.cs",
        "Assets/Scripts/DevTools/SovereignDevConsoleOverlay.cs",
    ]

    for rel_path in required_files:
        path = os.path.join(repo_root, rel_path.replace('/', os.sep))
        if not os.path.exists(path):
            violations.append(f"MISSING: Required file not found: {rel_path}")

    # Check for engine scenario with glob because the exact path might vary slightly
    power_acceptance_path = os.path.join(repo_root, "engine", "src", "**", "PowerAcceptance.cs")
    if not glob.glob(power_acceptance_path, recursive=True):
         violations.append(f"MISSING: Engine scenario 'PowerAcceptance.cs' not found in engine/src/.")

    # Rule: No 'control' folder inside 'Assets'
    if os.path.exists(os.path.join(repo_root, "Assets", "control")):
        violations.append("CRITICAL: Found 'control' directory inside 'Assets'. Move it to repo root.")

    # Rule: No 'engine' folder inside 'Assets'
    if os.path.exists(os.path.join(repo_root, "Assets", "engine")):
        violations.append("CRITICAL: Found 'engine' directory inside 'Assets'. Move it to repo root.")

    if violations:
        print("\n".join(violations))
        return False
        
    print("SUCCESS: Repository structure guardrails passed.")
    return True

if __name__ == "__main__":
    if not check_structure():
        sys.exit(1)
    sys.exit(0)
