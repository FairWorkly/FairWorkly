import asyncio
import json
import sys
from pathlib import Path

import pytest
from fastapi import HTTPException, status

sys.path.append(str(Path(__file__).resolve().parents[3]))

from agents.compliance.features.ask_ai_question.handler import (
    handle_ask_ai_question,
)
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
)


def test_handle_ask_ai_question_success(monkeypatch):
    def fake_generate_reply(prompt, question, response_format=None):
        payload = {
            "question_id": "q-1",
            "plain_explanation": "Explanation",
            "key_points": ["Point A"],
            "risk_level": "green",
            "what_to_do_next": ["Action"],
            "legal_references": [],
            "disclaimer": "Disclaimer",
        }
        return json.dumps(payload)

    monkeypatch.setattr(
        "agents.compliance.features.ask_ai_question.handler.generate_reply",
        fake_generate_reply,
    )

    req = AskAiQuestionRequest(question_id="q-1", question="Hello?")
    resp = asyncio.run(handle_ask_ai_question(req))
    assert resp.question_id == "q-1"
    assert resp.risk_level == "green"


def test_handle_ask_ai_question_invalid_json(monkeypatch):
    monkeypatch.setattr(
        "agents.compliance.features.ask_ai_question.handler.generate_reply",
        lambda *args, **kwargs: "not-json",
    )

    req = AskAiQuestionRequest(question_id="q-2", question="Bad reply?")

    with pytest.raises(HTTPException) as excinfo:
        asyncio.run(handle_ask_ai_question(req))

    assert excinfo.value.status_code == status.HTTP_502_BAD_GATEWAY
