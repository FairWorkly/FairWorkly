import asyncio
from types import SimpleNamespace

import pytest

from agents.debate import feature as debate_feature_module
from agents.debate.feature import DebateFeature
from shared.llm.factory import LLMProviderFactory


def _scenario_payload() -> dict:
    return {
        "scenario": {
            "employee_name": "Alice",
            "shift_date": "2024-03-16 (Saturday)",
            "shift_hours": 10,
            "week_hours_before_shift": 38,
            "award_name": "Hospitality Industry (General) Award 2020",
            "extra_context": "Full-time employee",
        }
    }


def test_debate_requires_scenario_payload():
    try:
        asyncio.run(DebateFeature().process({}))
    except ValueError as exc:
        assert str(exc) == "Debate payload must include a 'scenario' object."
    else:
        raise AssertionError("Expected ValueError for missing scenario payload")


def test_debate_rejects_non_positive_shift_hours():
    invalid_payload = _scenario_payload()
    invalid_payload["scenario"]["shift_hours"] = 0

    with pytest.raises(
        ValueError,
        match="Invalid debate scenario payload:",
    ):
        asyncio.run(DebateFeature().process(invalid_payload))


def test_debate_returns_insufficient_evidence_when_award_retrieval_is_empty(monkeypatch):
    class _EmptyRetriever:
        async def retrieve(self, query: str, top_k: int = 3):
            return SimpleNamespace(documents=[], metadatas=[])

        def format_context_for_llm(self, retrieval) -> str:
            return "No relevant documents found."

    def _unexpected_llm_create():
        raise AssertionError("LLM should not be created when Award evidence is missing")

    monkeypatch.setattr(
        debate_feature_module,
        "ensure_retriever",
        lambda *_args, **_kwargs: (_EmptyRetriever(), None),
    )
    monkeypatch.setattr(LLMProviderFactory, "create", _unexpected_llm_create)

    result = asyncio.run(DebateFeature().process(_scenario_payload()))

    assert result["final_ruling"].startswith(
        "Insufficient retrieved Award evidence to determine the exact pay rate."
    )
    assert result["cited_award_section"] is None
    assert result["rounds"] == [
        {
            "agent": "Fair Bot",
            "role": "Compliance Arbitrator",
            "icon": "⚖️",
            "stance": "Insufficient retrieved Award evidence to determine the exact pay rate.",
            "reasoning": (
                "FairWorkly could not retrieve usable excerpts from Hospitality Industry "
                "(General) Award 2020 that directly support the required rate tiers or "
                "overtime thresholds for this scenario. The debate was stopped to avoid "
                "unsupported payroll guidance. Retrieve the relevant Award clauses and "
                "rerun the analysis."
            ),
            "challenges": None,
            "agrees_with": None,
            "sources": [],
        }
    ]


def test_debate_passes_retrieved_award_excerpts_to_roster_agent(monkeypatch):
    excerpt = (
        "Based on these Fair Work documents:\n\n"
        "[1] hospitality-award.pdf (page 12): Clause 29.3 says Saturday work is paid at 150%.\n\n"
    )
    captured_messages = []
    captured_query = {}

    class _Retriever:
        async def retrieve(self, query: str, top_k: int = 3):
            captured_query["value"] = query
            return SimpleNamespace(
                documents=["Clause 29.3 says Saturday work is paid at 150%."],
                metadatas=[{"source": "hospitality-award.pdf", "page": 12}],
            )

        def format_context_for_llm(self, retrieval) -> str:
            return excerpt

    class _StubLLM:
        async def generate(self, messages, temperature: float = 0.4, max_tokens: int = 600):
            captured_messages.append(messages)
            system_prompt = messages[0]["content"]
            if "You are the **Roster Agent**" in system_prompt:
                return {
                    "content": (
                        '{"stance":"Saturday hours are paid at 150% based on the retrieved excerpt.",'
                        '"reasoning":"The supplied Award excerpt states that Saturday work is paid at 150%. '
                        'I am limiting this initial view to the shift in isolation and not '
                        'adding any weekly overtime assumptions."}'
                    ),
                    "model": "stub-model",
                }
            if "You are the **Payroll Agent**" in system_prompt:
                return {
                    "content": (
                        '{"stance":"The retrieved excerpts support Saturday work at 150%, but weekly overtime would need explicit Award support.",'
                        '"challenges":"I agree, and additionally the rate must stay grounded in the retrieved Award text.",'
                        '"reasoning":"The supplied excerpt supports the Saturday multiplier. I would need a retrieved overtime clause before asserting any higher rate."}'
                    ),
                    "model": "stub-model",
                }
            return {
                "content": (
                    '{"ruling":"The retrieved Award excerpt supports Saturday work at 150%, and no higher rate is justified without an overtime clause.",'
                    '"agrees_with":"Payroll Agent",'
                    '"reasoning":"The payroll view is the safest because it stays within the retrieved Award text. There is no retrieved overtime clause supporting a higher multiplier.",'
                    '"cited_section":"Clause 29.3 - Saturday work"}'
                ),
                "model": "stub-model",
            }

    monkeypatch.setattr(
        debate_feature_module,
        "ensure_retriever",
        lambda *_args, **_kwargs: (_Retriever(), None),
    )
    monkeypatch.setattr(LLMProviderFactory, "create", lambda: _StubLLM())

    result = asyncio.run(DebateFeature().process(_scenario_payload()))

    roster_system_prompt = captured_messages[0][0]["content"]
    assert excerpt in roster_system_prompt
    assert "Use ONLY the supplied Award excerpts as support for your answer." in roster_system_prompt
    assert "2024-03-16 (Saturday)" in captured_query["value"]
    assert result["rounds"][0]["sources"] == [{"source": "hospitality-award.pdf", "page": 12}]
    assert result["rounds"][2]["challenges"] is None
    assert result["rounds"][2]["agrees_with"] == "Payroll Agent"
    assert result["model"] == "stub-model"


def test_debate_rejects_invalid_structured_agent_response(monkeypatch):
    class _Retriever:
        async def retrieve(self, query: str, top_k: int = 3):
            return SimpleNamespace(
                documents=["Clause 29.3 says Saturday work is paid at 150%."],
                metadatas=[{"source": "hospitality-award.pdf", "page": 12}],
            )

        def format_context_for_llm(self, retrieval) -> str:
            return "Clause 29.3 says Saturday work is paid at 150%."

    class _StubLLM:
        async def generate(self, messages, temperature: float = 0.4, max_tokens: int = 600):
            return {
                "content": "STANCE: Saturday work might be 150%",
                "model": "stub-model",
            }

    monkeypatch.setattr(
        debate_feature_module,
        "ensure_retriever",
        lambda *_args, **_kwargs: (_Retriever(), None),
    )
    monkeypatch.setattr(LLMProviderFactory, "create", lambda: _StubLLM())

    with pytest.raises(
        ValueError,
        match="Roster Agent returned an invalid structured response.",
    ):
        asyncio.run(DebateFeature().process(_scenario_payload()))


def test_debate_preserves_llm_provider_error_detail(monkeypatch):
    class _Retriever:
        async def retrieve(self, query: str, top_k: int = 3):
            return SimpleNamespace(
                documents=["Clause 29.3 says Saturday work is paid at 150%."],
                metadatas=[{"source": "hospitality-award.pdf", "page": 12}],
            )

        def format_context_for_llm(self, retrieval) -> str:
            return "Clause 29.3 says Saturday work is paid at 150%."

    class _StubLLM:
        async def generate(self, messages, temperature: float = 0.4, max_tokens: int = 600):
            raise RuntimeError("context length exceeded for this model")

    monkeypatch.setattr(
        debate_feature_module,
        "ensure_retriever",
        lambda *_args, **_kwargs: (_Retriever(), None),
    )
    monkeypatch.setattr(LLMProviderFactory, "create", lambda: _StubLLM())

    with pytest.raises(
        ValueError,
        match="Roster Agent failed while generating the debate response: context length exceeded for this model",
    ):
        asyncio.run(DebateFeature().process(_scenario_payload()))


def test_debate_rejects_structured_response_with_blank_fields_or_extra_keys(monkeypatch):
    class _Retriever:
        async def retrieve(self, query: str, top_k: int = 3):
            return SimpleNamespace(
                documents=["Clause 29.3 says Saturday work is paid at 150%."],
                metadatas=[{"source": "hospitality-award.pdf", "page": 12}],
            )

        def format_context_for_llm(self, retrieval) -> str:
            return "Clause 29.3 says Saturday work is paid at 150%."

    class _StubLLM:
        async def generate(self, messages, temperature: float = 0.4, max_tokens: int = 600):
            system_prompt = messages[0]["content"]
            if "You are the **Roster Agent**" in system_prompt:
                return {
                    "content": (
                        '{"stance":"Saturday hours are paid at 150% based on the retrieved excerpt.",'
                        '"reasoning":"The supplied Award excerpt states that Saturday work is paid at 150%."}'
                    ),
                    "model": "stub-model",
                }
            if "You are the **Payroll Agent**" in system_prompt:
                return {
                    "content": (
                        '{"stance":"The retrieved excerpts support Saturday work at 150%.",'
                        '"challenges":"I agree, and additionally the rate must stay grounded in the retrieved Award text.",'
                        '"reasoning":"The supplied excerpt supports the Saturday multiplier."}'
                    ),
                    "model": "stub-model",
                }
            return {
                "content": (
                    '{"ruling":" ","agrees_with":"Payroll Agent",'
                    '"reasoning":"",'
                    '"cited_section":"Clause 29.3 - Saturday work",'
                    '"extra":"drift"}'
                ),
                "model": "stub-model",
            }

    monkeypatch.setattr(
        debate_feature_module,
        "ensure_retriever",
        lambda *_args, **_kwargs: (_Retriever(), None),
    )
    monkeypatch.setattr(LLMProviderFactory, "create", lambda: _StubLLM())

    with pytest.raises(
        ValueError,
        match="Fair Bot returned an invalid structured response.",
    ):
        asyncio.run(DebateFeature().process(_scenario_payload()))
