import sys, os
sys.path.append(os.path.dirname(os.path.dirname(__file__)))
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)


def test_health():
    r = client.get("/health")
    assert r.status_code == 200

def test_chat_basic():
    r = client.post("/chat", json={"message": "hi"})
    assert r.status_code == 200
    assert "reply" in r.json()