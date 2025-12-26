Write-Host "Running Sovereign State Guardrails..." -ForegroundColor Cyan

# Check for Python
try {
    python --version | Out-Null
} catch {
    Write-Host "ERROR: Python is not installed or not in PATH." -ForegroundColor Red
    exit 1
}

$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

# 1. Validate Control YAML
$ValidateControlScript = Join-Path $PSScriptRoot "control\tools\validate_control.py"
Write-Host "`n[1/2] Validating Control File ($ValidateControlScript)..."
python $ValidateControlScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Control file validation failed." -ForegroundColor Red
    exit 1
}

# 2. Validate Repo Structure
$ValidateRepoScript = Join-Path $PSScriptRoot "control\tools\validate_repo_guardrails.py"
Write-Host "`n[2/2] Validating Repo Structure ($ValidateRepoScript)..."
python $ValidateRepoScript
if ($LASTEXITCODE -ne 0) {
    Write-Host "FAILED: Repo structure validation failed." -ForegroundColor Red
    exit 1
}

Write-Host "`nALL GUARDRAILS PASSED." -ForegroundColor Green
exit 0
