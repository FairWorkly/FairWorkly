from fastapi import FastAPI, Form, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import RedirectResponse
from typing import Optional
from master_agent.intent_router import IntentRouter
from master_agent.feature_registry import FeatureRegistry


# Import features
from master_agent.features.compliance_feature import ComplianceFeature
from master_agent.features.demo_feature import DemoPayrollFeature
from master_agent.features.roster_feature import RosterFeature

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
    file: Optional[UploadFile] = File(None)
):
    # Step 1: use route to determine feature
    file_name = file.filename if file else None
    feature_type = router.route(message, file_name)

    # Step 2: get feature
    feature = registry.get_feature(feature_type)

    # Step 3: execution
    result = await feature.process({
        'message': message,
        'file_name': file_name,
        'file': file  # Also pass the file object if provided
    })
    
    return {
        "status": "success",
        "message": message,
        "file_name": file_name,
        "routed_to": feature_type,
        "result": result
    }
