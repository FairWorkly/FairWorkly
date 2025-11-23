from fastapi import FastAPI
from pydantic import BaseModel
from agent import generate_reply
from fastapi.responses import RedirectResponse

app = FastAPI(
    title="FairWorkly Agent v0",
    description="Minimal LangChain-based AI agent service for Compliance workflows.",
    version="0.0.1",
)

@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse(url="/docs")

class ChatRequest(BaseModel):
    message: str


class ChatResponse(BaseModel):
    reply: str

@app.get("/health")
async def health():
    """
    Simple health check endpoint.
    """
    return {"status": "ok"}


@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest) -> ChatResponse:
    """
    Single chat endpoint for Agent v0.
    Takes a user message and returns the AI response.
    """
    reply = generate_reply(req.message)
    return ChatResponse(reply=reply)


if __name__ == "__main__":
    import uvicorn
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)