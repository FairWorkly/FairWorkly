import sys, os
from pathlib import Path

sys.path.append(str(Path(__file__).resolve().parents[2]))
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)


def test_compliance_qa_endpoint():
    r = client.post("/agents/compliance/qa", json={"question": "hi"})
    assert r.status_code == 200
    assert "reply" in r.json()
