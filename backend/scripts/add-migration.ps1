# Add migration script
# Prompts for migration name and creates a new EF Core migration

$ErrorActionPreference = "Stop"

# Get the backend directory (parent of scripts folder)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$BackendDir = Split-Path -Parent $ScriptDir

Write-Host "Working directory: $BackendDir" -ForegroundColor Cyan
Push-Location $BackendDir

try {
    # Prompt for migration name
    $MigrationName = Read-Host "Enter migration name"

    # Validate input
    if ([string]::IsNullOrWhiteSpace($MigrationName)) {
        Write-Host "Error: Migration name cannot be empty" -ForegroundColor Red
        exit 1
    }

    Write-Host "Creating migration '$MigrationName'..." -ForegroundColor Yellow
    dotnet ef migrations add $MigrationName --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    Write-Host "Migration created successfully!" -ForegroundColor Green
}
finally {
    Pop-Location
}
