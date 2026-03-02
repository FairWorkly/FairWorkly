import importlib
import json
import sys
from typing import Callable

import pytest
from fastapi.testclient import TestClient

import shared.rag.rag_client as rag_client
from shared.llm.factory import LLMProviderFactory


@pytest.fixture
def client_factory(monkeypatch) -> Callable[..., TestClient]:
    """
    Returns a factory that builds a TestClient with a stubbed LLM provider.
    """

    def _factory(
        *,
        llm_answer: str = "Stub compliance answer",
        raise_error: bool = False,
        service_key: str = "test-service-key",
        include_service_key_header: bool = True,
        rate_limit_requests: int | None = None,
        max_request_bytes: int | None = None,
    ):
        class _StubLLM:
            def __init__(self):
                self.answer = llm_answer
                self.raise_error = raise_error

            async def generate(self, messages, temperature: float = 0.7, max_tokens: int = 800):
                if self.raise_error:
                    raise RuntimeError("LLM unavailable")
                return {"content": self.answer, "model": "stub-model"}

            def count_tokens(self, text: str) -> int:
                return len(text.split())

        def _missing_vectorstore(*_args, **_kwargs):
            raise FileNotFoundError("Vector store missing")

        monkeypatch.setattr(rag_client, "LLMProvider", lambda: _StubLLM())
        monkeypatch.setattr(rag_client, "ensure_retriever", _missing_vectorstore)
        monkeypatch.setenv("AGENT_SERVICE_KEY", service_key)
        if rate_limit_requests is not None:
            monkeypatch.setenv("RATE_LIMIT_REQUESTS", str(rate_limit_requests))
        if max_request_bytes is not None:
            monkeypatch.setenv("MAX_REQUEST_BYTES", str(max_request_bytes))

        module_name = "master_agent.main"
        if module_name in sys.modules:
            main_module = importlib.reload(sys.modules[module_name])
        else:
            main_module = importlib.import_module(module_name)

        headers = {"X-Service-Key": service_key} if include_service_key_header else None
        return TestClient(main_module.app, headers=headers)

    return _factory


def test_root_redirects_to_docs(client_factory):
    client = client_factory()
    response = client.get("/", follow_redirects=False)
    assert response.status_code in (302, 307)
    assert response.headers["location"].endswith("/docs")


def test_chat_success_returns_llm_answer(client_factory):
    client = client_factory(llm_answer="Penalty rates explanation")
    response = client.post("/api/agent/chat", data={"message": "Tell me about penalty rates"})
    assert response.status_code == 200

    data = response.json()
    assert isinstance(data.get("request_id"), str)
    assert data["request_id"]
    assert data["routed_to"] == "compliance_qa"
    result = data["result"]
    assert result["type"] == "compliance"
    assert result["message"] == "Penalty rates explanation"
    assert result["model"] == "stub-model"


def test_chat_with_empty_message_returns_error_note(client_factory):
    client = client_factory()
    response = client.post("/api/agent/chat", data={"message": ""})
    assert response.status_code == 200

    result = response.json()["result"]
    assert result["note"] == "MESSAGE_REQUIRED"


def test_chat_history_payload_is_injected_into_llm_messages(client_factory, monkeypatch):
    captured_messages = []

    class _CaptureLLM:
        async def generate(self, messages, temperature: float = 0.7, max_tokens: int = 800):
            captured_messages.extend(messages)
            return {"content": "History-aware answer", "model": "capture-model"}

        def count_tokens(self, text: str) -> int:
            return len(text.split())

    client = client_factory()
    monkeypatch.setattr(rag_client, "LLMProvider", lambda: _CaptureLLM())
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "What are the next steps?",
            "history_payload": json.dumps(
                [
                    {"role": "assistant", "content": "You should review the issues first."},
                    {"role": "user", "content": "Okay, and then?"},
                ]
            ),
        },
    )
    assert response.status_code == 200
    assert response.json()["result"]["message"] == "History-aware answer"

    roles = [item["role"] for item in captured_messages]
    assert roles[0] == "system"
    assert roles[1:3] == ["assistant", "user"]
    assert roles[-1] == "user"
    assert captured_messages[-1]["content"] == "What are the next steps?"


def test_chat_history_payload_accepts_pascal_case_keys(client_factory, monkeypatch):
    captured_messages = []

    class _CaptureLLM:
        async def generate(self, messages, temperature: float = 0.7, max_tokens: int = 800):
            captured_messages.extend(messages)
            return {"content": "History-aware answer", "model": "capture-model"}

        def count_tokens(self, text: str) -> int:
            return len(text.split())

    client = client_factory()
    monkeypatch.setattr(rag_client, "LLMProvider", lambda: _CaptureLLM())
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Continue.",
            "history_payload": json.dumps(
                [
                    {"Role": "assistant", "Content": "Previous answer."},
                    {"Role": "user", "Content": "Can you expand this?"},
                ]
            ),
        },
    )
    assert response.status_code == 200
    assert response.json()["result"]["message"] == "History-aware answer"

    roles = [item["role"] for item in captured_messages]
    assert roles[1:3] == ["assistant", "user"]
    assert captured_messages[1]["content"] == "Previous answer."
    assert captured_messages[2]["content"] == "Can you expand this?"


def test_missing_message_field_returns_validation_error(client_factory):
    client = client_factory()
    response = client.post("/api/agent/chat", data={})
    assert response.status_code == 422


def test_llm_failure_returns_structured_fallback(client_factory):
    client = client_factory(raise_error=True)
    response = client.post("/api/agent/chat", data={"message": "Need help"})
    assert response.status_code == 200

    result = response.json()["result"]
    assert result["note"] == "LLM invocation failed"
    assert "I cannot provide a compliance answer right now." in result["message"]
    assert "Reason: AI provider request failed." in result["message"]
    assert "Next steps:" in result["message"]


def test_roster_intent_hint_routes_to_roster_explain(client_factory):
    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={"message": "Explain this roster result", "intent_hint": "roster"},
    )
    assert response.status_code == 200

    payload = response.json()
    assert payload["routed_to"] == "roster_explain"
    assert payload["result"]["note"] == "ROSTER_CONTEXT_REQUIRED"


def test_roster_explain_classifies_rate_limit_error(client_factory, monkeypatch):
    def _raise_rate_limit_error(*_args, **_kwargs):
        raise RuntimeError("Rate limit exceeded (429)")

    monkeypatch.setattr(LLMProviderFactory, "create", _raise_rate_limit_error)

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Why are there issues?",
            "intent_hint": "roster",
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "totalIssues": 13,
                        "criticalIssues": 9,
                        "affectedEmployees": 4,
                        "issues": [],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    result = response.json()["result"]
    assert result["type"] == "roster_explain"
    assert result["note"] == "LLM_RATE_LIMITED"
    assert "rate limit or quota" in result["message"]


def test_roster_explain_strips_markdown_headings(client_factory, monkeypatch):
    class _RosterLLM:
        async def generate(self, messages, temperature: float = 0.2, max_tokens: int = 800):
            return {
                "content": (
                    "### 1) Concise Explanation\n"
                    "Validation failed due to 13 issues.\n\n"
                    "### 2) Checks/Issues\n"
                    "- Minimum shift hours\n"
                    "- Meal break\n\n"
                    "### 3) Next steps\n"
                    "Fix short shifts first."
                ),
                "model": "stub-roster-model",
            }

    monkeypatch.setattr(LLMProviderFactory, "create", lambda *args, **kwargs: _RosterLLM())

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Explain this roster result",
            "intent_hint": "roster",
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "status": "Failed",
                        "totalShifts": 10,
                        "passedShifts": 7,
                        "failedShifts": 3,
                        "totalIssues": 13,
                        "criticalIssues": 9,
                        "affectedEmployees": 4,
                        "issues": [],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    result = response.json()["result"]
    assert result["type"] == "roster_explain"
    assert result["note"] is None
    assert "###" not in result["message"]
    assert "Concise Explanation" in result["message"]


def test_roster_explain_returns_action_plan_data(client_factory, monkeypatch):
    class _RosterLLM:
        async def generate(self, messages, temperature: float = 0.2, max_tokens: int = 800):
            return {"content": "Short explanation.", "model": "stub-roster-model"}

    monkeypatch.setattr(LLMProviderFactory, "create", lambda *args, **kwargs: _RosterLLM())

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Explain this roster result",
            "intent_hint": "roster",
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "status": "Failed",
                        "totalShifts": 17,
                        "passedShifts": 9,
                        "failedShifts": 8,
                        "totalIssues": 13,
                        "criticalIssues": 9,
                        "affectedEmployees": 4,
                        "issues": [
                            {
                                "checkType": "MealBreak",
                                "severity": "Error",
                                "employeeName": "Sarah Chen",
                                "affectedDates": "2026-02-10",
                                "description": "Meal break too short",
                            },
                            {
                                "checkType": "MealBreak",
                                "severity": "Error",
                                "employeeName": "Emma Wilson",
                                "affectedDates": "2026-02-11",
                                "description": "No meal break",
                            },
                            {
                                "checkType": "RestPeriod",
                                "severity": "Error",
                                "employeeName": "Sarah Chen",
                                "affectedDates": "2026-02-12",
                                "description": "Only 8h rest",
                            },
                        ],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    result = response.json()["result"]
    assert result["type"] == "roster_explain"
    assert "data" in result
    assert result["data"]["action_plan"]["title"] == "Top 3 actions to fix this roster"
    assert len(result["data"]["action_plan"]["actions"]) > 0
    assert len(result["data"]["action_plan"]["quick_follow_ups"]) > 0
    assert result["data"]["action_plan"]["quick_follow_ups"][0]["label"].startswith("P1 ")


def test_roster_explain_follow_up_keeps_action_plan_for_non_action_question(
    client_factory, monkeypatch
):
    class _RosterLLM:
        async def generate(self, messages, temperature: float = 0.2, max_tokens: int = 800):
            return {"content": "This is risky because rest gaps are too short.", "model": "stub-roster-model"}

    monkeypatch.setattr(LLMProviderFactory, "create", lambda *args, **kwargs: _RosterLLM())

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Why is this risky?",
            "intent_hint": "roster",
            "history_payload": json.dumps(
                [{"role": "assistant", "content": "You should prioritize rest-period breaches first."}]
            ),
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "status": "Failed",
                        "totalIssues": 2,
                        "criticalIssues": 1,
                        "affectedEmployees": 1,
                        "issues": [
                            {
                                "checkType": "RestPeriod",
                                "severity": "Error",
                                "employeeName": "Sarah Chen",
                                "affectedDates": "2026-02-12",
                                "description": "Only 8h rest",
                            }
                        ],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    result = response.json()["result"]
    assert result["type"] == "roster_explain"
    assert result["note"] is None
    assert "data" in result
    assert result["data"]["action_plan"]["title"] == "Top 3 actions to fix this roster"


def test_roster_explain_follow_up_keeps_action_plan_for_next_steps_question(
    client_factory, monkeypatch
):
    class _RosterLLM:
        async def generate(self, messages, temperature: float = 0.2, max_tokens: int = 800):
            return {"content": "Use these step-by-step fixes.", "model": "stub-roster-model"}

    monkeypatch.setattr(LLMProviderFactory, "create", lambda *args, **kwargs: _RosterLLM())

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": "Can you give next steps?",
            "intent_hint": "roster",
            "history_payload": json.dumps(
                [{"role": "assistant", "content": "There are meal break and rest period issues."}]
            ),
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "status": "Failed",
                        "totalIssues": 3,
                        "criticalIssues": 2,
                        "affectedEmployees": 2,
                        "issues": [
                            {
                                "checkType": "MealBreak",
                                "severity": "Error",
                                "employeeName": "Emma Wilson",
                                "affectedDates": "2026-02-11",
                                "description": "No meal break",
                            },
                            {
                                "checkType": "RestPeriod",
                                "severity": "Error",
                                "employeeName": "Sarah Chen",
                                "affectedDates": "2026-02-12",
                                "description": "Only 8h rest",
                            },
                        ],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    result = response.json()["result"]
    assert result["type"] == "roster_explain"
    assert "data" in result
    assert result["data"]["action_plan"]["title"] == "Top 3 actions to fix this roster"


def test_roster_explain_quick_follow_up_prompt_uses_follow_up_contract(
    client_factory, monkeypatch
):
    captured_messages = []

    class _CaptureRosterLLM:
        async def generate(self, messages, temperature: float = 0.2, max_tokens: int = 800):
            captured_messages.extend(messages)
            return {"content": "Step-by-step roster edits.", "model": "stub-roster-model"}

    monkeypatch.setattr(LLMProviderFactory, "create", lambda *args, **kwargs: _CaptureRosterLLM())

    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={
            "message": (
                "For Fix meal break scheduling, provide concrete shift-level edits using only "
                "the affected shifts. Output per employee/date: before -> after."
            ),
            "intent_hint": "roster",
            "history_payload": json.dumps(
                [{"role": "assistant", "content": "Top actions to fix this roster are shown."}]
            ),
            "context_payload": json.dumps(
                {
                    "validation": {
                        "validationId": "val-1",
                        "status": "Failed",
                        "totalIssues": 3,
                        "criticalIssues": 2,
                        "affectedEmployees": 2,
                        "issues": [
                            {
                                "checkType": "MealBreak",
                                "severity": "Error",
                                "employeeName": "Emma Wilson",
                                "affectedDates": "2026-02-11",
                                "description": "No meal break",
                            }
                        ],
                    }
                }
            ),
        },
    )

    assert response.status_code == 200
    assert captured_messages

    user_prompt = captured_messages[-1]["content"]
    assert "This is a follow-up question. Continue from prior turns." in user_prompt
    assert "Do not repeat overall validation summary" in user_prompt


def test_chat_rejects_invalid_service_key(client_factory):
    client = client_factory()
    response = client.post(
        "/api/agent/chat",
        data={"message": "Hello"},
        headers={"X-Service-Key": "wrong-key"},
    )
    assert response.status_code == 401
    assert response.json()["detail"] == "Invalid service key"


def test_chat_rate_limit_enforced(client_factory):
    client = client_factory(rate_limit_requests=1)

    first = client.post("/api/agent/chat", data={"message": "first"})
    second = client.post("/api/agent/chat", data={"message": "second"})

    assert first.status_code == 200
    assert second.status_code == 429


def test_chat_request_size_limit_enforced(client_factory):
    client = client_factory(max_request_bytes=10)
    response = client.post("/api/agent/chat", data={"message": "message-too-large"})
    assert response.status_code == 413
