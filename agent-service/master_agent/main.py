from fastapi import FastAPI, Form, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
from typing import Optional
from .intent_router import IntentRouter
from .feature_registry import FeatureRegistry


# ⭐ ComplianceFeature（placeholder）
from agents.compliance.compliance_feature import ComplianceFeature
from .demo_feature import DemoPayrollFeature

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
compliance_feature = ComplianceFeature
registry.register("compliance_qa", compliance_feature())
registry.register("compliance_roster", compliance_feature())
registry.register("payroll_verify", DemoPayrollFeature())



@app.get("/")
async def root():
    return {
        "message": "Master Agent is running",
        "version": "1.0.0",
        "registered_features": registry.list_features()
        }

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
        'file': file # Also pass the file object
    })
    
    return {
        "status": "success",
        "message": message,
        "file_name": file_name,
        "routed_to": feature_type,
        "result": result
    }
