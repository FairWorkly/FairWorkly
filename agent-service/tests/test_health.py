import sys
from pathlib import Path

sys.path.append(str(Path(__file__).resolve().parents[1]))
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)


def test_health_endpoint():
    r = client.get("/health")
    assert r.status_code == 200
