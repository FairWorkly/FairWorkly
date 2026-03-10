# UAT Frontend ↔ Backend Connection Audit

> Date: 2026-03-10
> Branch: devops/eric-ECS-ECR-FARGATE

## Topology

```
Browser (https://uat.fairworkly.com)
  → ALB (api.fairworkly.com, HTTPS 443)
    → Target Group
      → ECS Fargate task (backend, HTTP 5680)
```

The ALB terminates TLS and forwards requests **verbatim** — it does **not** rewrite or strip paths.

---

## Bugs Found

### Bug 1 — 404 on all API endpoints

**Symptom:** `POST https://api.fairworkly.com/auth/refresh` returns 404.

**Root cause:** `frontend/.env.uat` had:
```
VITE_API_BASE_URL=https://api.fairworkly.com
```
The backend controller attribute is `[Route("api/[controller]")]`, so every route is prefixed `/api/`. The ALB passes the path through unchanged, so the backend never sees a route matching `/auth/refresh` — it only knows `/api/auth/refresh`.

This bug affects **every** API endpoint (login, roster, payroll, etc.), not just refresh. The refresh endpoint surfaced it first because it triggers on 401s after the 15-minute access token expiry.

**Fix:** `frontend/.env.uat` line 1:
```
VITE_API_BASE_URL=https://api.fairworkly.com/api
```

**Local dev note:** The same mismatch exists if a developer's local `frontend/.env` has `http://localhost:5680` without `/api`. Correct local value is `http://localhost:5680/api`.

---

### Bug 2 — CORS blocked for https://uat.fairworkly.com

**Symptom:** Browser blocks all requests with CORS error.

**Root cause:** `Program.cs` reads allowed origins from config:
```csharp
// Program.cs:124-126
var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();
policy.WithOrigins(allowedOrigins)...
```
The key `Cors:AllowedOrigins` did not exist in `appsettings.json` or any environment config, so `allowedOrigins` was an empty array — `WithOrigins()` with no entries blocks all cross-origin requests.

**Fix:** Add to `backend/src/FairWorkly.API/appsettings.json`:
```json
"Cors": {
  "AllowedOrigins": [
    "https://uat.fairworkly.com",
    "http://localhost:5173",
    "http://localhost:3000"
  ]
}
```

For future environments (prod, staging), override `Cors:AllowedOrigins` via ECS task environment variables using double-underscore notation:
```
Cors__AllowedOrigins__0=https://www.fairworkly.com
Cors__AllowedOrigins__1=https://uat.fairworkly.com
```

---

### Bug 3 — UAT env vars not loaded during build

**Symptom:** `VITE_API_BASE_URL` undefined in UAT build if built without the correct mode flag.

**Root cause:** Vite loads `.env.{mode}` based on the build mode. The default `vite build` command uses mode `production`, which loads `.env.production` — not `.env.uat`. Without `--mode uat`, the UAT env file is ignored and `VITE_API_BASE_URL` falls back to the hardcoded `/api` (relative path, broken in cross-origin UAT).

**Fix:** Always build UAT frontend with:
```bash
npx tsc -b && npx vite build --mode uat
```
Or use the `build:uat` npm script (added to `package.json`):
```bash
npm run build:uat
```

---

## Auth / Token Architecture (for debugging)

| Token | Storage | Lifetime | Transport |
|-------|---------|----------|-----------|
| Access token | Redux store (`auth.accessToken`) | 15 min | `Authorization: Bearer` header |
| Refresh token | HttpOnly cookie (`refreshToken`) | 7 days | Sent automatically by browser |

**Cookie flags:** `Secure=true, SameSite=None, Path=/`
Required for cross-domain cookies (`uat.fairworkly.com` → `api.fairworkly.com`). Both must be HTTPS.

**Refresh flow:**
1. Any request returns 401 → `setupInterceptors.ts` intercepts
2. `POST /api/auth/refresh` is called via a separate `refreshClient` axios instance (no interceptors, avoids recursion)
3. New access token returned → queued failed requests retried
4. On refresh failure → Redux `logout()` dispatched → redirect to `/login`

**Startup flow:**
`ReduxProvider.tsx` calls `POST /api/auth/refresh` with raw `axios` on app mount to restore session from the HttpOnly cookie. Uses `.data?.data` to manually unwrap the backend `{ code, msg, data }` envelope (since it bypasses the response interceptor intentionally).

---

## Key Files

| Purpose | Path |
|---------|------|
| UAT environment vars | `frontend/.env.uat` |
| Axios HTTP client | `frontend/src/services/httpClient.ts` |
| Token refresh interceptor | `frontend/src/services/setupInterceptors.ts` |
| Startup auth init | `frontend/src/app/providers/ReduxProvider.tsx` |
| Backend CORS + JWT setup | `backend/src/FairWorkly.API/Program.cs` |
| Base backend config | `backend/src/FairWorkly.API/appsettings.json` |
| Auth controller routes | `backend/src/FairWorkly.API/Controllers/Auth/AuthController.cs` |

---

## Watch Items

- **`UseHttpsRedirection` behind ALB:** `Program.cs:215` has `app.UseHttpsRedirection()`. In ECS, the ALB terminates TLS and forwards HTTP internally. If the backend receives an HTTP request (e.g., ALB health check), this middleware issues a 301 redirect — CORS preflight responses on redirects are dropped by browsers. Monitor ALB health check logs if health checks start failing.

---

## Verification Commands

After deploying fixes, run these to confirm:

```bash
# 1. Check CORS preflight
curl -i -X OPTIONS https://api.fairworkly.com/api/auth/refresh \
  -H "Origin: https://uat.fairworkly.com" \
  -H "Access-Control-Request-Method: POST"
# Expect: 200, Access-Control-Allow-Origin: https://uat.fairworkly.com

# 2. Confirm endpoint exists (no cookie = 401, not 404)
curl -i -X POST https://api.fairworkly.com/api/auth/refresh \
  -H "Origin: https://uat.fairworkly.com"
# Expect: 401 (missing refresh token) — NOT 404
```
