Write-Host "Running Sovereign State Guardrails..." -ForegroundColor Cyan

# Check for Python
try {
    python --version | Out-Null
} catch {
    Write-Host "ERROR: Python is not installed or not in PATH." -ForegroundColor Red
    exit 1
}

$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

# 0. Lint Guardrail Scripts (Ruff)
Write-Host "`n[0/5] Linting Guardrail Scripts (Ruff)..."
ruff check control/tools
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Python linting failed." -ForegroundColor Red
    exit 1
}

# 1. Validate Control YAML
$ValidateControlScript = Join-Path $PSScriptRoot "control\tools\validate_control.py"
Write-Host "`n[1/5] Validating Control File ($ValidateControlScript)..."
python $ValidateControlScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Control file validation failed." -ForegroundColor Red
    exit 1
}

# 2. Validate Repo Structure
$ValidateRepoScript = Join-Path $PSScriptRoot "control\tools\validate_repo_guardrails.py"
Write-Host "`n[2/5] Validating Repo Structure ($ValidateRepoScript)..."
python $ValidateRepoScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Repo structure validation failed." -ForegroundColor Red
    exit 1
}

# 3. Validate No Root C#
$ValidateNoRootScript = Join-Path $PSScriptRoot "control\tools\validate_no_root_cs.py"
Write-Host "`n[3/5] Validating No Root C# ($ValidateNoRootScript)..."
python $ValidateNoRootScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Root-level C# files found." -ForegroundColor Red
    exit 1
}

# 4. Validate Engine (No Unity)
$ValidateEngineNoUnityScript = Join-Path $PSScriptRoot "control\tools\validate_engine_no_unity.py"
Write-Host "`n[4/5] Validating Headless Engine ($ValidateEngineNoUnityScript)..."
python $ValidateEngineNoUnityScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Engine contains Unity references." -ForegroundColor Red
    exit 1
}

Write-Host "`nALL GUARDRAILS PASSED." -ForegroundColor Green
exit 0