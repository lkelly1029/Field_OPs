# build_engine.ps1
# Automates the build of the Sovereign Engine and copies artifacts to Unity.

$ErrorActionPreference = "Stop"
$ScriptPath = $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path (Split-Path (Split-Path $ScriptPath))

# Configuration
$EngineSln = Join-Path $RepoRoot "engine\Sovereign.Engine.sln"
$UnityPluginsDir = Join-Path $RepoRoot "Assets\Plugins\SovereignEngine"
$BuildConfiguration = "Debug" # Use Debug for development/debugging support

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  SOVEREIGN ENGINE BUILDER" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Solution: $EngineSln"
Write-Host "Target:   $UnityPluginsDir"
Write-Host "Config:   $BuildConfiguration"
Write-Host "------------------------------------------"

# 1. Build the Engine Solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build $EngineSln -c $BuildConfiguration

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed! Fix compiler errors before proceeding."
}

# 2. Ensure Target Directory Exists
if (-not (Test-Path $UnityPluginsDir)) {
    Write-Host "Creating plugins directory..." -ForegroundColor Gray
    New-Item -ItemType Directory -Force -Path $UnityPluginsDir | Out-Null
}

# 3. Copy Artifacts (DLLs and PDBs)
# We explicitly list the projects we want to deploy (excluding Tests)
$ProjectsToDeploy = @("Sovereign.Core", "Sovereign.Economy", "Sovereign.Sim", "Sovereign.Mods")

foreach ($Project in $ProjectsToDeploy) {
    $SourceDir = Join-Path $RepoRoot "engine\src\$Project\bin\$BuildConfiguration\netstandard2.1"
    
    # Check if build output exists
    if (-not (Test-Path $SourceDir)) {
        Write-Warning "Output directory not found for $Project at $SourceDir"
        continue
    }

    Write-Host "Copying artifacts for $Project..." -ForegroundColor Green
    
    # Copy DLL
    $DllPath = Join-Path $SourceDir "$Project.dll"
    if (Test-Path $DllPath) {
        Copy-Item -Path $DllPath -Destination $UnityPluginsDir -Force
        Write-Host "  [+] $Project.dll" -ForegroundColor Gray
    } else {
        Write-Warning "  [-] DLL not found: $DllPath"
    }

    # Copy PDB (for debugging)
    $PdbPath = Join-Path $SourceDir "$Project.pdb"
    if (Test-Path $PdbPath) {
        Copy-Item -Path $PdbPath -Destination $UnityPluginsDir -Force
        Write-Host "  [+] $Project.pdb" -ForegroundColor Gray
    }
}

Write-Host "------------------------------------------"
Write-Host "BUILD & DEPLOY COMPLETE." -ForegroundColor Cyan
Write-Host "Return to Unity and wait for compilation to finish."
