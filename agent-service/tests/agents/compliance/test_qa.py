import json
import sys
from pathlib import Path
from unittest.mock import patch

sys.path.append(str(Path(__file__).resolve().parents[3]))
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

MOCK_RESPONSE = {
    "question_id": "q-123",
    "plain_explanation": "Mock summary of the answer.",
    "key_points": [
        "First key point",
        "Second key point",
    ],
    "risk_level": "yellow",
    "what_to_do_next": [
        "First action",
        "Second action",
    ],
    "legal_references": [
        {
            "source": "NES",
            "title": "NES overview",
            "url": "https://www.fairwork.gov.au/example",
            "note": "Example reference",
        }
    ],
    "disclaimer": "This is general guidance only. Consult Fair Work or an employment lawyer for advice.",
}


def test_compliance_qa_endpoint():
    with patch(
        "agents.compliance.features.ask_ai_question.handler.generate_reply",
        return_value=json.dumps(MOCK_RESPONSE),
    ):
        r = client.post(
            "/agents/compliance/qa",
            json={"question": "hi", "question_id": "q-123"},
        )

    assert r.status_code == 200
    assert r.json() == MOCK_RESPONSE


def test_compliance_qa_requires_question():
    r = client.post("/agents/compliance/qa", json={"question_id": "q-1"})

    assert r.status_code == 422
    assert any(
        detail["loc"][-1] == "question" for detail in r.json()["detail"]
    )


def test_compliance_qa_rejects_invalid_audience():
    payload = {
        "question_id": "q-1",
        "question": "What is the minimum notice period?",
        "audience": "contractor",
    }
    r = client.post("/agents/compliance/qa", json=payload)

    assert r.status_code == 422
