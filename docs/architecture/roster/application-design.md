# Roster Application Layer Design Document

> This document helps new developers understand the Roster validation orchestration logic in the Application layer.

## 1. What Does the Application Layer Do?

### 1.1 Layer Responsibilities

In Clean Architecture, the **Application Layer** sits between the API (presentation) and Domain layers:

```
┌─────────────────────────────────────────┐
│           API Layer (Controllers)        │  ← Receives HTTP requests
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│       Application Layer (This doc)       │  ← Orchestrates business workflows
│  - Commands/Queries (CQRS)              │
│  - Handlers (MediatR)                    │
│  - Repository interfaces                 │
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│           Domain Layer                   │  ← Pure business rules (see roster-domain-design.md)
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│       Infrastructure Layer               │  ← Database, external services
└─────────────────────────────────────────┘
```

### 1.2 Application Layer Does NOT Contain

| ❌ Not Here | Where It Belongs |
|-------------|------------------|
| Business rules (e.g., "minimum shift is 3 hours") | Domain Layer |
| Database queries (EF Core, SQL) | Infrastructure Layer |
| HTTP endpoints, authentication | API Layer |
| UI components | Frontend |

### 1.3 Application Layer DOES Contain

| ✅ Here | Example |
|---------|---------|
| Use case orchestration | "Validate a roster" workflow |
| Transaction boundaries | When to commit changes |
| Input validation | "RosterId cannot be empty" |
| Response mapping | Convert entities to DTOs |
| Error handling | Catch exceptions, return Result |

---

## 2. Architecture Patterns Used

### 2.1 CQRS (Command Query Responsibility Segregation)

We separate **Commands** (write operations) from **Queries** (read operations):

```
Commands (modify state):
├── ValidateRosterCommand      → Creates RosterValidation + RosterIssues

Queries (read state):
├── GetValidationByIdQuery     → Returns validation details (future)
├── GetRosterIssuesQuery       → Returns issues for a roster (future)
```

**Why CQRS?**
- Commands and queries have different needs (validation vs. performance)
- Easier to scale read operations independently
- Clear intent: "Is this changing data or just reading?"

### 2.2 MediatR (Mediator Pattern)

MediatR decouples the sender from the handler:

```csharp
// Controller sends a command (doesn't know who handles it)
var result = await _mediator.Send(new ValidateRosterCommand { RosterId = id });

// Handler receives and processes it
public class ValidateRosterHandler : IRequestHandler<ValidateRosterCommand, Result<ValidateRosterResponse>>
{
    public async Task<Result<ValidateRosterResponse>> Handle(ValidateRosterCommand request, ...)
    {
        // Orchestration logic here
    }
}
```

**Why MediatR?**
- Controllers stay thin (no business logic)
- Handlers are testable in isolation
- Cross-cutting concerns via Pipeline Behaviors (validation, logging)

### 2.3 Repository Pattern

Interfaces live in Application layer; implementations in Infrastructure:

```
Application/Roster/Interfaces/
├── IRosterRepository.cs           ← Interface (Application owns this)
└── IRosterValidationRepository.cs

Infrastructure/Persistence/Repositories/Roster/
├── RosterRepository.cs            ← Implementation (Infrastructure owns this)
└── RosterValidationRepository.cs
```

**Why?**
- Application layer doesn't depend on EF Core or any database
- Easy to mock repositories for unit testing
- Can swap implementations (e.g., from EF Core to Dapper)

### 2.4 Unit of Work Pattern

`IUnitOfWork` manages transaction boundaries:

```csharp
// Multiple repository operations...
await validationRepository.CreateAsync(validation);
await validationRepository.AddIssuesAsync(issues);

// Single commit point - all or nothing
await unitOfWork.SaveChangesAsync(cancellationToken);
```

**Why?**
- Atomic operations (either all succeed or all rollback)
- Handler controls when data is persisted
- Repositories don't call SaveChanges themselves

---

## 3. Code Structure

### 3.1 Directory Layout

```
FairWorkly.Application/
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs     # MediatR pipeline for FluentValidation
│   └── Interfaces/
│       └── IUnitOfWork.cs
│
├── Roster/
│   ├── Features/
│   │   └── ValidateRoster/
│   │       ├── ValidateRosterCommand.cs           # Input DTO
│   │       ├── ValidateRosterCommandValidator.cs  # FluentValidation rules
│   │       ├── ValidateRosterHandler.cs           # Orchestration logic
│   │       └── ValidateRosterResponse.cs          # Output DTO
│   │
│   ├── Interfaces/
│   │   ├── IRosterRepository.cs                   # Read roster + shifts
│   │   └── IRosterValidationRepository.cs         # Write validation results
│   │
│   ├── Services/
│   │   ├── IRosterComplianceEngine.cs             # Engine interface
│   │   └── RosterComplianceEngine.cs              # Runs all rules
│   │
│   └── Orchestrators/
│       └── RosterAiOrchestrator.cs                # Future AI features
│
└── DependencyInjection.cs                         # Service registration
```

### 3.2 Feature Folder Structure

Each feature (use case) is self-contained:

```
Features/ValidateRoster/
├── ValidateRosterCommand.cs      # "What do we need?" (input)
├── ValidateRosterCommandValidator.cs  # "Is the input valid?"
├── ValidateRosterHandler.cs      # "How do we process it?"
└── ValidateRosterResponse.cs     # "What do we return?" (output)
```

**Benefits:**
- All related code in one place
- Easy to find and modify
- New features don't affect existing ones

---

## 4. ValidateRosterHandler Deep Dive

### 4.1 Complete Execution Flow

```
┌──────────────────────────────────────────────────────────────────────┐
│                    ValidateRosterHandler.Handle()                     │
└──────────────────────────────────────────────────────────────────────┘
                                  │
        ┌─────────────────────────┼─────────────────────────┐
        ▼                         ▼                         ▼
   1. Load Data              2. Create Record          3. Run Checks
   ────────────              ───────────────          ─────────────
   GetByIdWithShiftsAsync    new RosterValidation     complianceEngine
   ├─ Roster                 ├─ Status=InProgress     .EvaluateAll()
   ├─ Shifts                 └─ SaveChangesAsync()    ├─ DataQualityRule
   └─ Employees                  (persist early!)     ├─ MinimumShiftHoursRule
                                                      ├─ MealBreakRule
        │                         │                   ├─ RestPeriodRule
        │                         │                   ├─ WeeklyHoursLimitRule
        │                         │                   └─ ConsecutiveDaysRule
        ▼                         ▼                         │
   4. Enrich Issues          5. Calculate Stats            │
   ────────────────          ─────────────────             │
   foreach issue:            TotalShifts                   │
   ├─ issue.Id               FailedShifts                  │
   ├─ issue.OrganizationId   PassedShifts                  │
   └─ issue.ValidationId     TotalIssues ◄─────────────────┘
                             CriticalIssues (Severity >= Error)
        │                         │
        ▼                         ▼
   6. Persist Results        7. Build Response
   ──────────────────        ────────────────
   AddIssuesAsync            new ValidateRosterResponse
   UpdateAsync(validation)   ├─ ValidationId
   SaveChangesAsync()        ├─ Status (Passed/Failed)
                             ├─ Statistics
                             └─ Issues (with EmployeeName)
```

### 4.2 Key Design Decisions

#### 4.2.1 InProgress Status Persisted Early

```csharp
await validationRepository.CreateAsync(validation, cancellationToken);
// Persist InProgress record immediately so it's visible to frontend/background queries
await unitOfWork.SaveChangesAsync(cancellationToken);
```

**Why?**
- Frontend can poll and see "Validation in progress"
- If process crashes, database shows stuck InProgress record (can be cleaned up)
- Makes the status transitions observable: InProgress → Passed/Failed

#### 4.2.2 Handler Ensures Issue Fields Are Set

```csharp
// Ensure required fields are set on each issue
foreach (var issue in issues)
{
    if (issue.Id == Guid.Empty)
        issue.Id = Guid.NewGuid();

    // Ensure critical foreign keys are always set (don't rely solely on rules)
    issue.OrganizationId = request.OrganizationId;
    issue.RosterValidationId = validation.Id;
}
```

**Why?**
- Defense in depth: rules might forget to set fields
- Single place for critical field assignment
- Guarantees database constraints are satisfied

#### 4.2.3 Employee Name Lookup with Null Safety

```csharp
var employeeNameById = roster
    .Shifts.Where(s => s.EmployeeId != Guid.Empty && s.Employee != null)
    .Select(s => new { s.EmployeeId, Name = s.Employee!.FullName })
    .GroupBy(x => x.EmployeeId)
    .ToDictionary(g => g.Key, g => g.First().Name);
```

**Why?**
- Repository uses `AsNoTracking()` which might not load all navigation properties
- `s.Employee != null` prevents NullReferenceException
- Dictionary lookup is O(1) for each issue

#### 4.2.4 CriticalIssues Counts Error AND Critical

```csharp
// Count issues that cause validation to fail (Error or Critical)
var failingIssues = issues.Count(i => i.Severity >= IssueSeverity.Error);
```

**Why?**
- IssueSeverity enum: Info=1, Warning=2, Error=3, Critical=4
- `>= Error` includes both Error and Critical severities
- Warning and Info don't fail validation (may just need attention)

### 4.3 Exception Handling Strategy

```csharp
try
{
    // Run compliance checks and save results
}
catch (Exception ex) when (
    ex is not OperationCanceledException && !cancellationToken.IsCancellationRequested
)
{
    validation.Status = ValidationStatus.Failed;
    validation.Notes = ToSafeNotes($"Validation failed: {ex.Message}");
    await validationRepository.UpdateAsync(validation, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<ValidateRosterResponse>.Failure("Roster validation failed.");
}
```

**Key Points:**
- `OperationCanceledException` is NOT caught (let it propagate - user cancelled)
- Other exceptions are caught and recorded in database
- Validation record shows Failed status with error notes
- User gets a generic error message (no stack trace exposed)

---

## 5. Repository Interfaces

### 5.1 IRosterRepository

```csharp
public interface IRosterRepository
{
    Task<RosterEntity?> GetByIdWithShiftsAsync(
        Guid rosterId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
```

**Design Notes:**
- Uses type alias `RosterEntity` to avoid namespace conflict with `FairWorkly.Application.Roster`
- `organizationId` ensures tenant isolation
- Returns null if not found (caller handles with Result.NotFound)

### 5.2 IRosterValidationRepository

```csharp
public interface IRosterValidationRepository
{
    Task<RosterValidation> CreateAsync(RosterValidation validation, ...);
    Task UpdateAsync(RosterValidation validation, ...);
    Task AddIssuesAsync(IEnumerable<RosterIssue> issues, ...);
}
```

**Design Notes:**
- `CreateAsync` adds entity to context (doesn't save yet)
- `UpdateAsync` marks entity as modified (doesn't save yet)
- Handler calls `unitOfWork.SaveChangesAsync()` to commit

---

## 6. RosterComplianceEngine

### 6.1 Interface

```csharp
public interface IRosterComplianceEngine
{
    IReadOnlyList<RosterCheckType> GetExecutedCheckTypes();
    List<RosterIssue> EvaluateAll(IEnumerable<Shift> shifts, Guid validationId);
}
```

### 6.2 Implementation

```csharp
public class RosterComplianceEngine : IRosterComplianceEngine
{
    private readonly IEnumerable<IRosterComplianceRule> _rules;

    public RosterComplianceEngine(IEnumerable<IRosterComplianceRule> rules)
    {
        _rules = rules;  // DI injects all registered rules
    }

    public IReadOnlyList<RosterCheckType> GetExecutedCheckTypes()
    {
        return _rules.Select(r => r.CheckType).Distinct().OrderBy(c => (int)c).ToArray();
    }

    public List<RosterIssue> EvaluateAll(IEnumerable<Shift> shifts, Guid validationId)
    {
        var allIssues = new List<RosterIssue>();
        var shiftsList = shifts.ToList();

        foreach (var rule in _rules)
        {
            var issues = rule.Evaluate(shiftsList, validationId);
            allIssues.AddRange(issues);
        }

        return allIssues;
    }
}
```

**Why Engine in Application Layer?**
- Orchestrates multiple domain rules (cross-cutting concern)
- Domain rules are pure (no DI dependencies beyond parameters)
- Engine handles DI integration via constructor injection

---

## 7. Dependency Injection Registration

### 7.1 Application Layer Services

```csharp
// DependencyInjection.cs

// Register Roster Compliance Engine + Rules
// NOTE: Singleton is appropriate while parameters are static Award rules.
// Change to Scoped if parameters need to vary per-tenant or read from DB.
services.AddSingleton<IRosterRuleParametersProvider, AwardRosterRuleParametersProvider>();
services.AddScoped<IRosterComplianceEngine, RosterComplianceEngine>();

// DataQualityRule first - detects missing Employee data before other rules run
services.AddScoped<IRosterComplianceRule, DataQualityRule>();
services.AddScoped<IRosterComplianceRule, MinimumShiftHoursRule>();
services.AddScoped<IRosterComplianceRule, MealBreakRule>();
services.AddScoped<IRosterComplianceRule, RestPeriodRule>();
services.AddScoped<IRosterComplianceRule, WeeklyHoursLimitRule>();
services.AddScoped<IRosterComplianceRule, ConsecutiveDaysRule>();
```

**Key Points:**
- **Registration order matters**: DataQualityRule first ensures data issues are caught before other rules assume valid data
- **Singleton vs Scoped**: Parameters provider is Singleton (static values), rules are Scoped (may access DbContext indirectly)
- **Comment for future**: Explains when to change Singleton to Scoped

### 7.2 Infrastructure Layer Services

```csharp
// Register Repositories
services.AddScoped<IRosterRepository, RosterRepository>();
services.AddScoped<IRosterValidationRepository, RosterValidationRepository>();

// Register UnitOfWork
services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 8. Input Validation with FluentValidation

### 8.1 Command Validator

```csharp
public class ValidateRosterCommandValidator : AbstractValidator<ValidateRosterCommand>
{
    public ValidateRosterCommandValidator()
    {
        RuleFor(x => x.RosterId).NotEmpty().WithMessage("RosterId is required.");
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("OrganizationId is required.");
    }
}
```

### 8.2 Validation Pipeline

```csharp
// MediatR pipeline behavior
cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
```

**How It Works:**
1. Controller sends `ValidateRosterCommand`
2. `ValidationBehavior` intercepts before handler
3. Finds `ValidateRosterCommandValidator` and runs it
4. If validation fails, throws `ValidationException` (never reaches handler)
5. If validation passes, handler executes

**Why FluentValidation?**
- Declarative rules (easy to read)
- Automatic integration with MediatR
- Consistent error responses

---

## 9. Response DTOs

### 9.1 ValidateRosterResponse

```csharp
public class ValidateRosterResponse
{
    public Guid ValidationId { get; set; }
    public ValidationStatus Status { get; set; }
    public int TotalShifts { get; set; }
    public int PassedShifts { get; set; }
    public int FailedShifts { get; set; }
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }  // Severity >= Error
    public List<RosterIssueSummary> Issues { get; set; } = [];
}
```

### 9.2 RosterIssueSummary

```csharp
public class RosterIssueSummary
{
    public Guid Id { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }  // Denormalized for display
    public string CheckType { get; set; }
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; }
    public decimal? ExpectedValue { get; set; }
    public decimal? ActualValue { get; set; }
    public string? AffectedDates { get; set; }
}
```

**Why Separate DTOs?**
- Decouples API contract from domain entities
- Can include derived/computed fields (EmployeeName)
- Controls what data is exposed to clients

---

## 10. How to Extend

### 10.1 Adding a New Query (e.g., Get Validation History)

1. Create feature folder:

```
Features/GetValidationHistory/
├── GetValidationHistoryQuery.cs
├── GetValidationHistoryHandler.cs
└── GetValidationHistoryResponse.cs
```

2. Define the query:

```csharp
public class GetValidationHistoryQuery : IRequest<Result<List<ValidationHistoryItem>>>
{
    public Guid RosterId { get; set; }
    public Guid OrganizationId { get; set; }
}
```

3. Implement the handler (inject `IRosterValidationRepository` or a new read-specific repository).

### 10.2 Adding Repository Methods

1. Add method to interface (Application layer):

```csharp
public interface IRosterValidationRepository
{
    // ... existing methods
    Task<List<RosterValidation>> GetByRosterIdAsync(Guid rosterId, Guid organizationId, CancellationToken ct);
}
```

2. Implement in Infrastructure layer:

```csharp
public async Task<List<RosterValidation>> GetByRosterIdAsync(...)
{
    return await _context.RosterValidations
        .AsNoTracking()
        .Where(v => v.RosterId == rosterId && v.OrganizationId == organizationId)
        .OrderByDescending(v => v.StartedAt)
        .ToListAsync(cancellationToken);
}
```

### 10.3 Adding Cross-Cutting Concerns

Use MediatR Pipeline Behaviors:

```csharp
// Example: Logging behavior
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
```

Register in DI:

```csharp
cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
```

---

## 11. Required Background Knowledge

### 11.1 Technical Concepts

| Concept | Description | Learn More |
|---------|-------------|------------|
| **Clean Architecture** | Layers with dependency inversion | [Uncle Bob's article](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) |
| **CQRS** | Separate read/write models | [Martin Fowler](https://martinfowler.com/bliki/CQRS.html) |
| **MediatR** | In-process messaging library | [GitHub](https://github.com/jbogard/MediatR) |
| **FluentValidation** | Strongly-typed validation rules | [Docs](https://docs.fluentvalidation.net/) |
| **Repository Pattern** | Abstract data access | [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) |
| **Unit of Work** | Transaction management pattern | [Martin Fowler](https://martinfowler.com/eaaCatalog/unitOfWork.html) |
| **Result Pattern** | Error handling without exceptions | Return `Result<T>` instead of throwing |

### 11.2 C# Language Features Used

| Feature | Example | Purpose |
|---------|---------|---------|
| **Primary Constructor** | `public class Handler(IRepo repo)` | Concise DI |
| **Local Functions** | `static string? ToSafeNotes(...)` | Helper methods scoped to method |
| **Pattern Matching** | `ex is not OperationCanceledException` | Type checking in catch |
| **Collection Expressions** | `= []` | Initialize empty list |
| **Null-forgiving Operator** | `s.Employee!.FullName` | Assert non-null after check |
| **Namespace Alias** | `using RosterEntity = ...` | Resolve naming conflicts |

---

## 12. FAQ

### Q: Why doesn't the handler call repositories directly for SaveChanges?

A: Following Unit of Work pattern. Repositories only track changes; handler decides when to commit. This allows multiple operations in a single transaction.

### Q: Why is RosterComplianceEngine in Application layer, not Domain?

A: The engine orchestrates multiple rules and handles DI injection. Domain layer should remain pure (no DI container awareness). Rules themselves live in Domain layer.

### Q: Why use Result<T> instead of throwing exceptions?

A: Exceptions are for exceptional cases (unexpected errors). "Roster not found" is expected and should be handled gracefully. Result pattern makes success/failure explicit in the type system.

### Q: Why persist InProgress status before running checks?

A: Observable state. Frontend can show "Validation in progress" and users won't wonder if their request was received. Also helps with crash recovery.

### Q: Why does registration order of rules matter?

A: `IEnumerable<IRosterComplianceRule>` resolves in registration order. DataQualityRule first ensures it reports missing Employee data before other rules assume valid data.

### Q: Why Singleton for IRosterRuleParametersProvider?

A: Current implementation returns static Award parameters. No per-request state. If future requirements need tenant-specific or DB-loaded parameters, change to Scoped.

---

## 13. Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-02 | Initial version |

---

*Document Maintenance: Please update this file when Application layer changes are made*
