# Result<T> Pattern Guide

## What is Result<T>?

An "envelope" class that wraps operation results. All Handler return values are wrapped with it.

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }                    // Whether successful
    public bool IsFailure => !IsSuccess;              // Whether failed
    public T? Value { get; }                          // Data on success
    public string? ErrorMessage { get; }              // Error message on failure
    public List<ValidationError>? ValidationErrors { get; }  // Validation error list
    public ResultType Type { get; }                   // Result type
}
```

**What is the generic `<T>`?** The data type returned on success. e.g., `Result<OrderDto>` means on success, `Value` is an `OrderDto`.

---

## Factory Methods Overview

You cannot directly `new Result()`—must use static methods to create:

| Method | Purpose | Example |
|--------|---------|---------|
| `Success(value)` | Operation succeeded | `Result<OrderDto>.Success(dto)` |
| `ValidationFailure(errors)` | Input validation failed | `Result<OrderDto>.ValidationFailure(errorList)` |
| `Failure(message)` | Business logic failed | `Result<OrderDto>.Failure("Out of stock")` |
| `NotFound(message)` | Resource not found | `Result<OrderDto>.NotFound("Order not found")` |
| `Unauthorized(message)` | Not logged in | `Result<UserDto>.Unauthorized("Please log in first")` |
| `Forbidden(message)` | No permission | `Result<OrderDto>.Forbidden("No access to this operation")` |

---

## Property Values for Each Scenario

| Scenario | IsSuccess | Value | ErrorMessage | ValidationErrors | Type |
|----------|-----------|-------|--------------|------------------|------|
| Success | `true` | DTO data | `null` | `null` | `Success` |
| Validation Failed | `false` | `null` | `"Validation failed"` | Error list | `ValidationFailure` |
| Business Failed | `false` | `null` | Custom message | `null` | `BusinessFailure` |
| Resource Not Found | `false` | `null` | Custom message | `null` | `NotFound` |
| Not Logged In | `false` | `null` | Custom message | `null` | `Unauthorized` |
| No Permission | `false` | `null` | Custom message | `null` | `Forbidden` |

---

## How to Write Handlers

```csharp
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderQuery query, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(query.OrderId);

        // Failure cases - return corresponding Result
        if (order == null)
            return Result<OrderDto>.NotFound($"Order {query.OrderId} not found");

        if (order.UserId != _currentUser.Id)
            return Result<OrderDto>.Forbidden("You don't have access to view this order");

        // Success case - wrap DTO and return
        var dto = new OrderDto { Id = order.Id, ... };
        return Result<OrderDto>.Success(dto);
    }
}
```

**Note:** Even when returning failure, the generic must be `Result<OrderDto>` because the method signature requires consistent return type.

---

## How to Write Controllers

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetOrder(int id)
{
    var result = await _mediator.Send(new GetOrderQuery { OrderId = id });

    // Decide what to return based on Type
    if (result.Type == ResultType.ValidationFailure)
        return BadRequest(new
        {
            code = "VALIDATION_ERROR",
            errors = result.ValidationErrors  // Get validation error list
        });

    if (result.Type == ResultType.NotFound)
        return NotFound(new
        {
            code = "NOT_FOUND",
            message = result.ErrorMessage  // Get error message
        });

    if (result.Type == ResultType.Forbidden)
        return StatusCode(403, new
        {
            code = "FORBIDDEN",
            message = result.ErrorMessage
        });

    // Success
    return Ok(result.Value);  // Extract DTO data
}
```

---

## Complex Errors: Custom Error Classes

Simple errors use `ValidationError` (only `Field` and `Message`). But business errors may be more complex—e.g., CSV parsing errors need to return row number, employee info, etc.

**Solution: Inherit from `ValidationError` base class**

```csharp
// Domain/Payroll/CsvParsingError.cs
public class CsvParsingError : ValidationError
{
    public int RowNumber { get; init; }        // Which row
    public string EmployeeId { get; init; }    // Employee ID, "unknown" if unrecognized
    public string EmployeeName { get; init; }  // Employee name, "unknown" if unrecognized
}
```

**Handler usage:**

```csharp
var errors = new List<ValidationError>
{
    new CsvParsingError
    {
        RowNumber = 5,
        Field = "hourlyRate",
        Message = "Hourly rate must be a number",
        EmployeeId = "EMP001",
        EmployeeName = "John Smith"
    },
    new CsvParsingError
    {
        RowNumber = 12,
        Field = "workDate",
        Message = "Invalid date format",
        EmployeeId = "unknown",
        EmployeeName = "unknown"
    }
};

return Result<PayrollDto>.ValidationFailure("CSV parsing failed, please correct and re-upload", errors);
```

**Controller:**

```csharp
if (result.Type == ResultType.ValidationFailure)
    return BadRequest(new
    {
        code = "CSV_VALIDATION_ERROR",
        message = result.ErrorMessage,
        errors = result.ValidationErrors
    });
```

**Frontend receives 400:**

```json
{
  "code": "CSV_VALIDATION_ERROR",
  "message": "CSV parsing failed, please correct and re-upload",
  "errors": [
    {
      "rowNumber": 5,
      "field": "hourlyRate",
      "message": "Hourly rate must be a number",
      "employeeId": "EMP001",
      "employeeName": "John Smith"
    },
    {
      "rowNumber": 12,
      "field": "workDate",
      "message": "Invalid date format",
      "employeeId": "unknown",
      "employeeName": "unknown"
    }
  ]
}
```

**Key points:**
- Inherit `ValidationError`, add fields needed by business
- Place in corresponding Domain module (e.g., `Domain/Payroll/`)
- `List<ValidationError>` can hold subclasses—polymorphic serialization preserves subclass fields

---

## ValidationBehavior's Role

You don't need to manually handle validation failures—`ValidationBehavior` auto-intercepts:

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
Controller handles
```

---

## Complete Flow Example

**Request:** `GET /api/orders/999`

```
1. Controller calls _mediator.Send(new GetOrderQuery { OrderId = 999 })
2. ValidationBehavior validates OrderId > 0 ✓ Passed
3. Handler executes, queries database, order == null
4. Handler returns Result<OrderDto>.NotFound("Order 999 not found")
5. Controller receives result:
   - result.Type == ResultType.NotFound
   - result.ErrorMessage == "Order 999 not found"
   - result.Value == null
6. Controller returns NotFound(new { message = "Order 999 not found" })
7. Client receives 404 + JSON
```

---

## File Locations

- `src/FairWorkly.Domain/Common/Result.cs` - Result<T> class
- `src/FairWorkly.Domain/Common/IResultBase.cs` - Interface
- `src/FairWorkly.Domain/Common/ValidationError.cs` - Validation error class
- `src/FairWorkly.Application/Common/Behaviors/ValidationBehavior.cs` - Auto validation
