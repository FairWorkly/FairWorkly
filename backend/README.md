# FairWorkly Backend

The core backend service for FairWorkly, built on .NET 8. The backend adopts a **Hybrid Architecture** to balance the needs of complex business logic with simple data management.

## 🛠 Tech Stack

- **Framework**: .NET 8 (Web API)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 8
- **DI Container**: Autofac
- **Validation**: FluentValidation
- **Task Orchestration**: MediatR (CQRS)
- **Code Formatting**: CSharpier (Enforced)

## 🏗 Project Architecture (Hybrid Architecture)

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

## 🚀 Prerequisites

Before you begin, please ensure you have installed:

1. .NET 8 SDK
2. PostgreSQL (Ensure the local service is running)
3. IDE: Visual Studio 2022 (Recommended) or Visual Studio Code

## ⚡ Getting Started

### 1. Set Working Directory

Ensure your terminal is inside the `backend` folder:

```bash
cd backend
```

### 2. Initialize Toolchain

```bash
dotnet tool restore
```

### 3. Configure Git Blame

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
    "DefaultConnection": "Host=localhost;Port=5432;Database=FairWorklyDb;Username=postgres;Password=<YourPassword>"
  }
}
```

**Method B: Modify Configuration File**

Edit `src/FairWorkly.API/appsettings.json` -> `DefaultConnection`.

**Apply Migrations**:

```bash
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

### 6. Start the Project

```bash
dotnet run --project src/FairWorkly.API --launch-profile https
```

Access Swagger: `https://localhost:7075/swagger`

## 🔧 Development Patterns & Tools

### 1. Dependency Injection Standards

This project follows Clean Architecture, and dependency injection registration logic is split by layer. Whenever you add a new Service or Repository, **you must register the service in the corresponding layer**:

- **Application Layer Services**:
  - **Location**: `src/FairWorkly.Application/DependencyInjection.cs`
- **Infrastructure Layer Services**:
  - **Location**: `src/FairWorkly.Infrastructure/DependencyInjection.cs`

> **⚠️ Note**: `Program.cs` in the API layer is only responsible for calling these two extension methods. **It is strictly forbidden to register business services directly in `Program.cs`**.

### 2. AI Service Configuration (Mock vs Real)

The backend depends on a Python AI Service (`agent-service`). During the development phase, **Mock Mode** is enabled by default (does not depend on the real Python service) for easier debugging.

Configuration location: `appsettings.json`

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

> **⚠️ Forbidden**: Writing `modelBuilder.Entity<T>()` directly in `FairWorklyDbContext.OnModelCreating()`.

## 📂 Cheatsheet

**Add Migration:**

```
dotnet ef migrations add <MigrationName> --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

**Update Database:**

```
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

## 📚 Advanced Development Guide

For details on how to develop new Features, write AI Orchestrators, and interface with the Python Agent, please be sure to read the detailed internal tutorial.