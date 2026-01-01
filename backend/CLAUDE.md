# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## ⚠️ Constitutional Documents (Read First!)

**`.raw_materials/` is the project's "constitution" - AI can only READ, never WRITE.**

| Path | Permission | Description |
|------|------------|-------------|
| `.raw_materials/BUSINESS_RULES/` | 🔴 Read-Only | Rate tables, API contracts. ANY modification is a violation. |
| `.raw_materials/TECH_CONSTRAINTS/` | 🟡 Read-Only (can raise objections) | Technical constraints. Can question but cannot modify. |
| `.raw_materials/REFERENCE/` | 🟢 Read-Only Reference | Reference materials. Can redesign in `.doc/`. |
| `**/README.md` | 🔴 Read-Only | Human documentation. AI cannot modify/delete/create. |

**If you find issues in constitutional documents:**
1. DO NOT modify them
2. Report to human using the objection format
3. Wait for human confirmation

## Quick Start

**Before starting any work, read the appropriate entry document:**

1. **ALWAYS first**: Read `.raw_materials/AI_README_FIRST.md` (constitutional document, understand your boundaries)
2. If `.doc/` directory exists → read `.doc/AI_GUIDE.md` (project navigation and current task status)
3. If `.doc/` directory does NOT exist → follow instructions in `.raw_materials/AI_README_FIRST.md` to create `.doc/`

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

### Code Files (Red Line)

| Path | Reason |
|------|--------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entities are finalized |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | Audit logic configured |

### Documentation Files (Constitutional)

| Path | Reason |
|------|--------|
| `.raw_materials/*` | Constitutional documents - AI has READ-ONLY access |
| `.raw_materials/BUSINESS_RULES/*` | Legal-level constraints - absolute red line |
| `**/README.md` | Human documentation - written by humans for humans |

**Objection Format** (when you find issues in read-only docs):
```markdown
> **[Objection]**
> - Document says: XXX
> - Actual situation: YYY
> - My judgment: ZZZ
> - Suggestion: Wait for human confirmation
```

## Code Standards

- **Money**: Use `decimal`, never `float`/`double`
- **Timestamps**: Use `DateTimeOffset`, never `DateTime`
- **Date only**: Use `DateOnly`
- **Current time**: Inject `IDateTimeProvider`, never use `DateTime.Now`
- **DI registration**: In layer's `DependencyInjection.cs`, not in `Program.cs`
- **Comments/naming**: English only
- **Tolerance values**: $0.01 for rates, $0.05 for amounts

## Documentation System

This project uses a **hierarchical documentation approach** with strict permission levels:

### Constitutional Layer (READ-ONLY for AI)

| Path | Description |
|------|-------------|
| `.raw_materials/AI_README_FIRST.md` | Entry point - read this FIRST in every session |
| `.raw_materials/BUSINESS_RULES/` | Rate tables, API contracts (legal-level constraints) |
| `.raw_materials/TECH_CONSTRAINTS/` | Coding standards, architecture rules |
| `.raw_materials/REFERENCE/` | Reference materials (can be redesigned in `.doc/`) |
| `**/README.md` | Human documentation (never modify) |

### Working Layer (READ-WRITE for AI)

| Path | Description |
|------|-------------|
| `.doc/AI_GUIDE.md` | Project navigation - read at session start |
| `.doc/CODING_RULES.md` | AI's interpretation of coding rules |
| `.doc/SPEC_*.md` | Technical specifications |
| `.doc/DEVLOG.md` | Development log (decisions, discussions) |
| `.doc/issues/` | Current development tasks |
| `**/AI_GUIDE.md` (in code dirs) | Navigation guides for code directories |

**Session Start Workflow:**
1. Read `.raw_materials/AI_README_FIRST.md` (understand boundaries)
2. Read `.doc/AI_GUIDE.md` (current project state)
3. Check `.doc/issues/` for current task

## Workflow

1. Read the current ISSUE in `.doc/issues/`
2. Implement the feature
3. Write tests
4. Update relevant `AI_GUIDE.md` files
5. Update `.doc/DEVLOG.md` with decisions made
