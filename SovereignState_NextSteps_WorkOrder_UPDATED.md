# Sovereign State — Next Steps Work Order (Repo Hygiene + Next Build Milestone)

**Repo root:** `C:\Users\trulu\Field_OPs`  
**Purpose:** Single work-order for your code generator/agent to (1) correct file placement issues, (2) keep guardrails green, and (3) advance the build to the next milestone without Unity/engine boundary drift.

---

## 0) Non‑Negotiable Placement Contract

### 0.1 Where code is allowed to live
- **Design / build contracts:** `control/**`
- **Deterministic simulation engine (pure C#):** `engine/src/**`
- **Unity presentation scripts:** `Assets/Scripts/**` *(Unity-facing glue/presentation only)*
- **Unity scenes/assets:** `Assets/**` *(scenes, prefabs, art, etc.)*
- **Repo-root:** **NO C# FILES AT ROOT**.

### 0.2 Repo-root “allowed file types”
At repo root, new files may be created only if they are:
- `README.md`, `STATUS.md`, `GEMINI_AGENT_BRIEF.md`, `GEMINI_VSCODE_HOW_TO.md`
- `run_guardrails.ps1`, `ops_inventory_*.txt`, other explicitly approved docs/scripts

### 0.3 Hard guardrail
`python control\tools\validate_no_root_cs.py` must pass at all times.

---

## 1) Current Findings

### 1.1 Root-level C# files exist (must be fixed)
You observed:
- `TickIndex.cs` exists at repo root and triggers `validate_no_root_cs.py`
- `Universe.cs` is also present at repo root

These are **engine types** and must live under `engine/src/**`.

Canonical engine locations exist in the intended structure:
- `engine/src/Sovereign.Core/TickIndex.cs`
- `engine/src/Sovereign.Sim/Universe.cs`

So the repo-root copies are accidental duplicates.

### 1.2 Two script folders in Unity Assets
You have both:
- `Assets/Scripts/**` (new/primary)
- `Assets/_Scripts/**` (older/legacy)

Unity will load both, but `_Scripts` is typically a legacy/quarantine prefix. Consolidate to one active home.

### 1.3 Why engine scripts aren’t visible in Unity UI
Everything under `engine/**` is **not** in Unity’s `Assets/` tree. Unity’s Project window only shows `Assets/**` (and `Packages/**`).  
That is expected. Engine code is used only if Unity references the engine assemblies (DLL) or via a package/asmdef strategy.

---

## 2) Phase A — Fix Repo Hygiene (Placement + Consolidation)

### A1) Remove / relocate ALL repo-root C# files
**Goal:** No `*.cs` files directly under `C:\Users\trulu\Field_OPs\`.

**Actions**
1. Enumerate root-level `*.cs`:
   - `Get-ChildItem -Path . -Filter *.cs -File`
2. For each root `*.cs`:
   - If an authoritative copy exists under `engine/src/**`, **delete the root copy**.
   - If it is genuinely new, **move it** into the correct engine project folder.

**Required deletes**
- `TickIndex.cs` (root) → **DELETE** (canonical exists at `engine/src/Sovereign.Core/TickIndex.cs`)
- `Universe.cs` (root) → **DELETE** (canonical exists at `engine/src/Sovereign.Sim/Universe.cs`)

**Verification**
- Run: `python control\tools\validate_no_root_cs.py` (must be SUCCESS)

---

### A2) Consolidate Unity script folders (`Assets/_Scripts` → `Assets/Scripts`)
**Goal:** `Assets/_Scripts` should not contain active production code.

**Policy**
- If `Assets/_Scripts` contains Supabase + Secrets that must remain: move them to a clearly named folder under `Assets/Scripts`.

**Proposed target**
- `Assets/Scripts/Integrations/Supabase/`

**Move map**
- `Assets/_Scripts/SupabaseManager.cs` → `Assets/Scripts/Integrations/Supabase/SupabaseManager.cs`
- `Assets/_Scripts/Secrets.cs` → `Assets/Scripts/Integrations/Supabase/Secrets.cs`
- `Assets/_Scripts/ConstructionMaterial.cs` → `Assets/Scripts/Integrations/Supabase/ConstructionMaterial.cs` *(or rename if not Supabase-specific)*

**After moves**
- If `Assets/_Scripts` is empty: delete folder and its `.meta` (Unity will regenerate metas as needed).
- If you prefer keeping it as quarantine: keep folder but ensure no active runtime code resides there.

**Verification**
- Open Unity and confirm no compile errors.
- Run guardrails (Phase C below).

---

### A3) Normalize legacy folders
You already have `Assets/_Legacy/**`. This is correct.

**Rule:** Anything not part of the Sovereign State MVP (or required integrations) must live under:
- `Assets/_Legacy/**` (Unity-side experiments)
- `engine/_legacy/**` (engine-side experiments, if needed)

---

## 3) Phase B — Make Engine Code Usable by Unity (Explicit Integration)

Right now, engine code is correctly separated under `engine/**`. Next: make Unity able to run ticks without copying engine logic into `Assets/`.

Choose one. **Pattern 1 is recommended.**

### Pattern 1 (Recommended): Build engine to DLL(s) and reference from Unity
**Approach**
1. Build engine projects:
   - `engine/src/Sovereign.Core`
   - `engine/src/Sovereign.Economy`
   - `engine/src/Sovereign.Sim`
2. Copy DLLs into:
   - `Assets/Plugins/SovereignEngine/` (create if missing)

**Steps**
- Add `engine/tools/build_engine.ps1` that:
  - runs `dotnet build` on the engine solution
  - copies output DLLs into `Assets/Plugins/SovereignEngine/`
- In Unity, confirm assemblies load and `SimulationRunner` can instantiate engine types.

**Pros:** strict separation, faster Unity compiles  
**Cons:** requires a repeatable build step (solved by script)

### Pattern 2: Unity consumes engine as a UPM package (advanced)
Convert `engine/src` to a Unity Package and reference it via `Packages/manifest.json`.  
This works, but adds complexity—prefer Pattern 1 until MVP stabilizes.

---

## 4) Phase C — Guardrails: Required Checks After Changes

From repo root:

1. `./run_guardrails.ps1`
2. If it fails, fix immediately.

**Additional sanity checks**
- No `control/` or `engine/` duplicates nested under `Assets/SovereignState/...`
- `Assets/_Legacy` contains legacy experiments and does not leak into runtime

---

## 5) Phase D — Next Build Milestone (MVP “Power” Vertical)

### D1) Confirm “Power vertical” is deterministic end-to-end
**Objective**
- Houses generate demand
- Supply resolves Local → Exchange → AI (fallback)
- Deliveries apply loss + transport fees
- Ledger settles money flows
- Plot stability updates based on served/unserved demand

**Deliverables**
- Engine unit tests for ledger invariants
- One headless scenario test for `PowerAcceptance`
- Unity dev console displays:
  - current tick
  - treasury
  - produced/consumed/imported/exported power
  - AI share vs player share

### D2) Make Unity a pure viewer/controller
**Objective**
- Unity must not calculate economics.
- Unity only sends commands + displays snapshots.

**Deliverables**
- Snapshot DTO boundary (engine → unity) that is stable and versioned
- Minimal command interface (unity → engine)

### D3) Prepare “Water” replication
**Deliverables**
- Template folder + checklist for “new resource vertical”
- One “Water” mod JSON file and its schema validation test

---

## 6) Agent Instructions (What the generator should do next)

### 6.1 Immediate “Do Now”
1. Delete/move root-level `*.cs` (TickIndex.cs, Universe.cs, and any others found).
2. Consolidate `Assets/_Scripts` into `Assets/Scripts/Integrations/...` while preserving Supabase + Secrets.
3. Re-run guardrails until all pass.

### 6.2 “Do Next” (after hygiene passes)
1. Implement Pattern 1: engine DLL build + copy into `Assets/Plugins/SovereignEngine/`.
2. Confirm Unity can run ticks and render snapshot state without engine source in `Assets/`.
3. Expand Dev Console overlay to include “Power vertical” KPIs.

---

## 7) Commands Cheat Sheet (PowerShell)

```powershell
cd "C:\Users\trulu\Field_OPs"

# Find illegal root-level C# files
Get-ChildItem -Path . -Filter *.cs -File

# Run all guardrails
.\run_guardrails.ps1

# Validate only the "no root cs" rule
python control\tools\validate_no_root_cs.py

# After consolidation: check Supabase scripts location
Get-ChildItem -Recurse -Path .\Assets\Scripts\Integrations | Select-Object FullName
```

---

## 8) Definition of Done

Complete when:
- `validate_no_root_cs.py` passes (no root-level `.cs`)
- `run_guardrails.ps1` passes
- `Assets/_Scripts` is removed or empty (or only quarantine, no active runtime)
- Unity opens without compile errors
- Engine integration pattern is selected and documented in `STATUS.md`
