# FairWorkly Agent v0

Minimal LangChain-based AI agent service to support future Compliance workflows.

## Features

- Single LLM-based chat agent (no RAG, no tools, no multi-agent)
- HTTP endpoint: `POST /chat` → returns AI response
- Ready for future enhancements (RAG, tools, workflows)

## Setup

```bash
cd agent-service
python -m venv .venv
source .venv/bin/activate  
# Windows: .venv\Scripts\activate

pip install -r requirements.txt 

```
Create a `.env` file inside `agent-service/` with the following content:
```
OPENAI_API_KEY=your_api_key_here
OPENAI_MODEL=gpt-4o-mini
MODEL_TEMPERATURE=0
```

## Run

```bash
uvicorn main:app --reload --port 8000
```

## Run Tests

To run the automated tests:

```bash
cd agent-service
pytest
```

Pytest will automatically discover tests inside the `tests/` directory. Make sure your virtual environment is activated before running the tests.


## Manual Testing

### 1. Open Swagger UI

Visit:

```
http://localhost:8000/docs
```

Swagger UI will automatically load and display the `/chat` endpoint.

### 2. Test `/chat`

Click on:

- **POST /chat**
- Click **Try it out**
- Enter JSON payload:

```json
{
  "message": "Hello, what can you do?"
}
```

- Click **Execute**  
You should see an AI-generated response under **Response body**.

