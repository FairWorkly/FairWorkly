import asyncio
import os

import pytest
from fastapi.testclient import TestClient

import agents.payroll.feature as feature_module
import master_agent.main as main_module

# Header for endpoint integration tests (must match AGENT_SERVICE_KEY env var)
_AUTH_HEADERS = {"X-Service-Key": os.environ.get("AGENT_SERVICE_KEY", "")}

# ---------------------------------------------------------------------------
# Shared test payload
# ---------------------------------------------------------------------------

VALID_PAYLOAD = {
    "issueId": "test-001",
    "categoryType": "PenaltyRate",
    "employeeName": "Henry Park",
    "employeeId": "E008",
    "severity": "Error",
    "impactAmount": 31.6,
    "description": {
        "actualValue": 30.0,
        "expectedValue": 33.95,
        "affectedUnits": 8.0,
        "unitType": "Hour",
        "contextLabel": "Saturday (125% rate)",
    },
    "warning": None,
}

# ---------------------------------------------------------------------------
# Stubs
# ---------------------------------------------------------------------------


class StubRetrievalResult:
    def __init__(self):
        self.metadatas = [{"source": "AWARD.pdf", "page": 42}]
        self.documents = ["Stub document text for page 42."]


class StubRetriever:
    async def retrieve(self, query, top_k=3):
        return StubRetrievalResult()

    def format_context_for_llm(self, result):
        return "Stub award excerpt text."


def stub_ensure_retriever(config, logger):
    return StubRetriever(), None


class StubLLM:
    async def generate(self, messages, temperature=0.7, max_tokens=800):
        return {
            "content": (
                "DetailedExplanation:\n"
                "Test explanation text.\n\n"
                "Recommendation:\n"
                "1) Fix the rate.\n"
                "2) Process back-payment."
            ),
            "model": "stub-model",
        }


class BadFormatLLM:
    async def generate(self, messages, temperature=0.7, max_tokens=800):
        return {"content": "No labels here", "model": "stub"}


class FailingLLM:
    async def generate(self, messages, temperature=0.7, max_tokens=800):
        raise RuntimeError("Connection refused")


class RetryOnceLLM:
    def __init__(self):
        self.call_count = 0

    async def generate(self, messages, temperature=0.7, max_tokens=800):
        self.call_count += 1
        if self.call_count == 1:
            return {"content": "Bad format no labels", "model": "stub"}
        return {
            "content": (
                "DetailedExplanation:\n"
                "Explanation after retry.\n\n"
                "Recommendation:\n"
                "1) Fix it."
            ),
            "model": "stub-model",
        }


async def _instant_sleep(_seconds):
    pass


# ---------------------------------------------------------------------------
# Fixture: patch all external deps for feature.process() unit tests
# ---------------------------------------------------------------------------


@pytest.fixture
def patch_feature(monkeypatch):
    """Apply standard mocks for PayrollFeature unit tests.

    Returns a callable that accepts an optional LLM class override.
    """

    def _apply(llm_cls=StubLLM):
        monkeypatch.setattr(feature_module, "load_config", lambda: {})
        monkeypatch.setattr(feature_module, "ensure_retriever", stub_ensure_retriever)
        monkeypatch.setattr(feature_module, "LLMProvider", llm_cls)
        monkeypatch.setattr(feature_module, "asyncio", type("A", (), {"sleep": _instant_sleep}))

    return _apply


# ---------------------------------------------------------------------------
# Unit tests — direct feature.process() calls
# ---------------------------------------------------------------------------


def test_happy_path_returns_200(patch_feature):
    patch_feature()
    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process(VALID_PAYLOAD))

    assert result["code"] == 200
    assert result["msg"] == "OK"
    data = result["data"]
    assert data["detailedExplanation"]
    assert data["recommendation"]
    assert data["model"] == "stub-model"
    assert isinstance(data["sources"], list)


def test_missing_field_returns_400(patch_feature):
    patch_feature()
    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process({"issueId": "test-002"}))

    assert result["code"] == 400
    assert result["msg"] == "Invalid issue JSON"
    assert "data" not in result


def test_faiss_failure_returns_500(monkeypatch):
    monkeypatch.setattr(feature_module, "load_config", lambda: {})
    monkeypatch.setattr(feature_module, "LLMProvider", StubLLM)

    def broken_retriever(config, logger):
        raise RuntimeError("FAISS index not found")

    monkeypatch.setattr(feature_module, "ensure_retriever", broken_retriever)

    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process(VALID_PAYLOAD))

    assert result["code"] == 500
    assert result["msg"] == "Internal processing error"


def test_llm_bad_format_returns_502(patch_feature):
    patch_feature(llm_cls=BadFormatLLM)
    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process(VALID_PAYLOAD))

    assert result["code"] == 502
    assert result["msg"] == "LLM returned unparseable response"


def test_llm_failure_returns_503(patch_feature):
    patch_feature(llm_cls=FailingLLM)
    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process(VALID_PAYLOAD))

    assert result["code"] == 503
    assert result["msg"] == "LLM service unavailable"


def test_retry_success_on_second_attempt(monkeypatch):
    monkeypatch.setattr(feature_module, "load_config", lambda: {})
    monkeypatch.setattr(feature_module, "ensure_retriever", stub_ensure_retriever)
    monkeypatch.setattr(feature_module, "asyncio", type("A", (), {"sleep": _instant_sleep}))

    retry_llm = RetryOnceLLM()
    monkeypatch.setattr(feature_module, "LLMProvider", lambda: retry_llm)

    feature = feature_module.PayrollFeature()
    result = asyncio.run(feature.process(VALID_PAYLOAD))

    assert result["code"] == 200
    assert result["data"]["detailedExplanation"]
    assert retry_llm.call_count == 2


# ---------------------------------------------------------------------------
# Integration tests — TestClient through endpoint
# ---------------------------------------------------------------------------


def test_pydantic_422():
    client = TestClient(main_module.app)
    response = client.post(
        "/api/agent/payroll/explain",
        json={"categoryType": "PenaltyRate"},
        headers=_AUTH_HEADERS,
    )
    assert response.status_code == 422


def test_endpoint_catches_unexpected_exception(monkeypatch):
    class BrokenFeature:
        async def process(self, payload):
            raise RuntimeError("unexpected")

    monkeypatch.setattr(main_module, "payroll_feature", BrokenFeature())
    client = TestClient(main_module.app)
    response = client.post(
        "/api/agent/payroll/explain",
        json=VALID_PAYLOAD,
        headers=_AUTH_HEADERS,
    )

    assert response.status_code == 500
    assert response.json()["code"] == 500
    assert response.json()["msg"] == "Internal processing error"


def test_payroll_explain_rejects_missing_service_key():
    client = TestClient(main_module.app)
    response = client.post(
        "/api/agent/payroll/explain",
        json=VALID_PAYLOAD,
    )
    assert response.status_code == 401
