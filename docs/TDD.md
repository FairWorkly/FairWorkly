# FairWorkly – Technical Design Document (TDD)

## 1. System Architecture Overview

### High-Level Architecture

┌─────────────┐ HTTPS ┌──────────────┐
│ React │ ◄─────────────► │ .NET 8 API │
│ Frontend │ (JSON/REST) │ Backend │
└─────────────┘ └──────────────┘

┌─────────────┼─────────────┐
▼ ▼ ▼
┌───────────┐ ┌──────────┐ ┌─────────────┐
│PostgreSQL │ │Anthropic │ │Azure Blob │
│ Database │ │Claude API│ │Storage(PDF) │
└───────────┘ └──────────┘ └─────────────┘

### Technology Stack Justification

**Frontend: React 18 + TypeScript + MUI v5**

- ✅ React: Australian market standard, large talent pool
- ✅ TypeScript: Type safety critical for compliance data
- ✅ MUI: Enterprise-ready, accessibility built-in

**Backend: .NET 8 + EF Core**

- ✅ .NET: Dominant in Australian enterprise/fintech
- ✅ Strong typing: Critical for financial calculations
- ✅ Performance: Handles CSV parsing efficiently

**Database: PostgreSQL 15**

- ✅ JSONB support: Flexible for AI response storage
- ✅ ACID compliance: Critical for payroll data
- ✅ Free & open-source: Cost-effective for startup

**AI: Anthropic Claude**

- ✅ Strong reasoning: Better for legal/compliance logic
- ✅ Australian data residency options available
- ✅ Enterprise support

## 2. Frontend Architecture Decision
