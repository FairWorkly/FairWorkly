# FairWorkly – HR Compliance & Payroll Safety Copilot (MVP)

**Problem Statement**
**Current Pain Points:**

- Australian SMEs (20-150 employees) struggle with Fair Work compliance
- HR managers spend 15+ hours/week on manual compliance checks
- 60% of SMEs have experienced underpayment issues
- Document generation is error-prone and time-consuming

## 1. Overview

FairWorkly is a 12-week MVP that acts as a compliance copilot for small–medium Australian businesses.  
The system provides 4 key agents:

1. **Compliance Agent**  
   – Award interpretation, NES checks, roster risk scanning
2. **Document & Contract Agent**  
   – Employment contracts, letters, template governance
3. **Payroll & STP Check Agent**  
   – Pay run validation, underpayment flags, SG checks
4. **Employee Help Agent**  
   – Employee self-service Q&A, leave balance queries, access to documents

The product aims to reduce Fair Work non-compliance risk, support onboarding, and lower HR workload.

---

## 2. Target Persona

### Small Business Owner

Needs simplified answers, high-level risks, clear action guidance.

### HR Manager

Needs accurate documents, compliance warnings, STP correctness.

### Payroll Officer

Needs a “second line of defence” for pay runs.

### Employee

Needs self-service access to shifts, leave, and employment info.

---

## 3. MVP Scope (12 Weeks)

### 3.1 Compliance Agent (Core)

- Natural-language Q&A (“Can I make my casual work on Sunday?”)
- NES & Award summary references (non-verbatim)
- Risk level: Green / Yellow / Red
- Roster upload (CSV only)
- Checks:
  - Maximum weekly hours
  - Shift length limits
  - Breaks between shifts
  - Weekend / PH shift flags
- Output:
  - Issue list
  - Risk summary
  - Recommended adjustments
  - Cost difference estimate (simple)

### 3.2 Document & Contract Agent

- Generate:
  - Offer letter
  - Employment contract (FT / PT / Casual / Fixed-term)
  - Warning letters (first/final)
  - Show cause
  - Termination
  - Change of hours
- Form-based generation → AI draft → PDF export
- Minimum NES validation:
  - Notice period
  - SG rate
  - Leave entitlements references
- Template upload (DOCX only)
- Template diff summary:
  - Missing NES clause reference
  - High-risk clauses
  - Unusual probation terms

### 3.3 Payroll & STP Check Agent

- Pay run validation from CSV (Xero/KeyPay export)
- Checks:
  - Base rate below award minimum (simple)
  - Missing penalties
  - Missing allowances
- SG calculation (default 12%)
- STP Phase 2 field validation:
  - Employment basis
  - Income type
  - Country code inconsistencies
- Output:
  - Underpayment suspicion list
  - STP incorrect fields list

### 3.4 Employee Help Agent

- Employee Q&A (“Do I get penalty on PH?”)
- Access to:
  - Contracts
  - Policies
  - Warning letters
- Show:
  - Leave balance (from HRIS API)
  - Employment type & explanation
- Provide step-by-step workflow:
  - Flexible work request
  - Leave request

---

## 4. Non-Functional Requirements

- Uptime: 99%
- Response time: < 1.5s backend, < 3s for AI responses
- Security:
  - JWT auth
  - Role-based access
  - Row-level restrictions
  - Audit logs
- Data residency: Australia region (AWS → Sydney)
- Logging: Serilog + Application Insights

---

## 5. User Flows (High-Level)

### Login → Dashboard → Choose Agent

- Compliance → Chat / Roster Upload
- Document → Template → Editor → PDF
- Payroll → Upload Pay Run → Results
- Employee → Q&A / Documents / Info

---

## 6. Success Metrics

- Reduce compliance-related HR queries by 40%
- Reduce document creation time from 1 hour → <5 minutes
- Catch 50%+ common payroll mistakes in testing
- Customer satisfaction NPS > 30

---

## 7. Out of Scope

- Full Award calculation engine
- End-to-end payroll processing
- Multi-country support
- Xero integration (post-MVP)
