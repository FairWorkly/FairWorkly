from dotenv import load_dotenv
from fastapi import FastAPI
from pydantic import BaseModel
from agent import generate_reply

load_dotenv()

app = FastAPI(
    title="FairWorkly Agent v0",
    description="Minimal LangChain-based AI agent service for Compliance workflows.",
    version="0.0.1",
)


class ChatRequest(BaseModel):
    message: str


class ChatResponse(BaseModel):
    reply: str


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