# Roster Upload Feature - Design Document

## Overview

Roster Upload enables admins/managers to upload Excel (.xlsx) roster files through the frontend, which are parsed by the Agent Service, stored in the database, and prepared for compliance validation.

## Architecture

```
Frontend (React)                  Backend (.NET)                Agent Service (Python)
────────────────                  ──────────────                ──────────────────────
RosterUpload.tsx                  RosterController              /api/agent/chat
  │                                 │                              │
  │ POST /api/roster/upload         │                              │
  │ (multipart/form-data)           │                              │
  ├────────────────────────────────>│                              │
  │                                 │  1. Save to local storage    │
  │                                 │  2. Forward file via         │
  │                                 │     multipart POST           │
  │                                 │─────────────────────────────>│
  │                                 │                              │ Parse Excel
  │                                 │                              │ (RosterExcelParser)
  │                                 │     ParseResponse (JSON)     │
  │                                 │<─────────────────────────────│
  │                                 │  3. Match employees          │
  │                                 │  4. Create Roster + Shifts   │
  │                                 │  5. Save to PostgreSQL       │
  │  UploadRosterResponse (JSON)    │                              │
  │<────────────────────────────────│                              │
  │                                 │                              │
  │ Navigate to /roster/results      │                              │
```

## Data Flow

### 1. Frontend Upload

- **Page**: `frontend/src/modules/roster/pages/RosterUpload.tsx`
- **API**: `frontend/src/services/rosterApi.ts` -> `uploadRoster(file)`
- **HTTP Client**: `frontend/src/services/httpClient.ts` (axios, baseURL from `VITE_API_BASE_URL`)
- User selects .xlsx file -> clicks "Start Validation" -> `POST /api/roster/upload`
- File sent as `multipart/form-data` with field name `file`

### 2. Backend Processing

- **Controller**: `backend/src/FairWorkly.API/Controllers/Roster/RosterController.cs`

  - Extracts `userId` (JWT `sub` claim) and `organizationId` (JWT `orgId` claim)
  - Passes to `UploadRosterCommand` via MediatR

- **Handler**: `backend/src/FairWorkly.Application/Roster/Features/UploadRoster/UploadRosterHandler.cs`

  - Step 1: Upload file to storage (`IFileStorageService` -> LocalFileStorageService in dev)
  - Step 2: Forward file to Agent Service via `IAiClient.PostMultipartAsync()`
  - Step 3: Check for blocking parse errors
  - Step 4: Validate parsed data has entries and valid dates
  - Step 5: Bulk lookup employees by employee_number and email
  - Step 6: Create `Roster` entity with file storage reference
  - Step 7: Create `Shift` entities, matching to employees
  - Step 8: Save all in single transaction
  - Step 9: Build response with warnings

- **Validator**: `UploadRosterValidator.cs` (FluentValidation)
  - FileStream not null, FileName has .xlsx/.xls extension
  - FileSize > 0 and <= 50MB
  - ContentType not empty, OrganizationId and UserId not empty

### 3. Agent Service Parsing

- **Endpoint**: `POST /api/agent/chat` (FastAPI)
  - `agent-service/master_agent/main.py` -> `IntentRouter` -> `RosterFeature`
- **Feature**: `agent-service/master_agent/features/roster_feature.py`
  - Saves uploaded file to temp location
  - Parses via `RosterExcelParser` in LENIENT mode
  - Returns `ParseResponse` as JSON (via `model_dump()`)
- **Parser**: `agent-service/agents/roster/services/roster_import/`
  - Supports .xlsx only (openpyxl library)
  - Does NOT support legacy .xls format

### 4. Response Models

**Agent Service -> Backend** (`AgentChatResponse` wrapper):

```json
{
  "status": "success",
  "result": {
    "result": {
      "entries": [...],
      "week_start_date": "2026-02-02",
      "week_end_date": "2026-02-08",
      "total_shifts": 15,
      "total_hours": 120.5,
      "unique_employees": 5
    },
    "issues": [...],
    "summary": { "status": "ok", "error_count": 0, "warning_count": 2 }
  }
}
```

**Backend -> Frontend** (`UploadRosterResponse`):

```json
{
  "rosterId": "guid",
  "weekStartDate": "2026-02-02T00:00:00Z",
  "weekEndDate": "2026-02-08T00:00:00Z",
  "totalShifts": 15,
  "totalHours": 120.5,
  "totalEmployees": 5,
  "warnings": [
    { "code": "EMPLOYEE_NOT_FOUND", "message": "...", "row": 0, "hint": "..." }
  ]
}
```

## Database Changes

### Roster Entity (updated fields)

| Field               | Type        | Description                   |
| ------------------- | ----------- | ----------------------------- |
| `OriginalFileS3Key` | string(500) | Storage key for uploaded file |
| `OriginalFileName`  | string(255) | Original filename from user   |

Migration: `20260207120930_AddS3FieldsToRoster`

### File Storage

- **Development**: `LocalFileStorageService` -> saves to `wwwroot/uploads/`
- **Production**: `S3FileStorageService` -> AWS S3 bucket

## Authentication Flow

### JWT Design

```
Login:  POST /api/auth/login  ->  { accessToken, user }  +  HttpOnly cookie (refreshToken)
Refresh: POST /api/auth/refresh  ->  { accessToken }  (reads cookie)
```

- Access token contains claims: `sub` (userId), `email`, `orgId`, `role`
- Refresh token stored as hash in database, plain in HttpOnly cookie
- Frontend stores access token in Redux (not localStorage, not Redux Persist)

### Frontend Auth State

- `useAuth()` hook reads from Redux `auth` slice (not localStorage)
- `ReduxProvider.tsx` calls `/auth/refresh` on app startup to restore session
- `setupInterceptors.ts` attaches `Authorization: Bearer <token>` to all requests
- On 401 response, interceptor attempts token refresh, queues failed requests

## Key Files

| Layer    | File                                                    | Purpose                                   |
| -------- | ------------------------------------------------------- | ----------------------------------------- |
| Frontend | `modules/roster/pages/RosterUpload.tsx`                 | Upload UI, file selection, start analysis |
| Frontend | `services/rosterApi.ts`                                 | `uploadRoster()` API call                 |
| Frontend | `services/httpClient.ts`                                | Axios instance, baseURL config            |
| Frontend | `services/authApi.ts`                                   | `login()`, `logout()`, `me()`             |
| Frontend | `modules/auth/pages/LoginPage.tsx`                      | Real login (POST /api/auth/login)         |
| Frontend | `modules/auth/hooks/useAuth.tsx`                        | Redux-based auth state                    |
| Frontend | `app/providers/ReduxProvider.tsx`                       | Auth initialization on startup            |
| Backend  | `Controllers/Roster/RosterController.cs`                | Upload endpoint                           |
| Backend  | `Roster/Features/UploadRoster/UploadRosterCommand.cs`   | Command model                             |
| Backend  | `Roster/Features/UploadRoster/UploadRosterHandler.cs`   | Full upload flow                          |
| Backend  | `Roster/Features/UploadRoster/UploadRosterValidator.cs` | Input validation                          |
| Backend  | `Roster/Features/UploadRoster/AgentServiceModels.cs`    | Agent Service response mapping            |
| Backend  | `Roster/Features/UploadRoster/UploadRosterResponse.cs`  | API response model                        |
| Backend  | `Infrastructure/AI/PythonServices/PythonAiClient.cs`    | Multipart POST to Agent Service           |
| Backend  | `Infrastructure/Storage/S3FileStorageService.cs`        | S3 file storage                           |
| Backend  | `Infrastructure/Services/LocalFileStorageService.cs`    | Local dev file storage                    |
| Agent    | `master_agent/main.py`                                  | FastAPI entry, /api/agent/chat            |
| Agent    | `master_agent/features/roster_feature.py`               | File parsing orchestration                |

## Known Issues and TODOs

### DateTime UTC Conversion (Blocking)

PostgreSQL `timestamp with time zone` columns require `DateTime.Kind = Utc`. Dates from Agent Service are deserialized as `DateTimeKind.Unspecified`. Need to either:

- Add `Npgsql.EnableLegacyTimestampBehavior` (AppContext switch)
- Add EF Core `ConfigureConventions` with a UTC converter
- Or use `DateTime.SpecifyKind()` at the handler level

### Pending Items

- [ ] Fix DateTime UTC issue for PostgreSQL
- [x] Navigate to `/roster/results` after upload
- [ ] Results page integration with actual roster data (separate ticket)
- [x] Error display: backend error messages shown in shared `ComplianceUpload` error prop
- [ ] File re-upload (replace existing roster)

## Configuration

### Environment Variables

| Variable                              | Service  | Value (Dev)                  |
| ------------------------------------- | -------- | ---------------------------- |
| `VITE_API_BASE_URL`                   | Frontend | `http://localhost:5680/api`  |
| `AiSettings:BaseUrl`                  | Backend  | `http://localhost:8000`      |
| `ConnectionStrings:DefaultConnection` | Backend  | PostgreSQL connection string |

### CORS (Development)

Backend `Program.cs`:

```csharp
policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
```

### Ports

| Service                 | Port |
| ----------------------- | ---- |
| Frontend (Vite)         | 5173 |
| Backend (.NET)          | 5680 |
| Agent Service (FastAPI) | 8000 |
