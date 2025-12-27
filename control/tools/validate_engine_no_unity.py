import os
import sys

def check_engine_for_unity_refs():
    print("Checking engine for UnityEngine/UnityEditor references...")
    repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
    engine_src = os.path.join(repo_root, "engine", "src")
    
    forbidden = ["UnityEngine", "UnityEditor"]
    violations = []

    for root, dirs, files in os.walk(engine_src):
        for file in files:
            if file.endswith(".cs"):
                path = os.path.join(root, file)
                with open(path, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    for term in forbidden:
                        if term in content:
                            violations.append(f"VIOLATION: Found '{term}' in {os.path.relpath(path, repo_root)}")

    if violations:
        print("\n".join(violations))
        return False
        
    print("SUCCESS: Engine is clean of Unity references.")
    return True

if __name__ == "__main__":
    if not check_engine_for_unity_refs():
        sys.exit(1)
    sys.exit(0)

