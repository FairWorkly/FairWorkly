# FairBot Call Chain

This document traces the complete request lifecycle for FairBot's two primary flows:
**Roster Explain** (contextual explanation of validation results) and **General Compliance Q&A** (RAG-powered regulatory guidance).

## Architecture Overview

```
Frontend (React)
    │  POST /api/fairbot/chat  (JWT Bearer)
    ▼
Backend (.NET)
    │  POST /api/agent/chat  (X-Service-Key)
    ▼
Agent Service (FastAPI)
    │  IntentRouter → Feature → LLM
    ▼
OpenAI / Anthropic API
```

All FairBot requests flow through the same three-layer proxy chain. The only differences are:
1. Whether the frontend attaches roster context
2. Which Agent Service feature handles the request
3. What knowledge source the LLM uses (DB validation data vs FAISS vector store)

---

## Flow 1: Roster Explain

Triggered when a user navigates to FairBot from a roster results page.

### Step-by-step

#### 1. Navigation (Frontend)

The "Ask FairBot to Explain" button on the roster results page navigates with URL params:

```
/fairbot?intent=roster&rosterId={GUID}&validationId={GUID}
```

- **File**: `frontend/src/modules/roster/pages/RosterResults.tsx` (role check + Button)
- Only visible to users with `admin` role

#### 2. Context Loading (Frontend)

`useFairBotChat` reads URL search params and loads the roster validation context via API call.

- **File**: `frontend/src/modules/fairbot/hooks/useFairBotChat.ts` (context-loading `useEffect`)

```typescript
const intent = searchParams.get('intent')       // 'roster'
const rosterId = searchParams.get('rosterId')   // UUID
const validationId = searchParams.get('validationId') // UUID
const isRosterExplainMode = intent === 'roster' // true

// useEffect calls getValidationResults(rosterId) and on success:
setContext({
  intentHint: 'roster_explain',
  rosterId,
  payload: {
    kind: 'roster_validation',
    rosterId,
    validation,   // full validation data from API
  },
})
```

**Fallback**: If `getValidationResults` fails (e.g., 404), the context falls back to a lightweight reference:

```typescript
setContext({
  intentHint: 'roster',
  rosterId,
  payload: { kind: 'roster_reference', rosterId, validationId },
})
```

A small info Chip ("Roster context loaded" or "Roster context unavailable") appears in the chat UI.

- **File**: `frontend/src/modules/fairbot/components/ChatSection.tsx`

#### 3. Send Message (Frontend)

When the user submits a message, `sendMessage` attaches the context (if available) and calls the API service.

- **File**: `frontend/src/modules/fairbot/hooks/useFairBotChat.ts` (`sendMessage` callback)

```typescript
const response = await sendChatMessage(
  trimmedText,
  context
    ? { intentHint: context.intentHint, contextPayload: context.payload }
    : undefined,
)
```

#### 4. API Call (Frontend)

`sendChatMessage` builds a `FormData` payload and POSTs to the backend.

- **File**: `frontend/src/services/fairbotApi.ts`

**Primary path** (full validation loaded successfully):
```
POST /api/fairbot/chat
Content-Type: multipart/form-data (browser-set boundary)
Authorization: Bearer <JWT>
X-Request-Id: <UUID>

FormData:
  message:        "Why is this shift failing?"
  intentHint:     "roster_explain"
  contextPayload: '{"kind":"roster_validation","rosterId":"...","validation":{...full data...}}'
```

**Fallback path** (validation API failed):
```
FormData:
  message:        "Why is this shift failing?"
  intentHint:     "roster"
  contextPayload: '{"kind":"roster_reference","rosterId":"...","validationId":"..."}'
```

Uses the shared `httpClient` (base URL `/api`, credentials included).

- **File**: `frontend/src/services/httpClient.ts`

#### 5. Controller Receives Request (Backend)

`FairBotController.Chat()` receives the multipart form.

- **File**: `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs`
- **Auth**: `[Authorize(Policy = "RequireAdmin")]` — JWT validated, user extracted

Parameters: `message`, `intentHint`, `contextPayload` (with snake_case legacy fallbacks).

#### 6. Backend Processing

**Primary path** (`intentHint == "roster_explain"`): The backend does NOT match this against the `"roster"` enrichment branch (case-insensitive check for `"roster"` only). The `contextPayload` is forwarded to the Agent Service as-is — no database query is needed since the frontend already embedded the full validation data.

**Fallback path** (`intentHint == "roster"`): The controller enriches the lightweight reference with full validation data from the database.

- **File**: `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs` (roster context enrichment block)

```csharp
// Only triggered when intentHint == "roster" (fallback path)
// 1. Parse contextPayload JSON → extract rosterId, validationId
// 2. Query database via MediatR:
//    GetValidationResultsQuery { RosterId, OrganizationId }
// 3. Verify validationId matches (catch stale context)
// 4. Build enriched payload:
{
  "kind": "roster_validation",
  "rosterId": "...",
  "validation": { ...full issue array... }
}
```

**Graceful fallback**: If context resolution fails (missing data, stale validation, no org context), the controller drops the `intent_hint` field entirely. The agent then routes to compliance Q&A instead of returning an error.

#### 7. Forward to Agent Service (Backend)

`PythonAiClient.PostFormAsync()` rebuilds the form fields as multipart and POSTs to the agent.

- **File**: `backend/src/FairWorkly.Infrastructure/AI/PythonServices/PythonAiClient.cs`

```
POST http://localhost:8000/api/agent/chat
Headers: X-Service-Key: <configured key>

MultipartFormData:
  message:         "Why is this shift failing?"
  intent_hint:     "roster_explain" (or "roster" in fallback)
  context_payload: '{"kind":"roster_validation",...}'
```

#### 8. Agent Entry Point

FastAPI endpoint receives the request. Service key is verified, rate limit is enforced.

- **File**: `agent-service/master_agent/main.py`

#### 9. Intent Routing (Agent)

`IntentRouter.route()` checks the explicit `intent_hint` first.

- **File**: `agent-service/master_agent/intent_router.py`

**Primary path**:
```python
hint = "roster_explain"
# Rule: hint == "roster_explain" → return "roster_explain"
return "roster_explain"
```

**Fallback path** (backend enriched with `intent_hint="roster"`):
```python
hint = "roster"
# Rule: hint == "roster" and no file → "roster_explain"
return "roster_explain"
```

Both paths end up at the same feature.

#### 10. RosterExplainFeature.process() (Agent)

- **File**: `agent-service/master_agent/features/roster_explain_feature.py`

```
a. Extract validation from context_payload["validation"]
b. Trim to 25 issues for token budget
c. Build system prompt:
   "You are FairWorkly's roster explanation assistant.
    Explain existing roster validation outcomes in plain language..."
d. Build user prompt:
   "User question:\n{message}\n\n
    Roster validation context (JSON):\n{trimmed_context}\n\n
    Please provide: 1) explanation 2) checks involved 3) next steps"
e. Create LLM provider via factory
f. Call LLM: temperature=0.2, max_tokens=800
```

#### 11. LLM Call (Agent)

`LLMProviderFactory.create()` instantiates the configured provider (OpenAI or Anthropic).

- **File**: `agent-service/shared/llm/factory.py`

The provider calls the external API and returns:

```python
{ "content": "LLM explanation text...", "model": "gpt-4o-mini", "usage": {} }
```

If the LLM call fails, a fallback summary is generated from the validation stats (totals, issue counts) with an error classification note.

#### 12. Response (Agent → Backend → Frontend)

```json
{
  "status": "success",
  "message": "Why is this shift failing?",
  "file_name": null,
  "routed_to": "roster_explain",
  "result": {
    "type": "roster_explain",
    "message": "Based on the validation results, this shift...",
    "model": "gpt-4o-mini",
    "note": null
  }
}
```

Backend wraps in `Result<FairBotChatResponse>.Of200()` and returns. Frontend `useFairBotChat` extracts `result.message` and renders as an assistant chat bubble with optional metadata (model name).

---

## Flow 2: General Compliance Q&A

Triggered when a user opens `/fairbot` directly (no URL params) and asks a question.

### Step-by-step

#### 1-4. Frontend (Simplified)

Same component tree, but:
- `isRosterExplainMode = false`, `context = null`
- No Chip displayed
- `sendChatMessage(text, undefined)` — no intent hint, no context payload

```
POST /api/fairbot/chat
FormData:
  message: "What is the Saturday penalty rate for retail?"
```

#### 5-6. Backend (Pass-through)

- `intentHint = null` → skip roster context enrichment entirely
- `formFields = { "message": "What is the Saturday penalty rate for retail?" }`
- Forward to agent as-is

#### 7. Intent Routing (Agent)

No explicit hint → fall through to keyword matching and default.

- **File**: `agent-service/master_agent/intent_router.py`

```python
# No intent_hint, no file_name
# Check keywords: ['penalty rate', 'overtime', 'award', 'break']
# "penalty rate" found in message → "compliance_qa"
# (Default is also "compliance_qa")
```

#### 8. ComplianceFeature.process() (Agent)

- **File**: `agent-service/master_agent/features/compliance_feature.py`

```
a. Build system prompt via CompliancePromptBuilder
   "You are FairWorkly's compliance assistant specialising in
    Australian Fair Work regulations..."
b. Call run_rag(message, system_prompt, config)
```

#### 9. RAG Pipeline (Agent)

- **File**: `agent-service/shared/rag/rag_client.py`

```
a. ensure_retriever() → load FAISS vector store
   Vector store built from AWARD.pdf (273 chunks)

b. retriever.retrieve(message, top_k=3)
   → Embed query via OpenAI embeddings
   → Search FAISS for 3 most similar document chunks
   ← docs_text = formatted context with source/page/content

c. Augment system prompt with retrieved documents
   rag_system_prompt = system_prompt + "\n\nRelevant Documents:\n" + docs_text

d. Call LLM with augmented prompt
   messages = [
     { role: "system", content: rag_system_prompt },
     { role: "user",   content: message }
   ]
   temperature=0.3, max_tokens=800
```

#### 10. Response

```json
{
  "status": "success",
  "message": "What is the Saturday penalty rate for retail?",
  "file_name": null,
  "routed_to": "compliance_qa",
  "result": {
    "type": "compliance",
    "message": "Under the General Retail Industry Award (MA000004), clause 25.5...",
    "model": "gpt-4o-mini",
    "sources": [
      { "source": "AWARD.pdf", "page": 45, "content": "Saturday penalty rates..." }
    ],
    "note": null
  }
}
```

Frontend renders the answer with source citations displayed below the message bubble.

---

## Key Differences Between Flows

| Aspect | Roster Explain | Compliance Q&A |
|--------|---------------|----------------|
| **Trigger** | Button on roster results page (URL params) | Direct navigation to `/fairbot` |
| **Frontend context** | `{intentHint: 'roster_explain', rosterId, payload}` | `null` |
| **Payload to backend** | `message + intentHint + contextPayload` | `message` only |
| **Backend processing** | Primary: pass-through (full data from frontend). Fallback: query DB, enrich. | Pass-through |
| **Backend fallback** | On enrichment failure → drop intent, route to compliance Q&A | N/A |
| **Agent routing** | `intent_hint="roster_explain"` → `roster_explain` | Keyword match / default → `compliance_qa` |
| **Knowledge source** | Roster validation data from DB (injected in user prompt) | FAISS vector store / AWARD.pdf (injected in system prompt) |
| **LLM temperature** | 0.2 (more deterministic) | 0.3 |
| **Response sources** | None | RAG document citations |
| **Error handling** | Fallback summary from validation stats | RAG unavailable note |

## Data Transformations

**Primary path** (frontend loads full validation successfully):
```
Frontend contextPayload (full data):
  { kind: "roster_validation", rosterId: "...", validation: { ...all fields + issues... } }
                    │
                    ▼  Backend passes through as-is (intentHint != "roster")
Agent context_payload:
  { kind: "roster_validation", rosterId: "...", validation: { ...all fields + issues... } }
                    │
                    ▼  Agent trims for token budget
Agent trimmed_context (LLM-ready):
  { validation_id, status, totals: {...}, issues_preview: [...max 25...] }
```

**Fallback path** (frontend validation API fails):
```
Frontend contextPayload (lightweight reference):
  { kind: "roster_reference", rosterId: "...", validationId: "..." }
                    │
                    ▼  Backend enriches from DB (intentHint == "roster")
Backend context_payload (full data):
  { kind: "roster_validation", rosterId: "...", validation: { ...all fields + issues... } }
                    │
                    ▼  Agent trims for token budget
Agent trimmed_context (LLM-ready):
  { validation_id, status, totals: {...}, issues_preview: [...max 25...] }
```
