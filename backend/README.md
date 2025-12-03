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

### 1. Initialize Toolchain

This project uses local tools to manage dependencies (such as EF Core and CSharpier). Run the following command in the `backend` directory:

```
dotnet tool restore
```

### 2. Configure Git Blame (Mandatory ⚠️)

This project enforces code formatting. To prevent formatting commits from obscuring the true authors of the business code, **you must run the following command once in the project root directory**:

```
git config blame.ignoreRevsFile .git-blame-ignore-revs
```

### 3. Database Configuration

You need to configure the connection string to point to your local PostgreSQL instance.

Method A: Using User Secrets (Recommended, prevents password leakage)

In VS2022, right-click the FairWorkly.API project -> Select Manage User Secrets, and enter the following:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FairWorklyDb;Username=postgres;Password=<YourPassword>"
  }
}
```

Method B: Modify Configuration File

Directly modify the DefaultConnection field in src/FairWorkly.API/appsettings.Development.json.

After configuration is complete, apply the database migrations:

```
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

### 4. AI Service Configuration (Mock vs Real)

The backend depends on a Python AI Service (`agent-service`). During the development phase, **Mock Mode** is enabled by default (does not depend on the real Python service) for easier debugging.

Configuration location: `appsettings.Development.json`

```
"AiSettings": {
  "BaseUrl": "http://localhost:8000",
  "UseMockAi": true  // true = Use local mock data; false = Call Python interface
}
```

### 5. Start the Project

```
dotnet run --project src/FairWorkly.API
```

Access Swagger Documentation: `https://localhost:7075/swagger`

## 📏 Coding Standards

### 1. Code Formatting

This project enforces **CSharpier** for formatting.

#### Visual Studio 2022 Settings (Mandatory)

VS2022 users need to manually configure settings to avoid conflicts with native formatting:

1. **Install Extension**: Search for and install `CSharpier` in Extensions.
2. **Configure Save Actions**:
   - `Tools` -> `Options` -> `CSharpier`.
   - Tick **Reformat with CSharpier on Save**.
   - Ensure **Solution** is `True`, and **Global** is `False` (to prevent affecting other projects).
3. **Disable Native Interference**:
   - `Analyze` -> `Code Cleanup` -> `Configure Code Cleanup`.
   - Check Profile 1, **make sure to remove** `Format Document`.
4. **Remap Shortcuts**:
   - `Tools` -> `Options` -> `Environment` -> `Keyboard`.
   - Search for `Edit.FormatDocument` -> **Remove** (remove the original `Ctrl+K, Ctrl+D`).
   - Search for `ReformatWithCSharpier` -> In "Press shortcut keys", press `Ctrl+K, Ctrl+D` -> Click **Assign**.

#### Visual Studio Code Settings

The project comes with a pre-configured `.vscode/settings.json` that enforces CSharpier as the default formatter.

To ensure it works correctly, please **install the following extensions**:

1.  **C# Dev Kit** (id: `ms-dotnettools.csdevkit`) - *Required for running and debugging .NET projects.*
2.  **CSharpier - Code Formatter** (id: `csharpier.csharpier-vscode`) - *Required for automatic code formatting.*

Once installed, simply **Save (Ctrl+S)** any `.cs` file, and it will be formatted automatically. No further configuration is needed.

#### Manual Execution Command

> **⚠️ Note: This is a bulk operation!** Before running this command, please **ensure you Commit your current changes**. This command will format all `.cs` files and may generate a large number of changes. Please carefully check `git status` to ensure no unintended files (such as other people's code or Migration files) are accidentally modified before committing.

```
dotnet csharpier format .
```

### 2. Time Handling Standards (Time Provider)

To ensure the testability of business logic (such as "determining if it is within rostered hours"), **it is strictly forbidden to use `DateTime.Now` or `DateTimeOffset.Now` directly in the code**.

- **Correct Approach**: Inject `IDateTimeProvider` via the constructor.
- **Usage**: `_dateTimeProvider.Now`.

### 3. File Storage Strategy

This project uses the Adapter Pattern to handle file storage, with the core interface being `IFileStorageService`.

- **Development Environment**:
  - Injects `LocalFileStorageService` by default.
  - Physical Path: `src/FairWorkly.API/wwwroot/uploads/`.
  - **Note**: This directory is ignored in `.gitignore` and `.csharpierignore`.

### 4. Dependency Injection Standards

This project follows Clean Architecture, and dependency injection registration logic is split by layer. Whenever you add a new Service or Repository, **you must register the service in the corresponding layer**:

- **Application Layer Services** (e.g., `IEmployeeService`, `MediatR` Handlers):
  - **Location**: `src/FairWorkly.Application/DependencyInjection.cs`
- **Infrastructure Layer Services** (e.g., `IUserRepository`, `IDateTimeProvider`):
  - **Location**: `src/FairWorkly.Infrastructure/DependencyInjection.cs`

> **⚠️ Note**: `Program.cs` in the API layer is only responsible for calling these two extension methods. **It is strictly forbidden to register business services directly in `Program.cs`**.

## 📂 Cheatsheet

**Add Migration:**

```
dotnet ef migrations add <MigrationName> --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API --output-dir Persistence/Migrations
```

**Update Database:**

```
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

## 📚 Advanced Development Guide

For details on how to develop new Features, write AI Orchestrators, and interface with the Python Agent, please be sure to read the detailed internal tutorial.