# JWT Authentication Design

## Overview

FairWorkly uses JWT access tokens + HttpOnly refresh token cookies for stateless authentication. Access tokens are short-lived (15 min), refresh tokens are long-lived (7 days) and stored as hashed values in the database.

## Architecture

```
                           Login Flow
                           ──────────

Browser                     Backend (.NET)                  PostgreSQL
───────                     ──────────────                  ──────────
POST /api/auth/login
  { email, password }
  ──────────────────────────>
                             Verify password hash
                             Generate access token (JWT)
                             Generate refresh token (random)
                             Hash refresh token → DB
  <──────────────────────────
  Response JSON:
    { accessToken, user }
  Set-Cookie:
    refreshToken=<plain>; HttpOnly; SameSite=Lax


                        Session Restore Flow (App Startup)
                        ──────────────────────────────────

Browser                     Backend (.NET)                  PostgreSQL
───────                     ──────────────                  ──────────
POST /api/auth/refresh
  Cookie: refreshToken=<plain>
  ──────────────────────────>
                             Hash(plain) → compare with DB
                             Issue new access token
                             Rotate refresh token
  <──────────────────────────
  Response JSON:
    { accessToken }
  Set-Cookie:
    refreshToken=<new>; HttpOnly


                        Authenticated API Call
                        ─────────────────────

Browser                     Backend (.NET)
───────                     ──────────────
GET /api/roster/upload
  Authorization: Bearer <accessToken>
  ──────────────────────────>
                             Validate JWT signature + expiry
                             Extract claims (sub, orgId, role)
                             Process request
  <──────────────────────────
```

## JWT Token Structure

### Access Token Claims

| Claim   | Source                | Example                     | Purpose          |
| ------- | --------------------- | --------------------------- | ---------------- |
| `sub`   | `user.Id`             | `"a1b2c3d4-..."`            | User identity    |
| `email` | `user.Email`          | `"admin@fairworkly.com.au"` | User email       |
| `orgId` | `user.OrganizationId` | `"e5f6g7h8-..."`            | Tenant isolation |
| `role`  | `user.Role`           | `"Admin"`                   | Authorization    |
| `jti`   | Random GUID           | `"f9a0b1c2-..."`            | Token unique ID  |
| `exp`   | Now + 15min           | Unix timestamp              | Expiry           |

Generated in: `Infrastructure/Identity/TokenService.cs`

### Refresh Token

- 32-byte cryptographically random value (Base64 encoded)
- Stored as **hash** in `User.RefreshToken` column (never stored as plain text)
- Sent to browser as **plain** in HttpOnly cookie
- Cookie settings:
  - `HttpOnly = true` (prevent XSS access)
  - `Secure = false` (dev) / `true` (production)
  - `SameSite = Lax` (dev) / `None` (production)

## Role System

### Domain Roles

```csharp
// Domain/Auth/Enums/UserRole.cs
public enum UserRole
{
    Admin = 1,    // Business owner - full system access
    Manager = 2,  // Department/store manager - limited access
}
```

### Role-Based Access Matrix

| Module          | Route              | Admin | Manager | Backend Policy   |
| --------------- | ------------------ | :---: | :-----: | ---------------- |
| Roster Upload   | `/roster/upload`   |  Yes  |   Yes   | `RequireManager` |
| Roster Results  | `/roster/results`  |  Yes  |   Yes   | `RequireManager` |
| FairBot         | `/fairbot`         |  Yes  | **No**  | `RequireAdmin`   |
| Payroll Upload  | `/payroll/upload`  |  Yes  | **No**  | `RequireAdmin`   |
| Payroll Results | `/payroll/results` |  Yes  | **No**  | `RequireAdmin`   |
| Documents       | `/documents/*`     |  Yes  | **No**  | `RequireAdmin`   |
| Settings        | `/settings`        |  Yes  | **No**  | `RequireAdmin`   |

### Frontend Enforcement (Route Guards)

```
ProtectedRoute          → checks isAuthenticated (any logged-in user)
  └── RoleBasedRoute    → checks user.role against allow list
```

Route configuration in `app/routes/tools.routes.tsx`:

```typescript
// Admin only
<RoleBasedRoute allow={['admin']} />       → Payroll, Documents, Settings

// Admin + Manager
<RoleBasedRoute allow={['admin', 'manager']} />  → Roster
```

### Frontend Enforcement (Sidebar Navigation)

Sidebar menu items have `allowedRoles` property. Items are **hidden** (not just disabled) for unauthorized roles:

```typescript
// Sidebar/config/navigation.config.tsx
{ to: '/roster/upload',    label: 'Check Roster',        allowedRoles: ['admin', 'manager'] }
{ to: '/payroll/upload',   label: 'Verify Payroll',      allowedRoles: ['admin'] }
{ to: '/documents',        label: 'Documents Compliance', allowedRoles: ['admin'] }
{ to: '/settings',         label: 'Settings',            allowedRoles: ['admin'] }
```

`SidebarNav` component filters items with `filterByRole()` before rendering - Manager users only see Roster in their sidebar.

## Changes Made in #240

### Problem: Login Was a DEV Stub

**Before**: `LoginPage.tsx` used `simulateAuth()` which set localStorage and navigated to `/fairbot` without any backend call:

```typescript
// OLD - DEV stub
const simulateAuth = (options) => {
  localStorage.setItem("dev:user-name", options.name);
  navigate("/fairbot");
};
```

**After**: Real API call to backend:

```typescript
// NEW - Real login
const response = await authApi.login(values.email, values.password)
dispatch(setAuthData({ user: ..., accessToken: response.accessToken }))
navigate(DEFAULT_ROUTES[response.user.role.toLowerCase()] ?? '/403')
```

### Problem: useAuth Read from localStorage Instead of Redux

**Before**: `useAuth()` was a DEV stub reading from localStorage:

```typescript
// OLD - DEV stub
const name = localStorage.getItem("dev:user-name");
return { isAuthenticated: !!name, user: { name, role: "admin" } };
```

**After**: Reads from Redux auth slice:

```typescript
// NEW - Redux-based
const reduxUser = useSelector((state: RootState) => state.auth.user);
const accessToken = useSelector((state: RootState) => state.auth.accessToken);
const isAuthenticated = !!accessToken && !!user && status === "authenticated";
```

### Problem: No Session Restore on Page Refresh

**Before**: `ReduxProvider.tsx` did not restore auth state on startup. Page refresh = logged out.

**After**: Calls `/auth/refresh` on app startup, then `/auth/me` to fetch user data:

```typescript
// ReduxProvider.tsx
const initializeAuth = async () => {
  const refreshRes = await axios.post(`${baseURL}/auth/refresh`, null, {
    withCredentials: true,
  });
  const accessToken = refreshRes.data?.accessToken;
  if (!accessToken) return;

  const meRes = await axios.get(`${baseURL}/auth/me`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  const u = meRes.data;
  store.dispatch(setAuthData({
    user: { id: u.id, email: u.email, name: ..., role: u.role },
    accessToken,
  }));
};
```

### Problem: CORS Blocked Credential Requests

**Before**: `AllowAnyOrigin()` is incompatible with `AllowCredentials()`.

**After**: Explicit origin in `Program.cs`:

```csharp
policy.WithOrigins("http://localhost:5173").AllowCredentials();
```

### Problem: Missing Role Defaulted to Admin (Security)

**Before**: `useAuth()` used `|| 'admin'` fallback when role was missing or undefined, silently granting full admin access:

```typescript
// OLD - unsafe fallback
role: (reduxUser.role?.toLowerCase() || 'admin') as 'admin' | 'manager'
```

**After**: Pass raw role value without fallback. `RoleBasedRoute` handles invalid/missing roles by redirecting to `/403`:

```typescript
// NEW - no fallback, let RoleBasedRoute enforce
role: reduxUser.role?.toLowerCase() as 'admin' | 'manager'
```

`RoleBasedRoute` line 18: `if (!role || !allow.includes(role)) return <Navigate to="/403" replace />`

## Current Implementation Status

### Done

- [x] Backend: Login endpoint (`POST /api/auth/login`) with password hash verification
- [x] Backend: Refresh endpoint (`POST /api/auth/refresh`) with token rotation
- [x] Backend: Logout endpoint (`POST /api/auth/logout`) clears cookie + DB
- [x] Backend: Me endpoint (`GET /api/auth/me`) returns current user
- [x] Backend: JWT generation with claims (`sub`, `email`, `orgId`, `role`)
- [x] Backend: Refresh token hashing and storage
- [x] Backend: HttpOnly cookie for refresh token
- [x] Backend: Authorization policies defined (`RequireAdmin`, `RequireManager`)
- [x] Backend: `[Authorize]` on RosterController
- [x] Frontend: `authApi.ts` service (login, logout, me)
- [x] Frontend: `LoginPage.tsx` calls real API
- [x] Frontend: Redux `authSlice` stores user + accessToken
- [x] Frontend: `useAuth()` reads from Redux
- [x] Frontend: `ReduxProvider.tsx` restores session on startup via refresh
- [x] Frontend: `setupInterceptors.ts` attaches Bearer token + 401 refresh retry
- [x] Frontend: `ProtectedRoute` checks isAuthenticated
- [x] Frontend: `RoleBasedRoute` checks user.role against allow list
- [x] Frontend: Sidebar navigation filters items by role

### Missing / TODO

#### Authorization (High Priority)

- [ ] **Backend role enforcement on API endpoints**: Controllers use `[Authorize]` but not `[Authorize(Policy = "RequireAdmin")]`. All endpoints are accessible to any authenticated user regardless of role. Need to add policy-based authorization:
  - Payroll endpoints → `[Authorize(Policy = "RequireAdmin")]`
  - Document endpoints → `[Authorize(Policy = "RequireAdmin")]`
  - Settings endpoints → `[Authorize(Policy = "RequireAdmin")]`
  - Roster endpoints → `[Authorize(Policy = "RequireManager")]` (Admin + Manager)
- [x] ~~**Backend `RequireManager` policy role name mismatch**~~: Fixed `policy.RequireRole("Admin", "HrManager")` to `policy.RequireRole("Admin", "Manager")` to match domain enum.
- [x] ~~**FairBot route permission**~~: Changed `fairbot.routes.tsx` from `allow={['admin', 'manager']}` to `allow={['admin']}`. Manager cannot access FairBot.

#### Session & User (Medium Priority)

- [x] ~~**Refresh endpoint does not return user info**~~: Solved by calling `GET /api/auth/me` after refresh in `ReduxProvider.tsx` to fetch real user data.
- [x] ~~**Logout does not call backend**~~: `useAuth().logout()` now calls `POST /api/auth/logout` to clear the HttpOnly cookie before clearing Redux state.
- [ ] **Token expiry UX**: No proactive token refresh before expiry. Only refreshes on 401 error (reactive). Consider proactive refresh at `exp - buffer`.
- [ ] **Manager user seeding**: DbSeeder only creates Admin user. Need Manager test user for role-based testing.

#### Features Not Implemented (Low Priority)

- [ ] **No signup implementation**: `handleSignup()` in LoginPage is a console.log stub
- [ ] **No Google OAuth**: `handleGoogleLogin()` is a console.log stub
- [ ] **No password reset flow**: ForgotPasswordModal exists but no backend endpoint

## Key Files

| Layer    | File                                          | Purpose                                    |
| -------- | --------------------------------------------- | ------------------------------------------ |
| Backend  | `Controllers/Auth/AuthController.cs`          | Login, refresh, me, logout endpoints       |
| Backend  | `Application/Auth/Features/Login/`            | LoginCommand, Handler, Response, Validator |
| Backend  | `Application/Auth/Features/Refresh/`          | RefreshCommand, Handler                    |
| Backend  | `Infrastructure/Identity/TokenService.cs`     | JWT + refresh token generation             |
| Backend  | `API/Program.cs` (L167-172)                   | Authorization policies                     |
| Backend  | `Domain/Auth/Enums/UserRole.cs`               | Admin, Manager enum                        |
| Frontend | `services/authApi.ts`                         | Login, logout, me API calls                |
| Frontend | `services/httpClient.ts`                      | Axios with withCredentials: true           |
| Frontend | `services/setupInterceptors.ts`               | Bearer token injection + 401 refresh       |
| Frontend | `slices/auth/authSlice.ts`                    | Redux auth state                           |
| Frontend | `modules/auth/pages/LoginPage.tsx`            | Login form + API call                      |
| Frontend | `modules/auth/hooks/useAuth.tsx`              | Redux-based auth hook                      |
| Frontend | `app/providers/ReduxProvider.tsx`             | Session restore on startup                 |
| Frontend | `shared/components/guards/ProtectedRoute.tsx` | Authentication gate                        |
| Frontend | `shared/components/guards/RoleBasedRoute.tsx` | Role-based route gate                      |
| Frontend | `Sidebar/config/navigation.config.tsx`        | Role-filtered menu items                   |
| Frontend | `Sidebar/components/SidebarNav.tsx`           | Menu filtering logic                       |
