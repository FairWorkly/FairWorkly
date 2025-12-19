# Development Workflow & PR Rules

This document defines **how we work**, **how we review code**, and **how decisions flow** in the FairWorkly project.

Its goal is not to slow development down, but to **prevent bottlenecks and architectural damage**.

---

## 1. Why These Rules Exist

FairWorkly is an early-stage product with:

- Multiple agents (Compliance, Payroll, FairBot)
- A small team with mixed experience levels
- Limited review capacity

Without clear workflow rules:

- Pull requests pile up waiting for review
- Core contributors become bottlenecks
- Architecture degrades quickly

**These rules exist to protect development speed and system integrity.**

---

## 2. Ownership Model (Very Important)

### Module Ownership

Each major area of the system has an **owner**.

Owners are responsible for:

- Reviewing changes in their module
- Protecting architectural boundaries
- Deciding when escalation is required

> **Not every PR needs the Tech Lead.**

---

### Tech Lead Review Is Required Only For:

- Cross-module changes
- Architecture or layering changes
- Security / authentication / authorization
- Database schema or migration changes
- Changes affecting multiple agents
- Breaking API contracts

If your PR does **not** fall into these categories,  
**do not wait for Tech Lead review**.

---

## 3. PR Size & Scope Rules

### One PR = One Purpose

A good PR:

- Solves one problem
- Touches one module
- Can be reviewed in < 15 minutes

Avoid:

- “While I was here…” changes
- Mixing refactors with features
- Multi-module changes without discussion

If a PR is hard to explain in one sentence, it is too large.

---

## 4. Frontend-Specific Rules

### Where Logic Lives

- Pages and components **must not** call APIs directly
- All backend communication goes through `services/`
- Business logic belongs in module hooks, not UI

Forbidden:

- `fetch()` or `axios` in pages/components
- Calling backend from `shared/`
- Business decisions in route config

---

## 5. Backend-Specific Rules

### Layer Responsibilities

- **API layer**: request handling, auth, dispatch only
- **Application layer**: workflows (CQRS handlers)
- **Domain layer**: business rules (single source of truth)
- **Infrastructure layer**: persistence & external systems

Forbidden:

- Business rules in controllers
- Database queries in domain logic
- Compliance or payroll logic in repositories

If you are unsure which layer something belongs to, **stop and ask**.

---

## 6. Safe Contribution Guidelines (For New Contributors)

Not everyone needs to work on core logic.

### Safe Contribution Areas

- Frontend UI components
- Storybook stories
- Types / DTOs
- Tests (unit or integration)
- Documentation updates

These contributions:

- Do not require deep domain knowledge
- Are easier to review
- Improve overall development speed

---

## 7. When to Ask Before Coding

Ask before writing code if:

- You are unsure where logic should live
- The change affects more than one module
- You need to touch domain rules
- You are changing an existing workflow
- You are introducing a new dependency

Asking early is always cheaper than fixing later.

---

## 8. PR Rejection Rules (No Debate)

A PR may be closed or rejected if it:

- Violates architectural boundaries
- Introduces business logic in the wrong layer
- Is too large or unfocused
- Bypasses existing patterns without justification
- Breaks MVP scope assumptions

Rejection is not personal. It is a system-protection mechanism.

---

## 9. What This Document Is NOT

- This is not a tutorial
- This is not a complete architecture guide
- This is not a list of coding standards

It defines **how we collaborate**, not how to write every line of code.

---

## 10. Final Principle

> **Speed comes from clarity, not from skipping rules.**

If everyone follows the same workflow, FairWorkly can move fast **without breaking trust or architecture**.
