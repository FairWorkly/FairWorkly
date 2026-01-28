# Update database script
# Applies pending migrations to the database (preserves data)

$ErrorActionPreference = "Stop"

# Get the backend directory (parent of scripts folder)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$BackendDir = Split-Path -Parent $ScriptDir

Write-Host "Working directory: $BackendDir" -ForegroundColor Cyan
Push-Location $BackendDir

try {
    Write-Host "Applying pending migrations..." -ForegroundColor Yellow
    dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    Write-Host "Database update complete!" -ForegroundColor Green
}
finally {
    Pop-Location
}
