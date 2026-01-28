# Reset database and migrations script
# Drops the existing database, removes all migrations, recreates InitialCreate

$ErrorActionPreference = "Stop"

# Get the backend directory (parent of scripts folder)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$BackendDir = Split-Path -Parent $ScriptDir

Write-Host "Working directory: $BackendDir" -ForegroundColor Cyan
Push-Location $BackendDir

try {
    # Step 1: Drop database
    Write-Host "Dropping database..." -ForegroundColor Yellow
    dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    # Step 2: Remove all migrations
    Write-Host "Removing all migrations..." -ForegroundColor Yellow
    $MigrationsDir = "src/FairWorkly.Infrastructure/Migrations"
    if (Test-Path $MigrationsDir) {
        Remove-Item -Path $MigrationsDir -Recurse -Force
        Write-Host "Migrations folder deleted" -ForegroundColor Gray
    }

    # Step 3: Create fresh InitialCreate migration
    Write-Host "Creating InitialCreate migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    # Step 4: Apply migration to create new database
    Write-Host "Applying migration..." -ForegroundColor Yellow
    dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

    Write-Host "Database and migrations reset complete!" -ForegroundColor Green
}
finally {
    Pop-Location
}