import asyncio
import logging

import pytest

import agents.shared.rag.rag_client as rag_client
from agents.compliance.compliance_feature import ComplianceFeature


def test_empty_message_short_circuits():
    feature = ComplianceFeature()
    result = asyncio.run(feature.process({"message": " ", "file_name": "x"}))
    assert result["type"] == "compliance"
    assert result["note"] == "MESSAGE_REQUIRED"


def test_missing_vectorstore_still_uses_llm(monkeypatch):
    class _StubLLM:
        async def generate(self, messages, temperature: float = 0.7, max_tokens: int = 800):
            return {"content": "Stub answer", "model": "stub-model"}

        def count_tokens(self, text: str) -> int:
            return len(text.split())

    def _missing_vectorstore(*_args, **_kwargs):
        raise FileNotFoundError("Vector store missing")

    monkeypatch.setattr(rag_client, "LLMProvider", lambda: _StubLLM())
    monkeypatch.setattr(rag_client, "ensure_retriever", _missing_vectorstore)

    result = asyncio.run(
        rag_client.run(
            "Hello",
            system_prompt="sys",
            config={},
            logger=logging.getLogger("test"),
        )
    )

    assert result["content"] == "Stub answer"
    assert result["sources"] == []
    assert result.get("note") is None


def test_llm_unavailable_returns_placeholder(monkeypatch):
    def _llm_unavailable():
        raise RuntimeError("LLM unavailable")

    def _missing_vectorstore(*_args, **_kwargs):
        raise FileNotFoundError("Vector store missing")

    monkeypatch.setattr(rag_client, "LLMProvider", _llm_unavailable)
    monkeypatch.setattr(rag_client, "ensure_retriever", _missing_vectorstore)

    result = asyncio.run(
        rag_client.run(
            "Need help",
            system_prompt="sys",
            config={},
            logger=logging.getLogger("test"),
        )
    )

    assert result["note"] == "LLM provider not configured"
    assert "Compliance Feature placeholder" in result["content"]
