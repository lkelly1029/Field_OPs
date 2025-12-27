$legacyPaths = @(
    "engine/src/world",
    "engine/src/simulation",
    "engine/src/contracts",
    "engine/src/power",
    "engine/src/consumers"
)

$root = Resolve-Path "$PSScriptRoot/../.."

foreach ($path in $legacyPaths) {
    $fullPath = Join-Path $root $path
    if (Test-Path $fullPath) {
        Write-Host "Removing legacy path: $fullPath"
        Remove-Item -Recurse -Force $fullPath
    } else {
        Write-Host "Path not found (clean): $fullPath"
    }
}