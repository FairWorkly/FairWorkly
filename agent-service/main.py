from fastapi import FastAPI

from agents.compliance.router import router as compliance_router
from fastapi.responses import RedirectResponse

app = FastAPI(
    title="FairWorkly Agent v0",
    description="Minimal LangChain-based AI agent service for Compliance workflows.",
    version="0.0.1",
)

@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse(url="/docs")


@app.get("/health")
async def health():
    """Simple health check endpoint."""
    return {"status": "ok"}

app.include_router(compliance_router)


if __name__ == "__main__":
    import uvicorn
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)
