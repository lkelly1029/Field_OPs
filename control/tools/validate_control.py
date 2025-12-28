import yaml
import sys
import os
from rich.console import Console
from rich.panel import Panel

console = Console()

def validate_control_file(file_path):
    console.print(f"[bold blue]Validating control file:[/bold blue] {file_path}")
    if not os.path.exists(file_path):
        console.print(f"[bold red]ERROR:[/bold red] File not found: {file_path}")
        return False
    
    try:
        with open(file_path, 'r') as f:
            data = yaml.safe_load(f)
            
        required_keys = ['project', 'north_star', 'non_negotiables', 'verticals', 'milestones']
        missing_keys = [key for key in required_keys if key not in data]
        
        if missing_keys:
            console.print(f"[bold red]ERROR:[/bold red] Missing required keys: {missing_keys}")
            return False
            
        console.print(Panel("[bold green]SUCCESS:[/bold green] Control file has valid structure.", expand=False))
        return True
        
    except Exception as e:
        console.print(f"[bold red]ERROR:[/bold red] Failed to parse YAML: {e}")
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
