# Result\<T\> Usage Guide

## 1. What Our JSON Responses Look Like

All API responses revolve around three fields: `code`, `msg`, `data`.

**Success with data** (200, 201):

```json
{ "code": 200, "msg": "Employee retrieved", "data": { ... DTO fields ... } }
```

**Error with errors list** (400, 422 with errors):

```json
{ "code": 422, "msg": "CSV format validation failed", "data": { "errors": [...] } }
```

**Error with msg only** (401, 403, 404, 409, 422 simple rejection):

```json
{ "code": 404, "msg": "Roster not found" }
```

**No body** (204):

Nothing is returned.

That's all the formats. The sections below explain how to produce each type of response.

---

## 2. Success Responses: How to Return Data

### 200 — Query/Operation Succeeded

The most common case. Put your DTO in `data`.

```csharp
// In the Handler:
var dto = new EmployeeDto
{
    Id = 42,
    Name = "Alice Wang",
    Email = "alice@example.com"
};
return Result<EmployeeDto>.Of200("Employee retrieved", dto);
```

The frontend receives:

```json
{
  "code": 200,
  "msg": "Employee retrieved",
  "data": {
    "id": 42,
    "name": "Alice Wang",
    "email": "alice@example.com"
  }
}
```

The contents of `data` correspond directly to the fields of your DTO.

### 201 — Creation Succeeded

Used when creating a new resource. Same pattern, different method name.

```csharp
var dto = new EmployeeDto { Id = employee.Id, ... };
return Result<EmployeeDto>.Of201("Employee created", dto);
```

### 204 — Done, No Need to Return Anything

For operations like delete or logout where the frontend doesn't need any data — just knowing "it worked" is enough.

```csharp
return Result<Unit>.Of204();
```

Note the use of `Result<Unit>` instead of `Result<SomeDto>`. `Unit` is built into MediatR and semantically means "no return value".

Your Handler signature must match accordingly:

```csharp
public class LogoutCommandHandler(...)
    : IRequestHandler<LogoutCommand, Result<Unit>>
```

> **Do not use `Result<bool>` or `Result<string>` as substitutes.** They imply meaningful data on success, but there is none. For "no return value", always use `Result<Unit>`.

---

## 3. Error Responses: When Only msg Is Needed

For 401, 403, 404, and 409 errors, the frontend only needs a `msg` to display a prompt. No `data`.

Each has a default message, but you can also provide your own:

```csharp
// 401 Unauthorized
return Result<UserDto>.Of401();                              // msg: "Unauthorized"
return Result<LoginResponse>.Of401("Invalid email or password.");  // msg: custom

// 403 Forbidden
return Result<ValidatePayrollDto>.Of403();                   // msg: "Forbidden"
return Result<ValidatePayrollDto>.Of403("User does not belong to an organization");

// 404 Not Found
return Result<RosterDetailsResponse>.Of404();                // msg: "Not found"
return Result<RosterDetailsResponse>.Of404("Roster not found");

// 409 Conflict
return Result<EmployeeDto>.Of409();                          // msg: "Conflict"
return Result<EmployeeDto>.Of409("Employee number E999 already exists");
```

The frontend receives something like this (404 example):

```json
{ "code": 404, "msg": "Roster not found" }
```

Note: Even on failure paths, the generic type must match your Handler's return type (e.g., `Result<OrderDto>`) — you cannot change it.

---

## 4. Error Responses: With an Errors List

Two scenarios include an `errors` array in `data`: **400 (input validation)** and **422 (business processing errors)**.

### 422 — Errors Found During Business Processing

For example, when parsing a CSV and finding issues in certain rows, you want to report each row's error to the frontend.

**Simple rejection (no errors list needed):**

```csharp
return Result<UploadRosterResponse>.Of422("Could not determine week dates from roster");
```

```json
{ "code": 422, "msg": "Could not determine week dates from roster" }
```

**With structured errors:**

```csharp
var errors = new List<Csv422Error>
{
    new() { RowNumber = 5, Field = "Hourly Rate", Message = "Hourly Rate must be a positive number" },
    new() { RowNumber = 8, Field = "Employment Type", Message = "Must be one of: full-time, part-time, casual" },
};
return Result<ValidatePayrollDto>.Of422("CSV format validation failed", errors);
```

```json
{
  "code": 422,
  "msg": "CSV format validation failed",
  "data": {
    "errors": [
      { "rowNumber": 5, "field": "Hourly Rate", "message": "Hourly Rate must be a positive number" },
      { "rowNumber": 8, "field": "Employment Type", "message": "Must be one of: full-time, part-time, casual" }
    ]
  }
}
```

What each entry in `errors` looks like depends on the Error class you define (covered in Section 7).

### 400 — Input Validation Failed (You Don't Write This — It's Auto-Generated)

This one is special: **you don't need to write any 400-related code in the Handler.** You just write a Validator. When validation fails, the framework automatically intercepts the request and returns 400 — the Handler never executes at all.

```csharp
public class ValidatePayrollValidator : AbstractValidator<ValidatePayrollCommand>
{
    public ValidatePayrollValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File is required")      // -> message in errors
            .OverridePropertyName("file");         // -> field in errors

        RuleFor(x => x.AwardType)
            .NotEmpty()
            .WithMessage("Award type is required")
            .OverridePropertyName("awardType");
    }
}
```

Two key methods:

- `.WithMessage("...")` — the `message` in each entry of the `errors` array
- `.OverridePropertyName("...")` — the `field` in each entry of the `errors` array

The frontend receives:

```json
{
  "code": 400,
  "msg": "Request validation failed",
  "data": {
    "errors": [
      { "field": "file", "message": "File is required" },
      { "field": "awardType", "message": "Award type is required" }
    ]
  }
}
```

> **Summary**: Except for 400 which is handled automatically by the framework, all other Results (200, 201, 204, 401, 403, 404, 409, 422) are written by you in the Handler.

---

## 5. Controller: One Line Does It All

Controllers inherit `BaseApiController` and call `RespondResult` — that's it. **Controllers don't handle errors** — whether the Handler returns 200, 404, or 422, `BaseApiController` has already handled the JSON formatting for every case. Controllers don't manage messages; messages are set exclusively in the Handler's `Of{code}` methods.

```csharp
[Route("api/[controller]")]
public class PayrollController : BaseApiController
{
    private readonly IMediator _mediator;

    public PayrollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("validation")]
    [Authorize(Policy = "RequireManager")]
    public async Task<IActionResult> Validate(IFormFile file, [FromForm] string awardType, ...)
    {
        var command = new ValidatePayrollCommand { ... };
        var result = await _mediator.Send(command);
        return RespondResult(result);
    }
}
```

The `msg` comes from the message written in `Of200` inside the Handler:

```csharp
// In the Handler:
return Result<ValidatePayrollDto>.Of200("Payroll validation completed", dto);

// In the Controller:
return RespondResult(result);
```

```json
{ "code": 200, "msg": "Payroll validation completed", "data": { ... } }
```

---

## 6. What a Complete Handler Looks Like

Putting everything above together, here's a typical Handler:

```csharp
public class GetOrderHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(
        GetOrderQuery query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(query.OrderId, cancellationToken);

        if (order == null)
            return Result<OrderDto>.Of404("Order not found");           // -> { code: 404, msg: "..." }

        if (order.OrganizationId != currentUser.OrganizationId)
            return Result<OrderDto>.Of403("Not authorized to view this order");  // -> { code: 403, msg: "..." }

        var dto = new OrderDto { Id = order.Id, ... };
        return Result<OrderDto>.Of200("Order retrieved", dto);          // -> { code: 200, msg: "...", data: { ... } }
    }
}
```

---

## 7. Custom Error Classes

When a 422 needs to return structured errors, you create an Error class to define the fields in each entry of the `errors` array.

**Naming convention**: `<Context><Code>Error`

```csharp
// Domain/Payroll/Errors/Csv422Error.cs
public class Csv422Error
{
    public int RowNumber { get; init; }
    public required string Field { get; init; }
    public required string Message { get; init; }
}
```

Rules:

- Each Error class is standalone — no base class inheritance
- Fields are determined by the API contract (define whatever the frontend needs)
- Need a new code that isn't among the existing 9? Talk to Jason first — don't modify the Result class yourself

---

## Quick Reference

| Scenario | What to Write in Handler | JSON the Frontend Receives |
|----------|-------------------------|---------------------------|
| Query/operation succeeded | `Of200(msg, dto)` | `{ code, msg, data: { DTO } }` |
| Creation succeeded | `Of201(msg, dto)` | `{ code, msg, data: { DTO } }` |
| Done, no return needed | `Of204()` | No body |
| Unauthorized | `Of401()` or `Of401(msg)` | `{ code, msg }` |
| Forbidden | `Of403()` or `Of403(msg)` | `{ code, msg }` |
| Not found | `Of404()` or `Of404(msg)` | `{ code, msg }` |
| Conflict | `Of409()` or `Of409(msg)` | `{ code, msg }` |
| Processing error (simple) | `Of422(msg)` | `{ code, msg }` |
| Processing error (with list) | `Of422(msg, errors)` | `{ code, msg, data: { errors } }` |
| Input validation failed | No code needed — Validator handles it | `{ code: 400, msg, data: { errors } }` |

---

## Source Code Paths

To dive deeper into the internals, go straight to the source code (every method and property has detailed XML comments):

| File | Description |
|------|-------------|
| `Domain/Common/Result/Result.cs` | Result\<T\> main class, all Of{code} factory methods |
| `Domain/Common/Result/IResultBase.cs` | Marker interface for ValidationBehavior generic constraint |
| `Domain/Common/Result/Validation400Error.cs` | 400 error structure (used internally by ValidationBehavior) |
| `Application/Common/Behaviors/ValidationBehavior.cs` | FluentValidation auto-interception pipeline |
| `API/Controllers/BaseApiController.cs` | RespondResult unified response mapping |
