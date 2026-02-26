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

- **File**: `frontend/src/modules/roster/pages/RosterResults.tsx:52`
- Only visible to users with `admin` role

#### 2. Context Loading (Frontend)

`useFairBotChat` reads URL search params and silently loads the roster context into state.

- **File**: `frontend/src/modules/fairbot/hooks/useFairBotChat.ts:93-133`

```typescript
const intent = searchParams.get('intent')       // 'roster'
const rosterId = searchParams.get('rosterId')   // UUID
const validationId = searchParams.get('validationId') // UUID
const isRosterExplainMode = intent === 'roster' // true

// useEffect sets context:
setContext({
  intentHint: 'roster',
  rosterId,
  payload: { kind: 'roster_reference', rosterId, validationId },
})
```

A small info Chip ("Roster context loaded") appears in the chat UI. No errors are shown if context is missing — the user can always send messages.

- **File**: `frontend/src/modules/fairbot/components/ChatSection.tsx:55-62`

#### 3. Send Message (Frontend)

When the user submits a message, `sendMessage` attaches the context (if available) and calls the API service.

- **File**: `frontend/src/modules/fairbot/hooks/useFairBotChat.ts:145-197`

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

- **File**: `frontend/src/services/fairbotApi.ts:31-54`

```
POST /api/fairbot/chat
Content-Type: multipart/form-data (browser-set boundary)
Timeout: 30s
Authorization: Bearer <JWT>

FormData:
  message:        "Why is this shift failing?"
  intentHint:     "roster"
  contextPayload: '{"kind":"roster_reference","rosterId":"...","validationId":"..."}'
```

Uses the shared `httpClient` (base URL `/api`, credentials included).

- **File**: `frontend/src/services/httpClient.ts`

#### 5. Controller Receives Request (Backend)

`FairBotController.Chat()` receives the multipart form.

- **File**: `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs:39-46`
- **Auth**: `[Authorize(Policy = "RequireAdmin")]` — JWT validated, user extracted

Parameters: `message`, `intentHint`, `contextPayload` (with snake_case legacy fallbacks).

#### 6. Roster Context Enrichment (Backend)

Because `intentHint == "roster"`, the controller enriches the lightweight frontend reference with full validation data from the database.

- **File**: `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs:69-97`

```csharp
// 1. Parse contextPayload JSON → extract rosterId, validationId
// 2. Query database via MediatR:
//    GetValidationResultsQuery { RosterId, OrganizationId }
// 3. Verify validationId matches (catch stale context)
// 4. Build enriched payload:
{
  "kind": "roster_validation",
  "rosterId": "...",
  "validation": {
    "validationId": "...",
    "status": "Failed",
    "totalShifts": 20,
    "failedShifts": 5,
    "totalIssues": 8,
    "criticalIssues": 2,
    "affectedEmployees": 3,
    "issues": [ ...full issue array... ]
  }
}
```

**Graceful fallback**: If context resolution fails (missing data, stale validation, no org context), the controller drops the `intent_hint` field entirely. The agent then routes to compliance Q&A instead of returning an error.

- **File**: `backend/src/FairWorkly.API/Controllers/FairBot/FairBotController.cs:84-96`

#### 7. Forward to Agent Service (Backend)

`PythonAiClient.PostFormAsync()` rebuilds the form fields as multipart and POSTs to the agent.

- **File**: `backend/src/FairWorkly.Infrastructure/AI/PythonServices/PythonAiClient.cs:98-124`

```
POST http://localhost:8000/api/agent/chat
Headers: X-Service-Key: <configured key>
Timeout: 60s

MultipartFormData:
  message:         "Why is this shift failing?"
  intent_hint:     "roster"
  context_payload: '{"kind":"roster_validation","rosterId":"...","validation":{...full data...}}'
```

#### 8. Agent Entry Point

FastAPI endpoint receives the request. Service key is verified, rate limit is enforced.

- **File**: `agent-service/master_agent/main.py:140-149`

#### 9. Intent Routing (Agent)

`IntentRouter.route()` checks the explicit `intent_hint` first.

- **File**: `agent-service/master_agent/intent_router.py:10-16`

```python
hint = "roster"
file_name = None
# Rule: hint == "roster" and no file → "roster_explain"
return "roster_explain"
```

#### 10. RosterExplainFeature.process() (Agent)

- **File**: `agent-service/master_agent/features/roster_explain_feature.py:129-196`

```
a. Extract validation from context_payload["validation"]        (line 138-148)
b. Trim to 25 issues for token budget                           (line 36-56)
c. Build system prompt:                                          (line 154-159)
   "You are FairWorkly's roster explanation assistant.
    Explain existing roster validation outcomes in plain language..."
d. Build user prompt:                                            (line 160-167)
   "User question:\n{message}\n\n
    Roster validation context (JSON):\n{trimmed_context}\n\n
    Please provide: 1) explanation 2) checks involved 3) next steps"
e. Create LLM provider via factory                               (line 170)
f. Call LLM: temperature=0.2, max_tokens=800                     (line 171-178)
```

#### 11. LLM Call (Agent)

`LLMProviderFactory.create()` instantiates the configured provider (OpenAI or Anthropic).

- **File**: `agent-service/shared/llm/factory.py:27-42`

The provider calls the external API and returns:

```python
{ "content": "LLM explanation text...", "model": "gpt-4o-mini", "usage": {} }
```

If the LLM call fails, a fallback summary is generated from the validation stats (totals, issue counts) with an error classification note.

- **File**: `agent-service/master_agent/features/roster_explain_feature.py:189-196`

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

- **File**: `agent-service/master_agent/intent_router.py:30-36`

```python
# No intent_hint, no file_name
# Check keywords: ['penalty rate', 'overtime', 'award', 'break']
# "penalty rate" found in message → "compliance_qa"
# (Default is also "compliance_qa")
```

#### 8. ComplianceFeature.process() (Agent)

- **File**: `agent-service/master_agent/features/compliance_feature.py:18-47`

```
a. Build system prompt via CompliancePromptBuilder                (line 31)
   "You are FairWorkly's compliance assistant specialising in
    Australian Fair Work regulations..."
b. Call run_rag(message, system_prompt, config)                   (line 33-38)
```

#### 9. RAG Pipeline (Agent)

- **File**: `agent-service/shared/rag/rag_client.py:23-84`

```
a. ensure_retriever() → load FAISS vector store                   (line 40-58)
   Vector store built from AWARD.pdf (273 chunks)

b. retriever.retrieve(message, top_k=3)                           (line 49)
   → Embed query via OpenAI embeddings
   → Search FAISS for 3 most similar document chunks
   ← docs_text = formatted context with source/page/content

c. Augment system prompt with retrieved documents                  (line 60-64)
   rag_system_prompt = system_prompt + "\n\nRelevant Documents:\n" + docs_text

d. Call LLM with augmented prompt                                  (line 70-78)
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
| **Frontend context** | `{intentHint, rosterId, payload}` | `null` |
| **Payload to backend** | `message + intentHint + contextPayload` | `message` only |
| **Backend processing** | Query DB, enrich with full validation data | Pass-through |
| **Backend fallback** | On failure → drop intent, route to compliance Q&A | N/A |
| **Agent routing** | `intent_hint="roster"` → `roster_explain` | Keyword match / default → `compliance_qa` |
| **Knowledge source** | Roster validation data from DB (injected in user prompt) | FAISS vector store / AWARD.pdf (injected in system prompt) |
| **LLM temperature** | 0.2 (more deterministic) | 0.3 |
| **Response sources** | None | RAG document citations |
| **Error handling** | Fallback summary from validation stats | RAG unavailable note |

## Data Transformations

```
Frontend contextPayload (lightweight reference):
  { kind: "roster_reference", rosterId: "...", validationId: "..." }
                    │
                    ▼  Backend enriches from DB
Backend context_payload (full data):
  { kind: "roster_validation", rosterId: "...", validation: { ...all fields + issues... } }
                    │
                    ▼  Agent trims for token budget
Agent trimmed_context (LLM-ready):
  { validation_id, status, totals: {...}, issues_preview: [...max 25...] }
```
