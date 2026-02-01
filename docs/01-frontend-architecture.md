# Frontend Architecture Cheat Sheet

This document explains **how the FairWorkly frontend is structured**, and **where different types of code should live**.

Its goal is to help contributors write code **in the right place**, without breaking architectural boundaries or slowing reviews.

---

## 1. One-Sentence Mental Model

**`app/` boots the application, `modules/` deliver business features, `shared/` provides reusable building blocks, and `services/` talks to the backend.**

If you remember only one thing, remember this.

---

## 2. High-Level Folder Structure

```txt
src/
├── app/
├── modules/
├── shared/
├── services/
├── store/
├── slices/
├── styles/
└── main.tsx
```

Each folder exists for a specific reason. If something does not clearly belong in one of them, stop and ask.

## 3. app/ — Application Shell (Rarely Changes)

What app/ Is For:

app/ is the application shell. It wires the system together but contains no business logic.

Typical contents:

- Global providers (Theme, Redux, Query Client)
- Route configuration
- Route guards (e.g. authentication)

What Must NOT Go in app/

- Pages or screens
- API calls
- Business logic
- Feature-specific state

Rule: If something changes frequently, it probably does not belong in app/.

## 4. modules/ — Business Modules (Where Most Work Happens)

What a Module Represents

Each folder under modules/ represents a business domain, not a technical layer.

Examples:

- modules/roster
- nmodules/payroll
- modules/fairbot
- modules/auth
- modules/home

A module owns everything needed to deliver its feature.

Typical Module Structure
modules/<module>/
├── pages/
├── components/
├── hooks/
├── types/
└── index.ts

Not all folders are required, but the separation is intentional.

pages/

- Route-level screens
- Compose components and hooks
- No direct API calls

Pages should remain thin.

components/

- UI components specific to the module
- May use module hooks
- Should not be reused across unrelated modules

If a component becomes reusable across modules, move it to shared/.

hooks/

- Module-level logic
- TanStack Query hooks
- State and orchestration logic

This is where most frontend logic should live.

types/

- Types specific to the module
- DTOs, view models, helper types

Do not put API or backend-specific types here unless scoped to the module.

## 5. shared/ — Reusable Building Blocks (No Business Knowledge)

Purpose

shared/ contains generic, reusable code that has no knowledge of business domains.

Typical contents:

- UI components (buttons, cards, tables)
- Layout components
- Generic hooks
- Utilities and helpers

Critical Rule

shared/ must NOT import from modules/.

If this rule is broken, shared code becomes business-coupled and reuse collapses.

## 6. services/ — Backend Communication Layer

Purpose

services/ is the only place that knows about backend APIs.

Typical contents:

- httpClient.ts (low-level HTTP setup)
- baseApi.ts (typed wrappers, error handling)
- Domain APIs (complianceApi.ts, payrollApi.ts, etc.)

What Must NOT Happen

- Pages or components calling fetch, axios, or httpClient
- Business logic inside services
- UI concerns inside services

Rule: UI never talks to the backend directly.

## 7. Global State (store/ and slices/)

Global state should be used sparingly.

Good candidates:

- Authentication session
- Global UI state (notifications, modals)

If state belongs to a single module, keep it inside the module.

## 8. Data Flow (Canonical Pattern)

Page
→ module hook (TanStack Query)
→ services API
→ backend

If your code skips a step, it is probably wrong.

## 9. Pre-Login vs Post-Login Pages

- Pre-login pages belong to modules/home or modules/auth
- Post-login product pages belong to business modules

Routing and guards live in app/routes, but pages remain in their modules.

## 10. Common Mistakes to Avoid

- Putting pages in app/
- Calling APIs from components or pages
- Importing modules/ from shared/
- Using global store for module-only state
- Adding logic to route definitions

These mistakes will cause PRs to be rejected.

## 11. When to Ask Before Coding

Ask before writing code if:

- You are unsure which folder something belongs to
- The change affects more than one module
- You want to introduce a new global dependency

Asking early prevents rework.

12. Final Rule

If you are unsure where something belongs, do not guess.
Ask, or refer back to this document.
