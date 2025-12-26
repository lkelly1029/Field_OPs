import os
import sys

def check_structure():
    print("Checking repository structure guardrails...")
    repo_root = os.getcwd()
    assets_dir = os.path.join(repo_root, "Assets")
    
    violations = []
    
    # Rule 1: No 'control' folder inside 'Assets'
    if os.path.exists(os.path.join(assets_dir, "control")):
        violations.append("CRITICAL: Found 'control' directory inside 'Assets'. Move it to repo root.")

    # Rule 2: No 'engine' folder inside 'Assets'
    if os.path.exists(os.path.join(assets_dir, "engine")):
        violations.append("CRITICAL: Found 'engine' directory inside 'Assets'. Move it to repo root.")

    # Rule 3: Check for misplaced documentation
    restricted_docs = ["SovereignState_Build_Reference.md", "sovereign_state_control.yaml"]
    for root, dirs, files in os.walk(assets_dir):
        for file in files:
            if file in restricted_docs:
                violations.append(f"CRITICAL: Found authoritative doc '{file}' in Assets at: {root}")

    if violations:
        print("\n".join(violations))
        return False
        
    print("SUCCESS: Repository structure looks correct.")
    return True

if __name__ == "__main__":
    if not check_structure():
        sys.exit(1)
    sys.exit(0)

