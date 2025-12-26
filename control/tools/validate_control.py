import yaml
import sys
import os

def validate_control_file(file_path):
    print(f"Validating {file_path}...")
    if not os.path.exists(file_path):
        print(f"ERROR: File not found: {file_path}")
        return False
    
    try:
        with open(file_path, 'r') as f:
            data = yaml.safe_load(f)
            
        required_keys = ['project', 'north_star', 'non_negotiables', 'verticals', 'milestones']
        missing_keys = [key for key in required_keys if key not in data]
        
        if missing_keys:
            print(f"ERROR: Missing required keys: {missing_keys}")
            return False
            
        print("SUCCESS: Control file has valid structure.")
        return True
        
    except Exception as e:
        print(f"ERROR: Failed to parse YAML: {e}")
        return False

if __name__ == "__main__":
    # Path relative to repo root (assuming script runs from root or via helper)
    control_path = os.path.join("control", "sovereign_state_control.yaml")
    
    # Check if we are running from tools dir
    if not os.path.exists(control_path):
        control_path = os.path.join("..", "sovereign_state_control.yaml")
        
    if not validate_control_file(control_path):
        sys.exit(1)
    sys.exit(0)
