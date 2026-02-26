# FairBot API Security Fix: Agent Service Authentication Protection

**Status**: Completed
**Completion Date**: 2026-02-23
**Branch**: `Lillian-239-refactor-fairbot-from-chat-first-to-action-first-landing-page`

---

## Problem Overview

**Original State**: The frontend FairBot page was making requests directly to the Agent Service (`http://localhost:8000`), bypassing the .NET Backend's JWT authentication system. The Agent Service's `/api/agent/chat` endpoint was completely open to the public — anyone could call it without logging in.

**Scope of Impact**: All Agent Service endpoints (compliance Q&A, roster explain, roster parse, payroll verify)

**Severity**: P0

---

## Original Architecture vs Fixed Architecture

### Original Architecture (Security Flaw)

```
Browser ──── POST http://agent-service:8000/api/agent/chat ──→ Agent Service
              (Direct request, no JWT, no authentication)
```

The frontend code `fairbotApi.ts` created a standalone axios instance that connected directly to the Agent Service:

```typescript
// Before fix: frontend/src/services/fairbotApi.ts
const agentBaseURL = import.meta.env.VITE_AGENT_SERVICE_URL ?? 'http://localhost:8000'

const agentClient = axios.create({
  baseURL: agentBaseURL,  // Points directly to Agent Service
  timeout: 30000,
})
```

Additionally, the Agent Service's CORS configuration allowed any origin:

```python
# Before fix: agent-service/master_agent/main.py
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],   # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
```

### Fixed Architecture (Secure)

```
Browser ── JWT ──→ .NET Backend /api/fairbot/chat ── Internal call ──→ Agent Service
                   (Identity verification + rate limiting + audit)     (Not exposed externally)
```

---

## Why This Fix Was Necessary

### 1. Unauthenticated API Endpoint = Anyone Can Call It

Although the frontend has `ProtectedRoute` + `RoleBasedRoute(admin)` to protect page access, these are only **frontend route guards** — they prevent "the user from seeing the page", not "the API from being called".

Anyone who knows the Agent Service's address can call it directly:

```bash
# No token required, directly invokes LLM
curl -X POST http://your-domain:8000/api/agent/chat \
  -F "message=explain penalty rates for retail workers"
```

**Analogy**: Frontend route guards = locking the front door; unauthenticated Agent Service = leaving the back window wide open.

### 2. LLM Calls Have Real Costs

Each request triggers an OpenAI/Anthropic API call, incurring token costs. Without rate limiting, this means:

- Malicious users can send bulk requests, rapidly consuming API quota
- No per-user usage tracking

### 3. Agent Service Port Must Be Exposed at Deployment

The original architecture required the Agent Service's port 8000 to be reachable from the browser. In a production environment, this means either:

- Exposing the port directly (security risk)
- Exposing it through a reverse proxy (added complexity, still no authentication)

### 4. Inconsistent with the Existing Security Architecture

**All other features** in the project (roster upload, compliance check, settings) are routed through the .NET Backend and protected by JWT. FairBot was the only exception.

---

## Completed Fix Work

### Fix 1: Backend Proxy Endpoint

Added a `FairBotController` in the .NET Backend that receives frontend requests, verifies the JWT, and forwards them to the Agent Service.

**New Files**:

- `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs`
- `backend/src/FairWorkly.Application/FairBot/FairBotChatResponse.cs` (Agent Service response DTO)

**New Method Added to IAiClient Interface**:

```csharp
// backend/src/FairWorkly.Application/Common/Interfaces/IAiClient.cs
Task<TResponse> PostFormAsync<TResponse>(
    string route,
    Dictionary<string, string> formFields,
    CancellationToken cancellationToken = default
);
```

**Controller Implementation**:

```csharp
[Route("api/fairbot")]
[Authorize(Policy = "RequireAdmin")]  // Admin only
public class FairBotController(IAiClient aiClient, ICurrentUserService currentUser) : BaseApiController
{
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromForm] string message,
        [FromForm] string? intentHint = null,
        [FromForm] string? contextPayload = null,
        CancellationToken cancellationToken = default)
    {
        // JWT already verified by [Authorize]
        // Build form fields and forward to Agent Service via PythonAiClient
    }
}
```

### Fix 2: Frontend Switched to httpClient

Changed requests from directly calling the Agent Service to going through the .NET Backend, reusing the existing `httpClient` (which automatically includes the JWT).

**Modified File**: `frontend/src/services/fairbotApi.ts`

```typescript
// After fix: routes through .NET Backend, reusing httpClient (with built-in JWT interceptor)
import httpClient from './httpClient'

export async function sendChatMessage(message: string, options?: SendChatMessageOptions): Promise<AgentChatResponse> {
  const formData = new FormData()
  formData.append('message', message)
  // ...
  const response = await httpClient.post<AgentChatResponse>('/fairbot/chat', formData, {
    headers: { 'Content-Type': undefined },
    timeout: 30000,  // LLM calls need a longer timeout
  })
  return response.data
}
```

Also removed `VITE_AGENT_SERVICE_URL` from `frontend/.env.example` — the frontend no longer needs to know the Agent Service address.

### Fix 3: Tightened Agent Service CORS

**Modified File**: `agent-service/master_agent/main.py`

```python
# After fix: reads allowed origins from environment variable, defaults to Backend only
allowed_origins = os.getenv("ALLOWED_ORIGINS", "http://localhost:5680").split(",")

app.add_middleware(
    CORSMiddleware,
    allow_origins=allowed_origins,
    allow_credentials=False,
    allow_methods=["POST", "GET"],
    allow_headers=["*"],
)
```

### Fix 4: .NET 8 JWT Role Claim Mapping Issue

**Issue Discovered During Implementation**: `[Authorize(Policy = "RequireAdmin")]` always returned 403 Forbidden, even when the JWT did contain `"role": "Admin"`.

**Root Cause**: .NET 8 uses `JsonWebTokenHandler` by default, which automatically remaps JWT claim names. Our JWT stores the role as `"role": "Admin"`, but `RequireRole("Admin")` looks for `ClaimTypes.Role` (`http://schemas.microsoft.com/ws/2008/06/identity/claims/role`) by default, causing the match to fail.

**Why This Wasn't Caught Before**: All Controllers in the project only used `[Authorize]` (which only verifies JWT validity) and never used `[Authorize(Policy = "RequireAdmin")]` (which needs to match the role claim), so this issue remained hidden.

**Fix**: Added the following to the JWT configuration in `backend/src/FairWorkly.API/Program.cs`:

```csharp
options.MapInboundClaims = false; // Prevent .NET from remapping JWT claim names

options.TokenValidationParameters = new TokenValidationParameters
{
    // ...other configuration...
    RoleClaimType = "role", // Tell .NET to use the "role" claim from our JWT
};
```

**Impact**: This fix is not only effective for FairBot but also lays the groundwork for all future Controllers that use `RequireAdmin` / `RequireManager` policies (e.g., when RosterController adds a policy in the future, it will work immediately).

---

## Role Permission Model

| Role | Access Scope | Description |
| ---- | ------------ | ----------- |
| **Admin** | Full system access | Includes FairBot, Roster compliance checking, Settings, and all other features |
| **Manager** | Roster compliance checking | After login, can only use Roster upload and compliance checking features |

Therefore:

- `FairBotController` uses `[Authorize(Policy = "RequireAdmin")]` — FairBot is Admin-only
- `RosterController` should use `[Authorize]` or more granular policies in the future — accessible to both Admin and Manager

---

### Fix 5: Dead Code Cleanup — Removed MockAiClient

**Deleted Files**:

- `backend/src/FairWorkly.Infrastructure/AI/Mocks/MockAiClient.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/MockAiRouter.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/ComplianceMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/DocumentMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/EmployeeMock.cs`
- `backend/src/FairWorkly.Infrastructure/AI/Mocks/Agents/PayrollMock.cs`

**Modified Files**:

- `backend/src/FairWorkly.Infrastructure/DependencyInjection.cs` — Removed `UseMockAi` conditional branch, now always registers `PythonAiClient`
- 5 `appsettings*.json` files — Removed `"UseMockAi"` configuration entries
- `backend/README.md` — Updated AI service section, removed Mock Mode documentation

**Reason**: All methods in `MockAiClient` simply `throw new NotImplementedException()` without providing any mock data — it was entirely dead code.

---

## Complete Data Flow After Fix

```
1. User enters a question on the FairBot page
         │
2. Frontend calls httpClient.post('/fairbot/chat', formData)
   (httpClient automatically attaches the JWT Bearer token)
         │
3. .NET Backend receives the request
   ├── [Authorize(Policy = "RequireAdmin")] verifies JWT + role
   ├── ICurrentUserService extracts userId / orgId
   ├── (Optional) Logs audit trail
   └── (Optional) Checks rate limit
         │
4. Backend forwards to Agent Service via PythonAiClient.PostFormAsync() (internal call)
         │
5. Agent Service processes the request (intent routing → feature → LLM)
         │
6. Response returns via the same path: Agent Service → Backend → Frontend
```

---

## Summary of Modified Files

| File | Change Type | Description |
| ---- | ----------- | ----------- |
| `backend/src/FairWorkly.Application/Common/Interfaces/IAiClient.cs` | Modified | Added `PostFormAsync` method |
| `backend/src/FairWorkly.Infrastructure/AI/PythonServices/PythonAiClient.cs` | Modified | Implemented `PostFormAsync` |
| `backend/src/FairWorkly.Application/FairBot/FairBotChatResponse.cs` | Added | Agent Service response DTO |
| `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs` | Added | Proxy endpoint |
| `backend/src/FairWorkly.API/Program.cs` | Modified | JWT claim mapping fix |
| `backend/src/FairWorkly.Infrastructure/DependencyInjection.cs` | Modified | Removed mock branch |
| `backend/src/FairWorkly.Infrastructure/AI/Mocks/` | Deleted | 6 dead code files |
| `backend/appsettings*.json` (5 files) | Modified | Removed `UseMockAi` |
| `backend/README.md` | Modified | Updated documentation |
| `frontend/src/services/fairbotApi.ts` | Modified | Switched to httpClient |
| `frontend/.env.example` | Modified | Removed `VITE_AGENT_SERVICE_URL` |
| `agent-service/master_agent/main.py` | Modified | Tightened CORS |

---

## Verification Results

### Automated Tests

- Backend unit tests: All 190 passed
- Backend integration tests: All 41 passed
- Frontend TypeScript compilation: No errors

### Manual End-to-End Tests

```bash
# Test 1: Call without JWT → 401 Unauthorized (correctly rejected)
curl -s -o /dev/null -w "%{http_code}" -X POST http://localhost:5680/api/fairbot/chat \
  -F "message=hello"
# Result: 401

# Test 2: Call with Admin JWT → 200 OK (correctly allowed)
TOKEN=$(curl -s -X POST http://localhost:5680/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@fairworkly.com.au","password":"fairworkly123"}' | jq -r '.data.accessToken')

curl -s -w "\n%{http_code}" -X POST http://localhost:5680/api/fairbot/chat \
  -H "Authorization: Bearer $TOKEN" \
  -F "message=hello"
# Result: 200, returns normal response from Agent Service
```
