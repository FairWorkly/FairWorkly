import importlib
import sys
from typing import Callable

import pytest
from fastapi.testclient import TestClient

import agents.compliance.compliance_feature as compliance_module


@pytest.fixture
def client_factory(monkeypatch) -> Callable[..., TestClient]:
    """
    Returns a factory that builds a TestClient with a stubbed LLM provider.
    """

    def _factory(*, llm_answer: str = "Stub compliance answer", raise_error: bool = False):
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

        monkeypatch.setattr(compliance_module, "LLMProvider", lambda: _StubLLM())

        module_name = "master_agent.main"
        if module_name in sys.modules:
            main_module = importlib.reload(sys.modules[module_name])
        else:
            main_module = importlib.import_module(module_name)

        return TestClient(main_module.app)

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


def test_missing_message_field_returns_validation_error(client_factory):
    client = client_factory()
    response = client.post("/api/agent/chat", data={})
    assert response.status_code == 422


def test_llm_failure_falls_back_to_placeholder(client_factory):
    client = client_factory(raise_error=True)
    response = client.post("/api/agent/chat", data={"message": "Need help"})
    assert response.status_code == 200

    result = response.json()["result"]
    assert result["note"] == "LLM invocation failed"
    assert "Compliance Feature placeholder" in result["message"]
