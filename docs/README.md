# 📚 FairWorkly Documentation Guide

This folder contains all technical and product documentation for FairWorkly.

If you are new to the project, **do NOT read files randomly**.  
Follow the reading order below.

---

## 👋 New to FairWorkly? Start Here

**Required reading for everyone (in order):**

1. 📘 [Project Overview – How FairWorkly Works](00-project-overview.md)  
   Understand what FairWorkly is, what it solves, and what it does NOT do.

2. 🧠 [Frontend Architecture Cheat Sheet](01-frontend-architecture.md)  
   Required for anyone touching frontend code.

3. 🏗️ [Backend Architecture & Rules](02-backend-architecture.md)  
   Required for anyone touching backend or APIs.

4. 🔄 [Development Workflow & PR Rules](03-dev-workflow-and-pr.md)  
   Required before opening your first PR.

👉 **If you have not read the above, do not start coding.**

---

## 🧩 Module & Agent Context

FairWorkly is built around **decision-making agents** and **supporting modules**.

- **Core Agents**

  - Compliance Agent
  - Payroll Agent

- **Supporting Modules**

  - Home/ Payroll upload/ Roster upload
  - Auth / Identity / Shared UI

> Rule of thumb:  
> **Agents make decisions. Supporting modules provide data or outputs.**

---

## 🚫 Scope Guardrails (Important)

Before implementing any feature, check whether it violates MVP scope.

Out of scope for MVP:

- Online document/template editing
- Payroll calculation
- Leave management workflows
- Performance or training management
- Electronic signing

If unsure, **do not implement first**.  
Ask or propose a document update.

---

## 🧠 How to Use These Docs

- These documents define **architectural boundaries**, not just suggestions.
- PRs that violate documented rules may be rejected.
- If something is unclear or missing, improve the docs before adding logic.

---

## ✍️ Updating Documentation

If you change architecture, scope, or module responsibilities:

- Update the relevant doc in this folder
- Reference the change in your PR

Documentation is part of the codebase.

---

> When in doubt, read the docs again.
