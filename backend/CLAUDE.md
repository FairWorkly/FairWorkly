# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Quick Start

**Before starting any work, read `.raw_materials/AI_README_FIRST.md`** - it defines your permissions, constraints, and workflow.

## Common Commands

```bash
# Run the API
dotnet run --project src/FairWorkly.API

# Run all tests
dotnet test

# Run a single test file
dotnet test --filter "FullyQualifiedName~CsvParserServiceTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~CsvParserServiceTests.ParseAsync_ValidCsv_ReturnsRows"

# Database operations
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# Add a migration
dotnet ef migrations add MigrationName --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

## Architecture

This is a .NET 8 Clean Architecture project with CQRS pattern (MediatR).

```
src/
├── FairWorkly.API/           # Controllers (thin, only forwards to MediatR)
├── FairWorkly.Application/   # Use cases, services, handlers, DTOs
├── FairWorkly.Domain/        # Entities, enums (DO NOT MODIFY)
└── FairWorkly.Infrastructure/ # DbContext, repositories, external services
```

**Key patterns:**
- CQRS with MediatR: Commands/Queries → Validators → Handlers
- Vertical Slicing: Each feature in `Features/{FeatureName}/` contains Command, Validator, Handler, DTO
- Repository interfaces in Application layer, implementations in Infrastructure layer
- FluentValidation for input validation (not in handlers)

## Files You Cannot Modify

| Path | Reason |
|------|--------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entities are finalized |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | Audit logic configured |
| `.raw_materials/BUSINESS_RULES/*` | Legal-level constraints |

## Code Standards

- **Money**: Use `decimal`, never `float`/`double`
- **Timestamps**: Use `DateTimeOffset`, never `DateTime`
- **Date only**: Use `DateOnly`
- **Current time**: Inject `IDateTimeProvider`, never use `DateTime.Now`
- **DI registration**: In layer's `DependencyInjection.cs`, not in `Program.cs`
- **Comments/naming**: English only
- **Tolerance values**: $0.01 for rates, $0.05 for amounts

## Documentation System

This project uses a layered documentation approach:

- **`.doc/`**: Development documentation (specs, issues, dev log)
- **`AI_GUIDE.md` files**: Navigation guides in code directories
- **`.raw_materials/`**: Original requirements (read-only reference)

When starting a new session, read `.doc/AI_GUIDE.md` for current project state and task status.

## Workflow

1. Read the current ISSUE in `.doc/issues/`
2. Implement the feature
3. Write tests
4. Update relevant `AI_GUIDE.md` files
5. Update `.doc/DEVLOG.md` with decisions made
