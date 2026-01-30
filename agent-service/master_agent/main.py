from fastapi import FastAPI, Form, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import RedirectResponse
from typing import Optional, Union
from master_agent.intent_router import IntentRouter
from master_agent.feature_registry import FeatureRegistry


# ⭐ ComplianceFeature (placeholder)
from master_agent.features.compliance_feature import ComplianceFeature
from master_agent.features.demo_feature import DemoPayrollFeature

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
compliance_feature = ComplianceFeature()
registry.register("compliance_qa", compliance_feature)
registry.register("compliance_roster", compliance_feature)
registry.register("payroll_verify", DemoPayrollFeature())



@app.get("/health")
async def health_check():
    """Health check endpoint for Docker/K8s probes"""
    return {"status": "healthy"}


@app.get("/", include_in_schema=False)
async def root():
    # Redirect root requests straight to Swagger UI for convenience
    return RedirectResponse(url="/docs")

@app.post("/api/agent/chat")
async def chat(
    message: str = Form(...),
    file: Union[UploadFile, str, None] = File(None)
):
    # Step 1: use route to determine feature
    upload = file if isinstance(file, UploadFile) else None
    file_name = upload.filename if upload else None
    feature_type = router.route(message, file_name)

    # Step 2: get feature
    feature = registry.get_feature(feature_type)

    # Step 3: execution
    result = await feature.process({
        'message': message,
        'file_name': file_name,
        'file': upload  # Also pass the file object if provided
    })
    
    return {
        "status": "success",
        "message": message,
        "file_name": file_name,
        "routed_to": feature_type,
        "result": result
    }
