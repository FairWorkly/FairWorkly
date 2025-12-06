from typing import Dict, Any
from .feature_registry import FeatureBase

class DemoComplianceFeature(FeatureBase):
    """
    Demo Compliance Feature 
    """
    
    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        message = payload.get('message', '')
        
        return {
            "type": "compliance_qa",
            "answer": f"This is the Compliance feature response: Regarding the question about '{message}'",
            "note": "This is a demo version, the real version will call LLM and RAG"
        }


class DemoPayrollFeature(FeatureBase):
    """
    Demo Payroll Feature
    """
    
    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        file_name = payload.get('file_name', 'unknown')
        
        return {
            "type": "payroll_verify",
            "result": f"Payroll文件 {file_name} verification completed",
            "issues_found": 0,
            "note": "This is a demo version"
        }