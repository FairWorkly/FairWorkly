# Reset database script
# Drops the existing database and recreates it with all migrations

$ErrorActionPreference = "Stop"

# Get the backend directory (parent of scripts folder)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$BackendDir = Split-Path -Parent $ScriptDir

Write-Host "Working directory: $BackendDir" -ForegroundColor Cyan
Push-Location $BackendDir

try {
    Write-Host "Dropping database..." -ForegroundColor Yellow
    dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    Write-Host "Applying migrations..." -ForegroundColor Yellow
    dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    Write-Host "Database reset complete!" -ForegroundColor Green
}
finally {
    Pop-Location
}