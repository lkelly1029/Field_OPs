import os
import sys

ROOT = os.getcwd()

for f in os.listdir(ROOT):
    if f.endswith(".cs"):
        print(f"ERROR: C# file at repo root: {f}")
        sys.exit(1)

print("SUCCESS: No root-level C# files found.")
