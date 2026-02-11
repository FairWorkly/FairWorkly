# Error Handling Architecture

This document provides a comprehensive overview of FairWorkly's error handling architecture and how different error mechanisms work together.

## Architecture Overview

FairWorkly implements **three layers of error handling**, each serving a distinct purpose while working together cohesively:

```
┌─────────────────────────────────────────────────────────────────┐
│                    1️⃣ Result<T> Pattern                        │
│              (Foundation / Generic Framework)                    │
│   Purpose: Unified return value wrapper for all Application     │
│            layer Handlers                                        │
└─────────────────────────────────────────────────────────────────┘
                              │
                    ┌─────────▼──────────┐
                    │ 1b. ResultMapping  │
                    │      Filter        │
                    │ (API IActionFilter)│
                    │ Auto-maps Result<T>│
                    │ → HTTP response    │
                    └─────────┬──────────┘
                              │
                ┌─────────────┴─────────────┐
                │                           │
┌───────────────▼──────────┐   ┌────────────▼───────────────────┐
│ 2️⃣ GlobalExceptionHandler│   │ 3️⃣ Business-Specific Errors   │
│   (Global Exception       │   │  - RosterIssue (compliance)   │
│    Catcher)               │   │  - ParseIssue (Excel parsing) │
│                          │   │  - CSV error lists            │
│ Purpose: Catch exceptions│   │                              │
│ that escape Result       │   │ Purpose: Return business data,│
│ wrapper (database errors,│   │ not errors                    │
│ domain exceptions)       │   │                              │
└──────────────────────────┘   └───────────────────────────────┘
```

---

## 1️⃣ Result\<T\> Pattern - Foundation Framework

### Purpose
Unified error handling wrapper for the Handler layer.

### What It Handles
- ✅ Success with data
- ❌ Validation failures (FluentValidation)
- ❌ Business logic failures
- ❌ Resource not found
- ❌ Authentication/Authorization failures

### Implementation

**File Locations:**
- `src/FairWorkly.Domain/Common/Result.cs` - Main Result<T> class
- `src/FairWorkly.Domain/Common/IResultBase.cs` - Interface
- `src/FairWorkly.Domain/Common/ValidationError.cs` - Validation error class

**Result Types:**
```csharp
public enum ResultType
{
    Success,            // Operation succeeded
    ValidationFailure,  // Input validation failed
    BusinessFailure,    // Business logic failed
    NotFound,          // Resource not found
    Unauthorized,      // Not authenticated
    Forbidden,         // Not authorized
}
```

### Example Usage

**Handler:**
```csharp
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(...)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        // ❌ User not found or invalid password → Unauthorized
        if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Unauthorized("Invalid email or password.");
        }

        // ❌ Account disabled → Forbidden
        if (!user.IsActive)
        {
            return Result<LoginResponse>.Forbidden("Account is disabled.");
        }

        // ✅ Success
        return Result<LoginResponse>.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = userDto
        });
    }
}
```

**Controller (uses ResultMappingFilter):**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginCommand command)
{
    var result = await mediator.Send(command);

    // Filter auto-maps failures → ProblemDetails (400/401/403/404)
    if (result.IsFailure)
        return new ObjectResult(result);

    // Side effect: must set cookie before returning success
    SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiration);
    return Ok(result.Value);  // 200
}
```

> See [ResultMappingFilter](#1b-resultmappingfilter---centralized-result-to-http-mapping) for how `new ObjectResult(result)` is automatically transformed.

### ValidationBehavior - Automatic Validation

FluentValidation errors are automatically intercepted and converted to `Result<T>.ValidationFailure`:

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : IResultBase  // Only applies to Result-based handlers
{
    public async Task<TResponse> Handle(...)
    {
        // Execute FluentValidation validators
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            // 🔥 Auto-convert to ValidationError list
            var errors = failures.Select(f => new ValidationError
            {
                Field = f.PropertyName,
                Message = f.ErrorMessage,
            }).ToList();

            // 🔥 Return ValidationFailure using reflection
            return (TResponse)Result<T>.ValidationFailure(errors);
        }

        return await next();  // Validation passed, continue to handler
    }
}
```

**Flow:**
```
Request comes in
   ↓
ValidationBehavior (FluentValidation check)
   ↓ Failed → Auto-returns Result.ValidationFailure(errors)
   ↓ Passed
Handler executes business logic
   ↓
Returns Result<T>
   ↓
Controller returns new ObjectResult(result)
   ↓
ResultMappingFilter auto-maps → HTTP response
```

---

## 1b. ResultMappingFilter - Centralized Result-to-HTTP Mapping

### Purpose

Automatically transform `Result<T>` into the correct HTTP response. This eliminates manual `if/else` chains in controllers and keeps controllers thin (pure dispatchers).

### File Location

`src/FairWorkly.API/Filters/ResultMappingFilter.cs`

### How It Works

The filter is registered globally as an `IAsyncActionFilter`. It runs **after** each controller action:

1. Inspects `executedContext.Result` — if it's an `ObjectResult` whose `Value` implements `IResultBase`, the filter activates
2. **Success**: extracts `Result<T>.Value` and returns `OkObjectResult(value)` (200 OK)
3. **Failure**: maps `ResultType` to ProblemDetails with the appropriate HTTP status code

If the result is NOT a `Result<T>` (e.g., `FileResult`, `UnauthorizedResult`, plain `ObjectResult`), the filter does nothing.

### ResultType → HTTP Mapping

| ResultType | HTTP Status | ProblemDetails Title | Response Body |
|---|---|---|---|
| `Success` | 200 | — | `result.Value` (unwrapped) |
| `ValidationFailure` | 400 | "Validation Failed" | ProblemDetails + `errors` extension |
| `BusinessFailure` | 400 | "Bad Request" | ProblemDetails |
| `NotFound` | 404 | "Resource Not Found" | ProblemDetails |
| `Unauthorized` | 401 | "Unauthorized" | ProblemDetails |
| `Forbidden` | 403 | "Forbidden" | ProblemDetails |

All error responses use **ProblemDetails (RFC 7807)** format, consistent with `GlobalExceptionHandler`.

### Controller Patterns

**Pattern 1: Pure dispatch** — for actions that just send a command/query and return the result.

```csharp
[HttpGet]
public async Task<IActionResult> GetOrganizationAwards()
{
    // ... pre-checks ...
    var result = await mediator.Send(query);
    return new ObjectResult(result);  // Filter handles everything
}
```

**Pattern 2: Side effects on success** — for actions that need to do something (e.g., set cookies) before returning the success response.

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginCommand command)
{
    var result = await mediator.Send(command);

    if (result.IsFailure)
        return new ObjectResult(result);  // Filter handles error mapping

    SetRefreshTokenCookie(result.Value!.RefreshToken, ...);  // Side effect
    return Ok(result.Value);  // Manual success response
}
```

**Pattern 3: Custom success response** — for actions that return a different shape than `result.Value`.

```csharp
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
{
    var result = await mediator.Send(command);

    if (result.IsFailure)
        return new ObjectResult(result);  // Filter handles error mapping

    return Ok(new { message = "If that email exists, a reset link has been sent." });
}
```

**Pattern 4: Non-Result flow** — the filter automatically ignores actions that don't return `Result<T>`.

```csharp
[HttpGet("{rosterId}/download")]
public async Task<IActionResult> DownloadOriginalFile(Guid rosterId, CancellationToken ct)
{
    // ... manual logic, returns FileResult ...
    return File(fileStream, contentType, fileName);  // Filter ignores this
}
```

### Validation Error Response Format

For `ResultType.ValidationFailure`, the ProblemDetails includes an `errors` extension with field-level errors grouped by field name:

```json
{
  "status": 400,
  "title": "Validation Failed",
  "detail": "One or more validation errors occurred.",
  "instance": "/api/auth/login",
  "errors": {
    "Email": ["Email is required", "Email must be a valid email address"],
    "Password": ["Password is required"]
  }
}
```

This format is identical to what `GlobalExceptionHandler` produces for `ValidationException`, ensuring consistency.

### Frontend Integration

The frontend `normalizeApiError` function (in `frontend/src/shared/types/api.types.ts`) reads the error message from `data.detail` (ProblemDetails) with fallback to `data.message` (legacy format):

```typescript
const message =
  data?.detail ||    // ProblemDetails: { detail: "Invalid credentials" }
  data?.message ||   // Legacy format:  { message: "Invalid credentials" }
  error.message ||
  "Request failed. Please try again.";
```

### Registration

Registered globally in `Program.cs`:

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResultMappingFilter>();
});
```

---

## ValidationBehavior vs ResultMappingFilter — Why Both?

These two mechanisms look similar (both intercept and transform `Result<T>`), but they operate at **different layers** and solve **different problems**.

### Request Lifecycle

```
HTTP Request
   ↓
┌──────────────────────────────── ASP.NET MVC Pipeline ──────────────────────────────────┐
│  Controller action invoked                                                              │
│     ↓                                                                                   │
│  ┌───────────────────────────── MediatR Pipeline ─────────────────────────────────────┐ │
│  │  1️⃣ ValidationBehavior         (Application layer)                                 │ │
│  │     - Runs FluentValidation validators on the Command/Query                        │ │
│  │     - If invalid → short-circuits, returns Result<T>.ValidationFailure             │ │
│  │     - If valid   → passes through to Handler                                      │ │
│  │     ↓                                                                              │ │
│  │  Handler executes business logic                                                   │ │
│  │     ↓                                                                              │ │
│  │  Returns Result<T>  (Success / NotFound / Forbidden / etc.)                        │ │
│  └────────────────────────────────────────────────────────────────────────────────────┘ │
│     ↓                                                                                   │
│  Controller returns new ObjectResult(result)                                            │
│     ↓                                                                                   │
│  2️⃣ ResultMappingFilter           (API layer)                                          │
│     - Inspects the ObjectResult                                                         │
│     - Maps Result<T> → HTTP response (200/400/401/403/404 + ProblemDetails)             │
│     ↓                                                                                   │
│  HTTP Response sent to client                                                           │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

### Comparison

|  | ValidationBehavior | ResultMappingFilter |
|---|---|---|
| **Layer** | Application (`FairWorkly.Application`) | API (`FairWorkly.API`) |
| **Mechanism** | MediatR `IPipelineBehavior` | ASP.NET `IAsyncActionFilter` |
| **Runs when** | Before the Handler executes | After the Controller action returns |
| **Input** | MediatR `TRequest` (Command/Query) | `ObjectResult` containing `Result<T>` |
| **Output** | `Result<T>` (domain type) | `ActionResult` (HTTP response) |
| **Purpose** | Validate input data (FluentValidation) | Map result to HTTP status + ProblemDetails |
| **Knows about HTTP?** | No — pure domain types | Yes — status codes, ProblemDetails, headers |
| **Short-circuits?** | Yes — Handler never runs if validation fails | No — runs after action completes |

### Why Not Use a MediatR Behavior for HTTP Mapping?

You might wonder: "Can we add another pipeline behavior that maps `Result<T>` directly to an HTTP response, since we already have `ValidationBehavior` there?"

**No** — this would violate Clean Architecture:

```
Domain ← Application ← Infrastructure
                     ← API               ← HTTP concerns live here

MediatR behaviors live in Application.
HTTP concepts (ProblemDetails, StatusCodes, ActionResult) live in API.
Application must NOT reference API types.
```

A MediatR behavior that returns `ActionResult` or `ProblemDetails` would require the Application layer to depend on `Microsoft.AspNetCore.Mvc` — breaking the dependency rule.

### When to Use Which

| Scenario | Use |
|---|---|
| Validate command/query input fields (email format, required fields, etc.) | `ValidationBehavior` (via FluentValidation) |
| Cross-cutting input validation for all handlers | MediatR `IPipelineBehavior` |
| Map `Result<T>` success/failure to HTTP response | `ResultMappingFilter` (return `new ObjectResult(result)`) |
| Handle side effects before returning HTTP response (cookies, headers) | Controller manually checks `result.IsFailure`, delegates errors to filter |
| Catch unhandled exceptions from infrastructure | `GlobalExceptionHandler` |

### Summary

- **ValidationBehavior** answers: *"Is the input valid?"* (pre-processing)
- **ResultMappingFilter** answers: *"What HTTP response does this result become?"* (post-processing)
- They work together: ValidationBehavior produces `Result<T>.ValidationFailure` → Controller returns it → ResultMappingFilter transforms it to a 400 ProblemDetails response.

---

## 2️⃣ GlobalExceptionHandler - Safety Net

### Purpose
Catch exceptions that **escape the Result pattern** (primarily infrastructure layer exceptions).

### File Location
`src/FairWorkly.API/ExceptionHandlers/GlobalExceptionHandler.cs`

### Exception Types Handled

| Exception Type | HTTP Status | Scenario |
|---------------|-------------|----------|
| `ValidationException` | 400 | FluentValidation throws exception (legacy code) |
| `NotFoundException` | 404 | Domain exception: resource not found |
| `ForbiddenAccessException` | 403 | Domain exception: insufficient permissions |
| `DomainException` | 422 | Domain rule violation |
| **Database exceptions** (default) | 500 | `DbUpdateException`, `SqlException`, etc. |
| **Unknown exceptions** | 500 | All other exceptions |

### Implementation

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title, detail, extensions) = exception switch
        {
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                "One or more validation errors occurred.",
                new Dictionary<string, object?> { { "errors", valEx.Errors } }
            ),

            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                exception.Message,
                null
            ),

            ForbiddenAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message,
                null
            ),

            DomainException => (
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                exception.Message,
                null
            ),

            // 🔥 Database errors fall into default case
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An error occurred while processing your request.",
                null
            ),
        };

        // Return RFC 7807 ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
```

### Database Error Handling

Database exceptions (foreign key violations, unique constraint violations, etc.) are caught and returned as **500 Internal Server Error** because they are **system-level issues** that should not expose implementation details to users.

### Key Points
- GlobalExceptionHandler is the **last line of defense**
- It catches exceptions that escape the Result pattern
- Database errors are treated as system failures (500)
- Domain exceptions are mapped to appropriate HTTP status codes

---

## 3️⃣ Business-Specific Errors - Data Carriers (Not Exceptions!)

### ⚠️ Critical Clarification

These are **NOT error handling mechanisms** - they are **business data**! They are returned through `Result<T>.Value` as part of successful responses.

---

### A. RosterIssue - Compliance Check Results

**Classification:** Domain entity, persisted in database

**Purpose:** Record **compliance issues** found during roster validation

**File Location:** `src/FairWorkly.Domain/Roster/Entities/RosterIssue.cs`

**Structure:**
```csharp
public class RosterIssue : BaseEntity
{
    public RosterCheckType CheckType { get; set; }      // Which check found this
    public IssueSeverity Severity { get; set; }         // Critical/Error/Warning/Info
    public string Description { get; set; }             // Short description
    public decimal? ExpectedValue { get; set; }         // Expected value
    public decimal? ActualValue { get; set; }           // Actual value
    public bool IsResolved { get; set; }                // Has been fixed?
    public string? DetailedExplanation { get; set; }    // AI-generated explanation
    public string? Recommendation { get; set; }         // How to fix
    // ... more fields
}
```

**Usage in Handler:**
```csharp
public class ValidateRosterHandler : IRequestHandler<ValidateRosterCommand, Result<ValidateRosterResponse>>
{
    public async Task<Result<ValidateRosterResponse>> Handle(...)
    {
        var roster = await rosterRepository.GetByIdWithShiftsAsync(...);

        if (roster == null)
        {
            return Result<ValidateRosterResponse>.NotFound("Roster not found");
        }

        // ✅ Run compliance checks
        var issues = complianceEngine.EvaluateAll(roster.Shifts, validation.Id);

        var response = new ValidateRosterResponse
        {
            ValidationId = validation.Id,
            Status = failingIssues > 0 ? ValidationStatus.Failed : ValidationStatus.Passed,
            TotalIssues = issues.Count,  // Could be 50 issues
            Issues = issues.Select(i => new RosterIssueSummary { ... }).ToList()
        };

        // 🔥 This is SUCCESS! Even with 50 compliance issues
        return Result<ValidateRosterResponse>.Success(response);
    }
}
```

**Response to Frontend (200 OK):**
```json
{
  "validationId": "guid",
  "status": "Failed",
  "totalIssues": 50,
  "issues": [
    {
      "checkType": "MinimumShiftDuration",
      "severity": "Error",
      "description": "Shift only 2.5 hours, minimum is 3 hours",
      "expectedValue": 3.0,
      "actualValue": 2.5
    }
    // ... 49 more issues
  ]
}
```

---

### B. ParseIssue - Excel/CSV Parsing Results

#### Python Agent Service

**Classification:** Pydantic data model returned by parser

**File Location:** `agent-service/agents/roster/services/roster_import/models.py`

**Structure:**
```python
class ParseIssueSeverity(Enum):
    ERROR = "error"
    WARNING = "warning"

class ParseIssue(BaseModel):
    severity: ParseIssueSeverity  # ERROR or WARNING
    code: str                     # "MISSING_REQUIRED_COLUMNS"
    message: str                  # Error description
    row: int                      # Excel row number
    column: Optional[str]         # Which column
    value: Optional[str]          # Problematic value
    hint: Optional[str]           # Fix suggestion
    detail: Optional[str]         # Additional details
```

**Usage in Parser:**
```python
def parse_roster_excel(self, file_path: str, ...) -> ParseResponse:
    entries = []
    issues: list[ParseIssue] = []

    for row in data:
        try:
            entry, row_warnings = parse_roster_row(row, ...)
            entries.append(entry)
            issues.extend(row_warnings)  # Warnings are data
        except ParseIssueError as exc:
            issues.append(exc.issue)  # Errors converted to data
        except Exception as exc:
            issues.append(ParseIssue(
                row=real_row,
                severity=ParseIssueSeverity.ERROR,
                code="ROW_PARSE_ERROR",
                message=str(exc)
            ))

    # 🔥 Even with 20 parsing errors, return successful ParseResponse
    return build_response(
        RosterParseResult(entries=entries, raw_rows=raw_rows),
        issues
    )
```

**Response to Frontend:**
```json
{
  "result": {
    "entries": [...],
    "totalShifts": 100,
    "weekStartDate": "2024-01-01",
    "weekEndDate": "2024-01-07"
  },
  "summary": {
    "status": "row_error",
    "totalIssues": 15,
    "errorCount": 5,
    "warningCount": 10
  },
  "issues": [
    {
      "row": 15,
      "severity": "error",
      "code": "INVALID_DATE_FORMAT",
      "message": "Invalid date format in row 15",
      "column": "date",
      "value": "2024-13-45",
      "hint": "Use format: YYYY-MM-DD"
    }
  ]
}
```

---

#### C# Backend CSV Parser

**Classification:** Tuple return with data and error list

**File Location:** `src/FairWorkly.Application/Payroll/Services/CsvParserService.cs`

**Structure:**
```csharp
public class CsvParserService : ICsvParserService
{
    public async Task<(List<PayrollCsvRow> Rows, List<string> Errors)> ParseAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default
    )
    {
        var rows = new List<PayrollCsvRow>();
        var errors = new List<string>();  // 🔥 Error string list

        while (await csv.ReadAsync())
        {
            try
            {
                var row = csv.GetRecord<PayrollCsvRow>();
                var validationErrors = ValidateRow(row, rowNumber);

                if (validationErrors.Any())
                {
                    errors.AddRange(validationErrors);  // Collect errors
                    continue;  // Skip this row, continue parsing
                }

                rows.Add(row);
            }
            catch (Exception ex)
            {
                errors.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        // 🔥 Return partial success + error list
        return (rows, errors);
    }
}
```

**Usage in Handler:**
```csharp
public async Task<Result<PayrollDto>> Handle(...)
{
    var (rows, errors) = await _csvParser.ParseAsync(stream);

    // ❌ If errors exist, return ValidationFailure
    if (errors.Any())
    {
        var validationErrors = errors.Select(e => new ValidationError
        {
            Field = "csv",
            Message = e
        }).ToList();

        return Result<PayrollDto>.ValidationFailure(
            "CSV parsing failed",
            validationErrors
        );
    }

    // ✅ Success
    return Result<PayrollDto>.Success(payrollDto);
}
```

---

## How They Work Together

### Scenario 1: Roster Validation (Success with Issues)

```
User requests roster validation
   ↓
ValidateRosterHandler.Handle()
   ↓
Compliance engine checks → Finds 50 compliance issues (RosterIssue entities)
   ↓
✅ Result<ValidateRosterResponse>.Success(containing 50 RosterIssues)
   ↓
Controller: return new ObjectResult(result)
   ↓
ResultMappingFilter: IsSuccess → OkObjectResult(result.Value)  → 200
   ↓
Frontend displays: Validation failed, 50 issues need fixing
```

**Key Point:** This is a **successful Result**. Issues are part of the data.

---

### Scenario 2: Excel Parsing (Partial Success)

```
User uploads Excel file
   ↓
Agent Service: parse_roster_excel()
   ↓
Parse 100 rows → 95 succeed, 5 fail (ParseIssue)
   ↓
Return ParseResponse: { entries: 95, issues: 5 }
   ↓
Backend Handler receives ParseResponse
   ↓
✅ Result<RosterDto>.Success(containing 95 shifts + 5 issues)
   ↓
Controller: return new ObjectResult(result)
   ↓
ResultMappingFilter: IsSuccess → OkObjectResult(result.Value)  → 200
```

---

### Scenario 3: CSV Parsing Failure

```
User uploads CSV
   ↓
CsvParserService.ParseAsync() → (rows: 0, errors: ["Row 1: Invalid date"])
   ↓
Handler checks errors.Any() == true
   ↓
❌ Result<PayrollDto>.ValidationFailure(errors)
   ↓
Controller: return new ObjectResult(result)
   ↓
ResultMappingFilter: ValidationFailure → ProblemDetails 400 + errors
```

---

### Scenario 4: Database Constraint Violation

```
User creates duplicate Roster
   ↓
Handler: rosterRepository.CreateAsync()
   ↓
EF Core: DbUpdateException (unique index violation)
   ↓
❌ Exception escapes Handler (not try-caught)
   ↓
GlobalExceptionHandler catches it
   ↓
Returns 500 Internal Server Error
```

---

## Summary Comparison Table

| Error Type | Mechanism | Return Method | HTTP Status | Example |
|-----------|-----------|---------------|-------------|---------|
| **Validation Failure** | Result<T> | ValidationFailure | 400 | Invalid email format |
| **Business Failure** | Result<T> | Failure/NotFound | 400/404 | Roster not found |
| **Authentication** | Result<T> | Unauthorized | 401 | Wrong password |
| **Authorization** | Result<T> | Forbidden | 403 | Account disabled |
| **Compliance Issues** | Business Data | Success(with RosterIssue) | 200 | 50 roster violations |
| **Excel Parsing** | Business Data | Success(with ParseIssue) | 200 | 5 rows failed |
| **CSV Parsing** | Result<T> | ValidationFailure | 400 | Format error |
| **Database Error** | GlobalHandler | ProblemDetails | 500 | FK constraint violation |
| **Unknown Exception** | GlobalHandler | ProblemDetails | 500 | NullReferenceException |

---

## Design Philosophy

1. **Result\<T\>** - Predictable business errors (user input, permissions, etc.)
2. **Business Data** (RosterIssue/ParseIssue) - Successful execution results, even when containing issues
3. **GlobalExceptionHandler** - Safety net for system-level exceptions (database, network, etc.)

### Key Distinctions

- **RosterIssue** = "I successfully checked your roster and found 50 issues" (200 OK)
- **ValidationFailure** = "Your input has errors, cannot process" (400 Bad Request)
- **GlobalException** = "System error occurred, please contact admin" (500 Internal Error)

---

## Adding a New Feature — Error Handling Checklist

When building a new feature (e.g., Billing, Settings, Payroll), you get error handling **for free** if you follow the existing patterns. Here's what you need to do — and what you **don't** need to do.

### What the Framework Handles for You

| Concern | Handled by | You write |
|---|---|---|
| Input validation (required fields, format, range) | `ValidationBehavior` + FluentValidation | A `Validator` class only |
| Result → HTTP response mapping | `ResultMappingFilter` | Nothing — just `return new ObjectResult(result)` |
| Unhandled exceptions (DB errors, null refs) | `GlobalExceptionHandler` | Nothing — automatic |

### Step-by-Step: New Feature Example (Billing)

**1. Create a FluentValidation Validator** — input validation is automatic.

```csharp
// Application/Billing/Features/UpdatePlan/UpdatePlanCommandValidator.cs
public class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
{
    public UpdatePlanCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.Seats).GreaterThan(0).WithMessage("Seats must be at least 1");
    }
}
```

That's it. `ValidationBehavior` will run this automatically before your Handler. If validation fails, the controller receives `Result<T>.ValidationFailure` → the filter returns 400 ProblemDetails. **You write zero error handling code for this.**

**2. Write the Handler** — return `Result<T>` for business outcomes.

```csharp
// Application/Billing/Features/UpdatePlan/UpdatePlanHandler.cs
public class UpdatePlanHandler : IRequestHandler<UpdatePlanCommand, Result<UpdatePlanResponse>>
{
    public async Task<Result<UpdatePlanResponse>> Handle(...)
    {
        var plan = await _repo.GetByIdAsync(request.PlanId);

        if (plan == null)
            return Result<UpdatePlanResponse>.NotFound("Plan not found");

        if (plan.OrganizationId != request.OrganizationId)
            return Result<UpdatePlanResponse>.Forbidden("Not your plan");

        plan.UpdateSeats(request.Seats);
        await _repo.SaveAsync();

        return Result<UpdatePlanResponse>.Success(new UpdatePlanResponse { ... });

        // ❌ DO NOT: try/catch database exceptions — GlobalExceptionHandler handles them
        // ❌ DO NOT: throw new ValidationException — use Result<T>.ValidationFailure
        // ❌ DO NOT: return HTTP types (BadRequest, ProblemDetails) — that's the API layer's job
    }
}
```

**3. Write the Controller** — pure dispatch, no error mapping.

```csharp
// API/Controllers/Billing/BillingController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPut("plan")]
    public async Task<IActionResult> UpdatePlan([FromBody] UpdatePlanCommand command)
    {
        command.OrganizationId = Guid.Parse(currentUser.OrganizationId!);
        var result = await mediator.Send(command);
        return new ObjectResult(result);  // ✅ That's it. Filter handles everything.
    }

    // ❌ DO NOT: write if (result.IsFailure) return BadRequest(...)
    // ❌ DO NOT: write HandleValidationFailureAsync(...)
    // ❌ DO NOT: manually create ProblemDetails
}
```

**4. What happens automatically for each error scenario:**

| Scenario | Who handles it | HTTP Response |
|---|---|---|
| `Seats` is missing or ≤ 0 | Validator → `ValidationBehavior` → Filter | 400 + `{ errors: { "Seats": ["..."] } }` |
| Plan not found | Handler returns `NotFound` → Filter | 404 + ProblemDetails |
| Wrong organization | Handler returns `Forbidden` → Filter | 403 + ProblemDetails |
| DB connection failure | EF Core throws → `GlobalExceptionHandler` | 500 + ProblemDetails |
| Success | Handler returns `Success` → Filter | 200 + response body |

**You wrote 0 lines of error-to-HTTP mapping code.** The framework handles it all.

### When You DO Need Manual Handling

Only for controllers with **side effects between getting the result and returning the response**:

```csharp
// Example: Login sets a cookie on success — can't use pure dispatch
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginCommand command)
{
    var result = await mediator.Send(command);

    if (result.IsFailure)
        return new ObjectResult(result);  // Filter handles error → ProblemDetails

    // Side effect that must happen before response
    SetRefreshTokenCookie(result.Value!.RefreshToken, ...);
    return Ok(result.Value);
}
```

### Anti-Patterns — Do NOT Do These

```csharp
// ❌ Manual error mapping in controller — use the filter
if (result.Type == ResultType.ValidationFailure) { return BadRequest(...); }
if (result.Type == ResultType.NotFound) { return NotFound(...); }

// ❌ Duplicating HandleValidationFailureAsync — deleted, use the filter
private async Task<ActionResult> HandleValidationFailureAsync<T>(Result<T> result) { ... }

// ❌ Throwing exceptions for business logic — use Result<T>
if (plan == null) throw new NotFoundException("Plan not found");
// ✅ Instead:
if (plan == null) return Result<T>.NotFound("Plan not found");

// ❌ Catching DB exceptions in Handler — let GlobalExceptionHandler handle
try { await _repo.SaveAsync(); } catch (DbUpdateException ex) { return Result<T>.Failure(...); }
// ✅ Instead: just call await _repo.SaveAsync() without try/catch
```

---

## Best Practices Summary

### When to Use Result<T>

✅ **DO use Result\<T\>** for:
- Input validation failures (via FluentValidation + `ValidationBehavior`)
- Business rule violations
- Resource not found scenarios
- Authentication/authorization failures

❌ **DO NOT use Result\<T\>** for:
- Returning business data that includes issue lists (use `Result<T>.Success(dataWithIssues)`)
- System-level failures (let `GlobalExceptionHandler` catch them)

### When to Use Business Data Models

✅ **DO use RosterIssue/ParseIssue** when:
- The operation succeeded but found problems in the data
- Issues are part of the expected output
- Frontend needs detailed issue information for display

❌ **DO NOT throw exceptions** for:
- Parsing errors in individual rows (collect as issues)
- Compliance check violations (store as RosterIssue)

### When to Throw Exceptions

✅ **DO throw exceptions** for:
- Programming errors (null references, invalid state)
- Database errors (let EF Core throw, `GlobalExceptionHandler` will catch)
- Domain rule violations (throw `DomainException`)
- Infrastructure failures (network, file system)

---

## File References

### Core Result Pattern
- `src/FairWorkly.Domain/Common/Result.cs` - Result<T> implementation
- `src/FairWorkly.Domain/Common/IResultBase.cs` - Result interface
- `src/FairWorkly.Domain/Common/ValidationError.cs` - Base validation error
- `src/FairWorkly.Application/Common/Behaviors/ValidationBehavior.cs` - Auto validation

### Result-to-HTTP Mapping
- `src/FairWorkly.API/Filters/ResultMappingFilter.cs` - Centralized Result<T> → ActionResult filter
- `tests/FairWorkly.UnitTests/Unit/ResultMappingFilterTests.cs` - Filter unit tests

### Exception Handling
- `src/FairWorkly.API/ExceptionHandlers/GlobalExceptionHandler.cs` - Global exception catcher
- `src/FairWorkly.Domain/Exceptions/` - Custom domain exceptions

### Business Error Models
- `src/FairWorkly.Domain/Roster/Entities/RosterIssue.cs` - Compliance issue entity
- `agent-service/agents/roster/services/roster_import/models.py` - Parse issue models
- `src/FairWorkly.Application/Payroll/Services/CsvParserService.cs` - CSV parser

### Example Implementations
- `src/FairWorkly.Application/Auth/Features/Login/LoginCommandHandler.cs` - Auth example
- `src/FairWorkly.Application/Roster/Features/ValidateRoster/ValidateRosterHandler.cs` - Validation example
- `src/FairWorkly.API/Controllers/Auth/AuthController.cs` - Controller example

---

## Additional Documentation

For detailed guides on specific aspects:
- See `backend/docs/Result_Pattern_Guide.md` for Result<T> usage examples
- See `backend/docs/Backend_Development_Guide.md` for handler development patterns
- See `backend/CLAUDE.md` for backend architecture overview
