# FairWorkly Backend

The core backend service for FairWorkly, built on .NET 8. The backend adopts **Vertical Slicing** architecture, splitting code by business feature into independent Features.

## Tech Stack

- **Framework**: .NET 8 (Web API)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 8
- **DI Container**: Microsoft.Extensions.DependencyInjection (built-in)
- **Validation**: FluentValidation
- **Task Orchestration**: MediatR (CQRS)
- **Code Formatting**: CSharpier (Enforced)

## Project Architecture (Vertical Slicing)

All modules use the **CQRS + MediatR** pattern, vertically slicing functionality into independent Features.

- **Modules**:
  - `src/FairWorkly.Application/Roster/` (Roster Agent)
  - `src/FairWorkly.Application/Payroll/` (Payroll Agent)
  - `src/FairWorkly.Application/FairBot/` (FairBot Chat)
- **Code Structure**: Each Feature contains independent `Command`, `Handler`, `Validator`, and `DTO`.
- **AI Integration**: External AI services are called via **Refit** typed interfaces, registered per target service in DI.

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

### 5. Application Configuration

Both `appsettings.json` and `appsettings.Development.json` are git-ignored. The repo provides `.example` templates — copy them and edit locally.

**Step 1: Create `appsettings.json`** (base config, all environments)

```bash
cp src/FairWorkly.API/appsettings.example.json src/FairWorkly.API/appsettings.json
```

What to change:
- `ConnectionStrings:DefaultConnection` — your Postgres host, port, credentials
- `JwtSettings:Secret` — must be non-empty; 32+ chars recommended

What you can leave as-is for local dev:
- `AiSettings:BaseUrl` — defaults to `localhost:8000` (Python agent-service)
- `AiSettings:TimeoutSeconds` — defaults to `120` (AI call timeout)
- `AWS:S3:Enabled` — defaults to `false` (uses local file storage)
- `Serilog` / `FileLogging` — logging config, defaults are fine

What you **must** set:
- `AiSettings:ServiceKey` — shared secret for .NET ↔ Python service-to-service auth. Must match the `AGENT_SERVICE_KEY` env var in `agent-service/.env`. Without this, the backend will fail to start.

**Step 2: Create `appsettings.Development.json`** (local dev overrides)

```bash
cp src/FairWorkly.API/appsettings.Development.example.json src/FairWorkly.API/appsettings.Development.json
```

This file overrides `appsettings.json` when running locally. The example has `_comment` fields explaining each setting. Typically the defaults work out of the box.

> **Note on Postgres port**: The examples use port `5433` (Docker Postgres exposed to host). If you're running Postgres natively, change to `5432`.

**Step 3: Apply Migrations**

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

### 2. Time Handling Standards (Time Provider)

To ensure the testability of business logic (such as "determining if it is within rostered hours"), **it is strictly forbidden to use `DateTime.Now` or `DateTimeOffset.Now` directly in the code**.

- **Correct Approach**: Inject `IDateTimeProvider` via the constructor.
- **Usage**: `_dateTimeProvider.Now`.

### 3. Current User Service

To access the authenticated user's identity in Handlers or Services, inject `ICurrentUserService`. It reads JWT claims from the current HTTP request automatically.

**Available properties:** `UserId`, `OrganizationId`, `Email`, `Role`, `EmployeeId` (all nullable).

```csharp
public class MyHandler(ICurrentUserService currentUser)
{
    public async Task<Result<MyDto>> Handle(MyQuery query, CancellationToken ct)
    {
        if (currentUser.OrganizationId is not { } orgId)
            return Result<MyDto>.Of403("User does not belong to an organization");
        // ...
    }
}
```

- **Interface**: `src/FairWorkly.Application/Common/Interfaces/ICurrentUserService.cs`
- **Implementation**: `src/FairWorkly.Infrastructure/Services/CurrentUserService.cs`

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

All MediatR Handlers return `Result<T>` using `Of{code}` factory methods. Controllers inherit `BaseApiController` and call `RespondResult(result)` — no manual status code mapping needed. All responses follow the `{ code, msg, data }` envelope format.

> 📖 **You must read the full guide before writing any Handler**: [Result<T> Pattern Guide](docs/result-pattern.md)

## Database Scripts

All scripts are located in `backend/scripts/`. Run them in terminal:

```bash
cd backend
pwsh ./scripts/add-migration.ps1
```

| Script                             | Purpose                                           | Data Loss      |
| ---------------------------------- | ------------------------------------------------- | -------------- |
| `add-migration.ps1`                | Create a new migration                            | None           |
| `update-database.ps1`              | Apply pending migrations (preserves data)         | None           |
| `reset-database.ps1`               | Drop and recreate database with all migrations    | Data           |
| `init-migrations-and-database.ps1` | Delete all migrations and regenerate from scratch | Data + History |

### Before Committing Migrations

**Always verify your migration can be applied successfully before committing.**

Test with `reset-database.ps1` or `update-database.ps1` depending on your situation.

## Advanced Development Guide

For details on how to develop new Features, use Refit interfaces to call the Python Agent, and write tests, please refer to:

- [Backend Development Guide](docs/backend-development-guide.md) - Comprehensive guide with code examples
- [Result<T> Pattern Guide](docs/result-pattern.md) - How to use Result pattern in Handlers

> These documents are also available on the team's Google Drive.
