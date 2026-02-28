import asyncio
import json
import logging
import os
import time
import uuid
from collections import deque
from typing import Deque, Dict, Optional

from fastapi import Depends, FastAPI, File, Form, Header, HTTPException, Request, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import RedirectResponse
from starlette import status
from starlette.responses import JSONResponse

from master_agent.intent_router import IntentRouter
from master_agent.feature_registry import FeatureRegistry


# Import features from domain packages
from agents.compliance.feature import ComplianceFeature
from agents.payroll.feature import DemoPayrollFeature
from agents.roster.feature import RosterFeature
from agents.roster.explain_feature import RosterExplainFeature

app = FastAPI(title="FairWorkly Master Agent")

def _parse_allowed_origins(raw: str) -> list[str]:
    origins = [item.strip() for item in raw.split(",") if item.strip()]
    return origins or ["http://localhost:5680"]


ALLOWED_ORIGINS = _parse_allowed_origins(
    os.getenv("ALLOWED_ORIGINS", "http://localhost:5680")
)
AGENT_SERVICE_KEY = os.getenv("AGENT_SERVICE_KEY", "")
if not AGENT_SERVICE_KEY:
    raise RuntimeError(
        "AGENT_SERVICE_KEY env var is required. "
        "Export it in your shell or set it in docker-compose."
    )
MAX_REQUEST_BYTES = int(os.getenv("MAX_REQUEST_BYTES", "52428800"))
RATE_LIMIT_REQUESTS = int(os.getenv("RATE_LIMIT_REQUESTS", "60"))
RATE_LIMIT_WINDOW_SECONDS = int(os.getenv("RATE_LIMIT_WINDOW_SECONDS", "60"))

_RATE_LIMIT_LOCK = asyncio.Lock()
_RATE_LIMIT_BUCKETS: Dict[str, Deque[float]] = {}
_MAX_TRACKED_CLIENTS = 10_000
_LOGGER = logging.getLogger(__name__)

app.add_middleware(
    CORSMiddleware,
    allow_origins=ALLOWED_ORIGINS,
    allow_credentials=False,
    allow_methods=["POST", "GET"],
    allow_headers=["*"],
)


def verify_service_key(
    x_service_key: Optional[str] = Header(default=None, alias="X-Service-Key")
) -> None:
    if x_service_key != AGENT_SERVICE_KEY:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid service key",
        )


def _resolve_client_id(request: Request) -> str:
    # Use direct connection IP only.  X-Forwarded-For is trivially spoofable
    # and this service sits behind the .NET backend (authenticated via service
    # key), so the caller IP is the backend instance — which is the correct
    # granularity for service-to-service rate limiting.
    return request.client.host if request.client else "unknown"


def _resolve_request_id(request: Request) -> str:
    request_id = request.headers.get("x-request-id")
    if request_id and request_id.strip():
        return request_id.strip()
    return str(uuid.uuid4())


async def _enforce_rate_limit(client_id: str) -> None:
    now = time.monotonic()
    lower_bound = now - RATE_LIMIT_WINDOW_SECONDS

    async with _RATE_LIMIT_LOCK:
        # Evict stale clients to prevent unbounded memory growth
        if len(_RATE_LIMIT_BUCKETS) > _MAX_TRACKED_CLIENTS:
            stale = [k for k, v in _RATE_LIMIT_BUCKETS.items() if not v or v[-1] <= lower_bound]
            for k in stale:
                del _RATE_LIMIT_BUCKETS[k]

        # If still over cap after eviction, reject unknown clients
        if client_id not in _RATE_LIMIT_BUCKETS and len(_RATE_LIMIT_BUCKETS) >= _MAX_TRACKED_CLIENTS:
            raise HTTPException(
                status_code=status.HTTP_429_TOO_MANY_REQUESTS,
                detail="Too many tracked clients, please retry later",
            )

        bucket = _RATE_LIMIT_BUCKETS.setdefault(client_id, deque())
        while bucket and bucket[0] <= lower_bound:
            bucket.popleft()

        if len(bucket) >= RATE_LIMIT_REQUESTS:
            raise HTTPException(
                status_code=status.HTTP_429_TOO_MANY_REQUESTS,
                detail=f"Rate limit exceeded ({RATE_LIMIT_REQUESTS}/{RATE_LIMIT_WINDOW_SECONDS}s)",
            )

        bucket.append(now)


@app.middleware("http")
async def enforce_request_size_limit(request: Request, call_next):
    if request.method == "POST" and request.url.path == "/api/agent/chat":
        content_length = request.headers.get("content-length")
        if not content_length:
            # Reject requests without Content-Length (e.g. chunked transfers)
            # to prevent unbounded body reads.
            return JSONResponse(
                status_code=status.HTTP_411_LENGTH_REQUIRED,
                content={"detail": "Content-Length header is required"},
            )

        try:
            request_size = int(content_length)
        except ValueError:
            return JSONResponse(
                status_code=status.HTTP_400_BAD_REQUEST,
                content={"detail": "Invalid Content-Length header"},
            )

        if request_size > MAX_REQUEST_BYTES:
            return JSONResponse(
                status_code=status.HTTP_413_REQUEST_ENTITY_TOO_LARGE,
                content={
                    "detail": f"Request body too large (max {MAX_REQUEST_BYTES} bytes)"
                },
            )

    return await call_next(request)


# init router and Feature Registry
router = IntentRouter()
registry = FeatureRegistry()

# Register Features
registry.register("compliance_qa", ComplianceFeature())
registry.register("payroll_verify", DemoPayrollFeature())
registry.register("roster", RosterFeature())
registry.register("roster_explain", RosterExplainFeature())



@app.get("/health")
async def health_check():
    """Health check endpoint for Docker/K8s probes"""
    return {"status": "healthy"}


@app.get("/", include_in_schema=False)
async def root():
    # Redirect root requests straight to Swagger UI for convenience
    return RedirectResponse(url="/docs")

@app.post("/api/agent/chat")
async def chat(
    request: Request,
    message: str = Form(...),
    file: Optional[UploadFile] = File(None),
    intent_hint: Optional[str] = Form(None),
    context_payload: Optional[str] = Form(None),
    history_payload: Optional[str] = Form(None),
    conversation_id: Optional[str] = Form(None),
    _: None = Depends(verify_service_key),
):
    started_at = time.perf_counter()
    request_id = _resolve_request_id(request)
    await _enforce_rate_limit(_resolve_client_id(request))

    # Step 1: use route to determine feature
    file_name = file.filename if file else None
    feature_type = router.route(message, file_name, intent_hint)

    parsed_context_payload = None
    if context_payload:
        try:
            parsed_context_payload = json.loads(context_payload)
        except json.JSONDecodeError:
            _LOGGER.warning(
                "Agent chat context_payload parse failed: request_id=%s, length=%d",
                request_id,
                len(context_payload),
            )
            parsed_context_payload = None

    parsed_history_payload = None
    if history_payload:
        try:
            parsed_history_payload = json.loads(history_payload)
        except json.JSONDecodeError:
            _LOGGER.warning(
                "Agent chat history_payload parse failed: request_id=%s, length=%d",
                request_id,
                len(history_payload),
            )
            parsed_history_payload = None

    _LOGGER.info(
        "Agent chat received: request_id=%s, intent_hint=%s, context_payload_received=%s, context_payload_parsed=%s, history_payload_received=%s, history_payload_parsed=%s, has_conversation_id=%s",
        request_id,
        intent_hint,
        context_payload is not None,
        parsed_context_payload is not None,
        history_payload is not None,
        parsed_history_payload is not None,
        bool(conversation_id),
    )
    if parsed_context_payload and isinstance(parsed_context_payload, dict):
        _LOGGER.info(
            "Agent chat context: request_id=%s, keys=%s",
            request_id,
            list(parsed_context_payload.keys()),
        )
    if parsed_history_payload and isinstance(parsed_history_payload, list):
        _LOGGER.info(
            "Agent chat history: request_id=%s, items=%s",
            request_id,
            len(parsed_history_payload),
        )

    # Step 2: get feature
    feature = registry.get_feature(feature_type)

    # Step 3: execution
    result = await feature.process({
        'message': message,
        'file_name': file_name,
        'file': file,  # Also pass the file object if provided
        'intent_hint': intent_hint,
        'context_payload': parsed_context_payload,
        'history_payload': parsed_history_payload,
        'conversation_id': conversation_id,
        'request_id': request_id,
    })

    elapsed_ms = int((time.perf_counter() - started_at) * 1000)
    _LOGGER.info(
        "Agent chat completed: request_id=%s, routed_to=%s, note=%s, elapsed_ms=%s",
        request_id,
        feature_type,
        result.get("note") if isinstance(result, dict) else None,
        elapsed_ms,
    )
    
    return {
        "status": "success",
        "request_id": request_id,
        "message": message,
        "file_name": file_name,
        "routed_to": feature_type,
        "result": result
    }


if __name__ == "__main__":
    import uvicorn
    uvicorn.run("master_agent.main:app", host="127.0.0.1", port=8000, reload=True)
