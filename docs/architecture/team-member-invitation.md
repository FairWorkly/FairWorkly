# Team Member Invitation

Admins can invite new team members by email. The invitee receives a one-time link, sets a password, and joins the organisation. Admins can also resend or cancel a pending invitation.

---

## Table of Contents

1. [User Flow](#user-flow)
2. [Backend Design](#backend-design)
3. [Frontend Design](#frontend-design)
4. [Data Flow](#data-flow)
5. [Database Schema](#database-schema)
6. [Security & Edge Cases](#security--edge-cases)
7. [Configuration](#configuration)
8. [API Reference](#api-reference)

---

## User Flow

```text
Admin (Settings → Team)                  Invitee (email link)
───────────────────────                  ────────────────────
1. Clicks "Invite Member"
2. Fills in email, name, role
3. Submits form
        │
        ▼
4. Backend creates pending user,
   generates secure token, returns
   invite link
        │
5. Admin copies link from dialog
   and sends it to the invitee
                                    6. Invitee opens link in browser
                                       /accept-invite?token=<plain>
                                    7. Frontend validates token (preflight)
                                       → shows name/email of account
                                    8. Invitee sets a password
                                    9. Backend accepts invitation
                                       atomically
                                   10. Invitee is redirected to login
```

### Admin – Manage Pending Invitations

| Action | When | Result |
|--------|------|--------|
| **Resend** | Invitation expired or not received | Rotates token, returns fresh link |
| **Cancel** | Change of plans | Soft-deletes the pending user |

---

## Backend Design

### Layer Map

```text
API Controller
    └─ MediatR command/query
        └─ Application Handler   ← business logic
            ├─ Domain Entity      ← invariants & state transitions
            └─ Infrastructure     ← EF Core, ISecretHasher, IUnitOfWork
```

### Domain Entity – `User`

Invitation state lives directly on the `User` entity (`FairWorkly.Domain/Auth/Entities/User.cs`).

**Fields added:**

| Property | Type | Description |
|----------|------|-------------|
| `InvitationStatus` | `InvitationStatus` enum | `None` / `Pending` / `Accepted` |
| `InvitationToken` | `string?` | SHA-256 hash of the plain token (never stored plain) |
| `InvitationTokenExpiry` | `DateTime?` | UTC expiry timestamp |

**Domain methods:**

```csharp
// Stamp a new token onto a pending user (also used by Resend)
user.SetInvitationToken(tokenHash, expiresAtUtc);

// Verify status, set password, activate account, clear token fields
user.AcceptInvitation(passwordHash);      // throws if not Pending

// Verify status, soft-delete, clear token fields
user.CancelInvitation();                  // throws if not Pending
```

Both mutating methods guard on `InvitationStatus == Pending` and throw `InvalidDomainStateException` on misuse — preventing accidental state corruption from application-layer bugs.

`ValidateDomainRules()` is called by `FairWorklyDbContext.SaveChangesAsync()` before every write. The credential invariant reads:

> A user must have at least one credential (`PasswordHash` **or** `GoogleId`), **unless** they are a pending invitee **or** soft-deleted.

### Application Layer – Handlers

#### `InviteTeamMemberHandler`

```text
1. Normalise email (trim + lowercase)
2. Parse & whitelist role  →  must be Admin or Manager
   (Enum.TryParse alone is not enough; numeric strings like "999" pass – Enum.IsDefined guards this)
3. Check (OrganizationId, Email) uniqueness
4. Generate token  →  InvitationHelper.GenerateToken()
5. new User { InvitationStatus = Pending, InvitationToken = hash, … }
6. SaveChangesAsync  →  domain validation runs here
   └─ race-condition guard: re-check email uniqueness on DbException
7. Return 201 + { userId, inviteLink }
```

#### `ValidateInvitationTokenHandler`

Read-only preflight used by the frontend before showing the password form.

```text
1. Hash plain token
2. GetByInvitationTokenHashAsync  →  404 if not found
3. Status == Accepted  →  409 "already accepted"
4. Status != Pending   →  409 "no longer valid"
5. Expiry <= now       →  409 "expired"
6. Return 200 + { email, fullName }
```

#### `AcceptInvitationHandler`

Uses a **compare-and-set** pattern to guarantee exactly one concurrent request succeeds.

```text
1. Hash plain token
2. Pre-read user  →  surface friendly errors (accepted / cancelled / expired)
3. Hash the new password
4. AcceptInvitationAtomicAsync()
   └─ single SQL UPDATE WHERE token = @hash
                            AND status = Pending
                            AND expiry > @now
                            AND is_deleted = false
   └─ rowsAffected == 0  →  409 "concurrent acceptance lost the race"
5. Return 200 + { email, fullName }
```

Why atomic? Without this, two simultaneous requests can both pass the pre-read checks, then both write — the second overwriting the first password hash. The `ExecuteUpdateAsync` WHERE clause is the gate; only one UPDATE returns `rowsAffected = 1`.

> **Note:** `ExecuteUpdateAsync` bypasses EF Core's global query filter (`is_deleted = false`), so `!u.IsDeleted` is included explicitly in the WHERE predicate.

#### `CancelInvitationHandler`

```text
1. GetByIdAsync  →  404 if not found
2. OrganizationId mismatch  →  404 (not 403 — avoids leaking cross-org membership)
3. Status != Pending  →  409
4. user.CancelInvitation()  →  domain guard fires here
5. SaveChangesAsync  →  soft-deletes the user
```

#### `ResendInvitationHandler`

```text
1–3. Same org + pending checks as Cancel
4. InvitationHelper.GenerateToken()  →  fresh token replaces old one
5. user.SetInvitationToken(newHash, newExpiry)
6. SaveChangesAsync
7. Return 200 + { inviteLink }
```

### `InvitationHelper`

```csharp
// Plain token: cryptographically random, 32 bytes → base64url
// Token hash:  ISecretHasher (HMAC-SHA256 with server secret)
// Expiry:      DateTime.UtcNow + AuthSettings:InvitationTokenExpiryDays (default 7)
(string PlainToken, string TokenHash, DateTime ExpiresAtUtc) GenerateToken(...)

// Returns:  {Frontend:BaseUrl}/accept-invite?token={plainToken}
string BuildInviteLink(...)
```

The plain token is returned to the caller (and shown in the invite link) but **never persisted**. Only the hash lives in the database.

### `UserRepository` – Key Methods

| Method | Purpose |
|--------|---------|
| `GetByInvitationTokenHashAsync(hash)` | Lookup by hashed token; global filter excludes soft-deleted |
| `IsEmailUniqueAsync(orgId, email)` | Composite uniqueness check excluding soft-deleted rows |
| `AcceptInvitationAtomicAsync(hash, passwordHash, now)` | Single atomic UPDATE; returns rows affected |

---

## Frontend Design

### Module Structure

```text
frontend/src/
├── modules/
│   ├── settings/
│   │   ├── features/teamMembers/
│   │   │   └── TeamMembersSection.tsx      ← page-level orchestrator
│   │   ├── components/TeamMembers/
│   │   │   ├── TeamMembersTable.tsx         ← renders the member list
│   │   │   ├── InviteDialog.tsx             ← invite form + link display
│   │   │   ├── CancelInviteDialog.tsx       ← confirmation before cancel
│   │   │   ├── RoleChangeDialog.tsx         ← confirmation before role change
│   │   │   └── DeactivateDialog.tsx         ← confirmation before deactivate
│   │   ├── hooks/
│   │   │   └── useTeamMembers.ts            ← TanStack Query hooks
│   │   └── types/
│   │       └── teamMembers.types.ts
│   └── auth/
│       ├── pages/
│       │   └── AcceptInvitePage.tsx         ← public invitation landing page
│       └── hooks/
│           └── useAcceptInvitation.ts
└── services/
    └── settingsApi.ts                       ← all API calls
```

### State Machine – `InviteDialog`

The dialog is two-step; the same component handles both:

```text
Step 1 – Form
┌─────────────────────────────────┐
│  Email                          │
│  First Name   Last Name         │
│  Role  [Manager ▾]              │
│                                 │
│  [Cancel]       [Send Invite]   │
└─────────────────────────────────┘
         on success (inviteLink returned)
                  │
                  ▼
Step 2 – Link
┌─────────────────────────────────┐
│  Invitation Sent                │
│  ┌──────────────────────┐ [📋] │
│  │ https://…/accept-… │      │
│  └──────────────────────┘      │
│  (copy error hint shown 3 s)   │
│                    [Done]       │
└─────────────────────────────────┘
```

### Pending Invitation UX – `TeamMembersTable`

Pending members appear in the list with extra status indicators:

```text
┌─────────────┬──────────────────┬──────────┬──────────────────────────────┐
│ Name        │ Email            │ Role     │ Actions                      │
├─────────────┼──────────────────┼──────────┼──────────────────────────────┤
│ Jane Doe    │ jane@example.com │ Manager  │ [Resend] [Cancel]            │
│ [Pending]   │                  │          │ (spinner per button while    │
│             │                  │          │  in-flight)                  │
├─────────────┼──────────────────┼──────────┼──────────────────────────────┤
│ Bob Smith   │ bob@example.com  │ Manager  │ [Resend] [Cancel]            │
│ [Expired]   │                  │          │                              │
└─────────────┴──────────────────┴──────────┴──────────────────────────────┘
```

- `isInvitationExpired(member)` compares `invitationTokenExpiry` against `Date.now()`.
- Cancel loading uses `Set<string>` (not a single string) so cancelling multiple rows concurrently tracks each independently.

### `AcceptInvitePage` Flow

```text
URL: /accept-invite?token=<plain>

1. Extract token from search params
        │
        ├── no token  →  show "Invalid link" error
        │
2. GET /api/invite/validate?token=…
        │
        ├── 404/409   →  show "expired or already used" error
        │
        └── 200       →  show name + email greeting
                               │
                        3. Password form
                           (min 8 chars, confirm field)
                               │
                        4. POST /api/invite/accept
                               │
                               ├── error  →  show inline error
                               │
                               └── 200   →  "Welcome!" message
                                              → redirect to /login
```

### TanStack Query Hooks

All mutation hooks call `queryClient.invalidateQueries(['teamMembers'])` on success, keeping the table fresh without a manual refresh.

| Hook | Query key | Notes |
|------|-----------|-------|
| `useTeamMembers()` | `['teamMembers']` | Stale-while-revalidate |
| `useInviteTeamMember()` | — | Invalidates teamMembers |
| `useResendInvitation()` | — | Invalidates teamMembers |
| `useCancelInvitation()` | — | Invalidates teamMembers |
| `useUpdateTeamMember()` | — | Invalidates teamMembers |

---

## Data Flow

### Invite (Admin → System)

```text
Admin clicks "Send Invite"
    │
    ▼
InviteDialog  →  useInviteTeamMember()
    │
    ▼
POST /api/settings/team/invite
    { email, firstName, lastName, role }
    │
    ▼ InviteTeamMemberHandler
    ├─ validate input (FluentValidation)
    ├─ check email uniqueness
    ├─ generate (plainToken, tokenHash, expiry)
    ├─ INSERT users row:
    │    invitation_status = 1 (Pending)
    │    invitation_token  = SHA-256(plainToken)
    │    is_active         = false
    │    password_hash     = null
    └─ return { userId, inviteLink }
    │
    ▼
InviteDialog step 2: display inviteLink
Admin copies → sends to invitee out-of-band
```

### Accept (Invitee → System)

```text
Invitee opens /accept-invite?token=<plainToken>
    │
    ▼
GET /api/invite/validate?token=<plainToken>
    └─ validates token, returns { email, fullName }
    │
    ▼  (invitee sets password)
POST /api/invite/accept
    { token: <plainToken>, password: <new> }
    │
    ▼ AcceptInvitationHandler
    ├─ hash(plainToken)  →  tokenHash
    ├─ GetByInvitationTokenHashAsync(tokenHash)
    ├─ status / expiry checks
    ├─ hash(password)  →  passwordHash
    └─ AcceptInvitationAtomicAsync()
         UPDATE users
            SET password_hash     = @passwordHash
              , is_active         = true
              , invitation_status = 2 (Accepted)
              , invitation_token  = NULL
              , invitation_token_expiry = NULL
          WHERE invitation_token        = @tokenHash
            AND invitation_status       = 1 (Pending)
            AND invitation_token_expiry > @now
            AND is_deleted              = false
    │
    └─ rowsAffected = 1  →  200 { email, fullName }
       rowsAffected = 0  →  409 (concurrent request won)
    │
    ▼
AcceptInvitePage: redirect to /login
```

### Cancel (Admin → System)

```text
Admin clicks "Cancel" → CancelInviteDialog confirm
    │
    ▼
DELETE /api/settings/team/{userId}/invite
    │
    ▼ CancelInvitationHandler
    ├─ GetByIdAsync(userId)  →  404 if not found
    ├─ org mismatch          →  404
    ├─ status != Pending     →  409
    ├─ user.CancelInvitation()
    │    sets: is_deleted = true, invitation_status = 0,
    │          invitation_token = null
    └─ SaveChangesAsync
    │
    ▼
TanStack Query invalidates teamMembers → row disappears
```

---

## Database Schema

### New Columns on `users`

| Column | Type | Nullable | Default | Notes |
|--------|------|----------|---------|-------|
| `invitation_status` | `integer` | No | `0` | 0=None, 1=Pending, 2=Accepted |
| `invitation_token` | `varchar(500)` | Yes | null | HMAC-SHA256 hash of plain token |
| `invitation_token_expiry` | `timestamptz` | Yes | null | UTC expiry |

### Indexes

```sql
-- Unique; filtered so NULLs and soft-deleted rows are excluded
CREATE UNIQUE INDEX ix_users_invitation_token
    ON users (invitation_token)
    WHERE invitation_token IS NOT NULL
      AND is_deleted = false;
```

Token lookups in the validate and accept flows hit this index directly — O(1) regardless of user table size.

### Check Constraint

```sql
-- chk_user_has_auth_credential
-- Every active user must have at least one credential.
-- Pending invitees and soft-deleted users are exempt.
CHECK (
    is_deleted = TRUE
    OR (password_hash    IS NOT NULL AND LENGTH(TRIM(password_hash))    > 0)
    OR (google_id        IS NOT NULL AND LENGTH(TRIM(google_id))        > 0)
    OR (invitation_token IS NOT NULL AND LENGTH(TRIM(invitation_token)) > 0)
)
```

### Migrations

| Migration | What it does |
|-----------|-------------|
| `20260303144230_AddInvitationFieldsToUser` | Adds the three invitation columns |
| `20260303152227_UpdateAuthCredentialConstraintForInvitations` | Extends credential check to allow pending invitees |
| `20260304144930_AllowDeletedUsersWithoutCredentials` | Adds `is_deleted = TRUE` exemption to the constraint |
| `20260305041455_MakeInvitationTokenIndexUnique` | Converts the token index to filtered-unique |
| `20260305075447_FixCredentialConstraintForSoftDeletedUsers` | Idempotent re-apply of the soft-delete exemption |

---

## Security & Edge Cases

### Token Security

- Plain tokens are **never stored**; only the HMAC-SHA256 hash reaches the database.
- Tokens are 32 bytes of CSPRNG output (256-bit entropy) — brute force is not feasible.
- Accepting a token immediately NULLs `invitation_token` in the DB (one-time use).

### Race Condition — Concurrent Acceptance

Two browser tabs (or network retries) sending the same token simultaneously both pass the pre-read status check. The atomic `ExecuteUpdateAsync` WHERE clause is the serialisation point: PostgreSQL row-level locking means only one UPDATE sees `rowsAffected = 1`. The loser gets a 409.

### Cross-Organisation Opacity

Cancel and Resend return **404** (not 403) when the target user belongs to a different organisation. Returning 403 would confirm that the userId exists somewhere in the system.

### Role Validation — Numeric Enum Bypass

`Enum.TryParse<UserRole>("999")` succeeds (parses to unnamed value 999). The validator explicitly calls `Enum.IsDefined` after `TryParse`, then whitelists `Admin` and `Manager`. Plain numeric strings are rejected at the validation layer before the handler runs.

### Soft-Delete & DB Constraint

Cancelling a pending invitation sets `is_deleted = true` and nulls all credential fields. The DB check constraint exempts `is_deleted = TRUE` rows so the save never fails on missing credentials.

### Email PII in Logs

Accepted invitation logs contain only `UserId`. Email is not logged at any level in the auth/invitation flow.

---

## Configuration

| Key | Location | Default | Description |
|-----|----------|---------|-------------|
| `AuthSettings:InvitationTokenExpiryDays` | `appsettings.json` | `7` | Days until a pending invitation expires |
| `Frontend:BaseUrl` | `appsettings.json` | `http://localhost:5173` | Prepended to `/accept-invite?token=…` |
| `VITE_API_BASE_URL` | `.env` | `/api` | Frontend → backend base URL |

---

## API Reference

### Admin Endpoints — `[Authorize(Policy = "AdminOnly")]`

| Method | Path | Handler | Success |
|--------|------|---------|---------|
| `POST` | `/api/settings/team/invite` | `InviteTeamMemberHandler` | `201 { userId, inviteLink }` |
| `POST` | `/api/settings/team/{id}/resend-invite` | `ResendInvitationHandler` | `200 { inviteLink }` |
| `DELETE` | `/api/settings/team/{id}/invite` | `CancelInvitationHandler` | `200 { userId }` |

**POST /api/settings/team/invite — request body:**

```json
{
  "email": "jane@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "role": "Manager"
}
```

**Error responses (shared across admin endpoints):**

| Status | Meaning |
|--------|---------|
| `400` | Validation failure (missing/invalid fields) |
| `404` | User not found (or cross-org opacity) |
| `409` | Email already exists / invitation not pending |

---

### Public Endpoints — No Auth Required

| Method | Path | Handler | Success |
|--------|------|---------|---------|
| `GET` | `/api/invite/validate?token=…` | `ValidateInvitationTokenHandler` | `200 { email, fullName }` |
| `POST` | `/api/invite/accept` | `AcceptInvitationHandler` | `200 { email, fullName }` |

**POST /api/invite/accept — request body:**

```json
{
  "token": "<plain token from URL>",
  "password": "SecurePassword123"
}
```

**Error responses:**

| Status | Meaning |
|--------|---------|
| `400` | Password too short (< 8 chars) or token missing |
| `404` | Token not found |
| `409` | Already accepted / cancelled / expired / concurrent race |
