# FairWorkly Backend

The core backend service for FairWorkly, built on .NET 8. The backend adopts a **Hybrid Architecture** to balance the needs of complex business logic with simple data management.

## Tech Stack

- **Framework**: .NET 8 (Web API)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 8
- **DI Container**: Autofac
- **Validation**: FluentValidation
- **Task Orchestration**: MediatR (CQRS)
- **Code Formatting**: CSharpier (Enforced)

## Project Architecture (Hybrid Architecture)

### 1. Core Business: Vertical Slicing

For Agent modules with complex logic involving AI Orchestration, we use the **CQRS + MediatR** pattern, vertically slicing functionality into independent Features.

- **Applicable Modules**:
  - `src/FairWorkly.Application/Compliance/` (Compliance Agent)
  - `src/FairWorkly.Application/Payroll/` (Payroll Agent)
- **Code Structure**: Each Feature contains independent `Command`, `Handler`, `Validator`, and `DTO`.

### 2. Basic Operations: Traditional N-Layer

For management modules that are primarily CRUD-based with relatively linear logic, we retain the traditional **Service-Repository** pattern.

- **Applicable Modules**:
  - `src/FairWorkly.Application/Documents/` (Document Management)
  - `src/FairWorkly.Application/Employees/` (Employee Profiles)
- **Code Structure**: `Controller` -> `Service` (Business Logic) -> `Repository` (Data Access).

## Prerequisites

Before you begin, please ensure you have installed:

1. .NET 8 SDK
2. PostgreSQL (or run Postgres via Docker)
3. PowerShell (`pwsh`) if you want to run the repo's `*.ps1` scripts on macOS/Linux
4. IDE: Visual Studio 2022 (Recommended) or Visual Studio Code

## Getting Started

### 1. Set Working Directory

Ensure your terminal is inside the `backend` folder:

```bash
cd backend
```

### 2. Initialize Toolchain

```bash
dotnet tool restore
```

### 3. Configure Git Blame (Optional)

```bash
git config blame.ignoreRevsFile .git-blame-ignore-revs
```

### 4. Configure IDE & Formatting

Follow the instructions below for your chosen IDE.

#### Visual Studio 2022 Settings

1. **Install Extension**: Search for and install `CSharpier` in Extensions.
2. **Configure Save Actions**:
   - `Tools` -> `Options` -> `CSharpier`.
   - Tick **Reformat with CSharpier on Save**.
   - Ensure **Solution** is `True`, and **Global** is `False`.
3. **Disable Native Interference**:
   - `Analyze` -> `Code Cleanup` -> `Configure Code Cleanup`.
   - Check Profile 1, **remove** `Format Document`.
4. **Remap Shortcuts (Optional)**:
   - `Tools` -> `Options` -> `Environment` -> `Keyboard`.
   - Search for `Edit.FormatDocument` -> **Remove**.
   - Search for `ReformatWithCSharpier` -> Assign preferred key -> **Assign**.

#### JetBrains Rider Settings

1.  **Install Plugin**:
    - Go to **Settings** (`Ctrl+Alt+S`) -> **Plugins**.
    - Search for and install **CSharpier**.
2.  **Configure Plugin**:
    - Go to **Tools** -> **CSharpier**.
    - Check **Run on Save**.
    - Leave "Override CSharpier Executable" **unchecked**.
3.  **Disable Native Formatting (Crucial)**:
    - Go to **Tools** -> **Actions on Save**.
    - Ensure **Reformat code** is **unchecked**.
4.  **Configure Shortcut (Optional)**:
    - Go to **Settings** -> **Keymap**.
    - Search for **"Reformat with CSharpier"**.
    - Right-click it -> **Add Keyboard Shortcut**.
    - Assign your preferred combination.

#### Visual Studio Code Settings

**Install extensions**:

1.  **C# Dev Kit** (id: `ms-dotnettools.csdevkit`)
2.  **CSharpier - Code Formatter** (id: `csharpier.csharpier-vscode`)

Once installed, simply **Save (Ctrl+S)** any `.cs` file, and it will be formatted automatically. No further configuration is needed.

### 5. Database Configuration

**Method A: Using User Secrets**

In VS2022, right-click `FairWorkly.API` project -> **Manage User Secrets**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=FairWorklyDb;Username=postgres;Password=postgres"
  }
}
```

Notes:
- Port `5433` matches the local-dev default in `src/FairWorkly.API/appsettings.Development.example.json` (Docker Postgres exposed to host).
- If you're running Postgres natively, change the port to `5432` and update credentials accordingly.

**Method B: Create a local appsettings file (recommended)**

Create `appsettings.Development.json` from the tracked example, then edit values locally (this file is intentionally ignored by git):

```bash
cp src/FairWorkly.API/appsettings.Development.example.json src/FairWorkly.API/appsettings.Development.json
```

Then update:
- `ConnectionStrings:DefaultConnection`
- `JwtSettings:Secret` (must be non-empty; 32+ chars recommended)
- `AiSettings` if you want to point at a real `agent-service`

**Apply Migrations**:
```bash
cd backend
pwsh ./scripts/update-database.ps1
```

### 6. Start the Project

```bash
dotnet run --project src/FairWorkly.API --launch-profile https
```

Swagger:

- Local `dotnet run` (https launch profile): `https://localhost:7075/swagger`
- Local `dotnet run` (http): `http://localhost:5680/swagger`
- Docker Compose backend: `http://localhost:5680/swagger`

## Development Patterns & Tools

### 1. Dependency Injection Standards

This project follows Clean Architecture, and dependency injection registration logic is split by layer. Whenever you add a new Service or Repository, **you must register the service in the corresponding layer**:

- **Application Layer Services**:
  - **Location**: `src/FairWorkly.Application/DependencyInjection.cs`
- **Infrastructure Layer Services**:
  - **Location**: `src/FairWorkly.Infrastructure/DependencyInjection.cs`

> **Note**: `Program.cs` in the API layer is only responsible for calling these two extension methods. **It is strictly forbidden to register business services directly in `Program.cs`**.

### 2. AI Service Configuration (Mock vs Real)

The backend depends on a Python AI Service (`agent-service`). During the development phase, **Mock Mode** is enabled by default (does not depend on the real Python service) for easier debugging.

Configuration location: `src/FairWorkly.API/appsettings.Development.json` (or user secrets), with defaults in `src/FairWorkly.API/appsettings.Development.example.json`.

```json
"AiSettings": {
  "BaseUrl": "http://localhost:8000",
  "UseMockAi": true  // true = Use local mock data; false = Call Python interface
}
```

### 3. Time Handling Standards (Time Provider)

To ensure the testability of business logic (such as "determining if it is within rostered hours"), **it is strictly forbidden to use `DateTime.Now` or `DateTimeOffset.Now` directly in the code**.

- **Correct Approach**: Inject `IDateTimeProvider` via the constructor.
- **Usage**: `_dateTimeProvider.Now`.

### 4. File Storage Strategy

This project uses the Adapter Pattern to handle file storage, with the core interface being `IFileStorageService`.

- **Development Environment**:
  - Injects `LocalFileStorageService` by default.
  - Physical Path: `src/FairWorkly.API/wwwroot/uploads/`.
  - **Note**: This directory is ignored in `.gitignore`.

### 5. Entity Configuration Standards

Entity configuration (table mapping, relationships, constraints) must be placed in dedicated Configuration classes, **not in DbContext**.

- **Location**: `src/FairWorkly.Infrastructure/Persistence/Configurations/{Module}/`
- **Pattern**: One Configuration class per Entity, implementing `IEntityTypeConfiguration<T>`

> **Forbidden**: Writing `modelBuilder.Entity<T>()` directly in `FairWorklyDbContext.OnModelCreating()`.

### 6. Result<T> Pattern

MediatR Handlers return `Result<T>` instead of throwing exceptions for better control flow and explicit error handling.

**Available factory methods:**

| Method | When to use | HTTP Response |
|--------|-------------|---------------|
| `Success(value)` | Operation succeeded | 200 OK |
| `ValidationFailure(errors)` | Input validation failed | 400 Bad Request |
| `NotFound(message)` | Resource not found | 404 Not Found |
| `Forbidden(message)` | No permission | 403 Forbidden |

**Example:**

```csharp
// Handler
if (entity == null)
    return Result<MyDto>.NotFound("Resource not found");
return Result<MyDto>.Success(dto);

// Controller
if (result.Type == ResultType.NotFound)
    return NotFound(new { message = result.ErrorMessage });
return Ok(result.Value);
```

> 📖 For detailed usage, see [Result<T> Pattern Guide](docs/Result_Pattern_Guide.md)

Location: `src/FairWorkly.Domain/Common/Result.cs`

## Database Scripts

All scripts are located in `backend/scripts/`. Run them in terminal:
```bash
cd backend
pwsh ./scripts/add-migration.ps1
```

| Script                             | Purpose                                           | Data Loss        |
| ---------------------------------- | ------------------------------------------------- | ---------------- |
| `add-migration.ps1`                | Create a new migration                            | None             |
| `update-database.ps1`              | Apply pending migrations (preserves data)         | None             |
| `reset-database.ps1`               | Drop and recreate database with all migrations    | Data             |
| `init-migrations-and-database.ps1` | Delete all migrations and regenerate from scratch | Data + History   |

### Before Committing Migrations

**Always verify your migration can be applied successfully before committing.**

Test with `reset-database.ps1` or `update-database.ps1` depending on your situation.

## Advanced Development Guide

For details on how to develop new Features, write AI Orchestrators, and interface with the Python Agent, please refer to:

- [Backend Development Guide](docs/Backend_Development_Guide.md) - Comprehensive guide with code examples
- [Result<T> Pattern Guide](docs/Result_Pattern_Guide.md) - How to use Result pattern in Handlers

> These documents are also available on the team's Google Drive.
