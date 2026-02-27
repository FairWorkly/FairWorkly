# Agent Service Architecture

A beginner-friendly guide to how the Agent Service works, from a user clicking "Send" to the LLM response appearing in the chat.

## Table of Contents

1. [Big Picture](#big-picture)
2. [Directory Structure](#directory-structure)
3. [Request Lifecycle](#request-lifecycle)
4. [Intent Routing](#intent-routing)
5. [Features](#features)
6. [LLM Providers](#llm-providers)
7. [RAG Pipeline (Compliance Q&A)](#rag-pipeline)
8. [Conversation History](#conversation-history)
9. [Follow-up Question Detection](#follow-up-question-detection)
10. [Security](#security)
11. [Configuration](#configuration)
12. [Design Decisions & Trade-offs](#design-decisions--trade-offs)

---

## Big Picture

```
┌──────────────────────────────────────────────────────────────┐
│  User types a message in FairBot chat                        │
└──────────────┬───────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────┐
│  Frontend (React)                                            │
│  useFairBotChat.ts → fairbotApi.ts                           │
│  Builds FormData: message + intentHint + contextPayload      │
│                  + historyPayload + conversationId            │
│  POST /api/fairbot/chat (JWT Bearer)                         │
└──────────────┬───────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────┐
│  Backend (.NET)                                              │
│  FairBotController → SendChatHandler                         │
│  Validates + enriches (roster context from DB if needed)     │
│  Validates history payload (format, size, message count)     │
│  POST /api/agent/chat (X-Service-Key)                        │
└──────────────┬───────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────┐
│  Agent Service (FastAPI)                     ← YOU ARE HERE  │
│  main.py → IntentRouter → Feature → LLM Provider            │
│  Returns JSON: { status, result: { message, model, ... } }  │
└──────────────┬───────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────┐
│  OpenAI API  or  Anthropic API                               │
│  Processes prompt → returns natural language answer           │
└──────────────────────────────────────────────────────────────┘
```

**Key insight**: The Agent Service is stateless. Every request is independent. The frontend sends everything the agent needs (message, context, history) in a single request.

---

## Directory Structure

```
agent-service/
├── master_agent/                  # Core framework
│   ├── main.py                    # FastAPI app, /api/agent/chat endpoint
│   ├── intent_router.py           # Routes requests to the right feature
│   ├── feature_registry.py        # FeatureBase class + feature_response() helper
│   └── config.py                  # Loads config.yaml
│
├── agents/                        # Domain features (one folder per domain)
│   ├── compliance/
│   │   ├── feature.py             # ComplianceFeature (RAG-powered Q&A)
│   │   └── prompt_builder.py      # System prompt for compliance questions
│   ├── roster/
│   │   ├── feature.py             # RosterFeature (Excel file parsing)
│   │   ├── explain_feature.py     # RosterExplainFeature (explain validation results)
│   │   └── services/              # Roster import logic (Excel parsing, validation)
│   └── payroll/
│       └── feature.py             # DemoPayrollFeature (stub, not yet implemented)
│
├── shared/                        # Shared utilities
│   ├── llm/
│   │   ├── provider_base.py       # Abstract base: LLMProviderBase
│   │   ├── langchain_base.py      # Shared LangChain logic: LangChainProviderBase
│   │   ├── openai_provider.py     # OpenAIProvider (ChatOpenAI wrapper)
│   │   ├── anthropic_provider.py  # AnthropicProvider (ChatAnthropic wrapper)
│   │   ├── factory.py             # LLMProviderFactory.create("openai"|"anthropic")
│   │   ├── history_utils.py       # normalize_chat_history() for multi-turn
│   │   └── embeddings_factory.py  # Embedding model for RAG
│   ├── rag/
│   │   ├── rag_client.py          # RAG pipeline: retrieve docs → augment prompt → call LLM
│   │   └── retriever_manager.py   # FAISS vector store management
│   └── prompt_builder_base.py     # Abstract base for prompt builders
│
├── config.yaml                    # Feature config, model params, paths
├── tests/                         # pytest tests
└── Dockerfile
```

**Why this structure?** Features are organized by domain (`compliance/`, `roster/`, `payroll/`) instead of by layer. This makes it easy to find all code related to one business domain in one place.

---

## Request Lifecycle

Here's what happens step-by-step when a request hits `/api/agent/chat`:

```
main.py: chat()
    │
    ├─ 1. Verify X-Service-Key (reject 401 if invalid)
    ├─ 2. Enforce rate limit (reject 429 if exceeded)
    ├─ 3. Parse form fields: message, file, intent_hint, context_payload,
    │                         history_payload, conversation_id
    │
    ├─ 4. IntentRouter.route(message, file_name, intent_hint)
    │      → Returns a feature_type string (e.g. "compliance_qa")
    │
    ├─ 5. FeatureRegistry.get_feature(feature_type)
    │      → Returns the Feature instance
    │
    ├─ 6. feature.process(payload)
    │      → Feature does its work (build prompt, call LLM, etc.)
    │      → Returns a dict with { type, message, model, sources, note }
    │
    └─ 7. Return JSON response:
           {
             "status": "success",
             "request_id": "...",
             "routed_to": "compliance_qa",
             "result": { ...feature output... }
           }
```

---

## Intent Routing

**File**: `master_agent/intent_router.py`

The IntentRouter decides which feature handles a request. It uses a simple priority:

```
1. If intent_hint is provided → look up in hint map
2. If a file is uploaded → "roster" (file parsing)
3. Otherwise → "compliance_qa" (default)
```

### Hint Map

| intent_hint value | Feature type | Feature class |
|---|---|---|
| `roster_explain` | `roster_explain` | RosterExplainFeature |
| `roster` (no file) | `roster_explain` | RosterExplainFeature |
| `roster` (with file) | `roster` | RosterFeature |
| `payroll` | `payroll_verify` | DemoPayrollFeature |
| `payroll_explain` | `payroll_verify` | DemoPayrollFeature |
| `compliance` | `compliance_qa` | ComplianceFeature |
| `compliance_qa` | `compliance_qa` | ComplianceFeature |
| _(none, no file)_ | `compliance_qa` | ComplianceFeature |

**Design decision**: The router uses a dict lookup (no keyword guessing from message text). The frontend always sends an explicit `intent_hint` when it knows the context. When no hint is provided, default to compliance Q&A which is the safest fallback.

---

## Features

### 1. ComplianceFeature (compliance Q&A)

**File**: `agents/compliance/feature.py`
**Triggered by**: Default (no intent hint), or `intent_hint="compliance"`

Uses RAG (Retrieval-Augmented Generation) to answer questions about Australian Fair Work regulations.

```
User: "What is the Saturday penalty rate for retail?"
                │
                ▼
CompliancePromptBuilder.build_system_prompt()
  → "You are FairWorkly's compliance assistant specialising in
     Australian Fair Work regulations..."
                │
                ▼
rag_client.run(message, system_prompt, config)
  → 1. Search FAISS vector store for relevant AWARD.pdf chunks
  → 2. Augment system prompt with retrieved documents
  → 3. Call LLM with [system + docs, user question]
  → 4. Return answer with source citations
```

**Knowledge source**: FAISS vector store built from AWARD.pdf documents.
**LLM config**: temperature=0.3, max_tokens=800, top_k=5 documents retrieved.

### 2. RosterExplainFeature (explain validation results)

**File**: `agents/roster/explain_feature.py`
**Triggered by**: `intent_hint="roster_explain"` (from roster results page)

Explains roster validation results in plain language. Does NOT re-run compliance checks. Also generates a structured **action plan** with prioritized fix actions.

```
User: "Why are there 5 critical issues?"
  + context_payload: { validation: { issues: [...], totalIssues: 12, ... } }
                │
                ▼
1. Extract validation data from context_payload
2. Detect if this is a follow-up question
3. Build prompt context (full trimmed vs lightweight for follow-ups)
4. Build prompt with appropriate response contract
5. Include conversation history (if any)
6. Call LLM: [system, ...history, user+context]
7. Build action plan from validation issues (structured data, not LLM-generated)
8. Return LLM answer + action plan in response
```

**Knowledge source**: Validation data injected from the frontend/backend (not from vector store).
**LLM config**: temperature=0.2, max_tokens=800.
**Response**: Includes both `message` (LLM text) and `data.action_plan` (structured JSON). See [Action Plans](#action-plans).

### 3. RosterFeature (file upload + parsing)

**File**: `agents/roster/feature.py`
**Triggered by**: `intent_hint="roster"` with an uploaded `.xlsx` file

Parses an Excel roster file into structured shift data. No LLM involved.

```
User uploads: roster.xlsx
                │
                ▼
1. Save to temp file
2. RosterExcelParser.parse_roster_excel(file_path, mode=LENIENT)
3. Return ParseResponse { result, issues, summary }
4. Clean up temp file
```

**Response format**: Returns `model_dump()` fields at the top level (not nested in `data`). This is because the .NET backend's `AgentChatResponse.Result` maps directly to `ParseResponse` which expects `result`, `issues`, `summary` as top-level keys.

### 4. DemoPayrollFeature (stub)

**File**: `agents/payroll/feature.py`
**Triggered by**: `intent_hint="payroll"` or `"payroll_explain"`

Returns a "not yet available" message. Placeholder for future payroll verification.

**Note**: If a user types a payroll-related compliance question in FairBot chat WITHOUT clicking a specific payroll action, it routes to ComplianceFeature (the default) and CAN be answered if the AWARD.pdf covers the topic.

---

## LLM Providers

**Directory**: `shared/llm/`

### Class Hierarchy

```
LLMProviderBase (abstract)           ← provider_base.py
    │
    └── LangChainProviderBase        ← langchain_base.py
            │    Shared: generate(), count_tokens(),
            │            _to_langchain_messages()
            │
            ├── OpenAIProvider        ← openai_provider.py
            │     __init__: ChatOpenAI(model, api_key, timeout)
            │
            └── AnthropicProvider     ← anthropic_provider.py
                  __init__: ChatAnthropic(model, api_key, timeout)
```

### Why this design?

`OpenAIProvider` and `AnthropicProvider` had 95% identical `generate()` logic (convert messages → call LangChain → timeout handling → format response). The only difference was `__init__` (which LangChain chat model to create). So we extracted the shared logic into `LangChainProviderBase`.

### Factory

```python
# Usage:
llm = LLMProviderFactory.create(provider_type="openai")   # → OpenAIProvider
llm = LLMProviderFactory.create(provider_type="anthropic") # → AnthropicProvider

# Then:
response = await llm.generate(messages, temperature=0.3, max_tokens=800)
# Returns: { "content": "...", "model": "gpt-4o-mini", "usage": {...} }
```

### Which feature uses which provider?

Configured in `config.yaml`:

| Feature | Provider | Model |
|---|---|---|
| compliance_qa | openai | gpt-4o-mini |
| roster (explain) | openai | gpt-4o-mini |
| payroll | anthropic | claude-sonnet-4-20250514 |

---

## RAG Pipeline

**File**: `shared/rag/rag_client.py`

RAG = Retrieval-Augmented Generation. Used only by ComplianceFeature.

```
User question: "What are penalty rates for Sunday work?"
        │
        ▼
┌─ 1. RETRIEVE ──────────────────────────────────────────────┐
│  FAISS vector store (built from AWARD.pdf)                  │
│  → Embed the question using OpenAI embeddings               │
│  → Search for top 5 most similar document chunks             │
│  → Return: document text + metadata (source, page)           │
└─────────────────────────────────────────────────────────────┘
        │
        ▼
┌─ 2. AUGMENT ───────────────────────────────────────────────┐
│  System prompt = base compliance prompt                      │
│                + "\n\nRelevant Documents:\n"                  │
│                + retrieved document chunks                    │
└─────────────────────────────────────────────────────────────┘
        │
        ▼
┌─ 3. GENERATE ──────────────────────────────────────────────┐
│  LLM call with:                                              │
│    messages = [                                              │
│      { role: "system", content: augmented_system_prompt },   │
│      { role: "user",   content: user_question }              │
│    ]                                                         │
│  temperature=0.3, max_tokens=800                             │
└─────────────────────────────────────────────────────────────┘
        │
        ▼
  Answer + source citations
```

### Fallbacks

| Scenario | Behavior |
|---|---|
| Vector store not found | Skip retrieval, LLM answers from its own knowledge |
| Retrieval fails | Skip retrieval, LLM answers from its own knowledge |
| LLM provider not configured | Return fallback message with configuration instructions |
| LLM call fails | Return fallback message with error reason |

---

## Conversation History

### The Problem

FairBot was originally stateless. Each message was independent. So when a user asked:

```
User: "Explain the results"
Bot:  "There are 5 issues... Next steps: 1) Fix breaks 2) Review overtime"

User: "Tell me more about the next steps"
Bot:  [doesn't know what "next steps" it mentioned before]
      [generates a completely different answer]
```

### The Solution

The frontend now sends previous messages as `history_payload` alongside each new message.

### Data Flow

```
Frontend (useFairBotChat.ts)
  │  toHistoryPayload(messages)
  │  → Filter out welcome message
  │  → Trim each message to 1,200 chars
  │  → Keep last 12 messages
  │  → Format: [{ role: "user"|"assistant", content: "..." }, ...]
  │
  ▼
Backend (SendChatHandler.cs)
  │  TryNormalizeHistoryPayload()
  │  → Validate JSON array structure
  │  → Validate each item has role + content
  │  → Truncate content to 2,000 chars
  │  → Keep last 20 messages
  │  → Reject if payload > 125 KB
  │
  ▼
Agent Service (main.py → explain_feature.py)
  │  normalize_chat_history()
  │  → Validate role ∈ {"user", "assistant"}
  │  → Truncate content to 1,200 chars
  │  → Keep last 12 messages
  │
  ▼
LLM call:
  messages = [
    { role: "system",    content: system_prompt },
    { role: "user",      content: "Explain the results" },       ← history
    { role: "assistant", content: "There are 5 issues..." },     ← history
    { role: "user",      content: user_prompt_with_context },    ← current
  ]
```

### Validation Limits (Defense in Depth)

Each layer re-validates, with slightly different limits:

| Layer | Max Messages | Max Content Chars | Max Payload Size |
|---|---|---|---|
| Frontend | 12 | 1,200 | — |
| Backend (.NET) | 20 | 2,000 | 125 KB |
| Agent Service | 12 | 1,200 | — |

Backend limits are intentionally wider than frontend limits. This prevents a bypassed frontend from crashing the system, while still allowing legitimate requests through.

### Which features use history?

| Feature | Uses History? | Why |
|---|---|---|
| RosterExplainFeature | Yes | Multi-turn explanation of validation results |
| ComplianceFeature | No | Uses RAG pipeline which constructs its own message array |
| RosterFeature | No | File parsing, no conversation |
| DemoPayrollFeature | No | Stub |

### conversation_id

The frontend generates a UUID per conversation session and sends it as `conversation_id`. Currently no feature reads it — it's forward-compatible infrastructure for future persistent conversation threads.

---

## Follow-up Question Detection

**File**: `agents/roster/explain_feature.py` → `_is_follow_up_question()`

When the user asks a follow-up, the feature adjusts its behavior. Only triggers when conversation history is present (first message is never a follow-up).

### Detection Logic

```python
# Prerequisite: must have history (no history = not a follow-up)
if not history_messages:
    return False

# Step 1: Check for explicit markers in the question text
follow_up_markers = ["more", "detail", "explain more", "why", "how",
                     "next step", "elaborate", "clarify", "further", ...]
if any(marker in text for marker in follow_up_markers):
    return True  # e.g. "Tell me more about the breaks"

# Step 2: Quick-follow-up button prompts (generated by action plan cards)
if text.startswith("for ") and ("shift-level edits" in text or
                                 "execution checklist" in text or
                                 "validation checks" in text):
    return True  # e.g. "For Fix meal break scheduling, provide concrete shift-level edits..."

# Step 3: Short referential questions
if len(text) <= 48:
    if any(term in text for term in ["that", "this", "it", "those", "them"]):
        return True  # e.g. "What about those?"
```

### What Changes for Follow-ups

| Aspect | First Question | Follow-up |
|---|---|---|
| **Context sent to LLM** | Full trimmed context (totals, status, 25 issues) | Lightweight context (just issues for action) |
| **Response contract** | "Concise explanation (max 4 lines) + bridge to action cards" | "Continue from prior turns. Focus on what user asked. Don't repeat summary." |
| **System prompt addition** | — | "For follow-up questions, continue from conversation history..." |
| **Action plan** | Included (structured data) | Still included (UI continuity) |

This prevents the LLM from repeating the full validation summary on every follow-up.

---

## Action Plans

**File**: `agents/roster/explain_feature.py` → `_build_action_plan()`

Action plans are **structured data** (not LLM-generated text). They provide prioritized, actionable fix recommendations based on the validation issues.

### How It Works

```
Validation issues (from context_payload)
        │
        ▼
1. Group issues by check type (e.g. "mealbreak", "restperiod", "weeklyhourslimit")
2. Score each group: severity_weight × 2 + 1 per issue
   (error=3, warning=2, info=1)
3. Sort groups by score (highest = most critical)
4. Take top 3 groups
5. Map each group to a template from _CHECK_TEMPLATES
6. Generate quick-follow-up prompts from _FOLLOW_UP_TEMPLATES
```

### Check Templates

Pre-defined action templates for known compliance check types:

| Check Type | Title | Example Action |
|---|---|---|
| `consecutivedays` | Limit consecutive work days | Insert rest day, rebalance shifts |
| `mealbreak` | Fix meal break scheduling | Update shift templates with compliant break timing |
| `weeklyhourslimit` | Rebalance weekly hours | Reduce hours for over-limit employees |
| `minimumshifthours` | Adjust short shifts to minimum | Extend or merge short shifts |
| `restperiod` | Enforce minimum rest between shifts | Move start times or reassign shifts |
| `dataquality` | Correct roster data integrity | Fix inconsistent data, validate fields |
| `default` | Resolve remaining compliance issues | Review flagged shifts, apply edits |

Each template includes: `what_to_change`, `why`, `expected_outcome`, `risk_if_ignored`.

### Response Shape

```json
{
  "type": "roster_explain",
  "message": "Your roster has 5 critical issues...",     // LLM-generated text
  "model": "gpt-4o-mini",
  "data": {
    "action_plan": {
      "title": "Top 3 actions to fix this roster",
      "validation_id": "...",
      "actions": [
        {
          "id": "action_1",
          "priority": "P1",
          "title": "Fix meal break scheduling",
          "owner": "Roster manager",
          "check_type": "MealBreak",
          "issue_count": 8,
          "critical_count": 3,
          "affected_shifts": [
            { "employee": "Jane Smith", "dates": "2026-02-23", "description": "..." }
          ],
          "what_to_change": "Update shift templates to include compliant meal break...",
          "why": "Missing or short meal breaks are direct award non-compliance...",
          "expected_outcome": "Meal-break errors are removed...",
          "risk_if_ignored": "Ongoing meal-break breaches can lead to..."
        }
      ],
      "quick_follow_ups": [
        {
          "id": "follow_up_action_1",
          "label": "P1 Shift edits",
          "prompt": "For Fix meal break scheduling, provide concrete shift-level edits...",
          "action_id": "action_1"
        }
      ]
    }
  }
}
```

### Quick Follow-up Buttons

Each action generates follow-up prompts that the frontend can render as clickable buttons:

| Template | Purpose |
|---|---|
| **Shift edits** | "Provide concrete shift-level edits: before → after per employee/date" |
| **Manager checklist** | "Create an execution checklist with sequencing, owner, completion criteria" |
| **Validation checks** | "List exact validation checks to run after changes, with pass criteria" |

When clicked, these prompts are sent as the next message. The follow-up detector recognizes them (Step 2 above) and adjusts the response accordingly.

### Fallback (No Issues Array)

When `validation.issues` is empty or missing but `totalIssues` > 0, the action plan generates 3 generic actions: "Prioritize critical breaches" → "Adjust breaks/hours/rest" → "Re-run validation".

---

## Security

### Authentication

```
Frontend → Backend:  JWT Bearer token (user identity)
Backend → Agent:     X-Service-Key header (service-to-service)
```

The Agent Service does NOT validate JWTs. It trusts the backend's `X-Service-Key`. This keeps auth logic in one place (the .NET backend).

### Rate Limiting

**File**: `main.py` → `_enforce_rate_limit()`

- Per-client sliding window: 60 requests per 60 seconds (configurable)
- Client identified by `X-Forwarded-For` header or direct IP
- Stale client buckets evicted when tracked clients > 10,000

### Request Size Limit

**File**: `main.py` → `enforce_request_size_limit()` middleware

- Max 50 MB per request (configurable via `MAX_REQUEST_BYTES`)
- Only enforced on `POST /api/agent/chat`
- Checked via `Content-Length` header before reading body

### Payload Limits (Backend)

| Payload | Max Size | Notes |
|---|---|---|
| context_payload | 500 KB | Roster validation data can be large |
| history_payload | 125 KB | Conversation history |
| conversation_id | 128 chars | Alphanumeric + hyphens only (regex validated) |

---

## Configuration

**File**: `config.yaml`

```yaml
faiss:
  similarity_search_k_docs: 5       # Number of document chunks to retrieve
  similarity_search_k_feedback: 1

model_params:
  deployment_mode_embedding: online  # local or online
  deployment_mode_llm: online
  online_model_name: gpt-4o-mini           # Default OpenAI model
  anthropic_model_name: claude-sonnet-4-20250514  # Anthropic model
  temperature: 0.4

features:
  compliance_qa:
    llm_provider: openai             # Which provider for compliance Q&A
  roster:
    llm_provider: openai             # Which provider for roster explain
  payroll:
    llm_provider: anthropic          # Which provider for payroll
```

### Environment Variables

| Variable | Default | Purpose |
|---|---|---|
| `OPENAI_API_KEY` | — | Required for OpenAI provider |
| `ANTHROPIC_API_KEY` | — | Required for Anthropic provider |
| `AGENT_SERVICE_KEY` | `change-me-dev-agent-key` | Shared secret with backend |
| `ALLOWED_ORIGINS` | `http://localhost:5680` | CORS origins (comma-separated) |
| `MAX_REQUEST_BYTES` | `52428800` (50 MB) | Max request body size |
| `RATE_LIMIT_REQUESTS` | `60` | Requests per window |
| `RATE_LIMIT_WINDOW_SECONDS` | `60` | Window size in seconds |

---

## Design Decisions & Trade-offs

### 1. Stateless Agent (no server-side session)

**Decision**: Conversation history is sent from the frontend on every request.

**Why**: Simpler architecture. No need for Redis/database for sessions. Easy to scale horizontally. The trade-off is larger request payloads, but with 12 messages x 1,200 chars max, this is well under limits.

**Future**: `conversation_id` is already plumbed through for when we need server-side persistence (analytics, audit trail, cross-device continuity).

### 2. Domain-based Directory Structure

**Decision**: Features live in `agents/{domain}/` instead of `master_agent/features/`.

**Why**: All code for one business domain is in one place. When working on roster features, everything is in `agents/roster/`. Scales better as domains grow (each could eventually become its own service).

### 3. Response Format Compatibility

**Decision**: `RosterFeature` returns `model_dump()` fields at the top level, not nested in a `data` field.

**Why**: The .NET backend deserializes `AgentChatResponse.Result` directly as `ParseResponse`, which expects `result`, `issues`, `summary` at the top level. Nesting them in `data` would break deserialization. Other features use `feature_response()` which returns a standard shape with `type`, `message`, `model`, `sources`, `note`.

### 4. Compliance Prompt: Permissive over Restrictive

**Decision**: The compliance prompt says "Answer primarily based on provided context documents" instead of "Only answer based on...".

**Why**: With a restrictive prompt, the LLM would refuse to answer questions where the AWARD.pdf had partial but relevant coverage. Users got "I don't have enough information" for answerable questions. The current prompt encourages the LLM to synthesize what it can from the documents and note any gaps.

### 5. Follow-up Detection: Heuristic over LLM Classification

**Decision**: Use keyword matching to detect follow-up questions instead of asking the LLM.

**Why**: An LLM call to classify intent would add latency and cost to every request. The keyword heuristic is fast, free, and good enough for the common cases. False positives (treating a new question as follow-up) are harmless — the LLM still has the full context and can answer correctly.

### 6. No Conversation History for Compliance Q&A

**Decision**: Only RosterExplainFeature uses conversation history. ComplianceFeature does not.

**Why**: ComplianceFeature uses the RAG pipeline (`rag_client.run()`), which constructs its own `[system + retrieved_docs, user]` message array. Adding history would require changes to `rag_client` to inject history between system and user messages. This can be done in the future if needed.
