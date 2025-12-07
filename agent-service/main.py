from fastapi import FastAPI
from fastapi.responses import RedirectResponse

app = FastAPI(
    title="FairWorkly Agent v0",
    description="Minimal LangChain-based AI agent service for Compliance workflows.",
    version="0.0.1",
)
if __name__ == "__main__":
    import uvicorn
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)
