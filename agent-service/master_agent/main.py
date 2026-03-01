import logging

from fastapi import FastAPI, Form, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse, RedirectResponse
from typing import Optional
from master_agent.intent_router import IntentRouter
from master_agent.feature_registry import FeatureRegistry


# Import features
from master_agent.features.compliance_feature import ComplianceFeature
from master_agent.features.demo_feature import DemoPayrollFeature
from master_agent.features.roster_feature import RosterFeature
from agents.payroll.feature import PayrollFeature
from agents.payroll.models import PayrollExplainRequest

app = FastAPI(title="FairWorkly Master Agent")

#  add CORSMiddleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# init router and Feature Registry
router = IntentRouter()
registry = FeatureRegistry()

# Register Features
registry.register("compliance_qa", ComplianceFeature())
registry.register("payroll_verify", DemoPayrollFeature())
registry.register("roster", RosterFeature())

payroll_feature = PayrollFeature()

logger = logging.getLogger(__name__)


@app.get("/health")
async def health_check():
    """Health check endpoint for Docker/K8s probes"""
    return {"status": "healthy"}


@app.get("/", include_in_schema=False)
async def root():
    # Redirect root requests straight to Swagger UI for convenience
    return RedirectResponse(url="/docs")


@app.post("/api/agent/chat")
async def chat(message: str = Form(...), file: Optional[UploadFile] = File(None)):
    # Step 1: use route to determine feature
    file_name = file.filename if file else None
    feature_type = router.route(message, file_name)

    # Step 2: get feature
    feature = registry.get_feature(feature_type)

    # Step 3: execution
    result = await feature.process(
        {
            "message": message,
            "file_name": file_name,
            "file": file,  # Also pass the file object if provided
        }
    )

    return {
        "status": "success",
        "message": message,
        "file_name": file_name,
        "routed_to": feature_type,
        "result": result,
    }


@app.post("/api/agent/payroll/explain")
async def payroll_explain(request: PayrollExplainRequest):
    try:
        result = await payroll_feature.process(request.model_dump())
        return JSONResponse(content=result, status_code=result.get("code", 500))
    except Exception:
        logger.exception("Unhandled error in payroll_explain endpoint")
        return JSONResponse(
            content={"code": 500, "msg": "Internal processing error"},
            status_code=500,
        )


if __name__ == "__main__":
    import uvicorn

    uvicorn.run("master_agent.main:app", host="127.0.0.1", port=8000, reload=True)
