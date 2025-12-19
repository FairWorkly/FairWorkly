# Backend Architecture & Rules

This document explains **how the FairWorkly backend is structured**,  
**why these boundaries exist**, and **what is explicitly not allowed**.

Its purpose is to protect:

- correctness of decisions,
- auditability,
- and long-term development speed.

---

## 1. One-Sentence Mental Model

**The backend is the single source of truth for decisions.**  
It decides, validates, and records outcomes.  
The frontend asks; the backend decides.

---

## 2. High-Level Architecture

FairWorkly backend uses a **four-layer architecture**:

```txt
API → Application → Domain → Infrastructure
```

For Compliance and Payroll agents, we also use:

- CQRS
- MediatR

These are not abstractions for their own sake—they exist to protect rules and auditability.

## 3. Layer Responsibilities (Non-Negotiable)

3.1 API Layer (FairWorkly.API)

Purpose: Request handling and dispatch only.

Allowed:

- Controllers
- Authentication & authorization
- Input validation
- Mapping request DTOs
- Dispatching Commands / Queries via MediatR

Forbidden:

- Business rules
- Compliance or payroll logic
- Database queries
- Award interpretation

Rule: Controllers must be thin. If a controller contains business if/else, it is wrong.

3.2 Application Layer (FairWorkly.Application)

Purpose: Orchestrate workflows.

This is where:

- Commands and Queries live
- Handlers coordinate use cases
- Domain logic is invoked
- Persistence is triggered

Allowed:

- CQRS handlers
- Calling domain services/entities
- Coordinating repositories
- Producing results and responses

Forbidden:

- Encoding business rules directly
- Heavy calculations that belong in Domain
- Infrastructure-specific logic

Rule: Application defines how things happen, not what is correct.

3.3 Domain Layer (FairWorkly.Domain)

Purpose: Business truth.

The Domain layer contains:

- Entities
- Value Objects
- Domain services
- Compliance and payroll rules
- Invariants and validations

Allowed:

- Pure business logic
- Rule evaluation
- Award interpretation
- Throwing domain exceptions

Forbidden:

- Database access
- EF Core references
- HTTP concepts
- Application workflows

Rule: If it decides whether something is compliant, correct, or valid,
it belongs in Domain.

3.4 Infrastructure Layer (FairWorkly.Infrastructure)

Purpose: Technical implementation details.

Contains:

- EF Core persistence
- Repositories
- External service integrations
- Identity providers
- AI providers

Allowed:

- Database queries
- Mapping persistence models
- External API calls

Forbidden:

- Business decisions
- Compliance rules
- Payroll rules

Rule: Infrastructure answers how, never whether.

4. CQRS in Compliance & Payroll Agents

Why CQRS Exists Here

Compliance and Payroll decisions:

- must be auditable,
- must be explainable,
- must be reproducible.

CQRS helps by separating:

- Commands (actions that may change state)
- Queries (read-only access)

Commands

Examples:

- CheckRosterComplianceCommand
- ValidatePayComplianceCommand

Commands:

- may produce audit records,
- may persist outcomes,
- represent business intent.

Queries

Examples:

- GetComplianceIssuesQuery
- GetPayComplianceSummaryQuery

Queries:

- do not change state,
- return DTOs,
- are safe to call repeatedly.

5. Payroll Agent Scope (Important)

FairWorkly does NOT calculate payroll.

The Payroll Agent:

- receives already-calculated pay results,
- validates them against Award rules,
- produces compliance outcomes and explanations.

Out of scope:

- payroll calculation
- pay runs
- payslip generation

Any code attempting to calculate pay amounts is out of scope for this system.

6. Supporting Domains (Documents)

Documents is a supporting domain responsible for generating standard HR documents using predefined templates.

### Responsibilities

- Generate PDF documents from code-defined templates
- Assemble document data from multiple sources:
  - Employee module data
  - organisation-level configuration
  - user-provided inputs at generation time
- Store document metadata and generated outputs

### Out of Scope

Documents does NOT:

- calculate payroll
- interpret Award rules
- validate compliance
- make business decisions

### Architectural Rule

Documents is an output and record-keeping module, not a decision engine.

Any compliance or legality validation must occur in Compliance or Payroll Agents.

7. Auditability Is a First-Class Concern

Every decision produced by an agent should be:

- traceable,
- reproducible,
- explainable.

This affects:

- Domain modeling
- Command design
- Persistence decisions

If a decision cannot be explained after the fact, the implementation is incomplete.

8. Safe Areas for New Contributors

New or learning contributors should start with:

- Queries (read-only)
- DTO mapping
- Unit tests for Domain rules
- Integration tests for handlers

These areas reduce risk while improving coverage and confidence.

9. When to Ask Before Coding

Ask before implementing if:

- You are unsure which layer something belongs to
- You need to modify Domain rules
- You want to bypass CQRS patterns
- The change affects audit data
- The change impacts multiple agents

Asking early prevents architectural damage.

10. PR Rejection Rules (Backend)

A PR may be rejected if it:

- Adds business logic to controllers
- Implements rules in Application instead of Domain
- Mixes Infrastructure concerns into Domain
- Bypasses CQRS without justification
- Violates Payroll Agent scope

Rejection is a protection mechanism, not a judgement.

11. Final Principle

Correctness and auditability come before convenience.

If the system cannot explain its decisions, it has failed its purpose.
