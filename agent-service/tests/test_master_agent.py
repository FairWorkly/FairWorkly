from fastapi.testclient import TestClient

from master_agent.main import app


client = TestClient(app)


def test_root_redirects_to_docs():
    response = client.get("/", follow_redirects=False)
    assert response.status_code in (302, 307)
    assert response.headers["location"].endswith("/docs")


def test_chat_endpoint_without_file():
    payload = {"message": "Tell me about penalty rates"}
    response = client.post("/api/agent/chat", data=payload)
    assert response.status_code == 200
    data = response.json()

    assert data["status"] == "success"
    assert data["message"] == payload["message"]
    assert data["routed_to"] == "compliance_qa"

    result = data["result"]
    assert result["type"] == "compliance"
    assert "message" in result
    assert isinstance(result["message"], str)
    assert result["message"]
