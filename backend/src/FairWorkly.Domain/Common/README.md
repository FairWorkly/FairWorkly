# FairWorkly Domain Common Layer

## 📋 Overview

This directory contains shared base classes and enums used across all domain modules.

**Design Principle:** Keep it simple. Only include what MVP actually needs.

---

## 🎯 BaseEntity

### Purpose

Provides common fields that every entity needs:

- **Id**: Primary key (Guid)
- **CreatedAt**: When the entity was created
- **IsDeleted**: Soft delete support

### Fields Explanation

#### ✅ Id (Guid)

- **Type**: `Guid`
- **Purpose**: Unique identifier for each entity
- **Set when**: Entity is created
- **Used for**: Primary key, relationships, API responses

**Why Guid instead of int?**

- ✅ Globally unique (no collisions across distributed systems)
- ✅ Can be generated client-side
- ✅ Better for multi-tenant systems
- ✅ Harder to enumerate (security)

#### ✅ CreatedAt (DateTime)

- **Type**: `DateTime`
- **Purpose**: Record when entity was created
- **Set when**: Entity is first saved to database
- **Used for**:
  - Sorting (e.g., "Show recent employees")
  - Filtering (e.g., "Employees added this month")
  - Audit trail
  - Display in UI ("Employee since: 2024-12-01")

**Best Practice:**

```csharp
// Set in DbContext.SaveChanges override
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
{
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = DateTime.UtcNow;
        }
    }
    return await base.SaveChangesAsync(cancellationToken);
}
```

#### ✅ IsDeleted (bool)

- **Type**: `bool`
- **Purpose**: Soft delete support
- **Default**: `false`
- **Used for**: Mark entities as deleted without actually removing from database

**Why soft delete?**

- ✅ Preserve data integrity (keep historical records)
- ✅ Enable "undo" functionality
- ✅ Maintain foreign key relationships
- ✅ Audit requirements (who was paid what, even if employee left)

**Example:**

```csharp
// Instead of:
dbContext.Employees.Remove(employee);  // ❌ Hard delete

// Do this:
employee.IsDeleted = true;  // ✅ Soft delete
await dbContext.SaveChangesAsync();

// Query non-deleted entities:
var activeEmployees = await dbContext.Employees
    .Where(e => !e.IsDeleted)
    .ToListAsync();
```

---

## ✅ AuditableEntity

### Purpose

Extends BaseEntity with audit tracking for user actions.

**Use this when:** Entities are created or modified by users (Owner/Manager)  
**Don't use when:** Entities are system-generated (AI/automated)

### Fields

#### CreatedByUserId (Guid)

- **Required**: Yes
- **Purpose**: Track which user created this entity
- **Set when**: Entity is first saved
- **Used for**:
  - Display "Added by [Manager Name]" in UI
  - Audit trail
  - Permission checks (Manager can only edit their own employees)

#### UpdatedByUserId (Guid?)

- **Required**: No (null if never updated)
- **Purpose**: Track which user last modified this entity
- **Set when**: Entity is modified
- **Used for**:
  - Display "Last modified by [Manager Name]"
  - Audit trail
  - Compliance reporting

#### UpdatedAt (DateTime?)

- **Required**: No (null if never updated)
- **Purpose**: When the entity was last modified
- **Set when**: Entity is modified
- **Used for**:
  - Display "Last modified on [Date]"
  - Sorting by recent changes
  - Detecting stale data

### Inheritance Decision

```csharp
// ✅ Inherit from AuditableEntity
public class Employee : AuditableEntity { }      // User creates employees
public class Payslip : AuditableEntity { }       // User uploads payslips
public class Roster : AuditableEntity { }        // User creates rosters
public class Document : AuditableEntity { }      // User generates documents

// ❌ Inherit from BaseEntity only
public class PayrollIssue : BaseEntity { }       // AI generates issues
public class RosterIssue : BaseEntity { }        // AI generates issues
public class User : BaseEntity { }               // Users self-register
public class Organization : BaseEntity { }       // Created during registration
```

### Usage Example

```csharp
// Creating an employee (CreatedByUserId set automatically)
var employee = new Employee
{
    FirstName = "Alice",
    LastName = "Johnson",
    // CreatedByUserId will be set by DbContext.SaveChanges
    // from current user's JWT token
};

await _dbContext.Employees.AddAsync(employee);
await _dbContext.SaveChangesAsync();
// → employee.CreatedByUserId = current user ID
// → employee.CreatedAt = DateTime.UtcNow

// Updating an employee (UpdatedByUserId set automatically)
employee.FirstName = "Alicia";
await _dbContext.SaveChangesAsync();
// → employee.UpdatedByUserId = current user ID
// → employee.UpdatedAt = DateTime.UtcNow

// Querying with creator info
var employees = await _dbContext.Employees
    .Include(e => e.CreatedByUser)
    .Include(e => e.UpdatedByUser)
    .Where(e => !e.IsDeleted)
    .ToListAsync();

// Display in UI
foreach (var emp in employees)
{
    Console.WriteLine($"Added by: {emp.CreatedByUser?.FullName}");
    Console.WriteLine($"Added on: {emp.CreatedAt}");

    if (emp.UpdatedAt.HasValue)
    {
        Console.WriteLine($"Modified by: {emp.UpdatedByUser?.FullName}");
        Console.WriteLine($"Modified on: {emp.UpdatedAt}");
    }
}
```

### Why UpdatedAt is NOT in BaseEntity?

**Question:** Why don't all entities have UpdatedAt?

**Answer:**
Not all entities need UpdatedAt because:

1. **System-generated entities never get "updated"**

   - PayrollIssue: Generated by AI, then immutable
   - RosterIssue: Generated by AI, then immutable

2. **Some entities rarely change**

   - User: Only updated when changing profile
   - Organization: Rarely changes settings

3. **Performance consideration**

   - Fewer columns = smaller tables
   - Only add fields when actually needed

4. **YAGNI principle**
   - "You Aren't Gonna Need It"
   - Add UpdatedAt only to entities that are frequently edited

**Entities that DO need UpdatedAt:**

- Employee (frequently edited by Owner/Manager)
- Payslip (might be corrected)
- Roster (frequently modified)

→ These inherit from AuditableEntity

````
---

## ❌ What We Intentionally Removed

### UpdatedAt Field

**Removed from BaseEntity**

```csharp
// ❌ NOT included in MVP
public DateTime? UpdatedAt { get; set; }
````

**Why removed?**

- ⚠️ Not displayed in any MVP UI
- ⚠️ No business logic depends on it
- ⚠️ Adds complexity for little benefit
- ✅ Can be added later if needed

**When to add back?**

- When we need to show "Last modified" in UI
- When implementing optimistic concurrency control
- When audit requirements demand it

---

### Domain Events (IDomainEvent, DomainEvent)

**Removed from Common layer**

```csharp
// ❌ NOT included in MVP
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

**Why removed?**

- ⚠️ MVP has simple, synchronous workflows
- ⚠️ No event-driven architecture needed yet
- ⚠️ No email notifications, background jobs, etc.
- ⚠️ Requires Event Bus infrastructure (MediatR, MassTransit)
- ✅ Significant complexity for uncertain benefit

**When to add back?**

- When implementing async workflows (e.g., email notifications)
- When business logic requires event-driven patterns
- When scaling requires message queues
- When multiple aggregates need to react to changes

**Example use case (Post-MVP):**

```csharp
// Employee created → Send welcome email
public class EmployeeCreatedEvent : DomainEvent
{
    public Guid EmployeeId { get; set; }
    public string Email { get; set; }
}

// Handler
public class SendWelcomeEmailHandler : INotificationHandler<EmployeeCreatedEvent>
{
    public async Task Handle(EmployeeCreatedEvent evt, CancellationToken ct)
    {
        await _emailService.SendWelcomeEmailAsync(evt.Email);
    }
}
```

---

## 📊 CommonEnums

- `EmploymentType`
- `AwardType`
- `AustralianState`
- `IssueSeverity`
- `ValidationStatus`
- `DocumentType`

---

## 🎯 When to Add New Fields to BaseEntity

### Decision Framework

Ask these questions:

#### 1. Will this field be used by 80%+ of entities?

```
✅ Yes → Consider adding to BaseEntity
❌ No → Add to specific entity instead
```

#### 2. Does MVP actually use this field?

```
✅ Yes, displayed in UI → Add it
✅ Yes, used in business logic → Add it
❌ "Might be useful later" → Don't add (YAGNI)
```

#### 3. Can we wait until Post-MVP?

```
✅ Yes → Wait (reduce scope, ship faster)
❌ No, blocks MVP features → Add it now
```

#### 4. Example: Should we add `Version` field for optimistic locking?

```csharp
// Proposed addition
public int Version { get; set; }  // For concurrency control
```

**Decision process:**

1. ❓ Will 80%+ of entities use it?
   → No, only entities with concurrent updates
2. ❓ Does MVP need it?
   → No, MVP is single-user, low concurrency
3. ❓ Can we wait?
   → Yes, add when scaling becomes an issue

**Result: Don't add to MVP** ❌

---

## 🔧 Best Practices

### 1. Always inherit from BaseEntity

```csharp
// ✅ Good
public class Employee : BaseEntity
{
    // Employee-specific fields
}

// ❌ Bad
public class Employee  // Missing base class
{
    public Guid Id { get; set; }  // Don't duplicate base fields
}
```

### 2. Set CreatedAt automatically

```csharp
// In DbContext
public override async Task<int> SaveChangesAsync(CancellationToken ct)
{
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = DateTime.UtcNow;
        }
    }
    return await base.SaveChangesAsync(ct);
}
```

### 3. Use soft delete, not hard delete

```csharp
// ✅ Good
employee.IsDeleted = true;
await dbContext.SaveChangesAsync();

// ❌ Bad
dbContext.Employees.Remove(employee);  // Loses historical data
```

### 4. Filter out deleted entities in queries

```csharp
// ✅ Good
var employees = await dbContext.Employees
    .Where(e => !e.IsDeleted)
    .ToListAsync();

// Or configure global query filter in DbContext:
modelBuilder.Entity<Employee>()
    .HasQueryFilter(e => !e.IsDeleted);
```

---

## 🔄 Change Log

### 2025-12-15 - Initial MVP Version

- ❌ Removed Domain Events - not needed in MVP
- ❌ Removed UpdatedAt from BaseEntity - not used in MVP

### Future Considerations (Post-MVP)

- [ ] Add UpdatedAt when "last modified" is needed in UI
- [ ] Add Domain Events when async workflows are needed
- [ ] Add optimistic locking (Version field) when scaling

---

## 💡 Questions?

If you're unsure whether to add a new field to BaseEntity:

1. Check the decision framework above
2. Ask: "Does MVP need this?"
3. When in doubt, don't add it (YAGNI)
4. Discuss with the team in your PR

---

**Remember: It's easier to add fields later than to remove them!**
